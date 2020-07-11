using Downloader.Core.Helpers.DTOs;
using JJ.Framework.Repository.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Downloader.Core.Infrastructure {

    public interface IHistoryRepository : IRepository<DownloadHistory> {

        Task<bool> AnyAsync(string title);

        new Task<int> InsertAsync(DownloadHistory history);

        new Task<IEnumerable<int>> InsertAsync(IEnumerable<DownloadHistory> history);

        new Task<int> UpdateAsync(DownloadHistory history);

        new Task<IEnumerable<int>> UpdateAsync(IEnumerable<DownloadHistory> history);
    }
}