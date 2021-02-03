using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.action
{
    public class ActionPostModel
    {
        public string studentOsCislo { get; set; }
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
        // enum
        public bool attendanceDone { get; set; }

    }
}
