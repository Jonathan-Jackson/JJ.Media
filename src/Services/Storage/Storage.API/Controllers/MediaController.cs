using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class MediaController : ControllerBase {
        private readonly ILogger<MediaController> _logger;

        public MediaController(ILogger<MediaController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<dynamic> Get() {
            return Enumerable.Empty<dynamic>();
        }

        [HttpGet]
        public async Task<IActionResult> Process(string filePath) {
            if (string.IsNullOrWhiteSpace(filePath))
                return BadRequest($"File Path supplied is empty.");

            return Ok();
        }
    }
}