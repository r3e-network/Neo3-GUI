using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Neo.Models;

namespace Neo.Common
{
    /// <summary>
    /// Metadata for API method invocation
    /// </summary>
    public class MethodMetadata
    {
        private readonly ParameterInfo[] _parameters;
        private readonly dynamic _delegate;

        public Type DeclaringType { get; }
        public bool IsValid { get; }

        public MethodMetadata(MethodInfo methodInfo)
        {
            DeclaringType = methodInfo.DeclaringType;
            _parameters = methodInfo.GetParameters();
            
            var paras = new List<Type> { methodInfo.DeclaringType };
            paras.AddRange(_parameters.Select(p => p.ParameterType));
            paras.Add(methodInfo.ReturnType);

            if (methodInfo.ReturnType.IsGenericType && 
                methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                IsValid = true;
                var func = Expression.GetFuncType(paras.ToArray());
                _delegate = Delegate.CreateDelegate(func, methodInfo);
            }
        }

        public dynamic Invoke(object instance, WsRequest request)
        {
            var paras = PrepareParameters(request.Params);
            return InvokeInternal(instance, paras.ToArray());
        }

        private dynamic InvokeInternal(dynamic instance, params dynamic[] paras)
        {
            return _parameters.Length switch
            {
                0 => _delegate(instance),
                1 => _delegate(instance, paras[0]),
                2 => _delegate(instance, paras[0], paras[1]),
                3 => _delegate(instance, paras[0], paras[1], paras[2]),
                4 => _delegate(instance, paras[0], paras[1], paras[2], paras[3]),
                5 => _delegate(instance, paras[0], paras[1], paras[2], paras[3], paras[4]),
                6 => _delegate(instance, paras[0], paras[1], paras[2], paras[3], paras[4], paras[5]),
                _ => Task.FromResult(string.Empty)
            };
        }

        private List<object> PrepareParameters(JsonElement inputParas)
        {
            var paras = new List<object>();
            if (_parameters.Length == 0)
            {
                return paras;
            }
            if (inputParas.ValueKind == JsonValueKind.Undefined)
            {
                paras.AddRange(_parameters.Select(p => p.DefaultValue));
                return paras;
            }

            // Only accept one parameter
            if (_parameters.Length == 1)
            {
                var parameterType = _parameters[0].ParameterType;

                if (inputParas.ValueKind == JsonValueKind.Array && parameterType.IsArray)
                {
                    paras.Add(inputParas.GetRawText().DeserializeJson(parameterType));
                    return paras;
                }

                if (!parameterType.IsPrimitive && !parameterType.IsArray && 
                    !parameterType.IsEnum && parameterType != typeof(string) && 
                    parameterType != typeof(UInt256) && parameterType != typeof(UInt160))
                {
                    paras.Add(inputParas.GetRawText().DeserializeJson(parameterType));
                    return paras;
                }
            }

            // Input para is array, method accepts many parameters
            if (inputParas.ValueKind == JsonValueKind.Array)
            {
                var arrayLength = inputParas.GetArrayLength();
                for (int index = 0; index < _parameters.Length; index++)
                {
                    if (index < arrayLength)
                    {
                        paras.Add(inputParas[index].GetRawText().DeserializeJson(_parameters[index].ParameterType));
                    }
                    else
                    {
                        // Use default value for missing parameters
                        var defaultVal = _parameters[index].DefaultValue;
                        paras.Add(defaultVal != DBNull.Value ? defaultVal : _parameters[index].ParameterType.GetDefaultValue());
                    }
                }
                return paras;
            }

            // Input para is Object, method accepts many parameters
            foreach (var parameterInfo in _parameters)
            {
                if (inputParas.TryGetProperty(parameterInfo.Name, out var paraVal))
                {
                    paras.Add(paraVal.GetRawText().DeserializeJson(parameterInfo.ParameterType));
                }
                else
                {
                    // Try find paraVal case-insensitive
                    var paraToken = inputParas.EnumerateObject()
                        .FirstOrDefault(p => parameterInfo.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
                    
                    if (paraToken.Value.ValueKind != JsonValueKind.Undefined)
                    {
                        paras.Add(paraToken.Value.GetRawText().DeserializeJson(parameterInfo.ParameterType));
                    }
                    else
                    {
                        var defaultVal = parameterInfo.DefaultValue;
                        paras.Add(defaultVal != DBNull.Value ? defaultVal : parameterInfo.ParameterType.GetDefaultValue());
                    }
                }
            }
            return paras;
        }
    }
}
