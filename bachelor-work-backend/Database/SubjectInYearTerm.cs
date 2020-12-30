﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("SubjectInYearTerm")]
    public partial class SubjectInYearTerm
    {
        public SubjectInYearTerm()
        {
            Blocks = new HashSet<Block>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("subjectInYearId")]
        public int SubjectInYearId { get; set; }
        [Required]
        [Column("term")]
        [StringLength(50)]
        public string Term { get; set; }
        [Required]
        [Column("ucitIdno")]
        [StringLength(50)]
        public string UcitIdno { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }

        [ForeignKey(nameof(SubjectInYearId))]
        [InverseProperty("SubjectInYearTerms")]
        public virtual SubjectInYear SubjectInYear { get; set; }
        [InverseProperty(nameof(Block.SubjectInYearTerm))]
        public virtual ICollection<Block> Blocks { get; set; }
    }
}
