using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.person;
using bachelor_work_backend.Models;
using bachelor_work_backend.Models.Authentication;
using bachelor_work_backend.Services.Authentication;
using bachelor_work_backend.Services.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        public MailService MailService { get; private set; }


        public AuthenticationService(IConfiguration configuration, StagApiService stagApiService, BachContext context)
        {
            Configuration = configuration;
            StagApiService = stagApiService;
            Context = context;
            MailService = new MailService(configuration);
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
            var user = GetUser(userId);

            var userDto = new UserDto()
            {
                Email = user.Email,
                Name = user.Name,
                activeStagUserInfo = new StagUserInfo()
                {
                    Role = Constants.StagRole.Student
                },
                stagUserInfo = new List<StagUserInfo>()
            };

            return userDto;
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
                Email = userDTO.Email.Trim(),
                Guid = Guid.NewGuid().ToString(),
                Confirmed = false,
                Password = GetHashPassowrd(userDTO.Password)
            };


            Context.Users.Add(user);
            Context.SaveChanges();

            return true;
        }

        private string GetHashPassowrd(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public User? GetUser(string email)
        {
            return Context.Users.SingleOrDefault(c => c.Email.ToLower() == email.Trim().ToLower());
        }

        public void RecoverPassword(User user)
        {
            var passwordRecovery = new UserPasswordRecovery
            {
                Guid = Guid.NewGuid().ToString(),
                UserId = user.Id,
                ValidUntil = DateTime.Now.AddMinutes(30)
            };

            Context.UserPasswordRecoveries.Add(passwordRecovery);
            Context.SaveChanges();

            MailService.SendPasswordRecoveryMail(user.Email, passwordRecovery.Guid, passwordRecovery.ValidUntil);
        }

        public User? GetUser(int id)
        {
            return Context.Users.SingleOrDefault(c => c.Id == id);
        }
        public User? Login(string email, string password)
        {
            var user = GetUser(email);

            if (user == null)
            {
                return null;
            }

            var validPassword = VerifyPassword(password, user.Password);

            return validPassword ? user : null;
        }


        public bool IsEmailAvailable(string email)
        {
            return !Context.Users.Any(c => c.Email.ToLower() == email.ToLower());
        }

        public bool VerifyPassword(string password, string passwordHashed)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHashed);
        }

        public bool ConfirmAccount(string userGuid)
        {
            var user = Context.Users.SingleOrDefault(c => c.Guid == userGuid);

            if (user == null)
            {
                return false;
            }

            user.Confirmed = true;

            Context.SaveChanges();

            return true;
        }

        public bool SendConfirmAccountEmail(string email)
        {
            var user = Context.Users.SingleOrDefault(c => c.Email == email.ToLower().Trim());

            if (user == null)
            {
                return false;
            }

            if (!user.Confirmed)
            {
                MailService.SendConfirmationMail(user.Email, user.Guid);

                return true;
            }

            return false;
        }


        public bool PasswordRecover(string userGuid, string password)
        {
            var recovery = Context.UserPasswordRecoveries.Include(c => c.User).SingleOrDefault(c => c.Guid == userGuid);

            if (recovery == null)
            {
                return false;
            }

            if(recovery.ValidUntil < DateTime.Now)
            {
                return false;
            }

            recovery.User.Password = GetHashPassowrd(password);

            Context.SaveChanges();

            return true;
        }
    }
}
