﻿using OpenAI_API.Images;
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
        public static Random Random = new Random(1000);

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

                usingRawPrompt += " " + s;
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
            var useQuality = req.Quality ?? "standard";
            var outFn = $"{usePrompt.Trim().TrimEnd('_')}-{now.Year}{now.Month:00}{now.Day:00}-{useQuality}-{align}.png";

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

        public static IEnumerable<InternalTextSection> PickRandomPowersetValue(IEnumerable<InternalTextSection> items, int min, int max)
        {
            //rather than messing around with the pure iterator, better just make a 
            //bit vector into it which is better.
            int num = 0;
            if (min == 0 && max == int.MaxValue)
            {
                num = Random.Next(0, 1 << items.Count());
            }
            else
            {
                //okay from the range how many should we get.
                //just make it linear even though that doesn't represent the actual probabilities inside.
                var actualCount = min + Random.Next(max - min);

                //put the index of every item in the original list (items)
                var bitIndex = new List<short>();
                for (short ii = 0; ii < items.Count(); ii++)
                {
                    bitIndex.Add(ii);
                }

                Statics.Shuffle(bitIndex);
                //Console.WriteLine($"Trying to return an int with: {actualCount} bits");
                foreach (var index in bitIndex.Take(actualCount))
                {
                    num |= 1 << index;
                    //Console.WriteLine($"{Convert.ToString(num, 2)}/{Convert.ToString(1<<items.Count(), 2)}");
                }
            }

            var raw = IteratePowerSet(items, num, true);
            var el = raw.First();
            return el;
        }

        public static IEnumerable<IEnumerable<InternalTextSection>> IteratePowerSet(IEnumerable<InternalTextSection> items,
             int skip = 0, bool randomize = false)
        {
            while (true)
            {
                //long ss = items.Count();
                //if (ss > 62)
                //{
                //    throw new Exception("Too many items in the powerset. This is not supported.");
                //}
                //long lastIndex = 1L << ss;

                if (skip>0)
                {
                    var subset = new List<InternalTextSection>();
                    for (int ii = 0; ii < items.Count(); ii++)
                    {
                        if ((skip & (1 << ii)) != 0)
                        {
                            subset.Add(items.Skip(ii).First());
                        }
                    }

                    if (randomize)
                    {
                        subset = subset.OrderBy(el => Statics.Random.Next()).ToList();
                    }

                    yield return subset;
                }
                else
                {
                    var lastIndex = 1L << items.Count();
                    //just iterate through all the items.
                    for (long index = skip; index < lastIndex; index++)
                    {
                        var subset = new List<InternalTextSection>();
                        for (int ii = 0; ii < items.Count(); ii++)
                        {
                            if ((index & (1 << ii)) != 0)
                            {
                                subset.Add(items.Skip(ii).First());
                            }
                        }

                        if (randomize)
                        {
                            subset = subset.OrderBy(el => Statics.Random.Next()).ToList();
                        }

                        yield return subset;
                    }
                }
            }
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

        public static void UpdateWithFilterResult(IEnumerable<IEnumerable<InternalTextSection>> ungroupedSections, TextChoiceResultEnum el)
        {
            ///So for example a powerset of [a,b,c] might send (a,c) here.
            foreach (var section in ungroupedSections)
            {
                foreach (var p in section)
                {
                    if (p.Parent == null)
                    {
                        continue;
                    }
                    p.Parent.ReceiveChoiceResult(p.S, el); ;
                }
            }
        }
    }
}
