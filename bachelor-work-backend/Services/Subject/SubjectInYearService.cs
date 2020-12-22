using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.subject;
using bachelor_work_backend.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Services.SubjectFolder
{
    public class SubjectInYearService
    {
        private readonly BachContext context;
        private readonly IMapper mapper;
        public StagApiService StagApiService { get; private set; }
        public SubjectInYearService(BachContext context, IMapper mapper, StagApiService StagApiService)
        {
            this.context = context;
            this.mapper = mapper;
            this.StagApiService = StagApiService;
        }

        public void Create(SubjectInYear subject)
        {
            context.SubjectInYears.Add(subject);
            context.SaveChanges();
        }

        public bool DoesYearInSubjectExists(int subjectId, string year)
        {
            return context.SubjectInYears.Any(c => c.SubjectId == subjectId && c.Year.Trim() == year.Trim());
        }

        public async Task<List<SubjectInYearDTO>> GetDTOAsync(int subjectId, string ucitelIdno, string wscookie)
        {
            var subjectsDTO = new List<SubjectInYearDTO>();

            var subjects = context.SubjectInYears.Where(c => c.SubjectId == subjectId).ToList();

            foreach (var subject in subjects)
            {
                var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(subject.UcitIdno.Trim(), wscookie);
                var subjectDto = mapper.Map<SubjectInYear, SubjectInYearDTO>(subject);

                if (ucitelInfo != null)
                {
                    subjectDto.ucitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
                }

                subjectsDTO.Add(subjectDto);
            }

            subjectsDTO = subjectsDTO.OrderByDescending(c => int.Parse(c.Year.Trim().Split('-')[0])).ToList();

            return subjectsDTO;
        }

        public async Task<SubjectInYearDTO> GetSingleDTOAsync(int subjectInYearId, string ucitelIdno, string wscookie)
        {
            var subjectInYear = context.SubjectInYears.SingleOrDefault(c => c.Id == subjectInYearId);
            var subjectInYearDto = mapper.Map<SubjectInYear, SubjectInYearDTO>(subjectInYear);

            var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(subjectInYear.UcitIdno.Trim(), wscookie);

            if (ucitelInfo != null)
            {
                subjectInYearDto.ucitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
            }

            return subjectInYearDto;
        }


    }
}
