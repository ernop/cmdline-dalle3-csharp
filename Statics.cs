using OpenAI_API.Images;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dalle3
{
    internal static class Statics
    {
        public static string ApiKey { get; set; } = System.IO.File.ReadAllText("../../apikey.txt");
        public static string OrgId { get; set; } = System.IO.File.ReadAllText("../../organization.txt");

        public static void Usage()
        {
            Console.WriteLine("Dalle3.exe [--N] [prompt]\r\ndalle3.exe A very {big,blue,tall} photo of a {tall,small} {cat,dog,mouse monster}\r\nN=number of times to repeat prompt. Will die if any fail. Prompt can be multiple words with no quotes required, but no newlines.\r\n{}=run all permutations of the items within here. This can blow up your api limits.");
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

            var numre = new Regex(@"\-\-([\d]{1,2})");
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

                if (s == "-r")
                {
                    optionsModel.Random = true;
                    continue;
                }

                optionsModel.RawPrompt += " " + s;
            }
            optionsModel.RawPrompt = optionsModel.RawPrompt.Trim();

            if (string.IsNullOrEmpty(optionsModel.RawPrompt)) { return null; }
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
                usePrompt = usePrompt.Substring(0, 100);
            }
            var outfn = $"{usePrompt.Trim()}-{now.Year}{now.Month:00}{now.Day:00}-{req.Size}-{tries}.png";
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
                foreach (var part in parts)
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
