using OpenAI_API.Moderation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;

using static Dalle3.Statics;

namespace Dalle3
{
    public class TextPromptSection : IPromptSection
    {
        public bool IsFixed() => true;
        private InternalTextSection S { get; set; }
        private int BadCount { get; set; } = 0;
        private int GoodCount { get; set; } = 0;
        private Dictionary<string, List<string>> Results { get; } = new Dictionary<string, List<string>>();
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
        public string myContents() => S.L;
        private string _MyContents { get; set; }
        public InternalTextSection Current()
        {
            return S;
        }

        public void ReceiveChoiceResult(InternalTextSection section, TextChoiceResultEnum result)
        {
            if (!Results.ContainsKey(section.L))
            {
                Results[section.L] = new List<string>();
            }
            Results[section.L].Add(result.ToString());

            switch (result)
            {
                case TextChoiceResultEnum.RequestBlocked:
                case TextChoiceResultEnum.PromptRejected:
                case TextChoiceResultEnum.ImageDescriptionsGeneratedBad:
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

        public string ReportResults(bool includeGlobal)
        {
            var res = "";
            if (includeGlobal)
            {
                res += $"\r\n\t GLOBAL: {(100 * BadCount / (1.0 * BadCount + GoodCount)):0.0}b%. (b: {BadCount}, g: {GoodCount})";
            }
            if (GoodCount > 0 || BadCount > 0)
            {
                res += $"\r\n\t{S.L}";
            }
            return res;
        }

        public override string ToString()
        {
            return $"TextPromptSection:{S}";
        }

    }
}
