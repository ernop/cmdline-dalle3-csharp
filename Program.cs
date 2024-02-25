using OpenAI_API.Images;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using static Dalle3.Statics;

using System.ComponentModel;
using System.IO;

using System.Threading;
using Dalle3.Infra;
using OpenAI_API.Completions;
using System.Drawing.Text;

namespace Dalle3
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            bool locl = false;
            locl = true;
            //overriding for testing etc.

            if (locl && (args.Length == 0))
            {
                foreach (var argset in OverridePrompts.OverridePromptsForTesting)
                {
                    if (argset.Length == 0)
                    {
                        Console.WriteLine($"You triggered the help display by not submitting any arguments, so printing it and quitting.");
                        Usage();
                        Environment.Exit(0);
                    }

                    await AsyncMain(argset.Trim().Split(' '));
                }
            }
            else
            {
                if (args[0] == "-lookup")
                {
                    //do lookup of what is in the image?
                    var a = 4;
                }
                else
                {
                    await AsyncMain(args);
                }
            }
            Console.ReadLine();
        }

        static async Task AsyncMain(string[] args)
        {
            var tasks = new List<Task>();
            var throttler = new SemaphoreSlim(10, 10);

            var sz = ImageSize._1792x1024;

            //if n>1, and you also use curly brackets, then this
            //will make us skip the failed bad prompts, but not skip the 
            //other ones in the group which might actually be okay.

            var optionsModel = Statics.Parse(args);
            if (optionsModel == null)
            {
                Statics.Logger.Log("problem with input.");
                Usage();
                Console.ReadLine();
                Environment.Exit(0);
            }

            var api = new OpenAI_API.OpenAIAPI(Statics.ApiKey);

            var start = DateTime.Now;
            var billingBreak = false;

            for (var ii = 0; ii < optionsModel.ImageNumber; ii++)
            {
                if (billingBreak) { break; }
                //general obey rate limit.
                await throttler.WaitAsync();

                var taskId = ii + 10000;
                IEnumerable<InternalTextSection> textSections;
                //we do this so we can track the results of each individual section by punishing them later if there is a rejection..

                if (optionsModel.Random)
                {
                    textSections = optionsModel.PromptSections.Select(el => el.Sample()).ToList();
                }
                else
                {
                    try
                    {
                        textSections = optionsModel.PromptSections.Select(el => el.Next()).ToList();
                    }
                    catch (IterException ex)
                    {
                        Statics.Logger.Log($"One of the iterators ran out. System currently doesn't support more than a single instance of powerset or permutation section in non-random mode, since" +
                            $"we don't lockstep iterate, but rather just advance them all at once. {ex}");
                        break;
                    }

                }

                var req = new ImageGenerationRequest();
                req.Model = OpenAI_API.Models.Model.DALLE3;

                //the library magically returns null when the actual object is "standard" and you are using dalle3 for some reason.
                //so fix it here for tracking.
                req.Quality = optionsModel.Quality;

                req.Prompt = string.Join("", textSections.Select(el => el.L));
                req.Size = optionsModel.Size;
                var humanReadable = string.Join("_", textSections.Select(el => el.GetValueForHumanConsumption().Trim())).Replace(',', '_');

                var l = req.Prompt.Length;
                var displayedPromptLength = 100;
                var mm = Statics.GenerateMeaningfulSummaryOfChosenPromptOptions(optionsModel, textSections);

                Statics.Logger.Log($"Sending:\t\"{mm}");

                //during asyncification: actually, I was already doing things wrong, probably. This 
                // method is actually where the exceptions were popping out of, but like 100% of the time, the path from here to the big try catch below
                //was so fast, we'd probably always be there already by the time an exception was thrown.

                //the plan now: copy all the significant stuff from in there into this block, including exception handling. That way
                //each guy gets handled one by one, and the exceptions never pop up.
                //AND we can use semaphoreslim to manage it, smartly, so we can still jam up the outbound queue

                //okay so the idea now is that we have a "danger zone" which we enter.
                //we want to may tasks async so they can run past that point, but don't let them escape the trycatch
                //until we know they are done and can't at all be risky anymore.

                //okay, looking at how threads get used, a lot of this may be unsafe. Because apparently even if I try to stay single threaded,
                //and only use the async awaits to have the main thread work while others are waiting/downloading, there can apparently still be multiple coming in.
                //I shoudl test that. anyway, don't rely on this block being guarded.
                var task = Task.Run(async () =>
                {
                    //var req2 = new CompletionRequest();
                    //req2.
                    //api.Completions.CreateCompletionAsync()
                    //req2.
                    //api.ImageGenerations.CreateImageAsync(req2);
                    //client.DownloadProgressChanged += (sender, e) => DownloadProgressHappened(sender, e, tempFp, fp, req.Prompt);

                    //this is the final destination; in actuality we will temporarily store them up one folder!
                    //also we reserve this location with something.
                    var destFp = Statics.PromptToDestFpWithReservation(req, humanReadable, taskId);
                    var tempFp = $"{Path.GetTempPath()}{taskId}.png";

                    try
                    {
                        optionsModel.IncStr("RequestedCount");
                        var res = await api.ImageGenerations.CreateImageAsync(req);
                        using (WebClient client = new WebClient())
                        {
                            if (req.Prompt.Length > 4000)
                            {
                                Statics.Logger.Log($"Prompt too long, truncating. Cut {req.Prompt.Length - 4000} \"...{req.Prompt.Substring(4000, req.Prompt.Length - 4000)}\"");
                                optionsModel.IncStr("TooLong");
                                req.Prompt = req.Prompt.Substring(0, 4000);
                                return;
                            }

                            var url = res.Data[0].Url;
                            var revisedPrompt = res.Data[0].RevisedPrompt;

                            client.DownloadFileCompleted += (sender, e) => Getter.DownloadCompleted(sender, e, optionsModel, tempFp, destFp, req.Prompt, revisedPrompt, textSections);

                            client.DownloadFileAsync(new Uri(url), tempFp);
                            //Statics.Logger.Log($"Generated image for the complete prompt: {Statics.GenerateMeaningfulSummaryOfChosenPromptOptinos(optionsModel, textSections)}");
                        }
                    }

                    catch (Exception ex) //what this really means is, this particular set of ITS derived from optionsModel.PromptSections didn't work.  They each know their parent (or they are the core ITS?)
                    {
                        //wait a minute, by the time we get here, how do we know AT ALL that these fake section => Parent pointers objects all line up regarding the actual text that was used?
                        //actually, it's good we use the child temporary ITS since that, at least, isn't duplicated. So even if parent's "CurrentN" has moved on, it doesn't hurt us.
                        optionsModel.IncStr("Error");

                        billingBreak = await HandleOpenAIException(optionsModel, ex, req, destFp, textSections);
                        DoReport(optionsModel);
                    }
                    finally
                    {
                        throttler.Release();
                        
                    }
                });
                tasks.Add(task);
                var got = optionsModel.Results.TryGetValue("DownloadedCount", out var n);

                if (got && n >= 300)
                {
                    Statics.Logger.Log($"break early due to generating {n}. =================.");
                    break;
                }

            }

            await Task.WhenAll(tasks);
            Statics.Logger.Log("\r\n===============FinalReport=============\r\n");
            DoReport(optionsModel);
        }
    }
}