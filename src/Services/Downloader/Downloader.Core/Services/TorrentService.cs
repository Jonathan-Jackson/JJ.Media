using Downloader.Core.Helpers;
using Downloader.Core.Helpers.DTOs;
using Downloader.Core.Helpers.Options;
using JJ.Framework.Repository.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Downloader.Core.Services {

    public class TorrentService : ITorrentService {
        private readonly ILogger<TorrentService> _log;
        private readonly IMessageBroker _broker;
        private readonly ITorrentClient _torrentClient;
        private readonly string _downloadTorrentPath;

        private const string BrokerExchange = "DownloadedMedia";

        public TorrentService(ILogger<TorrentService> log, ITorrentClient torrentClient, TorrentServiceOptions options, IMessageBroker broker) {
            _log = log;
            _torrentClient = torrentClient;
            _downloadTorrentPath = options.DownloadTorrentPath;
            _broker = broker;
            _broker.DeclareExchange(BrokerExchange);
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

                TryNotifyToProcessFiles(completeTorrents.Select(x => x.Name));
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
            TryNotifyToProcessFiles(untracked.Select(x => x.Name));
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
        private void TryNotifyToProcessFiles(IEnumerable<string> fileNames) {
            if (!fileNames.Any())
                return;

            try {
                _broker.Publish("FilePath", fileNames.ToArray(), BrokerExchange);
            }
            catch (Exception ex) {
                _log.LogError(ex, $"Failed to notify of completion for torrents: {string.Join(", ", fileNames)}");
            }
        }
    }
}