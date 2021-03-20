using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.action
{
    public class SendMailDto
    {
        public List<string> Emails { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public SendMailDto(List<string> emails, string subjct, string content)
        {
            Emails = emails;
            Subject = subjct;
            Content = content;
        }
    }
}
