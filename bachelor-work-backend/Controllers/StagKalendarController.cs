using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using bachelor_work_backend.Services.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace bachelor_work_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class StagKalendarController : ControllerBase
    {
        public StagApiService StagApiService { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }
        public StagKalendarController(
            IConfiguration configuration,
            IHttpClientFactory clientFactory)
        {

            ClientFactory = clientFactory;
            Configuration = configuration;
            StagApiService = new StagApiService(configuration, clientFactory);
        }

        [HttpGet, Route("getAktualniObdobiInfo")]
        public async Task<IActionResult> Get()
        {
            var wscookie = Request.Cookies["WSCOOKIE"];

            if (string.IsNullOrEmpty(wscookie))
            {
                return Unauthorized();
            }

            var aktualniObdobiInfo = await StagApiService.StagApiKalendarService.GetAktualniObdobiInfo(wscookie);

            if(aktualniObdobiInfo == null)
            {
                return BadRequest();
            }

            return Ok(aktualniObdobiInfo);
        }
    }
}