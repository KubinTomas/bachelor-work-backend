﻿using System;
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

            var userName = await GetUserStagName();

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var filter = new ActionPostModelDTO()
            {
                StudentOsCislo = userName,
                AttendanceEnum = ActionAttendanceEnum.All,
                HistoryEnum = ActionHistoryEnum.All,
                SignEnum = ActionSignInEnum.All,
                IsStudent = true
            };
            var actions = StudentActionService.GetActions(filter);

            return Ok(actions);
        }
        private async Task<string> GetUserStagName()
        {
            var claims = User.Claims;
            var userNameClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.UserName);

            var wscookie = Request.Cookies["WSCOOKIE"];

            var stagUser = await AuthenticationService.GetStagUserAsync(wscookie);

            if(stagUser == null)
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

            var action =  StudentActionService.GetStudentAction(actionId, userName);

            if(action == null)
            {
                return BadRequest("action-id-for-user-not-permitted");
            }

            if (StudentActionService.IsActionFull(action))
            {
                return BadRequest("action-is-full");
            }

            StudentActionService.StudentJoinAction(action, userName);

            return Ok();
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

            StudentActionService.StudentLeaveAction(action, userName);

            return Ok();
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

            StudentActionService.StudentJoinActionQueue(action, userName);

            return Ok();
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

            StudentActionService.StudentLeaveActionQueue(action, userName);

            return Ok();
        }


    }
}