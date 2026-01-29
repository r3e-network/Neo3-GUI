using System.Text.Json.Serialization;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// Represents a contract method parameter
    /// </summary>
    public class ContractParameterModel
    {
        /// <summary>
        /// Creates a new ContractParameterModel from a definition
        /// </summary>
        public ContractParameterModel(ContractParameterDefinition? parameter)
        {
            if (parameter != null)
            {
                Name = parameter.Name;
                Type = parameter.Type;
            }
        }

        /// <summary>
        /// Parameter name identifier
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Parameter type (Signature, Boolean, Integer, Hash160, etc.)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContractParameterType Type { get; set; }
    }
}
