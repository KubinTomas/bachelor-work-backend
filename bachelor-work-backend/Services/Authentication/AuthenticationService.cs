using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.person;
using bachelor_work_backend.Models;
using bachelor_work_backend.Models.Authentication;
using bachelor_work_backend.Services.Authentication;
using bachelor_work_backend.Services.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace bachelor_work_backend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public IConfiguration Configuration { get; private set; }
        public StagApiService StagApiService { get; private set; }
        public BachContext Context { get; private set; }

        public AuthenticationService(IConfiguration configuration, StagApiService stagApiService, BachContext context)
        {
            Configuration = configuration;
            StagApiService = stagApiService;
            Context = context;
        }

        public async Task<AuthenticationResult> Authorize()
        {
            if (IsLoginValid())
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("SecretKey")));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(CustomClaims.StagToken, "true"),
                    new Claim(CustomClaims.UserId, "0"),
                };

                var tokenOptions = new JwtSecurityToken(
                    issuer: "https://localhost:44380",
                    audience: "http://localhost:4200",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: signingCredentials
                    );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return new AuthenticationResult(tokenString);
            }
            else
            {
                return new AuthenticationResult();
            }
        }

        public bool IsLoginValid()
        {
            return true;
        }

        public async Task<StagUserInfo> GetActiveStagUserInfoAsync(string wscookie, ClaimsPrincipal user)
        {
            var stagUser = await StagApiService.StagUserApiService.GetStagUserAsync(wscookie);

            var claims = user.Claims.ToList();
            var userNameClaimDefined = claims.SingleOrDefault(c => c.Type == CustomClaims.UserName);

            var roleByServerCookie = stagUser.stagUserInfo.SingleOrDefault(c => c.UserName == userNameClaimDefined.Value);

            if (roleByServerCookie != null)
            {
                stagUser.activeStagUserInfo = roleByServerCookie;
            }

            return stagUser.activeStagUserInfo;
        }


        public async Task<UserDto> GetStagUserAsync(string wscookie)
        {
            return await StagApiService.StagUserApiService.GetStagUserAsync(wscookie);
        }

        public UserDto GetDbUser(int userId)
        {
            throw new NotImplementedException("DbUser side is not implemented yet");
        }

        public Task<bool> IsStagUserCookieValidAsync(string wscookie)
        {
            return StagApiService.StagUserApiService.IsStagUserCookieValidAsync(wscookie);
        }

        public Task<string?> GetUcitelIdnoAsync(string wscookie)
        {
            return StagApiService.StagUserApiService.GetUcitelIdnoAsync(wscookie);
        }

        public async Task<bool> CanManageSubject(string wscookie, Subject subject)
        {
            var stagUser = await GetStagUserAsync(wscookie);

            if (stagUser == null)
            {
                return false;
            }

            var hasPermission = stagUser.stagUserInfo.Any(c => c.Fakulta == subject.Fakulta &&
                                                               c.Katedra == subject.Katedra &&
                                                               (Constants.StagRole.AdminRoles.Contains(c.Role)));

            return hasPermission;
        }
        //BCrypt.Net-Next
        // https://jasonwatmore.com/post/2020/07/16/aspnet-core-3-hash-and-verify-passwords-with-bcrypt
        // https://www.nuget.org/packages/BCrypt.Net-Next/
        public bool Registration(UserRegistrationDTO userDTO)
        {
            if (!IsEmailAvailable(userDTO.Email) || string.IsNullOrEmpty(userDTO.Name) || string.IsNullOrEmpty(userDTO.Surname))
            {
                return false;
            }

            byte[] salt = new byte[128 / 8];

            var user = new User
            {
                Name = userDTO.Name,
                Surname = userDTO.Surname,
                Email = userDTO.Email,
                Guid = Guid.NewGuid().ToString(),
                Confirmed = false,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password)
            };


            Context.Users.Add(user);
            Context.SaveChanges();

            return true;
        }


        public bool IsEmailAvailable(string email)
        {
            return !Context.Users.Any(c => c.Email.ToLower() == email.ToLower());
        }

        public bool VerifyPassword(string password, string passwordHashed)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHashed);
        }
    }
}
