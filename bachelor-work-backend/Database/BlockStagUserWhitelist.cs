using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("BlockStagUserWhitelist")]
    public partial class BlockStagUserWhitelist
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("blockId")]
        public int BlockId { get; set; }
        [Required]
        [Column("studentOsCislo")]
        [StringLength(50)]
        public string StudentOsCislo { get; set; }

        [ForeignKey(nameof(BlockId))]
        [InverseProperty("BlockStagUserWhitelists")]
        public virtual Block Block { get; set; }
    }
}
