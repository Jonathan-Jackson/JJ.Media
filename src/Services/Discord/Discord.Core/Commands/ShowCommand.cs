using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using JJ.Framework.Extensions;
using MediaInfo.API.Client.Client;
using MediaInfo.API.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Core.Commands {

    public class ShowCommand {
        private readonly IMediaInfoClient _mediaInfoRepository;

        public ShowCommand(IMediaInfoClient mediaInfoRepository) {
            _mediaInfoRepository = mediaInfoRepository;
        }

        [Command("show")]
        [Description("Finds information for a show. Example: !search [show name]")]
        public async Task Search(CommandContext ctx, params string[] values) {
            if (!values.Any()) {
                await ctx.RespondAsync($"📺 Enter a show name after the show command.");
                return;
            }

            string showName = string.Join(' ', values);

            try {
                int showId = await _mediaInfoRepository.ShowSearch(showName);
                if (showId > 0) {
                    var show = await _mediaInfoRepository.GetShow(showId);
                    await ctx.RespondAsync(embed: await CreateMessage(show));
                }
                else
                    await ctx.RespondAsync($"📺 No Results for '{showName}'");
            }
            // TODO: Add Logging.
            catch {
                await ctx.RespondAsync($"👀 An error occured.");
            }
        }

        private async Task<DiscordEmbedBuilder> CreateMessage(Show show) {
            var descriptionTask = CreateDescription(show);
            var bannerTask = GetRandomShowBanner(show.Id);

            return new DiscordEmbedBuilder {
                Title = show.Titles.FirstOrDefault(x => x.IsPrimary)?.Title ?? show.Titles.First()?.Title,
                Description = await descriptionTask,
                Color = DiscordColor.CornflowerBlue,
                ImageUrl = await bannerTask
            };
        }

        private async Task<string> CreateDescription(Show show) {
            var infoLinkTask = _mediaInfoRepository.GetShowExternalLink(show.Id);
            var overviewTask = _mediaInfoRepository.GetShowOverview(show.Id);

            string altShowTitles = show.Titles.Count() > 1
                ? Formatter.Italic(string.Join(", ", GetAltTitles(show)))
                : string.Empty;

            // This is quite confusing ~ How do we simplify this?
            if (string.IsNullOrWhiteSpace(altShowTitles))
                return string.Join("\r\n", await overviewTask, "", await infoLinkTask);
            else
                return string.Join("\r\n", altShowTitles, "", await overviewTask, "", await infoLinkTask);
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
            string[] urls = await _mediaInfoRepository.GetShowImages(showId);
            return urls.Random();
        }
    }
}