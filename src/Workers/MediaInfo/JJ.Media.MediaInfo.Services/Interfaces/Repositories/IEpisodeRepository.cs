using JJ.Media.MediaInfo.Core.Entities.Episodes;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Services.Interfaces {

    public interface IEpisodeRepository : IRepository<Episode> {

        Task<Episode?> FindAsync(int showId, uint seasonNumber, uint episodeNumber);
    }
}