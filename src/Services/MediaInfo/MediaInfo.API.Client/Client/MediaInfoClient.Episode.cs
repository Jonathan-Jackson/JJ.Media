using MediaInfo.API.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaInfo.API.Client.Client {

    public partial class MediaInfoClient {

        public Task<Episode> GetEpisode(int episodeId)
            => Get<Episode>($"api/episode/{episodeId}");

        public Task<Episode[]> GetEpisode(IEnumerable<int> episodeIds)
            => Post<Episode[]>($"api/episode/bulk", episodeIds);

        public Task<Episode[]> GetShowEpisodes(int showId)
            => Get<Episode[]>($"api/episode/show/{showId}");
    }
}