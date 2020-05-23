using JJ.Framework.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.API.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class ProcessedEpisodesController : EntityController<ProcessedEpisode> {
        private readonly ILogger<ProcessedEpisodesController> _logger;
        private readonly IProcessedEpisodeRepository _repository;

        public ProcessedEpisodesController(ILogger<ProcessedEpisodesController> logger, IProcessedEpisodeRepository repository)
            : base(repository) {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet("{id}")]
        public Task<IActionResult> Get(int id)
            => FindEntity(id);

        [HttpPost]
        public Task<IActionResult> Post([FromBody]ProcessedEpisode episode)
            => AddEntity(episode);

        [HttpPost("guid/episode")]
        public async Task<IActionResult> GuidByEpisode([FromBody]int[] episodeIds) {
            if (episodeIds?.Any() != true)
                return BadRequest("Episode Ids must be supplied within the body.");

            var processed = await _repository.FindByEpisodeAsync(episodeIds);
            var result = processed.Select(x => new { x.Guid, x.EpisodeId }).ToArray();
            return Ok(result);
        }

        [HttpGet("guid/episode/{episodeId}")]
        public Task<IActionResult> GuidByEpisode(int episodeId)
            => FindEntity(episodeId, _repository.FindByEpisodeAsync, (ProcessedEpisode episode) => episode.Guid);

        [HttpGet("episode/{episodeId}/output")]
        public Task<IActionResult> OutputByEpisode(int episodeId)
            => FindEntity(episodeId, _repository.FindByEpisodeAsync, (ProcessedEpisode episode) => episode.Output);

        [HttpGet("guid/{guid}/output")]
        public Task<IActionResult> OutputByGuid(Guid guid)
            => FindEntity(guid, _repository.FindByGuidAsync, (ProcessedEpisode episode) => episode.Output);
    }
}