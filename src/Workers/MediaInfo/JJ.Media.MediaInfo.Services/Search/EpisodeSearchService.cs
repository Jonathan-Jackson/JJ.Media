using JJ.Media.MediaInfo.Core.Entities.Episodes;
using JJ.Media.MediaInfo.Core.Models;
using JJ.Media.MediaInfo.Services.Episodes;
using JJ.Media.MediaInfo.Services.Interfaces;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Services.Search {

    public class EpisodeSearchService {
        private readonly EpisodeService _episodeService;
        private readonly ShowSearchService _showSearchService;
        private readonly IApiSearchService _apiSearchService;
        private readonly IMiningService _miningService;

        public EpisodeSearchService(EpisodeService episodeService, ShowSearchService showSearchService, IApiSearchService apiSearchService, IMiningService miningService) {
            _episodeService = episodeService;
            _showSearchService = showSearchService;
            _apiSearchService = apiSearchService;
            _miningService = miningService;
        }

        public async Task<Episode?> SearchAsync(string episodeName) {
            MinedEpisode minedData = _miningService.MineEpisodeName(episodeName);
            Show? show = await _showSearchService.SearchAsync(minedData.PossibleNames);

            if (show != null)
                return await SearchAsync(show, minedData.SeasonNumber, minedData.EpisodeNumber);
            else
                return null; // We can't find the show, so we cant find the episode.
        }

        public async Task<Episode?> SearchAsync(Show show, uint seasonNumber, uint episodeNumber) {
            // Check the DB.
            var result = await _episodeService.Find(show.Id, seasonNumber, episodeNumber);
            if (result != null)
                return result;

            // Check API.
            if (show.TvDbId > 0) {
                result = await _apiSearchService.FindEpisode(show.TvDbId, seasonNumber, episodeNumber);
                if (result != null)
                    await _episodeService.Add(result);
            }

            return result;
        }
    }
}