namespace TrackOMatic.ViewModels.Test.Broadcast;

/// <summary>
/// Test coverage for HomingScopeViewModel.
/// Tests constructor validation, event subscriptions, and state initialization.
/// Follows BroadcastItemViewModel patterns for dual-item tracking.
/// </summary>
public class HomingScopeViewModelTests
{
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock;
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;
    private readonly Mock<IDataSaverService> _dataSaverServiceMock;

    public HomingScopeViewModelTests()
    {
        _itemTrackingServiceMock = new Mock<IItemTrackingService>();
        _parsedSpoilerDataServiceMock = new Mock<IParsedSpoilerDataService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
        _dataSaverServiceMock = new Mock<IDataSaverService>();
    }

    private HomingScopeViewModel CreateViewModel()
    {
        return new HomingScopeViewModel(
            _itemTrackingServiceMock.Object,
            _parsedSpoilerDataServiceMock.Object,
            _savedProgressProviderMock.Object,
            _dataSaverServiceMock.Object
        );
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidDependencies_Succeeds()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException(IItemTrackingService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HomingScopeViewModel(
                service!,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("itemTrackingService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException(IParsedSpoilerDataService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HomingScopeViewModel(
                _itemTrackingServiceMock.Object,
                service!,
                _savedProgressProviderMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException(ISavedProgressProvider? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HomingScopeViewModel(
                _itemTrackingServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                service!,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("savedProgressProvider", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullDataSaverService_ThrowsArgumentNullException(IDataSaverService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HomingScopeViewModel(
                _itemTrackingServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                service!
            )
        );
        Assert.Equal("dataSaverService", ex.ParamName);
    }

    #endregion

    #region Event Subscription Tests

    [Fact]
    public void Constructor_SubscribesToItemTrackingServiceItemStateChanged()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _itemTrackingServiceMock.VerifyAdd(x => x.ItemStateChanged += It.IsAny<EventHandler<ItemStateChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToParsedSpoilerDataServiceParsedSpoilerDataChanged()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _parsedSpoilerDataServiceMock.VerifyAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToSavedProgressProviderProgressChanged()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _savedProgressProviderMock.VerifyAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToDataSaverServiceDataLoadingCompleted()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _dataSaverServiceMock.VerifyAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    #endregion


    #region Event Handler Tests - State Reinitialization

    [Fact]
    public void ParsedSpoilerDataChanged_ReinitializesState()
    {
        // Arrange - create viewmodel with default (unstarred) mock
        var viewModel = CreateViewModel();
        Assert.False(viewModel.IsStarred);

        // Step 1: Toggle the star - change mock to return starred item for HOMING_AMMO
        var starredItem = new SavedItem { ItemName = ItemName.HOMING_AMMO, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.HOMING_AMMO))
            .Returns(starredItem);
        _itemTrackingServiceMock.Raise(x => x.ItemStateChanged += null,
            new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating data reload)
        var unstarredItem = new SavedItem { ItemName = ItemName.HOMING_AMMO, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.HOMING_AMMO))
            .Returns(unstarredItem);

        // Act - raise the event, which triggers OnParsedSpoilerDataChanged -> InitializeState()
        _parsedSpoilerDataServiceMock.Raise(x => x.ParsedSpoilerDataChanged += null,
            new ParsedSpoilerDataChangedEventArgs(null, null));

        // Assert - star should be cleared because InitializeState() was called
        Assert.False(viewModel.IsStarred);
    }

    [Fact]
    public void ProgressChanged_ReinitializesState()
    {
        // Arrange - create viewmodel with default (unstarred) mock
        var viewModel = CreateViewModel();
        Assert.False(viewModel.IsStarred);

        // Step 1: Toggle the star - change mock to return starred item for HOMING_AMMO
        var starredItem = new SavedItem { ItemName = ItemName.HOMING_AMMO, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.HOMING_AMMO))
            .Returns(starredItem);
        _itemTrackingServiceMock.Raise(x => x.ItemStateChanged += null,
            new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating progress reload)
        var unstarredItem = new SavedItem { ItemName = ItemName.HOMING_AMMO, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.HOMING_AMMO))
            .Returns(unstarredItem);

        // Act - raise the event, which triggers OnProgressChanged -> InitializeState()
        var mockNewProgress = new SavedProgress();
        _savedProgressProviderMock.Raise(x => x.ProgressChanged += null,
            new ProgressReplacedEventArgs { OldProgress = null, NewProgress = mockNewProgress });

        // Assert - star should be cleared because InitializeState() was called
        Assert.False(viewModel.IsStarred);
    }

    [Fact]
    public void DataLoadingCompleted_ReinitializesState()
    {
        // Arrange - create viewmodel with default (unstarred) mock
        var viewModel = CreateViewModel();
        Assert.False(viewModel.IsStarred);

        // Step 1: Toggle the star - change mock to return starred item for HOMING_AMMO
        var starredItem = new SavedItem { ItemName = ItemName.HOMING_AMMO, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.HOMING_AMMO))
            .Returns(starredItem);
        _itemTrackingServiceMock.Raise(x => x.ItemStateChanged += null,
            new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating data load)
        var unstarredItem = new SavedItem { ItemName = ItemName.HOMING_AMMO, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.HOMING_AMMO))
            .Returns(unstarredItem);

        // Act - raise the event, which triggers OnDataLoadingCompleted -> InitializeState()
        var mockProgress = new SavedProgress();
        _dataSaverServiceMock.Raise(x => x.DataLoadingCompleted += null,
            new DataLoadingCompletedEventArgs(mockProgress));

        // Assert - star should be cleared because InitializeState() was called
        Assert.False(viewModel.IsStarred);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Constructor_InitializesImageResourceKey()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel.ImageResourceKey);
    }

