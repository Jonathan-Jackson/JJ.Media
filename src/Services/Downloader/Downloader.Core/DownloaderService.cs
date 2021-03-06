﻿using Downloader.Core.Feeds;
using Downloader.Core.Helpers;
using Downloader.Core.Helpers.DTOs;
using Downloader.Core.Infrastructure;
using JJ.Framework.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Downloader.Core.Services {

    public class DownloaderService {
        private readonly IReadOnlyCollection<IFeed> _feeds;
        private readonly ILogger<DownloaderService> _log;
        private readonly ITorrentService _torrentService;
        private readonly ITorrentClient _torrentClient;
        private readonly IHistoryRepository _historyRepo;

        private const int GetFeedIntervalMiliSeconds = 3_600_000;           // 60 minutes.
        private const int ProcessUntrackedIntervalMiliSeconds = 300_200;    // 5 minutes.
        private const int ProcessTrackedIntervalMiliseconds = 15_000;       // 15 seconds.
        private const int UntrackedDelayIntervalMiliseconds = 126_000;      // 2.1 minutes.

        public DownloaderService(IReadOnlyCollection<IFeed> feeds, ILogger<DownloaderService> log,
                TorrentService torrentService, HistoryRepository historyRepo, ITorrentClient torrentClient) {
            _feeds = feeds;
            _log = log;
            _torrentService = torrentService;
            _historyRepo = historyRepo;
            _torrentClient = torrentClient;
        }

        /// <summary>
        /// Runs the service. This executes
        /// processing downloads via the feed, picking up
        /// untracked files and monitors completions.
        /// </summary>
        public async Task Run() {
            _log.LogInformation("Downloader Service Ran..");
            if (!await TryCheckDependencies())
                await Task.Delay(60_000);

            // We should delay immediate checking for untracked downloads,
            // since processing completions may trigger duplicate messages.
            // its not the end of the world, since in theory duplicate messages wont
            // break things - but its better safe!
            Func<Task> delayedProcessUntrackedDownloadsAsync = async () => {
                await Task.Delay(UntrackedDelayIntervalMiliseconds);
                _log.LogInformation($"Start delay of {UntrackedDelayIntervalMiliseconds / 1000} seconds has passed, processing untracked downloads.");
                await ProcessUntrackedDownloadsAsync();
            };

            while (true) {
                try {
                    _log.LogInformation("Awaiting Event..");
                    await Task.WhenAll(
                       ProcessCompletionsAsync(),
                       ProcessFeedAsync(),
                       delayedProcessUntrackedDownloadsAsync()
                    );
                }
                catch (Exception ex) {
                    _log.LogError(ex, "Exception thrown when executing the main program");
                }

                await Task.Delay(5_000);
            }
        }

        /// <summary>
        /// Checks that dependency calls don't throw
        /// exceptions. This will simply log.
        /// </summary>
        private async Task<bool> TryCheckDependencies() {
            _log.LogInformation("Trying services..");
            bool isError = false;

            try {
                foreach (var feed in _feeds) {
                    await feed.ReadAsync();
                }
            }
            catch (Exception ex) {
                _log.LogError(ex, "Failed to load feeds.");
                isError = true;
            }

            try {
                await _historyRepo.FindAsync(1);
            }
            catch (Exception ex) {
                _log.LogError(ex, "Failed to use repository (history - likely database).");
                isError = true;
            }

            try {
                if (!await _torrentClient.TryAuth()) {
                    _log.LogError("Failed to authorize with Torrent client.");
                    isError = true;
                }
            }
            catch (Exception ex) {
                _log.LogError(ex, "Failed to call the torrent client.");
                isError = true;
            }

            _log.LogInformation($"Service Status Check: " + (isError ? "FAILURE" : "SUCCESS"));
            return !isError;
        }

        /// <summary>
        /// Scans for any complete torrents and removes/notifies processor of them.
        /// </summary>
        private async Task ProcessCompletionsAsync() {
            for (; ; await Task.Delay(ProcessTrackedIntervalMiliseconds)) {
                try {
                    await _torrentService.CompleteFinishedDownloads();
                    _log.LogDebug($"Processed untracked downloads. Next process in {(ProcessTrackedIntervalMiliseconds / 1000)} seconds.");
                }
                catch (Exception ex) {
                    _log.LogError(ex, "A fatal error was thrown while completing finished downloads!");
                }
            }
        }

        /// <summary>
        /// Scans for untracked downloads
        /// (not within the torrent client, but the file exists).
        /// </summary>
        private async Task ProcessUntrackedDownloadsAsync() {
            for (; ; await Task.Delay(ProcessUntrackedIntervalMiliSeconds)) {
                try {
                    await _torrentService.CompleteUntrackedDownloads();
                    _log.LogDebug($"Processed untracked downloads. Next process in {(ProcessUntrackedIntervalMiliSeconds / 1000) / 60} minutes.");
                }
                catch (Exception ex) {
                    _log.LogError(ex, "A fatal error was thrown while completing finished downloads!");
                }
            }
        }

        /// <summary>
        /// Scans the feed for new items to download,
        /// notifies the torrent service if there are any.
        /// </summary>
        private async Task ProcessFeedAsync() {
            for (; ; await Task.Delay(GetFeedIntervalMiliSeconds)) {
                try {
                    IList<Torrent> torrents = await GetFeedsAsync();

                    await foreach (var torrent in torrents.WhereAsync(IsTorrentEligibleToDownloadAsync)) {
                        await _torrentService.Download(torrent);
                        await _historyRepo.InsertAsync(new DownloadHistory { Title = torrent.Title, MagnetUri = torrent.MagnetUri });
                    }

                    _log.LogDebug($"Processed torrent feed. Next process in {(GetFeedIntervalMiliSeconds / 1000) / 60} minutes.");
                }
                catch (Exception ex) {
                    _log.LogError(ex, "A fatal error was thrown while processing the feed!");
                }
            }
        }

        /// <summary>
        /// Returns all feed torrents.
        /// </summary>
        private async Task<IList<Torrent>> GetFeedsAsync() {
            var torrents = await Task.WhenAll(_feeds.Select(x => x.ReadAsync()));
            return torrents.SelectMany(x => x).ToList();
        }

        /// <summary>
        /// Returns true if a torrent is eligible to download.
        /// </summary>
        private async Task<bool> IsTorrentEligibleToDownloadAsync(Torrent torrent) {
            try {
                bool isEligible = !await _historyRepo.AnyAsync(torrent.Title);
                _log.LogDebug($"{torrent.Title} - Eligible for download: {isEligible}");

                return isEligible;
            }
            catch (Exception ex) {
                _log.LogError(ex, $"Unable to get eligiblity for download due to error. Torrent title: {torrent.Title}");
                return false;
            }
        }
    }
}