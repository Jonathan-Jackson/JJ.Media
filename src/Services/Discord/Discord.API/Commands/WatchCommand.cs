using Discord.API.Models;
using Discord.API.Models.Options;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MediaInfo.API.Client.Client;
using MediaInfo.API.Client.Models;
using Storage.API.Client.Client;
using Storage.API.Client.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Commands {

    public class WatchCommand {
        private readonly MediaInfoClient _mediaInfoRepository;
        private readonly StorageClient _storageRepository;
        private readonly string _viewerDomain;

        public WatchCommand(MediaInfoClient mediaInfoRepository, StorageClient storageRepository, DiscordOptions options) {
            _mediaInfoRepository = mediaInfoRepository;
            _storageRepository = storageRepository;
            _viewerDomain = options.ViewerDomain;
        }

        [Command("watch")]
        [Description("Lists episodes that can be viewed for a show. Example: !watch [show name]")]
        public async Task Watch(CommandContext ctx, params string[] values) {
            string showName = string.Join(' ', values);

            try {
                int id = await _mediaInfoRepository.ShowSearch(showName);
                if (id == 0)
                    await ctx.RespondAsync($"🥤 Show Not Found 😓");
                else {
                    var show = await _mediaInfoRepository.GetShow(id);
                    await ReplyViewableEpisodes(ctx, show);
                }
            }
            // TODO: Add Logging.
            catch {
                await ctx.RespondAsync($"👀 An error occured.");
            }
        }

        private async Task ReplyViewableEpisodes(CommandContext ctx, Show show) {
            Episode[] episodes = await _mediaInfoRepository.GetShowEpisodes(show.Id);
            var episodeIds = episodes.Select(ep => ep.Id);
            var processedGuids = await _storageRepository.GetGuidByEpisode(episodeIds);

            if (!processedGuids.Any())
                await ctx.RespondAsync($"🥤 No episodes can be viewed for {show.PrimaryTitle} 😓");
            else
                await ctx.RespondAsync(embed: await CreateMessage(show, episodes, processedGuids));
        }

        private async Task<DiscordEmbedBuilder> CreateMessage(Show show, Episode[] episodes, EpisodeGuid[] processed) {
            var bannerTask = _mediaInfoRepository.GetShowImages(show.Id);

            return new DiscordEmbedBuilder {
                Title = show.PrimaryTitle,
                Description = CreateDescription(episodes, processed),
                Color = DiscordColor.CornflowerBlue,
                ImageUrl = (await bannerTask).FirstOrDefault()
            };
        }

        private string CreateDescription(Episode[] episodes, EpisodeGuid[] processed) {
            var builder = new StringBuilder();

            var pairs = processed
                .Select(item => new { item.Guid, Episode = episodes.First(ep => ep.Id == item.EpisodeId) });

            var seasons = pairs.GroupBy(x => x.Episode.SeasonNumber).OrderBy(x => x.Key);

            foreach (var season in seasons) {
                builder.AppendLine($"Season {season.Key}");

                foreach (var pair in pairs.OrderBy(p => p.Episode.AbsoluteNumber)) {
                    string text = $"{pair.Episode.EpisodeNumber}. {pair.Episode.Title}";
                    Uri uri = new Uri($@"{_viewerDomain}/{pair.Guid.ToString()}");
                    builder.AppendLine(Formatter.MaskedUrl(text, uri, "Watch the episode."));
                }
            }

            return builder.ToString();
        }
    }
}