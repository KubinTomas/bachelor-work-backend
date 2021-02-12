﻿using AutoMapper;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.person;
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
namespace bachelor_work_backend.Services.SubjectFolder
{
    public class ActionService
    {
        private readonly BachContext context;
        private readonly IMapper mapper;
        public StagApiService StagApiService { get; private set; }
        public ActionService(BachContext context, IMapper mapper, StagApiService StagApiService)
        {
            this.context = context;
            this.mapper = mapper;
            this.StagApiService = StagApiService;
        }

        public void Update(BlockActionDTO actionDTO)
        {
            var action = Get(actionDTO.Id);

            if (action != null)
            {
                Update(action, actionDTO);
            }
        }

        public void Update(BlockAction action, BlockActionDTO actionDTO)
        {
            action.Name = actionDTO.Name;

            context.SaveChanges();
        }

        public async Task<BlockActionDTO?> GetDto(int actionId, string wscookie)
        {
            var action = Get(actionId);

            if (action == null)
            {
                return default;
            }

            var actionDto = mapper.Map<BlockAction, BlockActionDTO>(action);

            var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(action.UcitIdno.Trim(), wscookie);

            if (ucitelInfo != null)
            {
                actionDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
            }

            actionDto.SignedUsers = await GetSignedPersons(action.BlockActionAttendances.ToList(), wscookie);
            actionDto.SignedUsersInQueue = await GetSignedPersonsQueue(action.BlockActionPeopleEnrollQueues.OrderByDescending(c => c.Id).ToList(), wscookie);

            return actionDto;
        }
        private async Task<List<ActionPersonDTO>> GetSignedPersonsQueue(List<BlockActionPeopleEnrollQueue> queue, string wscookie)
        {
            var persons = new List<ActionPersonDTO>();

            var order = 1;

            foreach (var entry in queue)
            {
                var student = new StagStudent();
                var isStudent = !string.IsNullOrEmpty(entry.StudentOsCislo);

                if (isStudent)
                {
                    student = await StagApiService.StagStudentApiService.GetStudentInfo(entry.StudentOsCislo, wscookie);
                }

                var person = new ActionPersonDTO
                {
                    Id = entry.Id,
                    Fullname = isStudent ? student.jmeno + " " + student.prijmeni : "",
                    IsStagStudent = !string.IsNullOrEmpty(entry.StudentOsCislo),
                    DateIn = entry.DateIn,
                    rocnik = student.rocnik,
                    fakultaSp = student.fakultaSp,
                    queueOrder = order++
                };

                persons.Add(person);
            }

            return persons;
        }


        private async Task<List<ActionPersonDTO>> GetSignedPersons(List<BlockActionAttendance> attendances, string wscookie)
        {
            var persons = new List<ActionPersonDTO>();

            foreach (var attendance in attendances)
            {
                var student = new StagStudent();
                var isStudent = !string.IsNullOrEmpty(attendance.StudentOsCislo);

                if (isStudent)
                {
                    student = await StagApiService.StagStudentApiService.GetStudentInfo(attendance.StudentOsCislo, wscookie);
                }

                var person = new ActionPersonDTO
                {
                    Id = attendance.Id,
                    Fullname = isStudent? student.jmeno + " " + student.prijmeni : "",
                    IsStagStudent = !string.IsNullOrEmpty(attendance.StudentOsCislo),
                    EvaluationDate = attendance.EvaluationDate,
                    Fulfilled = attendance.Fulfilled,
                    rocnik = student.rocnik,
                    fakultaSp = student.fakultaSp
                };

                persons.Add(person);
            }

            return persons;
        }

    


        public BlockAction? Get(int actionId)
        {
            return context.BlockActions
                .Include(c => c.BlockActionAttendances)
                .Include(c => c.BlockActionPeopleEnrollQueues)
                .Include(c => c.BlockActionRestriction)
                .SingleOrDefault(c => c.Id == actionId && c.IsActive);
        }


        public void Delete(int actionId)
        {
            var action = Get(actionId);

            if (action != null)
            {
                Delete(action);
            }
        }

        public void Delete(BlockAction action)
        {
            action.IsActive = false;
            context.SaveChanges();
        }

        public void Create(BlockAction action)
        {
            action.IsActive = true;
            action.DateIn = DateTime.Now;

            context.BlockActions.Add(action);
            context.SaveChanges();
        }


        public async Task<List<BlockActionDTO>> GetDTOAsync(int blockId, string ucitelIdno, string wscookie)
        {
            var actionsDTO = new List<BlockActionDTO>();

            var actions = context.BlockActions
                .Include(c => c.BlockActionAttendances)
                .Include(c => c.BlockActionPeopleEnrollQueues)
                .Include(c => c.BlockActionRestriction)
                .Where(c => c.BlockId == blockId && c.IsActive).ToList();

            foreach (var action in actions)
            {
                var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(action.UcitIdno.Trim(), wscookie);
                var actionDTO = mapper.Map<BlockAction, BlockActionDTO>(action);

                if (ucitelInfo != null)
                {
                    actionDTO.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
                }

                actionsDTO.Add(actionDTO);
            }

            return actionsDTO;
        }

        //public async Task<BlockDTO> GetSingleDTOAsync(int blockId, string ucitelIdno, string wscookie)
        //{
        //    var block = Get(blockId);

        //    if (block == null)
        //    {
        //        return default;
        //    }

        //    var blockDto = mapper.Map<Block, BlockDTO>(block);

        //    var ucitelInfo = await StagApiService.StagUserApiService.GetUcitelInfoAsync(block.UcitIdno.Trim(), wscookie);

        //    if (ucitelInfo != null)
        //    {
        //        blockDto.UcitelName = ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni;
        //    }

        //    return blockDto;

        //}
    }
}
