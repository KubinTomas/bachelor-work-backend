using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.subject
{
    public class TermStagConnectionDTO
    {
        public int Id { get; set; }
        public DateTime DateIn { get; set; }
        public string? UcitelName { get; set; }
        public string ZkrPredm { get; set; }
        public string Year { get; set; }
        public string Department { get; set; }
        public string Term { get; set; }
        public int termId { get; set; }
        public string? predmetNazev { get; set; }
        public int pocetStudentu { get; set; }
    }
}
