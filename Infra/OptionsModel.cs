using OpenAI_API.Images;

using System;
using System.Collections.Generic;

namespace Dalle3
{

    /// <summary>
    /// Data on a "run" of a specific prompt which may be blown up into tons of prompts.
    /// </summary>
    public class OptionsModel
    {
        public string RawPrompt { get; set; }
        public IEnumerable<IPromptSection> PromptSections { get; set; }
        public bool IncludeNormalImageOutput { get; set; }
        public bool IncludeAnnotatedImageOutput { get; set; }
        public int ImageNumber { get; set; }

        /// <summary>
        /// Random = sample randomly, this false means iterate in order.
        /// </summary>
        public bool Random { get; set; } = false;
        public ImageSize Size { get; set; }

        //hd, standard
        public string Quality { get; set; } = "standard";
        
        //vivid is super intense, and not ideal for some use cases.
        //otoh natural is super weird too.
        public string Style { get; set; } = "natural";

        /// <summary>
        /// for categorization by filtering status, etc.
        /// </summary>
        public Dictionary<string, int> Results { get; private set; } = new Dictionary<string, int>();

        public void IncStr(string key)
        {
            if (Results == null)
            {
                Results = new Dictionary<string, int>();
            }
            if (!Results.ContainsKey(key))
            {
                try
                {
                    Results[key] = 0;
                }
                catch (Exception ex)
                {
                    var a = 3;
                }
            }
            Results[key]++;
        }

    }

}
