using bachelor_work_backend.DTO.Rozvrh;
using bachelor_work_backend.DTO.student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.Whitelist
{
    public class BlockWhitelistPredmetRozvrhoveAkceDTO
    {
        public WhitelistRozvrhovaAkceDTO RozvrhovaAkce { get; set; }
        public List<WhitelistStagStudentDTO> Students { get; set; }

        public BlockWhitelistPredmetRozvrhoveAkceDTO()
        {
            Students = new List<WhitelistStagStudentDTO>();
        }
    }
}
