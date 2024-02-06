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

using System.ComponentModel;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using MetadataExtractor.Formats.Gif;
using System.Threading;
using MetadataExtractor.Formats.Exif.Makernotes;

namespace Dalle3
{
    internal class Program
    {
        public static int RequestedCount { get; set; } = 0;
        public static int DownloadedCount { get; set; } = 0;
        public static int ErrorCount{ get; set; } = 0;

        static async Task Main(string[] args)
        {
            await AsyncMain(args);
        }

        static async Task AsyncMain(string[] args)
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
            //Statics.Logger.Log($"There are: {optionsModel.EffectivePrompts.Count} prompts, which we will repeat {optionsModel.ImageNumber} times.");
            var api = new OpenAI_API.OpenAIAPI(Statics.ApiKey);
            
            var start = DateTime.Now;
            
            for (var ii = 0; ii < optionsModel.ImageNumber; ii++)
            {
                //general obey rate limit.
                await throttler.WaitAsync();

                var taskId = ii+10000;
                var textSections = optionsModel.PromptSections.ToList().Select(x => x.Sample());

                var req = new ImageGenerationRequest();
                req.Model = OpenAI_API.Models.Model.DALLE3;

                //the library magically returns null when the actual object is "standard" and you are using dalle3 for some reason.
                //so fix it here for tracking.
                req.Quality = quality ?? "standard";
                //var usingSubPrompt = Substitutions.SubstituteExpansionsIntoPrompt(subPrompt);
                req.Prompt = string.Join(" ", textSections.Select(el => el.L));
                req.Size = optionsModel.Size;
                var humanReadable = string.Join("_", textSections.Select(el => el.GetValueForHumanConsumption()));

                var l = req.Prompt.Length;
                var displayedPromptLength = 200;
                Statics.Logger.Log($"Sending to imagemaker:\t\"{req.Prompt.Substring(0, Math.Min(l, displayedPromptLength))}\"");

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
                    RequestedCount++;
                    var res = api.ImageGenerations.CreateImageAsync(req);

                    //this is the final destination; in actuality we will temporarily store them up one folder!
                    //also we reserve this location with something.
                    var destFp = Statics.PromptToDestFpWithReservation(req, humanReadable, taskId);
                    var tempFp = $"{Path.GetTempPath()}{taskId}.png";

                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            //client.DownloadProgressChanged += (sender, e) => DownloadProgressHappened(sender, e, tempFp, fp, req.Prompt);
                            client.DownloadFileCompleted += (sender, e) => DownloadCompleted(sender, e, tempFp, destFp, req.Prompt, textSections);
                            client.DownloadFileAsync(new Uri(res.Result.Data[0].Url), tempFp);
                        }

