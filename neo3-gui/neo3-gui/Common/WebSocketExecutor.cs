using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Neo.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.Services;

namespace Neo.Common
{
    /// <summary>
    /// WebSocket API method executor with caching
    /// </summary>
    public class WebSocketExecutor
    {
        private const string MethodNotFoundTemplate = "Method [{0}] not found!";
        private const string MethodRegFailedTemplate = "Failed to register method {0}: {1}";

        private readonly Dictionary<string, MethodMetadata> _methods;
        private readonly IServiceProvider _provider;
        private readonly ILogger<WebSocketExecutor> _logger;

        public WebSocketExecutor(
            IServiceProvider provider,
            ILogger<WebSocketExecutor> logger = null)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger;
            _methods = new Dictionary<string, MethodMetadata>(StringComparer.OrdinalIgnoreCase);
            
            RegisterMethods();
        }

        private void RegisterMethods()
        {
            var invokerType = typeof(IApiService);
            var types = invokerType.Assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && t != invokerType && invokerType.IsAssignableFrom(t));

            foreach (var type in types)
            {
                RegisterTypeMethods(type);
            }

            _logger?.LogInformation("Registered {Count} API methods", _methods.Count);
        }

        private void RegisterTypeMethods(Type type)
        {
            var methods = type.GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (var methodInfo in methods)
            {
                try
                {
                    var metadata = new MethodMetadata(methodInfo);
                    if (metadata.IsValid)
                    {
                        _methods[methodInfo.Name] = metadata;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, MethodRegFailedTemplate, methodInfo.Name, ex.Message);
                    throw;
                }
            }
        }

        public async Task<object> Execute(WsRequest request)
        {
            if (request == null)
            {
                return ErrorCode.InvalidPara.ToError();
            }

            if (string.IsNullOrEmpty(request.Method))
            {
                return ErrorCode.MethodNotFound.ToError();
            }

            if (!_methods.TryGetValue(request.Method, out var method))
            {
                return new WsError
                {
                    Code = (int)ErrorCode.MethodNotFound,
                    Message = string.Format(MethodNotFoundTemplate, request.Method)
                };
            }

            return await ExecuteMethod(method, request);
        }

        private async Task<object> ExecuteMethod(MethodMetadata method, WsRequest request)
        {
            var instance = _provider.GetService(method.DeclaringType);
            
            if (instance is ApiService invoker)
            {
                var session = _provider.GetService<WebSocketSession>();
                invoker.Client = session?.Connection;
            }

            return await method.Invoke(instance, request);
        }
    }
}
