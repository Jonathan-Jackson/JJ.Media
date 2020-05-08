using Downloader.Core.Helpers;
using Downloader.Core.Helpers.DTOs;
using Downloader.Core.Helpers.Options;
using Downloader.Core.Infrastructure;
using JJ.Media.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Downloader.Core.Services {

    public class TorrentService {
        private readonly ILogger<TorrentService> _log;
        private readonly ITorrentClient _torrentClient;
        private readonly StorageServiceNotifier _storage;
        private readonly string _downloadTorrentPath;

        public TorrentService(ILogger<TorrentService> log, ITorrentClient torrentClient, StorageServiceNotifier storage, TorrentServiceOptions options) {
            _log = log;
            _torrentClient = torrentClient;
            _storage = storage;
            _downloadTorrentPath = options.DownloadTorrentPath;
        }

        /// <summary>
        /// Removes finished torrents in the torrent service and
        /// notifies the media processor that files are ready to process.
        /// </summary>
        public async Task CompleteFinishedDownloads() {
            IEnumerable<QBitTorrent> completeTorrents = await _torrentClient.GetCompletedTorrentsAsync();

            if (completeTorrents.Any()) {
                await _torrentClient.DeleteAsync(completeTorrents.Select(x => x.Hash));
                // Allow a pause for file handler / lock release.
                await Task.Delay(1000);

                await NotifyToProcessFiles(completeTorrents.Select(x => x.Name));
            }
        }

        /// <summary>
        /// Notifies the media processor that files are ready to process
        /// which are not being tracked by the torrent service.
        /// </summary>
        public async Task CompleteUntrackedDownloads() {
            IEnumerable<DirectoryInfo> files = Directory.GetFiles(_downloadTorrentPath).Select(x => new DirectoryInfo(x));

            IEnumerable<string> torrentNames = (await _torrentClient.GetTorrentsAsync()).Select(x => x.Name.ToLower());
            var untracked = files.Where(x => !torrentNames.Contains(x.Name.ToLower()));

            if (untracked.Any()) {
                await NotifyToProcessFiles(untracked.Select(x => x.Name));
            }
        }

        /// <summary>
        /// Notifies the torrent service to download
        /// a torrent.
        /// </summary>
        /// <param name="torrent">Torrent to download.</param>
        public async Task Download(Torrent torrent) {
            try {
                await _torrentClient.DownloadAsync(torrent.MagnetUri);
                _log.LogInformation($"Downloaded Torrent: {@torrent.Title}", torrent);
            }
            catch (Exception ex) {
                _log.LogError(ex, $"Failed to download {@torrent.Title} : {@torrent}", torrent);
                throw;
            }

            // Audit DL
        }

        /// <summary>
        /// Notifies our end point of files that require processing.
        /// </summary>
        private async Task NotifyToProcessFiles(IEnumerable<string> fileNames) {
            foreach (string fileName in fileNames) {
                try {
                    var notification = new Notification<string>($"{fileName}");
                    await _storage.Notify(notification);
                    _log.LogInformation($"Notified Processor of: {fileName}");
                }
                catch (Exception ex) {
                    _log.LogError(ex, $"Failed to notify of completion for torrent: {fileName}");
                }
            }
        }
    }
}