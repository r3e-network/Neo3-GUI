namespace Neo.Models.Contracts
{
    /// <summary>
    /// Represents a consensus validator node
    /// </summary>
    public class ValidatorModel
    {
        /// <summary>
        /// Public key of the validator (hex encoded)
        /// </summary>
        public string? Publickey { get; set; }

        /// <summary>
        /// Number of votes received by this validator
        /// </summary>
        public string? Votes { get; set; }

        /// <summary>
        /// Whether this validator is currently active in consensus
        /// </summary>
        public bool Active { get; set; }
    }
}
