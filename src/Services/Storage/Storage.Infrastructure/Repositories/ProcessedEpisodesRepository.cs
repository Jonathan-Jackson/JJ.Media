using JJ.Media.Core.Infrastructure;
using SqlKata.Compilers;
using SqlKata.Execution;
using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Repository;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Repositories {

    public class ProcessedEpisodesRepository : Repository<ProcessedEpisode>, IProcessedEpisodeRepository {

        public ProcessedEpisodesRepository(IDbConnectionFactory dbFactory, Compiler sqlCompiler)
            : base("ProcessedEpisodes", dbFactory, sqlCompiler) {
        }

        public async Task<ProcessedEpisode> FindByEpisodeAsync(int episodeId) {
            if (episodeId < 1)
                throw new ArgumentOutOfRangeException(nameof(episodeId));

            return await Execute(async (DisposableQuery db)
                    => await db.Query(_tableName)
                        .Where("Id", episodeId)
                        .FirstOrDefaultAsync<ProcessedEpisode>()
            );
        }

        public async Task<ProcessedEpisode> FindByGuidAsync(Guid guid) {
            if (guid == default)
                throw new ArgumentOutOfRangeException(nameof(guid));

            return await Execute(async (DisposableQuery db)
                    => await db.Query(_tableName)
                        .Where("Guid", guid)
                        .FirstOrDefaultAsync<ProcessedEpisode>()
            );
        }

        public async override Task<int> InsertAsync(ProcessedEpisode history) {
            if (string.IsNullOrWhiteSpace(history.Output) || string.IsNullOrWhiteSpace(history.Source))
                throw new ValidationException("History output and cannot be null or empty.");

            return await Execute(async (DisposableQuery db)
                => await db.Query(_tableName).InsertGetIdAsync<int>(new {
                    history.Source,
                    history.Output,
                    history.ProcessedOn,
                    history.EpisodeId,
                    history.Guid
                })
            );
        }

        public async override Task<int> UpdateAsync(ProcessedEpisode history) {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            return await Execute(async (DisposableQuery db)
                => await db.Query(_tableName).UpdateAsync(new {
                    history.Source,
                    history.Output,
                    history.ProcessedOn,
                    history.EpisodeId
                })
            );
        }
    }
}