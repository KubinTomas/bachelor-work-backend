using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.action
{
    public class MailDto
    {
        public int ActionId { get; set; }
        public List<int> EntitiesIds { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool AttendanceEntityFlag { get; set; }

    }
}
