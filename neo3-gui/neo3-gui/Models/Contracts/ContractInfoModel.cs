namespace Neo.Models.Contracts
{
    /// <summary>
    /// Basic contract information
    /// </summary>
    public class ContractInfoModel
    {
        /// <summary>
        /// Contract script hash
        /// </summary>
        public UInt160? Hash { get; set; }

        /// <summary>
        /// Contract name
        /// </summary>
        public string? Name { get; set; }
    }
}
