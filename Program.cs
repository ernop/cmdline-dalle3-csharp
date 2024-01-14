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
            if (locl)
            {
                var inp = "A chonky mutant {cassowary,petroglyph,octopus} spaceship floating above the {rainbow, Lightning} surface of jupiter shooting cats eye lasers at aliens and floating and disturbing the wild red misty stone and dust rings";
                //inp = "a single kanji character for da \"big\", in {golden ink, purple ink, rainbow ink, blue ink, black ink}";
                //inp = "{watercolor, illustration, pointillism} of {boy, woman, man} in {mystery, romance, science fiction} set {mountains, ocean, desert}. -r";
                inp = "{watercolor, illustration, pointillism} of {boy, woman, man} in {mystery, romance, science fiction} set {mountains, ocean, desert}. -r";
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
                var modelOpptions = Statics.Parse(args);
                if (modelOpptions == null)
                {
                    Console.WriteLine("problem with input");
                    Usage();
                    return;
                }
                var list = new List<Task<bool>>();
                for (var ii = 0; ii < modelOpptions.ImageNumber; ii++)
                {
                    Console.WriteLine($"There are: {modelOpptions.EffectivePrompts.Count} prompts");
                    foreach (var subPrompt in modelOpptions.EffectivePrompts)
                    {
                        if (badPrompts.IndexOf(subPrompt) != -1)
                        {
                            Console.WriteLine($"Skipping: {badPrompts} {ii}");
                            continue;
                        }
                        GenerateOneImageAsync(api, subPrompt, sz, quality);
                        actuallyGeneratedCount++;
                        
                        if (actuallyGeneratedCount >= 100)
                        {
                            Console.WriteLine("break early.");
                            break;
                        }
                    }

                    //we are rate limited at 7/min so we should wait a little bit longer than that.
                    var amt = 1000 * (60 / 8.0);
                    Console.WriteLine($"sleeping: {amt/1000}");
                    System.Threading.Thread.Sleep((int)amt);
                }
                Console.WriteLine("Waiting now.");
                Console.ReadLine();
            }

            Usage();
        }

        /// <summary>
        /// {A,B,C} {blue,black,golden} {ivory,furry,squares} {3d,2d,calligraphy} on a clear blank white background
        /// Dalle3.exe A {Joyful, Solemn, Hopeful} {Alien, Human, Flower} in {Watercolor, Illustration, Photograph} around {Rainbow, Lightning, Fog}, with a clear white background -r
        /// </summary>
        static string SubstituteExpansionsIntoPrompt(string prompt)
        {
            var expansions = new List<Expando>() {
                new Expando("A","A single letter \"A\""),
                new Expando("B","A single letter \"B\""),
                new Expando("C","A single letter \"C\""),

                new Expando("blue","written in deep clear sharp blue ink, carefully written and drawn"),
                new Expando("black","written in black ink"),
                new Expando("golden","written in gold ink"),

                new Expando("alive","very simple normal form of a letter, with just one or two hidden tiny eyes, and one tiny claw or other subtle signs of life"),
                new Expando("furry", "covered with a short, fine fur, which waves and flows from the edges."),
                new Expando("squares","made of tiny squares stacked in rows"),

                new Expando("3d","as a 3d rendered protuding non-flat object viewed at an angle in pointillism style"),
                new Expando("2d","drawn in a completely flat comic side view style, in papyrus font, these are just shapes and lines in watercolor style"),
                new Expando("calligraphy","done in completely flat hand-written traditional calligraphy"),

                new Expando("Rainbow","in a glorious rainbow"),
                new Expando("Lightning","in a lightning storm"),
                new Expando("Fog","in a very foggy environment"),

                //{watercolor, illustration, photograph} of {boy, woman, man} in {mystery, romance, science fiction} set {mountains, ocean, desert}. -r
                new Expando("Boy","the main character is a young boy who loves the outdoors and lives in the wilds, ragged hair, a cute pet puppy, chewing in a thread of wheat"),
                new Expando("Woman","the main character is a young irish woman about 32 years old with long red hair and blue eyes in a modest white dress, perseverance, normal height, black shoes, lovely, with a dove flying overhead"),
                new Expando("Man","the main character is a fierce old thin argentinian man with a black moustache in his old green 1800s army uniform and medals standing proudly."),

                new Expando("Mountains","Up in the mountains near the matterhorn below a crescent moon in the dusk"),
                new Expando("Ocean","Next to an abandoned lush ocean beach and overgrown tropic cliffs with a distant getaway boat visible at dawn"),
                new Expando("Desert","In a barren, rocky desert similar to death valley with a full moon visible in the daytime sky"),

                new Expando("Mystery","A dramatic mystery scene showing the main character chasing a small villianous raccoon carrying a bag of gold"),
                new Expando("Science Fiction","a science fiction scene where the main character is climing up an arcology under a large sky where a tiny futuristic alien ship is approaching"),
                new Expando("Romance","a peaceful romance story showing the love of the main character for his large black horse with a plaid red blanket on its back and a flowing black mane, looking wild."),

                new Expando("Watercolor","an abstract sloppy watercolor with mottled blotchy color"),
                new Expando("Illustration","a hand-drawn colored pencil illustration, detailed bright and clear, with shading and details"),
                new Expando("Pointillism","a pointillism painting in amazingly detailed but clear dotted style with small dots and round"),
            };
            foreach (var e in expansions)
            {
                var key = $"AAA{e.S}AAA";
                if (prompt.ToLower().Contains(key.ToLower()))
                {
                    var usingValue = e.L;
                    if (string.IsNullOrEmpty(e.L))
                    {
                        usingValue = key;
                    }
                    prompt = prompt.ToLower().Replace(key.ToLower(), usingValue.ToLower());
                }
            }
            return prompt;
        }

        static bool GenerateOneImageAsync(OpenAI_API.OpenAIAPI api, string prompt, ImageSize s, string quality)
        {
            var req = new ImageGenerationRequest();
            req.Model = OpenAI_API.Models.Model.DALLE3;
            req.Quality = quality;
            req.Prompt = SubstituteExpansionsIntoPrompt(prompt);
            req.Size = s;

            Console.WriteLine($"Sending to imagemaker: \"{req.Prompt}\"");
            var res = api.ImageGenerations.CreateImageAsync(req);

            //obviously this is terrible.
            req.Prompt = prompt.Replace("AAA", "");
            var tries = 0;
            var outfn = Statics.PromptToFilename(req, tries);

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
