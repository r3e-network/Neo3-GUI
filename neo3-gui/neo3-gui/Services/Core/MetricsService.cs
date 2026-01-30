using System.Collections.Concurrent;
using Neo.Services.Abstractions;

namespace Neo.Services.Core
{
    /// <summary>
    /// Performance metrics service implementation
    /// </summary>
    public class MetricsService : IMetricsService
    {
        private long _totalCalls;
        private long _totalErrors;
        private long _totalDurationMs;
        private readonly object _lock = new();

        public void RecordApiCall(string method, long durationMs)
        {
            Interlocked.Increment(ref _totalCalls);
            Interlocked.Add(ref _totalDurationMs, durationMs);
        }

        public void RecordError(string method)
        {
            Interlocked.Increment(ref _totalErrors);
        }

        public MetricsSnapshot GetSnapshot()
        {
            var calls = Interlocked.Read(ref _totalCalls);
            return new MetricsSnapshot
            {
                TotalCalls = calls,
                TotalErrors = Interlocked.Read(ref _totalErrors),
                AvgResponseMs = calls > 0 
                    ? (double)Interlocked.Read(ref _totalDurationMs) / calls 
                    : 0
            };
        }
    }
}
