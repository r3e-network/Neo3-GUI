namespace Neo.Models.Wallets
{
    /// <summary>
    /// Multi-receiver transfer request
    /// </summary>
    public class MultiReceiverRequest
    {
        /// <summary>Receiver address</summary>
        public UInt160? Address { get; set; }

        /// <summary>Transfer amount</summary>
        public string? Amount { get; set; }
    }
}
