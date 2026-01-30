using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Neo.Common.Json;
using Neo.Services.Abstractions;

namespace Neo.Services.Core
{
    /// <summary>
    /// JSON serialization service implementation
    /// </summary>
    public class JsonService : IJsonService
    {
        public JsonSerializerOptions Options { get; }

        public JsonService()
        {
            Options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters =
                {
                    new UInt160Converter(),
                    new UInt256Converter(),
                    new NumToStringConverter(),
                    new BigDecimalConverter(),
                    new BigIntegerConverter(),
                    new DatetimeJsonConverter(),
                    new ByteArrayConverter(),
                    new JObjectConverter(),
                    new StackItemConverter(),
                }
            };
        }

        public string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, Options);
        }

        public byte[] SerializeToBytes<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj, Options);
        }

        public T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;
            return JsonSerializer.Deserialize<T>(json, Options);
        }

        public T Deserialize<T>(byte[] jsonBytes)
        {
            if (jsonBytes == null || jsonBytes.Length == 0)
                return default;
            return JsonSerializer.Deserialize<T>(jsonBytes, Options);
        }

        public object Deserialize(string json, Type targetType)
        {
            return JsonSerializer.Deserialize(json, targetType, Options);
        }
    }
}
