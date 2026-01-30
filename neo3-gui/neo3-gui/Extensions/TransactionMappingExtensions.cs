using Neo.Models.Transactions;
using Neo.Network.P2P.Payloads;

namespace Neo.Extensions
{
    /// <summary>
    /// Transaction mapping extensions
    /// </summary>
    public static class TransactionExtensions
    {
        public static TransactionPreviewModel ToPreview(this Transaction tx)
        {
            return new TransactionPreviewModel
            {
                TxId = tx.Hash
            };
        }
    }
}
