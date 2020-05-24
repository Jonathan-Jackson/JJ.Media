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
        [Description("Lists episodes that can be viewed for a show. To select a specific season, append 'season [number]' to the end. Example: !watch [show name] or !watch [show name] season 3")]
        public async Task Watch(CommandContext ctx, params string[] values) {
            int? season = null;
            if (EndsWithSeason(values)) {
                season = int.Parse(values.Last());
                values = values.Take(values.Length - 2).ToArray();
            }

            string showName = string.Join(' ', values);

            try {
                int id = await _mediaInfoRepository.ShowSearch(showName);
                if (id == 0)
                    await ctx.RespondAsync($"🥤 Show Not Found 😓");
                else {
                    var show = await _mediaInfoRepository.GetShow(id);
                    await ReplyViewableEpisodes(ctx, show, season);
                }
            }
            // TODO: Add Logging.
            catch {
                await ctx.RespondAsync($"👀 An error occured.");
            }
        }

        private bool EndsWithSeason(string[] values) {
            if (values.Length <= 2)
                return false;

            var seasonData = values.Skip(values.Length - 2).ToArray();
            return string.Equals(seasonData[0], "season", StringComparison.OrdinalIgnoreCase)
                && seasonData[1].All(char.IsDigit);
        }

        private async Task ReplyViewableEpisodes(CommandContext ctx, Show show, int? season = null) {
            Episode[] episodes = await _mediaInfoRepository.GetShowEpisodes(show.Id);
            var episodeIds = episodes.Select(ep => ep.Id);
            var processedGuids = await _storageRepository.GetGuidByEpisode(episodeIds);

            if (!processedGuids.Any())
                await ctx.RespondAsync($"🥤 No episodes can be viewed for {show.PrimaryTitle} 😓");
            else
                await ctx.RespondAsync(embed: await CreateMessage(show, episodes, processedGuids, season));
        }

        private async Task<DiscordEmbedBuilder> CreateMessage(Show show, Episode[] episodes, EpisodeGuid[] processed, int? season = null) {
            var bannerTask = _mediaInfoRepository.GetShowImages(show.Id);

            return new DiscordEmbedBuilder {
                Title = show.PrimaryTitle,
                Description = CreateDescription(episodes, processed, season),
                Color = DiscordColor.CornflowerBlue,
                ImageUrl = (await bannerTask).FirstOrDefault()
            };
        }

        private string CreateDescription(Episode[] episodes, EpisodeGuid[] processed, int? season = null) {
            var builder = new StringBuilder();

            var pairs = processed
                .Select(item => new { item.Guid, Episode = episodes.First(ep => ep.Id == item.EpisodeId) });

            var groupedSeasons = pairs.GroupBy(x => x.Episode.SeasonNumber);
            var displaySeason = season == null
                ? groupedSeasons.OrderBy(x => x.Key).First()
                : pairs.Where(x => x.Episode.SeasonNumber == season);

            if (displaySeason.Any()) {
                // Print the season.
                builder.AppendLine($"Season {displaySeason.First().Episode.SeasonNumber}");
                foreach (var pair in displaySeason.OrderBy(p => p.Episode.AbsoluteNumber)) {
                    string text = $"{pair.Episode.EpisodeNumber}. {pair.Episode.Title}";
                    Uri uri = new Uri($@"{_viewerDomain}/{pair.Guid.ToString()}");

                    if (builder.Length + text.Length <= 1950) {
                        builder.AppendLine(Formatter.MaskedUrl(text, uri, "Watch the episode."));
                    }
                    else {
                        builder.AppendLine("...");
                        break;
                    }
                }
            }
            else {
                builder.AppendLine($"No episodes found for season {season}");
            }

            builder.AppendLine();
            builder.AppendLine($"Available Seasons: {string.Join(", ", groupedSeasons.OrderBy(x => x.Key).Select(x => x.Key))}");

            // Description length cannot exceed 2048 characters
            // (DISCORD RULE).
            return builder.ToString();
        }
    }
}