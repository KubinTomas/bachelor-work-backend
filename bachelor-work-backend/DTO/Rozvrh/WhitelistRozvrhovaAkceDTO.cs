using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.Rozvrh
{
    public class WhitelistRozvrhovaAkceDTO
    {
        public string katedraPredmet { get; set; }
        public string katedra { get; set; }
        public string predmet { get; set; }
        public string roakIdno { get; set; }
        public string typAkce { get; set; }
        public string semestr { get; set; }
        public string rok { get; set; }
        public string den { get; set; }
        public string hodinaSkutOd { get; set; }
        public string hodinaSkutDo { get; set; }

    }
}
