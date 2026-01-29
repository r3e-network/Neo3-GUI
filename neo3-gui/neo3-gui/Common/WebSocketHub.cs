using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Util;
using Neo.Models;

namespace Neo.Common
{
    public class WebSocketHub
    {
        private const int MaxConnectionLimit = 10;
        private const int HeartBeatIntervalSeconds = 3;
        private const string HeartBeatMessage = "heart beat";

        private readonly ConcurrentDictionary<WebSocketConnection, byte> _clients = new();

        public bool Accept(WebSocketConnection connection)
        {
            lock (_clients)
            {
                if (_clients.Count >= MaxConnectionLimit)
                {
                    return false;
                }
                _clients.TryAdd(connection, 0);
                return true;
            }
        }

        public bool Remove(WebSocketConnection connection)
        {
            lock (_clients)
            {
                var success = _clients.TryRemove(connection, out var removedClient);
                return success;
            }
        }

        private volatile bool _isHeartBeating = false;

        /// <summary>
        /// Start the heartbeat loop
        /// </summary>
        public void StartHeartBeat()
        {
            if (!_isHeartBeating)
            {
                _ = HeartBeatLoop();
            }
        }

        private async Task HeartBeatLoop()
        {
            _isHeartBeating = true;
            while (_isHeartBeating)
            {
                if (_clients.Any())
                {
                    foreach (var client in _clients.Keys)
                    {
                        client.PushMessage(new WsMessage { MsgType = WsMessageType.HeartBeat, Result = HeartBeatMessage });
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(HeartBeatIntervalSeconds));
            }
        }


        /// <summary>
        /// Push Message to all Clients
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void PushAll(WsMessage msg)
        {
            foreach (var client in _clients.Keys)
            {
                client.PushMessage(msg);
            }
        }


        public void StopHeartBeat()
        {
            _isHeartBeating = false;
        }


    }
}
