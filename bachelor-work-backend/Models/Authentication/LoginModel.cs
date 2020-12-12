using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Models
{
    public class LoginModel
    {
        public string email { get; set; }
        public string password  { get; set; }
        public bool isStagUser { get; set; }
    }
}
