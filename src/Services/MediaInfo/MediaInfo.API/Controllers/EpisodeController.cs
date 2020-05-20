using JJ.Framework.Controller;
using MediaInfo.Domain.Helpers.DTOs.Episodes;
using MediaInfo.Domain.Helpers.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.API.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class EpisodeController : EntityController<Episode> {
        private readonly IEpisodeRepository _episodeRepo;

        public EpisodeController(IEpisodeRepository repository)
            : base(repository) {
            _episodeRepo = repository;
        }

        [HttpGet("{episodeId}")]
        public Task<IActionResult> Get(int episodeId)
            => FindEntity(episodeId);

        [HttpGet("show/{showId}")]
        public Task<IActionResult> ShowEpisodes(int showId)
            => FindEntities(showId, _episodeRepo.FindByShowAsync);
    }
}