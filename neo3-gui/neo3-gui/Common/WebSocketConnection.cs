using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neo.Models;

namespace Neo.Common
{
    public interface IWebSocketConnection
    {
        void PushMessage(WsMessage message);
    }

    /// <summary>
    /// WebSocket connection wrapper with message queue support
    /// </summary>
    public class WebSocketConnection : IWebSocketConnection, IDisposable
    {
        private const int BufferSize = 4 * 1024;
        private const string GuidFormat = "N";

        private readonly WebSocket _socket;
        private readonly BlockingCollection<object> _pushMessagesQueue = new BlockingCollection<object>();
        private readonly ArraySegment<byte> _buffer = WebSocket.CreateServerBuffer(BufferSize);
        private bool _disposed;

        public string ConnectionId { get; set; }

        public WebSocketConnection(WebSocket socket)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            ConnectionId = Guid.NewGuid().ToString(GuidFormat);
        }

        /// <summary>
        /// Send message (json format) to connection in queue
        /// </summary>
        /// <param name="message"></param>
        public void PushMessage(WsMessage message)
        {
            if (message != null && !_disposed)
            {
                _pushMessagesQueue.Add(message);
            }
        }

        /// <summary>
        /// Send message queue loop
        /// </summary>
        /// <returns></returns>
        public async Task PushLoop()
        {
            foreach (var msg in _pushMessagesQueue.GetConsumingEnumerable())
            {
                await SendAsync(msg);
            }
        }

        /// <summary>
        /// Send message (json format) to client directly
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task SendAsync(object data)
        {
            await _socket.SendAsync(data);
        }

        /// <summary>
        /// Close this connection
        /// </summary>
        /// <param name="closeStatus"></param>
        /// <param name="closeDescription"></param>
        /// <returns></returns>
        public async Task CloseAsync(WebSocketCloseStatus closeStatus, string closeDescription)
        {
            await _socket.CloseAsync(closeStatus, closeDescription, CancellationToken.None);
        }

        /// <summary>
        /// Receive string from client
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when message exceeds buffer size</exception>
        public async Task<WebSocketStringResult> ReceiveStringAsync()
        {
            var receiveResult = await _socket.ReceiveAsync(_buffer, CancellationToken.None);
            var result = new WebSocketStringResult(
                receiveResult.Count,
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                receiveResult.CloseStatus,
                receiveResult.CloseStatusDescription);

            if (result.EndOfMessage)
            {
                result.Message = Encoding.UTF8.GetString(_buffer.Array, 0, result.Count);
            }
            else
            {
                throw new InvalidOperationException("Message exceeds maximum buffer size");
            }
            return result;
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _pushMessagesQueue.CompleteAdding();
                    _pushMessagesQueue.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
