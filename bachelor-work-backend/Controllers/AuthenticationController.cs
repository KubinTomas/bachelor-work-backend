using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using bachelor_work_backend.Models;
using bachelor_work_backend.Models.Authentication;
using bachelor_work_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bachelor_work_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private AuthenticationService authenticationService;
        public AuthenticationController()
        {
            authenticationService = new AuthenticationService();
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody]LoginModel user)
        {
            if(user == null)
            {
                return BadRequest("Invalid client request");
            }

            // validation check 
            var authenticationResult = authenticationService.Authorize();

            if (authenticationResult.Result == AuthorizationResultEnum.ok)
            {
                authenticationService.Authorize();

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

            var stagUserClaim = user.Claims.ToList().SingleOrDefault(c => c.Type == CustomClaims.StagToken);

            if (stagUserClaim.Value == "true")
            {
                // TODO RETURN STAG USER, VIA API
                var WSCOOKIE = Request.Cookies["WSCOOKIE"];

                // TODO - Send HTTP requests
                //using (var client = new HttpClient())
                //{
                //    var response = client.GetAsync("umbraco/api/Member/Get?username=test");
                //    response.Wait();
                //}
            
            }
            else
            {
                // RETURN DB USER VIA ID
            }

            //var stream = Request.get
            //var handler = new JwtSecurityTokenHandler();
            //var jsonToken = handler.ReadToken(stream);
            //var tokenS = handler.ReadToken(stream) as JwtSecurityToken

            return Ok();
        }
    }
}