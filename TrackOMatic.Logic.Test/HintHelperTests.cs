using Xunit;
using TrackOMatic.Logic.Helpers;

namespace TrackOMatic.Logic.Test
{
    public class HintHelperTests
    {
        [Fact]
        public void GenerateThresholds_WithHintCap100_CreatesThresholds()
        {
            var thresholds = HintHelper.GenerateThresholds(100);
            Assert.NotNull(thresholds);
            Assert.NotEmpty(thresholds);
        }

        [Fact]
        public void GenerateThresholds_WithHintCap100_EndsWithHintCap()
        {
            var thresholds = HintHelper.GenerateThresholds(100);
            Assert.Equal(100, thresholds[thresholds.Count - 1]);
        }

        [Fact]
        public void GenerateThresholds_WithHintCap50_CreatesThresholds()
        {
            var thresholds = HintHelper.GenerateThresholds(50);
            Assert.NotEmpty(thresholds);
            Assert.Equal(50, thresholds[thresholds.Count - 1]);
        }

        [Fact]
        public void GenerateThresholds_WithDifferentCaps_ProducesDifferentThresholds()
        {
            var thresholds50 = HintHelper.GenerateThresholds(50);
            var thresholds100 = HintHelper.GenerateThresholds(100);

            Assert.NotEqual(thresholds50, thresholds100);
        }

        [Fact]
        public void GetAmountToNextHint_ReturnsZeroWhenAtOrPastAllThresholds()
        {
            var thresholds = HintHelper.GenerateThresholds(50);
            var amount = HintHelper.GetAmountToNextHint(50, thresholds);
            Assert.Equal(0, amount);
        }

        [Fact]
        public void GetAmountToNextHint_ReturnsPositiveAmountWhenBelowThreshold()
        {
            var thresholds = HintHelper.GenerateThresholds(100);
            var amount = HintHelper.GetAmountToNextHint(10, thresholds);
            Assert.True(amount > 0);
        }

        [Theory]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(75)]
        public void GetAmountToNextHint_ReturnsNonNegativeForVariousItemCounts(int itemCount)
        {
            var thresholds = HintHelper.GenerateThresholds(100);
            var amount = HintHelper.GetAmountToNextHint(itemCount, thresholds);
            Assert.True(amount >= 0);
        }

        [Fact]
        public void GetAmountToNextHint_MonotonicBehavior()
        {
            var thresholds = HintHelper.GenerateThresholds(100);
            var amount0 = HintHelper.GetAmountToNextHint(0, thresholds);
            var amount50 = HintHelper.GetAmountToNextHint(50, thresholds);
            var amount100 = HintHelper.GetAmountToNextHint(100, thresholds);

            // At max threshold, should return 0
            Assert.Equal(0, amount100);
            // Before max, should be positive
            Assert.True(amount0 > 0);
            Assert.True(amount50 >= 0);
        }

        [Fact]
        public void GetAmountToNextHint_AtZeroItems_ReturnsFirstThreshold()
        {
            var thresholds = HintHelper.GenerateThresholds(100);
            var amount = HintHelper.GetAmountToNextHint(0, thresholds);

            // Amount to next hint from 0 should equal the first threshold
            Assert.Equal(thresholds[0], amount);
        }
    }
}
