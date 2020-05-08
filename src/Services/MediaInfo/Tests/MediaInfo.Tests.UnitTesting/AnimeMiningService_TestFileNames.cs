using MediaInfo.Domain.Helpers.DTOs.Miners;
using MediaInfo.DomainLayer.Miners;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JJ.Media.MediaInfo.Tests.Unit {

    [TestClass]
    public class AnimeMiningService_TestFileNames {
        private readonly AnimeMiner _miner;

        public AnimeMiningService_TestFileNames() {
            _miner = new AnimeMiner();
        }

        [TestMethod]
        public void MineEpisodeNames_HorribleSubs_FruitBasket2019() {
            var expected = new MinedEpisode { EpisodeNumber = 1, SeasonNumber = 2, PossibleNames = new[] { "Fruits Basket S2 (2019)", "Fruits Basket (2019)", "Fruits Basket", "Fruits" } };
            AssertResult("[HorribleSubs] Fruits Basket S2 (2019) - 01 [720p].mkv", expected);
        }

        [TestMethod]
        public void MineEpisodeNames_HorribleSubs_Baruto() {
            var expected = new MinedEpisode { EpisodeNumber = 151, SeasonNumber = null, PossibleNames = new[] { "Boruto - Naruto Next Generations", "Boruto" } };
            AssertResult("[HorribleSubs] Boruto - Naruto Next Generations - 151 [1080p].mkv", expected);
        }

        [TestMethod]
        public void MineEpisodeNames_HorribleSubs_K() {
            var expected = new MinedEpisode { EpisodeNumber = 3, SeasonNumber = null, PossibleNames = new[] { "K - Seven Stories" } };
            AssertResult("[HorribleSubs] K - Seven Stories - 03 [1080p].mkv", expected);
        }

        [TestMethod]
        public void MineEpisodeNames_HorribleSubs_Jashin() {
            var expected = new MinedEpisode { EpisodeNumber = 6, SeasonNumber = 2, PossibleNames = new[] { "Jashin-chan Dropkick S2", "Jashin-chan Dropkick", "Jashin" } };
            AssertResult("[HorribleSubs] Jashin-chan Dropkick S2 - 06 [1080p].mkv", expected);
        }

        [TestMethod]
        public void MineEpisodeNames_HorribleSubs_OnePiece() {
            var expected = new MinedEpisode { EpisodeNumber = 19, SeasonNumber = 1, PossibleNames = new[] { "One Piece S01", "One Piece", "One" } };
            AssertResult("[HorribleSubs] One Piece S01 - 19 [720p].mkv", expected);
        }

        [TestMethod]
        public void MineEpisodeNames_HorribleSubs_Pet() {
            var expected = new MinedEpisode { EpisodeNumber = 13, SeasonNumber = null, PossibleNames = new[] { "Pet" } };
            AssertResult("[HorribleSubs] Pet - 13 [1080p].mkv", expected);
        }

        [TestMethod]
        public void MineEpisodeNames_InvalidArgumentsTest() {
            Assert.ThrowsException<ArgumentNullException>(() => _miner.MineEpisodeName(null));
            Assert.ThrowsException<ArgumentException>(() => _miner.MineEpisodeName(""));
            Assert.ThrowsException<ArgumentException>(() => _miner.MineEpisodeName(" "));
        }

        [TestMethod]
        public void MineEpisodeNames_Junk_Mp4Extension() {
            var expected = new MinedEpisode { EpisodeNumber = 0, SeasonNumber = null, PossibleNames = new[] { "" } };
            AssertResult(".mp4", expected);
        }

        [TestMethod]
        public void MineEpisodeNames_Junk_RightAngular() {
            var expected = new MinedEpisode { EpisodeNumber = 0, SeasonNumber = null, PossibleNames = new[] { ">>>>>>>" } };
            AssertResult(">>>>>>>", expected);
        }

        [TestMethod]
        public void MineEpisodeNames_Junk_RightSquare() {
            var expected = new MinedEpisode { EpisodeNumber = 0, SeasonNumber = null, PossibleNames = new[] { "[" } };
            AssertResult("[", expected);
        }

        [TestMethod]
        public void MineEpisodeNames_Junk_RomanNumeral() {
            var expected = new MinedEpisode { EpisodeNumber = 0, SeasonNumber = 1, PossibleNames = new[] { " I " } };
            AssertResult(" I ", expected);
        }

        [TestMethod]
        public void MineEpisodeNames_Junk_SeasonNumber() {
            var expected = new MinedEpisode { EpisodeNumber = 1, SeasonNumber = null, PossibleNames = new[] { "s01" } };
            AssertResult("s01", expected);
        }

        [TestMethod]
        public void MineEpisodeNames_Junk_Special() {
            var expected = new MinedEpisode { EpisodeNumber = 0, SeasonNumber = 0, PossibleNames = new[] { "special" } };
            AssertResult(" special ", expected);
        }

        private void AssertResult(string episodeName, MinedEpisode expectedResult) {
            var result = _miner.MineEpisodeName(episodeName);

            Assert.AreEqual(expectedResult.SeasonNumber, result.SeasonNumber);
            Assert.AreEqual(expectedResult.EpisodeNumber, result.EpisodeNumber);

            Assert.AreEqual(expectedResult.PossibleNames.Length, result.PossibleNames.Length);
            for (int i = 0; i < result.PossibleNames.Length; i++)
                Assert.AreEqual(expectedResult.PossibleNames[i], result.PossibleNames[i]);
        }
    }
}