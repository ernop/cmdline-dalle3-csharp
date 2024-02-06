using OpenAI_API.Images;
using OpenAI_API.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static Dalle3.Substitutions;

namespace Dalle3
{
    internal static class Statics
    {
        public static Logger Logger = new Logger("../../logs/log.txt");
        public static string ApiKey { get; set; } = System.IO.File.ReadAllText("../../apikey.txt");
        public static string OrgId { get; set; } = System.IO.File.ReadAllText("../../organization.txt");

        /// <summary>
        /// Configure your tier yourself! visit this page: https://platform.openai.com/docs/guides/rate-limits/usage-tiers while logged in
        /// and figure out what tier you are. This tier (based on your total historical payments to OpenAI) controls your rate limits.
        /// This data is current as of 2/2024 but will probably change.
        /// TODO obviously, put this into a config file so its easier to manage.
        /// </summary>
        public static int MyOpenAiTier { get; set; } = 4;

        public static int GetMyImagesPerMinuteRateLimit()
        {
            switch (MyOpenAiTier)
            {
                case 0:
                    return 1;
                case 1:
                    return 7;
                case 2:
                    return 7;
                case 3:
                    return 7;
                case 4:
                    return 15;
                case 5:
                    return 50;
                default:
                    return 0;
            }
        }

        public static void Usage()
        {
            Console.WriteLine("Dalle3.exe [-N] [-r] [-h|v] [-hd] [prompt]\r\ndalle3.exe A very {big,blue,tall} photo of a {tall,small} {cat,dog,mouse monster}\r\nN=number of times to repeat prompt. Will die if any fail. Prompt can be multiple words with no quotes required, but no newlines." +
                "\r\n{}=run all permutations of the items within here. This can blow up your api limits." +
                "\r\n-r output items in random order. default or missing, will output in permutation order." +
                "\r\n by default outputs normal and annotated versions of images. If you want no normal do '-nonormal', if you want no annotated do '-noann'" +
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

        /// <summary>
        /// This only looks for command line options
        /// Everything else left over will be used as the prompt including expanding permutations, etc.
        /// </summary>
        public static OptionsModel Parse(string[] args)
        {
            var optionsModel = new OptionsModel();
            //default 
            optionsModel.ImageNumber = 1;
            optionsModel.Size = ImageSize._1024;
            optionsModel.Quality = "standard";
            optionsModel.IncludeNormalImageOutput = true;
            optionsModel.IncludeAnnotatedImageOutput = true;

            var numre = new Regex(@"\-([\d]{1,2})");
            var count = 0;
            foreach (string s in args)
            {
                if (string.IsNullOrEmpty(s)) { continue; }
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
                    Console.WriteLine($"You triggered the help display by typing: {s}, so printing it and quitting.");
                    Usage();
                    Environment.Exit(0);
                }
                if (s == "-noann")
                {
                    optionsModel.IncludeAnnotatedImageOutput = false;
                }
                if (s == "-nonormal")
                {
                    optionsModel.IncludeNormalImageOutput = false;
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

                if (s[0] != '-' && s[0] == '-') //fallthrough, although for sanity we should at least allow bare - since thats probably just part of the prompt.
                {
                    Statics.Logger.Log($"Unknown option: {s}");
                    Usage();
                    return null;
                }

                optionsModel.RawPrompt += " " + s;
            }

            if (!optionsModel.IncludeNormalImageOutput && !optionsModel.IncludeAnnotatedImageOutput)
            {
                Statics.Logger.Log("You have disabled both normal and annotated image output. There is nothing to do, so quitting.");
                Usage();
                System.Environment.Exit(0);
            }
            optionsModel.RawPrompt = optionsModel.RawPrompt.Trim();

            if (string.IsNullOrEmpty(optionsModel.RawPrompt)) { return null; }

            var worked = false;
            var blowups = Blowups.GetBlowups();
            while (true)
            {
                worked = false;
                foreach (var b in blowups)
                {
                    var key = b.Short;
                    if (optionsModel.RawPrompt.IndexOf(key) >= 0)
                    {
                        optionsModel.RawPrompt = Statics.ReplaceOnce(optionsModel.RawPrompt, key, b.Long);
                        Statics.Logger.Log($"Blew up prompt to: {optionsModel.RawPrompt}");
                        worked = true;
                    }
                }
                if (!worked) { break; }
            }

            //randomThing

            optionsModel.EffectivePrompts = PermutationExpander.ExpandCurlyItems(optionsModel.RawPrompt);
            if (optionsModel.Random)
            {
                Statics.Shuffle(optionsModel.EffectivePrompts);
            }
            return optionsModel;
        }

        public static string PromptToFilename(ImageGenerationRequest req)
        {
            var now = DateTime.Now;
            var usePrompt = req.Prompt.Replace("AAA", "");
            usePrompt = Regex.Replace(usePrompt, "[^a-zA-Z0-9]", "_");
            while (usePrompt.Contains("__"))
            {
                usePrompt = usePrompt.Replace("__", "_");
            }

            if (usePrompt.Length > 130)
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
            var useQuality = req.Quality ?? "standard";
            var outfn = $"{usePrompt.Trim().TrimEnd('_')}-{now.Year}{now.Month:00}{now.Day:00}-{useQuality}-{align}.png";
            return outfn;
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
