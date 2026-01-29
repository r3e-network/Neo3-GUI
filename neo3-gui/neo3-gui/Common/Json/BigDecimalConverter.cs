using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Neo.Common.Json
{
    public class BigDecimalConverter:JsonConverter<BigDecimal>
    {

        public override BigDecimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, BigDecimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
