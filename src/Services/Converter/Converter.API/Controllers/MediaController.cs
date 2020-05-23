using Converter.API.Hosted;
using Converter.API.Models;
using Converter.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Converter.API.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase {
        private readonly ILogger<MediaController> _logger;
        private readonly MediaService _mediaService;
        private readonly StoreService _storeService;
        private readonly BackgroundConvertHostedService _backgroundConverter;

        public MediaController(ILogger<MediaController> logger, MediaService mediaService, BackgroundConvertHostedService backgroundConverter, StoreService storeService) {
            _logger = logger;
            _mediaService = mediaService;
            _backgroundConverter = backgroundConverter;
            _storeService = storeService;
        }

        [HttpPost("video-to-webm-await")]
        public async Task<IActionResult> VideoToWebmAwait([FromBody]string filePath) {
            if (string.IsNullOrWhiteSpace(filePath))
                return BadRequest("Path cannot be empty.");

            if (!_storeService.TryReplaceAlias(filePath, out filePath))
                return BadRequest($"Path does not start with a supported alias: {filePath}");

            await _mediaService.Convert(filePath);
            return Ok();
        }

        [HttpPost("video-to-webm")]
        public IActionResult VideoToWebm([FromBody]string filePath) {
            if (string.IsNullOrWhiteSpace(filePath))
                return BadRequest();

            if (!_storeService.TryReplaceAlias(filePath, out filePath))
                return BadRequest($"Path does not start with a supported alias: {filePath}");

            _backgroundConverter.TaskQueue.Enqueue((_) => _mediaService.Convert(filePath));
            return Ok();
        }

        [HttpPost("episode-to-webm")]
        public IActionResult EpisodeToWebm([FromBody]EpisodeFileRequest request) {
            if (string.IsNullOrWhiteSpace(request.FilePath) || request.EpisodeId < 1)
                return BadRequest();

            if (request.FilePath.EndsWith(".webm", System.StringComparison.OrdinalIgnoreCase))
                return BadRequest($"File is already a webm: {request.FilePath}");

            if (!_storeService.TryReplaceAlias(request.FilePath, out string filePath))
                return BadRequest($"Path does not start with a supported alias: {filePath}");

            _backgroundConverter.TaskQueue.Enqueue((_) => _mediaService.ConvertEpisode(filePath, request.EpisodeId));
            return Ok();
        }
    }
}