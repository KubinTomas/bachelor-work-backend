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
    public class SubjectInYearTermController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly BachContext context;
        public JwtAuthenticationService AuthenticationService { get; private set; }
        public SubjectInYearTermService TermService { get; private set; }
        public SubjectService SubjectService { get; private set; }
        public StagApiService StagApiService { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }
        public SubjectInYearTermController(
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
            SubjectService = new SubjectService(context, mapper, StagApiService);
            AuthenticationService = new JwtAuthenticationService(configuration, StagApiService);
        }


        [HttpPost, Route("create")]
        public async Task<IActionResult> Create(SubjectInYearTermDTO termDTO)
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

            if(TermService.DoesTermExists(termDTO.SubjectInYearId, termDTO.Term))
            {
                return BadRequest("term-for-this-year-exists");
            }

            var subject = SubjectService.Get(termDTO.SubjectId);

            if (subject == null)
            {
                return BadRequest("invalidSubjectId");
            }

            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid("invalidPermission");
            }


            var term = mapper.Map<SubjectInYearTermDTO, SubjectInYearTerm>(termDTO);
            term.UcitIdno = ucitelIdno;

            TermService.Create(term);

            return Ok();
        }


        [HttpDelete, Route("delete/{termId}")]
        public async Task<IActionResult> Delete(int termId)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var term = TermService.Get(termId);

            if (term == null)
            {
                return BadRequest();
            }

            var subject = term.SubjectInYear.Subject;

            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            TermService.Delete(term);

            return Ok();
        }

        [HttpGet, Route("{subjectId}")]
        public async Task<IActionResult> Get(int subjectId)
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

            var subjects = await TermService.GetDTOAsync(subjectId, ucitelIdno, wscookie);

            return Ok(subjects);
        }

        [HttpGet, Route("detail/{termId}")]
        public async Task<IActionResult> GetDetail(int termId)
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

            var subject = await TermService.GetSingleDTOAsync(termId, ucitelIdno, wscookie);

            return Ok(subject);
        }

    }
}