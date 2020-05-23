using Discord.API.Models.Options;
using DSharpPlus;
using DSharpPlus.Entities;
using JJ.Framework.Extensions;
using JJ.Framework.Helpers;
using MediaInfo.API.Client.Client;
using MediaInfo.API.Client.Models;
using Storage.API.Client.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.API.Services {

    public class EpisodeAlertService : DiscordService {
        private readonly MediaInfoClient _mediaInfoClient;
        private readonly StorageClient _storageClient;

        public EpisodeAlertService(MediaInfoClient mediaInfoClient, DiscordOptions options, StorageClient storageClient)
            : base(options) {
            _mediaInfoClient = mediaInfoClient;
            _storageClient = storageClient;
        }

        /// <summary>
        /// Posts an alert into discord regarding an episode.
        /// </summary>
        public async Task Alert(DiscordClient discord, int episodeId) {
            var channelTask = GetConfiguredChannels(discord);

            var guidTask = _storageClient.GetGuidByEpisode(episodeId);
            var episode = await _mediaInfoClient.GetEpisode(episodeId);
            var showTask = _mediaInfoClient.GetShow(episode.ShowId);

            var message = await CreateMessage(await showTask, episode, await guidTask);

            foreach (var channel in await channelTask)
                await channel.SendMessageAsync(embed: message);
        }

        private async Task<string> CreateDescription(Show show, Episode episode, Guid fileGuid) {
            var infoLinkTask = _mediaInfoClient.GetShowExternalLink(episode.ShowId);
            var overviewTask = _mediaInfoClient.GetShowOverview(episode.ShowId);

            string seasonEpisodeHeader = Formatter.Bold($"Season {episode.SeasonNumber} Episode {episode.EpisodeNumber}");
            string episodeDescription = IsDefaultedTitle(episode.Title) ? string.Empty : episode.Title;
            string viewUrl = $"@ {_viewerDomain}/{fileGuid.ToString()}";
            string altShowTitles = show.Titles.Count > 1
                ? Formatter.Italic(string.Join(", ", GetAltTitles(show)))
                : string.Empty;
            string overview = FormatOverview(await overviewTask);
            string infoUrl = await infoLinkTask;

            // This is quite confusing ~ How do we simplify this?
            if (string.IsNullOrWhiteSpace(episodeDescription) && string.IsNullOrWhiteSpace(altShowTitles))
                return string.Join("\r\n", seasonEpisodeHeader, viewUrl, "", overview, "", infoUrl);
            else if (string.IsNullOrWhiteSpace(episodeDescription) && !string.IsNullOrWhiteSpace(altShowTitles))
                return string.Join("\r\n", seasonEpisodeHeader, viewUrl, "", altShowTitles, "", overview, "", infoUrl);
            else if (!string.IsNullOrWhiteSpace(episodeDescription) && !string.IsNullOrWhiteSpace(altShowTitles))
                return string.Join("\r\n", seasonEpisodeHeader, episodeDescription, viewUrl, "", altShowTitles, "", overview, "", infoUrl);
            else
                return string.Join("\r\n", seasonEpisodeHeader, episodeDescription, viewUrl, "", overview, "", infoUrl);
        }

        private string FormatOverview(string overview) {
            if (overview.Length > 301)
                return $"{overview.Substring(0, 300)}...";
            else
                return overview;
        }

        private async Task<DiscordEmbedBuilder> CreateMessage(Show show, Episode episode, Guid fileGuid) {
            var descriptionTask = CreateDescription(show, episode, fileGuid);
            var bannerTask = GetRandomShowBanner(episode.ShowId);

            return new DiscordEmbedBuilder {
                Title = show.Titles.FirstOrDefault(x => x.IsPrimary)?.Title ?? show.Titles.FirstOrDefault()?.Title ?? "No Title",
                Description = await descriptionTask,
                Color = DiscordColor.CornflowerBlue,
                ImageUrl = await bannerTask
            };
        }

        private IEnumerable<string> GetAltTitles(Show show) {
            string primaryTitle = show.Titles.FirstOrDefault(x => x.IsPrimary)?.Title;

            if (!string.IsNullOrWhiteSpace(primaryTitle))
                return show.Titles.Where(x => !x.IsPrimary)
                    .Select(x => x.Title)
                    .OrderByDescending(title => string.Compare(title, primaryTitle, StringComparison.OrdinalIgnoreCase))
                    .Take(5);
            else {
                return show.Titles.Skip(1)
                    .Select(x => x.Title)
                    .Take(5);
            }
        }

        private async Task<string> GetRandomShowBanner(int showId) {
            string[] urls = await _mediaInfoClient.GetShowImages(showId);
            return urls.Random();
        }

        private bool IsDefaultedTitle(string title)
            => string.IsNullOrWhiteSpace(title)
            || StringHelper.RemoveDigits(title).Trim().Equals("episode", StringComparison.OrdinalIgnoreCase);
    }
}