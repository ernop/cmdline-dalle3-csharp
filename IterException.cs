using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalle3
{
    /// <summary>
    /// This is a fake exception that is used to signal that we are done iterating.
    /// </summary>
    public class IterException : Exception
    {
        public IterException(string s) : base(s)
        {
        }
    }
}
