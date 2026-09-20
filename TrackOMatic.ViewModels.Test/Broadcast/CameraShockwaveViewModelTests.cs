namespace TrackOMatic.ViewModels.Test.Broadcast;

/// <summary>
/// Test coverage for CameraShockwaveViewModel.
/// Tests constructor validation, event subscriptions, and state initialization.
/// Follows BroadcastItemViewModel patterns.
/// </summary>
public class CameraShockwaveViewModelTests
{
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock;
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;
    private readonly Mock<IDataSaverService> _dataSaverServiceMock;

    public CameraShockwaveViewModelTests()
    {
        _itemTrackingServiceMock = new Mock<IItemTrackingService>();
        _parsedSpoilerDataServiceMock = new Mock<IParsedSpoilerDataService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
        _dataSaverServiceMock = new Mock<IDataSaverService>();
    }

    private CameraShockwaveViewModel CreateViewModel()
    {
        return new CameraShockwaveViewModel(
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
            new CameraShockwaveViewModel(
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
            new CameraShockwaveViewModel(
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
            new CameraShockwaveViewModel(
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
            new CameraShockwaveViewModel(
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

        // Step 1: Toggle the star - change mock to return starred item for FAIRY_CAMERA
        var starredItem = new SavedItem { ItemName = ItemName.FAIRY_CAMERA, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.FAIRY_CAMERA))
            .Returns(starredItem);
        _itemTrackingServiceMock.Raise(x => x.ItemStateChanged += null,
            new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating data reload)
        var unstarredItem = new SavedItem { ItemName = ItemName.FAIRY_CAMERA, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.FAIRY_CAMERA))
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

        // Step 1: Toggle the star - change mock to return starred item for FAIRY_CAMERA
        var starredItem = new SavedItem { ItemName = ItemName.FAIRY_CAMERA, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.FAIRY_CAMERA))
            .Returns(starredItem);
        _itemTrackingServiceMock.Raise(x => x.ItemStateChanged += null,
            new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating progress reload)
        var unstarredItem = new SavedItem { ItemName = ItemName.FAIRY_CAMERA, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.FAIRY_CAMERA))
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

        // Step 1: Toggle the star - change mock to return starred item for FAIRY_CAMERA
        var starredItem = new SavedItem { ItemName = ItemName.FAIRY_CAMERA, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.FAIRY_CAMERA))
            .Returns(starredItem);
        _itemTrackingServiceMock.Raise(x => x.ItemStateChanged += null,
            new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating data load)
        var unstarredItem = new SavedItem { ItemName = ItemName.FAIRY_CAMERA, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.FAIRY_CAMERA))
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
        Assert.Equal("camera_shockwave_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithLeftKnownFromSpoilerData_SetsFairyOnlyImage()
    {
        // Arrange - set up spoiler data with FAIRY_CAMERA in a known region
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.FAIRY_CAMERA] = RegionName.JUNGLE_JAPES;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        // Act - create view model with spoiler data configured
        var viewModel = CreateViewModel();

        // Assert - should use fairy camera only image
        Assert.Equal("fairycamonly", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithRightKnownFromTracking_SetsShockwaveOnlyImage()
    {
        // Arrange - set up item tracking service to return SHOCKWAVE in a known region
        var shockwaveItem = new SavedItem { ItemName = ItemName.SHOCKWAVE, Region = RegionName.CRYSTAL_CAVES };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.SHOCKWAVE))
            .Returns(shockwaveItem);

        // Act - create view model with tracking data configured
        var viewModel = CreateViewModel();

        // Assert - should use shockwave only image
        Assert.Equal("shockwaveonly", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithBothItemsKnown_SetsFullImage()
    {
        // Arrange - set up both sources with known items
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.FAIRY_CAMERA] = RegionName.JUNGLE_JAPES;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        var shockwaveItem = new SavedItem { ItemName = ItemName.SHOCKWAVE, Region = RegionName.CRYSTAL_CAVES };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.SHOCKWAVE))
            .Returns(shockwaveItem);

        // Act - create view model with both sources configured
        var viewModel = CreateViewModel();

        // Assert - should use full camera shockwave image
        Assert.Equal("camera_shockwave", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithLeftKnownFromTracking_SetsFairyOnlyImage()
    {
        // Arrange - set up item tracking service to return FAIRY_CAMERA in a known region
        var cameraItem = new SavedItem { ItemName = ItemName.FAIRY_CAMERA, Region = RegionName.JUNGLE_JAPES };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.FAIRY_CAMERA))
            .Returns(cameraItem);

        // Act - create view model with tracking data configured
        var viewModel = CreateViewModel();

        // Assert - should use fairy camera only image
        Assert.Equal("fairycamonly", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithRightKnownFromSpoilerData_SetsShockwaveOnlyImage()
    {
        // Arrange - set up spoiler data with SHOCKWAVE in a known region
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.SHOCKWAVE] = RegionName.CRYSTAL_CAVES;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        // Act - create view model with spoiler data configured
        var viewModel = CreateViewModel();

        // Assert - should use shockwave only image
        Assert.Equal("shockwaveonly", viewModel.ImageResourceKey);
    }

    #endregion
}
