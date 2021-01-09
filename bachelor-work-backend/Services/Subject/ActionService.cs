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
    public class ActionService
    {
        private readonly BachContext context;
        private readonly IMapper mapper;
        public StagApiService StagApiService { get; private set; }
        public ActionService(BachContext context, IMapper mapper, StagApiService StagApiService)
        {
            this.context = context;
            this.mapper = mapper;
            this.StagApiService = StagApiService;
        }

        public void Update(BlockActionDTO actionDTO)
        {
            var action = Get(actionDTO.Id);

            if (action != null)
            {
                Update(action, actionDTO);
            }
        }

        public void Update(BlockAction action, BlockActionDTO actionDTO)
        {
            action.Name = actionDTO.Name;

            context.SaveChanges();
        }

        public BlockAction? Get(int actionId)
        {
            return context.BlockActions.SingleOrDefault(c => c.Id == actionId && c.IsActive);
        }


        public void Delete(int actionId)
        {
            var action = Get(actionId);

            if (action != null)
            {
                Delete(action);
            }
        }

        public void Delete(BlockAction action)
        {
            action.IsActive = false;
            context.SaveChanges();
        }

        public void Create(BlockAction action)
        {
            action.IsActive = true;
            action.DateIn = DateTime.Now;

            context.BlockActions.Add(action);
            context.SaveChanges();
        }


        //public async Task<List<BlockDTO>> GetDTOAsync(int termId, string ucitelIdno, string wscookie)
        //{
        //    var blocksDTO = new List<BlockDTO>();

        //    var blocks = context.Blocks.Include(c => c.BlockStagUserWhitelists).Include(c => c.BlockRestriction).Where(c => c.SubjectInYearTermId == termId && c.IsActive).ToList();

        //    foreach (var block in blocks)
        //    {
        //        var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(block.UcitIdno.Trim(), wscookie);
        //        var blockDto = mapper.Map<Block, BlockDTO>(block);

        //        if (ucitelInfo != null)
        //        {
        //            blockDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
        //        }

        //        blocksDTO.Add(blockDto);
        //    }

        //    return blocksDTO;
        //}

        //public async Task<BlockDTO> GetSingleDTOAsync(int blockId, string ucitelIdno, string wscookie)
        //{
        //    var block = Get(blockId);

        //    if (block == null)
        //    {
        //        return default;
        //    }

        //    var blockDto = mapper.Map<Block, BlockDTO>(block);

        //    var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(block.UcitIdno.Trim(), wscookie);

        //    if (ucitelInfo != null)
        //    {
        //        blockDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
        //    }

        //    return blockDto;

        //}
    }
}
