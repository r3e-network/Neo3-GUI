using System.Linq;
using Neo.SmartContract.Manifest;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// Contract ABI (Application Binary Interface) model
    /// </summary>
    public class ContractAbiModel
    {
        /// <summary>
        /// Creates a ContractAbiModel from a ContractAbi
        /// </summary>
        public ContractAbiModel(ContractAbi? abi)
        {
            if (abi != null)
            {
                Methods = abi.Methods.Select(m => new ContractMethodModel(m)).ToArray();
                Events = abi.Events.Select(m => new ContractEventModel(m)).ToArray();
            }
        }

        /// <summary>
        /// Contract entry point method
        /// </summary>
        public ContractMethodModel? EntryPoint { get; set; }

        /// <summary>
        /// Contract methods
        /// </summary>
        public ContractMethodModel[]? Methods { get; set; }

        /// <summary>
        /// Contract events
        /// </summary>
        public ContractEventModel[]? Events { get; set; }
    }
}
