using System.Numerics;
using Neo.SmartContract.Native;

namespace Neo.Services.Abstractions
{
    /// <summary>
    /// Balance query service interface
    /// </summary>
    public interface IBalanceService
    {
        BigDecimal GetBalanceOf(UInt160 address, UInt160 assetId);
        List<BigDecimal> GetBalanceOf(IEnumerable<UInt160> addresses, UInt160 assetId);
        BigInteger GetUnclaimedGas(UInt160 account, uint blockIndex);
    }
}
