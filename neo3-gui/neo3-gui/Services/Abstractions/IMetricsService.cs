namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Performance metrics service interface
    /// </summary>
    public interface IMetricsService
    {
        void RecordApiCall(string method, long durationMs);
        void RecordError(string method);
        MetricsSnapshot GetSnapshot();
    }

    public class MetricsSnapshot
    {
        public long TotalCalls { get; set; }
        public long TotalErrors { get; set; }
        public double AvgResponseMs { get; set; }
    }
}
