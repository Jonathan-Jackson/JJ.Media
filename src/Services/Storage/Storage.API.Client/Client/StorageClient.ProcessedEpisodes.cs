using JJ.Framework.Client;
using Storage.API.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Storage.API.Client.Client {

    public partial class StorageClient : ApiClient {

        public Task<string> GetOutputByEpisode(int episodeId)
            => Get<string>($"/api/processedepisodes/episode/{episodeId}/output");

        public Task<string> GetOutputByGuid(Guid guid)
            => Get<string>($"/api/processedepisodes/guid/{guid}/output");

        public Task<EpisodeGuid[]> GetGuidByEpisode(IEnumerable<int> episodeIds)
            => Post<EpisodeGuid[]>($"/api/processedepisodes/guid/episode", episodeIds.ToArray());

        public Task<Guid> GetGuidByEpisode(int episodeId)
            => Get<Guid>($"/api/processedepisodes/guid/episode/{episodeId}");
    }
}