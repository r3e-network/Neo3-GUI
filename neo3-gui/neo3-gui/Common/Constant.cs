using System;

namespace Neo.Common
{
    /// <summary>
    /// Application-wide constants
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// Maximum GAS for test mode execution (2000 GAS)
        /// </summary>
        public const long TestMode = 2000_00000000;

        /// <summary>
        /// Default page size for pagination
        /// </summary>
        public const int DefaultPageSize = 100;

        /// <summary>
        /// Default page index (1-based)
        /// </summary>
        public const int DefaultPageIndex = 1;

        /// <summary>
        /// WebSocket buffer size in bytes
        /// </summary>
        public const int WebSocketBufferSize = 4 * 1024;

        /// <summary>
        /// Maximum WebSocket connections
        /// </summary>
        public const int MaxWebSocketConnections = 10;

        /// <summary>
        /// Heartbeat interval in seconds
        /// </summary>
        public const int HeartbeatIntervalSeconds = 3;

        /// <summary>
        /// Default operation timeout in milliseconds
        /// </summary>
        public const int DefaultTimeoutMs = 5000;

        /// <summary>
        /// Maximum retry attempts for operations
        /// </summary>
        public const int MaxRetryAttempts = 3;
    }
}
