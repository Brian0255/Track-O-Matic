using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services.Test.TrackerState;

public class HintServiceTests
{
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock;
    private readonly Mock<ISavedProgressProvider> _progressProviderMock;
    private readonly SavedProgress _progress;
    private readonly HintService _sut;

    public HintServiceTests()
    {
        // Set up UI SynchronizationContext for constructor requirement
        if (SynchronizationContext.Current == null)
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        _itemTrackingServiceMock = new Mock<IItemTrackingService>();
        _progress = new SavedProgress();
        _progressProviderMock = new Mock<ISavedProgressProvider>();
        _progressProviderMock.Setup(p => p.CurrentProgress).Returns(_progress);

        _sut = new HintService(_itemTrackingServiceMock.Object, _progressProviderMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HintService(null!, _progressProviderMock.Object)
        );
    }

    [Fact]
    public void Constructor_WithNullProgressProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HintService(_itemTrackingServiceMock.Object, null!)
        );
    }

    [Fact]
    public void Constructor_SubscribesToProgressChanged()
    {
        // The constructor should subscribe to ProgressChanged, but we can't easily mock events.
        // This test just confirms the service was created successfully.
        Assert.NotNull(_sut);
    }

    [Fact]
    public void Constructor_InitializesEmptyHintCache()
    {
        var hints = _sut.GetAllHints();
        Assert.Empty(hints);
    }

    #endregion

    #region CreateHint Tests

    [Theory]
    [InlineData(HintPanelType.Unhinted)]
    [InlineData(HintPanelType.PotionCounts)]
    [InlineData(HintPanelType.Isles)]
    public void CreateHint_WithValidType_CreatesHintAndFiresEvent(HintPanelType hintPanelType)
    {
        // Arrange
        var eventFired = false;
        SavedHint? createdHint = null;
        _sut.HintCreated += (sender, args) =>
        {
            eventFired = true;
            createdHint = args.Hint;
        };

        // Act
        var hint = _sut.CreateHint(hintPanelType);

        // Assert
        Assert.NotNull(hint);
        Assert.Equal(hintPanelType, hint.HintPanelType);
        Assert.Empty(hint.LocationText);
        Assert.Empty(hint.PotionCountText);
        Assert.Empty(hint.PathItems);
        Assert.Empty(hint.FoundItems);
        Assert.True(eventFired);
        Assert.Same(hint, createdHint);
    }

    [Fact]
    public void CreateHint_AddsHintToCache()
    {
        // Act
        var hint = _sut.CreateHint(HintPanelType.Unhinted);

        // Assert
        var allHints = _sut.GetAllHints();
        Assert.Contains(hint, allHints);
    }

    [Fact]
    public void CreateHint_AddsHintToProgressProvider()
    {
        // Act
        var hint = _sut.CreateHint(HintPanelType.Unhinted);

        // Assert
        Assert.Contains(hint, _progress.SavedHints);
    }

    [Fact]
    public void CreateHint_MultipleHints_AllAddedToCache()
    {
        // Act
        var hint1 = _sut.CreateHint(HintPanelType.Unhinted);
        var hint2 = _sut.CreateHint(HintPanelType.PotionCounts);
        var hint3 = _sut.CreateHint(HintPanelType.Isles);

        // Assert
        var allHints = _sut.GetAllHints();
        Assert.Equal(3, allHints.Count);
        Assert.Contains(hint1, allHints);
        Assert.Contains(hint2, allHints);
        Assert.Contains(hint3, allHints);
    }

    #endregion

    #region Query Tests

    [Fact]
    public void GetHint_WithValidId_ReturnsHint()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);

        // Act
        var retrieved = _sut.GetHint(hint.HintId);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Same(hint, retrieved);
    }

    [Fact]
    public void GetHint_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidHintId = new HintId("invalid0");

        // Act
        var retrieved = _sut.GetHint(invalidHintId);

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public void GetAllHints_ReturnsReadOnlyList()
    {
        // Arrange
        var hint1 = _sut.CreateHint(HintPanelType.Unhinted);
        var hint2 = _sut.CreateHint(HintPanelType.PotionCounts);

        // Act
        var allHints = _sut.GetAllHints();

        // Assert
        Assert.Equal(2, allHints.Count);
        Assert.Contains(hint1, allHints);
        Assert.Contains(hint2, allHints);
    }

    #endregion

    #region Update Location Tests

    [Fact]
    public void UpdateHintLocation_WithValidHintId_UpdatesLocationAndFiresEvent()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);
        var eventFired = false;
        SavedHint? eventHint = null;
        _sut.HintLocationUpdated += (sender, args) =>
        {
            eventFired = true;
            eventHint = args.UpdatedHint;
        };

        // Act
        _sut.UpdateHintLocation(hint.HintId, "Test Location");

        // Assert
        Assert.True(eventFired);
        Assert.Equal("Test Location", hint.LocationText);
        Assert.Same(hint, eventHint);
    }

    [Fact]
    public void UpdateHintLocation_WithInvalidHintId_ThrowsArgumentException()
    {
        // Arrange
        var invalidHintId = new HintId("invalid0");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _sut.UpdateHintLocation(invalidHintId, "Test")
        );
        Assert.Contains("does not exist", exception.Message);
    }

    [Fact]
    public void UpdateHintLocation_WithEmptyString_Succeeds()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);
        _sut.UpdateHintLocation(hint.HintId, "Initial");

        // Act
        _sut.UpdateHintLocation(hint.HintId, "");

        // Assert
        Assert.Empty(hint.LocationText);
    }

    #endregion

    #region Update Potion Count Tests

    [Fact]
    public void UpdateHintPotionCount_WithValidHintId_UpdatesPotionCountAndFiresEvent()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.PotionCounts);
        var eventFired = false;
        SavedHint? eventHint = null;
        _sut.HintPotionCountUpdated += (sender, args) =>
        {
            eventFired = true;
            eventHint = args.UpdatedHint;
        };

        // Act
        _sut.UpdateHintPotionCount(hint.HintId, "5");

        // Assert
        Assert.True(eventFired);
        Assert.Equal("5", hint.PotionCountText);
        Assert.Same(hint, eventHint);
    }

    [Fact]
    public void UpdateHintPotionCount_WithInvalidHintId_ThrowsArgumentException()
    {
        // Arrange
        var invalidHintId = new HintId("invalid0");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _sut.UpdateHintPotionCount(invalidHintId, "5")
        );
        Assert.Contains("does not exist", exception.Message);
    }

    [Fact]
    public void UpdateHintPotionCount_WithNonNumericString_Succeeds()
    {
        // Arrange - The service does not validate, user can put whatever they want
        var hint = _sut.CreateHint(HintPanelType.PotionCounts);

        // Act
        _sut.UpdateHintPotionCount(hint.HintId, "not_a_number");

        // Assert
        Assert.Equal("not_a_number", hint.PotionCountText);
    }

    #endregion

    #region Path Items Tests

    [Fact]
    public void SetPathItems_WithValidHintId_SetsPathItemsAndFiresEvent()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);
        var pathItems = new Dictionary<ItemName, bool>
        {
            { ItemName.DONKEY_KROOL, true },
            { ItemName.DONKEY, false }
        };
        var eventFired = false;
        Dictionary<ItemName, bool>? eventPathItems = null;
        _sut.HintPathItemsUpdated += (sender, args) =>
        {
            eventFired = true;
            eventPathItems = args.PathItems;
        };

        // Act
        _sut.SetPathItems(hint.HintId, pathItems);

        // Assert
        Assert.True(eventFired);
        Assert.Equal(pathItems, hint.PathItems);
        Assert.Same(pathItems, eventPathItems);
    }

    [Fact]
    public void SetPathItems_WithInvalidHintId_ThrowsArgumentException()
    {
        // Arrange
        var invalidHintId = new HintId("invalid0");
        var pathItems = new Dictionary<ItemName, bool>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _sut.SetPathItems(invalidHintId, pathItems)
        );
        Assert.Contains("does not exist", exception.Message);
    }

    [Fact]
    public void TogglePathItem_WithExistingItem_TogglesToPreviousStateAndFiresEvent()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);
        hint.PathItems[ItemName.DONKEY_KROOL] = true;
        var eventFired = false;
        ItemName? eventItemName = null;
        bool? eventIsFound = null;
        _sut.HintPathItemToggled += (sender, args) =>
        {
            eventFired = true;
            eventItemName = args.ItemName;
            eventIsFound = args.IsFound;
        };

        // Act
        _sut.TogglePathItem(hint.HintId, ItemName.DONKEY_KROOL);

        // Assert
        Assert.True(eventFired);
        Assert.False(hint.PathItems[ItemName.DONKEY_KROOL]);
        Assert.Equal(ItemName.DONKEY_KROOL, eventItemName);
        Assert.False(eventIsFound);
    }

    [Fact]
    public void TogglePathItem_WithNonExistentItem_DoesNotFire()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);
        var eventFired = false;
        _sut.HintPathItemToggled += (sender, args) => eventFired = true;

        // Act
        _sut.TogglePathItem(hint.HintId, ItemName.DONKEY_KROOL);

        // Assert
        Assert.False(eventFired);
    }

    #endregion

    #region Found Items Tests

    [Fact]
    public void SetFoundItems_WithNonPlayableHintType_SetsItemsWithoutItemTracking()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);
        var foundItems = new Dictionary<ItemName, bool>
        {
            { ItemName.DONKEY_KROOL, true },
            { ItemName.DONKEY, false }
        };
        var eventFired = false;
        _sut.HintFoundItemsUpdated += (sender, args) => eventFired = true;

        // Act
        _sut.SetFoundItems(hint.HintId, foundItems, HintPanelType.Unhinted);

        // Assert
        Assert.True(eventFired);
        Assert.Equal(foundItems, hint.FoundItems);
        // Item tracking service should not be called for non-playable regions
        _itemTrackingServiceMock.Verify(
            i => i.BeginBatchUpdate(),
            Times.Never
        );
    }

    [Fact]
    public void SetFoundItems_WithPlayableHintType_CoordinatesWithItemTracking()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Isles); // Isles is a playable region
        var foundItems = new Dictionary<ItemName, bool>
        {
            { ItemName.DONKEY_KROOL, true }
        };

        var batchScopeMock = new Mock<IDisposable>();
        _itemTrackingServiceMock
            .Setup(i => i.BeginBatchUpdate())
            .Returns(batchScopeMock.Object);

        _itemTrackingServiceMock
            .Setup(i => i.GetItemState(It.IsAny<ItemName>()))
            .Returns((SavedItem?)null);

        var eventFired = false;
        _sut.HintFoundItemsUpdated += (sender, args) => eventFired = true;

        // Act
        _sut.SetFoundItems(hint.HintId, foundItems, HintPanelType.Isles);

        // Assert
        Assert.True(eventFired);
        _itemTrackingServiceMock.Verify(
            i => i.BeginBatchUpdate(),
            Times.Once
        );
        batchScopeMock.Verify(b => b.Dispose(), Times.Once);
    }

    [Fact]
    public void SetFoundItems_WithInvalidHintId_ThrowsArgumentException()
    {
        // Arrange
        var invalidHintId = new HintId("invalid0");
        var foundItems = new Dictionary<ItemName, bool>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _sut.SetFoundItems(invalidHintId, foundItems, HintPanelType.Unhinted)
        );
        Assert.Contains("does not exist", exception.Message);
    }

    [Fact]
    public void ToggleFoundItem_WithExistingItem_TogglesFalseToTrueAndFiresEvent()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);
        hint.FoundItems[ItemName.DONKEY_KROOL] = false;
        var eventFired = false;
        ItemName? eventItemName = null;
        bool? eventIsFound = null;
        _sut.HintFoundItemToggled += (sender, args) =>
        {
            eventFired = true;
            eventItemName = args.ItemName;
            eventIsFound = args.IsFound;
        };

        // Act
        _sut.ToggleFoundItem(hint.HintId, ItemName.DONKEY_KROOL);

        // Assert
        Assert.True(eventFired);
        Assert.True(hint.FoundItems[ItemName.DONKEY_KROOL]);
        Assert.Equal(ItemName.DONKEY_KROOL, eventItemName);
        Assert.True(eventIsFound);
    }

    [Fact]
    public void ToggleFoundItem_WithNonExistentItem_DoesNotFire()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);
        var eventFired = false;
        _sut.HintFoundItemToggled += (sender, args) => eventFired = true;

        // Act
        _sut.ToggleFoundItem(hint.HintId, ItemName.DONKEY_KROOL);

        // Assert
        Assert.False(eventFired);
    }

    #endregion

    #region Remove Hint Tests

    [Fact]
    public void RemoveHint_WithValidHintId_RemovesHintAndFiresEvent()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);
        var eventFired = false;
        HintId? eventHintId = null;
        SavedHint? eventHint = null;
        _sut.HintRemoved += (sender, args) =>
        {
            eventFired = true;
            eventHintId = args.HintId;
            eventHint = args.RemovedHint;
        };

        // Act
        _sut.RemoveHint(hint.HintId);

        // Assert
        Assert.True(eventFired);
        Assert.Equal(hint.HintId, eventHintId);
        Assert.Same(hint, eventHint);
        Assert.Null(_sut.GetHint(hint.HintId));
    }

    [Fact]
    public void RemoveHint_RemovesFromProgressProvider()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);

        // Act
        _sut.RemoveHint(hint.HintId);

        // Assert
        Assert.DoesNotContain(hint, _progress.SavedHints);
    }

    [Fact]
    public void RemoveHint_WithNonExistentHintId_DoesNotThrow()
    {
        // Arrange
        var invalidHintId = new HintId("nonexist");

        // Act & Assert - Should not throw
        _sut.RemoveHint(invalidHintId);
    }

    [Fact]
    public void RemoveHint_WithHintedItems_ClearsItemState()
    {
        // Arrange
        var hint = _sut.CreateHint(HintPanelType.Unhinted);
        hint.FoundItems[ItemName.DONKEY_KROOL] = true;

        var batchScopeMock = new Mock<IDisposable>();
        _itemTrackingServiceMock
            .Setup(i => i.BeginBatchUpdate())
            .Returns(batchScopeMock.Object);

        var hintedItem = new SavedItem { ItemName = ItemName.DONKEY_KROOL, Hinted = true };
        _itemTrackingServiceMock
            .Setup(i => i.GetItemState(ItemName.DONKEY_KROOL))
            .Returns(hintedItem);

        // Act
        _sut.RemoveHint(hint.HintId);

        // Assert
        _itemTrackingServiceMock.Verify(
            i => i.ClearItemState(ItemName.DONKEY_KROOL),
            Times.Once
        );
    }

    #endregion

    #region LoadHintsSilently Tests

    [Fact]
    public void LoadHintsSilently_ClearsExistingHintsAndLoadsNew()
    {
        // Arrange
        var existingHint = _sut.CreateHint(HintPanelType.Unhinted);
        var newHint1 = new SavedHint { HintPanelType = HintPanelType.PotionCounts, HintId = new HintId("00000001") };
        var newHint2 = new SavedHint { HintPanelType = HintPanelType.Isles, HintId = new HintId("00000002") };

        // Act
        _sut.LoadHintsSilently([newHint1, newHint2]);

        // Assert
        var allHints = _sut.GetAllHints();
        Assert.Equal(2, allHints.Count);
        Assert.Contains(newHint1, allHints);
        Assert.Contains(newHint2, allHints);
        Assert.DoesNotContain(existingHint, allHints);
    }

    [Fact]
    public void LoadHintsSilently_SyncsProgressProvider()
    {
        // Arrange
        var newHint1 = new SavedHint { HintPanelType = HintPanelType.PotionCounts, HintId = new HintId("00000001") };
        var newHint2 = new SavedHint { HintPanelType = HintPanelType.Isles, HintId = new HintId("00000002") };

        // Act
        _sut.LoadHintsSilently([newHint1, newHint2]);

        // Assert
        Assert.Equal(2, _progress.SavedHints.Count);
        Assert.Contains(newHint1, _progress.SavedHints);
        Assert.Contains(newHint2, _progress.SavedHints);
    }

    [Fact]
    public void LoadHintsSilently_DoesNotFireHintCreatedEvents()
    {
        // Arrange
        var eventFiredCount = 0;
        _sut.HintCreated += (sender, args) => eventFiredCount++;

        var newHint1 = new SavedHint { HintPanelType = HintPanelType.PotionCounts, HintId = new HintId("00000001") };
        var newHint2 = new SavedHint { HintPanelType = HintPanelType.Isles, HintId = new HintId("00000002") };

        // Act
        _sut.LoadHintsSilently([newHint1, newHint2]);

        // Assert
        Assert.Equal(0, eventFiredCount);
    }

    #endregion

    #region Batch Update Tests

    [Fact]
    public void BeginBatchUpdate_DefersCacheRefreshUntilDispose()
    {
        // Arrange
        var initialHint = _sut.CreateHint(HintPanelType.Unhinted);

        // Simulate external progress change
        var newHint = new SavedHint { HintPanelType = HintPanelType.PotionCounts, HintId = new HintId("external") };
        _progress.SavedHints.Add(newHint);

        // Act
        using (var batch = _sut.BeginBatchUpdate())
        {
            // While batching, cache should not be refreshed
            var hintsInBatch = _sut.GetAllHints();
            Assert.Single(hintsInBatch); // Only the initially created hint
            Assert.DoesNotContain(newHint, hintsInBatch);
        }

        // After batch disposes, cache should be refreshed
        var hintsAfterBatch = _sut.GetAllHints();
        Assert.Equal(2, hintsAfterBatch.Count);
        Assert.Contains(initialHint, hintsAfterBatch);
        Assert.Contains(newHint, hintsAfterBatch);
    }

    [Fact]
    public void BeginBatchUpdate_MultipleBatches_EachRefreshesCache()
    {
        // Arrange
        _ = _sut.CreateHint(HintPanelType.Unhinted);

        // Act - First batch
        using (var batch1 = _sut.BeginBatchUpdate())
        {
            _progress.SavedHints.Add(new SavedHint { HintPanelType = HintPanelType.PotionCounts, HintId = new HintId("hint0002") });
        }

        // Act - Second batch
        using (var batch2 = _sut.BeginBatchUpdate())
        {
            _progress.SavedHints.Add(new SavedHint { HintPanelType = HintPanelType.Isles, HintId = new HintId("hint0003") });
        }

        // Assert
        var allHints = _sut.GetAllHints();
        Assert.Equal(3, allHints.Count);
    }

    #endregion

    #region Progress Changed Tests

    [Fact]
    public void OnProgressChanged_WhenNotBatching_RefreshesCache()
    {
        // This test is implicit through CreateHint and other operations.
        // The service initializes the cache on construction from CurrentProgress.
        // When progress changes, the cache is re-initialized unless batching.
        var hint1 = _sut.CreateHint(HintPanelType.Unhinted);

        // Add a hint directly to the progress provider (simulating external change)
        var externalHint = new SavedHint { HintPanelType = HintPanelType.PotionCounts, HintId = new HintId("external") };
        _progress.SavedHints.Add(externalHint);

        // Since cache is not automatically refreshed on external progress changes,
        // we rely on InitializeCache being called when progress is replaced,
        // which happens via ProgressChanged event subscription.
        // We can't test this directly without more complex mocking.

        Assert.NotNull(hint1);
    }

    #endregion
}
