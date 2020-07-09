using Discord.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Discord.API.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase {
        private readonly ILogger<AlertController> _logger;
        private readonly EpisodeAlertService _episodeAlertService;
        private readonly DiscordBackgroundService _backgroundService;

        public AlertController(ILogger<AlertController> logger, EpisodeAlertService episodeAlertService, DiscordBackgroundService backgroundService) {
            _episodeAlertService = episodeAlertService;
            _logger = logger;
            _backgroundService = backgroundService;
        }

        [HttpPost("episode")]
        public IActionResult Episode([FromBody] int episodeId) {
            if (episodeId < 1)
                return BadRequest();

            _backgroundService.TaskQueue.Enqueue((discord, _) => _episodeAlertService.Alert(discord, episodeId));
            return Ok();
        }
    }
}