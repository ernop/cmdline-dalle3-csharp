using OpenAI_API.Images;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static Dalle3.Substitutions;

namespace Dalle3
{
    internal static class Statics
    {
        public static string ApiKey { get; set; } = System.IO.File.ReadAllText("../../apikey.txt");
        public static string OrgId { get; set; } = System.IO.File.ReadAllText("../../organization.txt");

        public static void Usage()
        {
            Console.WriteLine("Dalle3.exe [-N] [-r] [-h|v] [-hd] [prompt]\r\ndalle3.exe A very {big,blue,tall} photo of a {tall,small} {cat,dog,mouse monster}\r\nN=number of times to repeat prompt. Will die if any fail. Prompt can be multiple words with no quotes required, but no newlines." +
                "\r\n{}=run all permutations of the items within here. This can blow up your api limits." +
                "\r\n-r output items in random order. default or missing, will output in permutation order." +
                "\r\n-h|v make image horizontal or vertical. the default is square." +
                "\r\n-hd make image in hd. The default is standard, and is cheaper.");
        }

        public static void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static OptionsModel Parse(string[] args)
        {
            var optionsModel = new OptionsModel();
            //default 
            optionsModel.ImageNumber = 1;
            optionsModel.Size = ImageSize._1024;
            optionsModel.Quality = "standard";

            var numre = new Regex(@"\-([\d]{1,2})");
            var count = 0;
            foreach (string s in args)
            {
                if (count == 0)
                {
                    var t = numre.Match(s);
                    if (t.Success)
                    {
                        count = int.Parse(t.Groups[1].Value);
                        optionsModel.ImageNumber = count;
                        continue;
                    }
                }
                if (s == "-hd")
                {
                    optionsModel.Quality = "hd";
                    continue;
                }
                if (s == "-h")
                {
                    optionsModel.Size = ImageSize._1792x1024;
                    continue;
                }
                if (s == "/help" || s == "/h" || s == "/?" || s == "-help" || s == "--help" || s == "-ayuda" || s == "--h")
                {
                    Usage();
                    Environment.Exit(0);
                }
                if (s == "-v")
                {
                    optionsModel.Size = ImageSize._1024x1792;
                    continue;
                }
                if (s == "-r")
                {
                    optionsModel.Random = true;
                    continue;
                }

                optionsModel.RawPrompt += " " + s;
            }
            optionsModel.RawPrompt = optionsModel.RawPrompt.Trim();

            if (string.IsNullOrEmpty(optionsModel.RawPrompt)) { return null; }

            var awesomes = new Blowup("{AwesomeStyles}", "{Dungeons and Dragons, Pathfinder, Lamentations of the Flame Princess, " +
                "Dungeon Crawl Classics, Fantasy AGE, Warhammer Fantasy, " +
                "Palladium Fantasy, G.U.R.P.S, Basic Fantasy, Low Fantasy Gaming, " +
                "Vagabond, Tales of the Valiant, Cypher System, Savage Worlds, " +
                "RuneQuest, Ars Magica, Iron Kingdoms, Torchbearer, The One Ring, " +
                "Burning Wheel, Legend of the Five Rings, Fate, 13th Age, " +
                "Adventurer Conqueror King System, Forbidden Lands, Conan, OSR, " +
                "Fighting Fantasy, Tunnels and Trolls, Monsters Monsters, TTRPG, " +
                "EZD6, Index Card RPG, Dungeons of Drakkenheim}"
            );
            var gptStyles = new Blowup("{GPTStyles}", "" +
                "{" +
                //"The image features a dynamic composition with swirling lines and bright colors typical of the Expressionist movement evoking a sense of emotional turmoil, " +
                "A serene landscape painting in the Impressionist style, " +
                "A stark minimalist composition, " +
                "A vibrant pop art piece, " +
                "A digital artwork, " +
                "The charcoal drawing, " +
                "A Baroque-era oil painting, " +
                "a cubist collage using geometric shapes, " +
                "An Art Nouveau illustration, " +
                "A traditional Japanese woodblock print" +
                "A whimsical watercolor illustration, " +
                "A dynamic abstract expressionist canvas, " +
                "A detailed Renaissance fresco, " +
                "A Gothic tapestry rich with allegory, " +
                //"A bold graffiti mural in a street art style, " +
                "A photorealistic graphite sketch, " +
                "A Rococo pastel portrait, " +
                "A contemplative Zen ink wash painting, " +
                "A post-impressionist scene with vivid brushstrokes, " +
                "A modernist sculpture " +
                "}");
            var blowups = new List<Blowup>() { awesomes, gptStyles };
            foreach (var b in blowups)
            {
                var key = b.Short;
                if (optionsModel.RawPrompt.IndexOf(key) >= 0)
                {
                    optionsModel.RawPrompt = Statics.ReplaceOnce(optionsModel.RawPrompt, key, b.Long);
                    Console.WriteLine($"Blew up prompt to: {optionsModel.RawPrompt}");
                }
            }

            optionsModel.EffectivePrompts = PermutationExpander.ExpandCurlyItems(optionsModel.RawPrompt);
            if (optionsModel.Random)
            {
                Statics.Shuffle(optionsModel.EffectivePrompts);
            }
            return optionsModel;
        }

