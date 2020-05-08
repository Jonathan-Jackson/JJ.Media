using Downloader.Core.Helpers.DTOs;
using JJ.Media.Core.Infrastructure;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Downloader.Core.Infrastructure {

    public class HistoryRepository : Repository<DownloadHistory> {

        public HistoryRepository(IDbConnectionFactory dbFactory, Compiler sqlCompiler)
            : base("DownloadHistory", dbFactory, sqlCompiler) {
        }

        public async Task<bool> AnyAsync(string title) {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("The title supplied is empty.");

            return await Execute(async (DisposableQuery db)
                    => await db.Query(_tableName)
                        .Where("Title", title)
                        .CountAsync<int>("*") > 0
            );
        }

        public override async Task<int> InsertAsync(DownloadHistory history) {
            if (string.IsNullOrWhiteSpace(history.Title) || string.IsNullOrWhiteSpace(history.MagnetUri))
                throw new ValidationException(JsonSerializer.Serialize(history));

            return await Execute(async (DisposableQuery db)
                    => await db.Query(_tableName).InsertAsync(new {
                        history.MagnetUri,
                        history.Title,
                        history.DownloadedOn
                    })
            );
        }

        public override async Task<IEnumerable<int>> InsertAsync(IEnumerable<DownloadHistory> history) {
            return await Task.WhenAll(history.Select(InsertAsync));
        }

        public override async Task<int> UpdateAsync(DownloadHistory history) {
            if (string.IsNullOrWhiteSpace(history.Title) || string.IsNullOrWhiteSpace(history.MagnetUri))
                throw new ValidationException(JsonSerializer.Serialize(history));
            if (history.Id <= 0)
                throw new ValidationException($"History does not exist: {JsonSerializer.Serialize(history)}");

            return await Execute(async (DisposableQuery db)
                    => await db.Query(_tableName).UpdateAsync(new {
                        history.MagnetUri,
                        history.Title,
                        history.DownloadedOn
                    })
            );
        }

        public override async Task<IEnumerable<int>> UpdateAsync(IEnumerable<DownloadHistory> history) {
            return await Task.WhenAll(history.Select(UpdateAsync));
        }
    }
}