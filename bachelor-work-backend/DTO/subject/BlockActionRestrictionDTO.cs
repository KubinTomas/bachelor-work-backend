using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.subject
{
    public class BlockActionRestrictionDTO
    {
        public int actionId { get; set; }
        public bool allowOnlyStudentsOnWhiteList { get; set; }
        public bool allowExternalUsers { get; set; }
        public int maxCapacity { get; set; }
    }
}
