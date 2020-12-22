using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SubjectDTO, Subject>();
            CreateMap<Subject, SubjectDTO>()
                .ForMember(c => c.KatedraFakulta, s => s.MapFrom(c => c.Katedra + "/" + c.Fakulta));

            CreateMap<SubjectInYearDTO, SubjectInYear>();
            CreateMap<SubjectInYear, SubjectInYearDTO>();

            CreateMap<Block, BlockDTO>();
            CreateMap<BlockDTO, Block>();
        }
    }
}
