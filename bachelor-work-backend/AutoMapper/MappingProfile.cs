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
                .ForMember(c => c.FakultaKatedra, s => s.MapFrom(c => c.Fakulta + "/" + c.Katedra));

            CreateMap<SubjectInYearDTO, SubjectInYear>();
            CreateMap<SubjectInYear, SubjectInYearDTO>();

            CreateMap<Block, BlockDTO>()
                 .ForMember(c => c.TermId, s => s.MapFrom(c => c.SubjectInYearTermId));

            CreateMap<BlockDTO, Block>()
                 .ForMember(c => c.SubjectInYearTermId, s => s.MapFrom(c => c.TermId    ));

            CreateMap<SubjectInYearTermDTO, SubjectInYearTerm>();
            CreateMap<SubjectInYearTerm, SubjectInYearTermDTO>()
                      .ForMember(c => c.SubjectInYearName, s => s.MapFrom(c => c.SubjectInYear.Name))
                      .ForMember(c => c.SubjectInYearYear, s => s.MapFrom(c => c.SubjectInYear.Year));



        }
    }
}
