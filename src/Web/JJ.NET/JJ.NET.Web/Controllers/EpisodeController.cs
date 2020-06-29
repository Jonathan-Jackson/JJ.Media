using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaInfo.API.Client.Client;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Client.Client;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JJ.NET.Web.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class EpisodeController : ControllerBase {
        private StorageClient _storageClient;
        private MediaInfoClient _mediaInfoClient;

        // GET api/<EpisodeController>/5
        [HttpGet("latest/{count}")]
        public async Task<IActionResult> Latest(int count) {
            if (count > 50)
                return BadRequest("Limited to 50 per request.");

            // NOTE:
            // Lets update MediaInfo to supply us with a ViewableEpisode
            // which contains all of the below information.
            var latest = await _storageClient.GetLatestProcessedEpisodes(count);
            var episodes = await _mediaInfoClient.GetEpisode(latest.Select(x => x.EpisodeId));

            var latestEpisodes = latest.ToDictionary(x => x, x => episodes.First(ep => ep.Id == x.EpisodeId));
            var viewModels = latestEpisodes.Select(x => new {
                x.Value.Id,
                x.Value.Title,
                x.Key.Guid,
                x.Value.SeasonNumber,
                x.Value.EpisodeNumber,
                x.Key.ProcessedOn,
                Wallpaper = "https://cdn.wallpapersafari.com/32/11/1B9LhF.jpg"
            });

            return Ok(viewModels);
        }
    }
}