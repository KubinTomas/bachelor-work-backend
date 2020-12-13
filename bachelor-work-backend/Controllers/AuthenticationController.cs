using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using bachelor_work_backend.Models;
using bachelor_work_backend.Models.Authentication;
using bachelor_work_backend.Services;
using bachelor_work_backend.Services.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace bachelor_work_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public AuthenticationService AuthenticationService { get; private set; }
        public StagApiService StagApiService { get; private set; }

        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }
        public AuthenticationController(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
            Configuration = configuration;
            StagApiService = new StagApiService(configuration, clientFactory);
            AuthenticationService = new AuthenticationService(configuration, StagApiService);
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody]LoginModel user)
        {
            if(user == null)
            {
                return BadRequest("Invalid client request");
            }

            // validation check 
            var authenticationResult = AuthenticationService.Authorize();

            if (authenticationResult.Result == AuthorizationResultEnum.ok)
            {
                AuthenticationService.Authorize();

                return Ok(new { Token = authenticationResult.TokenString });
            }

            return Unauthorized();
        }

        [HttpGet, Route("user")]
        [Authorize]
        public IActionResult GetUser()
        {
            var user = User;
            var claims = user.Claims.ToList();

            var stagTokenClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.StagToken);
            var userIdClaim = claims.SingleOrDefault(c => c.Type == CustomClaims.UserId);

            if (stagTokenClaim.Value == "true")
            {
                var wscookie = Request.Cookies["WSCOOKIE"];

                return Ok(AuthenticationService.GetStagUser(wscookie));
            
            }
            else
            {
                var userId = int.Parse(userIdClaim.Value);

                return Ok(AuthenticationService.GetDbUser(userId));
            }
        }
    }
}