using System.Numerics;
using Neo.Common.Utility;
using Neo.Persistence;
using Neo.Services.Abstractions;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Services.Core
{
    /// <summary>
    /// Balance query service implementation
    /// </summary>
    public class BalanceService : IBalanceService
    {
        private readonly IBlockchainService _blockchainService;

        public BalanceService(IBlockchainService blockchainService)
        {
            _blockchainService = blockchainService;
        }

        public BigDecimal GetBalanceOf(UInt160 address, UInt160 assetId)
        {
            var snapshot = _blockchainService.GetSnapshot();
            return GetBalanceOf(address, assetId, snapshot);
        }

        public List<BigDecimal> GetBalanceOf(IEnumerable<UInt160> addresses, UInt160 assetId)
        {
            var snapshot = _blockchainService.GetSnapshot();
            return GetBalanceOf(addresses, assetId, snapshot);
        }

        public BigInteger GetUnclaimedGas(UInt160 account, uint blockIndex)
        {
            var snapshot = _blockchainService.GetSnapshot();
            return Helpers.GetUnclaimedGas(snapshot, account, blockIndex);
        }

        private BigDecimal GetBalanceOf(UInt160 address, UInt160 assetId, DataCache snapshot)
        {
            var assetInfo = AssetCache.GetAssetInfo(assetId, snapshot);
            if (assetInfo == null)
                return new BigDecimal(BigInteger.Zero, 0);

            using var sb = new ScriptBuilder();
            sb.EmitDynamicCall(assetId, "balanceOf", address);
            using var engine = sb.ToArray().RunTestMode(snapshot);
            
            if (engine.State.HasFlag(VMState.FAULT))
                return new BigDecimal(BigInteger.Zero, 0);

            var result = engine.ResultStack.Pop();
            var balance = result == StackItem.Null ? 0 : result.GetInteger();
            return new BigDecimal(balance, assetInfo.Decimals);
        }

        private List<BigDecimal> GetBalanceOf(IEnumerable<UInt160> addresses, UInt160 assetId, DataCache snapshot)
        {
            var assetInfo = AssetCache.GetAssetInfo(assetId, snapshot);
            if (assetInfo == null)
                throw new ArgumentException($"Invalid assetId: {assetId}");

            if (assetInfo.Asset == NativeContract.NEO.Hash)
                return GetNativeBalanceOf(addresses, NativeContract.NEO, snapshot);
            
            if (assetInfo.Asset == NativeContract.GAS.Hash)
                return GetNativeBalanceOf(addresses, NativeContract.GAS, snapshot);

            using var sb = new ScriptBuilder();
            foreach (var address in addresses)
            {
                sb.EmitDynamicCall(assetId, "balanceOf", address);
            }

            using var engine = sb.ToArray().RunTestMode(snapshot);
            if (engine.State.HasFlag(VMState.FAULT))
                throw new Exception("Query balance error");

            return engine.ResultStack
                .Select(p => new BigDecimal(p.GetInteger(), assetInfo.Decimals))
                .ToList();
        }

        private static List<BigDecimal> GetNativeBalanceOf<T>(
            IEnumerable<UInt160> addresses, 
            FungibleToken<T> asset, 
            DataCache snapshot) where T : AccountState, new()
        {
            return addresses
                .Select(account => new BigDecimal(asset.BalanceOf(snapshot, account), asset.Decimals))
                .ToList();
        }
    }
}
