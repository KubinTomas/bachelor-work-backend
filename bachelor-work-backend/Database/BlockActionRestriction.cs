using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace bachelor_work_backend.Database
{
    [Table("BlockActionRestriction")]
    public partial class BlockActionRestriction
    {
        [Key]
        [Column("actionId")]
        public int ActionId { get; set; }
        [Column("allowOnlyStudentsOnWhiteList")]
        public bool AllowOnlyStudentsOnWhiteList { get; set; }
        [Column("allowExternalUsers")]
        public bool AllowExternalUsers { get; set; }
        [Column("maxCapacity")]
        public int MaxCapacity { get; set; }

        [ForeignKey(nameof(ActionId))]
        [InverseProperty(nameof(BlockAction.BlockActionRestriction))]
        public virtual BlockAction Action { get; set; }
    }
}
