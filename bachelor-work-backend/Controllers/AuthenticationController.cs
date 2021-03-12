using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using bachelor_work_backend.Database;
using bachelor_work_backend.DTO.person;
using bachelor_work_backend.Models;
using bachelor_work_backend.Models.Authentication;
using bachelor_work_backend.Services;
using bachelor_work_backend.Services.Authentication;
using bachelor_work_backend.Services.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace bachelor_work_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public Services.AuthenticationService AuthenticationService { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }

        //private readonly SignInManager<ClaimsPrincipal> signInManager;
        //SignInManager<ClaimsPrincipal> signInManager
        public AuthenticationController(IConfiguration configuration, IHttpClientFactory clientFactory, BachContext context)
        {
            ClientFactory = clientFactory;
            Configuration = configuration;
            AuthenticationService = new Services.AuthenticationService(configuration, new StagApiService(configuration, clientFactory), context);
            //this.signInManager = signInManager;

        }

        [HttpGet, Route("role/student")]
        [Authorize]
        public async Task<IActionResult> RoleStudent()
        {
            var claims = User.Claims;

            var roleClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.Role);

            if (roleClaim == null)
            {
                return Ok(false);
            }

            return Ok(Constants.StagRole.StuentRoles.Contains(roleClaim.Value));
        }

        [HttpGet, Route("role/administrator")]
        [Authorize]
        public async Task<IActionResult> RoleAdministrator()
        {
            var claims = User.Claims;

            var roleClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.Role);

            if (roleClaim == null)
            {
                return Ok(false);
            }

            return Ok(Constants.StagRole.AdminRoles.Contains(roleClaim.Value));
        }

        
        [HttpGet, Route("role")]
        [Authorize]
        public async Task<IActionResult> Role()
        {
            var claims = User.Claims;

            var roleClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.Role);

            if(roleClaim == null)
            {
                return Unauthorized();
            }

            return Ok(roleClaim.Value);
        }


        [HttpGet, Route("email/available")]
        public async Task<IActionResult> IsEmailAvailable()
        {
            var email = HttpContext.Request.Headers.SingleOrDefault(c => c.Key == "email").Value;

            var result = AuthenticationService.IsEmailAvailable(email);

            return Ok(result);
        }


        [HttpPost, Route("registration")]
        public async Task<IActionResult> Registration(UserRegistrationDTO user)
        {
            if (!AuthenticationService.IsEmailAvailable(user.Email))
            {
                return BadRequest();

            }

            var result = AuthenticationService.Registration(user);

            return Ok();
        }


        [HttpPost, Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }


        [HttpGet, Route("login")]
        public async Task<IActionResult> Login()
        {

            var claims = new List<Claim>();
              

            var wscookie = Request.Cookies["WSCOOKIE"];

            if (!string.IsNullOrEmpty(wscookie))
            {
                // stag user claims

                var isWscookieValid = await AuthenticationService.IsStagUserCookieValidAsync(wscookie);

                if (!isWscookieValid)
                {
                    return Unauthorized();
                }

                var stagUser = await AuthenticationService.GetStagUserAsync(wscookie);

                claims.Add(new Claim(CustomClaims.UserName, stagUser.activeStagUserInfo.UserName));
                claims.Add(new Claim(CustomClaims.StagToken, "true"));
                claims.Add(new Claim(CustomClaims.Role, stagUser.activeStagUserInfo.Role));
            }
            else
            {
                var email = HttpContext.Request.Headers.SingleOrDefault(c => c.Key == "email").Value;
                var password = HttpContext.Request.Headers.SingleOrDefault(c => c.Key == "password").Value;

                var user = AuthenticationService.Login(email, password);

                if(user == null)
                {
                    return Unauthorized();
                }



                claims.Add(new Claim(CustomClaims.UserId, user.Id.ToString()));
                // external user claims
                claims.Add(new Claim(CustomClaims.StagToken, "false"));
                claims.Add(new Claim(CustomClaims.UserName, ""));
                claims.Add(new Claim(CustomClaims.Role, Constants.StagRole.Student));

            }



            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(new { Token = string.Empty });
        }

        [HttpGet, Route("authorize")]
        [Authorize]
        public async Task<IActionResult> Authorize()
        {
            var user = User;
            var claims = user.Claims.ToList();

            var stagTokenClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.StagToken);
            var userIdClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.UserId);

            if (stagTokenClaim.Value == "true")
            {
                var wscookie = Request.Cookies["WSCOOKIE"];

                if (string.IsNullOrEmpty(wscookie))
                {
                    return Unauthorized();
                }

                var isWscookieValid = await AuthenticationService.IsStagUserCookieValidAsync(wscookie);

                if (!isWscookieValid)
                {
                    return Unauthorized();
                }

                return Ok(true);

            }
            else
            {
                var userId = int.Parse(userIdClaim.Value);

                return Ok(true);
            }
        }

        /// <summary>
        /// zmeni uzivatelskou roli v ramci server cookie a vrati novy model s uzivatelem
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet, Route("user/change-role")]
        [Authorize]
        public async Task<IActionResult> ChangeRole()
        {
            var roleHeader = HttpContext.Request.Headers.SingleOrDefault(c => c.Key == "role");

            if (string.IsNullOrEmpty(roleHeader.Value))
            {
                return BadRequest("missing-role-name");
            }

            var user = User;
            var claims = user.Claims.ToList();

            var stagTokenClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.StagToken);
            var userIdClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.UserId);
            var userNameClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.UserName);
            var roleClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.Role);

            if (stagTokenClaim.Value == "true")
            {
                var wscookie = Request.Cookies["WSCOOKIE"];

                if (string.IsNullOrEmpty(wscookie))
                {
                    return Unauthorized();
                }

                var stagUser = await AuthenticationService.GetStagUserAsync(wscookie);

                // check if user contains role
                var stagUserInfo = stagUser.stagUserInfo.SingleOrDefault(c => c.UserName == roleHeader.Value);

                // nastaveni role ve ktere jsem
                if (stagUserInfo != null)
                {
                    claims.Remove(userNameClaim);
                    claims.Remove(roleClaim);

                    claims.Add(new Claim(CustomClaims.UserName, stagUserInfo.UserName));
                    claims.Add(new Claim(CustomClaims.Role, stagUserInfo.Role));

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {

                    };

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                           CookieAuthenticationDefaults.AuthenticationScheme,
                           new ClaimsPrincipal(claimsIdentity),
                           authProperties);
                }

                // nastaveni aktivni role pro UI, dle cookie claimu a nikoliv STAG claimu, jedna se o vizualni prepsani
                // uzivatel v te roli neni doslova, pokud vni bude must být, tak je třeba vyřešit na úrovni stagu
                var userNameClaimDefined = claims.SingleOrDefault(c => c.Type == CustomClaims.UserName);

                var roleByServerCookie = stagUser.stagUserInfo.SingleOrDefault(c => c.UserName == userNameClaimDefined.Value);

                if (roleByServerCookie != null)
                {
                    stagUser.activeStagUserInfo = roleByServerCookie;
                }

                return Ok(stagUser);

            }
            else
            {
                var userId = int.Parse(userIdClaim.Value);

                return Ok(AuthenticationService.GetDbUser(userId));
            }
        }



        [HttpGet, Route("user")]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var user = User;
            var claims = user.Claims.ToList();

            var stagTokenClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.StagToken);
            var userIdClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.UserId);
            var userNameClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.UserName);

            if (stagTokenClaim.Value == "true")
            {
                var wscookie = Request.Cookies["WSCOOKIE"];

                if (string.IsNullOrEmpty(wscookie))
                {
                    return Unauthorized();
                }

                var stagUser = await AuthenticationService.GetStagUserAsync(wscookie);

                // nastaveni role ve ktere jsem
                if (string.IsNullOrEmpty(userNameClaim.Value))
                {
                    claims.Remove(userNameClaim);
                    claims.Add(new Claim(CustomClaims.UserName, stagUser.activeStagUserInfo.UserName));
                }

                // nastaveni aktivni role pro UI, dle cookie claimu a nikoliv STAG claimu, jedna se o vizualni prepsani
                // uzivatel v te roli neni doslova, pokud vni bude must být, tak je třeba vyřešit na úrovni stagu
                var userNameClaimDefined = claims.SingleOrDefault(c => c.Type == CustomClaims.UserName);

                var roleByServerCookie = stagUser.stagUserInfo.SingleOrDefault(c => c.UserName == userNameClaimDefined.Value);

                if (roleByServerCookie != null)
                {
                    stagUser.activeStagUserInfo = roleByServerCookie;
                }

                return Ok(stagUser);

            }
            else
            {
                var userId = int.Parse(userIdClaim.Value);

                return Ok(AuthenticationService.GetDbUser(userId));
            }
        }

        [HttpGet, Route("email/send-confirm")]
        public async Task<IActionResult> SendConfirmAccountEmail()
        {
            var email = HttpContext.Request.Headers.SingleOrDefault(c => c.Key == "email").Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("specifie-email");
            }

            var result = AuthenticationService.SendConfirmAccountEmail(email);

            if (!result)
            {
                return BadRequest("user-email-was-not-send");
            }

            return Ok();
        }

        [HttpGet, Route("account/confirm")]
        public async Task<IActionResult> ConfirmAccount()
        {
            var userGuid = HttpContext.Request.Headers.SingleOrDefault(c => c.Key == "guid").Value;

            if (string.IsNullOrEmpty(userGuid))
            {
                return BadRequest("specifie-guid");
            }

            var result = AuthenticationService.ConfirmAccount(userGuid);

            if (!result)
            {
                return BadRequest("user-not-found");
            }

            return Ok();
        }

    
    }
}