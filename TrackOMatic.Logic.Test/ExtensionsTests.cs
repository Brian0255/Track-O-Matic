using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Test;

public class ExtensionsTests
{
    #region ToHintDisplayMode Tests

    [Fact]
    public void ToHintDisplayMode_WithOffEnumName_ReturnsOff()
    {
        // Act
        var result = "Off".FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.Equal(HintDisplayMode.Off, result);
    }

    [Fact]
    public void ToHintDisplayMode_WithMultipathHintsEnumName_ReturnsMultipathHints()
    {
        // Act
        var result = "MultipathHints".FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.Equal(HintDisplayMode.MultipathHints, result);
    }

    [Fact]
    public void ToHintDisplayMode_WithDirectItemHintsEnumName_ReturnsDirectItemHints()
    {
        // Act
        var result = "DirectItemHints".FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.Equal(HintDisplayMode.DirectItemHints, result);
    }

    [Fact]
    public void ToHintDisplayMode_WithOffLegacyName_ReturnsOff()
    {
        // Act
        var result = "Off".FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.Equal(HintDisplayMode.Off, result);
    }

    [Fact]
    public void ToHintDisplayMode_WithMultipathHintsLegacyName_ReturnsMultipathHints()
    {
        // Act
        var result = "Multipath Hints".FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.Equal(HintDisplayMode.MultipathHints, result);
    }

    [Fact]
    public void ToHintDisplayMode_WithDirectItemHintsLegacyName_ReturnsDirectItemHints()
    {
        // Act
        var result = "Direct Item Hints".FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.Equal(HintDisplayMode.DirectItemHints, result);
    }

