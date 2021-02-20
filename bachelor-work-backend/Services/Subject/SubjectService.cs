using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.subject;
using bachelor_work_backend.Models.Authentication;
using bachelor_work_backend.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Services.SubjectFolder
{
    public class SubjectService
    {
        private readonly BachContext context;
        private readonly IMapper mapper;
        public StagApiService StagApiService { get; private set; }
        public SubjectService(BachContext context, IMapper mapper, StagApiService StagApiService)
        {
            this.context = context;
            this.mapper = mapper;
            this.StagApiService = StagApiService;
        }

        public Subject? Get(int subjectId)
        {
            return context.Subjects.SingleOrDefault(c => c.Id == subjectId && c.IsActive);
        }
        public void Delete(int subjectId)
        {
            var subject = Get(subjectId);

            if (subject != null)
            {
                Delete(subject);
            }
        }
        public void Delete(Subject subject)
        {
            subject.IsActive = false;
            context.SaveChanges();
        }

        public void Update(SubjectDTO subjectDTO)
        {
            var subject = Get(subjectDTO.Id);

            Update(subjectDTO, subject);
        }

        public void Update(SubjectDTO subjectDTO, Subject subject)
        {
            subject.Name = subjectDTO.Name;
            subject.Description = subjectDTO.Description;

            context.SaveChanges();
        }

        public void Create(Subject subject)
        {
            subject.DateIn = DateTime.Now;
            subject.IsActive = true;

            context.Subjects.Add(subject);
            context.SaveChanges();
        }


        public async Task<List<SubjectDTO>> GetDTOAsync(string ucitelIdno, string wscookie, StagUserInfo userInfo)
        {
            var subjectsDTO = new List<SubjectDTO>();

            var subjects = context.Subjects.Where(c => c.IsActive && c.Fakulta == userInfo.Fakulta && c.Katedra == userInfo.Katedra).ToList();

            foreach (var subject in subjects)
            {
                var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(subject.UcitIdno.Trim(), wscookie);
                var subjectDto = mapper.Map<Subject, SubjectDTO>(subject);

                if (ucitelInfo != null)
                {
                    subjectDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
                }

                subjectsDTO.Add(subjectDto);
            }

            return subjectsDTO;
        }

        public async Task<SubjectDTO?> GetDTOAsync(int subjectId, string ucitelIdno, string wscookie)
        {
            var subjectDTO = new SubjectDTO();

            var subject = Get(subjectId);

            if(subject == null)
            {
                return default;
            }

            var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(subject.UcitIdno.Trim(), wscookie);

            subjectDTO = mapper.Map<Subject, SubjectDTO>(subject);

            if (ucitelInfo != null)
            {
                subjectDTO.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
            }

            return subjectDTO;
        }
    }

}
