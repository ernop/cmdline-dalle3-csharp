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
                inp = "in the style of {AwesomeStyles}, draw a completely new fantasy creature which an adventurer may make friends with, demonstrating its shape, si\r\nze, form,abilities, strengths, weaknesses, powers, dangers, and intelligence.  The creature is unique and is illustrated in digital art with depth of field and layered composition.  They are highly detailed, unusua\r\nl, very distinct, and full of emotions. Thing about a new environmental and ecological role each one might have.  Each has unique features and body forms, arms, colors, textures, and many other unusual, fantasy or\r\nhigh-tech related unique aspects to its appearance.";
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
                    Console.WriteLine("problem with input");
                    Usage();
                    return;
                }
                var list = new List<Task<bool>>();
                for (var ii = 0; ii < modelOptions.ImageNumber; ii++)
                {
                    Console.WriteLine($"There are: {modelOptions.EffectivePrompts.Count} prompts");
                    foreach (var subPrompt in modelOptions.EffectivePrompts)
                    {
                        if (badPrompts.IndexOf(subPrompt) != -1)
                        {
                            Console.WriteLine($"Skipping: {badPrompts} {ii}");
                            continue;
                        }
                        GenerateOneImageAsync(api, subPrompt, modelOptions.Size, modelOptions.Quality);
                        actuallyGeneratedCount++;

                        if (actuallyGeneratedCount >= 100)
                        {
                            Console.WriteLine("break early.");
                            break;
                        }
                    }

                    //we are rate limited at 7/min so we should wait a little bit longer than that.
                    var amt = 1000 * (60 / 8.0);
                    Console.WriteLine($"sleeping: {amt / 1000}");
                    System.Threading.Thread.Sleep((int)amt);
                }
                Console.WriteLine("Waiting now.");
                Console.ReadLine();
            }
            else
            {

                Usage();
            }
        }

        static bool GenerateOneImageAsync(OpenAI_API.OpenAIAPI api, string prompt, ImageSize s, string quality)
        {
            var req = new ImageGenerationRequest();
            req.Model = OpenAI_API.Models.Model.DALLE3;
            
            //the library magically returns null when the actual object is "standard" and you are using dalle3 for some reason.
            //so fix it here for tracking.
            req.Quality = quality ?? "standard";
            req.Prompt = Substitutions.SubstituteExpansionsIntoPrompt(prompt);
            req.Size = s;

            Console.WriteLine($"Sending to imagemaker: \"{req.Prompt}\"");
            var res = api.ImageGenerations.CreateImageAsync(req);

            //obviously this is terrible.
            req.Prompt = prompt.Replace("AAA", "");
            var tries = 0;
            var outfn = Statics.PromptToFilename(req, tries);

            //this is the final destination; in actuality we will temporarily store them up one folder!
            var fp = $"d:/proj/dalle3/output/{outfn}";

            while (true)
            {
                if (!System.IO.File.Exists(fp))
                {
                    break;
                }
                tries++;
                outfn = Statics.PromptToFilename(req, tries);
                fp = $"d:/proj/dalle3/output/{outfn}";
            }

            using (WebClient client = new WebClient())
            {
                try
                {
                    //artificial non-jpg file type to avoid google drive and others starting to upload it too soon.
                    //var fpx = fp.Replace(".png", ".bin");
                    //client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownloadCompleted);
                    client.DownloadFileAsync(new Uri(res.Result.Data[0].Url), fp);
                    Console.WriteLine($"Revised prompt was: {res.Result.Data[0].RevisedPrompt}");
                    if (!string.IsNullOrEmpty(res.Result.Data[0].RevisedPrompt))
                    {
                        var a = 34;
                    }
                    //System.IO.File.Move(fpx, fp);
                    Console.WriteLine($"File downloaded successfully. {fp}");
                    //var ann = new Annotator();
                    //var annotatedfp = fp.Replace(".png", "_annotated.png");
                    //ann.Annotate(fp, annotatedfp, prompt);
                    //IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(fp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"b, {ex}");
                    return false;
                }
                return true;
            }
        }
    }
}
