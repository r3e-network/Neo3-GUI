using System.Text.Json.Serialization;
using Neo.SmartContract;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// Contract parameter with type and value
    /// </summary>
    public class ContractParameterValueModel
    {
        /// <summary>
        /// Parameter type
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContractParameterType Type { get; set; }

        /// <summary>
        /// Parameter value as string
        /// </summary>
        public string? Value { get; set; }
    }
}
