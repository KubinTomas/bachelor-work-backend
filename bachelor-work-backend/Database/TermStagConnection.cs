using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("TermStagConnection")]
    public partial class TermStagConnection
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("subjectInYearTermId")]
        public int SubjectInYearTermId { get; set; }
        [Required]
        [Column("zkrPredm")]
        [StringLength(50)]
        public string ZkrPredm { get; set; }
        [Required]
        [Column("year")]
        [StringLength(50)]
        public string Year { get; set; }
        [Required]
        [Column("department")]
        [StringLength(50)]
        public string Department { get; set; }
        [Required]
        [Column("term")]
        [StringLength(50)]
        public string Term { get; set; }
        [Column("dateIn", TypeName = "datetime")]
        public DateTime DateIn { get; set; }
        [Required]
        [Column("ucitIdno")]
        [StringLength(50)]
        public string UcitIdno { get; set; }

        [ForeignKey(nameof(SubjectInYearTermId))]
        [InverseProperty("TermStagConnections")]
        public virtual SubjectInYearTerm SubjectInYearTerm { get; set; }
    }
}
