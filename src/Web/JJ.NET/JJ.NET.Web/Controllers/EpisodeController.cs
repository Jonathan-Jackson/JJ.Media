using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JJ.NET.Web.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class EpisodeController : ControllerBase {

        // GET: api/<EpisodeController>
        [HttpGet]
        public IEnumerable<string> Get() {
            return new string[] { "value1", "value2" };
        }

        // GET api/<EpisodeController>/5
        [HttpGet("latest/{count}")]
        public async Task<IActionResult> Latest(int count) {
            if (count > 50)
                return BadRequest("Limited to 50 per request.");

            return Ok("");
        }
    }
}