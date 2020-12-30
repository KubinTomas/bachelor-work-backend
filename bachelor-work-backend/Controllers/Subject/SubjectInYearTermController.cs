using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.subject;
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
    public class SubjectInYearTermController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly BachContext context;
        public JwtAuthenticationService AuthenticationService { get; private set; }
        public SubjectInYearService SubjectInYearService { get; private set; }
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

            SubjectInYearService = new SubjectInYearService(context, mapper, StagApiService);
            AuthenticationService = new JwtAuthenticationService(configuration, StagApiService);
        }


        [HttpPost, Route("create")]
        public async Task<IActionResult> Create(SubjectInYearDTO subjectInYearDTO)
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

            if(SubjectInYearService.DoesYearInSubjectExists(subjectInYearDTO.SubjectId, subjectInYearDTO.Year))
            {
                return BadRequest("year-for-this-subject-exists");
            }

            var subject = mapper.Map<SubjectInYearDTO, SubjectInYear>(subjectInYearDTO);
            subject.UcitIdno = ucitelIdno;
            subject.DateIn = DateTime.Now;

            SubjectInYearService.Create(subject);

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