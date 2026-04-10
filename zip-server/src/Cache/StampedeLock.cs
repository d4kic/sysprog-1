using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zip_server.src.Cache
{
    internal static class StampedeLock
    {
        private static readonly ConcurrentDictionary<string, object> locks = new();

        public static object Get(string key)
        {
            return locks.GetOrAdd(key, _ => new object());
        }
    }
}
