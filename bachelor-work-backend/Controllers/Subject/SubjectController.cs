using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.subject;
using bachelor_work_backend.Models;
using bachelor_work_backend.Models.Authentication;
using bachelor_work_backend.Services;
using bachelor_work_backend.Services.Authentication;
using bachelor_work_backend.Services.SubjectFolder;
using bachelor_work_backend.Services.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace bachelor_work_backend.Controllers
{
    [Route("teacher/[controller]")]
    [ApiController]
    [Authorize]
    public class SubjectController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly BachContext context;
        public JwtAuthenticationService AuthenticationService { get; private set; }
        public SubjectService SubjectService { get; private set; }
        public StagApiService StagApiService { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }
        public SubjectController(
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

            SubjectService = new SubjectService(context, mapper, StagApiService);
            AuthenticationService = new JwtAuthenticationService(configuration, StagApiService);
        }


        [HttpPost, Route("create")]
        public async Task<IActionResult> Create(SubjectDTO subjectDTO)
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

            var subject = mapper.Map<SubjectDTO, Subject>(subjectDTO);
            subject.UcitIdno = ucitelIdno;

            SubjectService.Create(subject);

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

            var subject = SubjectService.Get(subjectId);

            if(subject == null)
            {
                return BadRequest();
            }

            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            SubjectService.Delete(subject);

            return Ok();
        }

        [HttpPut, Route("update")]
        public async Task<IActionResult> Update(SubjectDTO subjectDTO)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var subject = SubjectService.Get(subjectDTO.Id);

            if (subject == null)
            {
                return BadRequest();
            }


            var hasPermission = await AuthenticationService.CanDeleteOrUpdateSubject(wscookie, subject);

            if (!hasPermission)
            {
                return Forbid();
            }

            SubjectService.Update(subjectDTO);

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Get()
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

            var subjects = await SubjectService.GetDTOAsync(ucitelIdno, wscookie);

            return Ok(subjects);
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

            var subject = await SubjectService.GetDTOAsync(subjectId, ucitelIdno, wscookie);

            return Ok(subject);
        }

    }
}