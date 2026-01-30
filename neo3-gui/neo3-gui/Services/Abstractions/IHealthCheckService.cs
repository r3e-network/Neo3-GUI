namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Health check service interface
    /// </summary>
    public interface IHealthCheckService
    {
        HealthStatus GetStatus();
    }

    public class HealthStatus
    {
        public bool IsHealthy { get; set; }
        public uint BlockHeight { get; set; }
        public uint HeaderHeight { get; set; }
        public int ConnectedPeers { get; set; }
        public DateTime Timestamp { get; set; }
        public string Version { get; set; }
    }
}
