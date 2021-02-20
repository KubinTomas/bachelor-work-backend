using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.student
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string StudentOsCislo { get; set; }
        public string rocnik { get; set; }
        public string fakultaSp { get; set; }
        public int blockOrActionId { get; set; }
        public string formaSp { get; set; }
    }
}
