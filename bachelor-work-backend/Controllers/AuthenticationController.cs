using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bachelor_work_backend.Models;
using bachelor_work_backend.Services;
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
    }
}