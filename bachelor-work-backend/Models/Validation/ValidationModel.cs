using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Models.Validation
{
    public class ValidationModel
    {
        public IActionResult ActionResult { get; set; }
        public bool IsValid { get; set; }
        public bool IsStudent { get; set; }
        public int UserId { get; set; }
        public string StudentOsCislo { get; set; }
        public ValidationModel()
        {

        }

        public ValidationModel(IActionResult ActionResult)
        {
            this.ActionResult = ActionResult;
            this.IsValid = false;
        }

        public ValidationModel(IActionResult ActionResult, int UserId)
        {
            this.ActionResult = ActionResult;
            this.UserId = UserId;
            this.IsStudent = false;
            this.IsValid = true;
        }

        public ValidationModel(IActionResult ActionResult, string StudentOsCislo)
        {
            this.ActionResult = ActionResult;
            this.StudentOsCislo = StudentOsCislo;
            this.IsStudent = true;
            this.IsValid = true;
        }
    }
}
