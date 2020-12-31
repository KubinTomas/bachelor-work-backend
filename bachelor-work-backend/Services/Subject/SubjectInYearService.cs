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
            subject.DateIn = DateTime.Now;
            subject.IsActive = true;

            context.SubjectInYears.Add(subject);
            context.SaveChanges();
        }

        public SubjectInYear? Get(int id)
        {
            return context.SubjectInYears.Include(c => c.Subject).SingleOrDefault(c => c.Id == id && c.IsActive);
        }

        public void Delete(int id)
        {
            var subject = Get(id);

            if(subject != null)
            {
                Delete(subject);
            }
        }

        public void Delete(SubjectInYear subject)
        {
            subject.IsActive = false;
            context.SaveChanges();
        }

        public void Update(SubjectInYear subject, SubjectInYearDTO subjectInYearDTO)
        {
            subject.Name = subjectInYearDTO.Name;
            subject.Description = subjectInYearDTO.Description;

            context.SaveChanges();
        }


        public bool DoesYearInSubjectExists(int subjectId, string year)
        {
            return context.SubjectInYears.Any(c => c.SubjectId == subjectId && c.Year.Trim() == year.Trim() && c.IsActive);
        }

        public async Task<List<SubjectInYearDTO>> GetDTOAsync(int subjectId, string ucitelIdno, string wscookie)
        {
            var subjectsDTO = new List<SubjectInYearDTO>();

            var subjects = context.SubjectInYears.Where(c => c.SubjectId == subjectId && c.IsActive).ToList();

            foreach (var subject in subjects)
            {
                var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(subject.UcitIdno.Trim(), wscookie);
                var subjectDto = mapper.Map<SubjectInYear, SubjectInYearDTO>(subject);

                if (ucitelInfo != null)
                {
                    subjectDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
                }

                subjectsDTO.Add(subjectDto);
            }

            subjectsDTO = subjectsDTO.OrderByDescending(c => int.Parse(c.Year.Trim().Split('-')[0])).ToList();

            return subjectsDTO;
        }

        public async Task<SubjectInYearDTO> GetSingleDTOAsync(int subjectInYearId, string ucitelIdno, string wscookie)
        {
            var subjectInYear = context.SubjectInYears.SingleOrDefault(c => c.Id == subjectInYearId && c.IsActive);
            var subjectInYearDto = mapper.Map<SubjectInYear, SubjectInYearDTO>(subjectInYear);

            var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(subjectInYear.UcitIdno.Trim(), wscookie);

            if (ucitelInfo != null)
            {
                subjectInYearDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
            }

            return subjectInYearDto;
        }


    }
}
