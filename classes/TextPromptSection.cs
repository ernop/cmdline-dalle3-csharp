using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;

using static Dalle3.Statics;

namespace Dalle3
{
    public class TextPromptSection : IPromptSection
    {
        private InternalTextSection S { get; set; }
        private int BadCount { get; set; } = 0;
        private int GoodCount { get; set; } = 0;
        private Dictionary<string, int> GoodCounts { get; } = new Dictionary<string, int>();
        private Dictionary<string, int> BadCounts { get; } = new Dictionary<string, int>();
        public TextPromptSection(string s)
        {
            S = new InternalTextSection(s, s, true, this);
        }
        public int GetCount() => 1;
        public InternalTextSection Sample() => S;

        public InternalTextSection Next()
        {
            return S;
        }
        public InternalTextSection Current()
        {
            return S;
        }

        public void ReceiveChoiceResult(InternalTextSection section, TextChoiceResultEnum result)
        {
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

        public string ReportResults(bool includeGlobal)
        {
            var res = "";
            if (includeGlobal)
            {
                res += $"\r\n\t GLOBAL{(100 * BadCount / (1.0 * BadCount + GoodCount)):0.0}b%. (b: {BadCount}, g: {GoodCount})";
            }
            if (GoodCount > 0 || BadCount > 0)
            {
                res += $"\r\n\t{Slice(S.L, SliceAmount)}";
            }
            return res;
        }

        public override string ToString()
        {
            return $"TextPromptSection:{S}";
        }

    }
}
