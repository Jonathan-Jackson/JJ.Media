using JJ.Media.MediaInfo.Core.Entities.Episodes;
using JJ.Media.MediaInfo.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Services.Interfaces {

    public interface IApiSearchService {

        Task<Show[]> FindShow(IEnumerable<string> showNames);

        Task<Show?> FindShow(string showName);

        Task<Episode?> FindEpisode(int apiShowId, uint seasonNumber, uint episodeNumber);
    }
}