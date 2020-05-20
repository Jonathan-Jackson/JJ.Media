using MediaInfo.Domain.Helpers.DTOs.Shows;
using MediaInfo.Infrastructure.Client;
using MediaInfo.Infrastructure.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using TvDbSharper;
using Xunit;

namespace MediaInfo.IntegrationTesting.Remote {

    public class TvDbSearchTests : TestBase {

        [Fact]
        public async Task Authenticate() {
            var tvdb = await GetAuthenticatedTvDbAsync();
            Assert.NotNull(tvdb);
        }

        [Theory]
        [InlineData("One Piece")]
        [InlineData("Doctor Who")]
        public async Task FindShow(string showName) {
            var tvdb = await GetAuthenticatedTvDbAsync();
            var result = await tvdb.FindShowAsync(showName);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData(81797)] // One Piece
        public async Task GetShowBanners(int tvdbId) {
            var tvdb = await GetAuthenticatedTvDbAsync();
            var result = await tvdb.GetShowBannersAsync(tvdbId);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData(81797)] // One Piece
        public async Task GetShowLink(int tvdbId) {
            var tvdb = await GetAuthenticatedTvDbAsync();
            var result = tvdb.GetShowLink(tvdbId);
            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Theory]
        [InlineData(81797, 1, 1)] // One Piece
        public async Task FindEpisode(int tvdbId, int season, int episode) {
            var tvdb = await GetAuthenticatedTvDbAsync();
            var result = await tvdb.FindEpisodeAsync(new Show { TvDbId = tvdbId }, season, episode);
            Assert.NotEmpty(result);
        }

        private async Task<TvDbSearch> GetAuthenticatedTvDbAsync() {
            var tvDb = GetTvDb();
            await tvDb.AuthenticateAsync();
            return tvDb;
        }

        private TvDbSearch GetTvDb() {
            var apiSearch = _services.GetService(typeof(ITvDbClient)) as ITvDbClient;
            var cache = _services.GetService(typeof(IMemoryCache)) as IMemoryCache;
            var options = _services.GetService(typeof(TvDbOptions)) as TvDbOptions;

            return new TvDbSearch(apiSearch, cache, options);
        }
    }
}