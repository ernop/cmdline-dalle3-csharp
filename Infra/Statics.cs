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

        public static IEnumerable<string> OverridePromptsForTesting
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
                inp = "{GPTProtagonists}, {GPTProtagonists}, and {GPTProtagonists} on the left team up to fight {GPTForts} which is on the right in the distance. -h -hd -20";
                inp = "-h -hd -30 a small golden {GPTProtagonists}, a silver {GPTProtagonists}, and a stone {GPTProtagonists} are resting on a windowsill in {GPTLocations} as a beautiful girl prepares to go out.";
                inp = "-h -hd -20 A serene, sunlit consultation room where a group of hopeful parents are engaged in a discussion with a compassionate, knowledgeable Doctor. " +
                    "On the wall are various images showcasing the top-ranked alleles and genes available via CRISPR or IVF PGS, each highlighted for their " +
                    "exceptional success rates, innovative lifestyle improvements, and amazing health benefits. The scene is one of hope and assurance, " +
                    "encapsulating the journey from uncertainty to the joy of potential parenthood. Outside the window is a view of a transformed, amazing solarpunk close-up scene of " +
                    "intense tall buildings, rich technological transformations of the natural world, grand-scale achievements in physics, learning and mathematics, and just plain fun. " +
                    "All this is available due to increased high potential human population" +
                    "The single word \"Hope\" is prominently displayed on the screen, symbolizing the core of this transformative journey. " +
                    "This image embodies the profound impact of the initiative on families around the globe, depicting a future where advanced " +
                    "reproductive technologies are accessible, transparent, and widely trusted, leading to the birth creation of thousands of " +
                    "well-loved children and a significant improvement in the quality and accessibility of reproductive healthcare." +
                    " ";
                //inp = "-h -hd -10 Imagine a bustling urban clinic, with walls painted in calming shades of blue and green, where a diverse group of healthcare professionals are engaged in a groundbreaking task. In the center, a compassionate nurse, radiating hope and expertise, uses a common smartphone to perform a pupillary light reflex test on an elderly patient seated comfortably in a well-lit, welcoming examination room. The smartphone screen displays a simple, user-friendly interface with live feedback on the patient's pupillary response. Around them, other patients wait their turn, watching with a mix of curiosity and optimism. The background is adorned with posters highlighting the importance of early diagnosis in neurological conditions. The clinic is filled with people from various walks of life, illustrating the wide-reaching applicability of this technology. The atmosphere is one of quiet revolution, as this simple tool makes advanced medical diagnostics accessible to all, transforming the landscape of neurological healthcare. The word \"Accessibility\" is prominently displayed in the corner of the image, serving as a powerful testament to the essence of this innovation. This image encapsulates not just the technological breakthrough but its profound impact on society, making critical health services available and affordable to communities worldwide, especially in underserved areas.";
                //inp = "-h -hd -10 Based on the scenario provided, imagine a successful outcome of Mark Webb's innovative approach to land reform, focusing on direct land purchases to prove the concept and attract larger funders. The image to encapsulate this scenario would depict a transformed rural landscape, showing small-scale farmers thriving due to the newly accessible land. A key element of this transformation would be the visible impact of sustainable farming practices, improved livelihoods, and community empowerment. The focal point would be a vibrant, green farm with a group of farmers working together, showcasing the newfound productivity and cooperation among the community. In the background, a before-and-after comparison could illustrate the stark contrast between the previously underutilized land and the flourishing farms post-reform. The central theme, encapsulated in a single word within the image, would be \"Empowerment,\" highlighting the core achievement of the initiative. This word would serve as a simple explanation of the profound change brought about by the project, emphasizing how direct action and innovative thinking have led to tangible benefits for the people and the environment.";
                //inp = "-h -hd -10 Illustrate a vibrant, modern educational campus in Fumba, Zanzibar, bustling with students and teachers engaged in learning and innovative projects. The architecture should reflect a blend of modern and local styles, set against the picturesque backdrop of Zanzibar. Include elements such as technology, books, and collaborative spaces to emphasize the campus's role in education and market development. The scene should capture the essence of 'Transformation,' showcasing how the project changes lives and contributes to economic growth in Africa, without using any words.";
                //inp = "-h -hd -10";
                //inp = "-h -hd -10";
                //inp = "-h -hd -10";
                //inp = "-h -hd -10 Imagine a world transformed by Marcin Kowrygo's Far Out Initiative, where his team has successfully harnessed the unique DNA" +
                //    "found in a woman from Scotland. They've created groundbreaking pharmacological and genetic interventions that mimic her condition, " +
                //    "eliminating physical and psychological suffering. Picture a serene and vibrant community, where the fruits of this discovery are visible " +
                //    "in every facet of life. People are interacting joyfully, free from the burden of unhappiness, in environments ranging from bustling city streets " +
                //    "to tranquil rural settings. Animals on farms live comfortably, contributing to a revolution in the production of cruelty-free meat. " +
                //    "The essence of this new world is captured in the word \"Freedom\", depicted in the scene without any additional text. " +
                //    "This image symbolizes the monumental shift in human and animal well-being, showcasing the profound impact of ending suffering and " +
                //    "enhancing the quality of life on a global scale.";
                //inp = "-h -hd -10 Now, to encapsulate this transformative journey in a visual narrative, consider an image that captures the essence of hope and renewal. Picture a vibrant, healthy tree growing in an urban setting, its branches extending over a diverse group of people gathered underneath—doctors, recipients, and donors alike, all united by their shared goal. This tree, flourishing against the backdrop of a city skyline, symbolizes the life-giving impact of the new legislation. The leaves of the tree are subtly shaped like kidneys, representing the specific focus on kidney donation, while the people underneath are depicted in a moment of celebration and gratitude. The city around them reflects a community that has been revitalized by the act, with hospitals and public spaces showing signs of vibrant health and well-being. The word \"Renewal\" is integrated into the image as a single, powerful expression of the scenario's essence, signifying the new beginnings made possible through the collective effort to change organ donation laws. This image conveys not just the successful enactment of the End Kidney Deaths Act but also its profound effect on individuals' lives and society as a whole, highlighting a future where organ shortages are a thing of the past, thanks to legislative action and community support.";
                //inp = "-h -hd -10 Illustrate a transformative evening in a neighborhood that was once plagued by mosquitoes but is now vibrant with outdoor life, thanks to the deployment of innovative anti-mosquito drones. The scene is set at dusk, with the drone actively using its glowing sonar to locate and eliminate mosquitoes, symbolizing 'Liberation' from the threat of mosquito-borne diseases. Families are seen enjoying their newfound freedom in the background, engaging in various outdoor activities, celebrating the success of this ambitious project.";
                //inp = "-h -hd -10 Imagine a bustling public space, perhaps a busy airport terminal or a crowded conference hall, where people from all walks of life come together, unhampered by the fear of airborne diseases. Above them, sleek, modern UV-C light fixtures emit a soft, safe glow, invisible to the eye but powerful in its purpose. These lights are not just ordinary fixtures; they are the culmination of Blueprint Biosecurity's pioneering research into germicidal far-UV-C technology, a beacon of hope in the fight against respiratory pandemics. In this scene, the air is crisp and clear, free of the invisible menace of germs thanks to the advanced ozone scrubbers seamlessly integrated into the light fixtures. These scrubbers effectively neutralize any ozone byproduct, ensuring the air remains safe and breathable while the UV-C technology works its magic, eradicating airborne pathogens before they can pose a threat to human health. The people below these lights, diverse in age, appearance, and background, share a common trait: a sense of security and normalcy. They engage freely, without masks or social distancing, in what was once considered a high-risk environment. Their interactions are unburdened by the fear of contagion, thanks to the invisible shield provided by the far-UV-C lights above. In the background, a simple plaque or digital display proudly bears the Blueprint Biosecurity logo, a subtle nod to the scientific innovation and perseverance that brought this vision to life. It's a testament to the successful resolution of one of the final hurdles in making UV-C technology a safe, universal solution for public health: the elimination of ozone-related risks. The word \"Protected\" is prominently featured in the scene, encapsulating the essence of this new reality. It serves as a simple yet powerful explanation of the transformative impact of far-UV-C technology: a world where people are shielded from airborne diseases, where public spaces are safe havens, and where the threat of respiratory pandemics is effectively neutralized. This image is a glimpse into a future where science and innovation have paved the way for a healthier, more connected world, free from the fear of the unseen enemies that once kept us apart. It's a vision of hope, resilience, and the triumph of human ingenuity over the challenges of our time.";
                //inp = "-h -hd -10 Imagine a future where Robert Yaman's investment of $100,000 into Innovate Animal Ag has led to a groundbreaking advancement in animal welfare, specifically targeting the egg industry's challenge of unwanted male chicks. This success story is visualized through a serene and hopeful farm scene, bathed in the golden light of dawn. The technology, 'in ovo sexing', is at the heart of this transformation, allowing farmers to determine the sex of a chick while it's still in the egg, thereby avoiding the need to hatch and subsequently cull billions of male chicks each year. The image captures the essence of this humane and sustainable approach to farming, with a focus on a farmer holding a device that gently scans an egg, surrounded by healthy, well-cared-for hens in a spacious, clean barn. The atmosphere is one of harmony and respect between humans and animals, symbolizing a leap forward in ethical farming practices. In the background, the farm extends into a lush, verdant landscape, suggesting the wider positive environmental impact of this innovation. The word 'Compassion' is prominently featured in the scene, serving as a simple yet powerful explanation of the driving force behind this change, highlighting the profound effect on animal welfare, farmer's lives, and the agricultural industry at large. This image tells a story of hope, innovation, and the profound impact of ethical advancements on the world.";
                var prompts = new List<string>()
                {
                    "-5 -h -hd a cute widdle kiddy {tat, cat}",
                    "-3 -hd -v a megadog in mega opposite world",
                };
                foreach (var prompt in prompts)
                {
                    yield return prompt;
                }
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
                var fp = $"d:/proj/dalle3/output/{outFn.Replace(".png", $"_{tries}.png")}";
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

        public static IEnumerable<IEnumerable<InternalTextSection>> IteratePowerSet(IEnumerable<InternalTextSection> items, int minElements = 0, int maxElements = int.MaxValue, int skip = 0, bool randomize = false)
        {
            while (true)
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

                    if (randomize)
                    {
                        subset = subset.OrderBy(el => Statics.Random.Next()).ToList();
                    }

                    if (subset.Count >= minElements)
                    {
                        subset = subset.Take(maxElements).ToList();
                        yield return subset;
                        //var newInternal = new InternalTextSection(string.Join(", ", subset.Select(el => el.L)), string.Join(", ", subset.Select(el => el.L)), true, null);
                        //yield return newInternal;
                        break;
                    }
                }
            }
        }

        public static IEnumerable<InternalTextSection> PickRandomPowersetValue(IEnumerable<InternalTextSection> items)
        {
            var num = Random.Next(0, 1 << items.Count());
            var raw = IteratePowerSet(items, 0, int.MaxValue, num, true);
            //var raw = IteratePowerSet(items, 0, 4, num, true);
            var el = raw.First();
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

        public static void UpdateWithFilterResult(IEnumerable<IEnumerable<InternalTextSection>> ungroupedSections, TextChoiceResultEnum el)
        {
            ///So for example a powerset of [a,b,c] might send (a,c) here.
            foreach (var section in ungroupedSections)
            {
                foreach (var p in section)
                {
                    if (p.Parent == null)
                    {
                        continue;
                    }
                    p.Parent.ReceiveChoiceResult(p.S, el); ;
                }
            }
        }
    }
}
