using OpenAI_API.Images;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

        public static void Usage()
        {
            Console.WriteLine("Dalle3.exe [-N] [-r] [-h|v] [-hd] [prompt]\r\ndalle3.exe A very {big,blue,tall} photo of a {tall,small} {cat,dog,mouse monster}\r\nN=number of times to repeat prompt. Will die if any fail. Prompt can be multiple words with no quotes required, but no newlines." +
                "\r\n{}=run all permutations of the items within here. This can blow up your api limits." +
                "\r\n-r output items in random order. default or missing, will output in permutation order." +
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
                    Usage();
                    Environment.Exit(0);
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
                if (s[0] == '-') //fallthrough.
                {
                    Statics.Logger.Log($"Unknown option: {s}");
                    Usage();
                    return null;
                }

                optionsModel.RawPrompt += " " + s;
            }
            optionsModel.RawPrompt = optionsModel.RawPrompt.Trim();

            if (string.IsNullOrEmpty(optionsModel.RawPrompt)) { return null; }

            //locations where women would shine and be thrilled to be admired and receive attention from powerful men
            //so they work to be beautiful and attractive and to be noticed and to be the center of attention
            var excitingLocations = new Blowup("{ExcitingLocations}", "{" +
                "Inside a very elite nightclub," +
                "At the emperor's court," +
                "At a holy women's temple ceremony in ancient babylonia," +
                "At a fashion show in Milan," +
                "At Bondi Beach in Sunny australia on Christmas day," +
                "In the VIP section at a high-profile Hollywood movie premiere," +
                "On a luxurious yacht cruising the French Riviera," +
                "At a celebrity-filled gala in New York City," +
                "In the royal gardens during a grand ball in Versailles," +
                "Front row at Paris Fashion Week," +
                "At an exclusive art gallery opening in London," +
                "On the red carpet at the Cannes Film Festival," +
                "In a box at the Vienna Opera Ball," +
                "In a warm ski lodge at Vail," +
                "In a guest room at the Vatican City," +
                "In a castle's dungeon but the nice part for guests," +
                "In a real new york pizza joint with Ray," +
                "At a late-night dance club in Barbados," +
                "In a Bachata competition in Mexico City," +
                "At a samba school parade in Rio de Janeiro," +
                "In an upscale Las Vegas casino lounge," +
                "At a famous Ibiza beach party," +
                "At the Carnival festivities in Venice," +
                "Inside a high-end exclusive nightclub with a reputation for glamorous parties and starlets," +
                "At the lavish and opulent court of a renowned emperor where beauty reigns," +
                "Participating in a mystical and enchanting temple ceremony in ancient Babylonia," +
                "Walking the runway at an avant-garde Milan fashion show," +
                "Mingling at a sophisticated and powerful after-party following a major UN convention," +
                "Basking in the sun at Bondi Beach Australia amidst a Christmas day celebration," +
                "Rubbing shoulders with celebrities at a star-studded Hollywood movie premiere," +
                "Sipping champagne on a sumptuous yacht cruising along the glittering French Riviera," +
                "Dazzling the crowd at an exclusive high-society gala in New York City," +
                "Waltzing in the moonlit royal gardens during a magnificent ball at Versailles," +
                "Captivating onlookers from the front row during the electrifying Paris Fashion Week," +
                "At the center of attention at a prestigious art gallery opening in the heart of London," +
                "Turning heads on the red carpet at the glamorous Cannes Film Festival," +
                "Adorned in elegance in a box at the enchanting Vienna Opera Ball," +
                "Wiling the night away at a rhythm-filled late-night club in Barbados," +
                "Swaying during a passionate Bachata competition in Mexico City," +
                "Shining bright in the midst of a samba parade during the vibrant Rio de Janeiro Carnival," +
                "Flaunting allure and grace in an upscale neon-lit Las Vegas casino lounge," +
                "Igniting the scene at a world-famous electrifying beach party in Ibiza," +
                "Captivating masked revelers during the mysterious and alluring Venice Carnival," +
                "Stealing the spotlight at a luxurious rooftop pool party in downtown Dubai," +
                "In the heart of Seoul at an exclusive K-pop dancing party known for its fashion and flair," +
                "At an opulent Moroccan riad during a star-lit soiree" +
            "}");

            var origins = new Blowup("{GPTOrigins}", "{Abkhazian Abkhazia,Acehnese Indonesian,Afghan,Ainu Indigenous people of Japan,Ainu,Akan Ghana,Albanian,Algerian,Ambonese Indonesian,Amhara,Amsterdam Netherlands,Andalusian Spain,Andorran,Aragonese Spain,Armenian,Ashanti Ghana,Ashkenazi Jewish,Assyrian,Asturian Spain,Athens Greece,Auckland New Zealand,Austrian,Avar,Azerbaijani,Bali Indonesia,Balinese Indonesian,Bamileke Cameroon,Bangkok Thailand,Bangladeshi,Banjar Indonesian,Barcelona Spain,Basajaun Basque mythology,Bashkir,Basque Basque Country,Basque,Batak Indonesian,Beijinger,Beirut Lebanon,Belgian,Bengali,Berber,Berlin Germany,Bhutanese,Bosnian Bosnia and Herzegovina,Breton,British,Bruneian,Budapest Hungary,Buenos Aires Argentina,Bugis Indonesian,Bulgarian,Burmese Myanmar,Burmese,Buryat,Cambodian,Cantabrian Spain,Cape Town South Africa,Castilian Spain,Catalan Catalonia,Catalan,Cebuano Filipino,Chamorro Guam,Chechen Russia,Chechen,China,Chinese,Chuvash,Circassian,Cook Islander,Copenhagen Denmark,Corsican,Croatian,Cypriot Cyprus,Czech,Dagestani Russia,Dagomba Ghana,Dai Chinese,Dayak Indonesian ,Dubai UAE,Dungan,Dutch Netherlands,Edinburgh Scotland,Egyptian,Emirati UAE,English,Ewe Ghana,Fang Central Africa\r\nFaroese Faroe Islands,Fijian,Filipino,France,France,French,Frisian,Fulani West Africa,Futunan Wallis and Futuna,Ga Ghana,Galician,Gallego Spain,Georgia,Georgian,German,Gibraltarian,Greek Cypriot Cyprus,Greek,Greenlandic Inuit,Gujarati,Hakka Chinese,Hakka Chinese,Han Chinese,Hani Chinese,Hausa West Africa,Havana Cuba,Hmong,Hui Chinese Muslim,Hungarian,Igbo,Ilocano Filipino,Indian,Indonesian,Ingush,Iranian,Iraqi,Irish,Israeli,Istanbul Turkey,Italian,Italy,Italy,Italy,Japanese,Javanese,Jordanian,Kabardian,Kabyle Algeria,Kalmyk,Kannada,Karelian,Kazakh Kazakhstan,Kazakh,Khakas,Kikuyu,Kirghiz,Kiribatian,Korean,Kosovar Kosovo,Kumyk,Kurdish,Kuwaiti,Kyrgyz Kyrgyzstan,Ladino,Laotian,Las Vegas USA,Lebanese,Lezgian,Li Chinese,Libyan,Liechtensteiner,Lisbon Portugal,London UK,Los Angeles USA,Luxembourger,Lyonnais Lyon,Maasai,Macedonian,Madrid Spain,Madurese Indonesian,Makassarese Indonesian,Malagasy Madagascar,Malayali,Malaysian,Maldivian,Maltese,Manchu Northeast China,Manchu,Mandinka West Africa,Maori New Zealand,Marathi,Mari,Marrakech Morocco,Marseillais Marseille,Marshallese,Melbourne Australia,Mexico City Mexico,Miami USA,Miao Chinese,Micronesian,Milan Italy,Min Chinese,Minangkabau Indonesian,Mizrahi Jewish,Moldovan,Mongolian,Mongolian,Montenegrin,Montreal Canada,Mordvin,Moroccan,Moscow Russia,Mumbai India,Murcian Spain,Nauruan,Navarrese Spain,Neapolitan Naples,Nepalese,New Orleans USA,New York City USA,Ni-Vanuatu Vanuatu,Niuean,North Korean,Northern Irish,Nubian,Occitan,Okinawan Japan,Omani,Oriya,Oromo,Ossetian North and South Ossetia,Ossetian,Pakistani,Palauan,Palestinian,Papua New Guinean,Paris France,Parisian France,Pashtun,Portuguese,Prague Czech Republic,Punjabi,Qatari,Riffian Rif, Morocco,Rio de Janeiro Brazil,Romani,Romanian,Rome Italy,Sami Northern Europe,Sami,Samoan,San Diego USA,San Francisco USA,Sardinian,Sasak Indonesian,Saudi Arabian,Scottish,Seoul South Korea,Sephardi Jewish,Serbian,Shanghainese,Siberian Russia,Sichuanese,Sicilian Sicily,Sicilian,Sindhi,Singaporean,Slovak,Slovenian,Solomon Islander,Somali,South Korean,Spain,Spain/France,Spaniard,Sri Lankan,Stockholm Sweden,Sundanese Indonesian,Swazi Eswatini,Swiss,Sydney Australia,Syrian,Sámi,Tagalog Filipino,Tahitian French Polynesia,Taiwanese,Tajik Tajikistan,Tamil,Tatar Russia,Tatar,Tel Aviv Israel,Telugu,Thai,Tibetan Tibet,Tibetan,Tigrayan,Timorese East Timor,Tokelauan,Tokyo Japan,Tongan,Toraja Indonesian,Tuareg Sahara,Tuareg,Tujia Chinese,Tunisian,Turkish Cypriot Northern Cyprus,Turkish,Turkmen Turkmenistan,Tuvaluan,Tuvinian,Udmurt,Uighur Xinjiang,Uyghur,Uzbek Uzbekistan,Valencian Spain,Vancouver Canada,Venetian Venice,Venice Italy,Vietnamese,Vietnamese,Wallisian Wallis and Futuna,Walloon,Warsaw Poland,Welsh,Wolof Senegal,Wu Chinese,Xhosa,Yakut,Yemeni,Yoruba,Zhuang Chinese,Zulu}");

            var locations = new Blowup("{GPTLocations}", "{" +
                "The Ruins of an Ancient City among the crumbling walls lost relics and symbols and abandoned empty streets of a once-great metropolis, " +
                "A Floating City in the Clouds with aerial platforms and suspended walkways that provide a unique three-dimensional space, " +
                "An Underwater City among submerged buildings and coral reefs with sea creatures and glowing hints of life and wear, " +
                "A Volcanic Crater where Lava flows and eruptions thunder nearby revealing glowing magma, " +
                "A Dense Misty Jungle where Visibility is limited and the dense unique foliage and flowers flora and fauna, " +
                "A Massive Moving Train with carriages on carriages led by a speeding gigantic oversized locomotive along incredible tracks along with a small raccoon detective," +
                "In the Tunnels of a Giant Ant Colony which is a A maze-like claustrophobic setting with numerous pathways," +
                "A Giant Ancient Library where Countless books and shelves hid hidden mystical or scientific knowledge beyond the ken of normal men inhabited by appropriately studious creatures, " +
                "An Abandoned Amusement Park with roller coasters and haunted houses and and carousels, " +
                "A Network of Ice Caves with Slippery surfaces and narrow passageways and a constant threat of collapse inhabited by ice denizens," +
                "0 BC Rome - Bustling streets of Ancient Rome with senators gladiators and grand architecture like the Colosseum, " +
                "Ancient Neanderthal Caves in Europe - Primitive cave dwellings adorned with early human paintings and artifacts," +
                "1200s Mayan Empire Temple Top - A vibrant Mayan temple amidst the dense jungle filled with intricate carvings and astronomical alignments," +
                "15th Century Forbidden City in Beijing - Majestic palaces and gardens showcasing Imperial China's grandeur," +
                "1940s Paris during WWII - War-torn yet resilient Paris with clandestine meetings and the spirit of resistance," +
                "Victorian London in the 1800s - Foggy streets horse-drawn carriages and the burgeoning industrial revolution," +
                "Ancient Egyptian Pyramids Giza circa 2500 BC - Monumental pyramids and the Sphinx with bustling construction and ancient rituals," +
                "Edo Period Tokyo (1603-1868) - Traditional Japanese architecture samurai warriors and vibrant street life," +
                "Viking Settlement in Scandinavia 9th Century - Rugged landscapes with longships mead halls and Nordic culture," +
                "Renaissance Florence in the 15th Century - Birthplace of the Renaissance filled with art innovation and political intrigue," +
                "1920s New York City during the Jazz Age - Skyscrapers rising speakeasies bustling and the Harlem Renaissance in full swing," +
                "Ancient Athens during the Golden Age (5th Century BC) - Philosophers orators and the Parthenon symbolizing the cradle of democracy," +
                "Mughal Empire in Delhi 17th Century - Opulent palaces and gardens epitomized by the Taj Mahal," +
                "Medieval Constantinople 10th Century - A crossroads of cultures with Byzantine and Ottoman influences," +
                "Aztec Capital Tenochtitlán 15th Century - An island city with intricate canals bustling markets and grand temples," +
                "18th Century Versailles during the French Monarchy - Extravagant palace life with opulent balls and intricate political plots," +
                "Ancient Carthage 3rd Century BC - A powerful Mediterranean port city with a mix of cultures and naval dominance," +
                "Gold Rush San Francisco 1850s - Rapid growth and diversity amidst the pursuit of fortune," +
                "Indus Valley Civilization 2500 BC in Mohenjo-Daro - Advanced urban planning and mysterious script in one of the world's earliest major cities," +
                "Sparta in the Classical Greek Period 5th Century BC - A city-state renowned for its military discipline and austere lifestyle, " +
                "A Desert Oasis with lush greenery and water springs amidst vast sand dunes sheltering diverse wildlife," +
                "A High-Speed Space Station orbiting a distant planet with futuristic technology and panoramic views of the cosmos," +
                "An Overgrown Ruined Castle shrouded in ivy and history echoes of ancient battles and forgotten tales," +
                "A Crystal Cavern sparkling with multi-colored gems and crystal formations reflecting light in dazzling patterns," +
                "A Skyborne Archipelago with floating islands connected by rope bridges harboring unique flora and fauna," +
                "A Deep-Sea Abyss where bioluminescent creatures and strange geological formations exist in perpetual darkness," +
                "A Futuristic Metropolis with towering skyscrapers neon lights and advanced technology teeming with life," +
                "A Wild West Ghost Town with dusty streets abandoned saloons and a sense of a time gone by," +
                "A Mystical Forest with enchanted trees whispering secrets magical beings and a sense of wonder," +
                "An Arctic Research Base isolated in a snowy expanse with cutting-edge facilities and a harsh climate.," +
                "The windswept plains of a post-apocalyptic Earth sorrowful yet simple.," +
                "An ancient ruin reclaimed by sand windswept yet retaining geometrical beauty.," +
                "An endless golden grain field beneath an alien yellow sky - solitude and freedom.," +
                "Wonderland from Alice in Wonderland - Curiosity and confusion in a nonsensical realm where magic mushrooms make you grow or shrink and time runs backward," +
                "The Shire from Lord of the Rings - Fear and despair in the bleak ashen plains surrounded by volcanoes where Sauron's eye watches endlessly.," +
                "Narnia from The Lion The Witch and the Wardrobe - Good triumphing over evil in a land watched over by a noble lion and currently cast under an endless winter.," +
                "An all-white limbo space influenced by The Matrix stripped-down yet disturbed.," +
                "Symmetrical brutalist structures from Equilibrium's monochromatic city perfection imposed by force.," +
                "Vast Zen rock gardens with raked patterns from memoirs of Japanese monks calming and centering.," +
                "An icy Watchtower like Game of Throne's Wall hardened shelter from risks unknown.," +
                "A precisely planned forest village resembling aesthetics from Princess Mononoke harmoniously balanced with nature.," +
                "On the side of a hill from a romantic nostalgic anime," +
                "Norse Gods at Ragnarok (Dread Bravery) - The ultimate battle with fate hanging in the balance.," +
                "Mythical Olympus at its Peak (Splendor Rivalry) - The home of the gods, resplendent but rife with rivalries.," +
                "A City on the Back of a Giant Moving Creature (Wanderlust Fear) - A nomadic city with the constant threat of the unknown.," +
                "Parallel Universe New York (Bewilderment Excitement) - Familiar yet bizarrely different, with endless possibilities.," +
                "Ghost Ship in the Bermuda Triangle (Mystery Fear) - A haunted vessel with an eerie and uncertain fate.," +
                "An Enchanted Forest with Warring Fae (Beauty Betrayal) - Ethereal landscapes shadowed by deceit and power struggles.," +
                "Middle Earth's Rivendell during War (Tranquility Tension) - A serene elven haven on the brink of war.," +
                "Atlantis Rising from the Sea (Wonder Awe) - The mythical city re-emerging, revealing ancient technologies and mysteries.," +
                "Timbuktu in the 14th Century - A thriving center of African scholarship and trade., " +
                "Ancient Babylon  6th Century BC - Hanging Gardens and grand ziggurats along the Euphrates River.," +
                "Machu Picchu in the 15th Century - Mysterious Incan city hidden high in the Andes Mountains.," +
                "Heian-kyo (Kyoto) during Japan's Heian Period - Elegant imperial courts and the flowering of Japanese culture.," +
                "19th Century St. Petersburg - Russian imperial splendor with grand canals and the Winter Palace.," +
                "Angkor Wat in the 12th Century  Cambodia - Majestic temples surrounded by dense jungle.," +
                "Viking Age Dublin  9th Century - A bustling Norse settlement and trading hub.," +
                "Venice in the Renaissance - Canals and gondolas  with a flourishing of arts and commerce.," +
                "Jerusalem during the Crusades - A city at the crossroads of religions and conflicts.," +
                "Han Dynasty China's Silk Road Cities - Exotic trade routes connecting East and West.," +
                "Istanbul during the Ottoman Empire - A melting pot of cultures  bridging Europe and Asia.," +
                "The Roaring Twenties in Chicago - Jazz  prohibition  and the rise of organized crime.," +
                "Baghdad during the Islamic Golden Age - Center of learning and culture with the House of Wisdom.," +
                "Pompeii just before the eruption of Vesuvius in 79 AD - Daily life in an ancient Roman city.," +
                "Ancient Athens at the time of Pericles - Flourishing arts and philosophy in the cradle of democracy.," +
                "Elizabethan London in the late 16th Century - The time of Shakespeare and the Globe Theatre.," +
                "The Hanseatic League in Medieval Lübeck - A rich and powerful merchant city-state.," +
                "Harlem during the Harlem Renaissance - A flourishing of African-American arts and culture.," +
                "Pre-Columbian Cusco  capital of the Inca Empire - Rich in culture and architecture.," +
                "Mohenjo-Daro in the Indus Valley Civilization  2500 BC - An advanced ancient city with intricate urban planning.," +
                "Ancient Alexandria in the Hellenistic Period - A center of learning and culture  home to the Great Library.," +
                "Teotihuacan  1st Century AD - Home to the Pyramid of the Sun  a pre-Columbian architectural wonder.," +
                "The Klondike Gold Rush  1890s in Yukon  Canada - Thrill and hardship in the search for gold.," +
                "The Height of the Ottoman Empire in 16th Century Istanbul - A cosmopolitan hub of the world.," +
                "Feudal Japan during the Time of Samurai - Castles  cherry blossoms  and warrior code.," +
                "Ancient Polynesian Settlements in Hawaii - Early navigators and unique island culture.," +
                "Colonial Williamsburg in the 18th Century - A living history of American colonial life.," +
                "The Grandeur of the Mughal Empire in 16th Century India - Exquisite architecture and rich culture," +
                "Ancient Rome at its Zenith under Emperor Trajan - A cosmopolitan empire at its peak," +
                "The Ming Dynasty's Forbidden City in 15th Century Beijing - The imperial palace complex at the heart of China," +
            "}");

            var awesomes = new Blowup("{AwesomeStyles}", "{Dungeons and Dragons, Pathfinder, Lamentations of the Flame Princess, " +
                "Dungeon Crawl Classics, Fantasy AGE, Warhammer Fantasy, " +
                "Palladium Fantasy, G.U.R.P.S, Basic Fantasy, Low Fantasy Gaming, " +
                "Vagabond, Tales of the Valiant, Cypher System, Savage Worlds, " +
                "RuneQuest, Ars Magica, Iron Kingdoms, Torchbearer, The One Ring, " +
                "Burning Wheel, Legend of the Five Rings, Fate, 13th Age, " +
                "Adventurer Conqueror King System, Forbidden Lands, Conan, OSR, " +
                "Fighting Fantasy, Tunnels and Trolls, Monsters Monsters, TTRPG, " +
                "EZD6, Index Card RPG, Dungeons of Drakkenheim}"
            );
            var gptStyles = new Blowup("{GPTStyles}", "" +
                "{" +
                //"The image features a dynamic composition with swirling lines and bright colors typical of the Expressionist movement evoking a sense of emotional turmoil, " +
                "A serene Impressionist painting, " +
                "A stark minimalist composition, " +
                "A vibrant pop art piece, " +
                "A digital artwork, " +
                "The charcoal drawing, " +
                "A Baroque-era oil painting, " +
                "a cubist collage using geometric shapes, " +
                "An Art Nouveau illustration, " +
                "A traditional Japanese woodblock print, " +
                "An icelandic etching, " +
                "A painted clay illustration, " +
                "An abstract watercolor illustration, " +
                "A dynamic abstract expressionist canvas, " +
                "A detailed Renaissance fresco, " +
                "A Gothic tapestry rich with allegory, " +
                //"A bold graffiti mural in a street art style, " +
                "A photorealistic graphite sketch, " +
                "A Rococo pastel portrait, " +
                "Comic book style image in the style of Tom Tomorrow, " +
                "Brilliant comic book style similar to that of transmetropolitain, " +
                "Drawing in the style of Matt Weurker, " +
                "Drawing in the style of dick sprang, " +
                "Drawing in the style of Paul Cesar Helleu, " +
                "Drawing in the style of Mark Shaw, " +
                "Drawing in Stark silhouette extreme chiaroscuro, " +
                "image made in cracked porcelain, " +
                "an image in the style of claire wendling engravings, " +
                "Zen ink wash painting, " +
                "Water-drip on paper image," +
                "Post-impressionist scene with vivid brushstrokes, " +
                "modernist sculpture, " +
                "retro-futuristic illustration, " +
                "vaporwave aesthetic scene," +
                "sultry Art Deco poster, " +
                //"An introspective Surrealist painting, " +
                "Vibrant Fauvist landscape with wild bold colors," +
                "Meticulous pointillist piece with tiny distinct dots, " +
                "Byzantine mosaic featuring rich golden tesserae, " +
                "Pre-Raphaelite oil painting with romantic themes, " +
                "Dada assemblage challenging conventional aesthetics," +
                "tranquil Thomas Cole-inspired Hudson River School landscape, " +
                "Constructivist design with industrial motifs," +
                "Bold Futurist painting capturing movement and speed, " +
                "}");

            var shapes = new Blowup("{GPTShapes}",
                "{ Circle, Triangle, Square, Rectangle, Pentagon, Hexagon, Octagon, Ellipse, Star, Heart}");
            var skies = new Blowup("{GPTSky}", 
                "{Supernovae, Black Holes, Auroras (Northern and Southern Lights), Comets, Solar Eclipses, Lunar Eclipses, Neutron Stars, Galactic Collisions, Pulsars, Planetary Transits}");
            var asianLocations = new Blowup("{GPTAsiaLocations}", "a rice field in summer with water filled and reflecting the infinite sky along narrow concrete paths, suburban tokyo, an industrial small town in the mountainous regions of honshu in central japan, yokohama in japan in a little izakaya as a server, a tokyo gigantic university library, the morning after in akahabara at 9am as people get back to work, a gritty overpass osaka in, medieval nara inside an entertainment venue for artisans down on theri luck, beijing at the important central city, the bund in shanghai, an electronics market in guangdong, chunking hotel in hong kong the scene of many hijinx, a secret military bunker and training school under seoul, the government\'s prepared government in exile centerpusan, the south korean countryside near the ocean, the island of jeju  which is famous as a place for newlyweds to spend time and fall deeper in love");
            var blowups = new List<Blowup>() {
                    awesomes,
                    gptStyles,
                    locations,
                    origins,
                    excitingLocations,
                    shapes,
                    skies,
                };

            var worked = false;
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

        public static class PermutationExpander
        {
            /// <summary>
            /// Expands a string with permutations defined in curly braces.
            /// Example: "A {red, yellow, purple} cat {and dog,and man}" => 
            /// ["A red cat and dog", "A yellow cat and dog", "A purple cat and dog", 
            /// "A red cat and man", "A yellow cat and man", "A purple cat and man"]
            /// WARNING: implementation is hacky with using magic string AAA as a guard which consumers of the code magically have to know to remove later.
            /// </summary>
            public static List<string> ExpandCurlyItems(string input)
            {
                var results = new List<string>();
                var match = Regex.Match(input, @"\{([^{}]*)\}");

                if (!match.Success)
                {
                    results.Add(input);
                    return results;
                }

                var chunk = match.Groups[1].Value;
                var parts = chunk.Split(',').Select(el => el.Trim());
                var target = $"{{{chunk}}}";
                foreach (var part in parts)
                {
                    var part2 = "AAA" + part + "AAA";
                    var replaced = ReplaceOnce(input, target, part2);
                    var subMatches = ExpandCurlyItems(replaced);
                    results.AddRange(subMatches);
                }
                return results;
            }
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
