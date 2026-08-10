using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using System.Text.Json;

namespace TrackOMatic.Logic.Test.Serialization;

/// <summary>
/// Verifies that enum-keyed dictionaries serialize correctly with System.Text.Json
/// to enable future removal of Newtonsoft.Json dependency.
/// </summary>
public class EnumKeyedDictionarySerializationTests
{
    [Fact]
    public void Dictionary_ItemName_Keys_SerializeAsStrings()
    {
        // Arrange
        var dict = new Dictionary<ItemName, string>
        {
            { ItemName.DONKEY, "value1" },
            { ItemName.TINY, "value2" }
        };

        // Act
        var json = JsonSerializer.Serialize(dict);

        // Assert — Should contain enum names as string keys, not numeric values
        Assert.Contains("DONKEY", json);
        Assert.Contains("TINY", json);
        Assert.Contains("value1", json);
        Assert.Contains("value2", json);
    }

    [Fact]
    public void Dictionary_RegionName_Keys_SerializeAsStrings()
    {
        // Arrange
        var dict = new Dictionary<RegionName, int>
        {
            { RegionName.JUNGLE_JAPES, 20 },
            { RegionName.SHOPS, 5 }
        };

        // Act
        var json = JsonSerializer.Serialize(dict);

        // Assert
        Assert.Contains("JUNGLE_JAPES", json);
        Assert.Contains("SHOPS", json);
        Assert.Contains("20", json);
        Assert.Contains("5", json);
    }

    [Fact]
    public void Dictionary_ItemName_Keys_DeserializeCorrectly()
    {
        // Arrange
        var original = new Dictionary<ItemName, SavedItem>
        {
            { ItemName.DONKEY, new SavedItem(ItemName.DONKEY, RegionName.JUNGLE_JAPES, ItemVisibilityState.Visible, false, 1.0) },
            { ItemName.TINY, new SavedItem(ItemName.TINY, RegionName.SHOPS, ItemVisibilityState.Hidden, false, 0.5) }
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<Dictionary<ItemName, SavedItem>>(json);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(2, restored.Count);
        Assert.True(restored.ContainsKey(ItemName.DONKEY));
        Assert.True(restored.ContainsKey(ItemName.TINY));
        Assert.Equal(ItemVisibilityState.Visible, restored[ItemName.DONKEY].Starred);
        Assert.Equal(ItemVisibilityState.Hidden, restored[ItemName.TINY].Starred);
    }

    [Fact]
    public void SavedProgress_WithStringKeyedDictionaries_SerializesCorrectly()
    {
        // Arrange
        var original = new SavedProgress();
        original.SavedGBCounts[RegionName.JUNGLE_JAPES] = "15/20";
        original.BLockerImageIndexes[RegionName.SHOPS] = 3;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };

        // Act
        var json = JsonSerializer.Serialize(original, options);
        var restored = JsonSerializer.Deserialize<SavedProgress>(json, options);

        // Assert — Simple value dictionaries work fine with System.Text.Json
        Assert.NotNull(restored);
        Assert.Equal("15/20", restored.SavedGBCounts[RegionName.JUNGLE_JAPES]);
        Assert.Equal(3, restored.BLockerImageIndexes[RegionName.SHOPS]);
    }

    [Fact]
    public void JsonOutput_ContainsEnumNames_NotNumericValues()
    {
        // Arrange
        var dict = new Dictionary<ItemName, int>
        {
            { ItemName.DONKEY, 1 },
            { ItemName.TINY, 2 },
            { ItemName.CHUNKY, 3 }
        };

        // Act
        var json = JsonSerializer.Serialize(dict);

        // Assert — Verify enum names are used, not numeric enum values
        Assert.Contains("\"DONKEY\"", json);
        Assert.Contains("\"TINY\"", json);
        Assert.Contains("\"CHUNKY\"", json);
        // The numeric values (0, 1, 2...) should NOT appear as keys
        Assert.DoesNotContain("\"0\":", json);
        Assert.DoesNotContain("\"1\":", json);
    }
}
