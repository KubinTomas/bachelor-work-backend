using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.subject;
using bachelor_work_backend.DTO.Whitelist;
using bachelor_work_backend.Filters.Permission;
using bachelor_work_backend.Services;
using bachelor_work_backend.Services.SubjectFolder;
using bachelor_work_backend.Services.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace bachelor_work_backend.Controllers
{
    [Route("teacher/[controller]")]
    [ApiController]
    [Authorize]
    [Administrator]
    public class BlockController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly BachContext context;
        public AuthenticationService AuthenticationService { get; private set; }
        public BlockService BlockService { get; private set; }
        public SubjectInYearTermService TermService { get; private set; }
        public StagApiService StagApiService { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }
        public BlockController(
            IConfiguration configuration,
            IHttpClientFactory clientFactory,
            IMapper mapper,
            BachContext context)
        {
            this.mapper = mapper;
            this.context = context;

            ClientFactory = clientFactory;
            Configuration = configuration;
            StagApiService = new StagApiService(configuration, clientFactory);

            BlockService = new BlockService(context, mapper, StagApiService);
            TermService = new SubjectInYearTermService(context, mapper, StagApiService);
            AuthenticationService = new AuthenticationService(configuration, StagApiService, context);
        }


        [HttpPost, Route("create")]
        public async Task<IActionResult> Create(BlockDTO blockDTO)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var ucitelIdno = await AuthenticationService.GetUcitelIdnoAsync(wscookie);

            if (string.IsNullOrEmpty(ucitelIdno))
            {
                return Unauthorized();
            }

            var term = TermService.Get(blockDTO.TermId);
            var subject = term.SubjectInYear.Subject;

            var hasPermission = await AuthenticationService.CanManageSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            var block = mapper.Map<BlockDTO, Block>(blockDTO);
            block.UcitIdno = ucitelIdno;

            BlockService.Create(block);

            return Ok();
        }


        [HttpDelete, Route("delete/{blockId}")]
        public async Task<IActionResult> Delete(int blockId)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var block = BlockService.Get(blockId);

            if (block == null)
            {
                return BadRequest();
            }

            var subject = block.SubjectInYearTerm.SubjectInYear.Subject;
            var hasPermission = await AuthenticationService.CanManageSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            BlockService.Delete(block);

            return Ok();
        }

        [HttpPut, Route("update")]
        public async Task<IActionResult> Update(BlockDTO blockDTO)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var block = BlockService.Get(blockDTO.Id);

            if (block == null)
            {
                return BadRequest();
            }

            var subject = block.SubjectInYearTerm.SubjectInYear.Subject;
            var hasPermission = await AuthenticationService.CanManageSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            BlockService.Update(block, blockDTO);

            return Ok();
        }


        [HttpGet, Route("{termId}")]
        public async Task<IActionResult> Get(int termId)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var ucitelIdno = await AuthenticationService.GetUcitelIdnoAsync(wscookie);

            if (string.IsNullOrEmpty(ucitelIdno))
            {
                return Unauthorized();
            }

            var term = TermService.Get(termId);

            if (term == null)
            {
                return BadRequest();
            }

            var subject = term.SubjectInYear.Subject;
            var hasPermission = await AuthenticationService.CanManageSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            var blocks = await BlockService.GetDTOAsync(termId, ucitelIdno, wscookie);

            return Ok(blocks);
        }

        [HttpGet, Route("students/{blockId}")]
        public async Task<IActionResult> GetBlockStudents(int blockId)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var ucitelIdno = await AuthenticationService.GetUcitelIdnoAsync(wscookie);

            if (string.IsNullOrEmpty(ucitelIdno))
            {
                return Unauthorized();
            }

            var block = BlockService.Get(blockId);

            if (block == null)
            {
                return BadRequest();
            }

            var subject = block.SubjectInYearTerm.SubjectInYear.Subject;
            var hasPermission = await AuthenticationService.CanManageSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            var studentsDto = await BlockService.GetBlockStudents(blockId, ucitelIdno, wscookie);

            return Ok(studentsDto);
        }

        [HttpGet, Route("detail/{blockId}")]
        public async Task<IActionResult> GetDetail(int blockId)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var ucitelIdno = await AuthenticationService.GetUcitelIdnoAsync(wscookie);

            if (string.IsNullOrEmpty(ucitelIdno))
            {
                return Unauthorized();
            }

            var block = BlockService.Get(blockId);

            if (block == null)
            {
                return BadRequest();
            }

            var subject = block.SubjectInYearTerm.SubjectInYear.Subject;
            var hasPermission = await AuthenticationService.CanManageSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            var blockDto = await BlockService.GetSingleDTOAsync(blockId, ucitelIdno, wscookie);

            return Ok(blockDto);
        }

        [HttpGet, Route("whitelist/{blockId}")]
        public async Task<IActionResult> GetWhitelist(int blockId)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var ucitelIdno = await AuthenticationService.GetUcitelIdnoAsync(wscookie);

            if (string.IsNullOrEmpty(ucitelIdno))
            {
                return Unauthorized();
            }

            var whitelist = await BlockService.GetWhiteListDTO(blockId, ucitelIdno, wscookie);

            if(whitelist == null)
            {
                return BadRequest();
            }

            var block = BlockService.Get(blockId);

            if (block == null)
            {
                return BadRequest();
            }

            var subject = block.SubjectInYearTerm.SubjectInYear.Subject;
            var hasPermission = await AuthenticationService.CanManageSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            return Ok(whitelist);
        }

        [HttpPost, Route("whitelist/save")]
        public async Task<IActionResult> SaveWhitelist(BlockWhitelistSaveDTO whitelistDTO)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var ucitelIdno = await AuthenticationService.GetUcitelIdnoAsync(wscookie);

            if (string.IsNullOrEmpty(ucitelIdno))
            {
                return Unauthorized();
            }

            var block = BlockService.Get(whitelistDTO.blockId);
            var subject = block.SubjectInYearTerm.SubjectInYear.Subject;

            var hasPermission = await AuthenticationService.CanManageSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }
      

            BlockService.SaveWhitelist(whitelistDTO);

            return Ok();
        }

    }
}