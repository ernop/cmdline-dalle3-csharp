﻿using OpenAI_API.Moderation;

using System.Collections.Generic;

namespace Dalle3
{
    public interface IPromptSection
    {
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
