using Downloader.Core.Helpers.DTOs;
using System;
using System.Collections.Generic;

namespace Downloader.Core.Feeds {

    internal class HorribleSubsFeed : IFeed {

        public IEnumerable<Torrent> Read() {
            throw new NotImplementedException();
        }
    }
}