using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.Whitelist
{
    public class BlockWhitelistPredmetDTO
    {
        public string Label { get; set; }
        public string ZkrPredm { get; set; }
        public string Year { get; set; }
        public string Department { get; set; }
        public string Term { get; set; }
        public string? predmetNazev { get; set; }
        public List<WhitelistStagStudentDTO> Students { get; set; }
        public List<BlockWhitelistPredmetRozvrhoveAkceDTO> RozvrhoveAkce { get; set; }

        public BlockWhitelistPredmetDTO()
        {
            Students = new List<WhitelistStagStudentDTO>();
            RozvrhoveAkce = new List<BlockWhitelistPredmetRozvrhoveAkceDTO>();
        }

        public BlockWhitelistPredmetDTO(TermStagConnection connection) : this()
        {
            ZkrPredm = connection.ZkrPredm;
            Year = connection.Year;
            Department = connection.Department;
            Term = connection.Term;

            Label = connection.Department + '/' + connection.ZkrPredm + " - " + Year + " " + (Term == string.Empty ? "LS a ZS" : Term);
        }

    }
}
