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
    public class SubjectInYearController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly BachContext context;
        public JwtAuthenticationService AuthenticationService { get; private set; }
        public SubjectInYearService SubjectInYearService { get; private set; }
        public SubjectService SubjectService { get; private set; }
        public StagApiService StagApiService { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }
        public SubjectInYearController(
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

            SubjectInYearService = new SubjectInYearService(context, mapper, StagApiService);
            SubjectService = new SubjectService(context, mapper, StagApiService);
            AuthenticationService = new JwtAuthenticationService(configuration, StagApiService);
        }


        [HttpPost, Route("create")]
        public async Task<IActionResult> Create(SubjectInYearDTO subjectInYearDTO)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized("invalidCredentials");
            }

            var ucitelIdno = await AuthenticationService.GetUcitelIdnoAsync(wscookie);

            if (string.IsNullOrEmpty(ucitelIdno))
            {
                return Unauthorized("invalidCredentials");
            }

            if(SubjectInYearService.DoesYearInSubjectExists(subjectInYearDTO.SubjectId, subjectInYearDTO.Year))
            {
                return BadRequest("yearForThisSubjectExists");
            }

            var subject = SubjectService.Get(subjectInYearDTO.SubjectId);

            if(subject == null)
            {
                return BadRequest("invalidSubjectId");
            }

            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid("invalidPermission");
            }


            var subjectInYear = mapper.Map<SubjectInYearDTO, SubjectInYear>(subjectInYearDTO);
            subjectInYear.UcitIdno = ucitelIdno;
            subjectInYear.DateIn = DateTime.Now;

            SubjectInYearService.Create(subjectInYear);

            return Ok();
        }

        [HttpDelete, Route("delete/{subjectId}")]
        public async Task<IActionResult> Delete(int subjectId)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var subjectInYear = SubjectInYearService.Get(subjectId);

            if (subjectInYear == null)
            {
                return BadRequest();
            }

            var subject = subjectInYear.Subject;

            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            SubjectInYearService.Delete(subjectInYear);

            return Ok();
        }

        [HttpPut, Route("update")]
        public async Task<IActionResult> Update(SubjectInYearDTO subjectInYearDTO)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var subjectInYear = SubjectInYearService.Get(subjectInYearDTO.Id);

            if (subjectInYear == null)
            {
                return BadRequest();
            }
            var subject = subjectInYear.Subject;

            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            SubjectInYearService.Update(subjectInYear, subjectInYearDTO);

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

            var subjects = await SubjectInYearService.GetDTOAsync(subjectId, ucitelIdno, wscookie);

            return Ok(subjects);
        }

        [HttpGet, Route("detail/{subjectInYearId}")]
        public async Task<IActionResult> GetDetail(int subjectInYearId)
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

            var subject = await SubjectInYearService.GetSingleDTOAsync(subjectInYearId, ucitelIdno, wscookie);

            return Ok(subject);
        }

    }
}