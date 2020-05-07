using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Domain.DomainLayer.Processor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class MediaController : ControllerBase {
        private readonly ILogger<MediaController> _logger;
        private readonly EpisodeProcessor _episodeProcessor;

        public MediaController(ILogger<MediaController> logger, EpisodeProcessor episodeProcessor) {
            _logger = logger;
            _episodeProcessor = episodeProcessor;
        }

        [HttpGet]
        public async Task<IActionResult> Process(string filePath) {
            if (string.IsNullOrWhiteSpace(filePath))
                return BadRequest($"File Path supplied is empty.");

            // TODO: QUEUE!
            await _episodeProcessor.ProcessAsync(filePath);

            return Ok();
        }
    }
}