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
        string ConnectionId { get; }
        void PushMessage(WsMessage message);
    }

    /// <summary>
    /// WebSocket connection wrapper with message queue support
    /// </summary>
    public class WebSocketConnection : IWebSocketConnection, IDisposable
    {
        private const int BufferSize = 4 * 1024;
        private const int MaxQueueSize = 1000;
        private const int SendTimeoutMs = 5000;
        private const string GuidFormat = "N";

        private readonly WebSocket _socket;
        private readonly BlockingCollection<WsMessage> _messageQueue;
        private readonly ArraySegment<byte> _buffer;
        private readonly CancellationTokenSource _disposeCts;
        private readonly SemaphoreSlim _sendLock;
        
        private volatile bool _disposed;
        private DateTime _lastActivityTime;

        /// <summary>
        /// Unique connection identifier
        /// </summary>
        public string ConnectionId { get; }

        /// <summary>
        /// Gets the last activity time
        /// </summary>
        public DateTime LastActivityTime => _lastActivityTime;

        /// <summary>
        /// Gets the current queue size
        /// </summary>
        public int QueueSize => _messageQueue.Count;

        /// <summary>
        /// Gets whether the connection is open
        /// </summary>
        public bool IsOpen => _socket.State == WebSocketState.Open && !_disposed;

        public WebSocketConnection(WebSocket socket)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _messageQueue = new BlockingCollection<WsMessage>(MaxQueueSize);
            _buffer = WebSocket.CreateServerBuffer(BufferSize);
            _disposeCts = new CancellationTokenSource();
            _sendLock = new SemaphoreSlim(1, 1);
            
            ConnectionId = Guid.NewGuid().ToString(GuidFormat);
            _lastActivityTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Queue a message for sending
        /// </summary>
        public void PushMessage(WsMessage message)
        {
            if (message == null || _disposed) return;

            // Try to add without blocking, drop if queue is full
            if (!_messageQueue.TryAdd(message))
            {
                Console.WriteLine($"[WebSocket] Queue full for {ConnectionId}, dropping message");
            }
        }

        /// <summary>
        /// Process the message queue (call from dedicated thread)
        /// </summary>
        public async Task PushLoop()
        {
            try
            {
                foreach (var msg in _messageQueue.GetConsumingEnumerable(_disposeCts.Token))
                {
                    if (_disposed) break;
                    await SendAsync(msg);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected on dispose
            }
        }

        /// <summary>
        /// Send message directly with timeout
        /// </summary>
        private async Task SendAsync(object data)
        {
            if (_disposed || _socket.State != WebSocketState.Open) return;

            await _sendLock.WaitAsync(_disposeCts.Token);
            try
            {
                using var timeoutCts = CancellationTokenSource
                    .CreateLinkedTokenSource(_disposeCts.Token);
                timeoutCts.CancelAfter(SendTimeoutMs);

                await _socket.SendAsync(data, timeoutCts.Token);
                _lastActivityTime = DateTime.UtcNow;
            }
            finally
            {
                _sendLock.Release();
            }
        }

        /// <summary>
        /// Close the connection gracefully
        /// </summary>
        public async Task CloseAsync(WebSocketCloseStatus closeStatus, string closeDescription)
        {
            if (_disposed) return;

            try
            {
                if (_socket.State == WebSocketState.Open ||
                    _socket.State == WebSocketState.CloseReceived)
                {
                    using var timeoutCts = new CancellationTokenSource(SendTimeoutMs);
                    await _socket.CloseAsync(closeStatus, closeDescription, timeoutCts.Token);
                }
            }
            catch (Exception)
            {
                // Ignore close errors
            }
        }

        /// <summary>
        /// Receive string from client
        /// </summary>
        public async Task<WebSocketStringResult> ReceiveStringAsync()
        {
            var receiveResult = await _socket.ReceiveAsync(_buffer, _disposeCts.Token);
            _lastActivityTime = DateTime.UtcNow;

            var result = new WebSocketStringResult(
                receiveResult.Count,
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                receiveResult.CloseStatus,
                receiveResult.CloseStatusDescription);

            if (result.EndOfMessage && result.Count > 0)
            {
                result.Message = Encoding.UTF8.GetString(_buffer.Array!, 0, result.Count);
            }
            else if (!result.EndOfMessage)
            {
                throw new InvalidOperationException("Message exceeds maximum buffer size");
            }

            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                _disposeCts.Cancel();
                _messageQueue.CompleteAdding();
                
                // Give queue time to drain
                Thread.Sleep(100);
                
                _messageQueue.Dispose();
                _disposeCts.Dispose();
                _sendLock.Dispose();
            }
        }
    }
}
