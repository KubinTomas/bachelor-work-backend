using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("SubjectInYear")]
    public partial class SubjectInYear
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [Column("year")]
        [StringLength(20)]
        public string Year { get; set; }
        [Column("description")]
        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        [Column("ucitIdno")]
        [StringLength(50)]
        public string UcitIdno { get; set; }
        [Column("dateIn", TypeName = "datetime")]
        public DateTime DateIn { get; set; }
        [Column("subjectId")]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        [InverseProperty("SubjectInYears")]
        public virtual Subject Subject { get; set; }
    }
}
