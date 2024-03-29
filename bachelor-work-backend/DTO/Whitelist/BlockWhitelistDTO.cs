﻿using bachelor_work_backend.DTO.student;
using bachelor_work_backend.DTO.Whitelist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.subject
{
    public class BlockWhitelistDTO
    {
        public List<BlockWhitelistPredmetDTO> Predmety{ get; set; }
        public List<WhitelistStagStudentDTO> SelectedStudents { get; set; }

        public BlockWhitelistDTO()
        {
            Predmety = new List<BlockWhitelistPredmetDTO>();
            SelectedStudents = new List<WhitelistStagStudentDTO>();
        }
    }
}
