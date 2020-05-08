using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Domain.DomainLayer.Processor;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Storage.API.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase {
        private readonly ILogger<MediaController> _logger;
        private readonly EpisodeProcessor _episodeProcessor;

        public MediaController(ILogger<MediaController> logger, EpisodeProcessor episodeProcessor) {
            _logger = logger;
            _episodeProcessor = episodeProcessor;
        }

        [HttpGet("process")]
        public async Task<IActionResult> Process([FromBody]string filePath) {
            if (string.IsNullOrWhiteSpace(filePath))
                return BadRequest($"File Path supplied is empty.");

            try {
                // TODO: QUEUE!
                await _episodeProcessor.ProcessAsync(filePath);

                return Ok();
            }
            catch (ValidationException validation) {
                return BadRequest(validation.Message);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occured when processing a file.");
                throw;
            }
        }
    }
}