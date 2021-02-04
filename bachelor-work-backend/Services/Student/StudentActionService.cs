using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.action;
using bachelor_work_backend.DTO.Rozvrh;
using bachelor_work_backend.DTO.student;
using bachelor_work_backend.DTO.subject;
using bachelor_work_backend.DTO.Whitelist;
using bachelor_work_backend.Models.Rozvrh;
using bachelor_work_backend.Models.Student;
using bachelor_work_backend.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace bachelor_work_backend.Services.Student
{
    public class StudentActionService
    {
        private readonly BachContext context;
        private readonly IMapper mapper;
        public StagApiService StagApiService { get; private set; }
        public StudentActionService(BachContext context, IMapper mapper, StagApiService StagApiService)
        {
            this.context = context;
            this.mapper = mapper;
            this.StagApiService = StagApiService;
        }

        public List<BlockActionDTO> GetActions(ActionPostModelDTO filter)
        {
            IQueryable<BlockAction> actions;

            if (filter.IsStudent)
            {
                actions = GetActionsForStudent(filter);
            }
            else
            {
                actions = GetActionsForExternalUser(filter);
            }

            actions = FilterActionsByAttendance(actions, filter);
            actions = FilterActionsBySignIn(actions, filter);
            actions = FilterActionsByHistory(actions, filter);

            return actions.ToList().Select(c => mapper.Map<BlockAction, BlockActionDTO>(c)).ToList();
        }

        private IQueryable<BlockAction> FilterActionsByHistory(IQueryable<BlockAction> actions, ActionPostModelDTO filter)
        {
            if (filter.HistoryEnum == ActionHistoryEnum.Future)
            {
                actions = actions.Where(c => c.EndDate >= DateTime.Now.Date);
            }

            if (filter.HistoryEnum == ActionHistoryEnum.Past)
            {
                actions = actions.Where(c => c.EndDate < DateTime.Now.Date);
            }

            return actions;
        }
        // pak dodelat ify i pro externiho uzivatele
        private IQueryable<BlockAction> FilterActionsBySignIn(IQueryable<BlockAction> actions, ActionPostModelDTO filter)
        {
            if (filter.SignEnum == ActionSignInEnum.CanSignIn)
            {
                actions = actions.Where(c => !c.BlockActionAttendances.Any(c => c.StudentOsCislo == filter.StudentOsCislo) && !c.BlockActionPeopleEnrollQueues.Any(c => c.StudentOsCislo == filter.StudentOsCislo));
            }

            if (filter.SignEnum == ActionSignInEnum.SignedIn)
            {
                actions = actions.Where(c => c.BlockActionAttendances.Any(c => c.StudentOsCislo == filter.StudentOsCislo));
            }

            if (filter.SignEnum == ActionSignInEnum.SignedInQueue)
            {
                actions = actions.Where(c => c.BlockActionPeopleEnrollQueues.Any(c => c.StudentOsCislo == filter.StudentOsCislo));
            }

            if (filter.SignEnum == ActionSignInEnum.SignedInOrSignedInQueue)
            {
                actions = actions.Where(c => c.BlockActionAttendances.Any(c => c.StudentOsCislo == filter.StudentOsCislo) || c.BlockActionPeopleEnrollQueues.Any(c => c.StudentOsCislo == filter.StudentOsCislo));
            }


            return actions;
        }
        private IQueryable<BlockAction> FilterActionsByAttendance(IQueryable<BlockAction> actions, ActionPostModelDTO filter)
        {
            if (filter.AttendanceEnum == ActionAttendanceEnum.WaitingForApproval)
            {
                actions = actions.Where(c => c.BlockActionAttendances.Any(c => c.StudentOsCislo == filter.StudentOsCislo && !c.EvaluationDate.HasValue));
            }

            if (filter.AttendanceEnum == ActionAttendanceEnum.Visited)
            {
                actions = actions.Where(c => c.BlockActionAttendances.Any(c => c.StudentOsCislo == filter.StudentOsCislo && c.EvaluationDate.HasValue && c.Fulfilled));
            }

            if (filter.AttendanceEnum == ActionAttendanceEnum.Unvisited)
            {
                actions = actions.Where(c => c.BlockActionAttendances.Any(c => c.StudentOsCislo == filter.StudentOsCislo && c.EvaluationDate.HasValue && !c.Fulfilled));
            }

            return actions;
        }

        private IQueryable<BlockAction> GetActionsForStudent(ActionPostModelDTO filter)
        {
            var actions = context.BlockActions
                .Include(c => c.Block)
                .Include(c => c.Block.SubjectInYearTerm)
                .Include(c => c.Block.SubjectInYearTerm.SubjectInYear)
                .Include(c => c.Block.SubjectInYearTerm.SubjectInYear.Subject)
                .Where(c => c.IsActive && c.Visible &&
                (!c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList ||
                (c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList && c.Block.BlockStagUserWhitelists.Any(c => c.StudentOsCislo == filter.StudentOsCislo))));

            return actions;
        }

        private IQueryable<BlockAction> GetActionsForExternalUser(ActionPostModelDTO filter)
        {

            return default;
        }

    }
}
