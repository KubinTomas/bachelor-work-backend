using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("Subject")]
    public partial class Subject
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [Column("ucitIdno")]
        [StringLength(50)]
        public string UcitIdno { get; set; }
        [Required]
        [Column("katedra")]
        [StringLength(50)]
        public string Katedra { get; set; }
        [Required]
        [Column("fakulta")]
        [StringLength(50)]
        public string Fakulta { get; set; }
        [Column("description")]
        [StringLength(1000)]
        public string Description { get; set; }
        [Column("dateIn", TypeName = "datetime")]
        public DateTime DateIn { get; set; }
    }
}
