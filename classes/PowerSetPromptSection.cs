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
        public int MinToReturn { get; set; } = 0;
        public int MaxToReturn { get; set; } = int.MaxValue;
        public PowerSetPromptSection(string s)
        {
            var rangeRe = new Regex(@"^(\d+)-(\d+),");
            var mm = rangeRe.Match(s);
            if (mm.Success)
            {
                MinToReturn = int.Parse(mm.Groups[1].Value);
                MaxToReturn = int.Parse(mm.Groups[2].Value);
                s = rangeRe.Replace(s, "");
            }
            Setup(s, this);
        }
        public int GetCount()
        {
            return Contents.Count();
        }

        public InternalTextSection Sample()
        {
            var x = Statics.PickRandomPowersetValue(Contents, MinToReturn, MaxToReturn);
            return x;
        }

        /// <summary>
        /// fake iteration?
        /// </summary>
        private int currentN { get; set; } = 0;

        public InternalTextSection Next()
        {
            currentN++;

            try
            {
                return Statics.GetNthPowersetValue(Contents, currentN - 1);
            }
            catch
            {
                throw new IterException(nameof(PowerSetPromptSection));
            }

        }
        public InternalTextSection Current()
        {
            return Statics.GetNthPowersetValue(Contents, currentN - 1);
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