        public static string PromptToFilename(ImageGenerationRequest req, int tries)
        {
            var now = DateTime.Now;
            var usePrompt = req.Prompt.Replace("AAA", "");
            usePrompt = Regex.Replace(usePrompt, "[^a-zA-Z0-9]", "_");
            while (usePrompt.Contains("__"))
            {
                usePrompt = usePrompt.Replace("__", "_");
            }

            if (usePrompt.Length > 100)
            {
                usePrompt = usePrompt.Substring(0, 130);
            }
            var align = "";
            //var w = new ImageSize("1792x1024");
            var dumbSize = req.Size.ToString();
            switch (dumbSize)
            {
                case "1024x1024":
                    align = "sq";
                    break;
                case "1792x1024":
                    align = "h";
                    break;
                case "1024x1792":
                    align = "v";
                    break;
                default:
                    align = "def";
                    break;
            }
            Console.WriteLine(dumbSize);
            var outfn = $"{usePrompt.Trim().TrimEnd('_')}-{now.Year}{now.Month:00}{now.Day:00}-{req.Quality}-{align}-{tries}.png";
            return outfn;
        }

        public static class PermutationExpander
        {
            /// <summary>
            /// Expands a string with permutations defined in curly braces.
            /// Example: "A {red, yellow, purple} cat {and dog,and man}" => 
            /// ["A red cat and dog", "A yellow cat and dog", "A purple cat and dog", 
            /// "A red cat and man", "A yellow cat and man", "A purple cat and man"]
            /// </summary>
            public static List<string> ExpandCurlyItems(string input)
            {
                var results = new List<string>();
                var match = Regex.Match(input, @"\{([^{}]*)\}");

                if (!match.Success)
                {
                    results.Add(input);
                    return results;
                }

                var chunk = match.Groups[1].Value;
                var parts = chunk.Split(',').Select(el => el.Trim());
                var target = $"{{{chunk}}}";
                foreach (
                    var part in parts)
                {
                    var part2 = "AAA" + part + "AAA";
                    var replaced = ReplaceOnce(input, target, part2);
                    var subMatches = ExpandCurlyItems(replaced);
                    results.AddRange(subMatches);
                }
                return results;
            }
        }

        public static string ReplaceOnce(string input, string target, string part)
        {
            var ii = input.IndexOf(target);
            if (ii == -1)
            {
                return "";
            }
            var res = input.Substring(0, ii);
            res += part;
            res += input.Substring(ii + target.Length, input.Length - ii - target.Length);
            return res;
        }
    }
}
