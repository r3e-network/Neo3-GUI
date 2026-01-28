using System;

namespace Neo.Models
{
    /// <summary>
    /// WebSocket exception with error code
    /// </summary>
    public class WsException : Exception
    {
        public int Code { get; }

        public WsException(ErrorCode code) : this(code, code.ToError().Message)
        {
        }

        public WsException(ErrorCode code, string message) : this((int)code, message)
        {
        }

        private WsException(int code, string message) : base(message)
        {
            Code = code;
        }

        public override string ToString()
        {
            return $"WsException[{Code}]: {Message}";
        }
    }
}
