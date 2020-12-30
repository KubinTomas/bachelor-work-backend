﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("Block")]
    public partial class Block
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("subjectInYearTermId")]
        public int SubjectInYearTermId { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [Column("ucitIdno")]
        [StringLength(50)]
        public string UcitIdno { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }

        [ForeignKey(nameof(SubjectInYearTermId))]
        [InverseProperty("Blocks")]
        public virtual SubjectInYearTerm SubjectInYearTerm { get; set; }
    }
}
