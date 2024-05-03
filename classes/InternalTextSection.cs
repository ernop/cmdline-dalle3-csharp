using static Dalle3.Statics;

namespace Dalle3
{
    /// <summary>
    /// {A boy} => S = a boy, L="long desc", isSingle = true. This means that its safe to use exclusively S in filenames.
    /// {a boy, a dog} => S = a boy, L="long desc", isSingle = true. just use S, it's okay
    /// {GPTLocations} => S = GPTLocations, L="an abandoned castle", isSingle = false. This means that the value is one of the options for the multiplexer GPTLocations.
    ///     Here, you should use L in filenames since it is a meaningful choice among many, but if you want to explain why/what category this choice came from,
    ///     you can also label it with S.
    /// </summary>
    public class InternalTextSection
    {
        /// <summary>
        /// generally only used for things like {GPTLocations}, as a quick alias.
        /// </summary>
        public string S { get; set; }

        /// <summary>
        /// looking back now, what is the point of this? Make it simpler, seems to me to be the right answer.
        /// </summary>
        public string L { get; set; }
        public bool IsSingle { get; set; }
        public IPromptSection Parent { get; set; }

        public string GetValueForHumanConsumption()
        {
            if (IsSingle)
            {
                return S;
            }
            else
            {
                if (S == L)
                {
                    //I guess this is the case where we don't expand.
                    return S;
                }

                //this is where it's more like an abbreviation.
                return $"{S}:{L}";
            }
        }
        public InternalTextSection(string s, string l, bool isSingle, IPromptSection parent)
        {
            S = s;
            L = l;
            IsSingle = isSingle;
            Parent = parent;
        }

        public override string ToString()
        {
            return "t:"+GetValueForHumanConsumption();
        }
    }
}
