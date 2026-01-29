namespace Neo.Models.Wallets
{
    /// <summary>
    /// Wallet account model
    /// </summary>
    public class AccountModel
    {
        /// <summary>Script hash</summary>
        public UInt160? ScriptHash { get; set; }

        /// <summary>Account address</summary>
        public string? Address { get; set; }

        /// <summary>Account type</summary>
        public AccountType AccountType { get; set; } = AccountType.Standard;

        /// <summary>Watch-only account</summary>
        public bool WatchOnly { get; set; }

        /// <summary>NEO balance</summary>
        public string Neo { get; set; } = "0";

        /// <summary>GAS balance</summary>
        public string Gas { get; set; } = "0";
    }
}
