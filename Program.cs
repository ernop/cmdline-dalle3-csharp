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
using MetadataExtractor.Formats.Gif;
using System.Threading;

namespace Dalle3
{
    internal class Program
    {
        public static int SentCount { get; set; } = 0;
        public static int DoneCount { get; set; } = 0;

        static async Task Main(string[] args)
        {
            await Thingie(args);
        }

        static async Task Thingie(string[] args)
        {
            var tasks = new List<Task>();
            var throttler = new SemaphoreSlim(10, 10);
            var locl = false;
            locl = true;
            //overriding for testing etc.
            if (locl && (args.Length == 0))
            {
                args = Statics.OverridePromptForTesting.Split(new[] { " " }, StringSplitOptions.None);
            }

            var quality = "hd";
            var sz = ImageSize._1792x1024;

            //if n>1, and you also use curly brackets, then this
            //will make us skip the failed bad prompts, but not skip the 
            //other ones in the group which might actually be okay.

            if (args.Length == 0)
            {
                Console.WriteLine($"You triggered the help display by not submitting any arguments, so printing it and quitting.");
                Usage();
                Environment.Exit(0);
            }

            var optionsModel = Statics.Parse(args);
            if (optionsModel == null)
            {
                Statics.Logger.Log("problem with input.");
                Usage();
                Environment.Exit(0);
            }
            Statics.Logger.Log($"There are: {optionsModel.EffectivePrompts.Count} prompts, which we will repeat {optionsModel.ImageNumber} times.");
            var api = new OpenAI_API.OpenAIAPI(Statics.ApiKey);
            var badPrompts = new List<string>();
            var actuallyGeneratedCount = 0;
            var start = DateTime.Now;

            for (var ii = 0; ii < optionsModel.ImageNumber; ii++)
            {
                foreach (var subPrompt in optionsModel.EffectivePrompts)
                {
                    //general obey rate limit.
                    await throttler.WaitAsync();

                    var taskId = ii;
                    if (badPrompts.IndexOf(subPrompt) != -1)
                    {
                        Statics.Logger.Log($"\r\n-----------Skipping due to previous badness: {badPrompts} {ii}");
                        continue;
                    }

                    var req = new ImageGenerationRequest();
                    req.Model = OpenAI_API.Models.Model.DALLE3;

                    //the library magically returns null when the actual object is "standard" and you are using dalle3 for some reason.
                    //so fix it here for tracking.
                    req.Quality = quality ?? "standard";
                    var usingSubPrompt = Substitutions.SubstituteExpansionsIntoPrompt(subPrompt);
                    req.Prompt = usingSubPrompt.Substring(0, Math.Min(usingSubPrompt.Length, 3000));
                    req.Size = optionsModel.Size;

                    var l = req.Prompt.Length;
                    var displayedPromptLength = 200;
                    Statics.Logger.Log($"Sending to imagemaker:\t\"{req.Prompt.Substring(0, Math.Min(l, displayedPromptLength))}\"");
                    SentCount++;

                    //during asyncification: actually, I was already doing things wrong, probably. This 
                    // method is actually where the exceptions were popping out of, but like 100% of the time, the path from here to the big try catch below
                    //was so fast, we'd probably always be there already by the time an exception was thrown.

                    //the plan now: copy all the significant stuff from in there into this block, including exception handling. That way
                    //each guy gets handled one by one, and the exceptions never pop up.
                    //AND we can use semaphoreslim to manage it, smartly, so we can still jam up the outbound queue

                    //okay so the idea now is that we have a "danger zone" which we enter.
                    //we want to may tasks async so they can run past that point, but don't let them escape the trycatch
                    //until we know they are done and can't at all be risky anymore.
                    var task = Task.Run(async () =>
                    {
                        var res = api.ImageGenerations.CreateImageAsync(req);
                        var outfn = Statics.PromptToFilename(req);

                        //this is the final destination; in actuality we will temporarily store them up one folder!

                        var fp = $"d:/proj/dalle3/output/{outfn}";

                        var tries = 0;
                        var downloadRes = false;
                        while (tries < 5)
                        {
                            tries++;
                            var randomSuffix = new Random().Next(1000, 9999);
                            var tempFP = $"{Path.GetTempPath()}{randomSuffix}.png";

                            using (WebClient client = new WebClient())
                            {
                                try
                                {
                                    client.DownloadProgressChanged += (sender, e) => DownloadProgressHappened(sender, e, tempFP, fp, req.Prompt);
                                    client.DownloadFileCompleted += (sender, e) => DownloadCompleted(sender, e, tempFP, fp, req.Prompt);
                                    await client.DownloadFileTaskAsync(new Uri(res.Result.Data[0].Url), tempFP);
                                    downloadRes = true;
                                    break;
                                }

                                catch (Exception ex)
                                {
                                    if (ex.InnerException.Message.Contains("Your request was rejected as a result of our safety system. Your prompt may contain text that is not allowed by our safety system."))
                                    {
                                        Statics.Logger.Log($"Prompt rejection.\t\"{req.Prompt}\"");
                                        downloadRes = false;
                                        break;
                                    }
                                    else if (ex.InnerException.Message.Contains("Your request was rejected as a result of our safety system. Image descriptions generated from your prompt may contain text that is not allowed by our safety system. If you believe this was done in error, your request may succeed if retried, or by adjusting your prompt."))
                                    {
                                        Statics.Logger.Log($"Image descriptions from output were bad.\t\"{req.Prompt}\"");
                                        downloadRes = false;
                                        break;
                                    }
                                    else if (ex.InnerException.Message.Contains("This request has been blocked by our content filters."))
                                    {
                                        Statics.Logger.Log($"Content filter block.\t\"{req.Prompt}\"");
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
                                        Statics.Logger.Log($"Your prompt was {req.Prompt.Length} characters and it started: \"{req.Prompt.Substring(0, 30)}...\". This is too long. The actual limit is X.");
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
                                finally
                                {
                                    throttler.Release();
                                }
                            }
                        }
                    });

                    tasks.Add(task);

                    actuallyGeneratedCount++;

                    if (actuallyGeneratedCount >= 300)
                    {
                        Statics.Logger.Log("break early due to generating so many. =================.");
                        break;
                    }
                }
                if (actuallyGeneratedCount >= 300)
                {
                    Statics.Logger.Log("break early due to generating so many. =================.");
                    break;
                }
            }

            Statics.Logger.Log("Got to end, now doing task.whenall. =================.");
            await Task.WhenAll(tasks);
            Statics.Logger.Log("done with WhenALL. =================.");
            Statics.Logger.Log($"{DoneCount}/{SentCount} Done, waiting for you to hit a button to give time for the last image to dl.");
            Console.ReadLine();
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

        private static void DownloadProgressHappened(object sender, DownloadProgressChangedEventArgs e, string tempfp, string fp, string prompt)
        {
            var progress = e.UserState as DownloadProgressChangedEventArgs;
            if (progress != null)
            {
                Statics.Logger.Log($"Downloading: {progress.ProgressPercentage}%");
            }
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
                var tries = 1;
                //lets find a new filename we can move the input one to.
                while (true)
                {
                    tries++;
                    //if its safe to move, then move it.
                    if (!System.IO.File.Exists(uniquefp))
                    {
                        try
                        {
                            System.IO.File.Move(tempfp, uniquefp);
                            Statics.Logger.Log(uniquefp + " success finally.");
                            break;
                        }
                        catch (System.IO.FileNotFoundException ex)
                        {
                            //we have already moved the file where its supposed to go.
                            //Statics.Logger.Log($"Disappeared so going on: {uniquefp}");
                            break;

                        }
                        catch (System.IO.IOException ex)
                        {
                            //well, sometimes we call download completed... before the image is completely downloaded. yah that's how things go.
                            Task.WaitAll(Task.Delay(1000));
                            continue;
                        }

                        catch (Exception ex)
                        {
                            //either it was busy, or the target existed? not sure about that latter thing.
                            Statics.Logger.Log(uniquefp + " failed.\r\n" + ex.ToString());
                            Task.WaitAll(Task.Delay(1000));
                            continue;
                        }
                    }

                    //we need to redo the target name cause its gotten taken in the meantime.
                    if (tries > 40)
                    {
                        Statics.Logger.Log(uniquefp + " failed after too many tries================== \r\n");
                        return;
                    }

                    uniquefp = fp.Replace(".png", $"_{tries}.png");
                }

                //assume the file exists at uniquefp now.
                var annotatedFp = uniquefp.Replace(".png", "_annotated.png");

                ann.Annotate(uniquefp, annotatedFp, prompt, true);
                DoneCount++;
                Statics.Logger.Log($"Saved {DoneCount}/{SentCount} to:\t\t\"{uniquefp}\"");
            }
            catch (Exception ex)
            {
                Statics.Logger.Log(ex.ToString());
            }
        }
    }
}
