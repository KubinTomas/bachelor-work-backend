using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("BlockActionAttendance")]
    public partial class BlockActionAttendance
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
        [Column("fulfilled")]
        public bool Fulfilled { get; set; }
        [Column("dateIn", TypeName = "datetime")]
        public DateTime DateIn { get; set; }
        [Column("evaluationDate", TypeName = "datetime")]
        public DateTime? EvaluationDate { get; set; }

        [ForeignKey(nameof(ActionId))]
        [InverseProperty(nameof(BlockAction.BlockActionAttendances))]
        public virtual BlockAction Action { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("BlockActionAttendances")]
        public virtual User User { get; set; }
    }
}
