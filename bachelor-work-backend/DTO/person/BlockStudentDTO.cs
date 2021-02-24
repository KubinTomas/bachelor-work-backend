using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.person
{
    public class BlockStudentDTO
    {
        public string StudentOsCislo { get; set; }
        public string Name { get; set; }
        public string Rocnik { get; set; }
        public string FormaSp { get; set; }
        public string FakultaSp { get; set; }
        public int AttendanceFulfillCount { get; set; }
        public int BlockAttendanceCount { get; set; }
        
    }
}
