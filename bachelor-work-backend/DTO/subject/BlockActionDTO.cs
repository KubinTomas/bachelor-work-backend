﻿using bachelor_work_backend.DTO.person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.subject
{
    public class BlockActionDTO
    {
        public int Id { get; set; }
        public int BlockId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Color{ get; set; }
        public DateTime? AttendanceAllowStartDate { get; set; }
        public DateTime? AttendanceAllowEndDate { get; set; }
        public DateTime AttendanceSignOffEndDate { get; set; }
        public bool Visible { get; set; }
        public bool IsActive { get; set; }
        public int GroupId { get; set; }
        public string? UcitelName { get; set; }
        public int StudentCount { get; set; }
        public BlockActionRestrictionDTO BlockActionRestriction { get; set; }
        public int SignedUsersCount { get; set; }
        public int UsersInQueueCount { get; set; }

        public List<ActionPersonDTO>? SignedUsers { get; set; }
        public List<ActionPersonDTO>? SignedUsersInQueue { get; set; }

        public BlockDTO? Block { get; set; }

        public bool IsDeleted { get; set; }

    }
}
