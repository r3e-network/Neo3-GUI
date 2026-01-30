using Microsoft.Extensions.Logging;
using Neo.Services.Abstractions;

namespace Neo.Services.Core
{
    /// <summary>
    /// Structured logging service implementation
    /// </summary>
    public class NeoLogger : INeoLogger
    {
        private readonly ILogger<NeoLogger> _logger;

        public NeoLogger(ILogger<NeoLogger> logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }
    }
}
