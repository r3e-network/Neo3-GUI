using System.Numerics;
using Neo.Common.Utility;
using Neo.Models;
using Neo.Services.Abstractions;

namespace Neo.Services.Core
{
    /// <summary>
    /// Asset information service implementation
    /// </summary>
    public class AssetService : IAssetService
    {
        private readonly IBlockchainService _blockchain;

        public AssetService(IBlockchainService blockchain)
        {
            _blockchain = blockchain;
        }

        public AssetInfo GetAssetInfo(UInt160 assetId)
        {
            return AssetCache.GetAssetInfo(assetId, _blockchain.GetSnapshot());
        }

        public (BigDecimal amount, AssetInfo asset) GetAssetAmount(
            BigInteger amount, 
            UInt160 assetId)
        {
            var asset = GetAssetInfo(assetId);
            if (asset == null)
                return (new BigDecimal(BigInteger.Zero, 0), null);

            return (new BigDecimal(amount, asset.Decimals), asset);
        }
    }
}
