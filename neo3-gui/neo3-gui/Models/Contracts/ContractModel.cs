using Neo.SmartContract;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// Represents a deployed smart contract
    /// </summary>
    public class ContractModel
    {
        /// <summary>
        /// Creates a ContractModel from a ContractState
        /// </summary>
        public ContractModel(ContractState contract)
        {
            ContractId = contract.Id;
            Script = contract.Script.ToArray();
            Manifest = new ManifestModel(contract.Manifest);
        }

        /// <summary>
        /// Contract ID in the blockchain
        /// </summary>
        public int ContractId { get; set; }

        /// <summary>
        /// Contract script hash
        /// </summary>
        public UInt160? ContractHash { get; set; }

        /// <summary>
        /// Contract bytecode
        /// </summary>
        public byte[]? Script { get; set; }

        /// <summary>
        /// Contract manifest
        /// </summary>
        public ManifestModel? Manifest { get; set; }
    }
}
