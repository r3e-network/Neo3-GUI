using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Neo.Common.Json
{
    public class UInt160Converter : JsonConverter<UInt160>
    {
        public override UInt160 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var data = reader.GetString();
            if (UInt160.TryParse(data, out var hash))
            {
                return hash;
            }
            try
            {
                return data.ToScriptHash();
            }
            catch (Exception)
            {
            }

            throw new ArgumentException($"invalid uint160 string:{data}");
        }

        public override void Write(Utf8JsonWriter writer, UInt160 value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
