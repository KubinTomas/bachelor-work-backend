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

        public void Create(Block block)
        {
            context.Blocks.Add(block);
            context.SaveChanges();
        }


        public async Task<List<BlockDTO>> GetDTOAsync(int subjectInYearId, string ucitelIdno, string wscookie)
        {
            var blocksDTO = new List<BlockDTO>();

            var blocks = context.Blocks.Where(c => c.SubjectInYearId == subjectInYearId).ToList();

            foreach (var block in blocks)
            {
                var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(block.UcitIdno.Trim(), wscookie);
                var blockDto = mapper.Map<Block, BlockDTO>(block);

                if (ucitelInfo != null)
                {
                    blockDto.ucitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
                }

                blocksDTO.Add(blockDto);
            }

            return blocksDTO;
        }

        public async Task<BlockDTO> GetSingleDTOAsync(int blockId, string ucitelIdno, string wscookie)
        {
            var block = context.Blocks.SingleOrDefault(c => c.Id == blockId);
            var blockDto = mapper.Map<Block, BlockDTO>(block);

            var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(block.UcitIdno.Trim(), wscookie);

            if (ucitelInfo != null)
            {
                blockDto.ucitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
            }

            return blockDto;

        }
    }
}
