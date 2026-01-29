using System;
using System.Collections.Generic;

namespace Neo.Common.Storage
{
    public class TransactionInfo
    {
        public UInt256 TxId { get; set; }
        public UInt160 Sender { get; set; }
        public uint BlockHeight { get; set; }
        public DateTime Time { get; set; }

        public List<TransferInfo> Transfers { get; set; }
    }
}
