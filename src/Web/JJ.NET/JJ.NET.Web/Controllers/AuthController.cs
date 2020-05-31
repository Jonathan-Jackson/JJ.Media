using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JJ.NET.Web.Controllers {

    public class AuthController : ControllerBase {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger) {
            _logger = logger;
        }

        public IActionResult Discord([FromBody] string code) {
            return Ok();
        }
    }
}