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
using bachelor_work_backend.Models.Validation;

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
            var validateResult = await Validate();

            if (!validateResult.IsValid)
            {
                return validateResult.ActionResult;
            }

            var actionDto = validateResult.IsStudent ? StudentActionService.GetStudentActionDTO(id, validateResult.StudentOsCislo) :
                 StudentActionService.GetUserActionDTO(id, validateResult.UserId);
         
            if (actionDto == null)
            {
                return BadRequest("action-not-found");
            }

            return Ok(actionDto);
        }

        private async Task<ValidationModel> Validate()
        {
            var validation = new ValidationModel();

            var user = User;
            var claims = user.Claims.ToList();

            var stagTokenClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.StagToken);
            var userIdClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.UserId);

            var wscookie = Request.Cookies["WSCOOKIE"];

            if (stagTokenClaim.Value == "true")
            {
                if (string.IsNullOrEmpty(wscookie))
                {
                    return new ValidationModel(Unauthorized());
                }

                var userName = await GetUserStagName();

                if (string.IsNullOrEmpty(userName))
                {
                    return new ValidationModel(Unauthorized());
                }

                validation.StudentOsCislo = userName;
            }
            else
            {
                if (string.IsNullOrEmpty(userIdClaim.Value))
                {
                    return new ValidationModel(Unauthorized());
                }

                validation.UserId = int.Parse(userIdClaim.Value);
            }

            validation.IsStudent = stagTokenClaim.Value == "true";
            validation.IsValid = true;

            return validation;
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> GetActions(ActionPostModelDTO filter)
        {
            var validateResult = await Validate();

            if (!validateResult.IsValid)
            {
                return validateResult.ActionResult;
            }

            filter.IsStudent = validateResult.IsStudent;
            filter.StudentOsCislo = validateResult.StudentOsCislo;
            filter.UserId = validateResult.UserId;

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
            var validateResult = await Validate();

            if (!validateResult.IsValid)
            {
                return validateResult.ActionResult;
            }


            var action = validateResult.IsStudent ? StudentActionService.GetStudentAction(actionId, validateResult.StudentOsCislo) :
                StudentActionService.GetStudentAction(actionId);

            if (action == null)
            {
                return BadRequest("action-id-for-user-not-permitted");
            }

            if (action.isDeleted)
            {
                return BadRequest("action-is-deleted");
            }

            if (StudentActionService.IsActionFull(action))
            {
                return BadRequest("action-is-full");
            }

            var res = validateResult.IsStudent ? StudentActionService.StudentJoinAction(action, validateResult.StudentOsCislo) :
                StudentActionService.StudentJoinAction(action, validateResult.UserId);

            return Ok(res);
        }

        [HttpGet, Route("leave/{actionId}")]
        public async Task<IActionResult> LeaveAction(int actionId)
        {
            var validateResult = await Validate();

            if (!validateResult.IsValid)
            {
                return validateResult.ActionResult;
            }

            var action = validateResult.IsStudent ? StudentActionService.GetStudentAction(actionId, validateResult.StudentOsCislo) :
                 StudentActionService.GetStudentAction(actionId);

            if (action == null)
            {
                return BadRequest("action-id-for-user-not-permitted");
            }

            var res = validateResult.IsStudent ? StudentActionService.StudentLeaveAction(action, validateResult.StudentOsCislo) :
                 StudentActionService.StudentLeaveAction(action, validateResult.UserId);

            return Ok(res);
        }

        [HttpGet, Route("queue/join/{actionId}")]
        public async Task<IActionResult> JoinActionQueue(int actionId)
        {
            var validateResult = await Validate();

            if (!validateResult.IsValid)
            {
                return validateResult.ActionResult;
            }

            var action = validateResult.IsStudent ? StudentActionService.GetStudentAction(actionId, validateResult.StudentOsCislo) :
            StudentActionService.GetStudentAction(actionId);

            if (action == null)
            {
                return BadRequest("action-id-for-user-not-permitted");
            }

            if (action.isDeleted)
            {
                return BadRequest("action-is-deleted");
            }

            if (!StudentActionService.IsActionFull(action))
            {
                return BadRequest("action-is-not-full");
            }

            var res = validateResult.IsStudent ? StudentActionService.StudentJoinActionQueue(action, validateResult.StudentOsCislo) :
            StudentActionService.StudentJoinActionQueue(action, validateResult.UserId);

            return Ok(res);
        }

        [HttpGet, Route("queue/leave/{actionId}")]
        public async Task<IActionResult> LeaveActionQueue(int actionId)
        {
            var validateResult = await Validate();

            if (!validateResult.IsValid)
            {
                return validateResult.ActionResult;
            }

            var action = validateResult.IsStudent ? StudentActionService.GetStudentAction(actionId, validateResult.StudentOsCislo) :
            StudentActionService.GetStudentAction(actionId);

            if (action == null)
            {
                return BadRequest("action-id-for-user-not-permitted");
            }
          
            var res = validateResult.IsStudent ? StudentActionService.StudentLeaveActionQueue(action, validateResult.StudentOsCislo) :
                StudentActionService.StudentLeaveActionQueue(action, validateResult.UserId);

            return Ok(res);
        }


    }
}