namespace Neo.Models.Contracts
{
    /// <summary>
    /// Represents an event emitted during contract invocation
    /// </summary>
    public class InvokeEventValueModel
    {
        /// <summary>
        /// Contract that emitted the event
        /// </summary>
        public UInt160? Contract { get; set; }

        /// <summary>
        /// Name of the emitted event
        /// </summary>
        public string? EventName { get; set; }

        /// <summary>
        /// Parameters passed with the event
        /// </summary>
        public object? EventParameters { get; set; }
    }
}
