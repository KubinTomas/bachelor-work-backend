using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.action
{
    public class ActionPostModelDTO
    {
        public string StudentOsCislo { get; set; }
        public string Fakulta { get; set; }
        public string Katedra { get; set; }
        public bool IsStudent { get; set; }
     
        // vse, navstivene, nenavstivene, ceka na vyplneni dochazky
        public ActionAttendanceEnum AttendanceEnum { get; set; }
        public ActionHistoryEnum HistoryEnum { get; set; }
        public ActionSignInEnum SignEnum { get; set; }

    }
}
