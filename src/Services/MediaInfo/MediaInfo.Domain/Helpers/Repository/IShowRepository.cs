using MediaInfo.Domain.Helpers.DTOs.Shows;
using MediaInfo.Domain.Helpers.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Services.Interfaces {

    public interface IShowRepository : IRepository<Show> {

        Task<Show?> FindAsync(string title);

        Task<IEnumerable<Show>> FindAsync(IEnumerable<string> titles);
    }
}