using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.Rozvrh;
using bachelor_work_backend.DTO.student;
using bachelor_work_backend.DTO.subject;
using bachelor_work_backend.DTO.Whitelist;
using bachelor_work_backend.Models.Rozvrh;
using bachelor_work_backend.Models.Student;
using bachelor_work_backend.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace bachelor_work_backend.Services.SubjectFolder
{
    public class BlockService
    {
        private readonly BachContext context;
        private readonly IMapper mapper;
        public StagApiService StagApiService { get; private set; }
        public BlockService(BachContext context, IMapper mapper, StagApiService StagApiService)
        {
            this.context = context;
            this.mapper = mapper;
            this.StagApiService = StagApiService;
        }

        public void Update(BlockDTO blockDTO)
        {
            var block = Get(blockDTO.Id);

            if (block != null)
            {
                Update(block, blockDTO);
            }
        }

        public void Update(Block block, BlockDTO blockDTO)
        {
            block.Name = blockDTO.Name;

            block.BlockRestriction.ActionAttendLimit = blockDTO.BlockRestriction.actionAttendLimit;
            block.BlockRestriction.AllowExternalUsers = blockDTO.BlockRestriction.allowExternalUsers;
            block.BlockRestriction.AllowOnlyStudentsOnWhiteList = blockDTO.BlockRestriction.allowOnlyStudentsOnWhiteList;

            context.SaveChanges();
        }

        public Block? Get(int blockId)
        {
            return context.Blocks.Include(c => c.SubjectInYearTerm.SubjectInYear.Subject).Include(c => c.BlockRestriction).SingleOrDefault(c => c.Id == blockId && c.IsActive);
        }

        public SubjectInYearTerm? GetTerm(int blockId)
        {
            var block = Get(blockId);

            if (block == null)
            {
                return default;
            }

            return block.SubjectInYearTerm;
        }


        public void Delete(int blockId)
        {
            var block = context.Blocks.SingleOrDefault(c => c.Id == blockId && c.IsActive);

            if (block != null)
            {
                Delete(block);
            }
        }

        public void Delete(Block block)
        {
            block.IsActive = false;
            context.SaveChanges();
        }



        public void Create(Block block)
        {
            block.IsActive = true;
            block.DateIn = DateTime.Now;

            context.Blocks.Add(block);
            context.SaveChanges();
        }


        public async Task<List<BlockDTO>> GetDTOAsync(int termId, string ucitelIdno, string wscookie)
        {
            var blocksDTO = new List<BlockDTO>();

            var blocks = context.Blocks.Include(c => c.BlockStagUserWhitelists).Include(c => c.BlockRestriction).Where(c => c.SubjectInYearTermId == termId && c.IsActive).ToList();

            foreach (var block in blocks)
            {
                var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(block.UcitIdno.Trim(), wscookie);
                var blockDto = mapper.Map<Block, BlockDTO>(block);

                if (ucitelInfo != null)
                {
                    blockDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
                }

                blocksDTO.Add(blockDto);
            }

            return blocksDTO;
        }

        public async Task<BlockDTO> GetSingleDTOAsync(int blockId, string ucitelIdno, string wscookie)
        {
            var block = Get(blockId);

            if (block == null)
            {
                return default;
            }

            var blockDto = mapper.Map<Block, BlockDTO>(block);

            var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(block.UcitIdno.Trim(), wscookie);

            if (ucitelInfo != null)
            {
                blockDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
            }

            return blockDto;

        }

        public List<TermStagConnection>? GetTermStagConnectionByBock(int blockId)
        {
            var block = Get(blockId);

            if (block == null)
            {
                return default;
            }

            return context.TermStagConnections.Where(c => c.SubjectInYearTermId == block.SubjectInYearTermId).ToList();
        }

        public async Task<List<WhitelistStagStudentDTO>> GetWhitelistStudents(int blockId, string wscookie)
        {
            var whitelist = GetWhitelist(blockId);

            var students = new List <WhitelistStagStudentDTO>();

            foreach (var whitelistEntry in whitelist)
            {
                var student = await StagApiService.StagStudentApiService.GetStudentInfo(whitelistEntry.StudentOsCislo, wscookie);

                if(student != null)
                {
                    students.Add(mapper.Map<StagStudent, WhitelistStagStudentDTO>(student));
                }
            }

            return students;
        }

        public async Task<BlockWhitelistDTO?> GetWhiteListDTO(int blockId, string ucitelIdno, string wscookie)
        {
            var whitelist = new BlockWhitelistDTO();
            whitelist.SelectedStudents = await GetWhitelistStudents(blockId, wscookie);

            var stagConnections = GetTermStagConnectionByBock(blockId);

            if (stagConnections == null)
            {
                return default;
            }

            foreach (var connection in stagConnections)
            {
                var predmet = new BlockWhitelistPredmetDTO(connection);
                whitelist.Predmety.Add(predmet);

                var rozvrhoveAkce = await StagApiService.StagRozvrhyApiService.GetRozvrhoveAkce(connection.Year, connection.Term, connection.ZkrPredm, wscookie);
                var predmetStudenti = (await StagApiService.StagPredmetyApiService.GetStudentiByPredmet(connection.Department, connection.Year, connection.Term, connection.ZkrPredm, wscookie));

                predmet.Students = predmetStudenti.Select(c => mapper.Map<StagStudent, WhitelistStagStudentDTO>(c)).ToList();

                if (rozvrhoveAkce != null)
                {
                    var rozvrhoveAkceByKatedra = rozvrhoveAkce.Where(c => c.katedra == connection.Department);

                    foreach (var akce in rozvrhoveAkceByKatedra)
                    {
                        var akceStudenti = await StagApiService.StagStudentApiService.GetStudentiByRoakce(akce.roakIdno, wscookie);

                        if (akceStudenti != null)
                        {
                            var rozvrhovaAkce = new BlockWhitelistPredmetRozvrhoveAkceDTO();
                            rozvrhovaAkce.RozvrhovaAkce = mapper.Map<StagRozvrhoveAkce, WhitelistRozvrhovaAkceDTO>(akce);
                            rozvrhovaAkce.Students = akceStudenti.Select(c => mapper.Map<StagStudent, WhitelistStagStudentDTO>(c)).ToList();

                            predmet.RozvrhoveAkce.Add(rozvrhovaAkce);
                        }
                    }
                }

            }

            return whitelist;
        }

        public List<BlockStagUserWhitelist> GetWhitelist(int blockId)
        {
            return context.BlockStagUserWhitelists.Where(c => c.BlockId == blockId).ToList();
        }

        public void SaveWhitelist(BlockWhitelistSaveDTO whitelistDTO)
        {
            DeleteWhitelist(whitelistDTO.blockId);

            var whitelistData = whitelistDTO.studentsOsCislo.Select(c => new BlockStagUserWhitelist() { StudentOsCislo = c, BlockId = whitelistDTO.blockId }).ToList();

            context.BlockStagUserWhitelists.AddRange(whitelistData);
            context.SaveChanges();
        }

        public void DeleteWhitelist(int blockId)
        {
            var whitelist = GetWhitelist(blockId);

            context.BlockStagUserWhitelists.RemoveRange(whitelist);
            context.SaveChanges();
        }
    }
}
