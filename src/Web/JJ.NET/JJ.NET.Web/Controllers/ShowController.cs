using JJ.Framework.Controller;
using MediaInfo.API.Client.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JJ.NET.Web.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class ShowController : ControllerBase {
        private readonly MediaInfoClient _mediaInfoClient;

        public ShowController(MediaInfoClient mediaInfoClient) {
            _mediaInfoClient = mediaInfoClient;
        }

        [HttpGet("paged")]
        public async Task<IActionResult> Paged(int index, int itemsPerPage) {
            if (index < 0)
                return BadRequest("Index cannot be negative.");
            if (itemsPerPage > 50)
                return BadRequest("Limited to 50 items per page.");

            var request = new PaginationRequest { Index = index, ItemsPerPage = itemsPerPage };
            var result = await _mediaInfoClient.GetShowsPaginated(request);

            return Ok(result);
        }

        [HttpGet("{showId}/episodes")]
        public async Task<IActionResult> Episodes(int showId) {
            if (showId < 1)
                return BadRequest("Show Id must be greater than 0.");

            var episodes = await _mediaInfoClient.GetShowEpisodes(showId);
            // TODO: update db to only get by season.
            var epViews = episodes
                .Select(ep => new { ep.Id, ep.ShowId, ep.SeasonNumber, ep.EpisodeNumber, ep.Title, Guid = Guid.NewGuid() }).ToArray();

            return Ok(episodes);
        }
    }
}