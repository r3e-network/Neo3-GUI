using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.Models;

namespace Neo.Common
{
    /// <summary>
    /// Middleware for handling WebSocket connections
    /// </summary>
    public class WebSocketHubMiddleware : IMiddleware
    {
        private const string TooManyConnectionsMessage = "Too many connections!";
        private const int UnknownErrorCode = -1;

        private readonly IServiceProvider _provider;
        private readonly ILogger<WebSocketHubMiddleware> _logger;

        public WebSocketHubMiddleware(
            IServiceProvider provider,
            ILogger<WebSocketHubMiddleware> logger = null)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await next(context);
                return;
            }

            var hub = context.RequestServices.GetService<WebSocketHub>();
            if (hub == null)
            {
                _logger?.LogError("WebSocketHub not registered");
                context.Response.StatusCode = 500;
                return;
            }

            WebSocket webSocket;
            try
            {
                webSocket = await context.WebSockets.AcceptWebSocketAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to accept WebSocket");
                return;
            }

            var client = new WebSocketConnection(webSocket);
            
            if (!hub.Accept(client))
            {
                _logger?.LogWarning("Connection rejected: {Message}", TooManyConnectionsMessage);
                await client.CloseAsync(WebSocketCloseStatus.PolicyViolation, TooManyConnectionsMessage);
                client.Dispose();
                return;
            }

            _logger?.LogInformation("Client connected: {ConnectionId}", client.ConnectionId);

            try
            {
                // Start push loop in background
                var pushTask = Task.Run(async () =>
                {
                    try
                    {
                        await client.PushLoop();
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Push loop error for {ConnectionId}", client.ConnectionId);
                    }
                });

                // Run receive loop
                await ReceiveLoop(client, hub);

                // Wait for push loop to complete
                await Task.WhenAny(pushTask, Task.Delay(1000));
            }
            finally
            {
                hub.Remove(client);
                _logger?.LogInformation("Client disconnected: {ConnectionId}", client.ConnectionId);
            }
        }

        private async Task ReceiveLoop(WebSocketConnection connection, WebSocketHub hub)
        {
            try
            {
                var result = await connection.ReceiveStringAsync();
                
                while (!result.CloseStatus.HasValue && connection.IsOpen)
                {
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        _ = Task.Run(() => ExecuteAsync(connection, result.Message));
                    }
                    result = await connection.ReceiveStringAsync();
                }

                await connection.CloseAsync(
                    result.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                    result.CloseStatusDescription ?? "Connection closed");
            }
            catch (WebSocketException ex)
            {
                _logger?.LogWarning(ex, "WebSocket error for {ConnectionId}", connection.ConnectionId);
            }
            catch (OperationCanceledException)
            {
                // Expected on shutdown
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Receive loop error for {ConnectionId}", connection.ConnectionId);
            }
        }

        private async Task ExecuteAsync(WebSocketConnection connection, string requestString)
        {
            var message = new WsMessage();
            
            try
            {
                var request = requestString.DeserializeJson<WsRequest>();
                if (request == null)
                {
                    message.MsgType = WsMessageType.Error;
                    message.Error = ErrorCode.InvalidPara.ToError();
                    connection.PushMessage(message);
                    return;
                }

                message.MsgType = WsMessageType.Result;
                message.Id = request.Id;
                message.Method = request.Method;

                var session = _provider.GetService<WebSocketSession>();
                session.Connection = connection;
                session.Request = request;

                var executor = _provider.GetService<WebSocketExecutor>();
                var result = await executor.Execute(request);
                
                if (result is WsError error)
                {
                    message.MsgType = WsMessageType.Error;
                    message.Error = error;
                }
                else
                {
                    message.Result = result;
                }
            }
            catch (ArgumentException ex)
            {
                message.MsgType = WsMessageType.Error;
                message.Error = new WsError
                {
                    Code = (int)ErrorCode.InvalidPara,
                    Message = ex.Message,
                };
            }
            catch (WsException wsEx)
            {
                message.MsgType = WsMessageType.Error;
                message.Error = new WsError
                {
                    Code = wsEx.Code,
                    Message = wsEx.Message,
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Execute error");
                message.MsgType = WsMessageType.Error;
                message.Error = new WsError
                {
                    Code = UnknownErrorCode,
                    Message = ex.Message,
                };
            }
            finally
            {
                connection.PushMessage(message);
            }
        }
    }
}
