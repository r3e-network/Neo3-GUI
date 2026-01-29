using System.Collections.Generic;

namespace Neo.Models.Wallets
{
    /// <summary>
    /// Wallet model containing accounts
    /// </summary>
    public class WalletModel
    {
        /// <summary>
        /// Wallet accounts
        /// </summary>
        public List<AccountModel> Accounts { get; set; } = new();
    }
}
