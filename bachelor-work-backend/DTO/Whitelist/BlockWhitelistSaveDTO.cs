using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.Whitelist
{
    public class BlockWhitelistSaveDTO
    {
        public int blockId { get; set; }
        public List<string> studentsOsCislo { get; set; }
    }
}
