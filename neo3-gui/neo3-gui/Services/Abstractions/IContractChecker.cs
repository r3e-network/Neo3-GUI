using Neo.Common.Storage;
using Neo.Common.Storage.SQLiteModules;
using Neo.SmartContract;

namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Contract checker service interface
    /// </summary>
    public interface IContractChecker
    {
        AssetType CheckAssetType(ContractState contract);
    }
}
