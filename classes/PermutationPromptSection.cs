using System.Collections.Generic;
using System.Linq;

namespace Dalle3
{
    public class PermutationPromptSection : MetaPromptSection, IPromptSection
    {
        public bool IsFixed() => false;
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
            var q = Contents[Statics.Random.Next(0, Contents.Count)];
            _MyContents = q.L;
            return q;
        }
        public string myContents() => _MyContents;
        private string _MyContents { get; set; }

        /// <summary>
        /// fake no worky
        /// </summary>
        public InternalTextSection Next()
        {
            return Contents[Statics.Random.Next(0, Contents.Count)];
            //try
            //{
            //    var toReturn = Contents[_CurrentN];
            //    return toReturn;
            //}
            //catch
            //{
            //    throw new IterException(nameof(PermutationPromptSection));
            //}
        }
        public override string ToString()
        {
            return $"PermutationPromptSection:with {Contents.Count} parts.";
        }        
    }
}
