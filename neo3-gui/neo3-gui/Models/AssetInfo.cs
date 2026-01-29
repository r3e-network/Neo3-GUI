using Neo.Common.Storage.SQLiteModules;

namespace Neo.Models
{
    public class AssetInfo
    {
        public UInt160 Asset { get; set; }

        public string Name { get; set; }
        public string Symbol { get; set; }

        public byte Decimals { get; set; }

        public AssetType Type { get; set; }
    }
}
