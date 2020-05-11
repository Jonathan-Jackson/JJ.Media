﻿using DSharpPlus;
using DSharpPlus.Entities;
using JJ.Media.Core.Extensions;
using Microsoft.Extensions.Logging;
using Storage.Domain.Helpers.Abstraction;
using Storage.Domain.Helpers.Events;
using Storage.Domain.Helpers.Options;
using Storage.Domain.Helpers.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Domain.Plugins {

    public class DiscordPlugin : IEventHandler<ProcessedEpisode> {
        private readonly string _channelName;
        private readonly DiscordClient _discord;
        private readonly ILogger<DiscordPlugin> _logger;
        private readonly IMediaInfoRepository _mediaInfoRepository;

        public DiscordPlugin(DiscordOptions options, IMediaInfoRepository mediaRepo, ILogger<DiscordPlugin> logger) {
            _discord = new DiscordClient(new DiscordConfiguration {
                Token = options.Token,
                TokenType = TokenType.Bot
            });

            _logger = logger;
            _mediaInfoRepository = mediaRepo;
            _channelName = options.AlertChannelName;
            _discord.ConnectAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Invokes the specific actions of ProcessedEpisode
        /// with the discord plugin.
        /// </summary>
        public Task InvokeAsync(ProcessedEpisode @event)
            => TryPromptMessageAsync(@event);

        /// <summary>
        /// Attempts to post a message into discord regarding the episode processed event.
        /// Any failures are caught and logged.
        /// </summary>
        public async Task TryPromptMessageAsync(ProcessedEpisode @event) {
            try {
                var channels = GetConfiguredChannels();
                var message = await CreateMessage(@event);

                foreach (var channel in channels)
                    await channel.SendMessageAsync(embed: message);
            }
            catch (Exception ex) {
                _logger.LogWarning(ex, "Failed to send processed message into discord.");
            }
        }

        private async Task<string> CreateDescription(ProcessedEpisode @event) {
            string seasonEpisodeHeader = Formatter.Bold($"Season {@event.SeasonNumber} Episode {@event.EpisodeNumber}");
            string episodeDescription = IsDefaultedTitle(@event.EpisodeTitle) ? string.Empty : @event.EpisodeTitle;
            string viewUrl = "@: {view-url-here}";
            string infoUrl = "info: " + await _mediaInfoRepository.GetShowRemoteLink(@event.ShowId);

            if (string.IsNullOrWhiteSpace(episodeDescription))
                return string.Join("\r\n", seasonEpisodeHeader, viewUrl, infoUrl);
            else
                return string.Join("\r\n", seasonEpisodeHeader, episodeDescription, viewUrl, infoUrl);
        }

        private async Task<DiscordEmbedBuilder> CreateMessage(ProcessedEpisode @event) {
            var descriptionTask = CreateDescription(@event);
            var bannerTask = GetRandomShowBanner(@event.ShowId);

            return new DiscordEmbedBuilder {
                Title = $"{@event.ShowTitle}",
                Description = await descriptionTask,
                Color = DiscordColor.CornflowerBlue,
                ImageUrl = await bannerTask
            };
        }

        private IEnumerable<DiscordChannel> GetConfiguredChannels()
            => _discord.Guilds.Values
                    .Select(guild => guild.Channels)
                    .SelectMany(x => x)
                    .Where(channel => string.Equals(channel.Name, _channelName, StringComparison.OrdinalIgnoreCase));

        private async Task<string> GetRandomShowBanner(int showId) {
            string[] urls = await _mediaInfoRepository.GetShowImages(showId);
            return urls.Random();
        }

        private bool IsDefaultedTitle(string title)
            => string.IsNullOrWhiteSpace(title)
            || title.RemoveDigits().Trim().Equals("episode", StringComparison.OrdinalIgnoreCase);
    }
}