    [Fact]
    public void Constructor_InitializesIsStarredToFalse()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.False(viewModel.IsStarred);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromAllEvents()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.Dispose();

        // Assert
        Assert.Multiple(
            () => _itemTrackingServiceMock.VerifyRemove(x => x.ItemStateChanged -= It.IsAny<EventHandler<ItemStateChangedEventArgs>>(), Times.Once),
            () => _parsedSpoilerDataServiceMock.VerifyRemove(x => x.ParsedSpoilerDataChanged -= It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once),
            () => _savedProgressProviderMock.VerifyRemove(x => x.ProgressChanged -= It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once),
            () => _dataSaverServiceMock.VerifyRemove(x => x.DataLoadingCompleted -= It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once)
        );
    }

    #endregion

    #region UpdateImageResourceKey Tests

    [Fact]
    public void UpdateImageResourceKey_WithBothItemsUnknown_SetsBWImage()
    {
        // Arrange - both items unknown (default mock behavior)
        var viewModel = CreateViewModel();

        // Assert - should use b/w image when both items are unknown
        Assert.Equal("homing_scope_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithLeftKnownFromSpoilerData_SetsHomingOnlyImage()
    {
        // Arrange - set up spoiler data with HOMING_AMMO in a known region
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.HOMING_AMMO] = RegionName.JUNGLE_JAPES;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        // Act - create view model with spoiler data configured
        var viewModel = CreateViewModel();

        // Assert - should use homing ammo only image
        Assert.Equal("homingonly", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithRightKnownFromTracking_SetsScopeOnlyImage()
    {
        // Arrange - set up item tracking service to return SNIPER_SCOPE in a known region
        var scopeItem = new SavedItem { ItemName = ItemName.SNIPER_SCOPE, Region = RegionName.CRYSTAL_CAVES };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.SNIPER_SCOPE))
            .Returns(scopeItem);

        // Act - create view model with tracking data configured
        var viewModel = CreateViewModel();

        // Assert - should use sniper scope only image
        Assert.Equal("scopeonly", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithBothItemsKnown_SetsFullImage()
    {
        // Arrange - set up both sources with known items
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.HOMING_AMMO] = RegionName.JUNGLE_JAPES;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        var scopeItem = new SavedItem { ItemName = ItemName.SNIPER_SCOPE, Region = RegionName.CRYSTAL_CAVES };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.SNIPER_SCOPE))
            .Returns(scopeItem);

        // Act - create view model with both sources configured
        var viewModel = CreateViewModel();

        // Assert - should use full homing scope image
        Assert.Equal("homing_scope", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithLeftKnownFromTracking_SetsHomingOnlyImage()
    {
        // Arrange - set up item tracking service to return HOMING_AMMO in a known region
        var homingItem = new SavedItem { ItemName = ItemName.HOMING_AMMO, Region = RegionName.JUNGLE_JAPES };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.HOMING_AMMO))
            .Returns(homingItem);

        // Act - create view model with tracking data configured
        var viewModel = CreateViewModel();

        // Assert - should use homing ammo only image
        Assert.Equal("homingonly", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithRightKnownFromSpoilerData_SetsScopeOnlyImage()
    {
        // Arrange - set up spoiler data with SNIPER_SCOPE in a known region
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.SNIPER_SCOPE] = RegionName.CRYSTAL_CAVES;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        // Act - create view model with spoiler data configured
        var viewModel = CreateViewModel();

        // Assert - should use sniper scope only image
        Assert.Equal("scopeonly", viewModel.ImageResourceKey);
    }

    #endregion
}
