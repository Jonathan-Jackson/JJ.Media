using JJ.Media.MediaInfo.Services.Interfaces;
using MediaInfo.API.ViewModels;
using MediaInfo.Domain.DomainLayer.Search;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase {
        private readonly EpisodeSearch _episodeSearch;
        private readonly ShowSearch _showSearch;
        private readonly IShowRepository _showRepo;

        public SearchController(EpisodeSearch episodeSearch, ShowSearch showSearch, IShowRepository showRepo) {
            _episodeSearch = episodeSearch;
            _showSearch = showSearch;
            _showRepo = showRepo;
        }

        /// <summary>
        /// Returns an episode search result by file name.
        /// </summary>
        [HttpGet("episode/{episodeName}")]
        public async Task<IActionResult> Episode(string episodeName) {
            if (string.IsNullOrWhiteSpace(episodeName))
                return BadRequest($"Search value cannot be empty.");

            var episode = await _episodeSearch.SearchAsync(episodeName);

            if (episode == null)
                return NotFound($"No results found for: {episodeName}");
            else {
                var show = await _showRepo.FindAsync(episode.ShowId);
                return Ok(new EpisodeSearchResponse(episode, show));
            }
        }

        /// <summary>
        /// Returns a show search result by show name.
        /// </summary>
        [HttpGet("show/{showname}")]
        public async Task<IActionResult> Show(string showName) {
            if (string.IsNullOrWhiteSpace(showName))
                return BadRequest($"Search value cannot be empty.");

            var result = await _showSearch.SearchAsync(showName);

            if (result == null)
                return NotFound($"No results found for: {showName}");
            else
                return Ok(result);
        }
    }
}