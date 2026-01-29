namespace Neo.Models.Wallets
{
    /// <summary>
    /// Transfer request model
    /// </summary>
    public class TransferRequest
    {
        /// <summary>Receiver address</summary>
        public UInt160? Receiver { get; set; }

        /// <summary>Transfer amount</summary>
        public string? Amount { get; set; }

        /// <summary>Asset hash or symbol</summary>
        public string? Asset { get; set; }

        /// <summary>Sender address</summary>
        public UInt160? Sender { get; set; }
    }
}
