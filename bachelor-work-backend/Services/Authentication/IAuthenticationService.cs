using bachelor_work_backend.DTO.person;
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
        Task<UserDto> GetStagUserAsync(string token = "");
        Task<bool> IsStagUserCookieValidAsync(string wscookie);
        UserDto GetDbUser(int userId);
        bool Registration(UserRegistrationDTO user);
        bool IsEmailAvailable(string email);
        bool VerifyPassword(string password, string passwordHashed);
    }
}
