using MediaInfo.Domain.Helpers.DTOs.Miners;
using MediaInfo.DomainLayer.Miners;
using System;
using Xunit;

namespace JJ.Media.MediaInfo.Tests.Unit {

    public class AnimeMinerTests {
        private readonly AnimeMiner _miner;

        public AnimeMinerTests() {
            _miner = new AnimeMiner();
        }

        [Fact]
        public void AlreadyNamed_BlackClover() {
            var expected = new MinedEpisode { EpisodeNumber = 32, SeasonNumber = 2, PossibleNames = new[] { "Black Clover", "Black" } };
            AssertResult("Black Clover - S02E32 (The Lion Awakens).mkv", expected);
        }

        [Fact]
        public void AlreadyNamed_OnePiece() {
            var expected = new MinedEpisode { EpisodeNumber = 13, SeasonNumber = 1, PossibleNames = new[] { "One Piece", "One" } };
            AssertResult("One Piece - S01E13 (Trouble in Town).mkv", expected);
        }

        [Fact]
        public void AlreadyNamed_StarWarsCloneWars() {
            var expected = new MinedEpisode { EpisodeNumber = 1, SeasonNumber = 1, PossibleNames = new[] { "Star Wars The Clone Wars", "Star Wars The Clone" } };
            AssertResult("Star Wars The Clone Wars - S01E01.mkv", expected);
        }

        [Fact]
        public void InvalidArgumentsTest() {
            Assert.Throws<ArgumentNullException>(() => _miner.MineEpisodeName(null));
            Assert.Throws<ArgumentException>(() => _miner.MineEpisodeName(""));
            Assert.Throws<ArgumentException>(() => _miner.MineEpisodeName(" "));
        }

        [Fact]
        public void Junk_Mp4Extension() {
            var expected = new MinedEpisode { EpisodeNumber = 0, SeasonNumber = null, PossibleNames = new[] { "" } };
            AssertResult(".mp4", expected);
        }

        [Fact]
        public void Junk_RightAngular() {
            var expected = new MinedEpisode { EpisodeNumber = 0, SeasonNumber = null, PossibleNames = new[] { ">>>>>>>" } };
            AssertResult(">>>>>>>", expected);
        }

        [Fact]
        public void Junk_RightSquare() {
            var expected = new MinedEpisode { EpisodeNumber = 0, SeasonNumber = null, PossibleNames = new[] { "[" } };
            AssertResult("[", expected);
        }

        [Fact]
        public void Junk_RomanNumeral() {
            var expected = new MinedEpisode { EpisodeNumber = 0, SeasonNumber = 1, PossibleNames = new[] { " I " } };
            AssertResult(" I ", expected);
        }

        [Fact]
        public void Junk_SeasonNumber() {
            var expected = new MinedEpisode { EpisodeNumber = 1, SeasonNumber = null, PossibleNames = new[] { "s01" } };
            AssertResult("s01", expected);
        }

        [Fact]
        public void Junk_Special() {
            var expected = new MinedEpisode { EpisodeNumber = 0, SeasonNumber = 0, PossibleNames = new[] { "special" } };
            AssertResult(" special ", expected);
        }

        [Fact]
        public void PineappleSubs_Baruto() {
            var expected = new MinedEpisode { EpisodeNumber = 151, SeasonNumber = null, PossibleNames = new[] { "Boruto - Naruto Next Generations", "Boruto" } };
            AssertResult("[PineappleSubs] Boruto - Naruto Next Generations - 151 [1080p].mkv", expected);
        }

        [Fact]
        public void PineappleSubs_FruitBasket2019() {
            var expected = new MinedEpisode { EpisodeNumber = 1, SeasonNumber = 2, PossibleNames = new[] { "Fruits Basket S2 (2019)", "Fruits Basket (2019)", "Fruits Basket", "Fruits" } };
            AssertResult("[PineappleSubs] Fruits Basket S2 (2019) - 01 [720p].mkv", expected);
        }

        [Fact]
        public void PineappleSubs_Jashin() {
            var expected = new MinedEpisode { EpisodeNumber = 6, SeasonNumber = 2, PossibleNames = new[] { "Jashin-chan Dropkick S2", "Jashin-chan Dropkick", "Jashin" } };
            AssertResult("[PineappleSubs] Jashin-chan Dropkick S2 - 06 [1080p].mkv", expected);
        }

        [Fact]
        public void PineappleSubs_K() {
            var expected = new MinedEpisode { EpisodeNumber = 3, SeasonNumber = null, PossibleNames = new[] { "K - Seven Stories" } };
            AssertResult("[PineappleSubs] K - Seven Stories - 03 [1080p].mkv", expected);
        }

        [Fact]
        public void PineappleSubs_OnePiece() {
            var expected = new MinedEpisode { EpisodeNumber = 19, SeasonNumber = 1, PossibleNames = new[] { "One Piece S01", "One Piece", "One" } };
            AssertResult("[PineappleSubs] One Piece S01 - 19 [720p].mkv", expected);
        }

        [Fact]
        public void PineappleSubs_Pet() {
            var expected = new MinedEpisode { EpisodeNumber = 13, SeasonNumber = null, PossibleNames = new[] { "Pet" } };
            AssertResult("[PineappleSubs] Pet - 13 [1080p].mkv", expected);
        }

        [Fact]
        public void PineappleSubs_ToaruKagakuNoRailgun() {
            var expected = new MinedEpisode { EpisodeNumber = 13, SeasonNumber = null, PossibleNames = new[] { "Toaru Kagaku no Railgun T", "Toaru Kagaku no Railgun" } };
            AssertResult("[HorribleSubs] Toaru Kagaku no Railgun T - 13 [1080p].mkv", expected);
        }

        [Fact]
        public void Standard_NoMetadata() {
            var expected = new MinedEpisode { EpisodeNumber = 1, SeasonNumber = null, PossibleNames = new[] { "fairy tail", "fairy" } };
            AssertResult("fairy tail - 01.webm", expected);
        }

        private void AssertResult(string episodeName, MinedEpisode expectedResult) {
            var result = _miner.MineEpisodeName(episodeName);

            Assert.Equal(expectedResult.SeasonNumber, result.SeasonNumber);
            Assert.Equal(expectedResult.EpisodeNumber, result.EpisodeNumber);

            Assert.Equal(expectedResult.PossibleNames.Length, result.PossibleNames.Length);
            for (int i = 0; i < result.PossibleNames.Length; i++)
                Assert.Equal(expectedResult.PossibleNames[i], result.PossibleNames[i]);
        }
    }
}