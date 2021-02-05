using bachelor_work_backend.Database;
using bachelor_work_backend.Models.Authentication;
using bachelor_work_backend.Services.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace bachelor_work_backend.Filters.Permission
{
    public class StudentAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.User;
            var roleClaim = user.Claims.SingleOrDefault(c => c.Type == CustomClaims.Role);

            if(roleClaim == null || !Constants.StagRole.Student.Contains(roleClaim.Value))
            {
                filterContext.Result = new ForbidResult("invalid-permission");
            }
        }
    }
}
