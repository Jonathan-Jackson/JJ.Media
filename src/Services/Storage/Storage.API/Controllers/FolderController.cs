using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Domain.Helpers.Abstraction;

namespace Storage.API.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class FolderController : ControllerBase {
        private readonly ILogger<FolderController> _logger;
        private readonly IShowStore _showStore;

        public FolderController(ILogger<FolderController> logger) {
            _logger = logger;
        }

        [HttpGet]
        [Route("hasShowFolder/{showName}")]
        public IActionResult HasShowFolder(string showName) {
            if (string.IsNullOrWhiteSpace(showName))
                return BadRequest("Show Name not supplied.");

            return Ok(_showStore.HasShowFolder(showName));
        }
    }
}