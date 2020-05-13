using JJ.Media.MediaInfo.Services.Interfaces;
using MediaInfo.Domain.Helpers.DTOs.Shows;
using MediaInfo.Domain.Helpers.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class ShowController : EntityControllerBase<Show> {
        private readonly IShowApiSearch _showApiSearch;

        public ShowController(IShowApiSearch showApiSearch, IShowRepository repository)
            : base(repository) {
            _showApiSearch = showApiSearch;
        }

        [HttpGet("{showId}")]
        public async Task<IActionResult> Get(int showId)
            => await HandleEntityRequest(showId, (Show show) => show);

        [HttpGet("{showId}/images")]
        public async Task<IActionResult> Images(int showId)
            => await HandleEntityRequest(showId, async (Show show) => await _showApiSearch.GetShowBannersAsync(show.TvDbId));

        [HttpGet("{showId}/api/showlink")]
        public async Task<IActionResult> ApiShowLink(int showId)
            => await HandleEntityRequest(showId, (Show show) => _showApiSearch.GetShowLink(show.TvDbId));

        [HttpGet("{showId}/overview")]
        public async Task<IActionResult> Overview(int showId)
            => await HandleEntityRequest(showId, (Show show) => show.Overview);
    }
}