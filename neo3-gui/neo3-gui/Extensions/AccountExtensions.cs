using Neo.Models.Wallets;
using Neo.Wallets;

namespace Neo.Extensions
{
    /// <summary>
    /// Account mapping extensions
    /// </summary>
    public static class AccountExtensions
    {
        public static AccountModel ToModel(this WalletAccount account)
        {
            return new AccountModel
            {
                Address = account.Address,
                ScriptHash = account.ScriptHash,
                WatchOnly = account.WatchOnly
            };
        }
    }
}
