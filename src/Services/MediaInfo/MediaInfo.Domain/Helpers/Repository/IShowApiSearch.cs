using MediaInfo.Domain.Helpers.DTOs.Shows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaInfo.Domain.Helpers.Repository {

    public interface IShowApiSearch {

        Task<Show[]> FindShowAsync(IEnumerable<string> showNames);

        Task<Show[]> FindShowAsync(string showName);

        Task<string[]> GetShowBannersAsync(int showApiId);

        string GetShowLink(int showTvDbId);
    }
}