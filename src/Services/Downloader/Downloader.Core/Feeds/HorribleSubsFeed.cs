using Downloader.Core.Helpers.DTOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace Downloader.Core.Feeds {

    public class HorribleSubsFeed : IFeed {
        private string _uri;
        private int _quality;
        private ILogger<HorribleSubsFeed> _log;

        public HorribleSubsFeed(ILogger<HorribleSubsFeed> log, HorribleSubsOptions options) {
            _log = log;
            _uri = options.Uri;
            _quality = options.Quality;
        }

        /// <summary>
        /// Returns the latest torrents on the feed.
        /// </summary>
        public async Task<IEnumerable<Torrent>> ReadAsync() {
            using (var reader = XmlReader.Create($"{_uri}{_quality}")) {
                return DeserializeFeed(SyndicationFeed.Load(reader));
            }
        }

        /// <summary>
        /// Converts the feed into torrents.
        /// </summary>
        private IEnumerable<Torrent> DeserializeFeed(SyndicationFeed feed) {
            _log.LogDebug($"{feed.Items.Count()} items found in HorribleSubs feed.");
            return feed.Items.Select(DeserializeFeedItem).ToList();
        }

        /// <summary>
        /// Converts a feed item into a torrent.
        /// </summary>
        private Torrent DeserializeFeedItem(SyndicationItem item) {
            return new Torrent() {
                MagnetUri = item.Links.FirstOrDefault().Uri.ToString(),
                Title = item.Title.Text,
                PublishDate = item.PublishDate
            };
        }
    }
}