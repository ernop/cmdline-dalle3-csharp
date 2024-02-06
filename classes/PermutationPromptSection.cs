using System.Collections.Generic;
using System.Linq;

namespace Dalle3
{
    public class PermutationPromptSection : MetaPromptSection, IPromptSection
    {
        public PermutationPromptSection(string s)
        {
            Setup(s, this);
        }

        public InternalTextSection Sample()
        {
            var el = Contents[Statics.Random.Next(0, Contents.Count)];
            return el;
        }

        public IEnumerable<InternalTextSection> Iterate()
        {
            foreach (var el in Contents)
            {
                yield return el;
            }
        }
        public override string ToString()
        {
            return $"PermutationPromptSection:with {Contents.Count} parts.";
        }
    }
}
