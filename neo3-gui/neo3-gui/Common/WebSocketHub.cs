using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Neo.Models;

namespace Neo.Common
{
    /// <summary>
    /// WebSocket connection hub for managing client connections and heartbeat
    /// </summary>
    public class WebSocketHub : IDisposable
    {
        private const int MaxConnectionLimit = 10;
        private const int HeartBeatIntervalSeconds = 3;
        private const string HeartBeatMessage = "heart beat";

        private readonly ConcurrentDictionary<WebSocketConnection, byte> _clients = new();
        private readonly CancellationTokenSource _heartbeatCts = new();
        private readonly object _heartbeatLock = new();
        
        private volatile bool _isHeartBeating;
        private bool _disposed;

        /// <summary>
        /// Gets the current number of connected clients
        /// </summary>
        public int ConnectionCount => _clients.Count;

        /// <summary>
        /// Gets whether the heartbeat is currently running
        /// </summary>
        public bool IsHeartBeatRunning => _isHeartBeating;

        /// <summary>
        /// Accept a new WebSocket connection
        /// </summary>
        /// <param name="connection">The connection to accept</param>
        /// <returns>True if accepted, false if connection limit reached</returns>
        public bool Accept(WebSocketConnection connection)
        {
            if (connection == null) return false;
            if (_disposed) return false;

            // ConcurrentDictionary is thread-safe, but we need atomic check-and-add
            lock (_clients)
            {
                if (_clients.Count >= MaxConnectionLimit)
                {
                    return false;
                }
                return _clients.TryAdd(connection, 0);
            }
        }

        /// <summary>
        /// Remove a WebSocket connection
        /// </summary>
        /// <param name="connection">The connection to remove</param>
        /// <returns>True if removed successfully</returns>
        public bool Remove(WebSocketConnection connection)
        {
            if (connection == null) return false;
            
            var success = _clients.TryRemove(connection, out _);
            if (success)
            {
                connection.Dispose();
            }
            return success;
        }

        /// <summary>
        /// Start the heartbeat loop (thread-safe, idempotent)
        /// </summary>
        public void StartHeartBeat()
        {
            if (_disposed) return;

            lock (_heartbeatLock)
            {
                if (!_isHeartBeating)
                {
                    _isHeartBeating = true;
                    _ = HeartBeatLoopAsync(_heartbeatCts.Token);
                }
            }
        }

        private async Task HeartBeatLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    SendHeartBeatToAll();
                    await Task.Delay(
                        TimeSpan.FromSeconds(HeartBeatIntervalSeconds), 
                        cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping
            }
            finally
            {
                _isHeartBeating = false;
            }
        }

        private void SendHeartBeatToAll()
        {
            if (!_clients.Any()) return;

            var heartbeatMsg = new WsMessage 
            { 
                MsgType = WsMessageType.HeartBeat, 
                Result = HeartBeatMessage 
            };

            foreach (var client in _clients.Keys)
            {
                try
                {
                    client.PushMessage(heartbeatMsg);
                }
                catch (Exception)
                {
                    // Client may be disconnected, will be cleaned up
                }
            }
        }

        /// <summary>
        /// Push message to all connected clients
        /// </summary>
        /// <param name="msg">The message to push</param>
        public void PushAll(WsMessage msg)
        {
            if (msg == null || _disposed) return;

            foreach (var client in _clients.Keys)
            {
                try
                {
                    client.PushMessage(msg);
                }
                catch (Exception)
                {
                    // Client may be disconnected
                }
            }
        }

        /// <summary>
        /// Stop the heartbeat loop
        /// </summary>
        public void StopHeartBeat()
        {
            lock (_heartbeatLock)
            {
                if (_isHeartBeating)
                {
                    _heartbeatCts.Cancel();
                }
            }
        }

        /// <summary>
        /// Dispose all resources and close all connections
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                StopHeartBeat();
                _heartbeatCts.Dispose();

                // Close all client connections
                foreach (var client in _clients.Keys)
                {
                    try
                    {
                        client.Dispose();
                    }
                    catch (Exception)
                    {
                        // Ignore disposal errors
                    }
                }
                _clients.Clear();
            }

            _disposed = true;
        }
    }
}
