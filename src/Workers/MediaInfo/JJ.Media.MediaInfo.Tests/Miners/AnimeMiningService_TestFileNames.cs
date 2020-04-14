using JJ.Media.MediaInfo.Core.Models;
using JJ.Media.MediaInfo.Services.Miners;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using JJ.Media.Core.Extensions;

namespace JJ.Media.MediaInfo.Tests.Miners {

    [TestClass]
    public class AnimeMiningService_TestFileNames {

        private static readonly IDictionary<string, MinedEpisode> _testEpisodes = new Dictionary<string, MinedEpisode>() {
            {"[HorribleSubs] One Piece S01 - 19 [720p].mkv", new MinedEpisode { EpisodeNumber = 19, SeasonNumber = 1, PossibleNames = new[] { "One Piece S01", "One Piece" } } }
        };

        private readonly AnimeMiningService _service;

        public AnimeMiningService_TestFileNames() {
            var logger = new LoggerFactory().CreateLogger<AnimeMiningService>();
            _service = new AnimeMiningService(logger);
        }

        [TestMethod]
        public void MineEpisodeNames_AreEqual() {
            foreach (var testEpisode in _testEpisodes) {
                var result = _service.MineEpisodeName(testEpisode.Key);

                Assert.AreEqual(result.SeasonNumber, testEpisode.Value.SeasonNumber);
                Assert.AreEqual(result.EpisodeNumber, testEpisode.Value.EpisodeNumber);
                Assert.IsTrue(result.PossibleNames.HasEqualContents(testEpisode.Value.PossibleNames));
            }
        }
    }
}