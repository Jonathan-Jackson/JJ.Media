using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Domain.DomainLayer.Processor;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Storage.Domain.Helpers.Exceptions;
using Storage.Domain.Helpers.DTOs;

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

        [HttpPost("process-anime")]
        public Task<IActionResult> ProcessAnime([FromBody]string filePath)
            => Process(filePath, eEpisodeType.Anime);

        [HttpPost("process-tvshow")]
        public Task<IActionResult> ProcessTvShow([FromBody]string filePath)
            => Process(filePath, eEpisodeType.Shows);

        private async Task<IActionResult> Process(string filePath, eEpisodeType episodeType) {
            _logger.LogDebug($"Media Process Request: {filePath ?? "NULL"}");

            if (string.IsNullOrWhiteSpace(filePath))
                return BadRequest($"File Path supplied is empty.");

            try {
                await _episodeProcessor.ProcessAsync(filePath, episodeType);
                return Ok();
            }
            catch (ValidationException) {
                return BadRequest($"File could not be saved.");
            }
            catch (EpisodeNotFoundException) {
                return UnprocessableEntity();
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"An error occured when processing a file: {filePath}");
                throw;
            }
        }
    }
}