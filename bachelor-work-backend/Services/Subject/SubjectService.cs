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

        public void Create(Subject subject)
        {
            context.Subjects.Add(subject);
            context.SaveChanges();
        }

        public async Task<List<SubjectDTO>> GetDTO(string ucitelIdno, string wscookie)
        {
            var subjectsDTO = new List<SubjectDTO>();

            var subjects = context.Subjects.ToList();

            // v DB JE SPOUSTA WHITESPACE PRI VYTVARENI SUBJECTU PROC..

            foreach (var subject in subjects)
            {
                var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(subject.UcitIdno.Trim(), wscookie);
                var subjectDto = mapper.Map<Subject, SubjectDTO>(subject);

                if(ucitelInfo != null)
                {
                    subjectDto.ucitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
                }

                subjectsDTO.Add(subjectDto);
            }

            return subjectsDTO;
        }
    }
}
