namespace Neo.Extensions
{
    /// <summary>
    /// DateTime extension methods
    /// </summary>
    public static class DateTimeExtensions
    {
        private static readonly DateTime UnixEpoch = 
            new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromTimestampMS(this ulong timestamp)
        {
            return UnixEpoch.AddMilliseconds(timestamp);
        }

        public static DateTime AsUtcTime(this DateTime time)
        {
            return DateTime.SpecifyKind(time, DateTimeKind.Utc);
        }
    }
}
