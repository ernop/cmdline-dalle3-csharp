using System;
using System.Collections.Generic;
using System.Linq;

using static Dalle3.Statics;

namespace Dalle3
{
    /// <summary>
    /// A general thingie that tracks its subelements and returns them.
    /// either a permutation one or a power set one or a simple alias.
    /// </summary>
    public abstract class MetaPromptSection
    {
        public IList<InternalTextSection> Contents { get; internal set; } = new List<InternalTextSection>();
        private int BadCount { get; set; } = 0;
        private int GoodCount { get; set; } = 0;
        private Dictionary<string, int> GoodCounts { get; } = new Dictionary<string, int>();
        private Dictionary<string, int> BadCounts { get; } = new Dictionary<string, int>();
        public void Setup(string input, IPromptSection parent)
        {
            ///three options:
            /// {GPTLocations} => a,b,c
            /// {a boy} => long verison
            /// {a boy, a dog, a horse} => 3 long versions
            /// Also actually the [] sections are included here too.

            var sp = new string[] { ",," };
            var parts = input.Split(sp, StringSplitOptions.RemoveEmptyEntries).Select(el => el.Trim());
            //so now we have either ["GPTLocations"], ["a boy"], or ["a boy", "a dog", "a horse"]

            foreach (var part in parts)
            {
                var alias = Aliases.GetAliases().FirstOrDefault(el => el.Name == part);
                if (alias == null) //raw text like {a newfoij, a  f2w3i}
                {
                    var abb = new InternalTextSection(part, part, true, parent);
                    Contents.Add(abb);
                }
                else //one or multiple aliases encountered.
                {
                    foreach (var subpart in alias.Contents) //i.e. for each location in GPTLocations.
                    {
                        var abb = new InternalTextSection(part, subpart, false, parent); //gptLocations (as a label), the specific part.
                        Contents.Add(abb);
                    }
                }
            }
        }

        public void ReceiveChoiceResult(InternalTextSection section, TextChoiceResultEnum result)
        {
            //okay, for the powerset case this isn't working yet.
            //i need to reconstruct which of the original internalTextSections were included.
            //given that there are limitations, that is possible, but seems wrong.

            if (!GoodCounts.ContainsKey(section.L))
            {
                GoodCounts[section.L] = 0;
            }
            if (!BadCounts.ContainsKey(section.L))
            {
                BadCounts[section.L] = 0;
            }
            switch (result)
            {
                case TextChoiceResultEnum.RequestBlocked:
                    BadCount++;
                    BadCounts[section.L]++;
                    break;
                case TextChoiceResultEnum.PromptRejected:
                    BadCount++;
                    BadCounts[section.L]++;
                    break;
                case TextChoiceResultEnum.ImageDescriptionsGeneratedBad:
                    BadCount++;
                    BadCounts[section.L]++;
                    break;
                case TextChoiceResultEnum.Okay:
                    GoodCount++;
                    GoodCounts[section.L]++;
                    break;
                case TextChoiceResultEnum.RateLimit:
                    break;
                case TextChoiceResultEnum.RateLimitRepeatedlyExceeded:
                    break;
                case TextChoiceResultEnum.UnknownError:
                    break;
                case TextChoiceResultEnum.TooLong:
                    break;
                case TextChoiceResultEnum.BillingLimit:
                    break;
                default:
                    throw new Exception("X");
            }
        }

        //display the block/try rate for each of our subcomponents
        public string ReportResults(bool includeGlobal)
        {
            var res = "";
            if (includeGlobal)
            {
                res += $"\r\n\tGLOBAL{(100.0 * BadCount / (1.0 * BadCount + GoodCount)):0.0}b% (b:{BadCount} g: {GoodCount})";
            }

            if (GoodCount > 0 || BadCount > 0)
            {
                var reorder = new List<Tuple<int,string, InternalTextSection>>();
                foreach (var content in Contents)
                {
                    int bc = 0; 
                    int gc = 0;
                    var gotBad = BadCounts.TryGetValue(content.L, out bc);
                    var gotGood = GoodCounts.TryGetValue(content.L, out gc);
                    if (bc + gc > 0)
                    {
                        var perc = 100.0 * bc / (bc + gc);
                        reorder.Add(new Tuple<int, string, InternalTextSection>((int)perc, $"{gc}/{(gc+bc)}", content));
                    }
                }
                foreach (var tuple in reorder.OrderByDescending(el => el.Item1))
                {
                    res += $"\r\n\t{tuple.Item1:0.0}b%\t{tuple.Item2}\t{Slice(tuple.Item3.L, SliceAmount)} ";
                }
            }
            return res;
        }
    }
}
