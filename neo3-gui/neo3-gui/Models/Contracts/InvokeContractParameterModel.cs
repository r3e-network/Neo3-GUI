using System.Collections.Generic;
using System.Text.Json;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// Parameters for invoking a smart contract
    /// </summary>
    public class InvokeContractParameterModel
    {
        /// <summary>
        /// Hash of the contract to invoke
        /// </summary>
        public UInt160? ContractHash { get; set; }

        /// <summary>
        /// Method name to call
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// Method parameters as JSON elements
        /// </summary>
        public List<JsonElement>? Parameters { get; set; }

        /// <summary>
        /// Co-signers for the transaction
        /// </summary>
        public List<CosignerModel>? Cosigners { get; set; }

        /// <summary>
        /// Whether to broadcast the transaction
        /// </summary>
        public bool SendTx { get; set; }
    }
}
