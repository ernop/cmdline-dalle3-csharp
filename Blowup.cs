namespace Dalle3
{
    public class Blowup
    {
        public string Short { get; set; }
        public string Long { get; set; }
        public Blowup(string s, string l)
        {
            Short = s;
            l = l.Trim();
            if (l[0]!='{')
            {
                l = "{" + l;
            }
            if (l[l.Length-1]!='}')
            {
                l = l + "}";
            }
            Long = l;
        }
    }
}
