using OpenAI_API.Images;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using CommandLine;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Linq;

namespace Dalle3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var api = new OpenAI_API.OpenAIAPI(Statics.ApiKey);
            //var inp = "A chonky cat spaceship floating above the surface of jupiter shooting cats eye lasers at aliens and floating and disturbing the wild {black, rocky, white} misty stone and dust rings";
            //args = inp.Split();

            //if n>1, and you also use curly brackets, then this
            //will make us skip the failed bad prompts, but not skip the 
            //other ones in the group which might actually be okay.
            var badPrompts = new List<string>();

            if (args.Length > 0)
            {
                var modelOpptions = Parse(args);
                if (modelOpptions == null)
                {
                    Console.WriteLine("problem with input");
                    Usage();
                    return;
                }
                for (var ii = 0; ii < modelOpptions.ImageNumber; ii++)
                {
                    foreach (var subPrompt in modelOpptions.EffectivePrompts)
                    {
                        if (badPrompts.IndexOf(subPrompt) != -1)
                        {
                            Console.WriteLine($"Skipping: {badPrompts} {ii}");
                            continue;
                        }
                        var res = GenerateOneImage(api, subPrompt, ImageSize._1024);

                        if (!res.Result)
                        {
                            Console.WriteLine("Breaking.");
                            badPrompts.Add(subPrompt);
                        }
                    }
                }

                return;
            }

            Usage();
        }

        static void Usage()
        {
            Console.WriteLine("Dalle3.exe [--N] [prompt]\r\ndalle3.exe A very {big,blue,tall} photo of a {tall,small} {cat,dog,mouse monster}\r\nN=number of times to repeat prompt. Will die if any fail. Prompt can be multiple words with no quotes required, but no newlines.\r\n{}=run all permutations of the items within here. This can blow up your api limits.");
        }

        static OptionsModel Parse(string[] args)
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

                optionsModel.RawPrompt += " " + s;
            }
            optionsModel.RawPrompt = optionsModel.RawPrompt.Trim();

            if (string.IsNullOrEmpty(optionsModel.RawPrompt)) { return null; }
            optionsModel.EffectivePrompts = PermutationExpander.ExpandCurlyItems(optionsModel.RawPrompt);
            return optionsModel;
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

        public static class PermutationExpander
        {
            /// <summary>
            /// Expands a string with permutations defined in curly braces.
            /// Example: "A {red, yellow, purple} cat {and dog,and man}" => "A red cat and dog", "A yellow cat and dog", "A purple cat and dog", "A red cat and man", "A yellow cat and man", "A purple cat and man"
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
                    var replaced = ReplaceOnce(input, target, part);
                    var subMatches = ExpandCurlyItems(replaced);
                    results.AddRange(subMatches);
                }
                return results;
            }
        }

        static string PromptToFilename(ImageGenerationRequest req, int tries)
        {
            var now = DateTime.Now;
            var usePrompt = req.Prompt;
            if (usePrompt.Length > 50)
            {
                usePrompt = usePrompt.Substring(0, 35);
            }
            var outfn = $"{usePrompt.Trim()}-{now.Year}{now.Month:00}{now.Day:00}-{req.Size}-{tries}.png";
            return outfn;
        }

        static async Task<bool> GenerateOneImage(OpenAI_API.OpenAIAPI api, string prompt, ImageSize s)
        {
            var req = new ImageGenerationRequest();
            req.Model = OpenAI_API.Models.Model.DALLE3;
            req.Quality = "hd";
            req.Prompt = prompt;
            req.Size = s;

            var res = api.ImageGenerations.CreateImageAsync(req);

            var tries = 0;
            var outfn = PromptToFilename(req, tries);

            var fp = $"d:/proj/dalle3/output/S_{outfn}";

            while (true)
            {
                if (!System.IO.File.Exists(fp))
                {
                    break;
                }
                tries++;
                outfn = PromptToFilename(req, tries);
                fp = $"d:/proj/dalle3/output/S_{outfn}"; ;
            }

            using (WebClient client = new WebClient())
            {
                try
                {
                    //artificial non-jpg file type to avoid google drive and others starting to upload it too soon.
                    var fpx = fp.Replace(".png",".bin");
                    Console.WriteLine(fpx);
                    client.DownloadFile(res.Result.Data[0].Url, fpx);
                    System.IO.File.Move(fpx, fp);
                    Console.WriteLine($"File downloaded successfully. {fp}");
                    //var ann = new Annotator();
                    //var annotatedfp = fp.Replace(".png", "_annotated.png");
                    //ann.Annotate(fp, annotatedfp, prompt);
                    //IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(fp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("b");
                    return false;
                }
                return true;
            }
        }
    }
}
