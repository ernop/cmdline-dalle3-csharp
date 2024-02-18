using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

using XmpCore.Impl;

namespace Dalle3.Infra
{
    internal static class OverridePrompts
    {
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
                    "a flat 2d incredibly detailed and emotional style cartoon image by arnold lobel and william sprang with a warm glow and nostalgia," +
                    "a bright colorful claire wendling engraving, " +
                    "a glowing fantastical matt wuerker and tom tomorrow style illustration," +
                    "} of {Boy, Woman, Man} in {Science Fiction, Romance} set {Desert, Mountains}.";

                inp = "A splendid cat in {GPTLocations}";
                inp = "“Tiger, one day you will come to a fork in the road,” he said. “And you’re going to have to make a decision about which direction you want to go.” He raised his hand and pointed. “If you go that way you can be somebody. You will have to make compromises and you will have to turn your back on your friends. But you will be a member of the club and you will get promoted and you will get good assignments.” Then Boyd raised his other hand and pointed another direction. “Or you can go that way and you can do something—something for your country and for your Air Force and for yourself. If you decide you want to do something, you may not get promoted and you may not get the good assignments and you certainly will not be a favorite of your superiors. But you won’t have to compromise yourself. You will be true to your friends and to yourself. And your work might make a difference.” He paused and stared into Leopold’s eyes and heart. “To be somebody or to do something. In life there is often a roll call. That’s when you will have to make a decision. To be or to do? Which way will you go?” " +
                    "{GPTLocations}" +
                    "{GPTStyles}  -r";
                inp = "{GPTProtagonists}, {GPTProtagonists}, and {GPTProtagonists} on the left team up to fight {GPTForts} which is on the right in the distance. -h -hd -20";
                inp = "-h -hd -30 a small golden {GPTProtagonists}, a silver {GPTProtagonists}, and a stone {GPTProtagonists} are resting on a windowsill in {GPTLocations} as a beautiful girl prepares to go out.";

                var prompts = new List<string>()
                {
                    //" -h -190 -r " +
                    ////" There are no shadows. The background is completely white. " +
                    //"{Hundreds of, two, one}"+
                    //"{" +
                    //    "navyblue," +
                    //    "lemonyellow," +
                    //    "lightred" +
                    //"} " +
                    //"{" +
                    //    "pyramid," +
                    //    "sphere," +
                    //    "cube" +
                    //"} " +
                    ////"plain, identical and deeply 3d almost protruding from the image.  " +
                    ////"It's very clean and has a white background." +
                    ////" in a flat row left to right, all the same, not touching, taking up most of the image. "+
                    //"{" +
                    //    "plants," +
                    //    "houses," +
                    //    "candles" +
                    //"}.    " +
                    ////Remember there are hundreds of shapes in a row and NOTHING ELSE IN THE IMAGE just the three 3 shapes.
                    //" The images are extremely simple and basic" +
                    //"and minimalistic and have very few details and extraneous elements. Just the shapes and their elements."
                    //,
                    //"-h -20 -r -hd A chonky mutant {cassowary,petroglyph,octopus} spaceship floating above the rainbow lightning" +
                    //"surface of jupiter shooting cats eye lasers at aliens and floating and disturbing the wild red misty stone and dust rings in {GPTArtstyles}",
                    //@"-h -10 -r -hd There was no possibility of taking a walk that day. We had been wandering, indeed, in the leafless shrubbery an hour in the morning; but since dinner (Mrs. Reed, when there was no company, dined early) the cold winter wind had brought with it clouds so sombre, and a rain so penetrating, that further outdoor exercise was now out of the question. I was glad of it: I never liked long walks, especially on chilly afternoons: dreadful to me was the coming home in the raw twilight, with nipped fingers and toes, and a heart saddened by the chidings of Bessie, the nurse, and humbled by the consciousness of my physical inferiority to Eliza, John, and Georgiana Reed. The said Eliza, John, and Georgiana were now clustered round their mama in the drawing-room: she lay reclined on a sofa by the fireside, and with her darlings about her(for the time neither quarrelling nor crying) looked perfectly happy.Me, she had dispensed from joining the group; saying, “She regretted to be under the necessity of keeping me at a distance; but that until she heard from Bessie, and could discover by her own observation, that I was endeavouring in good earnest to acquire a more sociable and childlike disposition, a more attractive and sprightly manner—something lighter, franker, more natural, as it were—she really must exclude me from privileges intended only for contented, happy, little children.” ",
                    
                    //@"-h -30 -hd Show a Vibrant and Saturated Color Palette scene where The image features highly saturated colors with a vivid range of hues that create a dynamic and energetic visual effect.Flat yet Dimensional Illustration: While the artwork employs flat color blocks and lacks heavy shading or gradient effects, there is a sense of depth and dimension achieved through layering and the use of perspective. This gives the scene a stylized, yet three-dimensional appearance. Synthetic and Geometric Forms: The image incorporates geometric shapes and forms, such as the perfect cubes in the sky and the stylized, rounded trees. These elements contribute to a sense of an artificial or constructed world.  The colors are intense and rich, contributing to the overall dream-like atmosphere. Draw an epic scene from each of the following video game roblox games as a movie { Watermelon Go | Squid game | Tiny Tanks | Cube Eat Cube | Natural Disaster Survival | Factorio | Catalog Heaven | Trade Hangout | Grass Cutting Tycoon | Bee swarm Simulator | Egg Hunt }"
                    
                    @"A {threatening, aggressive, violent} little kitty-cat."
                };
                foreach (var prompt in prompts)
                {
                    Console.WriteLine(prompt);
                    yield return prompt;
                }
            }
        }
    }
}
