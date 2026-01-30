using Neo.Models;

namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Asset information service interface
    /// </summary>
    public interface IAssetService
    {
        AssetInfo GetAssetInfo(UInt160 assetId);
        (BigDecimal amount, AssetInfo asset) GetAssetAmount(
            System.Numerics.BigInteger amount, 
            UInt160 assetId);
    }
}
