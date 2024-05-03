using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

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
        private Dictionary<string,List<string>> Results { get; } = new Dictionary<string, List<string>>();
        public void Setup(string input, IPromptSection parent)
        {
            ///three options:
            /// {GPTLocations} => a,b,c
            /// {a boy} => long version
            /// {a boy, a dog, a horse} => 3 long versions
            /// Also actually the [] sections are included here too.

            var parts = input.Split(new string[] { Statics.MetaPromptDivider }, StringSplitOptions.RemoveEmptyEntries).Select(el => el.Trim());
            //so now we have either ["GPTLocations"], ["a boy"], or ["a boy", "a dog", "a horse"]

            foreach (var part in parts)
            {
                var alias = Aliases.GetAliases().FirstOrDefault(el => el.Name == part);
                if (alias == null)
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

            foreach (var actualPart in section.L.Split(new string[] { Statics.MetaPromptDivider }, StringSplitOptions.RemoveEmptyEntries))
            {
                //var target = section.Parent;
                //target.ReceiveChoiceResult(new InternalTextSection(section.L, section.L, false,section.Parent), result);
                if (!Results.ContainsKey(actualPart))
                {
                    Results[actualPart] = new List<string>();
                }

                //TODO split these out into specific types of TextChoiceResultEnum
                Results[actualPart].Add(result.ToString());

                switch (result)
                {
                    case TextChoiceResultEnum.RequestBlocked:
                    case TextChoiceResultEnum.ImageDescriptionsGeneratedBad:
                    case TextChoiceResultEnum.PromptRejected:
                        BadCount++;
                        break;
                    case TextChoiceResultEnum.Okay:
                        GoodCount++;
                        break;
                    case TextChoiceResultEnum.RateLimit:
                    case TextChoiceResultEnum.RateLimitRepeatedlyExceeded:
                    case TextChoiceResultEnum.UnknownError:
                    case TextChoiceResultEnum.TooLong:
                    case TextChoiceResultEnum.BillingLimit:
                    case TextChoiceResultEnum.TaskCancelled:
                        break;
                    default:
                        throw new Exception("X");
                }
            }
        }

        //display the block/try rate for each of our subcomponents
        public string ReportResults(bool includeGlobal)
        {
            var res = "";
            if (includeGlobal)
            {
                res += $"\r\n\tGLOBAL: {(100.0 * BadCount / (1.0 * BadCount + GoodCount)):0.0}b% (b:{BadCount} g: {GoodCount})";
            }

            if (GoodCount > 0 || BadCount > 0)
            {
                var reorder = new List<Tuple<string, InternalTextSection>>();
                foreach (var content in Contents)
                {
                    //int bc = 0;
                    //int gc = 0;
                    var gg = Results.TryGetValue(content.L, out var bc);

                    //                    var gotBad = bc.Where(el=>el.Equals
                    //(TextChoiceResultEnum.RequestBlocked.ToString()) || el.Equals(TextChoiceResultEnum.ImageDescriptionsGeneratedBad.ToString()) || el.Equals(TextChoiceResultEnum.PromptRejected.ToString())).Count();

                    //var gotBad = BadCounts.TryGetValue(content.L, out bc);
                    //var gotGood = GoodCounts.TryGetValue(content.L, out gc);
                    //if (bc + gc > 0)
                    //{
                    //    var perc = 100.0 * bc / (bc + gc);
                    //    reorder.Add(new Tuple<string, InternalTextSection>($"{(int)perc:0.0}% {bc}b {gc}g", content));
                    //}
                    if (bc == null) { continue; }
                    var qq = bc.GroupBy(el => el).Select(el => $"{el.Key}:{el.Count()}");
                    var kk = string.Join(",", qq.OrderBy(el => el));
                    reorder.Add(new Tuple<string, InternalTextSection>(kk, content));

                }
                foreach (var tuple in reorder.OrderByDescending(el => el.Item1))
                {
                    res += $"\r\n\t{tuple.Item1}\t\t\t\t\t{(tuple.Item2.L)} ";
                }
            }
            return res;
        }
    }
}
