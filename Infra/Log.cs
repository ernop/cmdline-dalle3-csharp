using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Dalle3
{
    public class Logger
    {
        private readonly string Path;
        private static readonly object _lock = new object();
        private StreamWriter _sw;

        public Logger(string path)
        {
            var dirpart = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dirpart))
            {
                Console.WriteLine($"Creating directory: {dirpart} for logs.");
                System.IO.Directory.CreateDirectory(dirpart);
            }
            this.Path = path;
            _sw = new StreamWriter(Path, true);
        }

        public void Log(string message)
        {
            var now = DateTime.Now;
            lock (_lock)
            {
                message = message.Trim();
                _sw.WriteLine($"{now}\t{message}");
                Console.WriteLine($"{now}\t{message}");
            }
        }
    }
}
