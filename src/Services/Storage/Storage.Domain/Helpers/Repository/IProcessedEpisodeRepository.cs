using JJ.Media.Core.Infrastructure;
using Storage.Domain.Helpers.DTOs;
using System;
using System.Threading.Tasks;

namespace Storage.Domain.Helpers.Repository {

    public interface IProcessedEpisodeRepository : IRepository<ProcessedEpisode> {

        Task<ProcessedEpisode> FindByEpisodeAsync(int episodeId);

        Task<ProcessedEpisode> FindByGuidAsync(Guid guid);
    }
}