using Neo.Models.Contracts;
using Neo.SmartContract;

namespace Neo.Extensions
{
    /// <summary>
    /// Contract mapping extensions
    /// </summary>
    public static class ContractExtensions
    {
        public static ContractInfoModel ToInfo(this ContractState contract)
        {
            return new ContractInfoModel
            {
                Hash = contract.Hash,
                Name = contract.Manifest.Name
            };
        }
    }
}
