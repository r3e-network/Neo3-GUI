namespace Neo.Common.Storage
{
    public class ContractEventInfo
    {
        public UInt160 Contract { get; set; }

        /// <summary>
        /// Contract name
        /// </summary>
        public string Name { get; set; }

        public ContractEventType Event { get; set; }
    }
}
