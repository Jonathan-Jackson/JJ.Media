using JJ.Framework.Controller;
using JJ.Framework.Repository;
using MediaInfo.API.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaInfo.API.Client.Client {
    public interface IMediaInfoClient {
        Task<EpisodeSearchResult> EpisodeSearch(string fileName);
        Task<Episode[]> GetEpisode(IEnumerable<int> episodeIds);
        Task<Episode> GetEpisode(int episodeId);
        Task<Show> GetShow(int showId);
        Task<Episode[]> GetShowEpisodes(int showId);
        Task<string> GetShowExternalLink(int showId);
        Task<string[]> GetShowImages(int showId);
        Task<string> GetShowOverview(int showId);
        Task<Pagination<Show>> GetShowsPaginated(PaginationRequest request);
        Task<int> ShowSearch(string fileName);
    }
}