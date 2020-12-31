using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.subject
{
    public class SubjectInYearTermDTO
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int SubjectInYearId { get; set; }
        public string Term { get; set; }
        public string SubjectInYearName { get; set; }
        public string SubjectInYearYear { get; set; }
        public string? UcitelName { get; set; }
        public DateTime DateIn { get; set; }
    }
}
