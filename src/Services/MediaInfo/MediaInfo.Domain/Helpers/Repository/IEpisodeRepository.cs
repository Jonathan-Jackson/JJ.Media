using MediaInfo.Domain.Helpers.DTOs.Episodes;
using System.Threading.Tasks;

namespace MediaInfo.Domain.Helpers.Repository {

    public interface IEpisodeRepository : IRepository<Episode> {

        Task<Episode?> FindAsync(int showId, int seasonNumber, int episodeNumber);

        Task<Episode?> FindAsync(int showId, int absoluteNumber);
    }
}