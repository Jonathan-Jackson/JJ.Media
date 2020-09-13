using Downloader.Core.Helpers.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Downloader.Core.Helpers {

    public interface ITorrentClient {

        Task<bool> TryAuth();

        Task TryDeleteAsync(IEnumerable<string> hashes);

        Task DownloadAsync(string magnet);

        Task<IEnumerable<QBitTorrent>> GetCompletedTorrentsAsync();

        Task<IEnumerable<QBitTorrent>> GetTorrentsAsync();
    }
}