using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Test
{
    public class SongFormatterTests
    {
        [Theory]
        [InlineData("dkc", "DKC")]
        [InlineData("dk", "DK")]
        [InlineData("pac-man", "Pac-Man")]
        public void FormatSongString_AppliesSingleWordReplacements(string input, string expected)
        {
            var result = SongFormatter.FormatSongString(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("live a live", "Live A Live")]
        [InlineData("nights into dreams...", "NiGHTS into Dreams...")]
        [InlineData("rise of the triad", "Rise Of The Triad")]
        public void FormatSongString_AppliesEntireNameReplacements(string input, string expected)
        {
            var result = SongFormatter.FormatSongString(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("donkey kong country ii", "Donkey Kong Country II")]
        [InlineData("final fantasy iv", "Final Fantasy IV")]
        [InlineData("metroid", "Metroid")]
        public void FormatSongString_AppliesTitleCase(string input, string expected)
        {
            var result = SongFormatter.FormatSongString(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatSongString_HandlesNullSafely()
        {
            // Null should throw, or we document the expected behavior
            // Adjust this test based on actual implementation expectations
            Assert.Throws<NullReferenceException>(() => SongFormatter.FormatSongString(null!));
        }

        [Fact]
        public void FormatSongString_HandlesEmptyString()
        {
            var result = SongFormatter.FormatSongString("");
            Assert.Equal("", result);
        }

        [Theory]
        [InlineData("UPPERCASE", "Uppercase")]
        [InlineData("lowercase", "Lowercase")]
        [InlineData("MiXeD cAsE", "Mixed Case")]
        public void FormatSongString_NormalizesCase(string input, string expected)
        {
            var result = SongFormatter.FormatSongString(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("donkey kong country i", "Donkey Kong Country I")]
        [InlineData("final fantasy v", "Final Fantasy V")]
        [InlineData("castlevania iv", "Castlevania IV")]
        public void FormatSongString_AppliesRomanNumeralReplacements(string input, string expected)
        {
            var result = SongFormatter.FormatSongString(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("yu-gi-oh!", "Yu-Gi-Oh!")]
        [InlineData("earthbound", "Earthbound")]
        [InlineData("super mario rpg", "Super Mario RPG")]
        public void FormatSongString_AppliesSpecificGameReplacements(string input, string expected)
        {
            var result = SongFormatter.FormatSongString(input);
            Assert.Equal(expected, result);
        }
    }
}
