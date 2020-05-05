using Downloader.Core.Helpers.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Downloader.Core.Feeds {

    internal interface IFeed {

        Task<IEnumerable<Torrent>> Read();
    }
}