using JJ.Framework.Repository;
using JJ.Framework.Repository.Abstraction;
using MediaInfo.Domain.Helpers.DTOs.Episodes;
using MediaInfo.Domain.Helpers.Repository;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaInfo.Infrastructure.Repositories {

    /// <summary>
    /// Processes entity data logic for Episodes.
    /// </summary>
    public class EpisodeRepository : Repository<Episode>, IEpisodeRepository {

        public EpisodeRepository(IDbConnectionFactory dbFactory, Compiler sqlCompiler)
            : base("Episodes", dbFactory, sqlCompiler) {
        }

        /// <summary>
        /// Returns a matching episode with the showId and absolute number.
        /// </summary>
        public async Task<Episode?> FindAsync(int showId, int absoluteNumber) {
            if (showId < 0 || absoluteNumber < 0)
                return null;

            return await Execute(async (DisposableQueryFactory db)
                    => await db.Query(_tableName)
                        .Where("showId", showId)
                        .Where("AbsoluteNumber", absoluteNumber)
                        .FirstOrDefaultAsync<Episode?>()
            );
        }

        /// <summary>
        /// Returns a matching episode with the showId, season and episode number.
        /// </summary>
        public async Task<Episode?> FindAsync(int showId, int seasonNumber, int episodeNumber) {
            if (showId <= 0 || seasonNumber < 0 || episodeNumber <= 0)
                throw new ArgumentOutOfRangeException();

            return await Execute(async (DisposableQueryFactory db)
                    => await db.Query(_tableName)
                        .Where("showId", showId)
                        .Where("SeasonNumber", seasonNumber)
                        .Where("EpisodeNumber", episodeNumber)
                        .FirstOrDefaultAsync<Episode?>()
            );
        }

        /// <summary>
        /// Returns episode ids that match a show id.
        /// </summary>
        public Task<Episode[]> FindByShowAsync(int showId) {
            if (showId <= 0)
                return Task.FromResult(Array.Empty<Episode>());

            return FindByShowAsync(new[] { showId });
        }

        /// <summary>
        /// Returns episode ids that match a show id.
        /// </summary>
        public async Task<Episode[]> FindByShowAsync(IEnumerable<int> showIds) {
            if (!showIds.Any())
                return Array.Empty<Episode>();

            return (await Execute(async (DisposableQueryFactory db)
                => await db.Query(_tableName)
                    .WhereIn("showId", showIds.ToArray())
                    .GetAsync<Episode>()
                )).ToArray();
        }

        /// <summary>
        /// Inserts an episode into the repository.
        /// </summary>
        public override async Task<int> InsertAsync(Episode episode) {
            if (episode == null)
                throw new ArgumentNullException(nameof(episode));

            return await Execute(async (DisposableQueryFactory db)
                => await db.Query(_tableName).InsertGetIdAsync<int>(new {
                    episode.IsMovie,
                    episode.Overview,
                    episode.SeasonNumber,
                    episode.ShowId,
                    episode.Title,
                    episode.TvDbId,
                    episode.EpisodeNumber,
                    episode.AbsoluteNumber
                })
            );
        }

        /// <summary>
        /// Updates an episode in the repository with its current details.
        /// </summary>
        public override async Task<int> UpdateAsync(Episode episode) {
            if (episode == null)
                throw new ArgumentNullException(nameof(episode));

            return await Execute(async (DisposableQueryFactory db)
                => await db.Query(_tableName).UpdateAsync(new {
                    episode.IsMovie,
                    episode.Overview,
                    episode.SeasonNumber,
                    episode.ShowId,
                    episode.Title,
                    episode.TvDbId,
                    episode.EpisodeNumber,
                    episode.AbsoluteNumber
                })
            );
        }
    }
}