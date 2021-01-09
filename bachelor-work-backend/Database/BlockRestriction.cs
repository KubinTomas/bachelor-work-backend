using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("BlockRestriction")]
    public partial class BlockRestriction
    {
        [Key]
        [Column("blockId")]
        public int BlockId { get; set; }
        [Column("allowOnlyStudentsOnWhiteList")]
        public bool AllowOnlyStudentsOnWhiteList { get; set; }
        [Column("allowExternalUsers")]
        public bool AllowExternalUsers { get; set; }
        [Column("actionAttendLimit")]
        public int ActionAttendLimit { get; set; }

        [ForeignKey(nameof(BlockId))]
        [InverseProperty("BlockRestriction")]
        public virtual Block Block { get; set; }
    }
}
