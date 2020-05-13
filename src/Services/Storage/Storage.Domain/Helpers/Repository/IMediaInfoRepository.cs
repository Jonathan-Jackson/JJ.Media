using Storage.Domain.Helpers.DTOs;
using System.Threading.Tasks;

namespace Storage.Domain.Helpers.Repository {

    public interface IMediaInfoRepository {

        Task<EpisodeSearch> SearchEpisode(string episodeFileName);

        Task<string[]> GetShowImages(int showId);

        Task<string> GetShowRemoteLink(int showId);

        Task<string> GetShowOverview(int showId);
    }
}