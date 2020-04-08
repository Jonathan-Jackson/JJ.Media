using JJ.Media.MediaInfo.Core.Entities.Episodes;
using JJ.Media.MediaInfo.Data.Interfaces;
using JJ.Media.MediaInfo.Data.Models;
using JJ.Media.MediaInfo.Services.Interfaces;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Data.Repositories {

    public class EpisodeRepository : Repository<Episode>, IEpisodeRepository {

        public EpisodeRepository(IDbConnectionFactory dbFactory, Compiler sqlCompiler)
            : base("Episodes", dbFactory, sqlCompiler) {
        }

        public async Task<Episode?> FindAsync(int showId, uint seasonNumber, uint episodeNumber) {
            return await Execute(async (DisposableQuery db)
                    => await db.Query(_tableName)
                        .Where("showId", showId)
                        .Where("SeasonNumber", seasonNumber)
                        .Where("EpisodeNumber", episodeNumber)
                        .FirstOrDefaultAsync<Episode?>()
            );
        }
    }
}