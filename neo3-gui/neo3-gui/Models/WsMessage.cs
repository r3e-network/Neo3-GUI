using Neo.Network.P2P.Payloads;

namespace Neo.Models
{
    /// <summary>
    /// Represents a WebSocket message for bidirectional communication
    /// </summary>
    public class WsMessage
    {
        /// <summary>
        /// Message identifier for request-response correlation
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Type of message (Request, Response, Event)
        /// </summary>
        public WsMessageType MsgType { get; set; }

        /// <summary>
        /// Method name associated with this message
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// Result payload for response messages
        /// </summary>
        public object? Result { get; set; }

        /// <summary>
        /// Error information if the request failed
        /// </summary>
        public WsError? Error { get; set; }
    }
}
