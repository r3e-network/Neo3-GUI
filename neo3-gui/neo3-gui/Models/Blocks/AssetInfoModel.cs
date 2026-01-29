using System;

namespace Neo.Models.Blocks
{
    /// <summary>
    /// Extended asset information model
    /// </summary>
    public class AssetInfoModel : AssetInfo
    {
        /// <summary>Asset creation time</summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>Total supply</summary>
        public BigDecimal? TotalSupply { get; set; }

        /// <summary>Transaction count</summary>
        public int TransactionCount { get; set; }
    }
}
