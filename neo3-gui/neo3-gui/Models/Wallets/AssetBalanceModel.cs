namespace Neo.Models.Wallets
{
    /// <summary>
    /// Asset balance model
    /// </summary>
    public class AssetBalanceModel
    {
        /// <summary>Asset hash</summary>
        public UInt160? Asset { get; set; }

        /// <summary>Asset symbol</summary>
        public string? Symbol { get; set; }

        /// <summary>Balance amount</summary>
        public BigDecimal Balance { get; set; }
    }
}
