using JJ.Framework.Repository;
using JJ.Framework.Repository.Abstraction;
using SqlKata.Compilers;
using SqlKata.Execution;
using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Repositories {

    public class ProcessedEpisodesRepository : Repository<ProcessedEpisode>, IProcessedEpisodeRepository {

        public ProcessedEpisodesRepository(IDbConnectionFactory dbFactory, Compiler sqlCompiler)
            : base("ProcessedEpisodes", dbFactory, sqlCompiler) {
        }

        public Task<ProcessedEpisode> FindByEpisodeAsync(int episodeId) {
            if (episodeId < 1)
                throw new ArgumentOutOfRangeException(nameof(episodeId));

            return Execute(async (DisposableQueryFactory db)
                    => await db.Query(_tableName)
                        .Where("EpisodeId", episodeId)
                        .FirstOrDefaultAsync<ProcessedEpisode>()
            );
        }

        public async Task<ProcessedEpisode[]> FindByEpisodeAsync(IEnumerable<int> episodeIds) {
            if (!episodeIds.Any())
                return Array.Empty<ProcessedEpisode>();

            return (await Execute(async (DisposableQueryFactory db)
                    => await db.Query(_tableName)
                        .WhereIn("EpisodeId", episodeIds.ToArray())
                        .GetAsync<ProcessedEpisode>()
            )).ToArray();
        }

        public Task<ProcessedEpisode> FindByGuidAsync(Guid guid) {
            if (guid == default)
                throw new ArgumentOutOfRangeException(nameof(guid));

            return Execute(async (DisposableQueryFactory db)
                    => await db.Query(_tableName)
                        .Where("Guid", guid)
                        .FirstOrDefaultAsync<ProcessedEpisode>()
            );
        }

        public override Task<int> InsertAsync(ProcessedEpisode history) {
            if (string.IsNullOrWhiteSpace(history.Output) || string.IsNullOrWhiteSpace(history.Source))
                throw new ValidationException("History output and cannot be null or empty.");

            return Execute(async (DisposableQueryFactory db)
                => await db.Query(_tableName).InsertGetIdAsync<int>(new {
                    history.Source,
                    history.Output,
                    history.ProcessedOn,
                    history.EpisodeId,
                    history.Guid
                })
            );
        }

        public override Task<int> UpdateAsync(ProcessedEpisode history) {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            return Execute(async (DisposableQueryFactory db)
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