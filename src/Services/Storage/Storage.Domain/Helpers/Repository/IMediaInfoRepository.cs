using Storage.Domain.Helpers.DTOs;
using System.Threading.Tasks;

namespace Storage.Domain.Helpers.Repository {

    public interface IMediaInfoRepository {

        Task<EpisodeSearch> SearchEpisode(string episodeFileName);
    }
}