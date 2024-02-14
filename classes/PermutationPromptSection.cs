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

        public IEnumerable<InternalTextSection> Sample()
        {
            return new List<InternalTextSection>() { Contents[Statics.Random.Next(0, Contents.Count + 1] };
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
