using JJ.Media.MediaInfo.Core.Entities;
using JJ.Media.MediaInfo.Core.Entities.Episodes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Services.Interfaces {

    public interface IApiSearchService {

        Task AuthenticateAsync();

        Task<Show[]> FindShowAsync(IEnumerable<string> showNames);

        Task<Show[]> FindShowAsync(string showName);

        Task<Episode[]> FindEpisodeAsync(int apiShowId, uint seasonNumber, uint episodeNumber);
    }
}