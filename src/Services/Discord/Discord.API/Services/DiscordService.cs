using Discord.API.Models.Options;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.API.Services {

    public abstract class DiscordService {
        protected readonly string _channelName;
        protected readonly string _viewerDomain;

        public DiscordService(DiscordOptions options) {
            _channelName = options.AlertChannelName;
            _viewerDomain = options.ViewerDomain;
        }

        protected virtual async Task<IEnumerable<DiscordChannel>> GetConfiguredChannels(DiscordClient discord) {
            if (discord.Guilds.Any())
                return GetMatchedChannels(discord);
            else {
                // Guilds may not be loaded yet
                await Task.Delay(2000);
                return GetMatchedChannels(discord);
            }
        }

        private IEnumerable<DiscordChannel> GetMatchedChannels(DiscordClient discord)
            => discord.Guilds.Values
                    .Select(guild => guild.Channels)
                    .SelectMany(x => x)
                    .Where(channel => string.Equals(channel.Name, _channelName, StringComparison.OrdinalIgnoreCase));
    }
}