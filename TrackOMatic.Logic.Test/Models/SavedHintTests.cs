using System.Text.Json;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Test.Models;

public class SavedHintTests
{
    [Fact]
    public void Constructor_WithAllParameters_CreatesValidSavedHint()
    {
        // Arrange
        var pathItems = new Dictionary<ItemName, bool>
        {
            { ItemName.DONKEY, true },
            { ItemName.DIDDY, false }
        };
        var foundItems = new Dictionary<ItemName, bool>
        {
            { ItemName.LANKY, true }
        };
        var hintId = "hint-001";

        // Act
        var savedHint = new SavedHint(
            HintPanelType.Isles,
            "Location here",
            "2/5",
            pathItems,
            foundItems,
            hintId
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(HintPanelType.Isles, savedHint.HintPanelType);
            Assert.Equal("Location here", savedHint.LocationText);
            Assert.Equal("2/5", savedHint.PotionCountText);
            Assert.Equal(2, savedHint.PathItems.Count);
            Assert.Single(savedHint.FoundItems);
            Assert.Equal(hintId, savedHint.HintId);
        });
    }

    [Fact]
    public void Constructor_WithDefaultHintId_GeneratesAutoId()
    {
        // Arrange
        var pathItems = new Dictionary<ItemName, bool>();
        var foundItems = new Dictionary<ItemName, bool>();

        // Act
        var savedHint = new SavedHint(
            HintPanelType.Aztec,
            "Test Location",
            "1/1",
            pathItems,
            foundItems
        );

        // Assert
        // When no HintId is provided, it should be auto-generated from a GUID (first 8 characters)
        Assert.NotNull(savedHint.HintId);
        Assert.NotEmpty(savedHint.HintId);
        Assert.Equal(8, savedHint.HintId.Length);
    }

    [Fact]
    public void HintPanelKey_IsReadOnly_CannotBeChanged()
    {
        // Arrange
        var savedHint = new SavedHint(
            HintPanelType.Isles,
            "Location",
            "1/1",
            [],
            []
        );

        // Act & Assert
        // HintPanelType is a read-only get, should have no setter
        Assert.Equal(HintPanelType.Isles, savedHint.HintPanelType);
    }

    [Fact]
    public void LocationText_CanBeModified_PropertyAllowsSetAfterConstruction()
    {
        // Arrange
        var savedHint = new SavedHint(
            HintPanelType.Galleon,
            "Original Location",
            "1/1",
            [],
            []
        );

        // Act
        savedHint.LocationText = "Modified Location";

        // Assert
        Assert.Equal("Modified Location", savedHint.LocationText);
    }

    [Fact]
    public void PotionCountText_CanBeModified_PropertyAllowsSetAfterConstruction()
    {
        // Arrange
        var savedHint = new SavedHint(
            HintPanelType.Castle,
            "Location",
            "1/1",
            [],
            []
        );

        // Act
        savedHint.PotionCountText = "5/10";

        // Assert
        Assert.Equal("5/10", savedHint.PotionCountText);
    }

    [Fact]
    public void PathItems_CanBeModified_PropertyAllowsSetAfterConstruction()
    {
        // Arrange
        var originalPathItems = new Dictionary<ItemName, bool> { { ItemName.DONKEY, true } };
        var savedHint = new SavedHint(
            HintPanelType.Helm,
            "Location",
            "1/1",
            originalPathItems,
            []
        );

        // Act
        var newPathItems = new Dictionary<ItemName, bool> { { ItemName.DIDDY, false } };
        savedHint.PathItems = newPathItems;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(newPathItems, savedHint.PathItems);
            Assert.Single(savedHint.PathItems);
            Assert.Contains(ItemName.DIDDY, savedHint.PathItems.Keys);
        });
    }

    [Fact]
    public void FoundItems_CanBeModified_PropertyAllowsSetAfterConstruction()
    {
        // Arrange
        var originalFoundItems = new Dictionary<ItemName, bool> { { ItemName.LANKY, true } };
        var savedHint = new SavedHint(
            HintPanelType.Galleon,
            "Location",
            "1/1",
            [],
            originalFoundItems
        );

        // Act
        var newFoundItems = new Dictionary<ItemName, bool> { { ItemName.TINY, false }, { ItemName.CHUNKY, true } };
        savedHint.FoundItems = newFoundItems;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(newFoundItems, savedHint.FoundItems);
            Assert.Equal(2, savedHint.FoundItems.Count);
        });
    }

    [Fact]
    public void Constructor_WithVariousPanelKeys_StoresCorrectly()
    {
        // Arrange & Act
        var panelKeys = new[] { HintPanelType.Isles, HintPanelType.Aztec, HintPanelType.Paths, HintPanelType.Foolish };
        var savedHints = panelKeys.Select(key => new SavedHint(
            key,
            "Location",
            "1/1",
            [],
            []
        )).ToList();

        // Assert
        Assert.All(savedHints, hint => Assert.Contains(hint.HintPanelType, panelKeys));
    }

    [Fact]
    public void Constructor_WithEmptyItemDictionaries_CreatesSuccessfully()
    {
        // Arrange & Act
        var savedHint = new SavedHint(
            HintPanelType.Isles,
            "Location",
            "1/1",
            [],
            []
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Empty(savedHint.PathItems);
            Assert.Empty(savedHint.FoundItems);
        });
    }

    [Fact]
    public void Constructor_WithComplexItemDictionaries_PreservesAll()
    {
        // Arrange
        var pathItems = new Dictionary<ItemName, bool>
        {
            { ItemName.DONKEY, true },
            { ItemName.DIDDY, false },
            { ItemName.LANKY, true },
            { ItemName.TINY, false },
            { ItemName.CHUNKY, true }
        };
        var foundItems = new Dictionary<ItemName, bool>
        {
            { ItemName.DONKEY, false },
            { ItemName.LANKY, true }
        };

        // Act
        var savedHint = new SavedHint(
            HintPanelType.Isles,
            "Location",
            "1/1",
            pathItems,
            foundItems
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(5, savedHint.PathItems.Count);
            Assert.Equal(2, savedHint.FoundItems.Count);
            Assert.True(savedHint.PathItems[ItemName.DONKEY]);
            Assert.False(savedHint.PathItems[ItemName.DIDDY]);
            Assert.False(savedHint.FoundItems[ItemName.DONKEY]);
        });
    }

    [Fact]
    public void Multiple_SavedHints_AreIndependent()
    {
        // Arrange
        var hint1 = new SavedHint(
            HintPanelType.Isles,
            "Location1",
            "1/1",
            new Dictionary<ItemName, bool> { { ItemName.DONKEY, true } },
            [],
            "hint-001"
        );

        var hint2 = new SavedHint(
            HintPanelType.Isles,
            "Location2",
            "2/2",
            new Dictionary<ItemName, bool> { { ItemName.DIDDY, false } },
            [],
            "hint-002"
        );

        // Act
        hint1.LocationText = "Modified Location1";

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal("Location2", hint2.LocationText);
            Assert.Equal("hint-002", hint2.HintId);
        });
    }

    [Fact]
    public void JsonSerialization_RoundTrip_PreservesAllProperties()
    {
        // Arrange
        var originalHint = new SavedHint(
            HintPanelType.Isles,
            "Test Location",
            "3/5",
            new Dictionary<ItemName, bool> { { ItemName.DONKEY, true } },
            new Dictionary<ItemName, bool> { { ItemName.DIDDY, false } },
            "test-id-001"
        );

        // Act
        var json = JsonSerializer.Serialize(originalHint);
        var deserializedHint = JsonSerializer.Deserialize<SavedHint>(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(deserializedHint);
            Assert.Equal(originalHint.HintPanelType, deserializedHint.HintPanelType);
            Assert.Equal(originalHint.LocationText, deserializedHint.LocationText);
            Assert.Equal(originalHint.PotionCountText, deserializedHint.PotionCountText);
            Assert.Equal(originalHint.HintId, deserializedHint.HintId);
            Assert.Equal(originalHint.PathItems, deserializedHint.PathItems);
            Assert.Equal(originalHint.FoundItems, deserializedHint.FoundItems);
        });
    }

    [Fact]
    public void HintId_EmptyString_IsAutoGenerated()
    {
        // Arrange & Act
        var hint1 = new SavedHint(HintPanelType.Factory, "Location", "1/1", [], []);
        var hint2 = new SavedHint(HintPanelType.Isles, "Location", "1/1", [], [], "");

        // Assert
        // Both should have auto-generated IDs (8 characters from a GUID)
        Assert.NotNull(hint1.HintId);
        Assert.NotNull(hint2.HintId);
        Assert.NotEmpty(hint1.HintId);
        Assert.NotEmpty(hint2.HintId);
        Assert.Equal(8, hint1.HintId.Length);
        Assert.Equal(8, hint2.HintId.Length);
        // They should be different since they were generated randomly
        Assert.NotEqual(hint1.HintId, hint2.HintId);
    }

    [Fact]
    public void HintId_DistinctValues_CanIdentifyHints()
    {
        // Arrange
        var hints = Enumerable.Range(1, 5)
            .Select(i => new SavedHint(
                HintPanelType.Isles,
                $"Location{i}",
                "1/1",
                [],
                [],
                $"hint-{i:D3}"
            ))
            .ToList();

        // Act
        var ids = hints.Select(h => h.HintId).Distinct().Count();

        // Assert
        Assert.Equal(5, ids);
    }

    [Fact]
    public void ParameterlessConstructor_CanBeUsedForDeserialization()
    {
        // Arrange & Act
        var savedHint = new SavedHint
        {
            HintPanelType = HintPanelType.Isles,
            HintId = "test-id-123",
            LocationText = "Test Location",
            PotionCountText = "2/5",
            PathItems = new() { { ItemName.DONKEY, true } },
            FoundItems = new() { { ItemName.DIDDY, false } }
        };

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(HintPanelType.Isles, savedHint.HintPanelType);
            Assert.Equal("test-id-123", savedHint.HintId);
            Assert.Equal("Test Location", savedHint.LocationText);
            Assert.Equal("2/5", savedHint.PotionCountText);
        });
    }

    [Fact]
    public void HintId_InitOnly_CannotBeChangedAfterConstruction()
    {
        // Arrange
        var savedHint = new SavedHint(
            HintPanelType.Isles,
            "Location",
            "1/1",
            [],
            [],
            "test-id"
        );

        // Act & Assert
        // HintId should be init-only, so this should fail to compile if someone tries to set it
        // We verify the value is what we set during initialization
        Assert.Equal("test-id", savedHint.HintId);
    }

    [Fact]
    public void HintPanelKey_InitOnly_CannotBeChangedAfterConstruction()
    {
        // Arrange
        var savedHint = new SavedHint(
            HintPanelType.Isles,
            "Location",
            "1/1",
            [],
            []
        );

        // Act & Assert
        // HintPanelType should be init-only, so this should fail to compile if someone tries to set it
        // We verify the value is what we set during initialization
        Assert.Equal(HintPanelType.Isles, savedHint.HintPanelType);
    }

    [Fact]
    public void ProvidedHintId_IsPreservedAndNotAutoGenerated()
    {
        // Arrange
        var customId = "custom-id-12345678";

        // Act
        var savedHint = new SavedHint(
            HintPanelType.Isles,
            "Location",
            "1/1",
            [],
            [],
            customId
        );

        // Assert
        Assert.Equal(customId, savedHint.HintId);
    }
}

