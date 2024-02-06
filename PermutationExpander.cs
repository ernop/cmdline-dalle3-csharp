using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Dalle3.Statics;

namespace Dalle3
{
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
}

