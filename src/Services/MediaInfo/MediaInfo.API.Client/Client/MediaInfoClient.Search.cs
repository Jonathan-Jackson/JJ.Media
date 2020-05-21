using MediaInfo.API.Client.Models;
using System.Threading.Tasks;

namespace MediaInfo.API.Client.Client {

    public partial class MediaInfoClient {

        public Task<int> ShowSearch(string fileName)
            => Find<int>($"api/search/show/{fileName}");

        public Task<EpisodeSearchResult> EpisodeSearch(string fileName)
            => Find<EpisodeSearchResult>($"api/search/episode/{fileName}");
    }
}