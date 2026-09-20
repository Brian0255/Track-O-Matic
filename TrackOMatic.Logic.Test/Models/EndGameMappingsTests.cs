using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Test.Models;

public class EndGameMappingsTests
{
    [Theory]
    [InlineData(1, "donkey")]
    [InlineData(2, "diddy")]
    [InlineData(3, "lanky")]
    [InlineData(4, "tiny")]
    [InlineData(5, "chunky")]
    [InlineData(6, "army")]
    [InlineData(7, "doga")]
    [InlineData(8, "madjack")]
    [InlineData(9, "pufftoss")]
    [InlineData(10, "doga2")]
    [InlineData(11, "army2")]
    [InlineData(12, "kutout")]
    public void EndGameMappings_EndGameIndexToImageResource_ValidIndices_ReturnsCorrectImageId(int index, string expectedResource)
    {
        // Act
        var result = EndGameMappings.EndGameIndexToImageResource(index);

        // Assert
        Assert.Equal(expectedResource, result);
    }

    [Theory]
    [InlineData(999)]
    [InlineData(-1)]
    [InlineData(0)]
    public void EndGameMappings_EndGameIndexToImageResource_InvalidIndex_ReturnsUnknownKong(int index)
    {
        // Act
        var result = EndGameMappings.EndGameIndexToImageResource(index);
        // Assert
        Assert.Equal("unknown_kong_bw", result);
    }

    [Theory]
    [InlineData(Bosses.DONKEY, "donkey")]
    [InlineData(Bosses.DIDDY, "diddy")]
    [InlineData(Bosses.LANKY, "lanky")]
    [InlineData(Bosses.TINY, "tiny")]
    [InlineData(Bosses.CHUNKY, "chunky")]
    public void EndGameMappings_EndGameIndexToImageResource_WithBossesEnum_ReturnsCorrectImageId(Bosses boss, string expectedResource)
    {
        // Act
        var result = EndGameMappings.EndGameIndexToImageResource(boss);

        // Assert
        Assert.Equal(expectedResource, result);
    }

    [Theory]
    [InlineData(1, "blueprint")]
    [InlineData(2, "pearl")]
    [InlineData(3, "crown")]
    [InlineData(4, "medal")]
    [InlineData(5, "rainbow_coin")]
    [InlineData(6, "fairy")]
    [InlineData(7, "company_coin")]
    [InlineData(8, "bean")]
    public void EndGameMappings_DoorIndexToImageResource_ValidIndices_ReturnsCorrectImageId(int index, string expectedResource)
    {
        // Act
        var result = EndGameMappings.DoorIndexToImageResource(index);

        // Assert
        Assert.Equal(expectedResource, result);
    }

    [Fact]
    public void EndGameMappings_DoorIndexToImageResource_UnknownIndex_ReturnsGoldenBanana()
    {
        // Act
        var result = EndGameMappings.DoorIndexToImageResource(999);

        // Assert
        Assert.Equal("golden_banana", result);
    }

    [Fact]
    public void EndGameMappings_DoorIndexToImageResource_NegativeIndex_ReturnsGoldenBanana()
    {
        // Act
        var result = EndGameMappings.DoorIndexToImageResource(-1);

        // Assert
        Assert.Equal("golden_banana", result);
    }

    [Fact]
    public void EndGameMappings_DoorIndexToImageResource_ZeroIndex_ReturnsGoldenBanana()
    {
        // Act
        var result = EndGameMappings.DoorIndexToImageResource(0);

        // Assert
        Assert.Equal("golden_banana", result);
    }

    [Theory]
    [InlineData(BarrierItems.BLUEPRINT, "blueprint")]
    [InlineData(BarrierItems.PEARL, "pearl")]
    [InlineData(BarrierItems.CROWN, "crown")]
    [InlineData(BarrierItems.MEDAL, "medal")]
    [InlineData(BarrierItems.RAINBOW_COIN, "rainbow_coin")]
    [InlineData(BarrierItems.FAIRY, "fairy")]
    [InlineData(BarrierItems.COMPANY_COIN, "company_coin")]
    [InlineData(BarrierItems.BEAN, "bean")]
    public void EndGameMappings_DoorIndexToImageResource_WithBarrierItemsEnum_ReturnsCorrectImageId(BarrierItems item, string expectedResource)
    {
        // Act
        var result = EndGameMappings.DoorIndexToImageResource(item);

        // Assert
        Assert.Equal(expectedResource, result);
    }

    [Fact]
    public void EndGameMappings_AllKongIndices_MapToDistinctResources()
    {
        // Arrange
        var indices = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        // Act
        var resources = indices.Select(i => EndGameMappings.EndGameIndexToImageResource(i)).ToList();

        // Assert
        Assert.Equal(resources.Count, resources.Distinct().Count());
    }

    [Fact]
    public void EndGameMappings_AllDoorIndices_MapToDistinctResources()
    {
        // Arrange
        var indices = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        // Act
        var resources = indices.Select(i => EndGameMappings.DoorIndexToImageResource(i)).ToList();

        // Assert
        Assert.Equal(resources.Count, resources.Distinct().Count());
    }

    [Fact]
    public void EndGameMappings_DoorAndKongMappings_AreIndependent()
    {
        // Act
        var doorBlueprintResource = EndGameMappings.DoorIndexToImageResource(1);
        var kongDonkeyResource = EndGameMappings.EndGameIndexToImageResource(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotEqual(doorBlueprintResource, kongDonkeyResource);
            Assert.Equal("blueprint", doorBlueprintResource);
            Assert.Equal("donkey", kongDonkeyResource);
        });
    }
}
