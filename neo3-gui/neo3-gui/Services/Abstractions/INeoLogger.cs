using Microsoft.Extensions.Logging;

namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Structured logging service interface
    /// </summary>
    public interface INeoLogger
    {
        void LogInfo(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(Exception ex, string message, params object[] args);
        void LogDebug(string message, params object[] args);
    }
}
