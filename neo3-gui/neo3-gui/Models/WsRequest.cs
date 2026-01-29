using System.Text.Json;

namespace Neo.Models
{
    /// <summary>
    /// Represents a WebSocket request message
    /// </summary>
    public class WsRequest
    {
        /// <summary>
        /// Unique request identifier for correlation
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Method name to invoke
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// Method parameters as JSON element
        /// </summary>
        public JsonElement Params { get; set; }
    }
}
