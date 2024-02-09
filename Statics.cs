using OpenAI_API.Images;
using OpenAI_API.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static Dalle3.Substitutions;

namespace Dalle3
{
    internal static class Statics
    {
        public static Logger Logger = new Logger("../../logs/log.txt");
        public static string ApiKey { get; set; } = System.IO.File.ReadAllText("../../apikey.txt");
        public static string OrgId { get; set; } = System.IO.File.ReadAllText("../../organization.txt");

        public static string OverridePromptForTesting
        {
            get
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

                inp = "-h -hd -5 Meric Cattanay Proposes to Paint Griaule: This scene takes place in the lush, fertile valley of Teocinte, under the shadow of the massive dragon Griaule. Meric, a lanky young man with a shock of black hair, dressed in peasant's clothes, stands before the skeptical city fathers of Teocinte, gesturing passionately. The city fathers, a group of stern, sour-faced men, sit at a long table with a backdrop of a soot-smudged wall, symbolizing their shared burdens. Meric's eyes are wide with fervor as he unveils his audacious plan to paint the dragon as a means to poison and eventually kill it. The room is filled with tension, intrigue, and the weight of centuries of history between the people and the dragon.";
                //inp = "-h -hd -4 a cat in a hat.";
                
                return inp;
            }
        }

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

            var numre = new Regex(@"\-([\d]{1,2})");
            var count = 0;
            foreach (string s in args)
            {
                if (string.IsNullOrEmpty(s)) { continue; }
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
                if (s == "/help" || s == "/h" || s == "/?" || s == "-help" || s == "--help" || s == "-ayuda" || s == "--h")
                {
                    Console.WriteLine($"You triggered the help display by typing: {s}, so printing it and quitting.");
                    Usage();
                    Environment.Exit(0);
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
                if (s == "-r")
                {
                    optionsModel.Random = true;
                    continue;
                }

                if (s[0] != '-' && s[0] == '-') //fallthrough, although for sanity we should at least allow bare - since thats probably just part of the prompt.
                {
                    Statics.Logger.Log($"Unknown option: {s}");
                    Usage();
                    return null;
                }

                optionsModel.RawPrompt += " " + s;
            }

            if (!optionsModel.IncludeNormalImageOutput && !optionsModel.IncludeAnnotatedImageOutput)
            {
                Statics.Logger.Log("You have disabled both normal and annotated image output. There is nothing to do, so quitting.");
                Usage();
                System.Environment.Exit(0);
            }
            optionsModel.RawPrompt = optionsModel.RawPrompt.Trim();

            if (string.IsNullOrEmpty(optionsModel.RawPrompt)) { return null; }

            var worked = false;
            var blowups = Blowups.GetBlowups();
            while (true)
            {
                worked = false;
                foreach (var b in blowups)
                {
                    var key = b.Short;
                    if (optionsModel.RawPrompt.IndexOf(key) >= 0)
                    {
                        optionsModel.RawPrompt = Statics.ReplaceOnce(optionsModel.RawPrompt, key, b.Long);
                        Statics.Logger.Log($"Blew up prompt to: {optionsModel.RawPrompt}");
                        worked = true;
                    }
                }
                if (!worked) { break; }
            }

            //randomThing

            optionsModel.EffectivePrompts = PermutationExpander.ExpandCurlyItems(optionsModel.RawPrompt);
            if (optionsModel.Random)
            {
                Statics.Shuffle(optionsModel.EffectivePrompts);
            }
            return optionsModel;
        }

        public static string PromptToFilename(ImageGenerationRequest req)
        {
            var now = DateTime.Now;
            var usePrompt = req.Prompt.Replace("AAA", "");
            usePrompt = Regex.Replace(usePrompt, "[^a-zA-Z0-9]", "_");
            while (usePrompt.Contains("__"))
            {
                usePrompt = usePrompt.Replace("__", "_");
            }

            if (usePrompt.Length > 130)
            {
                usePrompt = usePrompt.Substring(0, 130);
            }
            var align = "";
            //var w = new ImageSize("1792x1024");
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
            var outfn = $"{usePrompt.Trim().TrimEnd('_')}-{now.Year}{now.Month:00}{now.Day:00}-{useQuality}-{align}.png";
            return outfn;
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
    }
}
