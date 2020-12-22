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
        public string Name { get; set; }
        public string? ucitelName { get; set; }
    }
}
