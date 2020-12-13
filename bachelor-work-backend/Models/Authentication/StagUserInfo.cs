using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Models.Authentication
{
    public class StagUserInfo
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public string RoleNazev { get; set; }
        public string Fakulta { get; set; }
        public string Katedra { get; set; }
        public int? UcitIdno { get; set; }
        public string Aktivni { get; set; }
        public bool HasAnyRozvrharRole { get; set; }
    }
}
