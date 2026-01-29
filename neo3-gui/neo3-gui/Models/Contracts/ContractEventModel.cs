using System.Linq;
using Neo.SmartContract.Manifest;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// Represents a contract event descriptor
    /// </summary>
    public class ContractEventModel
    {
        /// <summary>
        /// Creates a new ContractEventModel from a descriptor
        /// </summary>
        public ContractEventModel(ContractEventDescriptor? e)
        {
            if (e != null)
            {
                Name = e.Name;
                Parameters = e.Parameters.Select(p => new ContractParameterModel(p)).ToArray();
            }
        }

        /// <summary>
        /// Event name identifier
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Event parameters
        /// </summary>
        public ContractParameterModel[]? Parameters { get; set; }
    }
}
