using System.Collections.Generic;
using System.Linq;

namespace Dalle3
{
    public class Alias
    {
        public string Name { get; set; }
        public IEnumerable<string> Contents { get; set; }
        public Alias(string name, string input)
        {
            Name = name;
            Contents = input.Split(',').Select(el => el.Trim());
        }

        public override string ToString()
        {
            string con;
            if (Contents.Count() == 1)
            {
                con = Contents.First();
            }
            else
            {
                con = $"With {Contents.Count()} items.";
            }

            return $"<{Name}> with contents:{con}";
        }
    }
}