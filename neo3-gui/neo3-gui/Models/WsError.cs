namespace Neo.Models
{
    /// <summary>
    /// Represents a WebSocket error response
    /// </summary>
    public class WsError
    {
        /// <summary>
        /// Error code identifying the type of error
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Human-readable error message
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Creates a new WsError from an ErrorCode enum value
        /// </summary>
        public static WsError FromCode(ErrorCode code)
        {
            return code.ToError();
        }
    }
}
