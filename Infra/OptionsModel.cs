using OpenAI_API.Images;

using System.Collections.Generic;

namespace Dalle3
{
    class OptionsModel
    {
        public string RawPrompt { get; set; }
        public IEnumerable<IPromptSection> PromptSections{ get; set; }
        public bool IncludeNormalImageOutput { get; set; }
        public bool IncludeAnnotatedImageOutput { get; set; }

        public int ImageNumber { get; set; }
        /// <summary>
        /// Random = sample randomly, this false means iterate in order.
        /// </summary>
        public bool Random { get; set; } = true; //this HAS to be true otherwise it breaks for now, fix this bug.
        public ImageSize Size { get; set; }

        //hd, standard
        public string Quality { get; set; } = "standard";

    }

}
