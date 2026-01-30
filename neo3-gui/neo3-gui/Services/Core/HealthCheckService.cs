using System.Reflection;
using Neo.Services.Abstractions;

namespace Neo.Services.Core
{
    /// <summary>
    /// Health check service implementation
    /// </summary>
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IBlockchainService _blockchainService;

        public HealthCheckService(IBlockchainService blockchainService)
        {
            _blockchainService = blockchainService;
        }

        public HealthStatus GetStatus()
        {
            var blockHeight = _blockchainService.GetCurrentHeight();
            var headerHeight = _blockchainService.GetCurrentHeaderHeight();

            return new HealthStatus
            {
                IsHealthy = headerHeight - blockHeight < 10,
                BlockHeight = blockHeight,
                HeaderHeight = headerHeight,
                ConnectedPeers = GetConnectedPeers(),
                Timestamp = DateTime.UtcNow,
                Version = GetVersion()
            };
        }

        private static int GetConnectedPeers()
        {
            try
            {
                return Program.Starter.LocalNode?.ConnectedCount ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetVersion();
        }
    }
}
