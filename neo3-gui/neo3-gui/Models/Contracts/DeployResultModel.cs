namespace Neo.Models.Contracts
{
    /// <summary>
    /// Result of a contract deployment operation
    /// </summary>
    public class DeployResultModel
    {
        /// <summary>
        /// Transaction ID of the deployment
        /// </summary>
        public UInt256? TxId { get; set; }

        /// <summary>
        /// Hash of the deployed contract
        /// </summary>
        public UInt160? ContractHash { get; set; }

        /// <summary>
        /// GAS consumed by the deployment
        /// </summary>
        public BigDecimal GasConsumed { get; set; }
    }
}
