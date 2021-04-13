using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.action;
using bachelor_work_backend.DTO.subject;
using bachelor_work_backend.DTO.Whitelist;
using bachelor_work_backend.Services;
using bachelor_work_backend.Services.Student;
using bachelor_work_backend.Services.SubjectFolder;
using bachelor_work_backend.Services.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using bachelor_work_backend.Filters.Permission;
using bachelor_work_backend.Models.Authentication;
using bachelor_work_backend.DTO.student;
using bachelor_work_backend.Models.Student;

namespace bachelor_work_backend.Controllers.Student
{
    [Route("[controller]")]
    [ApiController]
    [Administrator]
    public class PersonController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly BachContext context;
        public AuthenticationService AuthenticationService { get; private set; }
        public StagApiService StagApiService { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }
        public PersonController(
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

            AuthenticationService = new AuthenticationService(configuration, StagApiService, context, mapper);
        }

        [HttpGet, Route("student/{studentOsCislo}")]
        public async Task<IActionResult> GetStudent(string studentOsCislo)
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var userName = await GetUserStagName();

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var student = await StagApiService.StagStudentApiService.GetStudentInfo(studentOsCislo, wscookie);

            if (student == null)
            {
                return BadRequest("student-not-found");
            }

            var studentDTO = mapper.Map<StagStudent, StudentDTO>(student);

            return Ok(studentDTO);
        }

        private async Task<string> GetUserStagName()
        {
            var claims = User.Claims;
            var userNameClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.UserName);

            var wscookie = Request.Cookies["WSCOOKIE"];

            var stagUser = await AuthenticationService.GetStagUserAsync(wscookie);

            if (stagUser == null)
            {
                return string.Empty;
            }

            var stagUserInfo = stagUser.stagUserInfo.SingleOrDefault(c => c.UserName == userNameClaim.Value);

            if (stagUserInfo != null)
            {
                return stagUserInfo.UserName;
            }

            return string.Empty;
        }
    }
}