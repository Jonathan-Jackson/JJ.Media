using JJ.Media.MediaInfo.Core.Models;
using JJ.Media.MediaInfo.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Services.Search {

    public class ShowSearchService {
        private readonly IApiSearchService _apiSearchService;
        private readonly IShowRepository _showRepository;

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

        private Show? FindMatch(IEnumerable<Show> shows, IEnumerable<string> possibleNames) {
        }

        private Show? FindSimilarMatch(IEnumerable<Show> shows, IEnumerable<string> possibleNames) {
        }
    }
}