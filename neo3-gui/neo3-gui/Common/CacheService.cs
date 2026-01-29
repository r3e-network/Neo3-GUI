using System;
using Microsoft.Extensions.Caching.Memory;

namespace Neo.Common
{
    /// <summary>
    /// Simple cache service wrapper
    /// </summary>
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expiration = null);
        void Remove(string key);
    }

    public class MemoryCacheService : ICacheService
    {
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public T Get<T>(string key)
        {
            return _cache.TryGetValue(key, out T value) ? value : default;
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
            };
            _cache.Set(key, value, options);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
