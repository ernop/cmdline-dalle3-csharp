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
        public bool IsFixed() => false;
        public int MinToReturn { get; set; } = 0;
        public int MaxToReturn { get; set; } = int.MaxValue - 1;

        public PowerSetPromptSection(string s)
        {
            var rangeRe = new Regex($@"^(\d+)-(\d+){Statics.MetaPromptDivider}");
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
            var fakeITS = Statics.PickRandomPowersetValue(Contents, MinToReturn, MaxToReturn);
            fakeITS.Parent = this;
            _MyContents = fakeITS.ToString();
            return fakeITS;
        }

        public string myContents() => _MyContents;
        private string _MyContents { get; set; }
        /// <summary>
        /// okay, this doesn't actually work since now even when allegedly iterating, we actually just pick a random value.
        /// TODO fix it once I figure out how it should be. when there are multiple different types, it gets complicated on how to iterate right.
        /// In general I suppose a global looping behavior is good, but even then, not perfect.
        /// </summary>
        public InternalTextSection Next()
        {
            try
            {
                return Statics.GetNthPowersetValue(Contents, Statics.Random.Next(0, (1 << Contents.Count - 1)));
            }
            catch
            {
                throw new IterException(nameof(PowerSetPromptSection));
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
