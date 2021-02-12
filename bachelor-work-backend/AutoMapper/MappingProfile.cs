using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.Rozvrh;
using bachelor_work_backend.DTO.student;
using bachelor_work_backend.DTO.subject;
using bachelor_work_backend.Models.Rozvrh;
using bachelor_work_backend.Models.Student;
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
                 .ForMember(c => c.TermId, s => s.MapFrom(c => c.SubjectInYearTermId))
                 .ForMember(c => c.BlockRestriction, s => s.MapFrom(c => c.BlockRestriction))
                 .ForMember(c => c.Term, s => s.MapFrom(c => c.SubjectInYearTerm))
                 .ForMember(c => c.WhitelistUserCount, s => s.MapFrom(c => c.BlockStagUserWhitelists.Count));

            CreateMap<BlockRestriction, BlockRestrictionDTO>();
            CreateMap<BlockRestrictionDTO, BlockRestriction>();

            CreateMap<BlockActionRestriction, BlockActionRestrictionDTO>();
            CreateMap<BlockActionRestrictionDTO, BlockActionRestriction>();

            CreateMap<BlockDTO, Block>()
                 .ForMember(c => c.BlockRestriction, s => s.MapFrom(c => c.BlockRestriction))
                 .ForMember(c => c.SubjectInYearTermId, s => s.MapFrom(c => c.TermId));

            CreateMap<SubjectInYearTermDTO, SubjectInYearTerm>();
            CreateMap<SubjectInYearTerm, SubjectInYearTermDTO>()
                      .ForMember(c => c.SubjectId, s => s.MapFrom(c => c.SubjectInYear != null ? c.SubjectInYear.SubjectId : 0))
                      .ForMember(c => c.SubjectInYearName, s => s.MapFrom(c => c.SubjectInYear.Name))
                      .ForMember(c => c.SubjectInYearYear, s => s.MapFrom(c => c.SubjectInYear.Year));

            CreateMap<TermStagConnectionDTO, TermStagConnection>()
                      .ForMember(c => c.SubjectInYearTermId, s => s.MapFrom(c => c.termId));
            CreateMap<TermStagConnection, TermStagConnectionDTO>();

            CreateMap<StagStudent, WhitelistStagStudentDTO>();

            CreateMap<StagRozvrhoveAkce, WhitelistRozvrhovaAkceDTO>()
                      .ForMember(c => c.katedraPredmet, s => s.MapFrom(c => c.katedra + "/" + c.predmet))
                      .ForMember(c => c.hodinaSkutOd, s => s.MapFrom(c => c.hodinaSkutOd != null ? c.hodinaSkutOd.value : string.Empty))
                      .ForMember(c => c.hodinaSkutDo, s => s.MapFrom(c => c.hodinaSkutDo != null ? c.hodinaSkutDo.value : string.Empty));

            CreateMap<BlockActionDTO, BlockAction>()
                .ForMember(c => c.BlockActionRestriction, s => s.MapFrom(c => c.BlockActionRestriction));
            CreateMap<BlockAction, BlockActionDTO>()
                   .ForMember(c => c.BlockActionRestriction, s => s.MapFrom(c => c.BlockActionRestriction))
                   .ForMember(c => c.UsersInQueueCount, s => s.MapFrom(c => c.BlockActionPeopleEnrollQueues.Count))
                   .ForMember(c => c.SignedUsersCount, s => s.MapFrom(c => c.BlockActionAttendances.Count));


            CreateMap<BlockAction, StudentBlockActionDTO>()
                 .ForMember(c => c.CanSignOfTheAction, s => s.MapFrom(c => c.AttendanceSignOffEndDate >= DateTime.Now))
                 .ForMember(c => c.MaxCapacity, s => s.MapFrom(c => c.BlockActionRestriction.MaxCapacity))
                 .ForMember(c => c.UsersInQueueCount, s => s.MapFrom(c => c.BlockActionPeopleEnrollQueues.Count))
                 .ForMember(c => c.SignedUsersCount, s => s.MapFrom(c => c.BlockActionAttendances.Count));
        }
    }
}
