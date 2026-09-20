using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Test.Enums;

public class HintPanelTypeTests
{
    [Fact]
    public void HintPanelType_HasExpectedNumberOfValues()
    {
        // Arrange & Act
        var values = Enum.GetValues<HintPanelType>();

        // Assert - 9 region panels (Isles-Helm) + 7 special panels (Paths, Foolish, Kongs, WayOfTheHoard, PotionCounts, Unhinted) = 15 total
        Assert.Equal(15, values.Length);
    }

    [Theory]
    [InlineData(HintPanelType.Isles, 0)]
    [InlineData(HintPanelType.Aztec, 1)]
    [InlineData(HintPanelType.Galleon, 2)]
    [InlineData(HintPanelType.Caves, 3)]
    [InlineData(HintPanelType.Japes, 4)]
    [InlineData(HintPanelType.Factory, 5)]
    [InlineData(HintPanelType.Forest, 6)]
    [InlineData(HintPanelType.Castle, 7)]
    [InlineData(HintPanelType.Helm, 8)]
    public void HintPanelType_RegionPanels_HaveCorrectValues(HintPanelType panel, int expectedValue)
    {
        // Act & Assert
        Assert.Equal(expectedValue, (int)panel);
    }

    [Theory]
    [InlineData(HintPanelType.Paths, 9)]
    [InlineData(HintPanelType.Foolish, 10)]
    [InlineData(HintPanelType.Kongs, 11)]
    [InlineData(HintPanelType.WayOfTheHoard, 12)]
    [InlineData(HintPanelType.PotionCounts, 13)]
    [InlineData(HintPanelType.Unhinted, 14)]
    public void HintPanelType_SpecialPanels_HaveCorrectValues(HintPanelType panel, int expectedValue)
    {
        // Act & Assert
        Assert.Equal(expectedValue, (int)panel);
    }

    [Theory]
    [InlineData(0, HintPanelType.Isles)]
    [InlineData(1, HintPanelType.Aztec)]
    [InlineData(2, HintPanelType.Galleon)]
    [InlineData(3, HintPanelType.Caves)]
    [InlineData(4, HintPanelType.Japes)]
    [InlineData(5, HintPanelType.Factory)]
    [InlineData(6, HintPanelType.Forest)]
    [InlineData(7, HintPanelType.Castle)]
    [InlineData(8, HintPanelType.Helm)]
    [InlineData(9, HintPanelType.Paths)]
    [InlineData(10, HintPanelType.Foolish)]
    [InlineData(11, HintPanelType.Kongs)]
    [InlineData(12, HintPanelType.WayOfTheHoard)]
    [InlineData(13, HintPanelType.PotionCounts)]
    [InlineData(14, HintPanelType.Unhinted)]
    public void HintPanelType_CastFromInt_ReturnsCorrectEnumValue(int intValue, HintPanelType expected)
    {
        // Act & Assert
        Assert.Equal(expected, (HintPanelType)intValue);
    }

    [Fact]
    public void HintPanelType_RegionPanelsAreConsecutive()
    {
        // Arrange & Act
        int islesValue = (int)HintPanelType.Isles;
        int helmValue = (int)HintPanelType.Helm;

        // Assert - Verify the region panels form a continuous sequence 0-8
        Assert.Multiple(() =>
        {
            Assert.Equal(0, islesValue);
            Assert.Equal(8, helmValue);
        });
    }

    [Fact]
    public void HintPanelType_AllValuesAreUniqueAndNonNegative()
    {
        // Arrange & Act
        var values = Enum.GetValues<HintPanelType>().ToList();
        var intValues = values.Select(v => (int)v).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.All(intValues, v => Assert.True(v >= 0, $"Enum value {v} is negative"));
            Assert.Equal(intValues.Count, intValues.Distinct().Count());
        });
    }
}
