using System.Linq;
using System.Text.Json.Serialization;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// Represents a contract method descriptor
    /// </summary>
    public class ContractMethodModel : ContractEventModel
    {
        /// <summary>
        /// Creates a new ContractMethodModel from a descriptor
        /// </summary>
        public ContractMethodModel(ContractMethodDescriptor? method) : base(method)
        {
            if (method != null)
            {
                Offset = method.Offset;
                Safe = method.Safe;
                ReturnType = method.ReturnType;
            }
        }

        /// <summary>
        /// Byte offset in the script
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Whether this method is safe (read-only)
        /// </summary>
        public bool Safe { get; set; }

        /// <summary>
        /// Return type of the method
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContractParameterType ReturnType { get; set; }
    }
}
