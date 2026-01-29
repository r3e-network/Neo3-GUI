namespace Neo.Models.Wallets
{
    /// <summary>
    /// Account type enumeration
    /// </summary>
    public enum AccountType
    {
        /// <summary>Non-standard account</summary>
        NonStandard,

        /// <summary>Standard single-signature</summary>
        Standard,

        /// <summary>Multi-signature account</summary>
        MultiSignature,

        /// <summary>Deployed contract</summary>
        DeployedContract,
    }
}
