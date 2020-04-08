using JJ.Media.MediaInfo.Core.Entities.Episodes;
using JJ.Media.MediaInfo.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Services.Interfaces {

    public interface IShowRepository : IRepository<Show> {

        Task<Show?> FindAsync(string title);

        Task<IEnumerable<Show>> FindAsync(IEnumerable<string> titles);
    }
}