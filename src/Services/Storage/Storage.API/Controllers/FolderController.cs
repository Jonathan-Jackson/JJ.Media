using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Storage.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class FolderController : ControllerBase {
        private readonly ILogger<FolderController> _logger;

        public FolderController(ILogger<FolderController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<dynamic> Get() {
            return Enumerable.Empty<dynamic>();
        }

        [HttpGet]
        public IEnumerable<int> GetLatestSeasonNumber() {
            return default;
        }
    }
}