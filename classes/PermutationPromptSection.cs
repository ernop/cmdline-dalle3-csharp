﻿using System.Collections.Generic;
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
            var num = Statics.Random.Next(0, Contents.Count);
            return new List<InternalTextSection>() { Contents[num] };
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
