using MediaInfo.Domain.Helpers.DTOs.Episodes;
using MediaInfo.Domain.Helpers.DTOs.Shows;
using MediaInfo.Domain.Helpers.Exceptions;
using MediaInfo.Domain.Helpers.Repository;
using MediaInfo.DomainLayer.Miners;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MediaInfo.Domain.DomainLayer.Search {

    /// <summary>
    /// Searches for Episodes via criteria.
    /// </summary>
    public class EpisodeSearch {
        private readonly IEpisodeApiSearch _apiSearch;
        private readonly IEpisodeRepository _episodeRepo;
        private readonly ShowSearch _showSearch;

        public EpisodeSearch(ShowSearch showSearch, IEpisodeRepository episodeRepo, IEpisodeApiSearch apiSearch) {
            _showSearch = showSearch;
            _episodeRepo = episodeRepo;
            _apiSearch = apiSearch;
        }

        /// <summary>
        /// Returns an episode found matching the value input.
        /// i.e. "[HorribleSubs] One Piece - 201 [720p].mkv"
        /// would return a One Piece episode.
        /// </summary>
        /// <exception cref="SearchNotFoundException">Thrown if the show cannot be found.</exception>
        public async Task<Episode> SearchAsync(string value) {
            IMiner miner = GetMiner(value);
            var minedResult = miner.MineEpisodeName(value);

            var show = await _showSearch.SearchAsync(minedResult.PossibleNames);

            if (show == null) {
                throw new SearchNotFoundException(JsonSerializer.Serialize(minedResult));
            }
            else {
                var seasonNumber = minedResult.SeasonNumber ?? _showSearch.GetLatestStorageSeason(show, minedResult.EpisodeNumber);
                return await SearchAsync(show, seasonNumber, minedResult.EpisodeNumber);
            }
        }

        /// <summary>
        /// Returns the episode of the show that fits the season and episode number.
        /// A missing episode is returned upon no matches.
        /// </summary>
        public async Task<Episode> SearchAsync(Show show, int seasonNumber, int episodeNumber) {
            // Check DB.
            var result = await _episodeRepo.FindAsync(show.Id, seasonNumber, episodeNumber);
            result ??= episodeNumber > 48 ? await _episodeRepo.FindAsync(show.Id, episodeNumber) : null;

            if (result != null)
                return result;

            // Check API.
            result = await SearchAPIAsync(show, seasonNumber, episodeNumber);

            return result ?? new MissingEpisode(show.Id, seasonNumber, episodeNumber);
        }

        /// <summary>
        /// Returns the best miner that matches the string.
        /// </summary>
        private IMiner GetMiner(string value) {
            // TODO: Add Logic to decide how we should mine
            // this file - basic mining? or add a source param?
            return new AnimeMiner();
        }

        /// <summary>
        /// Returns the highest priority episode that matches the season and episode.
        /// </summary>
        private Episode? GetPriorityEpisode(IEnumerable<Episode> episodes, int seasonNumber, int episodeNumber) {
            var ordered = episodes.OrderByDescending(x => x.SeasonNumber == seasonNumber && x.EpisodeNumber == episodeNumber);

            if (episodeNumber > 24 && seasonNumber == 1)
                ordered = ordered.ThenBy(x => x.AbsoluteNumber == episodeNumber);

            return ordered.FirstOrDefault(x => (x.SeasonNumber == seasonNumber && x.EpisodeNumber == episodeNumber) || ((seasonNumber == 1 || episodeNumber > 24) && x.AbsoluteNumber == episodeNumber));
        }

        /// <summary>
        /// Returns an episode found via the API search.
        /// </summary>
        private async Task<Episode?> SearchAPIAsync(Show show, int seasonNumber, int episodeNumber) {
            var results = await _apiSearch.FindEpisodeAsync(show, seasonNumber, episodeNumber);
            var result = results.Any() ? GetPriorityEpisode(results, seasonNumber, episodeNumber) : null;

            if (result != null)
                result.Id = await _episodeRepo.InsertAsync(result);

            return result;
        }
    }
}