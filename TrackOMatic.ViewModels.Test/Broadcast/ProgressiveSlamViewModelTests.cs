namespace TrackOMatic.ViewModels.Test.Broadcast;

/// <summary>
/// Test coverage for ProgressiveSlamViewModel.
/// Tests constructor validation, event subscriptions, and state initialization.
/// Follows BroadcastItemViewModel patterns for multi-stage items.
/// </summary>
public class ProgressiveSlamViewModelTests
{
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock;
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;
    private readonly Mock<IDataSaverService> _dataSaverServiceMock;

    public ProgressiveSlamViewModelTests()
    {
        _itemTrackingServiceMock = new Mock<IItemTrackingService>();
        _parsedSpoilerDataServiceMock = new Mock<IParsedSpoilerDataService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
        _dataSaverServiceMock = new Mock<IDataSaverService>();
    }

    private ProgressiveSlamViewModel CreateViewModel()
    {
        return new ProgressiveSlamViewModel(
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
            new ProgressiveSlamViewModel(
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
            new ProgressiveSlamViewModel(
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
            new ProgressiveSlamViewModel(
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
            new ProgressiveSlamViewModel(
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

        // Step 1: Toggle the star - change mock to return starred item for PROGRESSIVE_SLAM_1
        var starredItem = new SavedItem { ItemName = ItemName.PROGRESSIVE_SLAM_1, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.PROGRESSIVE_SLAM_1))
            .Returns(starredItem);
        _itemTrackingServiceMock.Raise(x => x.ItemStateChanged += null,
            new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating data reload)
        var unstarredItem = new SavedItem { ItemName = ItemName.PROGRESSIVE_SLAM_1, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.PROGRESSIVE_SLAM_1))
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

        // Step 1: Toggle the star - change mock to return starred item for PROGRESSIVE_SLAM_1
        var starredItem = new SavedItem { ItemName = ItemName.PROGRESSIVE_SLAM_1, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.PROGRESSIVE_SLAM_1))
            .Returns(starredItem);
        _itemTrackingServiceMock.Raise(x => x.ItemStateChanged += null,
            new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating progress reload)
        var unstarredItem = new SavedItem { ItemName = ItemName.PROGRESSIVE_SLAM_1, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.PROGRESSIVE_SLAM_1))
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

        // Step 1: Toggle the star - change mock to return starred item for PROGRESSIVE_SLAM_1
        var starredItem = new SavedItem { ItemName = ItemName.PROGRESSIVE_SLAM_1, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.PROGRESSIVE_SLAM_1))
            .Returns(starredItem);
        _itemTrackingServiceMock.Raise(x => x.ItemStateChanged += null,
            new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating data load)
        var unstarredItem = new SavedItem { ItemName = ItemName.PROGRESSIVE_SLAM_1, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.PROGRESSIVE_SLAM_1))
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
    public void UpdateImageResourceKey_WithNoSlamItemsKnown_SetsZeroBWImage()
    {
        // Arrange - no slams known (default mock behavior)
        var viewModel = CreateViewModel();

        // Assert - should use zero slam b/w image when no slams are known
        Assert.Equal("progressive_slam_1_bc_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithOneSlamKnown_SetsOneSlamImage()
    {
        // Arrange - only PROGRESSIVE_SLAM_1 known from spoiler data
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.PROGRESSIVE_SLAM_1] = RegionName.JUNGLE_JAPES;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        // Act - create view model with one slam in spoiler data
        var viewModel = CreateViewModel();

        // Assert - should use one slam image
        Assert.Equal("progressive_slam_1_bc", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithTwoSlamsKnown_SetsTwoSlamImage()
    {
        // Arrange - PROGRESSIVE_SLAM_1 and PROGRESSIVE_SLAM_2 known from different sources
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.PROGRESSIVE_SLAM_1] = RegionName.JUNGLE_JAPES;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        var slam2Item = new SavedItem { ItemName = ItemName.PROGRESSIVE_SLAM_2, Region = RegionName.CRYSTAL_CAVES };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.PROGRESSIVE_SLAM_2))
            .Returns(slam2Item);

        // Act - create view model with two slams known
        var viewModel = CreateViewModel();

        // Assert - should use two slam image
        Assert.Equal("progressive_slam_2_bc", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithThreeSlamsKnown_SetsThreeSlamImage()
    {
        // Arrange - all three slams known from different sources
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.PROGRESSIVE_SLAM_1] = RegionName.JUNGLE_JAPES;
        spoilerData.StartingItems[ItemName.PROGRESSIVE_SLAM_2] = RegionName.CRYSTAL_CAVES;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        var slam3Item = new SavedItem { ItemName = ItemName.PROGRESSIVE_SLAM_3, Region = RegionName.GLOOMY_GALLEON };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.PROGRESSIVE_SLAM_3))
            .Returns(slam3Item);

        // Act - create view model with all three slams known
        var viewModel = CreateViewModel();

        // Assert - should use three slam image
        Assert.Equal("progressive_slam_3_bc", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithSlamsInUnknownRegions_CountsAsUnknown()
    {
        // Arrange - slams in spoiler data but with UNKNOWN region (should not count)
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.PROGRESSIVE_SLAM_1] = RegionName.UNKNOWN;
        spoilerData.StartingItems[ItemName.PROGRESSIVE_SLAM_2] = RegionName.UNKNOWN;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        // Act - create view model with slams in unknown regions
        var viewModel = CreateViewModel();

        // Assert - should use zero slam b/w image (items in unknown regions don't count)
        Assert.Equal("progressive_slam_1_bc_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithMixedSourcesForSlams_CountsBoothSources()
    {
        // Arrange - SLAM_1 from spoiler, SLAM_2 from tracking, SLAM_3 unknown
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.PROGRESSIVE_SLAM_1] = RegionName.JUNGLE_JAPES;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        var slam2Item = new SavedItem { ItemName = ItemName.PROGRESSIVE_SLAM_2, Region = RegionName.CRYSTAL_CAVES };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.PROGRESSIVE_SLAM_2))
            .Returns(slam2Item);

        // Act - create view model with mixed sources
        var viewModel = CreateViewModel();

        // Assert - should use two slam image (counting both spoiler and tracked items)
        Assert.Equal("progressive_slam_2_bc", viewModel.ImageResourceKey);
    }

    #endregion
}
