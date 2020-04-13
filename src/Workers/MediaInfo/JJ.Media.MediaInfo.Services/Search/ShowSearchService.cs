using JJ.Media.MediaInfo.Core.Models;
using JJ.Media.MediaInfo.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Services.Search {

    public class ShowSearchService {
        private readonly IApiSearchService _apiSearchService;
        private readonly IShowRepository _showRepository;

        public ShowSearchService(IApiSearchService apiSearchService, IShowRepository showRepository) {
            _apiSearchService = apiSearchService;
            _showRepository = showRepository;
        }

        public async Task<Show?> SearchAsync(IEnumerable<string> possibleNames) {
            // DB Search
            var dbResults = await _showRepository.FindAsync(possibleNames);
            var dbMatch = FindMatch(dbResults, possibleNames);
            if (dbMatch != null)
                return dbMatch;

            // API Search
            var apiResults = await _apiSearchService.FindShow(possibleNames);
            var apiMatch = FindMatch(apiResults, possibleNames);
            if (apiMatch != null)
                return apiMatch;

            // No Match - Just find something similar..
            var allResults = dbResults.Concat(apiResults);
            var allMatch = FindSimilarMatch(allResults, possibleNames);

            // note this CAN be null if we don't have any similar match.
            return allMatch;
        }

        /// <summary>
        /// Returns the show that matches the strongest against the possible names.
        /// </summary>
        /// <param name="possibleNames"> Names of possible matches. It's important that these are ordered by most likely.</param>
        private Show? FindMatch(IEnumerable<Show> shows, IEnumerable<string> possibleNames) {
        }

        private Show? FindSimilarMatch(IEnumerable<Show> shows, IEnumerable<string> possibleNames) {
        }
    }
}