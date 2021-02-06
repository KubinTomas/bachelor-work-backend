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

        public List<StudentBlockActionDTO> GetActions(ActionPostModelDTO filter)
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

            var actionsDto = new List<StudentBlockActionDTO>();

            actions.ToList().ForEach(c =>
            {
                var actionDto = mapper.Map<BlockAction, StudentBlockActionDTO>(c);
                actionDto.IsUserSignedIn = c.BlockActionAttendances.Any(c => c.StudentOsCislo == filter.StudentOsCislo);
                actionDto.IsUserSignedInQueue = c.BlockActionPeopleEnrollQueues.Any(c => c.StudentOsCislo == filter.StudentOsCislo);

                actionDto.BlockAttendanceRestrictionAllowSignIn = BlockAttendanceRestrictionAllowSignIn(c, filter.StudentOsCislo);

                // nejsem prihlasen, nejsem ve fronte, kapacita je volna, a mam nesplnenou dochazku TODO: Dotat kontrolu te dochazky...
                actionDto.CanSignIn = !actionDto.IsUserSignedIn && !actionDto.IsUserSignedInQueue && actionDto.MaxCapacity > actionDto.SignedUsersCount && actionDto.BlockAttendanceRestrictionAllowSignIn;
                actionDto.CanSignInQueue = !actionDto.IsUserSignedIn && !actionDto.IsUserSignedInQueue && actionDto.SignedUsersCount >= actionDto.MaxCapacity && actionDto.BlockAttendanceRestrictionAllowSignIn;

                actionsDto.Add(actionDto);
            });

            return actionsDto;
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
                .Include(c => c.BlockActionRestriction)
                .Include(c => c.BlockActionAttendances)
                .Include(c => c.BlockActionPeopleEnrollQueues)
                .Include(c => c.Block.BlockStagUserWhitelists)
                //.Include(c => c.Block)
                //.Include(c => c.Block.SubjectInYearTerm)
                //.Include(c => c.Block.SubjectInYearTerm.SubjectInYear)
                //.Include(c => c.Block.SubjectInYearTerm.SubjectInYear.Subject)
                .Where(c => c.IsActive && c.Visible &&
                (!c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList ||
                (c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList && c.Block.BlockStagUserWhitelists.Any(c => c.StudentOsCislo == filter.StudentOsCislo))));

            return actions;
        }

        private IQueryable<BlockAction> GetActionsForExternalUser(ActionPostModelDTO filter)
        {

            return default;
        }
        public bool IsActionFull(BlockAction action)
        {
            return action.BlockActionAttendances.Count >= action.BlockActionRestriction.MaxCapacity;
        }
        public BlockAction? GetStudentAction(int id, string studentOsCislo)
        {
            return context.BlockActions
            .Include(c => c.BlockActionRestriction)
            .Include(c => c.BlockActionAttendances)
            .Include(c => c.BlockActionPeopleEnrollQueues)
            .SingleOrDefault(c => c.Id == id && c.IsActive && c.Visible &&
            (!c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList ||
            (c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList && c.Block.BlockStagUserWhitelists.Any(c => c.StudentOsCislo == studentOsCislo))));
        }

        public bool IsStudentSignedInActionQueue(BlockAction action, string studentOsCislo)
        {
            return action.BlockActionAttendances.Any(c => c.StudentOsCislo == studentOsCislo);

        }

        public bool IsStudentSignedInAction(BlockAction action, string studentOsCislo)
        {
            return action.BlockActionPeopleEnrollQueues.Any(c => c.StudentOsCislo == studentOsCislo);
        }

        public bool BlockAttendanceRestrictionAllowSignIn(BlockAction action, string studentOsCislo)
        {
            return true;
        }

        public void StudentJoinAction(BlockAction action, string studentOsCislo)
        {
            if (IsStudentSignedInActionQueue(action, studentOsCislo) || IsStudentSignedInAction(action, studentOsCislo) || !BlockAttendanceRestrictionAllowSignIn(action, studentOsCislo))
            {
                return;
            }

            var actionSign = new BlockActionAttendance()
            {
                ActionId = action.Id,
                DateIn = DateTime.Now,
                Fulfilled = false,
                StudentOsCislo = studentOsCislo,
            };

            action.BlockActionAttendances.Add(actionSign);
            context.SaveChanges();
        }


        public void StudentJoinActionQueue(BlockAction action, string studentOsCislo)
        {
            if (IsStudentSignedInActionQueue(action, studentOsCislo) || IsStudentSignedInAction(action, studentOsCislo) || !BlockAttendanceRestrictionAllowSignIn(action, studentOsCislo))
            {
                return;
            }

            var queue = new BlockActionPeopleEnrollQueue()
            {
                ActionId = action.Id,
                DateIn = DateTime.Now,
                StudentOsCislo = studentOsCislo,
            };

            action.BlockActionPeopleEnrollQueues.Add(queue);
            context.SaveChanges();
        }


        public void StudentLeaveActionQueue(BlockAction action, string studentOsCislo)
        {
            var queue = action.BlockActionPeopleEnrollQueues.SingleOrDefault(c => c.StudentOsCislo == studentOsCislo);

            if (queue == null)
            {
                return;
            }

            context.BlockActionPeopleEnrollQueues.Remove(queue);
            context.SaveChanges();
        }

        public void StudentLeaveAction(BlockAction action, string studentOsCislo)
        {
            var actionSign = action.BlockActionAttendances.SingleOrDefault(c => c.StudentOsCislo == studentOsCislo);

            if (actionSign == null)
            {
                return;
            }

            context.BlockActionAttendances.Remove(actionSign);
            context.SaveChanges();
        }

    }
}
