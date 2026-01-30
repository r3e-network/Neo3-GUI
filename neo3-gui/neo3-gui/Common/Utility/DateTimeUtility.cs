namespace Neo.Common.Utility
{
    /// <summary>
    /// DateTime utility methods
    /// </summary>
    public static class DateTimeUtility
    {
        public static DateTime UtcNow => DateTime.UtcNow;
        
        public static long ToUnixTimestamp(DateTime dt)
        {
            return ((DateTimeOffset)dt).ToUnixTimeSeconds();
        }
    }
}
