using System.Collections.Generic;

namespace Neo.Common.Storage.LevelDBModules
{
    public class ContractEventStorageItem
    {
        public UInt256 TxId { get; set; }
        public List<ContractEventInfo> Events { get; set; }
    }
}
