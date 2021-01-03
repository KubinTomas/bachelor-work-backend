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

        public void Create(TermStagConnection connection)
        {
            connection.DateIn = DateTime.Now;

            context.TermStagConnections.Add(connection);
            context.SaveChanges();
        }


        public bool DoesConnectionExists(TermStagConnectionDTO connectionDTO)
        {
            return context.TermStagConnections.Any(c => c.SubjectInYearTermId == connectionDTO.termId &&
                                                        c.Department == connectionDTO.Department &&
                                                        c.Term == connectionDTO.Term &&
                                                        c.ZkrPredm == connectionDTO.ZkrPredm);
        }

        public TermStagConnection? Get(int id)
        {
            return context.TermStagConnections.SingleOrDefault(c => c.Id == id);
        }

        public void Delete(int id)
        {
            var connection = Get(id);

            if (connection != null)
            {
                Delete(connection);
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


        public void Delete(TermStagConnection connection)
        {
            context.TermStagConnections.Remove(connection);
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

        public async Task<List<TermStagConnectionDTO>> GetDTOAsync(int termId, string ucitelIdno, string wscookie)
        {
            var connectionsDTO = new List<TermStagConnectionDTO>();

            var connections = context.TermStagConnections.Where(c => c.SubjectInYearTermId == termId).ToList();

            foreach (var connection in connections)
            {
                var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(connection.UcitIdno.Trim(), wscookie);
                var connectionDTO = mapper.Map<TermStagConnection, TermStagConnectionDTO>(connection);

                if (ucitelInfo != null)
                {
                    connectionDTO.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
                }

                var stagPredmetInfo = await StagApiService.StagPredmetyApiService.GetPredmetInfo(connection.Department, connection.Year, connection.Term, connection.ZkrPredm, wscookie);
                var stagPredmetStudents = await StagApiService.StagPredmetyApiService.GetStudentiByPredmet(connection.Department, connection.Year, connection.Term, connection.ZkrPredm, wscookie);

                if (stagPredmetStudents != null)
                {
                    connectionDTO.pocetStudentu = stagPredmetStudents.Count;
                }

                if (stagPredmetInfo != null)
                {
                    connectionDTO.predmetNazev = stagPredmetInfo.nazev;
                }

                connectionsDTO.Add(connectionDTO);
            }

            connectionsDTO = connectionsDTO.OrderByDescending(c => c.Term).ToList();

            return connectionsDTO;
        }

    }
}
