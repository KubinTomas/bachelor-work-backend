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
    public class ActionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly BachContext context;
        public JwtAuthenticationService AuthenticationService { get; private set; }
        public ActionService ActionService { get; private set; }
        public BlockService BlockService{ get; private set; }
        public SubjectInYearTermService TermService { get; private set; }
        public StagApiService StagApiService { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }
        public ActionController(
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

            ActionService = new ActionService(context, mapper, StagApiService);
            TermService = new SubjectInYearTermService(context, mapper, StagApiService);
            BlockService = new BlockService(context, mapper, StagApiService);
            AuthenticationService = new JwtAuthenticationService(configuration, StagApiService);
        }


        [HttpPost, Route("create")]
        public async Task<IActionResult> Create(BlockActionDTO actionDTO)
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

            var block = BlockService.Get(actionDTO.BlockId);

            if (block == null)
            {
                return BadRequest();
            }

            var subject = block.SubjectInYearTerm.SubjectInYear.Subject;
            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            var action = mapper.Map<BlockActionDTO, BlockAction>(actionDTO);
            action.UcitIdno = ucitelIdno;

            ActionService.Create(action);

            return Ok();
        }


        [HttpDelete, Route("delete/{actionId}")]
        public async Task<IActionResult> Delete(int actionId)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var action = ActionService.Get(actionId);

            if (action == null)
            {
                return BadRequest();
            }

            var block = BlockService.Get(action.Id);

            if (block == null)
            {
                return BadRequest();
            }

            var subject = block.SubjectInYearTerm.SubjectInYear.Subject;
            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            ActionService.Delete(action);

            return Ok();
        }

        [HttpPut, Route("update")]
        public async Task<IActionResult> Update(BlockActionDTO actionDTO)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var action = ActionService.Get(actionDTO.Id);

            var block = BlockService.Get(action.BlockId);

            if (block == null)
            {
                return BadRequest();
            }

            var subject = block.SubjectInYearTerm.SubjectInYear.Subject;
            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            ActionService.Update(action, actionDTO);

            return Ok();
        }


        [HttpGet, Route("{blockId}")]
        public async Task<IActionResult> Get(int blockId)
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

            var actions = await ActionService.GetDTOAsync(blockId, ucitelIdno, wscookie);

            return Ok(actions);
        }

        //[HttpGet, Route("detail/{blockId}")]
        //public async Task<IActionResult> GetDetail(int blockId)
        //{
        //    var wscookie = Request.Cookies["WSCOOKIE"];

        //    if (string.IsNullOrEmpty(wscookie))
        //    {
        //        return Unauthorized();
        //    }

        //    var ucitelIdno = await AuthenticationService.GetUcitelIdnoAsync(wscookie);

        //    if (string.IsNullOrEmpty(ucitelIdno))
        //    {
        //        return Unauthorized();
        //    }

        //    var block = await ActionService.GetSingleDTOAsync(blockId, ucitelIdno, wscookie);

        //    return Ok(block);
        //}

    }
}