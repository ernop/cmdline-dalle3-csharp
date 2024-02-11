using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;

namespace Dalle3
{
    public class TextPromptSection : IPromptSection
    {
        private InternalTextSection S { get; set; }
        private int BadCount { get; set; } = 0;
        private int GoodCount { get; set; } = 0;
        public TextPromptSection(string s)
        {
            S = new InternalTextSection(s, s, true, this);
        }

        public IEnumerable<InternalTextSection> Sample() => new List<InternalTextSection>() { S };

        public IEnumerable<IEnumerable<InternalTextSection>> Iterate()
        {
            while (true)
            {
                yield return new List<InternalTextSection>() { S };
            }
        }

        public void ReceiveChoiceResult(string choice, TextChoiceResultEnum result)
        {
            //Console.WriteLine($"I am a thingie and for \"{choice.Substring(0,20)}...\" I got {result}");
            switch (result)
            {
                case TextChoiceResultEnum.RequestBlocked:
                    BadCount++;
                    break;
                case TextChoiceResultEnum.PromptRejected:
                    BadCount++;
                    break;
                case TextChoiceResultEnum.DescriptionsBad:
                    BadCount++;
                    break;
                case TextChoiceResultEnum.Okay:
                    GoodCount++;
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
            var res = $"\t{S.L}";
            if (GoodCount == 0 && BadCount == 0)
            {
                res += " - None yet.";
            }
            else
            {
                res += $"\t{(100 * BadCount / (1.0 * BadCount + GoodCount)):0.0}b%. (b: {BadCount}, g: {GoodCount})";
            }
            return res;
        }

        public override string ToString()
        {
            return $"TextPromptSection:{S}";
        }
    }
}
