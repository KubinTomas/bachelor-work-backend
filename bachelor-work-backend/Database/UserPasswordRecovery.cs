using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("UserPasswordRecovery")]
    public partial class UserPasswordRecovery
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("userId")]
        public int UserId { get; set; }
        [Column("validUntil", TypeName = "datetime")]
        public DateTime ValidUntil { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserPasswordRecoveries")]
        public virtual User User { get; set; }
    }
}
