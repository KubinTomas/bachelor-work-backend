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

namespace bachelor_work_backend.Controllers.Student
{
    //[TypeFilter(typeof(AdministratorAttribute))]

    [Route("[controller]")]
    [ApiController]
    [Student]
    public class StudentActionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly BachContext context;
        public AuthenticationService AuthenticationService { get; private set; }
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

            AuthenticationService = new AuthenticationService(configuration, StagApiService, context);
            StudentActionService = new StudentActionService(context, mapper, StagApiService);
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetAction(int id)
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

            var actionDto = StudentActionService.GetStudentActionDTO(id, userName);

            if (actionDto == null)
            {
                return BadRequest("action-not-found");
            }

            return Ok(actionDto);
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> GetActions(ActionPostModelDTO filter)
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

            filter.StudentOsCislo = userName;
            filter.IsStudent = true;

            //var filter = new ActionPostModelDTO()
            //{
            //    StudentOsCislo = userName,
            //    AttendanceEnum = ActionAttendanceEnum.All,
            //    HistoryEnum = ActionHistoryEnum.All,
            //    SignEnum = ActionSignInEnum.All,
            //    IsStudent = true
            //};
            var actions = StudentActionService.GetActions(filter);

            return Ok(actions);
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

        [HttpGet, Route("join/{actionId}")]
        public async Task<IActionResult> JoinAction(int actionId)
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

            var action = StudentActionService.GetStudentAction(actionId, userName);

            if (action == null)
            {
                return BadRequest("action-id-for-user-not-permitted");
            }

            if (StudentActionService.IsActionFull(action))
            {
                return BadRequest("action-is-full");
            }

            var res = StudentActionService.StudentJoinAction(action, userName);

            return Ok(res);
        }

        [HttpGet, Route("leave/{actionId}")]
        public async Task<IActionResult> LeaveAction(int actionId)
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

            var action = StudentActionService.GetStudentAction(actionId, userName);

            if (action == null)
            {
                return BadRequest("action-id-for-user-not-permitted");
            }

            var res = StudentActionService.StudentLeaveAction(action, userName);

            return Ok(res);
        }

        [HttpGet, Route("queue/join/{actionId}")]
        public async Task<IActionResult> JoinActionQueue(int actionId)
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

            var action = StudentActionService.GetStudentAction(actionId, userName);

            if (action == null)
            {
                return BadRequest("action-id-for-user-not-permitted");
            }

            if (!StudentActionService.IsActionFull(action))
            {
                return BadRequest("action-is-not-full");
            }

            var res = StudentActionService.StudentJoinActionQueue(action, userName);

            return Ok(res);
        }

        [HttpGet, Route("queue/leave/{actionId}")]
        public async Task<IActionResult> LeaveActionQueue(int actionId)
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

            var action = StudentActionService.GetStudentAction(actionId, userName);

            if (action == null)
            {
                return BadRequest("action-id-for-user-not-permitted");
            }

            var res = StudentActionService.StudentLeaveActionQueue(action, userName);

            return Ok(res);
        }


    }
}