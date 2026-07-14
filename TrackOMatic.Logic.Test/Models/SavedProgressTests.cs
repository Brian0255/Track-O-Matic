using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using Newtonsoft.Json;

namespace TrackOMatic.Logic.Test.Models;

public class SavedProgressTests
{
    [Fact]
    public void Constructor_CreatesEmptyCollections()
    {
        // Arrange & Act
        var progress = new SavedProgress();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(progress.SavedItems);
            Assert.Empty(progress.SavedItems);
            Assert.NotNull(progress.SavedHints);
            Assert.Empty(progress.SavedHints);
            Assert.NotNull(progress.SavedGBCounts);
            Assert.Empty(progress.SavedGBCounts);
            Assert.NotNull(progress.BLockerImageIndexes);
            Assert.Empty(progress.BLockerImageIndexes);
            Assert.NotNull(progress.HelmDoorImageIndexes);
            Assert.Empty(progress.HelmDoorImageIndexes);
            Assert.NotNull(progress.HelmDoorCounts);
            Assert.Empty(progress.HelmDoorCounts);
        });
    }

    [Fact]
    public void Constructor_InitializesSpoilerPathToEmpty()
    {
        // Arrange & Act
        var progress = new SavedProgress();

        // Assert
        Assert.Equal("", progress.spoilerPath);
    }

    [Fact]
    public void SavedItems_CanAddItems()
    {
        // Arrange
        var progress = new SavedProgress();
        var item = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            autotracked: false,
            opacity: 1.0
        );

        // Act
        progress.SavedItems[ItemName.DONKEY] = item;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Single(progress.SavedItems);
            Assert.Equal(item, progress.SavedItems[ItemName.DONKEY]);
        });
    }

    [Fact]
    public void SavedItems_CanHoldMultipleItems()
    {
        // Arrange
        var progress = new SavedProgress();
        var item1 = new SavedItem(ItemName.DONKEY, RegionName.JUNGLE_JAPES, ItemVisibilityState.Visible, false, 1.0);
        var item2 = new SavedItem(ItemName.TINY, RegionName.SHOPS, ItemVisibilityState.Hidden, false, 0.5);
        var item3 = new SavedItem(ItemName.CHUNKY, RegionName.FUNGI_FOREST, ItemVisibilityState.Collapsed, true, 0.75);

        // Act
        progress.SavedItems[ItemName.DONKEY] = item1;
        progress.SavedItems[ItemName.TINY] = item2;
        progress.SavedItems[ItemName.CHUNKY] = item3;

        // Assert
        Assert.Equal(3, progress.SavedItems.Count);
    }

    [Fact]
    public void SavedHints_CanAddHints()
    {
        // Arrange
        var progress = new SavedProgress();
        var hint = new SavedHint(
            "test_key",
            "Test Location",
            "5",
            new Dictionary<ItemName, bool> { { ItemName.DONKEY, true } },
            new Dictionary<ItemName, bool> { { ItemName.TINY, false } }
        );

        // Act
        progress.SavedHints.Add(hint);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Single(progress.SavedHints);
            Assert.Equal(hint, progress.SavedHints[0]);
        });
    }

    [Fact]
    public void SavedGBCounts_CanStoreRegionCounts()
    {
        // Arrange
        var progress = new SavedProgress();

        // Act
        progress.SavedGBCounts[RegionName.JUNGLE_JAPES] = "5/20";
        progress.SavedGBCounts[RegionName.SHOPS] = "10/20";

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(2, progress.SavedGBCounts.Count);
            Assert.Equal("5/20", progress.SavedGBCounts[RegionName.JUNGLE_JAPES]);
        });
    }

    [Fact]
    public void BLockerImageIndexes_CanStoreIndexes()
    {
        // Arrange
        var progress = new SavedProgress();

        // Act
        progress.BLockerImageIndexes[RegionName.JUNGLE_JAPES] = 0;
        progress.BLockerImageIndexes[RegionName.SHOPS] = 5;

        // Assert
        Assert.Equal(2, progress.BLockerImageIndexes.Count);
    }

    [Fact]
    public void HelmDoorImageIndexes_CanStoreMultipleValues()
    {
        // Arrange
        var progress = new SavedProgress();

        // Act
        progress.HelmDoorImageIndexes.Add(0);
        progress.HelmDoorImageIndexes.Add(1);
        progress.HelmDoorImageIndexes.Add(2);

        // Assert
        Assert.Equal(3, progress.HelmDoorImageIndexes.Count);
    }

    [Fact]
    public void HelmDoorCounts_CanStoreMultipleCounts()
    {
        // Arrange
        var progress = new SavedProgress();

        // Act
        progress.HelmDoorCounts.Add("1/2");
        progress.HelmDoorCounts.Add("3/4");

        // Assert
        Assert.Equal(2, progress.HelmDoorCounts.Count);
    }

    [Fact]
    public void HelmKongs_NullByDefault()
    {
        // Arrange & Act
        var progress = new SavedProgress();

        // Assert
        Assert.Null(progress.HelmKongs);
    }

    [Fact]
    public void HelmKongs_CanBePopulated()
    {
        // Arrange
        var progress = new SavedProgress();

        // Act
        progress.HelmKongs = [0, 1, 2, 3, 4];

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(progress.HelmKongs);
            Assert.Equal(5, progress.HelmKongs.Count);
        });
    }

    [Fact]
    public void BossKongs_NullByDefault()
    {
        // Arrange & Act
        var progress = new SavedProgress();

        // Assert
        Assert.Null(progress.BossKongs);
    }

    [Fact]
    public void BossKongs_CanBePopulated()
    {
        // Arrange
        var progress = new SavedProgress();

        // Act
        progress.BossKongs = [0, 1, 2];

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(progress.BossKongs);
            Assert.Equal(3, progress.BossKongs.Count);
        });
    }

    [Fact]
    public void LevelOrder_NullByDefault()
    {
        // Arrange & Act
        var progress = new SavedProgress();

        // Assert
        Assert.Null(progress.LevelOrder);
    }

    [Fact]
    public void LevelOrder_CanBePopulated()
    {
        // Arrange
        var progress = new SavedProgress();

        // Act
        progress.LevelOrder = [1, 2, 3, 4, 5];

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(progress.LevelOrder);
            Assert.Equal(5, progress.LevelOrder.Count);
        });
    }

    [Fact]
    public void JsonSerialization_RoundTrip_WithEmptyProgress()
    {
        // Arrange
        var original = new SavedProgress();
        var testPath = Path.Combine(AppContext.BaseDirectory, "test_path.json");
        original.spoilerPath = testPath;

        // Act
        var json = JsonConvert.SerializeObject(original);
        var restored = JsonConvert.DeserializeObject<SavedProgress>(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(restored);
            Assert.Equal(testPath, restored.spoilerPath);
            Assert.Empty(restored.SavedItems);
            Assert.Empty(restored.SavedHints);
        });
    }

    [Fact]
    public void JsonSerialization_RoundTrip_WithSavedItems()
    {
        // Arrange
        var original = new SavedProgress();
        original.SavedItems[ItemName.DONKEY] = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            autotracked: true,
            opacity: 0.8
        );
        original.SavedItems[ItemName.TINY] = new SavedItem(
            ItemName.TINY,
            RegionName.SHOPS,
            ItemVisibilityState.Hidden,
            autotracked: false,
            opacity: 0.5
        );

        // Act
        var json = JsonConvert.SerializeObject(original);
        var restored = JsonConvert.DeserializeObject<SavedProgress>(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(restored);
            Assert.Equal(2, restored.SavedItems.Count);
            Assert.True(restored.SavedItems[ItemName.DONKEY].Autotracked);
            Assert.Equal(ItemVisibilityState.Hidden, restored.SavedItems[ItemName.TINY].Starred);
        });
    }

    [Fact]
    public void JsonSerialization_RoundTrip_WithComplexData()
    {
        // Arrange
        var testDir = Path.Combine(AppContext.BaseDirectory, "spoilers");
        var testPath = Path.Combine(testDir, "test.json");
        var original = new SavedProgress
        {
            spoilerPath = testPath
        };
        original.SavedItems[ItemName.DONKEY] = new SavedItem(ItemName.DONKEY, RegionName.JUNGLE_JAPES, ItemVisibilityState.Visible, false, 1.0);
        original.SavedGBCounts[RegionName.JUNGLE_JAPES] = "15/20";
        original.BLockerImageIndexes[RegionName.SHOPS] = 3;
        original.HelmDoorImageIndexes.Add(0);
        original.HelmDoorCounts.Add("1/2");
        original.HelmKongs = [1, 2, 3];
        original.LevelOrder = [1, 2, 3, 4, 5];

        // Act
        var json = JsonConvert.SerializeObject(original);
        var restored = JsonConvert.DeserializeObject<SavedProgress>(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(restored);
            Assert.NotNull(restored.HelmKongs);
            Assert.NotNull(restored.LevelOrder);
            Assert.Equal(testPath, restored.spoilerPath);
            Assert.Single(restored.SavedItems);
            Assert.Equal("15/20", restored.SavedGBCounts[RegionName.JUNGLE_JAPES]);
            Assert.Equal(3, restored.BLockerImageIndexes[RegionName.SHOPS]);
            Assert.Single(restored.HelmDoorImageIndexes);
            Assert.Equal(3, restored.HelmKongs.Count);
            Assert.Equal(5, restored.LevelOrder.Count);
        });
    }

    [Fact]
    public void MultipleInstances_AreIndependent()
    {
        // Arrange
        var progress1 = new SavedProgress();
        var progress2 = new SavedProgress();

        // Act
        progress1.SavedItems[ItemName.DONKEY] = new SavedItem(ItemName.DONKEY, RegionName.JUNGLE_JAPES, ItemVisibilityState.Visible, false, 1.0);
        progress1.spoilerPath = "path1.json";

        progress2.spoilerPath = "path2.json";

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Single(progress1.SavedItems);
            Assert.Empty(progress2.SavedItems);
            Assert.Equal("path1.json", progress1.spoilerPath);
            Assert.Equal("path2.json", progress2.spoilerPath);
        });
    }
}
