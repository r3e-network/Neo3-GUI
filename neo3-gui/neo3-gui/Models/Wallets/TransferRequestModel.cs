namespace Neo.Models.Wallets
{
    /// <summary>
    /// Transfer request with typed amount
    /// </summary>
    public class TransferRequestModel
    {
        /// <summary>Receiver address</summary>
        public UInt160? Receiver { get; set; }

        /// <summary>Transfer amount</summary>
        public BigDecimal Amount { get; set; }

        /// <summary>Asset hash</summary>
        public UInt160? Asset { get; set; }

        /// <summary>Sender address</summary>
        public UInt160? Sender { get; set; }
    }
}
