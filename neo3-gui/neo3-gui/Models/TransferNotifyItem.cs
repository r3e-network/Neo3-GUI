using System.Numerics;

namespace Neo.Models
{
    public class TransferNotifyItem
    {
        public UInt160 Asset { get; set; }
        public UInt160 From { get; set; }
        public UInt160 To { get; set; }
        public BigInteger Amount { get; set; }

        /// <summary>
        /// Nep11 Token
        /// </summary>
        public byte[] TokenId { get; set; }
        public string Symbol { get; set; }
        public byte Decimals { get; set; }
    }
}
