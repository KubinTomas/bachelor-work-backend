using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.subject;
using bachelor_work_backend.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Services.SubjectFolder
{
    public class TermStagConnectionService
    {
        private readonly BachContext context;
        private readonly IMapper mapper;
        public StagApiService StagApiService { get; private set; }
        public TermStagConnectionService(BachContext context, IMapper mapper, StagApiService StagApiService)
        {
            this.context = context;
            this.mapper = mapper;
            this.StagApiService = StagApiService;
        }

        public void Create(SubjectInYearTerm term)
        {
            term.IsActive = true;
            term.DateIn = DateTime.Now;

            context.SubjectInYearTerms.Add(term);
            context.SaveChanges();
        }

        public SubjectInYearTerm? Get(int id)
        {
            return context.SubjectInYearTerms.Include(c => c.SubjectInYear.Subject).SingleOrDefault(c => c.Id == id && c.IsActive);
        }

        public void Delete(int id)
        {
            var term = Get(id);

            if (term != null)
            {
                Delete(term);
            }
        }

        public void Update(SubjectInYearTermDTO termDTO)
        {
            var term = Get(termDTO.Id);

            if(term != null)
            {
                Update(term, termDTO);
            }

        }


        public void Update(SubjectInYearTerm term, SubjectInYearTermDTO termDTO)
        {
            var isNewTermAvailable = !DoesTermExists(term.SubjectInYearId, termDTO.Term);

            if (isNewTermAvailable)
            {
                term.Term = termDTO.Term;
                context.SaveChanges();
            }

        }


        public void Delete(SubjectInYearTerm term)
        {
            term.IsActive = false;
            context.SaveChanges();
        }

        public bool DoesTermExists(int subjectInYearId, string term)
        {
            return context.SubjectInYearTerms.Any(c => c.SubjectInYearId == subjectInYearId && c.Term.Trim() == term.Trim() && c.IsActive);
        }

        public List<string> GetAvailableTerms(int subjectInYearId)
        {
            return context.SubjectInYearTerms.Where(c => c.SubjectInYearId == subjectInYearId && c.IsActive).Select(c => c.Term).ToList();
        }

        public async Task<List<SubjectInYearTermDTO>> GetDTOAsync(int subjectInYearId, string ucitelIdno, string wscookie)
        {
            var subjectsDTO = new List<SubjectInYearTermDTO>();

            var subjects = context.SubjectInYearTerms.Where(c => c.SubjectInYearId == subjectInYearId && c.IsActive).ToList();

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

        public async Task<SubjectInYearTermDTO?> GetSingleDTOAsync(int termId, string ucitelIdno, string wscookie)
        {
            var term = Get(termId);

            if(term == null)
            {
                return default;
            }

            var termDto = mapper.Map<SubjectInYearTerm, SubjectInYearTermDTO>(term);
            termDto.SubjectId = term.SubjectInYear.SubjectId;

            var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(term.UcitIdno.Trim(), wscookie);

            if (ucitelInfo != null)
            {
                termDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
            }

            return termDto;
        }


    }
}
