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
        /// This refers to the order that images are generated. Going random gives you faster visibility into coverage
        /// </summary>
        public bool Random { get; set; } = false;
        public ImageSize Size { get; set; }
        
        //hd, standard
        public string Quality { get; set; } 
    }

}
