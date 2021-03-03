using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("BlockActionPeopleEnrollQueue")]
    public partial class BlockActionPeopleEnrollQueue
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("actionId")]
        public int ActionId { get; set; }
        [Column("studentOsCislo")]
        [StringLength(50)]
        public string StudentOsCislo { get; set; }
        [Column("userId")]
        public int? UserId { get; set; }
        [Column("dateIn", TypeName = "datetime")]
        public DateTime DateIn { get; set; }

        [ForeignKey(nameof(ActionId))]
        [InverseProperty(nameof(BlockAction.BlockActionPeopleEnrollQueues))]
        public virtual BlockAction Action { get; set; }
    }
}
