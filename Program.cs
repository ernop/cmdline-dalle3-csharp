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
using static Dalle3.Statics;
using static Dalle3.Substitutions;
using System.ComponentModel;
using System.IO;
using System.Diagnostics.Eventing.Reader;

namespace Dalle3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var api = new OpenAI_API.OpenAIAPI(Statics.ApiKey);

            //NOTE: if you are testing in VS, uncomment this.  But remember to comment it again before building the binary to actually run.
            var locl = false;
            locl = true;
            if (locl && (args.Length == 0))
            {
                var inp = "A chonky mutant {cassowary,petroglyph,octopus} spaceship floating above the {rainbow, Lightning} surface of jupiter shooting cats eye lasers at aliens and floating and disturbing the wild red misty stone and dust rings";
                //inp = "a single kanji character for da \"big\", in {golden ink, purple ink, rainbow ink, blue ink, black ink}";
                //inp = "{watercolor, illustration, pointillism} of {boy, woman, man} in {mystery, romance, science fiction} set {mountains, ocean, desert}. -r";
                inp = "{watercolor, illustration, pointillism} of {boy, woman, man} in {mystery, romance, science fiction} set {mountains, ocean, desert}. -r";
                inp = "{Watercolor, Illustration, Sprang} of {Boy, Woman, Man} in {Mystery, Romance, Science Fiction} set {Mountains}. -r";
                inp = "{huchol art style, stark silhouette extreme chiaroscuro style, reminiscent of the style of dick sprang, similar ot the style of paul cesar helleu, in the photographic style of mark shaw} of {Man} in {Mystery, Romance, Science Fiction} set {Mountains}. -r";
                inp = "{an 2d overlay paper cutout by william steig, " +
                    //"in the emotionally resonant glowing style of jon carling, " +
                    //"a chiaroscuro etching and black edge silhouettes like gregory crewdson, " +

                    "a flat 2d incredibly detailed and emotional style cartoon image by arnold lobel and william sprang with a warm glow and nostalgia," +
                    //"a mo willems flat edgeless geometric cartoon style , " + //this is fucking amazing but a bit inconsistent.
                    //"in the style of george shaw photographs, " +
                    "a bright colorful claire wendling engraving, " +
                    //"a rich detailed margaret tarrant flat 2d highly textured drawings, " +
                    "a glowing fantastical matt wuerker and tom tomorrow style illustration," +
                    //"a very colorful and rich tom tomorrow drawing, " +
                    //"in the style of thomas nast," +
                    //"an incredibly detailed and emotional william sprang style" +
                    "} of {Boy, Woman, Man} in {Science Fiction, Romance} set {Desert, Mountains}.";
                //inp = "{Watercolor, Illustration, Sprang} of {Boy, Woman, Man} in {Mystery, Romance, Science Fiction} set {Mountains}. -r";

                inp = "cat in {GPTLocations} {GPTLocations}";
                args = inp.Split();
            }

            var quality = "standard";
            quality = "hd";
            var sz = ImageSize._1024;
            sz = ImageSize._1792x1024;

            //if n>1, and you also use curly brackets, then this
            //will make us skip the failed bad prompts, but not skip the 
            //other ones in the group which might actually be okay.
            var badPrompts = new List<string>();
            var actuallyGeneratedCount = 0;

            if (args.Length > 0)
            {
                var modelOptions = Statics.Parse(args);
                if (modelOptions == null)
                {
                    Statics.Logger.Log("problem with input");
                    Usage();
                    return;
                }
                var list = new List<Task<bool>>();
                Statics.Logger.Log($"There are: {modelOptions.EffectivePrompts.Count} prompts, which we will repeat {modelOptions.ImageNumber} times.");
                for (var ii = 0; ii < modelOptions.ImageNumber; ii++)
                {
                    foreach (var subPrompt in modelOptions.EffectivePrompts)
                    {
                        if (badPrompts.IndexOf(subPrompt) != -1)
                        {
                            Statics.Logger.Log($"\r\n-----------Skipping due to previous badness: {badPrompts} {ii}");
                            continue;
                        }
                        GenerateOneImageAsync(api, subPrompt, modelOptions.Size, modelOptions.Quality);
                        actuallyGeneratedCount++;

                        if (actuallyGeneratedCount >= 300)
                        {
                            Statics.Logger.Log("break early due to generating so many. =================.");
                            break;
                        }
                    }

                    //we are rate limited at 7/min so we should wait a little bit longer than that.
                    //tier 3 = 7
                    //tier 4 = 15
                    //tier 5 = 50
                    var myRateLimit = 15.0;
                    var amt = 1000 * (60 / (myRateLimit - 1));
                    Statics.Logger.Log($"sleeping: {amt / 1000}");
                    System.Threading.Thread.Sleep((int)amt);
                }
                Statics.Logger.Log("Done, waiting for you to hit a button to give time for the last image to dl.");
                Console.ReadLine();
            }
            else
            {
                Usage();
            }
        }

        static bool GenerateOneImageAsync(OpenAI_API.OpenAIAPI api, string prompt, ImageSize size, string quality)
        {
            var req = new ImageGenerationRequest();
            req.Model = OpenAI_API.Models.Model.DALLE3;

            //the library magically returns null when the actual object is "standard" and you are using dalle3 for some reason.
            //so fix it here for tracking.
            req.Quality = quality ?? "standard";
            prompt = Substitutions.SubstituteExpansionsIntoPrompt(prompt);
            req.Prompt = prompt;
            req.Size = size;

            var l = req.Prompt.Length;
            var displayedPromptLength = 200;
            Statics.Logger.Log($"Sending to imagemaker:\t\"{req.Prompt.Substring(0, Math.Min(l, displayedPromptLength))}\"");
            var res = api.ImageGenerations.CreateImageAsync(req);
            var outfn = Statics.PromptToFilename(req);

            //this is the final destination; in actuality we will temporarily store them up one folder!
            var fp = $"d:/proj/dalle3/output/{outfn}";
            var randomSuffix = new Random().Next(1000, 9999);
            var tempFP = $"{Path.GetTempPath()}{randomSuffix}.png";
            var tries = 0;
            var downloadRes = false;
            while (tries < 5)
            {
                tries++;

                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFileCompleted += (sender, e) => DownloadCompleted(sender, e, tempFP, fp);
                        client.DownloadFileAsync(new Uri(res.Result.Data[0].Url), tempFP);
                        if (!string.IsNullOrEmpty(res.Result.Data[0].RevisedPrompt))
                        {
                            Statics.Logger.Log($"Revised prompt was: {res.Result.Data[0].RevisedPrompt}");
                        }
                        downloadRes = true;
                        break;
                        //var ann = new Annotator();
                        //var annotatedfp = fp.Replace(".png", "_annotated.png");
                        //ann.Annotate(fp, annotatedfp, prompt);
                        //IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(fp);
                    }
                    catch (Exception ex)
                    {
                        //split multiple types - input error, vs prompt good but generated image bad, are there more too?
                        if (ex.InnerException.Message.Contains("Your request was rejected as a result of our safety system. Your prompt may contain text that is not allowed by our safety system."))
                        {
                            Statics.Logger.Log($"Prompt rejection.\t\"{req.Prompt}\"");
                            //Statics.Logger.Log($"{GetMessageLine(ex.InnerException.Message)}");
                            downloadRes = false;
                            break;
                        }
                        else if (ex.InnerException.Message.Contains("Your request was rejected as a result of our safety system. Image descriptions generated from your prompt may contain text that is not allowed by our safety system. If you believe this was done in error, your request may succeed if retried, or by adjusting your prompt."))
                        {
                            Statics.Logger.Log($"Image descriptions from output were bad.\t\"{req.Prompt}\"");
                            //Statics.Logger.Log($"{GetMessageLine(ex.InnerException.Message)}");
                            downloadRes = false;
                            break;
                        }
                        else if (ex.InnerException.Message.Contains("This request has been blocked by our content filters."))
                        {
                            Statics.Logger.Log($"Content filter block.\t\"{req.Prompt}\"");
                            //Statics.Logger.Log($"{GetMessageLine(ex.InnerException.Message)}");
                            downloadRes = false;
                            break;
                        }
                        else if (ex.InnerException.Message.Contains("\"Rate limit exceeded for images per minute in organization"))
                        {
                            var sleepTime = 10;
                            Statics.Logger.Log($"{ex.InnerException.Message}  sleep for: {sleepTime * 1000}");
                            //after sleeping, then try to download again.
                            continue;
                        }
                        else if (ex.InnerException.Message.Contains("\"Billing hard limit has been reached\""))
                        { 
                            Statics.Logger.Log(ex.InnerException.Message);
                            downloadRes = false;
                            break;
                        }
                        else
                        {
                        var aww = 34;
                        Statics.Logger.Log($"\t.\t\"{req.Prompt}\"");
                        Statics.Logger.Log($"{GetMessageLine(ex.InnerException.Message)}");
                        downloadRes = false;
                        break;
                    }
                }
            }
        }
            return downloadRes;
        }

    private static string GetMessageLine(string inlines)
    {
        var p = inlines.Split(new[] { "\n" }, StringSplitOptions.None);
        foreach (var el in p)
        {
            if (el.Contains("\"message\":"))
            {
                return el.Trim();
            }
        }
        return "";
    }

    //Google photo uploader and other programs will incorrectly start messing with files before they're finished.
    //1. if you save them with another extension which would be ignored, then you get locking issues when trying to move them.
    //2. so instead i just save them out of view then move them back.
    private static void DownloadCompleted(object sender, AsyncCompletedEventArgs e, string tempfp, string fp)
    {
        try
        {
            var uniquefp = fp.Replace(".png", "_1.png");
            var tries = 0;
            while (true)
            {
                if (!System.IO.File.Exists(uniquefp))
                {
                    break;
                }
                tries++;
                uniquefp = fp.Replace(".png", $"_{tries}.png");
            }
            System.IO.File.Move(tempfp, uniquefp);
            Statics.Logger.Log($"Saved to:\t\t\"{uniquefp}\"");
        }
        catch (Exception ex)
        {
            var a = 4;
            Statics.Logger.Log(ex.ToString());
        }
    }
}
}
