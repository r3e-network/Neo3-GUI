using System.Collections.Generic;

namespace Neo.Models.Wallets
{
    /// <summary>
    /// Address balance model
    /// </summary>
    public class AddressBalanceModel
    {
        /// <summary>Address hash</summary>
        public UInt160? AddressHash { get; set; }

        /// <summary>Address string</summary>
        public string? Address => AddressHash?.ToAddress();

        /// <summary>Asset balances</summary>
        public List<AssetBalanceModel> Balances { get; set; } = new();
    }
}
