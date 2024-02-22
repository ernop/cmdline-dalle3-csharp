using OpenAI_API.Images;
using OpenAI_API.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dalle3
{
    internal static class Statics
    {
        public static Logger Logger = new Logger("../../logs/log.txt");
        public static string ApiKey { get; set; } = System.IO.File.ReadAllText("../../apikey.txt");
        public static string OrgId { get; set; } = System.IO.File.ReadAllText("../../organization.txt");
        public static Random Random = new Random();

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
            Console.WriteLine("\r\nFormat------ 'Dalle3.exe [-N] [-r] [-h|v] [-hd] [prompt]'" +
                "\r\nExample-------- 'Dalle3.exe A very {big,blue,tall} photo of a [tall,small] [1-2,cat,dog,mouse monster] i the stylue of {GPTArtstyles}'" +
                "\r\n\r\nExplanation of terms:" +
                "\r\n\tN = \t\t\tnumber of times to repeat prompt. Will die if any fail. Prompt can be multiple words with no quotes required, but no newlines." +
                "\r\n\t-r =\t\t\toutput items in random order. default or missing, will output in permutation order. This applies to both permutations and powersets. So without -r you will iterate through all subsets in order." +
                "\r\n\t-h|v =\t\t\tmake image horizontal or vertical. the default is square." +
                "\r\n\t-hd = \t\t\tmake image in hd. The default is standard, and is cheaper." +
                "\r\n\t{X,Y,Z,...} =\t\tpick one of these and run with it, just one." +
                "\r\n\t[X,Y,Z,...] = \t\tpowerset operator. two ways to use it: either the first item is in the form A-B where it will pick between A and B items from the list," +
                "\r\n\t{GPTArtstyles} =\tthis will pick one of the ~450 artstyles hardcoded into the program. There are a ton of them from all over the world. The program has LOTS of aliases built in" +
                "\r\nfor all kinds of different things. These are useful for forcing the program to get out of the normal vectors. But there are SO many ways to go, I'm still finding out more and more." +
                "\r\n\tPrompt = \t\tYour text input from the command line. Or, you can edit the file OverridePrompt and run it that way so you can debug, step through etc." +
                "\r\nor if you omit that, like in [tall,small], it will pick a random element of the powerset. reminder: powerset means ALL subsets, so everything from none of the items, to 1, to 2, ... to all of them." +
                "\r\nNote that for powersets that is a LOT of images. 2^N where N is the number of items in the powerset. Also this is broken right now..." +
                "\r\n by default outputs normal and annotated versions of images. If you want no normal do '-nonormal', if you want no annotated do '-noann'");
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

            var numre = new Regex(@"\-([\d]{1,3})");
            var count = 0;
            var usingRawPrompt = "";
            foreach (string s in args)
            {
                if (string.IsNullOrEmpty(s)) { continue; }
                if (s == "/help" || s == "/h" || s == "/?" || s == "-help" || s == "--help" || s == "-ayuda" || s == "--h")
                {
                    Console.WriteLine($"You triggered the help display by typing: {s}, so printing it and quitting.");
                    Usage();
                    Environment.Exit(0);
                }
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

                if (s.Length > 1 && s[0] == '-' && s[1] != '-') //fallthrough, although for sanity we should at least allow bare - since thats probably just part of the prompt.
                {
                    Statics.Logger.Log($"Unknown option: {s}");
                    Usage();
                    return null;
                }

                if (string.IsNullOrWhiteSpace(s))
                {
                    usingRawPrompt += s;
                }
                else
                {
                    usingRawPrompt += " " + s;
                }

            }

            if (!optionsModel.IncludeNormalImageOutput && !optionsModel.IncludeAnnotatedImageOutput)
            {
                Statics.Logger.Log("You have disabled both normal and annotated image output. There is nothing to do, so quitting.");
                Usage();
                System.Environment.Exit(0);
            }

            //okay, now we have a raw prompt with sections like <text> {text which should be made into an alias}, etc.

            //okay, I don't want to use a syntax parser and all that here. I'll just use some magical stuff
            //to switch this out in a safe section where at the end I"m guaranteed to have all significant "segments".
            //a segment now is either "{...}" (a permuatation sectrion) or "...".  If later
            //I want to add a powerset section such as [], I can use some magic to do that. The point here is that:
            //the first char of the chunk is the pointer for who should parse it into an IPromptSection, and we remove the last char.

            optionsModel.PromptSections = Parser.ParseInput(usingRawPrompt);
            var max = 1;
            if (!optionsModel.Random)
            {
                foreach (var ps in optionsModel.PromptSections)
                {
                    max = Math.Max(max, ps.GetCount());
                    var n = Math.Min(10, max);
                    if (n > optionsModel.ImageNumber)
                    {
                        Statics.Logger.Log($"Set {optionsModel.ImageNumber} to generate {n} images");
                        optionsModel.ImageNumber = n;
                    }
                }
            }
            return optionsModel;
        }

        public static string PromptToDestFpWithReservation(ImageGenerationRequest req, string humanReadable, int taskNumber)
        {
            var now = DateTime.Now;
            string usePrompt = Regex.Replace(humanReadable, "[^a-zA-Z0-9]", "_");
            while (usePrompt.Contains("__"))
            {
                usePrompt = usePrompt.Replace("__", "_");
            }

            if (usePrompt.Length > 180)
            {
                usePrompt = usePrompt.Substring(0, 60);
            }

            usePrompt = usePrompt + "_" + taskNumber.ToString();
            string align;

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
            var outFn = $"{usePrompt.Trim().TrimEnd('_')}-{now.Year}{now.Month:00}{now.Day:00}-{req.Quality}-{align}.png";

            var tries = 0;
            while (true)
            {
                var fp = $"d:/proj/dalle3/output/{outFn.Replace(".png", $"_{tries}.png")}";
                if (!System.IO.File.Exists(fp))
                {
                    //touch a file there.
                    System.IO.File.Create(fp).Close();
                    return fp;
                }
                tries++;
                if (tries > 1000)
                {
                    throw new Exception("Not able to find a place to store the file.");
                }
            }
        }

        public static InternalTextSection PickRandomPowersetValue(IEnumerable<InternalTextSection> items, int minToReturn, int maxToReturn)
        {
            var actualNumberOfValuesToReturn = Random.Next(minToReturn, maxToReturn + 1);
            var res = GetNthPowersetValue(items, actualNumberOfValuesToReturn);
            return res;
        }

        public static InternalTextSection GetNthPowersetValue(IEnumerable<InternalTextSection> items, int actualNumberOfValuesToReturn)
        {
            var subset = new List<InternalTextSection>();
            var indices = new List<int>();
            for (int jj = 0; jj < items.Count(); jj++)
            {
                indices.Add(jj);
            }

            Statics.Shuffle(indices);

            for (int ii = 0; ii < actualNumberOfValuesToReturn; ii++)
            {
                subset.Add(items.Skip(indices[ii]).First());
            }

            var joined = string.Join(",", subset.Select(el => el.L));
            var its = new InternalTextSection(joined, joined, false, null);
            return its;
        }

        /// <summary>
        /// You survive... for now.
        /// </summary>
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

        public static void UpdateWithFilterResult(IEnumerable<InternalTextSection> ungroupedSections, TextChoiceResultEnum el)
        {
            ///So for example a powerset of [a,b,c] might send (a,c) here.
            foreach (var section in ungroupedSections)
            {
                if (section.Parent == null)
                {
                    continue;
                }
                section.Parent.ReceiveChoiceResult(section, el); ;
            }
        }
    }
}
