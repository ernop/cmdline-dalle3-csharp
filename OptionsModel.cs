using System.Collections.Generic;

namespace Dalle3
{
    class OptionsModel
    {
        public string RawPrompt { get; set; }
        public List<string> EffectivePrompts { get; set; } = new List<string>();
        public int ImageNumber { get; set; }
        /// <summary>
        /// This refers to the order that images are generated. Going random gives you faster visibility into coverage
        /// </summary>
        public bool Random { get; set; } = false;
    }
}
