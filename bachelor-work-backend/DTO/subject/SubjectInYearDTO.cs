using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.subject
{
    public class SubjectInYearDTO
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public string? Description { get; set; }
        public string? UcitelName { get; set; }
        public DateTime DateIn { get; set; }
    }
}
