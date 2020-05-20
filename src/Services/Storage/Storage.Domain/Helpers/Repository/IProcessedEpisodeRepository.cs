using JJ.Framework.Repository.Abstraction;
using Storage.Domain.Helpers.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Domain.Helpers.Repository {

    public interface IProcessedEpisodeRepository : IRepository<ProcessedEpisode> {

        Task<ProcessedEpisode[]> FindByEpisodeAsync(IEnumerable<int> episodeIds);

        Task<ProcessedEpisode> FindByEpisodeAsync(int episodeId);

        Task<ProcessedEpisode> FindByGuidAsync(Guid guid);
    }
}