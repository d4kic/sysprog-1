using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zip_server.src.Cache
{
    internal class CacheItem
    {
        public byte[]? Data { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
