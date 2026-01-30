using System.Numerics;
using Neo.SmartContract.Native;

namespace Neo.Extensions
{
    /// <summary>
    /// BigInteger extension methods
    /// </summary>
    public static class BigIntegerExtensions
    {
        public static BigDecimal ToNeo(this BigInteger amount)
        {
            return new BigDecimal(amount, NativeContract.NEO.Decimals);
        }

        public static BigDecimal ToGas(this BigInteger amount)
        {
            return new BigDecimal(amount, NativeContract.GAS.Decimals);
        }
    }
}
