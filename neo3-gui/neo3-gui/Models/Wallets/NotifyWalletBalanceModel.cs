using System.Collections.Generic;

namespace Neo.Models.Wallets
{
    /// <summary>
    /// Wallet balance notification model
    /// </summary>
    public class NotifyWalletBalanceModel
    {
        /// <summary>Unclaimed GAS</summary>
        public BigDecimal UnclaimedGas { get; set; }

        /// <summary>Account list</summary>
        public List<AccountModel>? Accounts { get; set; }
    }
}
