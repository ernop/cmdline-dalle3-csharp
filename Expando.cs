namespace Dalle3
{
    /// <summary>
    /// This is so you can use one short simple term (S) in the prompt given as input within a permutation block, like {a,b,c}
    /// and it will be used for constructing the filename, but will also automatically be expanded to a broader meaning
    /// when in actual communication to dalle3.
    /// </summary>
    public class Expando
    {
        public string S { get; set; }
        public string L { get; set; }
        public Expando(string s, string l)
        {
            S = s;
            L = l;
        }
    }

}
