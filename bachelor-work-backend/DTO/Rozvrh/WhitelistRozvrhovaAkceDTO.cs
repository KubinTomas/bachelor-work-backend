using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.Rozvrh
{
    public class WhitelistRozvrhovaAkceDTO
    {
        public string typAkce { get; set; }
        public string semestr { get; set; }
        public string rok { get; set; }
        public string den { get; set; }
        public string hodinaSkutOd { get; set; }
        public string hodinaSkutDo { get; set; }

    }
}
