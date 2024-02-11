using System;
using System.Collections.Generic;
using System.Linq;

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

            var parts = input.Split(',').Select(el => el.Trim());
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

        public void ReceiveChoiceResult(string choice, TextChoiceResultEnum result)
        {
            //Console.WriteLine($"I am a thingie and for {choice} I got {result}");
            if (!GoodCounts.ContainsKey(choice))
            {
                GoodCounts[choice] = 0;
            }
            if (!BadCounts.ContainsKey(choice))
            {
                BadCounts[choice] = 0;
            }
            switch (result)
            {
                case TextChoiceResultEnum.RequestBlocked:
                    BadCount++;
                    BadCounts[choice]++;
                    break;
                case TextChoiceResultEnum.PromptRejected:
                    BadCount++;
                    BadCounts[choice]++;
                    break;
                case TextChoiceResultEnum.DescriptionsBad:
                    BadCount++;
                    BadCounts[choice]++;
                    break;
                case TextChoiceResultEnum.Okay:
                    GoodCount++;
                    GoodCounts[choice]++;
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

        public string ReportResults()
        {
            var res = "\t";
            if (GoodCount == 0 && BadCount == 0)
            {
                res += " None yet.";
            }
            else
            {
                res += $"Overall: {(100.0 * BadCount / (1.0*BadCount + GoodCount)):0.0}b% (b:{BadCount} g: {GoodCount})";
                var keys = GoodCounts.Keys.ToList();
                keys.AddRange(BadCounts.Keys);
                var reorder = new List<Tuple<int, string>>();
                foreach (var key in keys.Distinct())
                {
                    var bk = BadCounts[key];
                    var gk = GoodCounts[key];
                    if (bk + gk > 0)
                    {
                        var perc = 100.0 * bk / (1.0 * bk + gk);
                        //res += $"\r\n\t\t{perc:0.0}b%\t{key}";
                        reorder.Add(new Tuple<int, string>((int)perc, key));
                    }
                    else
                    {
                        reorder.Add(new Tuple<int, string>((int)0, key));
                    }
                }
                foreach (var tuple in reorder.OrderByDescending(el => el.Item1))
                {
                    res += $"\r\n\t\t{tuple.Item1:0.0}b%\t{tuple.Item2} ({BadCounts[tuple.Item2] + GoodCounts[tuple.Item2]})";
                }
            }
            return res;
        }
    }
}
