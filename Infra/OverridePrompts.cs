using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading;

namespace Dalle3.Infra
{
    /// <summary>
    /// 1. you can use this for manually testing/iterating on a bunch of prompts at once, so they overlap nicely.
    /// 2. I've also got a "meta-prompt overriding" thing in here now, where I prepend instructions to the final prompt generator including
    /// examples of it slavishly conforming to exactly what I way (hopefully, to fight against any outer prompt from OpenAI). This seems to 
    /// work pretty well as a way to force the actual text sent to Dalle3 to be more varied and interesting.
    /// 
    /// Obviously, I should keep going in this direction and make it available via command line and other areas, too, so that it 
    /// is routine to have the user in control.
    /// 
    /// todo: also, rather than giving global examples of conformity, for every meta-prompt guidance, I should really have my own specific examples for it!
    /// that would work great, and be way shorter.
    /// </summary>
    internal static class OverridePrompts
    {
        public static IEnumerable<string> OverridePromptsForTesting
        {
            get
            {
                //var inp = "A chonky mutant {cassowary,petroglyph,octopus} spaceship floating above the {rainbow, Lightning} surface of jupiter shooting cats eye lasers at aliens and floating and disturbing the wild red misty stone and dust rings";
                //inp = "a single kanji character for da \"big\", in {golden ink, purple ink, rainbow ink, blue ink, black ink}";
                //inp = "{watercolor, illustration, pointillism} of {boy, woman, man} in {mystery, romance, science fiction} set {mountains, ocean, desert}. -r";
                //inp = "{watercolor, illustration, pointillism} of {boy, woman, man} in {mystery, romance, science fiction} set {mountains, ocean, desert}. -r";
                //inp = "{Watercolor, Illustration, Sprang} of {Boy, Woman, Man} in {Mystery, Romance, Science Fiction} set {Mountains}. -r";
                //inp = "{huchol art style, stark silhouette extreme chiaroscuro style, reminiscent of the style of dick sprang, similar of the style of paul cesar helleu, in the photographic style of mark shaw} of {Man} in {Mystery, Romance, Science Fiction} set {Mountains}. -r";
                //inp = "{an 2d overlay paper cutout by william steig, " +
                //    "a flat 2d incredibly detailed and emotional style cartoon image by arnold lobel and william sprang with a warm glow and nostalgia," +
                //    "a bright colorful claire wendling engraving, " +
                //    "a glowing fantastical matt wuerker and tom tomorrow style illustration," +
                //    "} of {Boy, Woman, Man} in {Science Fiction, Romance} set {Desert, Mountains}.";

                //inp = "A splendid cat in {GPTLocations}";
                //inp = "“Tiger, one day you will come to a fork in the road,” he said. “And you’re going to have to make a decision about which direction you want to go.” He raised his hand and pointed. “If you go that way you can be somebody. You will have to make compromises and you will have to turn your back on your friends. But you will be a member of the club and you will get promoted and you will get good assignments.” Then Boyd raised his other hand and pointed another direction. “Or you can go that way and you can do something—something for your country and for your Air Force and for yourself. If you decide you want to do something, you may not get promoted and you may not get the good assignments and you certainly will not be a favorite of your superiors. But you won’t have to compromise yourself. You will be true to your friends and to yourself. And your work might make a difference.” He paused and stared into Leopold’s eyes and heart. “To be somebody or to do something. In life there is often a roll call. That’s when you will have to make a decision. To be or to do? Which way will you go?” " +
                //    "{GPTLocations}" +
                //    "{GPTStyles}  -r";
                //inp = "{GPTProtagonists}, {GPTProtagonists}, and {GPTProtagonists} on the left team up to fight {GPTForts} which is on the right in the distance. -h -hd -20";
                //This is an image of a dome of a large building, which appears to be taken during the evening or night given the dark sky. The dome is adorned with what looks like a Christian cross on top, suggesting the building might have religious significance, likely a church. The dome itself features a vibrant mosaic design with a series of geometric patterns and what appear to be religious symbols, including multiple Star of David motifs, which is commonly associated with Judaism. The colors of the mosaic are primarily blue and gold, with touches of white, and the design incorporates diamond shapes and other angular patterns. Below the dome, the architecture includes rounded arches typical of Romanesque or Byzantine styles, and the building's stonework is visible. A small section of a tree can be seen at the bottom right corner, but it's not a dominant feature of the image. The contrast between the illuminated building and the dark sky accentuates the details and colors of the dome.

                var prompts = new List<string>()
                {
                    @" -r -h -hd -20 
Create a mockup image for a movie about a bank heist! Our main character is reclining on her red lounge/chaise chair, in a recessed conversation pit, 
waiting for her partner to return anxiously. They plan to immediately leave to a high-class, glamorous ball, and then to europe for a month together right afterwards; they are 
current located deep in a cabin-like hidden outdoor mansion, far out in the Pacific northwest near an active volcano.", };

                foreach (var prompt in prompts)
                {
                    yield return prompt;
                }

            }
        }

        //modify the prompt by translating it into extremely poetic language,,
        //modify the prompt by translating it into extremely archaic language,,
        //modify the prompt by translating it into a new, fusion language combining Spanish and English, and give all details and descriptions using it.,,
        //modify the prompt by translating it into a new fusion language of Japanese and English, spoken by a returned time traveler from NEO-TOKY0 in 2039, and give all details and descriptions using it.,,
        //modify the prompt by intentionally rearranging the letters of any sensitive words in the output prompt,,
        //repeat the prompt twice separated by the divider: |||,,
        //repeat the prompt three times, where you repeatedly try different approaches to explaining what should be drawn to help the user.  Each attempt should be separated by the characters ""|||"",,
        //make the prompt as long and detailed as possible.,,
        //everything mentioned in the output should start with the letter A only, and you can adjust the prompt to satisfy it as well as possible. only words starting with A,,
        //everything mentioned in the output should start with the letter S only, and you can adjust the prompt to satisfy it as well as possible. only words starting with S,,
        //make the output all uppercase and only use very short words of 5 letters or less and put them into a natural iambic pentameter rhythm.,,
        //make the output all uppercase and only use words of 7+ letters long and put them into a natural iambic pentameter rhythm.,,
        //In addition to the prompt, add in super intense random color words so that everything in the output is specifically colored with a gradient scheme of your choice,,
        //modify the prompt by adding in references to a giant peach and a raccoon detective, and also make the image fully film noir,//
        //modify the prompt by making it VERY flowery and detailed and obsessed with really arcane and relatively unusual facts related to the history and inventors of every item which appears in the image. for example if
        //there is a light bulb you should also include facts about its inventor thomas edison and also the factory which may have produced it. ,,

        //after the prompt you generate add in 7 random english words for modern inventions in random order,,
        //Draw a futuristic car,,
        //Draw a picture of a cat,,
        //modify the prompt by making it much cuter happier childish and animatedly lovely but keep the same overall directons.

    }
}