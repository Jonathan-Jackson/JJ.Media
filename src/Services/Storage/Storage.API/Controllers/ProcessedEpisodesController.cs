using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Repository;
using System;
using System.Threading.Tasks;

namespace Storage.API.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class ProcessedEpisodesController : EntityControllerBase<ProcessedEpisode> {
        private readonly ILogger<ProcessedEpisodesController> _logger;
        private readonly IProcessedEpisodeRepository _repository;

        public ProcessedEpisodesController(ILogger<ProcessedEpisodesController> logger, IProcessedEpisodeRepository repository)
            : base(repository) {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        [Route("episode/{episodeId}/output")]
        public async Task<IActionResult> OutputByEpisode(int episodeId)
            => await HandleEntityRequest(episodeId, _repository.FindByEpisodeAsync, (ProcessedEpisode episode) => episode.Output);

        [HttpGet]
        [Route("guid/{guid}/output")]
        public async Task<IActionResult> OutputByEpisode(Guid guid)
            => await HandleEntityRequest(guid, _repository.FindByGuidAsync, (ProcessedEpisode episode) => episode.Output);
    }
}