                        catch (Exception ex)
                        {
                            ErrorCount++;
                            if (ex.InnerException.Message.Contains("Your request was rejected as a result of our safety system. Your prompt may contain text that is not allowed by our safety system."))
                            {
                                Statics.Logger.Log($"Prompt rejection.\t\"{req.Prompt}\"");
                                System.IO.File.Delete(destFp);
                                UpdateWithFilterResult(textSections, TextChoiceResultEnum.PromptRejected);
                            }
                            else if (ex.InnerException.Message.Contains("Your request was rejected as a result of our safety system. Image descriptions generated from your prompt may contain text that is not allowed by our safety system. If you believe this was done in error, your request may succeed if retried, or by adjusting your prompt."))
                            {
                                Statics.Logger.Log($"Image descriptions from output were bad.\t\"{req.Prompt}\"");
                                System.IO.File.Delete(destFp);
                                UpdateWithFilterResult(textSections, TextChoiceResultEnum.DescriptionsBad);
                            }
                            else if (ex.InnerException.Message.Contains("This request has been blocked by our content filters."))
                            {
                                Statics.Logger.Log($"Content filter block.\t\"{req.Prompt}\"");
                                System.IO.File.Delete(destFp);
                                UpdateWithFilterResult(textSections, TextChoiceResultEnum.RequestBlocked);
                            }
                            else if (ex.InnerException.Message.Contains("\"Rate limit exceeded for images per minute in organization"))
                            {
                                var sleepTime = 10;
                                Statics.Logger.Log($"{ex.InnerException.Message}  sleep for: {sleepTime}s");
                                //UpdateWithFilterResult(textSections, TextChoiceResultEnum.RateLimit);
                                System.IO.File.Delete(destFp);
                                await Task.Delay(sleepTime * 1000);
                            }
                            else if (ex.InnerException.Message.Contains("\"Billing hard limit has been reached\""))
                            {
                                Statics.Logger.Log(ex.InnerException.Message);
                                System.IO.File.Delete(destFp);
                                UpdateWithFilterResult(textSections, TextChoiceResultEnum.BillingLimit);
                            }
                            else if (ex.InnerException.Message.Contains("invalid_request_error") && ex.InnerException.Message.Contains(" is too long - \'prompt\'"))
                            {
                                Statics.Logger.Log($"Your prompt was {req.Prompt.Length} characters and it started: \"{req.Prompt.Substring(0, 30)}...\". This is too long. The actual limit is X.");
                                System.IO.File.Delete(destFp);
                                UpdateWithFilterResult(textSections, TextChoiceResultEnum.TooLong);
                            }
                            else if (ex.InnerException.Message.Contains("Rate limit repeatedly exceeded"))
                            {
                                Statics.Logger.Log($"You blew up the rate limit unfortunately.");
                                Statics.Logger.Log($"{GetMessageLine(ex.InnerException.Message)}");
                                System.IO.File.Delete(destFp);
                                UpdateWithFilterResult(textSections, TextChoiceResultEnum.RateLimitRepeatedlyExceeded);
                            }
                            else
                            {
                                Statics.Logger.Log($"\tUnknownException.\t\"{req.Prompt}\"\r\n{ex}");
                                Statics.Logger.Log($"{GetMessageLine(ex.InnerException.Message)}");
                                System.IO.File.Delete(destFp);
                            }
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }
                });

                tasks.Add(task);

                if (DownloadedCount >= 300)
                {
                    Statics.Logger.Log("break early due to generating so many. =================.");
                    break;
                }
            }

            Statics.Logger.Log("Got to end, waiting. =================.");
            await Task.WhenAll(tasks);
            Statics.Logger.Log("======done with WhenALL. =================.");
            
            while (true)
            {
                Statics.Logger.Log($"\tDownloaded:{DownloadedCount}, Requested:{RequestedCount}, Errored:{ErrorCount}");
                if (RequestedCount <= DownloadedCount + ErrorCount)
                {
                    break;
                }
                await Task.Delay(1000);
            }
            
            Statics.Logger.Log("======Reports: =================.");
            foreach (var el in optionsModel.PromptSections)
            {
                Statics.Logger.Log(el.ReportResults());
            }

            Statics.Logger.Log($"{DownloadedCount}/{RequestedCount} downloaded successfully ({100.0*DownloadedCount / (RequestedCount * 1.0):0.0}%). Hit a key to end.");

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
            var progress = e as DownloadProgressChangedEventArgs;
            if (progress != null)
            {
                Statics.Logger.Log($"\t\t{fp}\t{progress.ProgressPercentage}%");
            }
        }

        //Google photo uploader and other programs will incorrectly start messing with files before they're finished.
        //1. if you save them with another extension which would be ignored, then you get locking issues when trying to move them.
        //2. so instead i just save them out of view then move them back.
        //3. we also delay creating the unique filename.
        //4. also save an annotated version?
        private static void DownloadCompleted(object sender, AsyncCompletedEventArgs e, string srcFp, string destFp, string prompt, IEnumerable<InternalTextSection> textSections)
        {
            try
            {
                //the new reservation system? ugh.
                if (!System.IO.File.Exists(destFp))
                {
                    throw new Exception("No reservation.");
                }
                var fa = new FileInfo(destFp);
                if (fa.Length > 0)
                {
                    throw new Exception("too bad.'");
                }

                try
                {
                    System.IO.File.Copy(srcFp, destFp, true);
                    System.IO.File.Delete(srcFp);
                    DownloadedCount++;
                    
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    //we have already moved the file where its supposed to go.
                    Statics.Logger.Log($"Disappeared so going on: {destFp}");
                    ErrorCount++;
                    //break;

                }
                catch (System.IO.IOException ex)
                {
                    //well, sometimes we call download completed... before the image is completely downloaded. yah that's how things go.
                    Task.WaitAll(Task.Delay(1000));
                    Statics.Logger.Log($"IOexcpeton. {ex} {destFp}");
                    ErrorCount++;
                    //continue;
                }

                catch (Exception ex)
                {
                    //either it was busy, or the target existed? not sure about that latter thing.
                    Statics.Logger.Log($"\t{destFp}\tFailed with ex: {ex}.");
                    Task.WaitAll(Task.Delay(1000));
                    ErrorCount++;
                    //continue;
                }

                UpdateWithFilterResult(textSections, TextChoiceResultEnum.Okay);
                //we need to redo the target name cause its gotten taken in the meantime.

                var ann = new Annotator();
                //assume the file exists at uniquefp now.
                var annotatedFp = destFp.Replace(".png", "_annotated.png");
                
                var directory = Path.GetDirectoryName(annotatedFp);
                var filename = Path.GetFileName(annotatedFp);
                annotatedFp = directory + "/annotated/" + filename;
                
                ann.Annotate(destFp, annotatedFp, prompt, true);
                
                Statics.Logger.Log($"\t{DownloadedCount}/{RequestedCount} Saved. \t{destFp}\t");
            }
            catch (Exception ex)
            {
                Statics.Logger.Log("weird error."+ex.ToString());
                ErrorCount++;
            }
        }
    }
}
