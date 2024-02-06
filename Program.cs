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
        public static int SentCount { get; set; } = 0;
        public static int DoneCount { get; set; } = 0;

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

                inp = "A splendid cat in {GPTLocations}";
                inp = "“Tiger, one day you will come to a fork in the road,” he said. “And you’re going to have to make a decision about which direction you want to go.” He raised his hand and pointed. “If you go that way you can be somebody. You will have to make compromises and you will have to turn your back on your friends. But you will be a member of the club and you will get promoted and you will get good assignments.” Then Boyd raised his other hand and pointed another direction. “Or you can go that way and you can do something—something for your country and for your Air Force and for yourself. If you decide you want to do something, you may not get promoted and you may not get the good assignments and you certainly will not be a favorite of your superiors. But you won’t have to compromise yourself. You will be true to your friends and to yourself. And your work might make a difference.” He paused and stared into Leopold’s eyes and heart. “To be somebody or to do something. In life there is often a roll call. That’s when you will have to make a decision. To be or to do? Which way will you go?” " +
                    "{GPTLocations}" +
                    "{GPTStyles}  -r";
                inp = "Three can keep a secret, if two of them are dead. {GPTLocations} {GPTStyles}  -r -hd -h";
                inp = " -hd -h -3   Cozy Café in Downtown San Francisco: The Apple Vision Pro™, a white modern curved AR/VR device makes its grand entrance into the vintage café, worn by Alex, a software developer with a knack for immersing himself in the latest technology. His friends, Mia and Liam, are initially amused by Alex's enthusiastic demonstration of the device's capabilities. Mia, with her mismatched clothes and sketchbook in hand, playfully teases Alex about living in a virtual world. Liam, adjusting his signature bow tie, tries the device with a mix of curiosity and skepticism. As Alex dives deeper into his augmented reality, the café buzzes with his exclamations and virtual interactions. Mia's amusement turns to annoyance as she fails to capture Alex's attention for a real-world conversation. She confronts him, her words sharp but tinged with concern. Liam, ever the mediator, attempts to bridge the gap between technology and personal connection. The tension escalates as Zara, the novelist in the corner, observes intently, her typewriter forgotten. The scene reaches a fever pitch as Alex, engrossed in his AR world, accidentally knocks over Mia's coffee, spilling it over her sketchbook. The café falls silent, all eyes on the trio, as Mia's next words hang in the air, poised to either forgive or further inflame the situation.";
                inp = "-hd -h -3  Bustling New York Subway During Rush Hour: On the crowded subway, Jordan dons the Apple Vision Pro™, a white modern curved AR/VR device, his eyes alight with the thrill of the latest gadget. His excitement, however, is oblivious to the cramped space and the weary commuters around him. Emily, just off a grueling night shift, watches with a mix of exhaustion and irritation as Jordan bumps into her repeatedly, lost in his augmented adventure. Carlos, with his camera slung over his shoulder, is initially captivated by the technology but grows concerned about Jordan's lack of spatial awareness. The tension in the subway car builds as Jordan, engrossed in his game, nearly steps on Rachel, the retired schoolteacher. Rachel, her patience fraying, confronts Jordan with a stern reprimand about respecting others' space. The situation escalates as more passengers express their frustration. Jordan, finally snapping out of his virtual world, realizes the chaos he's caused. As he begins to apologize, the subway lurches unexpectedly, throwing him off balance and into a moment of vulnerability that could either lead to a heated confrontation or an understanding resolution.\r\n";
                //inp = "-hd -h -3  Trendy Rooftop Bar in Los Angeles at Night: The night is alive at the rooftop bar as Chloe showcases her Apple Vision Pro™, live-streaming her experience to her online followers. Her vibrant personality and daring fashion choices draw attention, but not all of it is positive. Ethan, trying to impress his date, finds Chloe's loud streaming disruptive to the romantic ambiance he's trying to create. Nina, spotting a potential social media rivalry, watches Chloe with a calculating eye. The bar's atmosphere shifts from celebratory to tense as Ethan approaches Chloe, his request for her to lower her voice clashing with her desire for online engagement. Nina senses an opportunity and intervenes, suggesting a collaboration that could either defuse the situation or escalate it. Derek, the bartender, watches the unfolding drama, ready to step in if the conflict disrupts his patrons' enjoyment. As Chloe turns to respond, her stream captures a moment that could either skyrocket her fame or lead to a social media disaster, leaving her followers on the edge of their seats.";
                //inp = "-hd -h -3 Public Library in Chicago: Sarah, wearing the sleek Apple Vision Pro™, enters the library, her voice echoing as she narrates her experience for her tech blog. Her enthusiasm, however, disrupts the library's tranquil ambiance. Mr. Thompson, the librarian, watches with a mix of fascination and concern, his love for order clashing with his curiosity about new technology. Ben, cramming for his finals, tries to concentrate but finds his attention repeatedly diverted by Sarah's loud commentary. The library, usually a haven of quiet study, becomes a stage for a growing conflict. Sophie, another tech enthusiast, is intrigued by the device but disapproves of Sarah's disregard for library etiquette. The tension comes to a head as Ben confronts Sarah, his frustration boiling over. Words are exchanged, each more heated than the last. Sarah, caught off guard, faces a choice: to defend her right to explore technology or to acknowledge the disruption she's caused. The library holds its breath, waiting for her response, as the situation teeters on the edge of a dramatic climax.";
                args = inp.Split();
            }

            //var a = "d:/proj/dalle3/output/A_splendid_cat_in_The_Ruins_of_an_Ancient_City_among_the_crumbling_walls_lost_relics_and_symbols_and_abandoned_empty_streets_of_a-20240203-standard-sq_3.png";
            //var p = "A splendid cat in The Ruins of an Ancient City among the crumbling walls lost relics and symbols and abandoned empty streets of a once-great metropolis";
            //var ann = new Annotator();
            //var a2 = a.Replace(".png", "_annotated.png");
            //ann.Annotate(a, a2, p);

            var quality = "standard";
            quality = "hd";
            var sz = ImageSize._1024;
            sz = ImageSize._1792x1024;

            //if n>1, and you also use curly brackets, then this
            //will make us skip the failed bad prompts, but not skip the 
            //other ones in the group which might actually be okay.
            var badPrompts = new List<string>();
            var actuallyGeneratedCount = 0;
            var start = DateTime.Now;

            if (args.Length > 0)
            {
                var modelOptions = Statics.Parse(args);
                if (modelOptions == null)
                {
                    Statics.Logger.Log("problem with input.");
                    Usage();
                    return;
                }
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
                        var res = GenerateOneImageAsync(api, subPrompt, modelOptions.Size, modelOptions.Quality);
                        actuallyGeneratedCount++;

                        if (actuallyGeneratedCount >= 300)
                        {
                            Statics.Logger.Log("break early due to generating so many. =================.");
                            break;
                        }

                        ///okay just pause if you find yourself (temporarily) over the rate limit.
                        var actualGenerationRate = actuallyGeneratedCount / (DateTime.Now - start).TotalMinutes;
                        if (actualGenerationRate > GetMyImagesPerMinuteRateLimit())
                        {
                            //damn, how do I format this to not show many digits of numbers, just 2 digits?
                            Statics.Logger.Log($"sleeping: 4s since my generation rate was at: {actualGenerationRate.ToString("0.00")}/min, and based on the hardcoded tier I am in in statics.cs, my limit in images/min is: {GetMyImagesPerMinuteRateLimit()}");
                            System.Threading.Thread.Sleep(4000);
                        }
                    }

                }
                Statics.Logger.Log($"{DoneCount}/{SentCount} Done, waiting for you to hit a button to give time for the last image to dl.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine($"You triggered the help display by not submitting any arguments, so printing it and quitting.");
                Usage();
            }
        }

        static async Task<bool> GenerateOneImageAsync(OpenAI_API.OpenAIAPI api, string prompt, ImageSize size, string quality)
        {
            var req = new ImageGenerationRequest();
            req.Model = OpenAI_API.Models.Model.DALLE3;

            //the library magically returns null when the actual object is "standard" and you are using dalle3 for some reason.
            //so fix it here for tracking.
            req.Quality = quality ?? "standard";
            prompt = Substitutions.SubstituteExpansionsIntoPrompt(prompt);
            req.Prompt = prompt.Substring(0,Math.Min(prompt.Length, 3000));
            req.Size = size;

            var l = req.Prompt.Length;
            var displayedPromptLength = 200;
            Statics.Logger.Log($"Sending to imagemaker:\t\"{req.Prompt.Substring(0, Math.Min(l, displayedPromptLength))}\"");
            SentCount++;
            var res = await api.ImageGenerations.CreateImageAsync(req);
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
                        client.DownloadFileCompleted += (sender, e) => DownloadCompleted(sender, e, tempFP, fp, prompt);
                        client.DownloadFileAsync(new Uri(res.Data[0].Url), tempFP);
                        downloadRes = true;
                        break;
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
                        else if (ex.InnerException.Message.Contains("invalid_request_error") && ex.InnerException.Message.Contains(" is too long - \'prompt\'"))
                        {
                            Statics.Logger.Log($"Your prompt was {req.Prompt.Length} characters and it started: \"{req.Prompt.Substring(0,30)}...\". This is too long. The actual limit is X.");
                            downloadRes = false;
                            break;
                        }
                        else if (ex.InnerException.Message.Contains("Rate limit repeatedly exceeded"))
                        {
                            Statics.Logger.Log($"You blew up the rate limit unfortunately.");
                            Statics.Logger.Log($"{GetMessageLine(ex.InnerException.Message)}");
                            downloadRes = false;
                            break;
                        }
                        
                        else
                        {
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
        //3. we also delay creating the unique filename.
        //4. also save an annotated version?
        private static void DownloadCompleted(object sender, AsyncCompletedEventArgs e, string tempfp, string fp, string prompt)
        {
            try
            {
                var ann = new Annotator();
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
                var annotatedFp = uniquefp.Replace(".png", "_annotated.png");
                
                ann.Annotate(uniquefp, annotatedFp, prompt, true);
                DoneCount++;
                Statics.Logger.Log($"Saved {DoneCount}/{SentCount} to:\t\t\"{uniquefp}\"");
            }
            catch (Exception ex)
            {
                var a = 4;
                Statics.Logger.Log(ex.ToString());
            }
        }
    }
}
