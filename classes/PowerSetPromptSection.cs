using System;
using System.Collections.Generic;
using System.Linq;

namespace Dalle3
{
    /// <summary>
    /// An alias class is one which expands and splits.
    /// either a blowup of a single word into a comma separated list which we then further expand,
    /// or just the list itself.
    /// </summary>
    public class PowerSetPromptSection : MetaPromptSection, IPromptSection
    {
        public PowerSetPromptSection(string s)
        {
            Setup(s, this);
        }

        public InternalTextSection Sample()
        {
            return Statics.PickRandomPowersetValue(Contents);
        }

        public IEnumerable<InternalTextSection> Iterate()
        {
            foreach (var el in Statics.IteratePowerSet(Contents, 0, 0, 0))
            {
                yield return el ;
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
                con = $"with {Contents.Count()} items.";
            }
            return $"PowerSetPromptSection:ew with {con} parts.";
        }
    }
}
