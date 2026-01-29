using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neo.Common.Storage.SQLiteModules
{
    [Table("Identity")]
    public class IdentityEntity
    {
        [Key]
        public long Id { get; set; }

        public byte[] Data { get; set; }
    }
}
