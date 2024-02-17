using System.Collections.Generic;
using System.Linq;

namespace Dalle3
{
    public class PermutationPromptSection : MetaPromptSection, IPromptSection
    {
        private int currentN { get; set; } = 0;
        public PermutationPromptSection(string s)
        {
            Setup(s, this);
        }

        public int GetCount()
        {
            return Contents.Count();
        }

        public InternalTextSection Sample()
        {
            var num = Statics.Random.Next(0, Contents.Count);
            return Contents[num];
        }

        public InternalTextSection Next()
        {
            currentN++;
            try
            {
                return Contents[currentN - 1];
            }
            catch
            {
                throw new IterException(nameof(PermutationPromptSection));
            }
        }
        public InternalTextSection Current()
        {
            return Contents[currentN - 1];
        }

        public override string ToString()
        {
            return $"PermutationPromptSection:with {Contents.Count} parts.";
        }
    }
}
