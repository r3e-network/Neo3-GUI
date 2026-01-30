using Neo.SmartContract.Native;

namespace Neo.Services.Strategies
{
    /// <summary>
    /// Asset conversion strategy interface
    /// </summary>
    public interface IAssetConversionStrategy
    {
        UInt160 Convert(string asset, out string error);
    }

    public class AssetConversionStrategy : IAssetConversionStrategy
    {
        public UInt160 Convert(string asset, out string error)
        {
            error = "";
            
            if ("neo".Equals(asset, StringComparison.OrdinalIgnoreCase))
                return NativeContract.NEO.Hash;
            
            if ("gas".Equals(asset, StringComparison.OrdinalIgnoreCase))
                return NativeContract.GAS.Hash;

            try
            {
                return UInt160.Parse(asset);
            }
            catch (Exception e)
            {
                error = e.Message;
                return null;
            }
        }
    }
}
