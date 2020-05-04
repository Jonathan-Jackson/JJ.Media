using MediaInfo.Domain.Helpers.DTOs.Episodes;
using MediaInfo.Domain.Helpers.DTOs.Shows;
using System.Threading.Tasks;

namespace MediaInfo.Domain.Helpers.Repository {

    public interface IEpisodeApiSearch {

        Task<Episode[]> FindEpisodeAsync(Show show, int seasonNumber, int episodeNumber);
    }
}