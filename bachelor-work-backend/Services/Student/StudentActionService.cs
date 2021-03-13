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
                var actionDto = filter.IsStudent? GetStudentActionDTO(c, filter.StudentOsCislo) : GetUserActionDTO(c, filter.UserId);
               
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
                actions = actions.Where(c => !c.isDeleted && !c.BlockActionAttendances.Any(c => c.StudentOsCislo == filter.StudentOsCislo) && !c.BlockActionPeopleEnrollQueues.Any(c => c.StudentOsCislo == filter.StudentOsCislo));
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
                .Include(c => c.Block.BlockRestriction)
                .Where(c => c.IsActive && c.Visible &&
                (!c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList ||
                (c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList && c.Block.BlockStagUserWhitelists.Any(c => c.StudentOsCislo == filter.StudentOsCislo))));

            return actions;
        }

        private IQueryable<BlockAction> GetActionsForExternalUser(ActionPostModelDTO filter)
        {
            var actions = context.BlockActions
            .Include(c => c.BlockActionRestriction)
            .Include(c => c.BlockActionAttendances)
            .Include(c => c.BlockActionPeopleEnrollQueues)
            .Include(c => c.Block.BlockStagUserWhitelists)
            .Include(c => c.Block.BlockRestriction)
            .Where(c => c.IsActive && c.Visible && c.BlockActionRestriction.AllowExternalUsers);

            return actions;
        }
        public bool IsActionFull(BlockAction action)
        {
            return action.BlockActionAttendances.Count >= action.BlockActionRestriction.MaxCapacity;
        }
        public StudentBlockActionDTO? GetStudentActionDTO(int id, string studentOsCislo)
        {
            var action = GetStudentAction(id, studentOsCislo);

            if (action == null)
            {
                return default;
            }

            return GetStudentActionDTO(action, studentOsCislo);
        }

        public int GetOrderInQueue(BlockAction action, int userId)
        {
            var order = 1;
            var queueList = action.BlockActionPeopleEnrollQueues.Select(c => userId).ToList();

            foreach (var queue in queueList)
            {
                if (queue == userId)
                {
                    return order;
                }

                order++;
            }

            return order;
        }

        public int GetOrderInQueue(BlockAction action, string studentOsCislo)
        {
            var order = 1;
            var queueList = action.BlockActionPeopleEnrollQueues.Select(c => studentOsCislo).ToList();

            foreach (var queue in queueList)
            {
                if (queue == studentOsCislo)
                {
                    return order;
                }

                order++;
            }
     
            return order;
        }

        public StudentBlockActionDTO GetUserActionDTO(int actionId, int userId)
        {
            var action = GetStudentAction(actionId);

            if (action == null)
            {
                return default;
            }

            return GetUserActionDTO(action, userId);
        }


            public StudentBlockActionDTO GetUserActionDTO(BlockAction action, int userId)
        {
            var actionDto = mapper.Map<BlockAction, StudentBlockActionDTO>(action);

            actionDto.IsUserSignedIn = action.BlockActionAttendances.Any(c => c.UserId == userId);
            actionDto.IsUserSignedInQueue = action.BlockActionPeopleEnrollQueues.Any(c => c.UserId == userId);

            if (actionDto.IsUserSignedInQueue)
            {
                actionDto.OrderInQueue = GetOrderInQueue(action, userId);
            }

            actionDto.BlockAttendanceRestrictionAllowSignIn = BlockAttendanceRestrictionAllowSignIn(action, userId);
            actionDto.DateRestrictionCanSignIn = DateRestrictionCanSignToAction(action);



            if (!actionDto.BlockAttendanceRestrictionAllowSignIn)
            {
                actionDto.BlockAttendanceRestrictionAllowSignInMessageCode = BlockAttendanceRestrictionAllowSignInMessageCode();
            }

            // nejsem prihlasen, nejsem ve fronte, kapacita je volna, a mam nesplnenou dochazku TODO: Dotat kontrolu te dochazky...
            actionDto.CanSignIn = !actionDto.IsUserSignedIn && !actionDto.IsUserSignedInQueue && actionDto.MaxCapacity > actionDto.SignedUsersCount && actionDto.BlockAttendanceRestrictionAllowSignIn;
            actionDto.CanSignInQueue = !actionDto.IsUserSignedIn && !actionDto.IsUserSignedInQueue && actionDto.SignedUsersCount >= actionDto.MaxCapacity && actionDto.BlockAttendanceRestrictionAllowSignIn;

            return actionDto;
        }

        public StudentBlockActionDTO GetStudentActionDTO(BlockAction action, string studentOsCislo)
        {
            var actionDto = mapper.Map<BlockAction, StudentBlockActionDTO>(action);

            actionDto.IsUserSignedIn = action.BlockActionAttendances.Any(c => c.StudentOsCislo == studentOsCislo);
            actionDto.IsUserSignedInQueue = action.BlockActionPeopleEnrollQueues.Any(c => c.StudentOsCislo == studentOsCislo);

            if (actionDto.IsUserSignedInQueue)
            {
                actionDto.OrderInQueue = GetOrderInQueue(action, studentOsCislo);
            }

            actionDto.BlockAttendanceRestrictionAllowSignIn = BlockAttendanceRestrictionAllowSignIn(action, studentOsCislo);
            actionDto.DateRestrictionCanSignIn = DateRestrictionCanSignToAction(action);



            if (!actionDto.BlockAttendanceRestrictionAllowSignIn)
            {
                actionDto.BlockAttendanceRestrictionAllowSignInMessageCode = BlockAttendanceRestrictionAllowSignInMessageCode();
            }

            // nejsem prihlasen, nejsem ve fronte, kapacita je volna, a mam nesplnenou dochazku TODO: Dotat kontrolu te dochazky...
            actionDto.CanSignIn = !actionDto.IsUserSignedIn && !actionDto.IsUserSignedInQueue && actionDto.MaxCapacity > actionDto.SignedUsersCount && actionDto.BlockAttendanceRestrictionAllowSignIn;
            actionDto.CanSignInQueue = !actionDto.IsUserSignedIn && !actionDto.IsUserSignedInQueue && actionDto.SignedUsersCount >= actionDto.MaxCapacity && actionDto.BlockAttendanceRestrictionAllowSignIn;

            return actionDto;
        }

        public BlockAction? GetStudentAction(int id)
        {
            return context.BlockActions
               .Include(c => c.BlockActionRestriction)
               .Include(c => c.BlockActionAttendances)
               .Include(c => c.BlockActionPeopleEnrollQueues)
               .Include(c => c.Block.BlockStagUserWhitelists)
               .Include(c => c.Block.BlockRestriction)
            .SingleOrDefault(c => c.Id == id && c.IsActive && c.Visible && c.BlockActionRestriction.AllowExternalUsers);
        }

        public BlockAction? GetStudentAction(int id, string studentOsCislo)
        {
            return context.BlockActions
               .Include(c => c.BlockActionRestriction)
               .Include(c => c.BlockActionAttendances)
               .Include(c => c.BlockActionPeopleEnrollQueues)
               .Include(c => c.Block.BlockStagUserWhitelists)
               .Include(c => c.Block.BlockRestriction)
            .SingleOrDefault(c => c.Id == id && c.IsActive && c.Visible &&
            (!c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList ||
            (c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList && c.Block.BlockStagUserWhitelists.Any(c => c.StudentOsCislo == studentOsCislo))));
        }

        public bool IsStudentSignedInActionQueue(BlockAction action, string studentOsCislo)
        {
            return action.BlockActionAttendances.Any(c => c.StudentOsCislo == studentOsCislo);

        }

        public bool IsStudentSignedInActionQueue(BlockAction action, int userId)
        {
            return action.BlockActionAttendances.Any(c => c.UserId == userId);

        }

        public bool IsStudentSignedInAction(BlockAction action, int userId)
        {
            return action.BlockActionPeopleEnrollQueues.Any(c => c.UserId == userId);
        }


        public bool IsStudentSignedInAction(BlockAction action, string studentOsCislo)
        {
            return action.BlockActionPeopleEnrollQueues.Any(c => c.StudentOsCislo == studentOsCislo);
        }

        public int BlockAttendanceRestrictionAllowSignInMessageCode()
        {
            return Constants.Action.WaitingForAttendanceEvaluation;
        }

        public bool BlockAttendanceRestrictionAllowSignIn(BlockAction action, int userId)
        {
            var blockMaxActionAttendLimit = action.Block.BlockRestriction.ActionAttendLimit;

            if (blockMaxActionAttendLimit == 0)
            {
                return true;
            }

            var blockActions = context.BlockActions
                .Include(c => c.BlockActionAttendances)
                .Include(c => c.BlockActionPeopleEnrollQueues)
                .Where(c => c.BlockId == action.BlockId && c.IsActive && c.Visible && c.BlockActionRestriction.AllowExternalUsers).ToList();

            var attendanceToEvaluate = action.Block.BlockActions.Count(c => c.BlockActionAttendances.Any(x => x.UserId == userId && !x.EvaluationDate.HasValue));
            var attendanceFinished = action.Block.BlockActions.Count(c => c.BlockActionAttendances.Any(x => x.UserId == userId && x.EvaluationDate.HasValue && x.Fulfilled));

            var attendanceInQueueWhichCanStillMoveUserToAttendance = action.Block.BlockActions
                .Count(c => c.BlockActionPeopleEnrollQueues.Any(x => x.UserId == userId && c.AttendanceSignOffEndDate > DateTime.Now));


            return (attendanceToEvaluate + attendanceFinished + attendanceInQueueWhichCanStillMoveUserToAttendance) < blockMaxActionAttendLimit;
        }

        /// <summary>
        /// Blok má nastaveno kolikrát uživatel může být v řadě přihlášen na akce. Pokud 0, uživatel se může přihlásit na všechny akce. Pokud třeba 3. Uživatel se může současně přihlásit max na 3 akce.
        /// Pokud splní docházku akce, pak se může přihlásit jen na 2. Pokud se čeká na vyhodnocení docházky, stále se může přihlásit jen na 3 - počet akcí, kde se vyhodnocuje docházka - již úspěšně absolbované akce
        /// </summary>
        /// <param name="action"></param>
        /// <param name="studentOsCislo"></param>
        /// <returns></returns>
        public bool BlockAttendanceRestrictionAllowSignIn(BlockAction action, string studentOsCislo)
        {
            var blockMaxActionAttendLimit = action.Block.BlockRestriction.ActionAttendLimit;

            if (blockMaxActionAttendLimit == 0)
            {
                return true;
            }

            var blockActions = context.BlockActions
                .Include(c => c.BlockActionAttendances)
                .Include(c => c.BlockActionPeopleEnrollQueues)
                .Where(c => c.BlockId == action.BlockId && c.IsActive && c.Visible &&
                (!c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList ||
                (c.BlockActionRestriction.AllowOnlyStudentsOnWhiteList && c.Block.BlockStagUserWhitelists.Any(c => c.StudentOsCislo == studentOsCislo)))).ToList();

            var attendanceToEvaluate = action.Block.BlockActions.Count(c => c.BlockActionAttendances.Any(x => x.StudentOsCislo == studentOsCislo && !x.EvaluationDate.HasValue));
            var attendanceFinished = action.Block.BlockActions.Count(c => c.BlockActionAttendances.Any(x => x.StudentOsCislo == studentOsCislo && x.EvaluationDate.HasValue && x.Fulfilled));

            var attendanceInQueueWhichCanStillMoveUserToAttendance = action.Block.BlockActions
                .Count(c => c.BlockActionPeopleEnrollQueues.Any(x => x.StudentOsCislo == studentOsCislo && c.AttendanceSignOffEndDate > DateTime.Now));


            return (attendanceToEvaluate + attendanceFinished + attendanceInQueueWhichCanStillMoveUserToAttendance) < blockMaxActionAttendLimit;
        }

        public bool StudentJoinAction(BlockAction action, string studentOsCislo)
        {
            if (!DateRestrictionCanSignToAction(action) || IsStudentSignedInActionQueue(action, studentOsCislo) || IsStudentSignedInAction(action, studentOsCislo) || !BlockAttendanceRestrictionAllowSignIn(action, studentOsCislo))
            {
                return false;
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

            return true;
        }

        public bool StudentJoinAction(BlockAction action, int userId)
        {
            if (!DateRestrictionCanSignToAction(action) 
                || IsStudentSignedInActionQueue(action, userId) 
                || IsStudentSignedInAction(action, userId)
                || !BlockAttendanceRestrictionAllowSignIn(action, userId))
            {
                return false;
            }

            var actionSign = new BlockActionAttendance()
            {
                ActionId = action.Id,
                DateIn = DateTime.Now,
                Fulfilled = false,
                UserId = userId,
            };

            action.BlockActionAttendances.Add(actionSign);
            context.SaveChanges();

            return true;
        }



        public bool StudentJoinActionQueue(BlockAction action, string studentOsCislo)
        {
            if (!DateRestrictionCanSignToAction(action) || IsStudentSignedInActionQueue(action, studentOsCislo) || IsStudentSignedInAction(action, studentOsCislo) || !BlockAttendanceRestrictionAllowSignIn(action, studentOsCislo))
            {
                return false;
            }

            var queue = new BlockActionPeopleEnrollQueue()
            {
                ActionId = action.Id,
                DateIn = DateTime.Now,
                StudentOsCislo = studentOsCislo,
            };

            action.BlockActionPeopleEnrollQueues.Add(queue);
            context.SaveChanges();

            return true;
        }

        public bool StudentJoinActionQueue(BlockAction action, int userId)
        {
            if (!DateRestrictionCanSignToAction(action) || 
                IsStudentSignedInActionQueue(action, userId) || 
                IsStudentSignedInAction(action, userId) || 
                !BlockAttendanceRestrictionAllowSignIn(action, userId))
            {
                return false;
            }

            var queue = new BlockActionPeopleEnrollQueue()
            {
                ActionId = action.Id,
                DateIn = DateTime.Now,
                UserId = userId,
            };

            action.BlockActionPeopleEnrollQueues.Add(queue);
            context.SaveChanges();

            return true;
        }

        public bool CanSignOffFromAction(BlockAction action)
        {
            return action.AttendanceSignOffEndDate >= DateTime.Now && !(DateTime.Now >= action.StartDate);
        }

        public bool DateRestrictionCanSignToAction(BlockAction action)
        {
            if (action.AttendanceAllowStartDate.HasValue && DateTime.Now < action.AttendanceAllowStartDate.Value)
            {
                return false;
            }

            if (action.AttendanceAllowEndDate.HasValue && DateTime.Now > action.AttendanceAllowEndDate.Value)
            {
                return false;
            }

            return true;
        }

        public bool StudentLeaveActionQueue(BlockAction action, string studentOsCislo, bool ignoreValidation = false)
        {
            var queue = action.BlockActionPeopleEnrollQueues.SingleOrDefault(c => c.StudentOsCislo == studentOsCislo);

            if (queue == null)
            {
                return true;
            }

            if (!ignoreValidation && !CanSignOffFromAction(action))
            {
                return false;
            }

            context.BlockActionPeopleEnrollQueues.Remove(queue);
            context.SaveChanges();

            return true;
        }

        public bool StudentLeaveActionQueue(BlockAction action, int userId, bool ignoreValidation = false)
        {
            var queue = action.BlockActionPeopleEnrollQueues.SingleOrDefault(c => c.UserId == userId);

            if (queue == null)
            {
                return true;
            }

            if (!ignoreValidation && !CanSignOffFromAction(action))
            {
                return false;
            }

            context.BlockActionPeopleEnrollQueues.Remove(queue);
            context.SaveChanges();

            return true;
        }

        public bool StudentLeaveAction(BlockAction action, string studentOsCislo, bool ignoreValidation = false)
        {
            var actionSign = action.BlockActionAttendances.SingleOrDefault(c => c.StudentOsCislo == studentOsCislo);

            if (actionSign == null)
            {
                return true;
            }

            if (!ignoreValidation && !CanSignOffFromAction(action))
            {
                return false;
            }

            context.BlockActionAttendances.Remove(actionSign);
            context.SaveChanges();

            TryMovePeopleInQueue(action.Id);

            return true;
        }

        public bool StudentLeaveAction(BlockAction action, int userId)
        {
            var actionSign = action.BlockActionAttendances.SingleOrDefault(c => c.UserId == userId);

            if (actionSign == null)
            {
                return true;
            }

            if (!CanSignOffFromAction(action))
            {
                return false;
            }

            context.BlockActionAttendances.Remove(actionSign);
            context.SaveChanges();

            TryMovePeopleInQueue(action.Id);

            return true;
        }
        

        /// <summary>
        /// Zatim funguje jen pro studenty
        /// </summary>
        /// <param name="actionId"></param>
        public void TryMovePeopleInQueue(int actionId)
        {
            var action = context.BlockActions
               .Include(c => c.BlockActionRestriction)
               .Include(c => c.BlockActionAttendances)
               .Include(c => c.BlockActionPeopleEnrollQueues)
               .Include(c => c.Block.BlockStagUserWhitelists)
               .Include(c => c.Block.BlockRestriction)
               .Single(c => c.Id == actionId);

            var actionLimit = action.BlockActionRestriction.MaxCapacity;
            var signedPeopleCount = action.BlockActionAttendances.Count();

            if (signedPeopleCount < actionLimit && action.BlockActionPeopleEnrollQueues.Count != 0)
            {
                var peopleInQueue = action.BlockActionPeopleEnrollQueues.OrderBy(c => c.DateIn);
                var personToMove = action.BlockActionPeopleEnrollQueues.First();

                if (personToMove.UserId.HasValue)
                {
                    var result = StudentJoinActionQueue(action, personToMove.UserId.Value);

                    if (result)
                    {
                        StudentLeaveActionQueue(action, personToMove.UserId.Value, true);
                    }
                }
                else if(!string.IsNullOrEmpty(personToMove.StudentOsCislo))
                {
                    var result = StudentJoinActionQueue(action, personToMove.StudentOsCislo);

                    if (result)
                    {
                        StudentLeaveActionQueue(action, personToMove.StudentOsCislo, true);
                    }
                }



            }
        }

    }
}
