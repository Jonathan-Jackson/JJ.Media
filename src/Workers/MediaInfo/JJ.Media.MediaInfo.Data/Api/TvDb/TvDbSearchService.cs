using JJ.Media.MediaInfo.Core.Dto;
using JJ.Media.MediaInfo.Core.Entities;
using JJ.Media.MediaInfo.Core.Entities.Episodes;
using JJ.Media.MediaInfo.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TvDbSharper;
using TvDbSharper.Dto;

namespace JJ.Media.MediaInfo.Data.Api {

    public class TvDbSearchService : IApiSearchService {
        private readonly IMemoryCache _cache;
        private readonly ITvDbClient _client;
        private readonly TvDbOptions _options;

        public TvDbSearchService(ITvDbClient client, IMemoryCache cache, TvDbOptions options) {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        private static SemaphoreSlim _semaphore { get; } = new SemaphoreSlim(1, 1);

        public async Task AuthenticateAsync() {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                throw new ArgumentException("TvDb Auth: ApiKey is empty.");
            if (string.IsNullOrWhiteSpace(_options.Username))
                throw new ArgumentException("TvDb Auth: Username is empty.");
            if (string.IsNullOrWhiteSpace(_options.UserKey))
                throw new ArgumentException("TvDb Auth: UserKey is empty.");

            await _semaphore.WaitAsync();
            try {
                if (_client.Authentication.Token?.Length > 0)
                    await _client.Authentication.RefreshTokenAsync();
                else
                    await _client.Authentication.AuthenticateAsync(_options.ApiKey, _options.Username, _options.UserKey);
            }
            finally {
                _semaphore.Release();
            }
        }

        public async Task<Episode[]> FindEpisodeAsync(int apiShowId, uint seasonNumber, uint episodeNumber) {
            var records = await FindEpisodeRecordAsync(apiShowId, seasonNumber, episodeNumber);
            return records.Select(CreateEpisode).ToArray();
        }

        public async Task<Show[]> FindShowAsync(IEnumerable<string> showNames) {
            // Change to be fully async? need to be careful with 10+ names
            var results = await Task.WhenAll(showNames.Select(FindShowAsync));
            return results.SelectMany(x => x).ToArray();
        }

        public async Task<Show[]> FindShowAsync(string showName) {
            var results = await _cache.GetOrCreateAsync($"TvDbSearchService.FindShowAsync.{showName}", (cacheFactory) => {
                cacheFactory.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(10);
                return _client.Search.SearchSeriesByNameAsync(showName);
            });
            return results.Data.Select(CreateShow).ToArray();
        }

        private Episode CreateEpisode(EpisodeRecord record)
            => new Episode {
                Title = record.EpisodeName ?? string.Empty,
                AbsoluteNumber = record.AbsoluteNumber,
                Overview = record.Overview ?? string.Empty,
                IsMovie = record.IsMovie > 0,
                SeasonNumber = record.AiredSeason,
                EpisodeNumber = record.AiredEpisodeNumber,
                TvDbId = record.Id
            };

        private Show CreateShow(SeriesSearchResult record) {
            var titles = record.Aliases.Select(alias => new ShowTitle { Name = alias }).ToList();
            titles.Add(new ShowTitle { Name = record.SeriesName, IsPrimary = true });
            return new Show {
                Titles = titles,
                Overview = record.Overview ?? string.Empty,
                AirDate = DateTimeOffset.TryParse(record.FirstAired, out DateTimeOffset airDate) ? (DateTimeOffset?)airDate : null,
                TvDbId = record.Id
            };
        }

        private async Task<IEnumerable<EpisodeRecord>> FindEpisodeRecordAsync(int apiShowId, uint seasonNumber, uint episodeNumber) {
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

        private bool IsPossibleAbsoluteNumberMatch(EpisodeRecord record, uint seasonNumber, uint episodeNumber)
            => seasonNumber == 1 && record.AbsoluteNumber == episodeNumber;

        private bool IsSeasonEpisodeMatch(EpisodeRecord record, uint seasonNumber, uint episodeNumber)
           => (record.AiredSeason == seasonNumber || record.DvdSeason == seasonNumber)
                && (record.AiredEpisodeNumber == episodeNumber || record.DvdEpisodeNumber == episodeNumber);
    }
}