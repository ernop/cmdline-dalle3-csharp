using System.Collections.Generic;

namespace Dalle3
{
    class OptionsModel
    {
        public string RawPrompt { get; set; }
        public List<string> EffectivePrompts { get; set; } = new List<string>();
        public int ImageNumber { get; set; }
    }
}
