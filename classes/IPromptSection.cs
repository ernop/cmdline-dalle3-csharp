using OpenAI_API.Moderation;

using System.Collections.Generic;

namespace Dalle3
{
    public interface IPromptSection
    {
        /// <summary>
        /// Pick a random value from the set of possible values.
        /// </summary>
        IEnumerable<InternalTextSection> Sample();

        /// <summary>
        /// Iterate over all possible values.
        /// </summary>
        IEnumerable<InternalTextSection> Iterate();

        /// <summary>
        /// For tracking failures by choices etc.
        /// </summary>
        void ReceiveChoiceResult(string choice, TextChoiceResultEnum result);

        /// <summary>
        /// Outputting basic content filtering, etc. types of things.
        /// </summary>
        string ReportResults();
    }
}
