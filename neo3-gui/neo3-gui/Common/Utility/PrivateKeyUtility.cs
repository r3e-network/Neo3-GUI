using Neo.Wallets;

namespace Neo.Common.Utility
{
    /// <summary>
    /// Private key utility methods
    /// </summary>
    public static class PrivateKeyUtility
    {
        public static byte[] TryGetPrivateKey(this string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            try
            {
                return Wallet.GetPrivateKeyFromWIF(key);
            }
            catch (FormatException) { }

            if (key.Length == 64)
            {
                try
                {
                    return key.HexToBytes();
                }
                catch { }
            }
            return null;
        }
    }
}
