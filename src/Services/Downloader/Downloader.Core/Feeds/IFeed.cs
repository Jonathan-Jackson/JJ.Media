using Downloader.Core.Helpers.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Downloader.Core.Feeds {

    public interface IFeed {

        Task<IEnumerable<Torrent>> ReadAsync();
    }
}