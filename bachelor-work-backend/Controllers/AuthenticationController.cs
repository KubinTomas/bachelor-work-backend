using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
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
        public Services.Authentication.IAuthenticationService AuthenticationService { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }

        //private readonly SignInManager<ClaimsPrincipal> signInManager;
        //SignInManager<ClaimsPrincipal> signInManager
        public AuthenticationController(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
            Configuration = configuration;
            AuthenticationService = new JwtAuthenticationService(configuration, new StagApiService(configuration, clientFactory));
            //this.signInManager = signInManager;
        }

        [HttpPost, Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }


        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginModel user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            var claims = new List<Claim>
                {
                    new Claim(CustomClaims.StagToken, "true"),
                    new Claim(CustomClaims.UserId, "0"),
                    new Claim(CustomClaims.UserName, string.Empty),
                };

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
            //var authenticationResult = await AuthenticationService.Authorize();


            //if (authenticationResult.Result == AuthorizationResultEnum.ok)
            //{
            //    return Ok(new { Token = authenticationResult.TokenString });
            //}
            //return Unauthorized();
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

            if (stagTokenClaim.Value == "true")
            {
                var wscookie = Request.Cookies["WSCOOKIE"];

                if (string.IsNullOrEmpty(wscookie))
                {
                    return Unauthorized();
                }

                var stagUser = await AuthenticationService.GetStagUserAsync(wscookie);

                // check if user contains role
                var userHasRole = stagUser.stagUserInfo.Any(c => c.UserName == roleHeader.Value);

                // nastaveni role ve ktere jsem
                if (userHasRole)
                {
                    claims.Remove(userNameClaim);
                    claims.Add(new Claim(CustomClaims.UserName, roleHeader.Value));

                    // TODO ULOZIT CLAIM !!
                    //await this.signInManager.RefreshSignInAsync(user);

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
    }
}