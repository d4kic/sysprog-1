using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zip_server.src
{
    internal class Logger
    {
        private static readonly object lockObj = new();
        
        public static void Log(string msg)
        {
            lock(lockObj)
            {
                Console.WriteLine($"[{DateTime.Now}] {msg}");
            }
        }
    }
}
