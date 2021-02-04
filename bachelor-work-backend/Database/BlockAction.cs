using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("BlockAction")]
    public partial class BlockAction
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("blockId")]
        public int BlockId { get; set; }
        [Required]
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }
        [Column("location")]
        [StringLength(255)]
        public string Location { get; set; }
        [Column("startDate", TypeName = "datetime")]
        public DateTime StartDate { get; set; }
        [Column("endDate", TypeName = "datetime")]
        public DateTime EndDate { get; set; }
        [Column("color")]
        [StringLength(7)]
        public string Color { get; set; }
        [Column("attendanceAllowStartDate", TypeName = "datetime")]
        public DateTime? AttendanceAllowStartDate { get; set; }
        [Column("attendanceAllowEndDate", TypeName = "datetime")]
        public DateTime? AttendanceAllowEndDate { get; set; }
        [Column("attendanceSignOffEndDate", TypeName = "datetime")]
        public DateTime AttendanceSignOffEndDate { get; set; }
        [Column("visible")]
        public bool Visible { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("groupId")]
        public int GroupId { get; set; }
        [Required]
        [Column("ucitIdno")]
        [StringLength(50)]
        public string UcitIdno { get; set; }
        [Column("dateIn", TypeName = "datetime")]
        public DateTime DateIn { get; set; }
        [Column("description")]
        [StringLength(1000)]
        public string Description { get; set; }

        [ForeignKey(nameof(BlockId))]
        [InverseProperty("BlockActions")]
        public virtual Block Block { get; set; }
        [InverseProperty("Action")]
        public virtual BlockActionRestriction BlockActionRestriction { get; set; }
    }
}
