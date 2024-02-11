using OpenAI_API.Images;
using OpenAI_API.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                inp = "a minimalistic image combining [lightning, lava, lasers, dinosaurs, volcanos, magma, explosions, constellations, waterfalls," +
                    "ice walls, glaciers, numinosity, cumulonimbus, thunderstorms, ball lightning, will'o'the wisps, horsemen, wild horses, cliffs, " +
                    "barren, plains, walls, arrow slits, towers, grandiosity, naturalness, minimalism, chiaroscuro, opacity, translucency, " +
                    "symbolism, adonis, statues, monumental architecture, brutalism, naturalism, spheres, aliens, ocean vessels, kayaks, ice floes, icebergs, " +
                    "sheer cliffs, crevasses, natural disasters, tornados, waterspouts, milky way galaxy, galaxies, andromeda, black dwarves, dwarf horses, venus fly traps," +
                    "carnivorous plants, ice cream,] -hd -h -40";
                //inp = "a minimalistic image combining {lightning, lava, lasers, dinosaurs, volcanos, magma, explosions, constellations, waterfalls," +
                //    "ice walls, glaciers, numinosity, cumulonimbus, thunderstorms, ball lightning, will'o'the wisps, horsemen, wild horses, cliffs, " +
                //    "barren, plains, walls, arrow slits, towers, grandiosity, naturalness, minimalism, chiaroscuro, opacity, translucency, " +
                //    "symbolism, adonis, statues, monumental architecture, brutalism, naturalism, spheres, aliens, ocean vessels, kayaks, ice floes, icebergs, " +
                //    "sheer cliffs, crevasses, natural disasters, tornados, waterspouts, milky way galaxy, galaxies, andromeda, black dwarves, dwarf horses, venus fly traps," +
                //    "carnivorous plants, ice cream} {lightning, lava, lasers, dinosaurs, volcanos, magma, explosions, constellations, waterfalls, ice walls, glaciers, " +
                //    "numinosity, cumulonimbus, thunderstorms, ball lightning, will'o'the wisps, horsemen, wild horses, cliffs, barren, plains, walls, arrow slits, " +
                //    "towers, grandiosity, naturalness, minimalism, chiaroscuro, opacity, translucency, symbolism, adonis, statues, monumental architecture, " +
                //    "brutalism, naturalism, spheres, aliens, ocean vessels, kayaks, ice floes, icebergs, sheer cliffs, crevasses, natural disasters, " +
                //    "tornados, waterspouts, milky way galaxy, galaxies, andromeda, black dwarves, dwarf horses, venus fly traps,carnivorous plants, ice cream} " +
                //    "{lightning, lava, lasers, dinosaurs, volcanos, magma, explosions, constellations, waterfalls,ice walls, glaciers, numinosity, cumulonimbus," +
                //    " thunderstorms, ball lightning, will'o'the wisps, horsemen, wild horses, cliffs, barren, plains, walls, arrow slits, towers, grandiosity, " +
                //    "naturalness, minimalism, chiaroscuro, opacity, translucency, symbolism, adonis, statues, monumental architecture, brutalism, naturalism, " +
                //    "spheres, aliens, ocean vessels, kayaks, ice floes, icebergs, sheer cliffs, crevasses, natural disasters, tornados, waterspouts, milky way galaxy," +
                //    " galaxies, andromeda, black dwarves, dwarf horses, venus fly traps,carnivorous plants, ice cream} {lightning, lava, lasers, dinosaurs, volcanos, magma, " +
                //    "explosions, constellations, waterfalls,ice walls, glaciers, numinosity, cumulonimbus, thunderstorms, ball lightning, will'o'the wisps, horsemen, wild horses, " +
                //    "cliffs, barren, plains, walls, arrow slits, towers, grandiosity, naturalness, minimalism, chiaroscuro, opacity, translucency, symbolism, adonis, statues,"+
                //    "monumental architecture, brutalism, naturalism, spheres, aliens, ocean vessels, kayaks, ice floes, icebergs, sheer cliffs, crevasses, natural disasters, "+
                //    "tornados, waterspouts, milky way galaxy, galaxies, andromeda, black dwarves, dwarf horses, venus fly traps,carnivorous plants, ice cream} -hd -h -40";
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

            //optionsModel.RawPrompt = optionsModel.RawPrompt.Trim();

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
                var fp = $"d:/proj/dalle3/output/{outFn.Replace(".png",$"_{tries}.png")}";
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

        public static IEnumerable<InternalTextSection> IteratePowerSet(IEnumerable<InternalTextSection> items, int minElements = 0, int maxElements = int.MaxValue, int skip = 0, bool randomize = false)
        {
            int lastIndex = 1 << items.Count();

            for (int index = skip; index < lastIndex; index++)
            {
                var subset = new List<InternalTextSection>();
                for (int ii = 0; ii < items.Count(); ii++)
                {
                    if ((index & (1 << ii)) != 0)
                    {
                        subset.Add(items.Skip(ii).First());
                    }
                }
                if (subset.Count < minElements || subset.Count > maxElements)
                {
                    continue;
                }

                if (randomize)
                {
                    subset = subset.OrderBy(el=>Statics.Random.Next()).ToList();
                }
                
                var newInternal = new InternalTextSection(string.Join(", ", subset.Select(el => el.L)), string.Join(" ", subset.Select(el => el.L)), true, null);
                yield return newInternal;
            }
        }

        public static InternalTextSection PickRandomPowersetValue(IEnumerable<InternalTextSection> items)
        {
            var num = Random.Next(0, 1 << items.Count());
            var el = IteratePowerSet(items, 0, int.MaxValue, num, true).First();
            return el;
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

        public static void UpdateWithFilterResult(IEnumerable<InternalTextSection> sections, TextChoiceResultEnum el)
        {
            foreach (var section in sections)
            {
                if (section.Parent == null)
                {
                    continue;
                }
                section.Parent.ReceiveChoiceResult(section.S, el); ;
            }
        }
    }
}
