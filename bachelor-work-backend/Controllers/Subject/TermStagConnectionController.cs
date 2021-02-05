using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.subject;
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
    public class TermStagConnectionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly BachContext context;
        public JwtAuthenticationService AuthenticationService { get; private set; }
        public SubjectInYearTermService TermService { get; private set; }
        public TermStagConnectionService TermStagConnectionService { get; private set; }
        public SubjectService SubjectService { get; private set; }
        public StagApiService StagApiService { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }
        public TermStagConnectionController(
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

            TermService = new SubjectInYearTermService(context, mapper, StagApiService);
            TermStagConnectionService = new TermStagConnectionService(context, mapper, StagApiService);
            SubjectService = new SubjectService(context, mapper, StagApiService);
            AuthenticationService = new JwtAuthenticationService(configuration, StagApiService);
        }

        [HttpGet, Route("subjects")]
        public async Task<IActionResult> GetDetail([FromQuery]string department, [FromQuery]string year, [FromQuery]string? term)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(term))
            {
                term = string.Empty;
            }

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var ucitelIdno = await AuthenticationService.GetUcitelIdnoAsync(wscookie);

            if (string.IsNullOrEmpty(ucitelIdno))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(department) || string.IsNullOrEmpty(year))
            {
                return BadRequest();
            }

            var stagSubjects = await StagApiService.StagPredmetyApiService.GetPredmetyByKatedra(department, year, term, wscookie);

            return Ok(stagSubjects);
        }


        [HttpPost, Route("create")]
        public async Task<IActionResult> Create(TermStagConnectionDTO connectionDTO)
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

            if (TermStagConnectionService.DoesConnectionExists(connectionDTO))
            {
                return BadRequest("connection-exists");
            }

            var term = TermService.Get(connectionDTO.termId);

            if(term == null)
            {
                return BadRequest("invalid-term");
            }

            var subject = term.SubjectInYear.Subject;

            if (subject == null)
            {
                return BadRequest("invalidSubjectId");
            }

            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid("invalidPermission");
            }


            var connection = mapper.Map<TermStagConnectionDTO, TermStagConnection>(connectionDTO);
            connection.UcitIdno = ucitelIdno;

            TermStagConnectionService.Create(connection);

            return Ok();
        }


        [HttpDelete, Route("delete/{connectionId}")]
        public async Task<IActionResult> Delete(int connectionId)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var connection = TermStagConnectionService.Get(connectionId);

            if (connection == null)
            {
                return BadRequest();
            }

            var term = TermService.Get(connection.SubjectInYearTermId);

            var subject = term.SubjectInYear.Subject;

            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            TermStagConnectionService.Delete(connection);

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

            var connections = await TermStagConnectionService.GetDTOAsync(termId, ucitelIdno, wscookie);

            return Ok(connections);
        }


    }
}