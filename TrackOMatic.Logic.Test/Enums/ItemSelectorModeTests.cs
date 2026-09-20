using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Test.Enums;

public class ItemSelectorModeTests
{
    [Fact]
    public void ItemSelectorMode_HasThreeValues()
    {
        // Arrange & Act
        var values = Enum.GetValues(typeof(ItemSelectorMode));

        // Assert - Door, Helm, Boss
        Assert.Equal(3, values.Length);
    }

    [Fact]
    public void ItemSelectorMode_Door_HasCorrectValue()
    {
        // Act & Assert
        Assert.Equal(0, (int)ItemSelectorMode.Door);
    }

    [Fact]
    public void ItemSelectorMode_Helm_HasCorrectValue()
    {
        // Act & Assert
        Assert.Equal(1, (int)ItemSelectorMode.Helm);
    }

    [Fact]
    public void ItemSelectorMode_Boss_HasCorrectValue()
    {
        // Act & Assert
        Assert.Equal(2, (int)ItemSelectorMode.Boss);
    }

    [Theory]
    [InlineData(0, ItemSelectorMode.Door)]
    [InlineData(1, ItemSelectorMode.Helm)]
    [InlineData(2, ItemSelectorMode.Boss)]
    public void ItemSelectorMode_CastFromInt_ReturnsCorrectEnumValue(int intValue, ItemSelectorMode expected)
    {
        // Act & Assert
        Assert.Equal(expected, (ItemSelectorMode)intValue);
    }

    [Theory]
    [InlineData(ItemSelectorMode.Door)]
    [InlineData(ItemSelectorMode.Helm)]
    [InlineData(ItemSelectorMode.Boss)]
    public void ItemSelectorMode_AllValuesCanBeCreated(ItemSelectorMode mode)
    {
        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.IsType<ItemSelectorMode>(mode);
            Assert.True(Enum.IsDefined(mode));
        });
    }

    [Fact]
    public void ItemSelectorMode_AllValuesAreSequential()
    {
        // Arrange & Act
        var expected = new[] { 0, 1, 2 };
        var values = Enum.GetValues<ItemSelectorMode>().Cast<int>().OrderBy(v => v).ToList();

        // Assert
        Assert.Equal(expected, values);
    }
}
