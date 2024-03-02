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
                var prompts = new List<string>()
                {
//"A father and his son are in a car accident. The father dies at the scene and the son is rushed to the hospital. At the hospital the surgeon looks at the boy and says \"I can't operate on this boy, he is my son.\" How can this be?",

//"The beholder is a rare and benevolent specimen of its kind, who has developed an interest in psychology and counseling. It uses its eye rays to probe the minds of its patients, and to induce various emotional states that can help them overcome their traumas. The prismatic sphere is a way of ensuring privacy and confidentiality for the sessions, as well as protecting the beholder from any hostile reactions from the people or the authorities.",

//"a small squad of eyeball soldiers, each trained in a different specialty, with its own eyeball weapons, and each with a different body configuration, where his body is also made of eyeballs. Obviously, within nature there are many types of eyeballs, such as cat eyes, lizard, octopus, insect, and many many more. They all appear and are relevant here. Also, think about what weapons would be effective against enemies whose entire bodies are made of eyes? obviously, things which attack the eye, like poking fingers, bright sunlight, etc.",

//"A medieval army lined up to go to battle, except that at within their ranks stand various large {Daschhunds,,Dobermans,,Labradors,,Chickens,,Giants,,Laser Pods,,Teslas,,Eyeball lasers,,Tyrannosauruses} army units. The image is in full day, and very clear and sharp and full of interesting details. God forgive any enemy who will encounter them. The image utilizes {GPTArtstyles} and features extremely long perspective and depth of field, unbelievable sights, distant towers, winding rivers and extremely rough terrain, with symbols of death, life, crucifixes, holiness and purity in the war against the evil.",

@"A cute little platypus pet in the big city! Its poisonous bite, its electrical shock ability, its webbed feet impress all the powerful people who see it in an expensive downtown Beijing Hutong. utilizing {GPTArtstyles} with a hint of {GPTArtstyles}"
                };

                foreach (var prompt in prompts)
                {
                    var usePrompt = prompt;
                    if (prompt.Trim()[0] != '-')
                    {
                        usePrompt = $"-r -h -hd -20 " + usePrompt;// + " Also: remember to add in a cute little puppy to this image.";
                    };
                    yield return usePrompt;
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