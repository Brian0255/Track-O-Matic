using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Test;

/// <summary>
/// Unit tests for region enum label mapping extensions.
/// </summary>
public class RegionNameLabelTests
{
    #region GetSpoilerLogLabel Tests

    [Fact]
    public void GetSpoilerLogLabel_WithAttribute_ReturnsLabelValue()
    {
        // Arrange
        var region = RegionName.ANGRY_AZTEC;

        // Act
        var label = region.GetSpoilerLogLabel();

        // Assert
        Assert.Equal("Angry Aztec", label);
    }

    [Fact]
    public void GetSpoilerLogLabel_DKIsles_ReturnsCorrectLabel()
    {
        // Act
        var label = RegionName.DK_ISLES.GetSpoilerLogLabel();

        // Assert
        Assert.Equal("DK Isles", label);
    }

    [Fact]
    public void GetSpoilerLogLabel_AllLabeledRegions_HaveDistinctLabels()
    {
        // Arrange
        var regions = new[]
        {
            RegionName.DK_ISLES,
            RegionName.JUNGLE_JAPES,
            RegionName.ANGRY_AZTEC,
            RegionName.FRANTIC_FACTORY,
            RegionName.GLOOMY_GALLEON,
            RegionName.FUNGI_FOREST,
            RegionName.CRYSTAL_CAVES,
            RegionName.CREEPY_CASTLE,
            RegionName.HIDEOUT_HELM,
            RegionName.SHOPS
        };

        // Act
        var labels = regions.Select(r => r.GetSpoilerLogLabel()).ToList();

        // Assert
        Assert.Equal(labels.Count, labels.Distinct().Count());
    }

    [Fact]
    public void GetSpoilerLogLabel_WithoutAttribute_ReturnsEnumName()
    {
        // Act
        var label = RegionName.UNKNOWN.GetSpoilerLogLabel();

        // Assert
        Assert.Equal("UNKNOWN", label);
    }

    [Fact]
    public void GetSpoilerLogLabel_HideoutHelm_ReturnsCorrectSpelling()
    {
        // Act
        var label = RegionName.HIDEOUT_HELM.GetSpoilerLogLabel();

        // Assert
        Assert.Equal("Hideout Helm", label);
    }

    #endregion

    #region TryParseRegionFromLabel Tests

    [Fact]
    public void TryParseRegionFromLabel_WithValidLabel_ReturnsCorrectRegion()
    {
        // Act
        var region = Extensions.TryParseRegionFromLabel("Angry Aztec");

        // Assert
        Assert.Equal(RegionName.ANGRY_AZTEC, region);
    }

    [Fact]
    public void TryParseRegionFromLabel_WithDKIsles_ReturnsCorrectRegion()
    {
        // Act
        var region = Extensions.TryParseRegionFromLabel("DK Isles");

        // Assert
        Assert.Equal(RegionName.DK_ISLES, region);
    }

    [Fact]
    public void TryParseRegionFromLabel_WithAllRegions_RoundTripsCorrectly()
    {
        // Arrange
        var regions = new[]
        {
            RegionName.DK_ISLES,
            RegionName.JUNGLE_JAPES,
            RegionName.ANGRY_AZTEC,
            RegionName.FRANTIC_FACTORY,
            RegionName.GLOOMY_GALLEON,
            RegionName.FUNGI_FOREST,
            RegionName.CRYSTAL_CAVES,
            RegionName.CREEPY_CASTLE,
            RegionName.HIDEOUT_HELM,
            RegionName.SHOPS
        };

        // Act & Assert
        foreach (var originalRegion in regions)
        {
            var label = originalRegion.GetSpoilerLogLabel();
            var parsedRegion = Extensions.TryParseRegionFromLabel(label);
            Assert.Equal(originalRegion, parsedRegion);
        }
    }

    [Fact]
    public void TryParseRegionFromLabel_WithInvalidLabel_ReturnsUnknown()
    {
        // Act
        var region = Extensions.TryParseRegionFromLabel("Invalid Region");

        // Assert
        Assert.Equal(RegionName.UNKNOWN, region);
    }

    [Fact]
    public void TryParseRegionFromLabel_WithNullLabel_ReturnsUnknown()
    {
        // Act
        var region = Extensions.TryParseRegionFromLabel(null!);

        // Assert
        Assert.Equal(RegionName.UNKNOWN, region);
    }

    [Fact]
    public void TryParseRegionFromLabel_WithEmptyLabel_ReturnsUnknown()
    {
        // Act
        var region = Extensions.TryParseRegionFromLabel("");

        // Assert
        Assert.Equal(RegionName.UNKNOWN, region);
    }

    [Fact]
    public void TryParseRegionFromLabel_IsCaseSensitive_ReturnUnknownForDifferentCase()
    {
        // Act - lowercase version should NOT match
        var region = Extensions.TryParseRegionFromLabel("angry aztec");

        // Assert
        Assert.Equal(RegionName.UNKNOWN, region);
    }

    #endregion

    #region Integration Tests

    [Theory]
    [InlineData(RegionName.DK_ISLES, "DK Isles")]
    [InlineData(RegionName.JUNGLE_JAPES, "Jungle Japes")]
    [InlineData(RegionName.ANGRY_AZTEC, "Angry Aztec")]
    [InlineData(RegionName.FRANTIC_FACTORY, "Frantic Factory")]
    [InlineData(RegionName.GLOOMY_GALLEON, "Gloomy Galleon")]
    [InlineData(RegionName.FUNGI_FOREST, "Fungi Forest")]
    [InlineData(RegionName.CRYSTAL_CAVES, "Crystal Caves")]
    [InlineData(RegionName.CREEPY_CASTLE, "Creepy Castle")]
    [InlineData(RegionName.HIDEOUT_HELM, "Hideout Helm")]
    [InlineData(RegionName.SHOPS, "Shops")]
    public void RegionLabelMapping_AllCases_MapCorrectly(RegionName region, string expectedLabel)
    {
        // Act
        var actualLabel = region.GetSpoilerLogLabel();
        var parsedRegion = Extensions.TryParseRegionFromLabel(actualLabel);

        // Assert
        Assert.Equal(expectedLabel, actualLabel);
        Assert.Equal(region, parsedRegion);
    }

    #endregion
}
