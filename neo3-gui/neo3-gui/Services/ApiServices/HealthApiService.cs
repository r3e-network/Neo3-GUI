using Neo.Services.Abstractions;

namespace Neo.Services.ApiServices
{
    /// <summary>
    /// Health check API service
    /// </summary>
    public class HealthApiService : ApiService
    {
        private readonly IHealthCheckService _health;

        public HealthApiService(IHealthCheckService health)
        {
            _health = health;
        }

        public async Task<object> GetHealth()
        {
            return _health.GetStatus();
        }
    }
}
