using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.DTO.student
{
    public class StudentBlockActionDTO
    {
        public int Id { get; set; }
        public int BlockId { get; set; }
        public string Name { get; set; }
        public string SubjectName { get; set; }
        public string SubjectYear { get; set; }
        public string SubjectTerm { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Color{ get; set; }
        public DateTime? AttendanceAllowStartDate { get; set; }
        public DateTime? AttendanceAllowEndDate { get; set; }
        public DateTime AttendanceSignOffEndDate { get; set; }
        public string? UcitelName { get; set; }
        public int MaxCapacity { get; set; }
        public int SignedUsersCount { get; set; }
        public int UsersInQueueCount { get; set; }
        public bool IsUserSignedIn { get; set; }
        public bool IsUserSignedInQueue { get; set; }
        public bool CanSignIn { get; set; }
        public bool BlockAttendanceRestrictionAllowSignIn { get; set; }
        public int BlockAttendanceRestrictionAllowSignInMessageCode { get; set; }
        public bool CanSignInQueue { get; set; }
        public bool CanSignOfTheAction { get; set; }
        public bool DateRestrictionCanSignIn { get; set; }
        public int OrderInQueue { get; set; }
        public bool IsDeleted { get; set; }

    }
}
