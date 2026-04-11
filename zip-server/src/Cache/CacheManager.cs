using System.Collections.Concurrent;

namespace zip_server.src.Cache
{
    internal static class CacheManager
    {
        private static readonly ConcurrentDictionary<string, CacheItem> cache = new();
        private static readonly TimeSpan ttl = TimeSpan.FromSeconds(30);

        public static bool TryGet(string key, out byte[] data)
        {
            data = null!;

            if (cache.TryGetValue(key, out var item))
            {
                if (DateTime.Now - item.CreatedAt < ttl)
                {
                    data = item.Data!;
                    return true;
                }
            }

            return false;
        }

        public static void Set(string key, byte[] data)
        {
            cache[key] = new CacheItem
            {
                Data = data,
                CreatedAt = DateTime.Now
            };
        }
    }
}
