using System.Text.Json;

namespace Neo.Services.Abstractions
{
    /// <summary>
    /// JSON serialization service interface
    /// </summary>
    public interface IJsonService
    {
        string Serialize<T>(T obj);
        byte[] SerializeToBytes<T>(T obj);
        T Deserialize<T>(string json);
        T Deserialize<T>(byte[] jsonBytes);
        object Deserialize(string json, Type targetType);
        JsonSerializerOptions Options { get; }
    }
}
