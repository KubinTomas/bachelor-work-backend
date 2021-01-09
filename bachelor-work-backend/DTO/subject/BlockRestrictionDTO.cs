using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.subject
{
    public class BlockRestrictionDTO
    {
        public int id { get; set; }
        public int blockId { get; set; }
        public bool allowOnlyStudentsOnWhiteList { get; set; }
        public bool allowExternalUsers { get; set; }
        public int actionAttendLimit { get; set; }
    }
}
