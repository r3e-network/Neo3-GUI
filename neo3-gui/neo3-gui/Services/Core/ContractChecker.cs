using Neo.Common.Storage;
using Neo.Services.Abstractions;
using Neo.SmartContract;

namespace Neo.Services.Core
{
    /// <summary>
    /// Contract checker service implementation
    /// </summary>
    public class ContractChecker : IContractChecker
    {
        public AssetType CheckAssetType(ContractState contract)
        {
            var methods = contract.Manifest.Abi.Methods;
            
            bool hasBase = HasMethod(methods, "totalSupply", 0) &&
                          HasMethod(methods, "symbol", 0) &&
                          HasMethod(methods, "decimals", 0) &&
                          HasMethod(methods, "balanceOf", 1);

            if (!hasBase) return AssetType.None;

            if (HasNep17Transfer(methods))
                return AssetType.Nep17;

            if (HasNep11Transfer(methods) && HasMethod(methods, "tokensOf", 1))
                return AssetType.Nep11;

            return AssetType.None;
        }

        private static bool HasMethod(
            IEnumerable<ContractMethodDescriptor> methods, 
            string name, 
            int paramCount)
        {
            return methods.Any(m => 
                m.Name == name && m.Parameters.Length == paramCount);
        }

        private static bool HasNep17Transfer(
            IEnumerable<ContractMethodDescriptor> methods)
        {
            return methods.Any(m => 
                m.Name == "transfer" && m.Parameters.Length == 4);
        }

        private static bool HasNep11Transfer(
            IEnumerable<ContractMethodDescriptor> methods)
        {
            return methods.Any(m => 
                m.Name == "transfer" && 
                m.Parameters.Length == 3 &&
                m.Parameters[0].Type == ContractParameterType.Hash160);
        }
    }
}
