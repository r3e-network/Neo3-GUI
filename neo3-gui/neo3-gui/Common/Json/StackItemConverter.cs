using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Neo.VM.Types;
using Array = Neo.VM.Types.Array;
using Boolean = Neo.VM.Types.Boolean;
using Buffer = Neo.VM.Types.Buffer;

namespace Neo.Common.Json
{
    public class StackItemConverter : JsonConverter<StackItem>
    {
        public const int MaxJsonLength = 10 * 1024 * 1024;

        private const string JsonNameType = "t";
        private const string JsonNameValue = "v";

        public override StackItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            var currentElement = document.RootElement;
            return ReadToStackItem(currentElement);

        }

        public override void Write(Utf8JsonWriter writer, StackItem value, JsonSerializerOptions options)
        {
            try
            {
                WriteToJson(value, writer);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}]");
                writer.Reset();
            }
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(StackItem).IsAssignableFrom(typeToConvert);
        }

        public StackItem ReadToStackItem(JsonElement currentElement)
        {
            if (currentElement.TryGetProperty(JsonNameType, out var type))
            {
                var hasValue = currentElement.TryGetProperty(JsonNameValue, out var value);
                switch (type.GetInt32())
                {
                    case (int)StackItemType.ByteString:
                        return new ByteString(value.GetBytesFromBase64());
                    case (int)StackItemType.Buffer:
                        return new Buffer(value.GetBytesFromBase64());
                    case (int)StackItemType.Boolean:
                        return value.GetBoolean();
                    case (int)StackItemType.Integer:
                        return BigInteger.Parse(value.GetString()!);
                    case (int)StackItemType.Pointer:
                        return new Pointer(null, value.GetInt32());
                    case (int)StackItemType.Any:
                        return hasValue ? new ByteString(value.GetBytesFromBase64()) : StackItem.Null;
                    case (int)StackItemType.Array:
                        var array = new Array();
                        foreach (var jsonElement in value.EnumerateArray())
                        {
                            array.Add(ReadToStackItem(jsonElement));
                        }
                        return array;
                    case (int)StackItemType.Struct:
                        var structs = new Struct();
                        foreach (var jsonElement in value.EnumerateArray())
                        {
                            structs.Add(ReadToStackItem(jsonElement));
                        }
                        return structs;

                    case (int)StackItemType.Map:
                        var map = new Map();
                        foreach (var jsonElement in value.EnumerateArray())
                        {
                            var key = jsonElement.GetProperty("key");
                            var val = jsonElement.GetProperty("val");
                            map[(PrimitiveType)ReadToStackItem(key)] = ReadToStackItem(val);
                        }
                        return map;
                }
            }
            throw new Exception($"Unknown Json {currentElement}");
        }
        
        public void WriteToJson(StackItem value, Utf8JsonWriter writer)
        {
            switch (value)
            {
                case ByteString:
                    writer.WriteStartObject();
                    writer.WriteNumber(JsonNameType, (int)value.Type);
                    writer.WriteBase64String(JsonNameValue, value.GetSpan());
                    writer.WriteEndObject();
                    break;
                case Buffer:
                    writer.WriteStartObject();
                    writer.WriteNumber(JsonNameType, (int)value.Type);
                    writer.WriteBase64String(JsonNameValue, value.GetSpan());
                    writer.WriteEndObject();
                    break;
                case Boolean:
                    writer.WriteStartObject();
                    writer.WriteNumber(JsonNameType, (int)value.Type);
                    writer.WriteBoolean(JsonNameValue, value.GetBoolean());
                    writer.WriteEndObject();
                    break;
                case Integer:
                    writer.WriteStartObject();
                    writer.WriteNumber(JsonNameType, (int)value.Type);
                    writer.WriteString(JsonNameValue, value.GetInteger().ToString());
                    writer.WriteEndObject();
                    break;
                case Pointer point:
                    writer.WriteStartObject();
                    writer.WriteNumber(JsonNameType, (int)value.Type);
                    writer.WriteNumber(JsonNameValue, point.Position);
                    writer.WriteEndObject();
                    break;
                case Struct structs:
                    writer.WriteStartObject();
                    writer.WriteNumber(JsonNameType, (int)value.Type);
                    writer.WritePropertyName(JsonNameValue);
                    writer.WriteStartArray();
                    foreach (var item in structs)
                    {
                        WriteToJson(item, writer);
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    break;
                case Array array:
                    writer.WriteStartObject();
                    writer.WriteNumber(JsonNameType, (int)value.Type);
                    writer.WritePropertyName(JsonNameValue);
                    writer.WriteStartArray();
                    foreach (var item in array)
                    {
                        WriteToJson(item, writer);
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    break;
                case Map map:
                    writer.WriteStartObject();
                    writer.WriteNumber(JsonNameType, (int)value.Type);
                    writer.WritePropertyName(JsonNameValue);
                    writer.WriteStartArray();
                    foreach (var item in map)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("key");
                        WriteToJson(item.Key, writer);
                        writer.WritePropertyName("val");
                        WriteToJson(item.Value, writer);
                        writer.WriteEndObject();

                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    break;
                case InteropInterface op:
                    writer.WriteStartObject();
                    writer.WriteNumber(JsonNameType, (int)value.Type);
                    writer.WriteEndObject();
                    break;
                case Null:
                    writer.WriteStartObject();
                    writer.WriteNumber(JsonNameType, (int)value.Type);
                    writer.WriteEndObject();
                    break;
                default:
                    throw new Exception($"Unknown StackItem {value}");
            }
            if (writer.BytesCommitted > MaxJsonLength)
            {
                throw new InvalidCastException("json is too long to write!");
            }
        }
    }
}
