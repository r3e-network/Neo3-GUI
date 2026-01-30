using System.Collections.Concurrent;

namespace Neo.Common.Utility
{
    /// <summary>
    /// Byte array utility methods
    /// </summary>
    public static class ByteArrayUtility
    {
        private static readonly ConcurrentDictionary<byte[], string> Cache = 
            new(ByteArrayValueComparer.Default);

        public static string ToBase64Cached(this byte[] data)
        {
            return Cache.GetOrAdd(data, Convert.ToBase64String);
        }
    }
}
