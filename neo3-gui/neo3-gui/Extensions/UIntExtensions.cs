namespace Neo.Extensions
{
    /// <summary>
    /// UInt160/UInt256 extension methods
    /// </summary>
    public static class UIntExtensions
    {
        public static string ToBigEndianHex(this UInt160 address)
        {
            return address.ToArray().ToHexString(reverse: true);
        }

        public static string ToBigEndianHex(this UInt256 txId)
        {
            return txId.ToArray().ToHexString(reverse: true);
        }
    }
}
