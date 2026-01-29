namespace Neo.Models.Contracts
{
    /// <summary>
    /// Result containing a transaction ID
    /// </summary>
    public class TxResultModel
    {
        /// <summary>
        /// Transaction hash identifier
        /// </summary>
        public UInt256? TxId { get; set; }
    }
}
