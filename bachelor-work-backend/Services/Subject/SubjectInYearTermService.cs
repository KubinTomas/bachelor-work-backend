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
    public class SubjectInYearTermService
    {
        private readonly BachContext context;
        private readonly IMapper mapper;
        public StagApiService StagApiService { get; private set; }
        public SubjectInYearTermService(BachContext context, IMapper mapper, StagApiService StagApiService)
        {
            this.context = context;
            this.mapper = mapper;
            this.StagApiService = StagApiService;
        }

        public void Create(SubjectInYearTerm subject)
        {
            context.SubjectInYearTerms.Add(subject);
            context.SaveChanges();
        }

        public bool DoesTermExists(int subjectInYearId, string term)
        {
            return context.SubjectInYearTerms.Any(c => c.SubjectInYearId == subjectInYearId && c.Term.Trim() == term.Trim());
        }

        public List<string> GetAvailableTerms(int subjectInYearId)
        {
            return context.SubjectInYearTerms.Where(c => c.SubjectInYearId == subjectInYearId).Select(c => c.Term).ToList();
        }

        public async Task<List<SubjectInYearTermDTO>> GetDTOAsync(int subjectInYearId, string ucitelIdno, string wscookie)
        {
            var subjectsDTO = new List<SubjectInYearTermDTO>();

            var subjects = context.SubjectInYearTerms.Where(c => c.SubjectInYearId == subjectInYearId).ToList();

            foreach (var subject in subjects)
            {
                var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(subject.UcitIdno.Trim(), wscookie);
                var subjectDto = mapper.Map<SubjectInYearTerm, SubjectInYearTermDTO>(subject);

                if (ucitelInfo != null)
                {
                    subjectDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
                }

                subjectsDTO.Add(subjectDto);
            }

            subjectsDTO = subjectsDTO.OrderByDescending(c => c.Term).ToList();

            return subjectsDTO;
        }

        public async Task<SubjectInYearTermDTO> GetSingleDTOAsync(int subjectInYearId, string ucitelIdno, string wscookie)
        {
            var subjectInYear = context.SubjectInYears.SingleOrDefault(c => c.Id == subjectInYearId);
            var subjectInYearDto = mapper.Map<SubjectInYear, SubjectInYearTermDTO>(subjectInYear);

            var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(subjectInYear.UcitIdno.Trim(), wscookie);

            if (ucitelInfo != null)
            {
                subjectInYearDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
            }

            return subjectInYearDto;
        }


    }
}
