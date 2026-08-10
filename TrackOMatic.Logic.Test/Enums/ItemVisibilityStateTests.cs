using TrackOMatic.Logic.Enums;
using System.Text.Json;

namespace TrackOMatic.Logic.Test.Enums;

public class ItemVisibilityStateTests
{
    [Fact]
    public void ItemVisibilityState_HasThreeValues()
    {
        // Arrange & Act
        var values = Enum.GetValues(typeof(ItemVisibilityState));

        // Assert
        Assert.Equal(3, values.Length);
    }

    [Fact]
    public void ItemVisibilityState_Visible_HasCorrectValue()
    {
        // Act & Assert
        Assert.Equal(0, (int)ItemVisibilityState.Visible);
    }

    [Fact]
    public void ItemVisibilityState_Hidden_HasCorrectValue()
    {
        // Act & Assert
        Assert.Equal(1, (int)ItemVisibilityState.Hidden);
    }

    [Fact]
    public void ItemVisibilityState_Collapsed_HasCorrectValue()
    {
        // Act & Assert
        Assert.Equal(2, (int)ItemVisibilityState.Collapsed);
    }

    [Theory]
    [InlineData(0, ItemVisibilityState.Visible)]
    [InlineData(1, ItemVisibilityState.Hidden)]
    [InlineData(2, ItemVisibilityState.Collapsed)]
    public void ItemVisibilityState_CastFromInt_ReturnsCorrectEnumValue(int intValue, ItemVisibilityState expected)
    {
        // Act & Assert
        Assert.Equal(expected, (ItemVisibilityState)intValue);
    }

    [Fact]
    public void ItemVisibilityState_JsonSerialization_PreservesEnumAsInteger()
    {
        // Arrange
        var state = ItemVisibilityState.Hidden;

        // Act
        var json = JsonSerializer.Serialize(state);

        // Assert
        // System.Text.Json serializes enums as integers by default
        Assert.Equal("1", json.Trim());  // Hidden is 1
    }

    [Fact]
    public void ItemVisibilityState_JsonDeserialization_RestoresEnumFromInteger()
    {
        // Arrange
        var json = "2";  // Collapsed

        // Act
        var deserialized = JsonSerializer.Deserialize<ItemVisibilityState>(json);

        // Assert
        Assert.Equal(ItemVisibilityState.Collapsed, deserialized);
    }

    [Theory]
    [InlineData(ItemVisibilityState.Visible)]
    [InlineData(ItemVisibilityState.Hidden)]
    [InlineData(ItemVisibilityState.Collapsed)]
    public void ItemVisibilityState_RoundTrip_PreservesValue(ItemVisibilityState original)
    {
        // Act
        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<ItemVisibilityState>(json);

        // Assert
        Assert.Equal(original, restored);
    }

    [Fact]
    public void ItemVisibilityState_ToString_ReturnsEnumName()
    {
        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.Equal("Visible", ItemVisibilityState.Visible.ToString());
            Assert.Equal("Hidden", ItemVisibilityState.Hidden.ToString());
            Assert.Equal("Collapsed", ItemVisibilityState.Collapsed.ToString());
        });
    }

    [Theory]
    [InlineData("Visible", ItemVisibilityState.Visible)]
    [InlineData("Hidden", ItemVisibilityState.Hidden)]
    [InlineData("Collapsed", ItemVisibilityState.Collapsed)]
    public void ItemVisibilityState_Parse_ReturnsCorrectEnumValue(string name, ItemVisibilityState expected)
    {
        // Act
        var parsed = Enum.Parse<ItemVisibilityState>(name);

        // Assert
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public void ItemVisibilityState_IsDefined_ForAllThreeValues()
    {
        // Assert — Verify all three enum members exist
        Assert.Multiple(() =>
        {
            Assert.True(Enum.IsDefined(ItemVisibilityState.Visible));
            Assert.True(Enum.IsDefined(ItemVisibilityState.Hidden));
            Assert.True(Enum.IsDefined(ItemVisibilityState.Collapsed));
        });
    }

    [Fact]
    public void ItemVisibilityState_NotEqual_DifferentValues()
    {
        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotEqual(ItemVisibilityState.Visible, ItemVisibilityState.Hidden);
            Assert.NotEqual(ItemVisibilityState.Hidden, ItemVisibilityState.Collapsed);
            Assert.NotEqual(ItemVisibilityState.Visible, ItemVisibilityState.Collapsed);
        });
    }

    [Fact]
    public void ItemVisibilityState_Equality_SameEnumValue()
    {
        // Arrange
        var state1 = ItemVisibilityState.Hidden;
        var state2 = ItemVisibilityState.Hidden;

        // Assert
        Assert.Equal(state1, state2);
    }
}
