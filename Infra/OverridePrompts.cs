using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    "-h -r -hd -25 [2-4,a cat, a dog, lightning, lava, rainbows, double rainbows, meteors, cliffs, glaciers, kayakers in foldable kayaks, explosions, peace, calm, joy, love, reconciliation, erosion, battle scars, scales, charizard, kaiju, 1970s music, 1930s fashion, translucency, Roblox, User-made mspaint recangular ads on Roblox, lasers, the final battle] in an amazing {GPTArtstyles} image",
                    
                };
                foreach (var prompt in prompts)
                {
                    yield return prompt;
                }
            }
        }
    }
}
