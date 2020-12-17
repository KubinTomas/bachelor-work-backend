using bachelor_work_backend.Models;
using bachelor_work_backend.Models.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace bachelor_work_backend.Services.Authentication
{
    public class CookieAuthenticationService : IAuthenticationService
    {
       
        public async Task<AuthenticationResult> Authorize()
        {
         

            return null;
        }

        public User GetDbUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetStagUserAsync(string token = "")
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsStagUserCookieValidAsync(string wscookie)
        {
            throw new NotImplementedException();
        }
    }
}
