using System;
using Neo.Models;

namespace Neo.Exceptions
{
    /// <summary>
    /// Base exception for all Neo GUI exceptions
    /// </summary>
    public abstract class NeoGuiException : Exception
    {
        public ErrorCode ErrorCode { get; }

        protected NeoGuiException(ErrorCode errorCode)
            : base(errorCode.ToError().Message)
        {
            ErrorCode = errorCode;
        }

        protected NeoGuiException(ErrorCode errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        protected NeoGuiException(ErrorCode errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public WsError ToWsError()
        {
            return new WsError
            {
                Code = (int)ErrorCode,
                Message = Message
            };
        }
    }
}
