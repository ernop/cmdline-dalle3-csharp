using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading;

namespace Dalle3
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
    public static class OverridePrompts
    {
        public static IEnumerable<string> OverridePromptsForTesting
        {
            get
            {
                var prompts = Statics.LoadPrompts("../../prompts.txt");
                var made = 0;
                foreach (var prompt in prompts)
                {
                    if (prompt.StartsWith("//")) { continue; }
                    var usePrompt = prompt;
                    if (prompt.Trim()[0] != '-')
                    {
                        var realUsePrompt = $" -4 -r '{usePrompt}'.  Exclude all textual elements";
                        usePrompt = realUsePrompt;
                        yield return usePrompt;
                        made += 1;
                    }
                    if (made > 400) { yield break; }
                }
            }

            //TODO: meta prompt modifications for the outer prompt.

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
}