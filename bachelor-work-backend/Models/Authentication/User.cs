using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Models.Authentication
{
    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsStagUser { get; set; }
        public int RoleId { get; set; }
        public StagUserInfo activeStagUserInfo { get; set; }
        public List<StagUserInfo> stagUserInfo { get; set; }
    }
}
