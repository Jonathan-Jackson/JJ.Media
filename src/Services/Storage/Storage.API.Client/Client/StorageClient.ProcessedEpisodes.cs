using JJ.Framework.Client;
using Storage.API.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.API.Client.Client {

    public partial class StorageClient : ApiClient {

        public Task<ProcessedEpisode> GetProcessedEpisode(int id)
            => Get<ProcessedEpisode>($"/api/processedepisodes/{id}");

        public Task<ProcessedEpisode[]> GetLatestProcessedEpisodes(int count)
            => Get<ProcessedEpisode[]>($"/api/processedepisodes/latest?count={count}");

        public Task AddProcessedEpisode(ProcessedEpisode processedEpisode)
            => Post($"/api/processedepisodes", processedEpisode);

        public Task<string> GetOutputByEpisode(int episodeId)
            => Get<string>($"/api/processedepisodes/episode/{episodeId}/output");

        public Task<string> GetOutputByGuid(Guid guid)
            => Get<string>($"/api/processedepisodes/guid/{guid}/output");

        public Task<string> FindOutputByGuid(Guid guid)
            => Find<string>($"/api/processedepisodes/guid/{guid}/output");

        public Task<EpisodeGuid[]> GetGuidByEpisode(IEnumerable<int> episodeIds)
            => episodeIds.Any()
                ? Post<EpisodeGuid[]>($"/api/processedepisodes/guid/episode", episodeIds.ToArray())
                : Task.FromResult(Array.Empty<EpisodeGuid>());

        public Task<Guid> GetGuidByEpisode(int episodeId)
            => Get<Guid>($"/api/processedepisodes/guid/episode/{episodeId}");
    }
}