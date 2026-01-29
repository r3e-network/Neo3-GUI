using System.Collections.Generic;
using System.Text.Json.Serialization;
using Neo.VM;

namespace Neo.Models.Contracts
{
    /// <summary>
    /// Result of a contract invocation
    /// </summary>
    public class InvokeResultModel
    {
        /// <summary>
        /// Transaction ID if the invocation was broadcast
        /// </summary>
        public UInt256? TxId { get; set; }

        /// <summary>
        /// Hash of the invoked contract
        /// </summary>
        public UInt160? ContractHash { get; set; }

        /// <summary>
        /// GAS consumed by the invocation
        /// </summary>
        public BigDecimal GasConsumed { get; set; }

        /// <summary>
        /// VM execution state after invocation
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public VMState VmState { get; set; }

        /// <summary>
        /// Stack items returned by the contract
        /// </summary>
        public List<JStackItem>? ResultStack { get; set; }

        /// <summary>
        /// Notifications emitted during execution
        /// </summary>
        public List<InvokeEventValueModel>? Notifications { get; set; }
    }
}
