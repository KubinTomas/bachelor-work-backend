using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("User")]
    public partial class User
    {
        public User()
        {
            BlockActionAttendances = new HashSet<BlockActionAttendance>();
            BlockActionPeopleEnrollQueues = new HashSet<BlockActionPeopleEnrollQueue>();
            UserPasswordRecoveries = new HashSet<UserPasswordRecovery>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [Column("surname")]
        [StringLength(100)]
        public string Surname { get; set; }
        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        [Column("password")]
        [StringLength(500)]
        public string Password { get; set; }
        [Column("confirmed")]
        public bool Confirmed { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }

        [InverseProperty(nameof(BlockActionAttendance.User))]
        public virtual ICollection<BlockActionAttendance> BlockActionAttendances { get; set; }
        [InverseProperty(nameof(BlockActionPeopleEnrollQueue.User))]
        public virtual ICollection<BlockActionPeopleEnrollQueue> BlockActionPeopleEnrollQueues { get; set; }
        [InverseProperty(nameof(UserPasswordRecovery.User))]
        public virtual ICollection<UserPasswordRecovery> UserPasswordRecoveries { get; set; }
    }
}
