using JJ.Media.Core.Infrastructure;
using SqlKata.Compilers;
using SqlKata.Execution;
using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Repository;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Repositories {

    public class ProcessedRepository : Repository<ProcessedHistory>, IProcessedRepository {

        public ProcessedRepository(IDbConnectionFactory dbFactory, Compiler sqlCompiler)
            : base("ProcessedHistory", dbFactory, sqlCompiler) {
        }

        public async override Task<int> InsertAsync(ProcessedHistory history) {
            if (string.IsNullOrWhiteSpace(history.Output) || string.IsNullOrWhiteSpace(history.Source))
                throw new ValidationException("History output and cannot be null or empty.");

            return await Execute(async (DisposableQuery db)
                => await db.Query(_tableName).InsertGetIdAsync<int>(new {
                    history.Source,
                    history.Output,
                    history.ProcessedOn,
                    history.Type
                })
            );
        }

        public async override Task<int> UpdateAsync(ProcessedHistory history) {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            return await Execute(async (DisposableQuery db)
                => await db.Query(_tableName).UpdateAsync(new {
                    history.Source,
                    history.Output,
                    history.ProcessedOn,
                    history.Type
                })
            );
        }
    }
}