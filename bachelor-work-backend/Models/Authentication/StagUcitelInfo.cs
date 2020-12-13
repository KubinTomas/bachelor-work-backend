using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Models.Authentication
{
    public class StagUcitelInfo
    {
        public int UcitIdno { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string TitulPred { get; set; }
        public string TitulZa { get; set; }
        public string Zamestnanec { get; set; }
        public string Katedra { get; set; }
        public string PracovisteDalsi { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Telefon2 { get; set; }
    }
}