    [Fact]
    public void ToHintDisplayMode_CaseInsensitive_ReturnsCorrectValue()
    {
        // Act
        var result1 = "off".FromLegacyToEnum<HintDisplayMode>();
        var result2 = "OFF".FromLegacyToEnum<HintDisplayMode>();
        var result3 = "oFf".FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(HintDisplayMode.Off, result1);
            Assert.Equal(HintDisplayMode.Off, result2);
            Assert.Equal(HintDisplayMode.Off, result3);
        });
    }

    [Fact]
    public void ToHintDisplayMode_WithInvalidString_ReturnsOffAsDefault()
    {
        // Act
        var result = "InvalidMode".FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.Equal(HintDisplayMode.Off, result);
    }

    [Fact]
    public void ToHintDisplayMode_WithEmptyString_ReturnsOffAsDefault()
    {
        // Act
        var result = "".FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.Equal(HintDisplayMode.Off, result);
    }

    [Fact]
    public void ToHintDisplayMode_WithNullString_ReturnsOffAsDefault()
    {
        // Act
        var result = ((string)null!).FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.Equal(HintDisplayMode.Off, result);
    }

    [Theory]
    [InlineData("Off")]
    [InlineData("Multipath Hints")]
    [InlineData("Direct Item Hints")]
    public void ToHintDisplayMode_WithAllValidLegacyNames_ReturnsValidEnum(string legacyName)
    {
        // Act
        var result = legacyName.FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.True(Enum.IsDefined(result));
    }

    [Fact]
    public void ToHintDisplayMode_RoundTrip_WithEnumName_PreservesValue()
    {
        // Arrange
        var original = HintDisplayMode.DirectItemHints;

        // Act
        var stringValue = original.ToString();
        var restored = stringValue.FromLegacyToEnum<HintDisplayMode>();

        // Assert
        Assert.Equal(original, restored);
    }

    #endregion

    #region ShouldKeepSavedItem Tests

    [Fact]
    public void ShouldKeepSavedItem_WithVisibleStarred_ReturnsTrue()
    {
        // Arrange
        var item = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            true,
            false,
            0.0
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_WithHiddenNotStarred_ReturnsFalse()
    {
        // Arrange
        var item = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            false,
            false,
            0.0
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_WithOpacity0375InRegion_ReturnsTrue()
    {
        // Arrange - Right-click drag hint (opacity 0.375 in a region)
        var item = new SavedItem(
            ItemName.DIDDY,
            RegionName.ANGRY_AZTEC,
            false,
            false,
            0.375
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_WithOpacity0375InUnknownRegion_ReturnsFalse()
    {
        // Arrange - Right-click drag hint in UNKNOWN region should not be kept
        var item = new SavedItem(
            ItemName.DIDDY,
            RegionName.UNKNOWN,
            false,
            false,
            0.375
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_WithOpacityGreaterThan1InRegion_ReturnsTrue()
    {
        // Arrange - Found item (opacity >= 1.0, not autotracked)
        var item = new SavedItem(
            ItemName.LANKY,
            RegionName.GLOOMY_GALLEON,
            false,
            false,
            1.0
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_WithAutoTrackedItem_ReturnsTrue()
    {
        // Arrange - Autotracked item (opacity >= 1.0, autotracked=true)
        var item = new SavedItem(
            ItemName.TINY,
            RegionName.CRYSTAL_CAVES,
            false,
            true,
            1.0
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_WithAutoTrackedItemHighOpacity_ReturnsTrue()
    {
        // Arrange
        var item = new SavedItem(
            ItemName.CHUNKY,
            RegionName.FRANTIC_FACTORY,
            false,
            true,
            1.5
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_WithOpacityGreaterThan1InUnknownRegion_ReturnsFalse()
    {
        // Arrange - Found item in UNKNOWN region should not be kept
        var item = new SavedItem(
            ItemName.LANKY,
            RegionName.UNKNOWN,
            false,
            false,
            1.0
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_WithLowOpacityNotHint_ReturnsFalse()
    {
        // Arrange - Opacity 0.5 is not a valid hint marker (0.375) or found item (>= 1.0)
        var item = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            false,
            false,
            0.5
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_WithOpacityAlmostZero375_ReturnsTrue()
    {
        // Arrange - Test tolerance around 0.375 (within 0.01)
        var item = new SavedItem(
            ItemName.DONKEY,
            RegionName.SHOPS,
            false,
            false,
            0.380  // Within 0.01 of 0.375
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_WithOpacityTolerance_CircaZero375_ReturnsTrue()
    {
        // Arrange - Test lower tolerance bound
        var item = new SavedItem(
            ItemName.DONKEY,
            RegionName.SHOPS,
            false,
            false,
            0.370
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_WithOpacityOutsideTolerance_ReturnsFalse()
    {
        // Arrange - opacity 0.36 is outside the 0.01 tolerance from 0.375
        var item = new SavedItem(
            ItemName.DONKEY,
            RegionName.SHOPS,
            false,
            false,
            0.36
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    [InlineData(100.0)]
    public void ShouldKeepSavedItem_WithVariousOpacitiesGreaterThan1_ReturnsTrue(double opacity)
    {
        // Arrange - Found or autotracked items have opacity >= 1.0
        var item = new SavedItem(
            ItemName.DONKEY,
            RegionName.SHOPS,
            false,
            false,
            opacity
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_StarredItemWithAnyOpacity_ReturnsTrue()
    {
        // Arrange - Starred items are always kept, regardless of opacity
        var item = new SavedItem(
            ItemName.DONKEY,
            RegionName.DK_ISLES,
            true,
            false,
            0.1  // Low opacity, but starred
        );

        // Act
        var result = item.ShouldKeepSavedItem();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldKeepSavedItem_MultipleItemsFollowRules()
    {
        // Arrange - Multiple items to test the rules
        SavedItem[] items =
        [
            new(ItemName.DONKEY, RegionName.JUNGLE_JAPES, true, false, 0.0), // Starred
            new(ItemName.DIDDY, RegionName.ANGRY_AZTEC, false, false, 0.375), // Hint (0.375)
            new(ItemName.LANKY, RegionName.GLOOMY_GALLEON, false, false, 1.0), // Found
            new(ItemName.TINY, RegionName.CRYSTAL_CAVES, false, true, 1.0), // Autotracked
            new(ItemName.CHUNKY, RegionName.FRANTIC_FACTORY, false, false, 0.5), // Low opacity - remove
        ];

        // Act
        var results = items.Select(item => item.ShouldKeepSavedItem()).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(results[0], "Starred item should be kept");
            Assert.True(results[1], "Hint item (0.375 opacity) should be kept");
            Assert.True(results[2], "Found item (opacity 1.0) should be kept");
            Assert.True(results[3], "Autotracked item should be kept");
            Assert.False(results[4], "Item with low opacity (0.5) should not be kept");
        });
    }

    #endregion
}
