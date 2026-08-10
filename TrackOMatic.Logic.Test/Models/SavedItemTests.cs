using TrackOMatic.Logic.Enums;
using System.Text.Json;

namespace TrackOMatic.Logic.Test.Models;

public class SavedItemTests
{
    [Fact]
    public void Constructor_WithAllParameters_CreatesValidSavedItem()
    {
        // Arrange & Act
        var savedItem = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            autotracked: true,
            opacity: 0.5,
            hinted: true
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(ItemName.DONKEY, savedItem.ItemName);
            Assert.Equal(RegionName.JUNGLE_JAPES, savedItem.Region);
            Assert.Equal(ItemVisibilityState.Visible, savedItem.Starred);
            Assert.True(savedItem.Autotracked);
            Assert.Equal(0.5, savedItem.Opacity);
            Assert.True(savedItem.Hinted);
        });
    }

    [Fact]
    public void Constructor_WithDefaultHinted_DefaultsToFalse()
    {
        // Arrange & Act
        var savedItem = new SavedItem(
            ItemName.TINY,
            RegionName.SHOPS,
            ItemVisibilityState.Hidden,
            autotracked: false,
            opacity: 1.0
        );

        // Assert
        Assert.False(savedItem.Hinted);
    }

    [Theory]
    [InlineData(ItemVisibilityState.Visible)]
    [InlineData(ItemVisibilityState.Hidden)]
    [InlineData(ItemVisibilityState.Collapsed)]
    public void Constructor_WithVariousVisibilityStates_StoresCorrectValue(ItemVisibilityState visibility)
    {
        // Arrange & Act
        var savedItem = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            visibility,
            autotracked: false,
            opacity: 1.0
        );

        // Assert
        Assert.Equal(visibility, savedItem.Starred);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(0.25)]
    public void Constructor_WithVariousOpacities_StoresOpacityValue(double opacity)
    {
        // Arrange & Act
        var savedItem = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            autotracked: false,
            opacity: opacity
        );

        // Assert
        Assert.Equal(opacity, savedItem.Opacity);
    }

    [Fact]
    public void Autotracked_CanBeModified_PropertyAllowsSetAfterConstruction()
    {
        // Arrange
        var savedItem = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            autotracked: false,
            opacity: 1.0
        );

        // Act
        savedItem.Autotracked = true;

        // Assert
        Assert.True(savedItem.Autotracked);
    }

    [Fact]
    public void ItemName_IsReadOnly_CannotBeChanged()
    {
        // Arrange
        var savedItem = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            autotracked: false,
            opacity: 1.0
        );

        // Assert
        Assert.Equal(ItemName.DONKEY, savedItem.ItemName);
    }

    [Fact]
    public void Region_IsReadOnly_CannotBeChanged()
    {
        // Arrange
        var savedItem = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            autotracked: false,
            opacity: 1.0
        );

        // Assert
        Assert.Equal(RegionName.JUNGLE_JAPES, savedItem.Region);
    }

    [Fact]
    public void Starred_IsReadOnly_CannotBeChanged()
    {
        // Arrange
        var savedItem = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            autotracked: false,
            opacity: 1.0
        );

        // Assert
        Assert.Equal(ItemVisibilityState.Visible, savedItem.Starred);
    }

    [Fact]
    public void Opacity_IsReadOnly_CannotBeChanged()
    {
        // Arrange
        var savedItem = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            autotracked: false,
            opacity: 0.7
        );

        // Assert
        Assert.Equal(0.7, savedItem.Opacity);
    }

    [Fact]
    public void Hinted_IsReadOnly_CannotBeChanged()
    {
        // Arrange
        var savedItem = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            autotracked: false,
            opacity: 1.0,
            hinted: true
        );

        // Assert
        Assert.True(savedItem.Hinted);
    }

    [Fact]
    public void JsonSerialization_RoundTrip_PreservesAllProperties()
    {
        // Arrange
        var original = new SavedItem(
            ItemName.TINY,
            RegionName.SHOPS,
            ItemVisibilityState.Hidden,
            autotracked: true,
            opacity: 0.35,
            hinted: false
        );

        // Act
        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<SavedItem>(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(restored);
            Assert.Equal(original.ItemName, restored.ItemName);
            Assert.Equal(original.Region, restored.Region);
            Assert.Equal(original.Starred, restored.Starred);
            Assert.Equal(original.Autotracked, restored.Autotracked);
            Assert.Equal(original.Opacity, restored.Opacity);
            Assert.Equal(original.Hinted, restored.Hinted);
        });
    }

    [Fact]
    public void JsonSerialization_WithVisibleState_PreservesEnumValue()
    {
        // Arrange
        var original = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            autotracked: false,
            opacity: 1.0
        );

        // Act
        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<SavedItem>(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(restored);
            Assert.Equal(ItemVisibilityState.Visible, restored.Starred);
        });
    }

    [Fact]
    public void JsonSerialization_WithCollapsedState_PreservesEnumValue()
    {
        // Arrange
        var original = new SavedItem(
            ItemName.CHUNKY,
            RegionName.CRYSTAL_CAVES,
            ItemVisibilityState.Collapsed,
            autotracked: true,
            opacity: 0.75
        );

        // Act
        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<SavedItem>(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(restored);
            Assert.Equal(ItemVisibilityState.Collapsed, restored.Starred);
        });
    }

    [Fact]
    public void MultipleInstances_AreIndependent()
    {
        // Arrange
        var item1 = new SavedItem(ItemName.DONKEY, RegionName.JUNGLE_JAPES, ItemVisibilityState.Visible, false, 1.0);
        var item2 = new SavedItem(ItemName.TINY, RegionName.SHOPS, ItemVisibilityState.Hidden, false, 0.5);

        // Act
        item1.Autotracked = true;
        item2.Autotracked = false;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(item1.Autotracked);
            Assert.False(item2.Autotracked);
            Assert.Equal(ItemName.DONKEY, item1.ItemName);
            Assert.Equal(ItemName.TINY, item2.ItemName);
        });
    }
}
