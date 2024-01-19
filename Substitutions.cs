using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dalle3
{
    internal static class Substitutions
    {

        public class Blowup
        {
            public string Short { get; set; }
            public string Long { get; set; }
            public Blowup(string s, string l)
            {
                Short = s;
                Long = l;
            }
        }

        /// <summary>
        /// {A,B,C} {blue,black,golden} {ivory,furry,squares} {3d,2d,calligraphy} on a clear blank white background
        /// Dalle3.exe A {Joyful, Solemn, Hopeful} {Alien, Human, Flower} in {Watercolor, Illustration, Photograph} around {Rainbow, Lightning, Fog}, with a clear white background -r
        /// </summary>
        public static string SubstituteExpansionsIntoPrompt(string prompt)
        {

            var expansions = new List<Expando>() {
                new Expando("A","A single letter \"A\""),
                new Expando("B","A single letter \"B\""),
                new Expando("C","A single letter \"C\""),
                new Expando("blue","written in deep clear sharp blue ink, carefully written and drawn"),
                new Expando("black","written in black ink"),
                new Expando("golden","written in gold ink"),
                new Expando("alive","very simple normal form of a letter, with just one or two hidden tiny eyes, and one tiny claw or other subtle signs of life"),
                new Expando("furry", "covered with a short, fine fur, which waves and flows from the edges."),
                new Expando("squares","made of tiny squares stacked in rows"),
                new Expando("3d","as a 3d rendered protuding non-flat object viewed at an angle in pointillism style"),
                new Expando("2d","drawn in a completely flat comic side view style, in papyrus font, these are just shapes and lines in watercolor style"),
                new Expando("calligraphy","done in completely flat hand-written traditional calligraphy"),

                new Expando("Rainbow","in a glorious rainbow"),
                new Expando("Lightning","in a lightning storm"),
                new Expando("Fog","in a very foggy environment"),

                //{watercolor, illustration, photograph} of {boy, woman, man} in {mystery, romance, science fiction} set {mountains, ocean, desert}. -r
                new Expando("Boy","a young white 14yo ragged boy who loves outdoors and his cute coonhound puppy"),
                new Expando("Woman","a white irish woman 30yo with long red hair and blue eyes in a modest white dress with a small dove flying overhead"),
                new Expando("Man","a fierce 65yo thin gaunt Argentinian man and black moustache and old green army uniform and few medals, holding his vintage binoculars"),

                //set...
                new Expando("Mountains","up in the mountains near the matterhorn below a crescent moon at midnight, and it's cold"),
                new Expando("Ocean","next to an abandoned beach near a cliffy short and overgrown tropical jungle, with a small boat visible in the distance, at sunrise, with slight mist"),
                new Expando("Desert","in a jagged barren desert near death valley, very hotin the daytime, with a full moon visible"),

                //in...
                new Expando("Mystery","a dramatic mystery scene of MC chasing a small evil raccoon with trench coat carrying bag of dropping gold "),
                new Expando("Science Fiction","a scifi scene, holding a high-tech gizmo looking up at a small alien spaceship"),
                new Expando("Romance","a peaceful romance scene love and protection between mc and a large black horse with a plaid red blanket on its back and a flowing black mane"),

                //okay, adding too many words to the style descriptor here basically destroys the ability of the system to remember detail which is mentioned later on.
                new Expando("Watercolor","an abstract sloppy watercolor with mottled blotchy color"),
                new Expando("Sprang","reminiscent of the style of dick sprang"),
                new Expando("Pointillism","a pointillism painting in amazingly detailed but clear dotted style with small dots and round"),
            };
            foreach (var e in expansions)
            {
                var key = $"AAA{e.S}AAA";
                if (prompt.Contains(key))
                {
                    var usingValue = e.L;
                    if (string.IsNullOrEmpty(e.L))
                    {
                        usingValue = key;
                    }
                    prompt = prompt.Replace(key, usingValue);
                }
            }
            if (prompt.ToLower().Contains("aaa"))
            {
                var a = 43;
            }
            prompt = prompt.Replace("AAA", "").Replace("aaa", "");
            return prompt;
        }
    }
}
