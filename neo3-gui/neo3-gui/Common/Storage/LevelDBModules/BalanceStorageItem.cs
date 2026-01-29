using System.Numerics;

namespace Neo.Common.Storage.LevelDBModules
{
    public class BalanceStorageItem
    {
        public BigInteger Balance { get; set; }
        public uint Height { get; set; }
    }
}
