using JJ.Media.MediaInfo.Services.Interfaces;
using MediaInfo.Domain.Helpers.DTOs.Shows;
using MediaInfo.Domain.Helpers.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class ShowController : ControllerBase {
        private readonly IShowApiSearch _showApiSearch;
        private readonly IShowRepository _repository;

        public ShowController(IShowApiSearch showApiSearch, IShowRepository repository) {
            _showApiSearch = showApiSearch;
            _repository = repository;
        }

        [HttpGet("{showId}/images")]
        public async Task<IActionResult> Images(int showId)
            => await HandleRequest(showId, async (Show show) => await _showApiSearch.GetShowBannersAsync(show.TvDbId));

        [HttpGet("{showId}/api/showlink")]
        public async Task<IActionResult> ApiShowLink(int showId)
            => await HandleRequest(showId, (Show show) => _showApiSearch.GetShowLink(show.TvDbId));

        private async Task<IActionResult> HandleRequest<TResult>(int showId, Func<Show, Task<TResult>> requestAction) {
            if (showId < 1)
                return BadRequest();

            var show = await _repository.FindAsync(showId);
            if (show == null)
                return NotFound();
            else
                return Ok(await requestAction(show));
        }

        private async Task<IActionResult> HandleRequest<TResult>(int showId, Func<Show, TResult> requestAction) {
            if (showId < 1)
                return BadRequest();

            var show = await _repository.FindAsync(showId);
            if (show == null)
                return NotFound();
            else
                return Ok(requestAction(show));
        }
    }
}