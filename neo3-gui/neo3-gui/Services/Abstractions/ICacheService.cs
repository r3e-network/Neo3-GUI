namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Cache service interface
    /// </summary>
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expiry = null);
        void Remove(string key);
        bool Exists(string key);
    }
}
