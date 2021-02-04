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

namespace bachelor_work_backend.Controllers.Student
{
    [Route("[controller]")]
    [ApiController]
    public class StudentActionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly BachContext context;
        public JwtAuthenticationService AuthenticationService { get; private set; }
        public StudentActionService StudentActionService { get; private set; }
        public BlockService BlockService { get; private set; }
        public SubjectInYearTermService TermService { get; private set; }
        public StagApiService StagApiService { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }
        public StudentActionController(
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

            AuthenticationService = new JwtAuthenticationService(configuration, StagApiService);
            StudentActionService = new StudentActionService(context, mapper, StagApiService);
        }


        [HttpGet, Route("")]
        public async Task<IActionResult> GetActions()
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var user = await AuthenticationService.GetStagUserAsync(wscookie);

            if (user == null)
            {
                return Unauthorized();
            }

            var filter = new ActionPostModelDTO()
            {
                StudentOsCislo = user.activeStagUserInfo.UserName,
                AttendanceEnum = ActionAttendanceEnum.All,
                HistoryEnum = ActionHistoryEnum.All,
                SignEnum = ActionSignInEnum.All,
                IsStudent = true
            };
            var actions = StudentActionService.GetActions(filter);

            return Ok(actions);
        }
    }
}