using bachelor_work_backend.Models;
using bachelor_work_backend.Models.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> Authorize();
        Task<User> GetStagUserAsync(string token = "");
        Task<bool> IsStagUserCookieValidAsync(string wscookie);
        User GetDbUser(int userId);
    }
}
