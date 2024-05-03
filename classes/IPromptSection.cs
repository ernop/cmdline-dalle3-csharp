using OpenAI_API.Moderation;

using System.Collections.Generic;

namespace Dalle3
{
    public interface IPromptSection
    {
        /// <summary>
        /// Whether or not the inner thingie is a fixed type, i.e. just a blob of text? (and hence not subject to any choice layer).
        /// </summary>
        /// <returns></returns>
        bool IsFixed();

        /// <summary>
        /// okay this is definitely a bad reimplementation.
        /// </summary>
        string myContents();

        /// <summary>
        /// Pick a random value from the set of possible values.
        /// </summary>
        InternalTextSection Sample();

        /// <summary>
        /// Next over all possible values.
        /// </summary>
        InternalTextSection Next();
        int GetCount();

        /// <summary>
        /// For tracking failures by choices etc.
        /// </summary>
        void ReceiveChoiceResult(InternalTextSection section, TextChoiceResultEnum result);

        /// <summary>
        /// Outputting basic content filtering, etc. types of things.
        /// </summary>
        string ReportResults(bool includeGlobal);
    }
}
