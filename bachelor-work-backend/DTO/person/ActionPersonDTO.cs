using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.person
{
    public class ActionPersonDTO
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public bool IsStagStudent { get; set; }
        public DateTime? EvaluationDate { get; set; }
        public DateTime DateIn { get; set; }
        public bool Fulfilled { get; set; }
        public string rocnik { get; set; }
        public string fakultaSp { get; set; }
        public int queueOrder { get; set; }
    }
}
