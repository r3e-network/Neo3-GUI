using Neo.Services.Abstractions;

namespace Neo.Services.ApiServices
{
    /// <summary>
    /// Metrics API service
    /// </summary>
    public class MetricsApiService : ApiService
    {
        private readonly IMetricsService _metrics;

        public MetricsApiService(IMetricsService metrics)
        {
            _metrics = metrics;
        }

        public async Task<object> GetMetrics()
        {
            return _metrics.GetSnapshot();
        }
    }
}
