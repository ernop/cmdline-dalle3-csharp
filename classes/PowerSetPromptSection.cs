using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dalle3
{
    /// <summary>
    /// An alias class is one which expands and splits.
    /// either a blowup of a single word into a comma separated list which we then further expand,
    /// or just the list itself.
    /// ALso: you can set a range of how many items you want to pick from the set by adding 
    /// an entry like [1-2,a cat, a dog, a mouse, a house] which will pick 1-2 items only.
    /// </summary>
    public class PowerSetPromptSection : MetaPromptSection, IPromptSection
    {
        public int Min { get; set; } = 0;
        public int Max { get; set; } = int.MaxValue;
        public PowerSetPromptSection(string s)
        {
            var rangeRe = new Regex(@"^(\d+)-(\d+),");
            var mm = rangeRe.Match(s);
            if (mm.Success)
            {
                Min = int.Parse(mm.Groups[1].Value);
                Max = int.Parse(mm.Groups[2].Value);
                s = rangeRe.Replace(s, "");
            }
            Setup(s, this);
        }

        public IEnumerable<InternalTextSection> Sample()
        {
            return Statics.PickRandomPowersetValue(Contents, Min, Max);
        }

        public IEnumerable<InternalTextSection> Iterate()
        {
            foreach (var el in Statics.IteratePowerSet(Contents))
            {
                yield return el.First();
            }
        }

        public override string ToString()
        {
            string con;
            if (Contents.Count() == 1)
            {
                con = Contents.First().L;
            }
            else
            {
                con = $"with {Contents.Count()} parts";
            }
            return $"PowerSetPromptSection:{con}";
        }
    }
}
