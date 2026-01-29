using System;
using System.Collections.Generic;

namespace Neo.Models.Transactions
{
    public class TransactionPreviewModel
    {
        public UInt256 TxId { get; set; }
        public uint? BlockHeight { get; set; }
        public DateTime? BlockTime => Timestamp.HasValue ? Timestamp.Value.FromTimestampMS().ToLocalTime() : (DateTime?)null;
        public ulong? Timestamp { get; set; }
        public List<TransferModel> Transfers { get; set; }
    }
}
