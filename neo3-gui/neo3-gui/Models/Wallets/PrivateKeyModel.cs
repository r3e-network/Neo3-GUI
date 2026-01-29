namespace Neo.Models.Wallets
{
    /// <summary>
    /// Private key export model
    /// </summary>
    public class PrivateKeyModel
    {
        /// <summary>Script hash</summary>
        public UInt160? ScriptHash { get; set; }

        /// <summary>Address string</summary>
        public string? Address => ScriptHash?.ToAddress();

        /// <summary>Public key (hex)</summary>
        public string? PublicKey { get; set; }

        /// <summary>Private key (hex)</summary>
        public string? PrivateKey { get; set; }

        /// <summary>WIF format</summary>
        public string? Wif { get; set; }
    }
}
