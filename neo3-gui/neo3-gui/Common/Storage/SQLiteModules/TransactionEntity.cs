using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neo.Common.Storage.SQLiteModules
{
    [Table("Transaction")]

    public class TransactionEntity
    {
        [Key]
        public string TxId { get; set; }
        public uint BlockHeight { get; set; }

        public long? SenderId { get; set; }

        public AddressEntity Sender { get; set; }

        public DateTime Time { get; set; }
        public IList<TransferEntity> Transfers { get; set; }
    }
}
