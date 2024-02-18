using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dalle3
{
    /// <summary>
    /// These are basically aliases you can use in a prompt if you want to force variation. They will obviously blow up your bill & the amount and time it'll take.
    /// e.g. add in {GPTLocations} at the end of your prompt with proper connecting words, and you'll get a bunch of copies of your prompt hopefully in that location!
    /// </summary>
    public static class Aliases
    {

        public static List<Alias> GetAliases()
        {
            var protagonists = new Alias("GPTProtagonists", "lightning, lava, lasers, dinosaurs, volcanos, magma, explosions, constellations, " +
                "waterfalls, melting ice walls, numinosity, cumulonimbus cloud banks, thunderstorms, ball lightning, will'o'the wisps, " +
                "disappearing horsemen, native wild horses, " +
                "cliffs, steep striped pyramids, a gigantic zero, a zeppelin, a jeep, " +
                "far future realistic aliens, ocean vessels, kayaks, ice floes, icebergs, " +
                "natural disasters, tornados, waterspouts, black dwarves, dwarf horses, " +
                "venus fly traps, carnivorous plants, ice cream, cybernetics, nanotech, fusion power, " +
                "holograms, antimatter, spaceships, chonky cats, yeti, loch ness monster, crypids, cryptocurrency, gold bars, barrels and crates, " +
                "alien relics, the sea elves, paladins, d&d artifacts, +5 greatswords, ancient dragons,  orchids, greenhouses, galactic empires, ringworlds, dyson spheres, " +
                "singularities, black holes, solar panels, teslas, drone haircut matchines, geodes, fulgurites, glow worms, sandworms, " +
                "hexapods, ant farms, butterflies, cats, shark teeth, sharks, moon rocks, stalactites, missiles, cluster munitions, bullets, fireballs, electricity, " +
                "DNA helices, princesses, knights,  " +
                "the word 'ROBLOX', the alphabet, a mega-kanji, kaiju, centipedes, alien kaiju,lich, " +
                "tsunamis,  beholder monsters, manatees, shattered spheres,  double rainbows, triple rainbows");

            var forts = new Alias("GPTForts",
                "zero-point energy, dark matter, plasma, monumental architecture, fort knox, the white house, the louvre, the golden temple of amritsar, the taj mahal, " +
                "brutalist architecture, giant curtain walls, arrow slits, a wizard's tower,supernovae, quasars, parallel universes, monumental glaciers, sheer cliffs, crevasses, " +
                "infinite stairways, oil rigs, a castle, moats, megaliths, arcologies,Hokkaido, " +
                "red square, mordor, narnia, the white witch, the washington monument, milky way galaxy, " +
                "galaxies, andromeda");

            ///okay this one is awesome.
            var compositionTypes = new Alias("GPTCompositions",
                "low horizon composition, high horizon composition, gradient following exploratory composition, minimalist sky composition, " +
                "rule of thirds composition with extreme offset, negative space composition, " +
                "leading lines composition, symmetrical composition, " +
                "diagonal composition, golden ratio geometrical compsition, spiral twirling composition, shallow depth of field extreme bokeh composition, " +
                "textured foreground composition, vibrant color contrast composition, silhouette composition, " +
                "dynamic tension composition, abstract pattern composition, reflective symmetry composition, " +
                "juxtaposition composition, balanced elements composition");

            var locations = new Alias("GPTLocations",
                "Seaside cliffs of rural Seoul, the moors of central barren foggy Scotland along a plateau edge above an abandoned castle, " +
                "the San Francisco bay trail in fog, " +
                "The sf peninsula foggy hills above Half Moon Bay, " +
                "An infinitely high cliff overlooking a beautiful detailed paradise farming world, " +
                "A post-apocalyptic restoration center for humanity" +
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
                "Mythical Olympus at its Peak (Splendor Rivalry) - The home of the gods resplendent but rife with rivalries.," +
                "A City on the Back of a Giant Moving Creature (Wanderlust Fear) - A nomadic city with the constant threat of the unknown.," +
                "Parallel Universe New York (Bewilderment Excitement) - Familiar yet bizarrely different with endless possibilities.," +
                "Ghost Ship in the Bermuda Triangle (Mystery Fear) - A haunted vessel with an eerie and uncertain fate.," +
                "An Enchanted Forest with Warring Fae (Beauty Betrayal) - Ethereal landscapes shadowed by deceit and power struggles.," +
                "Middle Earth's Rivendell during War (Tranquility Tension) - A serene elven haven on the brink of war.," +
                "Atlantis Rising from the Sea (Wonder Awe) - The mythical city re-emerging revealing ancient technologies and mysteries.," +
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
                "Feudal Japan during the Time of Samurai - Castles and cherry blossoms and and warrior code.," +
                "Ancient Polynesian Settlements in Hawaii - Early navigators and unique island culture.," +
                "Colonial Williamsburg in the 18th Century - A living history of American colonial life.," +
                "The Grandeur of the Mughal Empire in 16th Century India - Exquisite architecture and rich culture," +
                "Ancient Rome at its Zenith under Emperor Trajan - A cosmopolitan empire at its peak," +
                "The Ming Dynasty's Forbidden City in 15th Century Beijing - The imperial palace complex at the heart of China");

            var rpgstyles = new Alias("RPGStyles",
                "Dungeons and Dragons, Pathfinder, Lamentations of the Flame Princess, " +
                "Dungeon Crawl Classics, Fantasy AGE, Warhammer Fantasy, " +
                "Palladium Fantasy, G.U.R.P.S, Basic Fantasy, Low Fantasy Gaming, " +
                "Vagabond, Tales of the Valiant, Cypher System, Savage Worlds, " +
                "RuneQuest, Ars Magica, Iron Kingdoms, Torchbearer, The One Ring, " +
                "Burning Wheel, Legend of the Five Rings, Fate, 13th Age, " +
                "Adventurer Conqueror King System, Forbidden Lands, Conan, OSR, " +
                "Fighting Fantasy, Tunnels and Trolls, Monsters Monsters, TTRPG, " +
                "EZD6, Index Card RPG, Dungeons of Drakkenheim");

            var robloxGames = new Alias("GPTRobloxGames", "Adopt Me: Neon Pets, Apocalypse Rising, Bad Business, Balloon Simulator, Bird Simulator, Clone Tycoon 2, Crazy Knife, Cube eat Cube, Dance Your Blox Off, Firefighter Simulator, Flood Escape 2, Guest World, Heroes Online, Hide and Seek Extreme, Ice Cream Simulator, Mining Simulator 2, Parkour Simulator, Please Donate, Prison Life, Restaurant Tycoon 2, RoCitizens, Robeats, Robloxian Highschool, Rumble Quest, SCP: Roleplay, Saber Simulator, SharkBite, Snow Shoveling Simulator, Soccer Legends, Strucid, Super Bomb Survival!!, Super Doomspire, Survive the Killer, Swordburst 2, Terrain Parkour, Texting Simulator, The Floor is Lava, Tower Battles, Ultimate Driving: Westover Islands, Wizard Tycoon - 2 Player, World // Zero, Zombie Rush, Blade ball, Cube eat cube, Egg Hunt, Trade Hangout, Big paintball, Ride a cart simulator, Tiny Tanks, Hueblox, Classic Crossroads, Grass Cutting Tycoon, Murder Mystery 2, Swordfight on the heights, Sword fighting simulator, Bee Swarm Simulator, Sword Fight Tornument, +1 fat every second, Trade Hangout, Catalog Heaven, Mermaid Life, Hueblox, My Droplets, Crossroads, The Underground War, mortal coil, Wacky Wizards, Welcome to the town of Robloxia - Building, Nuke the Whales, BIRD, Bloxfruits, Deception Infection, Super Golf, Weight Lifting Simulator, Watermelon Go, BIG Paintball, Bladeball, Survive a plane crash, Fall down a 10000 stud hole, Build to survive the dragobloxers, VR Hands, Condo Games, On Tap, DOORS, Santa's Winter Fortress, Tower Defense Simulator, A wolf or other, Cleaning Simulator, Loomian Legacy, Broken Bones IV, Project Lazuras, Apocalypse Rising, Obby but you're on a bike, The Roblox Happy Home, Gucci Garden, Catalog Avatar Creator, Reason 2 Die, Mad Murderer, Miner's Haven, The Wind, Islands, Adopt Me!, Tower of Hell, Brookhaven 🏡RP, MeepCity, Murder Mystery 2, Blox Fruits, Arsenal, Royale High, Piggy, Jailbreak, Shindo Life, Anime Fighting Simulator, Bee Swarm Simulator, Welcome to Bloxburg, Build A Boat For Treasure, Mad City, Dungeon Quest, Vehicle Simulator, Ninja Legends, Bubble Gum Simulator, Tower Defense Simulator, Phantom Forces, Boku No Roblox, Super Power Training Simulator, Mining Simulator, Dragon Ball Z Final Stand, Shinobi Life 2, Natural Disaster Survival, Work at a Pizza Place, Blox Piece, Adopt and Raise a Cute Kid, Speed Run 4, Weight Lifting Simulator 3, Roblox High School, Theme Park Tycoon 2, Epic Minigames, Lumber Tycoon 2, Super Hero Tycoon, Treasure Hunt Simulator, Island Royale, Zombie Attack, Pet Simulator X, Flee the Facility, Skywars, Vehicle Tycoon, Booga Booga, Lucky Block Battlegrounds, Pokemon Brick Bronze, Among Us");
            var games = new Alias("GPTGames", "Clash of Clans, Angry Birds, Candy Crush, Pokemon Go, Call of Duty: Modern Warfare, Super Mario Bros., The Legend of Zelda, Tetris, Minecraft, GTA V, The Elder Scrolls V: Skyrim ,  World of Warcraft, Overwatch, Super Smash Bros. Ultimate, Dark Souls III, The Witcher 3, Counter-Strike, DOTA 2, League of Legends, Everquest, Plants vs Zombies, 2048, Minesweeper, Freecell, Fruit Ninja, Simcity, The Sims, Rocket League, Diablo II, Path of Exile, Stardew Vallye, Garry's Mod, PubG, Bejeweled, Final Fantasy VII, Assassin's Creed, Metal Gear Solid V, Animal Crossing, Halo: Combat Evolved, Portal, Beat Saber, Werewolf, Sudoku, Yu-Gi-Oh!, Magic: The Gathering, Dance Dance Revolution, Quidditch, Chess, Checkers, Baduk aka Weiqi aka go, Tic-Tac-Toe, Hades, Nethack, Solar Scuffle, Apex Legends, Splatoon 2, Sea of Thieves, Military Madness aka Nectaris, Arkanoid, Donkey Kong Jr., Subnautica, The Last of Us, Pinball, Quake, Myst, Street Fighter II Turbo, Sonic the Hedgehog, GoldenEye 007, Half-Life, StarCraft, Guitar Hero, Wii Sports, Bioshock, Fallout 2, Pong, Battleship, Club, Frogger, Asteroids, Duck Hunt, Prince of Persia, Mortal Kombat, The Oregon Trail, Lemmings, fortnite, slay the spire, factorio, Mega Man X, Factorio, EarthBound, Deus Ex, Populous, Baldur's Gate, Age of Empires II: The Age of Kings,Axis and Allies, Risk, Gears of War, Castlevania ");

            var adjectives = new Alias("GPTArtstyles", "Aquarela brasileira, 3D, 3D model, 3D printing art, Abstract, Abstract geometry, Acrylic pour, Action painting, Aerial drone art, " +
                "Ancient pottery techniques, Animación stop motion, Anime, Ansel Adams-like stark, Apocalyptic, Apocalyptic cityscape, Architectural model building, Arte cinético (Spanish)," +
                "Arte conceptual (Spanish),Arte digital (Spanish),Arte povera (Italian), Arte mosaico bizantino, Arte quilling decorativo, Arte urbano graffiti, Augmented reality, " +
                "Bamboo weaving, Baroque, Barro negro oaxaqueño, Basquiat-inspired street, Batik javanés, Bead weaving, Berniniesque dynamic, Bijutsu shashin (美術写真),Black velvet, " +
                "Bioluminescent installation, Blackwork embroidery, Body painting, Bone carving, Book folding, Bordado punto cruz, Bordado suzani, Boschian fantastical, Botticellian grace, " +
                "Brancusiesque simplified, Bruegelian detailed, Calado en madera, Caligrafía árabe, Calligraphic, Calligraphic graffiti fusion, Calligraphic lettering, Calligraphy, " +
                "Candle crafting, Caravaggesque chiaroscuro, Cascading light display, Cerámica gres, Cerámica raku abstracta, Cestería mapuche, Chagallesque floating, Chainmaille fabric, " +
                "Chalk street art, Prismatic Spray wave image, Cinematic virtual scenery, Clay animation storytelling, Clay pottery, Coffee art, Collage, Color-shifting mural art, Conceptual art," +
                " Conceptual space exploration, Constable-inspired pastoral, Constructivist, Coral assemblage, Coral reef painting, Cosmic, Cosmic art, Cosmic photography, Courbet-like realism, " +
                "Crackle glaze, Cubism, Cubist, Cubist figure, Cyanotype print, Cybernetic, Cybernetic organism, Cyberpunk, Cézannesque geometric, Da Vincian intricate, Dada, Daliesque surreal," +
                " Danza butoh visual, Danza contemporánea (Spanish),contemporary dance., Deco, Deco elegance, Dibujo carboncillo expresivo, Digital 3D, Digital fabric weaving, Digital mosaic, " +
                "Digital painting, Digital vector graphics, Dot painting, Dreamy, Dreamy landscape, Drybrush technique, Dystopian landscape sketch, Ebru sanatı, Eco, Eco-friendly sculpture, " +
                "Eco-sculpture, Embroidered, Embroidered fabric, Embroidery, En plein air, Encaustic painting, Ephemeral sand portraits, Escheresque impossible, Escultura hielo luminoso," +
                " Escultura reciclaje innovador, Estampa ukiyo-e, Ethereal, Ethereal glass sculpture, Ethereal vision, Experimental shadow theater, Expressionist, Fabric batik, Fauvist, " +
                "Faux finish, Feather art, Felted wool landscapes, Fiber optic tapestry, Filigrana italiana, Fire ink drawing, Floral arrangement, Fluid art, Fluorescent body art, Folk, " +
                "Folk embroidery, Food carving, Fotografía callejera monocromo, Fractal laser etching, Fresco, Fridaesque personal, Futurist, Futuristic, Futuristic skyline, " +
                "Galactic spray paint, Gauguinesque exotic, Geodesic dome mural, Geometric, Glacial, Glacial sculpture, Glass blowing, Glass etching, Glitch, Glowing ice sculpture, Gold leaf, " +
                "Gothic, Gothic cathedral, Goyesque grotesque, Grabado linóleo detallado, Graffiti, Graffiti wall, Graffito, Gravura em madeira,Gyotaku fish print, Hand-dyed fabric art, " +
                "Handmade paper making, Handwoven light patterns, Haptic VR art, Haute couture,Highbrow, Highbrow, Highbrow critique, Hokusai-inspired wave, " +
                "Holographic art, Holographic display, Holographic street art, Hopperesque solitude, Hyperbolic tessellation art, Hyperreal, Hyperrealism sketch, Ice, " +
                "Ice carving, Ice sculpting, Ice sculpture, Iconografía bizantina, Glacier carving, fire-melted ice, Lava sculpture, Ikebana (生け花),Mixed media collage, Ilustración digital fantasía, Immersive virtual reality, Impressionist, " +
                "Ink brush, Ink wash, Instalación arte lumínico, Instalación artística (Spanish),Intaglio, Instalación sonido inmersivo, Interactive light installation, Interactive mural," +
                " Interactive sidewalk chalk, Iridescent bubble art, Joyería artesanal étnica, Junk sculpture, Kahloesque vivid, Kalamkari indio, Kilning, Kinetic, Kinetic mobile, " +
                "Kinetic sand drawing, Kinetic sculpture, Kinetic wind art, Kintsugi (金継ぎ),Kintsugi repair, Klimtesque ornate, Lace making, Lacquer, Land art, Large-scale mural painting," +
                " Laser-cut paper art, Leather tooling, Levitating sculpture art, Light painting, Light projection, Linocut print, Liquid metal sculpture, Litho, Litho print, Lithograph, " +
                "Lost wax casting, Lowbrow, Lowbrow humor, Luces y sombras (Spanish),lights and shadows., Luminous thread embroidery, Macabre, Macro photography, Magnetic fluid art, " +
                "Magrittesque mysterious, Mandala design, Manetesque modern, Manga, Manga sketch, Marbling, Metal forging, Michelangelesque heroic, Miniature book binding, Miniature model," +
                " Minimal, Minimal design, Minimalist, Mixed reality installation, Mokuhanga (木版画),Batik tulis,Céramique raku,Papiroflexia,Arte callejero," +
                "Joie de vivre,Sgraffito (Italian),Guóhuà (国画),traditional Chinese painting., Monetesque impressionist, Mosaic, Munchian angst, Mural, Mural street, Muralismo digital interactivo," +
                " Muralismo mexicano (Spanish),Mexican muralism., Máscara veneciana, Natural landscape photography, Needle felting, Neon, Neon calligraphy, Neon portrait, Neon wireframe sculptures, " +
                "Nihonga, Noir, Noir mystery, O'Keeffesque enlarged, Optical, Optical art, Optical illusion, Organic ceramic forms, Origami, Origami crane, Origami tessellation, Paisaje sonoro urbano, " +
                "Panoramic digital fresco, Papel picado mexicano, Paper cutting, Paper mâché, Paper quilling, Papiroflexia modular avanzada, Parchment scroll, Pastel, Pastel drawing, Peinture abstraite," +
                "abstract painting., Peisaj urban (Romanian),urban landscape., Performance art, Performance teatro callejero, Perfume blending, Phosphorescent forest art, Photoreal, Pintura al fresco, " +
                "Pintura al óleo (Spanish),Pyrography, Pintura sumi-e, Pintura óleo realista, Pirograbado ruso, Pixel, Pixel animation, Pixelated, Pixelated character, Pollock-style drip, Pop, Pop icon, Primitivism, " +
                "Programmable matter sculpture, Proyección mapping dinámico, Psychedelic, Psychedelic swirl, Qi baishi (齐白石),Chinese ink painting style., Quantum dot canvas, Quilling art, Quilting, Quink art, Raku, Rakú japonés, " +
                "Raphaelesque serene, Reactive sound mural," +
                " Real-time animation projection, Recycled art, Relief carving, Relieve precolombino, Rembrandtesque dramatic, Resin art, Retro, Retro diner, Retrofuturism, " +
                "Robotic arm painting, Rodinesque sculptural, Rothko-inspired abstract, Rubenesque voluptuous, Rustic, Rustic barn, Rustic pottery, Salt flat mirage, Salt painting, Sand, Sand animation, " +
                "Sand castle, Sand mandala, Scented ink calligraphy, Scherenschnitte (German),Scrap metal sculpture, Seuratesque pointillist, Sgraffito, Sgraffito etching, Shadow art, Shadow play, Shell mosaic," +
                " Shibori dyeing, Silhouette, Silhouette cutout, Silk embroidery, Site-specific performance art, Snow architecture, Soap sculpture, Solar plate etching, Solar reactive painting, Sound installation, Sound wave art, " +
                "Sound-activated light art, Stained glass, Steampunk, Steampunk gadget, Stencil, Stenciled, Stenciled message, Stone masonry, Sumi-e ink, Surreal, Surreal dreamscape, Surrealist, " +
                "Ta moko (Māori),traditional skin art., Talla piedra ancestral, Tape art, Tapestry, Tapestry weave, Tapiz flamenco, Tapiz mural contemporáneo, Tatuaje polinesio, Terrazzo flooring, Textile dyeing," +
                " Thangka, Thermal imaging, Thermal reactive murals, Three-dimensional street art, Time-lapse drawing, Tinta china (Spanish),Chinese ink, Tissage africain, Titianesque vibrant, Toulouse-Lautrec-esque nightlife," +
                " Traditional woodblock printing, Tribal, Tribal mask, Trompe l'oeil, Trompe-l'œil,Ukiyo-e, Turner-inspired atmospheric, Técnica collage mixto, Ultraviolet landscape art, Urban, Urban decay photography, " +
                "Urban sketching, Urban sprawl, Van Goghian swirl, Vaporwave, Vaporwave scene, Vermeeresque light, Vexel illustration, Vintage photo restoration, Virtual sculpture, Vitrail, Vitraux gothique, Warholian pop, Watercolor," +
                " Watercolor garden, Weaving, Wheatpaste poster, Wire sculpture, Wood carving, Woodblock, Woodblock print, Woodcut, Wool felting, Xylography, Yarn bombing, Yarnbomb, Zen, Zen garden, Zen garden design, Zentangle drawing");

            var backgrounds = new Alias("GPTBackgrounds",
                "a barren grotto, a jungle clearing, a forest glen, a cliff temple, a riverine valley, a whitewater rapids, " +
                "a boulder field, a portrait factory, a wood mill, a fantasy quarry, a crowded swimming pool, " +
                "maximum security bank vault, magicians tower of artifacts, archaelogocial site in a deep pit in a modern italian city, " +
                "dragons lair, prison, infinite scale furtniture factory, parrot heaven, garden of eden, morocco, bazaar, pinnacle, " +
                "outdoor boulder field, gigantic crevasse, ice floe, volcano top, lava flow, moonscape, alien planet, mega tower, " +
                "battle mech control panel, battle mech warfield, war game, world made of cubes, hexagonal scale wall, gigantic plane, " +
                "minimalistic plane of air, civilization in a cloud city, a metallurgical factory, an elemental plane of fire, a flooded castle, " +
                "a mansion, a polar bear igloo, a ceramic vase, a wooden vessel, a gigantic redwood tree top, a treehouse, a romantic barn, " +
                "a warm greenhouse in a cold nation, a hothouse orchid farm beneath an icy sky, an overcast sky green lush ireland," +
                " a castle tower with magical gems, a jeweled mine, a wall made of jewelry, a jewelry store, a hidden cache of gold coins, " +
                "a gigantic dice, a continent-sized gear, a tube of transportation, a wormhole, a gas giant, a chonky cat spaceship, a cream factory, " +
                "a sound-proofed room, on top of a gigantic lit candle, a realm of infinite mirrors, a field covered with gigantic frosted blocks of glass, " +
                "sea glass museum, a bubble dome on an alien planet, inside a snowglobe, a dome of shattered glass, a crystal cavern," +
                "a sunken pirate cove, an enchanted mushroom forest, a sky-high monastery, a serpentine river gorge, behind a thunderous waterfall," +
                " an ancient ruins overgrown with vines, an alchemist's laboratory full of mystical brews, a hidden valley of flowers," +
                " an obsidian castle amidst fiery lava lakes, an underground city illuminated by glow worms, a towering ice castle," +
                " a secret garden with enchanted creatures, a Sphinx's sandstone library, a clockwork city running on steam and gears, " +
                "a subterranean tunnel network, floating islands with waterfalls pouring into the void, a deep sea Atlantis with bioluminescent life," +
                " a nebula cloud in outer space as a living habitat, an abandoned theme park reclaimed by nature, an infinite library with magical tomes, " +
                "a dinosaur sanctuary in a hidden valley, a giant's causeway of mythical proportions, an astral plane with floating celestial bodies, " +
                "a cybernetic hive city, a haunted Victorian manor with secret passages, the Grand Canyon filled with fog and mysteries, " +
                "an ethereal Northern Lights village, a coral reef city under the sea, a phoenix nest on a volcanic island, " +
                "a time-frozen battlefield with historical relics, a futuristic agropolis with vertical farms, a rainbow bridge connecting floating castles, " +
                "an Arctic research station with aurora observatory, a crystal iceberg labyrinth, a bamboo forest with hidden temples, " +
                "a starship junkyard on a deserted planet, an enormous anthill metropolis, a witches' market with magical goods, " +
                "a valley of geysers and hot springs, an infinite staircase to the stars, an underwater bubble city, a solar observatory on a remote peak, " +
                "a planetary ring habitat with artificial gravity, an electric storm planet with constant lightning, " +
                "a Titan's chessboard - a landscape of massive chess pieces, a ghost ship harbor, a giant's garden with enormous flora and fauna, " +
                "an eldritch dimension of unfathomable geometry, an oasis in a glass dome on a desert planet");

            var origins = new Alias("GPTOrigins",
                "Abkhazian Abkhazia,Acehnese Indonesian,Afghan,Ainu Indigenous people of Japan," +
                "Ainu,Akan Ghana,Albanian,Algerian,Ambonese Indonesian,Amhara,Amsterdam Netherlands," +
                "Andalusian Spain,Andorran,Aragonese Spain,Armenian,Ashanti Ghana,Ashkenazi Jewish," +
                "Assyrian,Asturian Spain,Athens Greece,Auckland New Zealand,Austrian,Avar,Azerbaijani," +
                "Bali Indonesia,Balinese Indonesian,Bamileke Cameroon,Bangkok Thailand,Bangladeshi,Banjar Indonesian," +
                "Barcelona Spain,Basajaun Basque mythology,Bashkir,Basque Basque Country,Basque,Batak Indonesian,Beijinger," +
                "Beirut Lebanon,Belgian,Bengali,Berber,Berlin Germany,Bhutanese,Bosnian Bosnia and Herzegovina,Breton,British," +
                "Bruneian,Budapest Hungary,Buenos Aires Argentina,Bugis Indonesian,Bulgarian,Burmese Myanmar,Burmese,Buryat,Cambodian," +
                "Cantabrian Spain,Cape Town South Africa,Castilian Spain,Catalan Catalonia,Catalan,Cebuano Filipino,Chamorro Guam,Chechen Russia," +
                "Chechen,China,Chinese,Chuvash,Circassian,Cook Islander,Copenhagen Denmark,Corsican,Croatian,Cypriot Cyprus,Czech,Dagestani Russia," +
                "Dagomba Ghana,Dai Chinese,Dayak Indonesian ,Dubai UAE,Dungan,Dutch Netherlands,Edinburgh Scotland,Egyptian,Emirati UAE,English,Ewe Ghana," +
                "Fang Central Africa, Faroese Faroe Islands,Fijian,Filipino,France,France,French,Frisian,Fulani West Africa,Futunan Wallis and Futuna," +
                "Ga Ghana,Galician,Gallego Spain,Georgia,Georgian,German,Gibraltarian,Greek Cypriot Cyprus,Greek,Greenlandic Inuit,Gujarati," +
                "Hakka Chinese,Hakka Chinese,Han Chinese,Hani Chinese,Hausa West Africa,Havana Cuba,Hmong,Hui Chinese Muslim,Hungarian,Igbo," +
                "Ilocano Filipino,Indian,Indonesian,Ingush,Iranian,Iraqi,Irish,Israeli,Istanbul Turkey,Italian,Italy,Italy,Italy,Japanese," +
                "Javanese,Jordanian,Kabardian,Kabyle Algeria,Kalmyk,Kannada,Karelian,Kazakh Kazakhstan,Kazakh,Khakas,Kikuyu,Kirghiz,Kiribatian," +
                "Korean,Kosovar Kosovo,Kumyk,Kurdish,Kuwaiti,Kyrgyz Kyrgyzstan,Ladino,Laotian,Las Vegas USA,Lebanese,Lezgian,Li Chinese,Libyan," +
                "Liechtensteiner,Lisbon Portugal,London UK,Los Angeles USA,Luxembourger,Lyonnais Lyon,Maasai,Macedonian,Madrid Spain,Madurese Indonesian," +
                "Makassarese Indonesian,Malagasy Madagascar,Malayali,Malaysian,Maldivian,Maltese,Manchu Northeast China,Manchu,Mandinka West Africa," +
                "Maori New Zealand,Marathi,Mari,Marrakech Morocco,Marseillais Marseille,Marshallese,Melbourne Australia,Mexico City Mexico,Miami USA," +
                "Miao Chinese,Micronesian,Milan Italy,Min Chinese,Minangkabau Indonesian,Mizrahi Jewish,Moldovan,Mongolian,Mongolian,Montenegrin," +
                "Montreal Canada,Mordvin,Moroccan,Moscow Russia,Mumbai India,Murcian Spain,Nauruan,Navarrese Spain,Neapolitan Naples,Nepalese,New Orleans USA," +
                "New York City USA,Ni-Vanuatu Vanuatu,Niuean,North Korean,Northern Irish,Nubian,Occitan,Okinawan Japan,Omani,Oriya,Oromo,Ossetian North and South Ossetia," +
                "Ossetian,Pakistani,Palauan,Palestinian,Papua New Guinean,Paris France,Parisian France,Pashtun,Portuguese,Prague Czech Republic,Punjabi," +
                "Qatari,Riffian Rif, Morocco,Rio de Janeiro Brazil,Romani,Romanian,Rome Italy,Sami Northern Europe,Sami,Samoan,San Diego USA,San Francisco USA," +
                "Sardinian,Sasak Indonesian,Saudi Arabian,Scottish,Seoul South Korea,Sephardi Jewish,Serbian,Shanghainese,Siberian Russia,Sichuanese,Sicilian Sicily," +
                "Sicilian,Sindhi,Singaporean,Slovak,Slovenian,Solomon Islander,Somali,South Korean,Spain,Spain/France,Spaniard,Sri Lankan,Stockholm Sweden,Sundanese Indonesian," +
                "Swazi Eswatini,Swiss,Sydney Australia,Syrian,Sámi,Tagalog Filipino,Tahitian French Polynesia,Taiwanese,Tajik Tajikistan,Tamil,Tatar Russia,Tatar,Tel Aviv Israel," +
                "Telugu,Thai,Tibetan Tibet,Tibetan,Tigrayan,Timorese East Timor,Tokelauan,Tokyo Japan,Tongan,Toraja Indonesian,Tuareg Sahara,Tuareg,Tujia Chinese,Tunisian," +
                "Turkish Cypriot Northern Cyprus,Turkish,Turkmen Turkmenistan,Tuvaluan,Tuvinian,Udmurt,Uighur Xinjiang,Uyghur,Uzbek Uzbekistan,Valencian Spain," +
                "Vancouver Canada,Venetian Venice,Venice Italy,Vietnamese,Vietnamese,Wallisian Wallis and Futuna,Walloon,Warsaw Poland,Welsh,Wolof Senegal," +
                "Wu Chinese,Xhosa,Yakut,Yemeni,Yoruba,Zhuang Chinese,Zulu");

            var gptStyles = new Alias("GPTStyles",
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
                "A detailed graphic novel illustration, " +
                "super high resolution close-up drawing," +
                " intensely emotional watercolor, " +
                "pop poster in Rococo style, " +
                "Art Nouveau image"+
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
                "retro-futuristic illustration, " +
                "vaporwave aesthetic background image," +
                "sultry Art Deco poster, " +
                "Vibrant Fauvist landscape," +
                "Meticulous pointillist piece with tiny distinct dots, " +
                "Byzantine mosaic featuring rich golden tesserae, " +
                "Pre-Raphaelite oil painting with romantic themes, " +
                "Dada assemblage challenging conventional aesthetics," +
                "tranquil Thomas Cole-inspired Hudson River School landscape, " +
                "Constructivist design with industrial motifs," +
                "Bold Futurist painting capturing movement and speed");

            var excitingLocations = new Alias("ExcitingLocations",
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
                "At an opulent Moroccan riad during a star-lit soiree");

            var shapes = new Alias("GPTShapes",
                "Circle, Triangle, Square, Rectangle, Pentagon, Hexagon, Octagon, Ellipse, Star, Heart}");

            var skies = new Alias("GPTSky",
                "Supernovae, Black Holes, Auroras (Northern and Southern Lights), Comets, Solar Eclipses," +
                "Lunar Eclipses, Neutron Stars, Galactic Collisions, Pulsars, Planetary Transits}");

            var asianLocations = new Alias("GPTAsiaLocations",
                "a rice field in summer with water filled and reflecting the infinite sky along narrow concrete paths, " +
                "suburban tokyo, an industrial small town in the mountainous regions of honshu in central japan, " +
                "yokohama in japan in a little izakaya as a server, a tokyo gigantic university library, " +
                "the morning after in akahabara at 9am as people get back to work, a gritty overpass osaka in, " +
                "medieval nara inside an entertainment venue for artisans down on theri luck, beijing at the important central city, " +
                "the bund in shanghai, an electronics market in guangdong, chunking hotel in hong kong the scene of many hijinx, " +
                "a secret military bunker and training school under seoul, the government\'s prepared government in exile center in pusan, " +
                "the south korean countryside near the ocean, " +
                "the island of jeju which is famous as a place for newlyweds to spend time and fall deeper in love");

            var aliases = new List<Alias>() {
                    rpgstyles,
                    protagonists,
                    backgrounds,
                    gptStyles,
                    locations,
                    origins,
                    excitingLocations,
                    shapes,
                    skies,
                    asianLocations,
                    robloxGames,
                    games,
                    forts,
                    compositionTypes,

                    adjectives,
                    ///These are for making set decks, and more function as simple terms to refer to variation beyond which we probably don't really need to see.
                    //new Alias("A", "A single letter \"A\""),
                    //new Alias("B", "A single letter \"B\""),
                    //new Alias("C", "A single letter \"C\""),
                    //new Alias("blue", "written in deep clear sharp blue ink, carefully written and drawn"),
                    //new Alias("black", "written in black ink"),
                    //new Alias("golden", "written in gold ink"),
                    //new Alias("alive", "very simple normal form of a letter, with just one or two hidden tiny eyes, and one tiny claw or other subtle signs of life"),
                    //new Alias("furry", "covered with a short, fine fur, which waves and flows from the edges."),
                    //new Alias("squares", "made of tiny squares stacked in rows"),
                    //new Alias("3d", "as a 3d rendered protuding non-flat object viewed at an angle in pointillism style"),
                    //new Alias("2d", "drawn in a completely flat comic side view style, in papyrus font, these are just shapes and lines in watercolor style"),
                    //new Alias("calligraphy", "done in completely flat hand-written traditional calligraphy"),

                    new Alias("Rainbow", "in a glorious rainbow"),
                    new Alias("Lightning", "in a lightning storm"),
                    new Alias("Fog", "in a very foggy environment"),

                    //{watercolor, illustration, photograph} of {boy, woman, man} in {mystery, romance, science fiction} set {mountains, ocean, desert}. -r
                    new Alias("Boy", "a young white 14yo ragged boy who loves outdoors and his cute coonhound puppy"),
                    new Alias("Woman", "a white irish woman 30yo with long red hair and blue eyes in a modest white dress with a small dove flying overhead"),
                    new Alias("Man", "a fierce 65yo thin gaunt Argentinian man and black moustache and old green army uniform and few medals, holding his vintage binoculars"),

                    //set...
                    new Alias("Mountains", "up in the mountains near the matterhorn below a crescent moon at midnight, and it's cold"),
                    new Alias("Ocean", "next to an abandoned beach near a cliffy short and overgrown tropical jungle, with a small boat visible in the distance, at sunrise, with slight mist"),
                    new Alias("Desert", "in a jagged barren desert near death valley, very hotin the daytime, with a full moon visible"),

                    //in...
                    new Alias("Mystery", "a dramatic mystery scene of MC chasing a small evil raccoon with trench coat carrying bag of dropping gold "),
                    new Alias("Science Fiction", "a scifi scene, holding a high-tech gizmo looking up at a small alien spaceship"),
                    new Alias("Romance", "a peaceful romance scene love and protection between mc and a large black horse with a plaid red blanket on its back and a flowing black mane"),

                    //okay, adding too many words to the style descriptor here basically destroys the ability of the system to remember detail which is mentioned later on.
                    new Alias("Watercolor", "an abstract sloppy watercolor with mottled blotchy color"),
                    new Alias("Sprang", "reminiscent of the style of dick sprang"),
                    new Alias("Pointillism", "a pointillism painting in amazingly detailed but clear dotted style with small dots and round"),
                    new Alias("Photo", "a super detailed incredibly resilient close-up masterwork photo from ansel adems on the most modern digital equipment."),
                    
                    //this is for a later SET
                    new Alias("plants", "which are flower pots and are totally overgrown with plants of many types and have dripping waterfalls over their sides"),
                    new Alias("houses", "which are houses arranged on a flat white infinite plain. They have lots of damage and are surrounded with glowing lightning bolts"),
                    new Alias("candles", "which are candles with a small yellow flame and covered with pure white snow and wisps of fog circling through beneath a night sky.")
            };

            return aliases;
        }
    }
}
