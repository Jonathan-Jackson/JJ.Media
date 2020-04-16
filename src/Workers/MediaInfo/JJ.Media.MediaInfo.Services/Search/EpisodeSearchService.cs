using JJ.Media.MediaInfo.Core.Entities;
using JJ.Media.MediaInfo.Core.Entities.Episodes;
using JJ.Media.MediaInfo.Core.Models;
using JJ.Media.MediaInfo.Services.Episodes;
using JJ.Media.MediaInfo.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

            if (show != null) {
                uint seasonNumber = minedData.SeasonNumber ?? FindEpisodeSeason(show, minedData.EpisodeNumber);
                return await SearchAsync(show, seasonNumber, minedData.EpisodeNumber);
            }
            else
                return null; // We can't find the show, so we cant find the episode.
        }

        private uint FindEpisodeSeason(Show show, uint episodeNumber) {
            throw new NotImplementedException();
        }

        public async Task<Episode?> SearchAsync(Show show, uint seasonNumber, uint episodeNumber) {
            // Check the DB.
            var result = await _episodeService.Find(show.Id, seasonNumber, episodeNumber);
            if (result != null)
                return result;

            // Check API.
            if (show.TvDbId > 0) {
                var results = await _apiSearchService.FindEpisodeAsync(show.TvDbId, seasonNumber, episodeNumber);
                if (results.Any()) {
                    result = GetPriorityEpisode(results, seasonNumber, episodeNumber);
                    await _episodeService.Add(result);
                }
            }

            return result;
        }

        private Episode GetPriorityEpisode(IEnumerable<Episode> results, uint seasonNumber, uint episodeNumber) {
            throw new NotImplementedException();
        }
    }
}