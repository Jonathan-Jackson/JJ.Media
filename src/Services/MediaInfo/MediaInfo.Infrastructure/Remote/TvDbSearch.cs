using MediaInfo.Domain.Helpers.DTOs.Episodes;
using MediaInfo.Domain.Helpers.DTOs.Shows;
using MediaInfo.Domain.Helpers.Repository;
using MediaInfo.Domain.Helpers.Repository.Interfaces;
using MediaInfo.Infrastructure.Helpers.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TvDbSharper;
using TvDbSharper.Dto;

namespace MediaInfo.Infrastructure.Remote {

    /// <summary>
    /// API Search for TVDB.
    /// See: https://api.thetvdb.com/swagger
    /// </summary>
    public class TvDbSearch : IApiSearch, IEpisodeApiSearch, IShowApiSearch {
        private readonly IMemoryCache _cache;
        private readonly ITvDbClient _client;
        private readonly TvDbOptions _options;
        private readonly SemaphoreSlim _semaphore;

        public TvDbSearch(ITvDbClient client, IMemoryCache cache, TvDbOptions options) {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _semaphore = new SemaphoreSlim(1, 1);

            AuthenticateAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Authenticates the searches, this usually needs
        /// refreshing every 24 hours.
        /// </summary>
        public async Task AuthenticateAsync() {
            if (string.IsNullOrWhiteSpace(_options.Token))
                throw new ArgumentException("TvDb Auth: Token is empty.");

            await _semaphore.WaitAsync();
            try {
                if (_client.Authentication.Token?.Length > 0)
                    await _client.Authentication.RefreshTokenAsync();
                else
                    await _client.Authentication.AuthenticateAsync(_options.Token);
            }
            finally {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Returns episodes which match the season and episode number.
        /// </summary>
        public async Task<Episode[]> FindEpisodeAsync(Show show, int seasonNumber, int episodeNumber) {
            var records = await FindEpisodeRecordAsync(show.TvDbId, seasonNumber, episodeNumber);
            return records.Select(record => CreateEpisode(record, show)).ToArray();
        }

        /// <summary>
        /// Returns shows found via the API that match the names.
        /// </summary>
        public async Task<Show[]> FindShowAsync(IEnumerable<string> showNames) {
            var results = await Task.WhenAll(showNames.Select(FindShowAsync));
            return results.SelectMany(x => x).ToArray();
        }

        /// <summary>
        /// Returns shows found via the API that match the name.
        /// </summary>
        public async Task<Show[]> FindShowAsync(string showName) {
            var results = await _cache.GetOrCreateAsync($"TvDbSearchService.FindShowAsync.{showName}", (cacheFactory) => {
                cacheFactory.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(10);
                return _client.Search.SearchSeriesByNameAsync(showName);
            });
            return results.Data.Select(CreateShow).ToArray();
        }

        /// <summary>
        /// Creates an episode from the EpisodeRecord.
        /// </summary>
        private Episode CreateEpisode(EpisodeRecord record, Show show)
            => new Episode {
                Title = record.EpisodeName ?? string.Empty,
                AbsoluteNumber = record.AbsoluteNumber,
                Overview = record.Overview ?? string.Empty,
                IsMovie = record.IsMovie > 0,
                SeasonNumber = record.AiredSeason,
                EpisodeNumber = record.AiredEpisodeNumber,
                TvDbId = record.Id,
                ShowId = show.Id
            };

        /// <summary>
        /// Creates a show from the series search result.
        /// </summary>
        private Show CreateShow(SeriesSearchResult record) {
            var titles = record.Aliases.Distinct().Select(alias => new ShowTitle { Title = alias }).ToList();
            titles.Add(new ShowTitle { Title = record.SeriesName, IsPrimary = true });

            return new Show {
                Titles = titles,
                Overview = record.Overview ?? string.Empty,
                AirDate = DateTimeOffset.TryParse(record.FirstAired, out DateTimeOffset airDate) ? (DateTimeOffset?)airDate : null,
                TvDbId = record.Id
            };
        }

        /// <summary>
        /// Finds any episode records that match the show, season and episode number.
        /// </summary>
        private async Task<IEnumerable<EpisodeRecord>> FindEpisodeRecordAsync(int apiShowId, int seasonNumber, int episodeNumber) {
            var tasks = new List<Task<TvDbResponse<EpisodeRecord[]>>>();

            var firstResponse = await _client.Series.GetEpisodesAsync(apiShowId, 1);

            for (int i = 2; i <= firstResponse.Links.Last; i++) {
                tasks.Add(_client.Series.GetEpisodesAsync(apiShowId, i));
            }

            var results = await Task.WhenAll(tasks);
            var episodes = firstResponse.Data.Concat(results.SelectMany(x => x.Data));

            return episodes.Where(record => IsSeasonEpisodeMatch(record, seasonNumber, episodeNumber)
                                     || IsPossibleAbsoluteNumberMatch(record, seasonNumber, episodeNumber));
        }

        /// <summary>
        /// Returns true if the record matches the season and episode number via the absolute value.
        /// </summary>
        private bool IsPossibleAbsoluteNumberMatch(EpisodeRecord record, int seasonNumber, int episodeNumber)
            => (seasonNumber == 1 || episodeNumber > 48) && record.AbsoluteNumber == episodeNumber;

        /// <summary>
        /// Returns true if the record matches the season and episode number standard season and episode numbering.
        /// </summary>
        private bool IsSeasonEpisodeMatch(EpisodeRecord record, int seasonNumber, int episodeNumber)
           => (record.AiredSeason == seasonNumber || record.DvdSeason == seasonNumber)
                && (record.AiredEpisodeNumber == episodeNumber || record.DvdEpisodeNumber == episodeNumber);
    }
}