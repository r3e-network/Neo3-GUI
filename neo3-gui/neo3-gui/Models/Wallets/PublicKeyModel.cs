namespace Neo.Models.Wallets
{
    /// <summary>
    /// Public key model
    /// </summary>
    public class PublicKeyModel
    {
        /// <summary>Public key bytes</summary>
        public byte[]? PublicKey { get; set; }

        /// <summary>Address string</summary>
        public string? Address { get; set; }
    }
}
