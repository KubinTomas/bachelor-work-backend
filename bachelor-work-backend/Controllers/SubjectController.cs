using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bachelor_work_backend.DTO.subject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bachelor_work_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SubjectController : ControllerBase
    {
        public SubjectController()
        {

        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        }

        [HttpPost, Route("create")]
        public async Task<IActionResult> Create(SubjectDTO subjectDTO)
        {

       
            return Ok();
        }

    }
}