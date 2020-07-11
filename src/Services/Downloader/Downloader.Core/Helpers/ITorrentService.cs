using Downloader.Core.Helpers.DTOs;
using System.Threading.Tasks;

namespace Downloader.Core.Helpers {

    public interface ITorrentService {

        Task CompleteFinishedDownloads();

        Task CompleteUntrackedDownloads();

        Task Download(Torrent torrent);
    }
}