using Microsoft.Extensions.Caching.Memory;
using Neo.Services.Abstractions;

namespace Neo.Services.Core
{
    /// <summary>
    /// Memory cache service implementation
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(5);

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            return _cache.TryGetValue(key, out T value) ? value : default;
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            _cache.Set(key, value, expiry ?? DefaultExpiry);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }
    }
}
