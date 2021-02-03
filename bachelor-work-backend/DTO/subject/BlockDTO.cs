using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.subject
{
    public class BlockDTO
    {
        public int Id { get; set; }
        public int SubjectInYearId { get; set; }
        public int TermId { get; set; }
        public string Name { get; set; }
        public DateTime DateIn { get; set; }
        public string? UcitelName { get; set; }
        public int WhitelistUserCount { get; set; }
        public BlockRestrictionDTO BlockRestriction { get; set; }
        public SubjectInYearTermDTO Term { get; set; }
    }
}
