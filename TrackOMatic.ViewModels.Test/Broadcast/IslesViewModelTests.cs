namespace TrackOMatic.ViewModels.Test.Broadcast;

/// <summary>
/// Test coverage for the <see cref="IslesViewModel"/> class.
/// </summary>
public class IslesViewModelTests
{
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock;
    private readonly Mock<ILevelOrderService> _levelOrderServiceMock;
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock;
    private readonly Mock<IUserSettingsService> _userSettingsServiceMock;
    private readonly Mock<IDataSaverService> _dataSaverServiceMock;

    public IslesViewModelTests()
    {
        _itemTrackingServiceMock = new Mock<IItemTrackingService>();
        _levelOrderServiceMock = new Mock<ILevelOrderService>();
        _parsedSpoilerDataServiceMock = new Mock<IParsedSpoilerDataService>();
        _userSettingsServiceMock = new Mock<IUserSettingsService>();
        _dataSaverServiceMock = new Mock<IDataSaverService>();

        // Setup default returns to avoid null reference exceptions
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns((ParsedSpoilerData?)null);
        _userSettingsServiceMock.Setup(x => x.BroadcastNumberLabel).Returns(BroadcastNumberLabel.Points);
    }

    private IslesViewModel CreateViewModel()
    {
        return new IslesViewModel(
            _itemTrackingServiceMock.Object,
            _levelOrderServiceMock.Object,
            _parsedSpoilerDataServiceMock.Object,
            _userSettingsServiceMock.Object,
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
            new IslesViewModel(
                service!,
                _levelOrderServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _userSettingsServiceMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("itemTrackingService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullLevelOrderService_ThrowsArgumentNullException(ILevelOrderService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new IslesViewModel(
                _itemTrackingServiceMock.Object,
                service!,
                _parsedSpoilerDataServiceMock.Object,
                _userSettingsServiceMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("levelOrderService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException(IParsedSpoilerDataService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new IslesViewModel(
                _itemTrackingServiceMock.Object,
                _levelOrderServiceMock.Object,
                service!,
                _userSettingsServiceMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullUserSettingsService_ThrowsArgumentNullException(IUserSettingsService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new IslesViewModel(
                _itemTrackingServiceMock.Object,
                _levelOrderServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                service!,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("userSettingsService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullDataSaverService_ThrowsArgumentNullException(IDataSaverService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new IslesViewModel(
                _itemTrackingServiceMock.Object,
                _levelOrderServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _userSettingsServiceMock.Object,
                service!
            )
        );
        Assert.Equal("dataSaverService", ex.ParamName);
    }

    #endregion

    #region Event Subscription Tests

    [Fact]
    public void Constructor_SubscribesToLevelOrderServiceLevelOrderChanged()
    {
        // Act
        CreateViewModel();

        // Assert
        _levelOrderServiceMock.VerifyAdd(x => x.LevelOrderChanged += It.IsAny<EventHandler<LevelOrderStateChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToItemTrackingServiceItemStateChanged()
    {
        // Act
        CreateViewModel();

        // Assert
        _itemTrackingServiceMock.VerifyAdd(x => x.ItemStateChanged += It.IsAny<EventHandler<ItemStateChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToParsedSpoilerDataServiceParsedSpoilerDataChanged()
    {
        // Act
        CreateViewModel();

        // Assert
        _parsedSpoilerDataServiceMock.VerifyAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToDataSaverServiceDataLoadingCompleted()
    {
        // Act
        CreateViewModel();

        // Assert
        _dataSaverServiceMock.VerifyAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToUserSettingsServicePropertyChanged()
    {
        // Act
        CreateViewModel();

        // Assert
        _userSettingsServiceMock.VerifyAdd(x => x.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.Once);
    }

    #endregion

    #region Initial State Tests

    [Fact]
    public void Constructor_InitializesLevelNumberToNine()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal(9, viewModel.LevelNumber);
    }

    [Fact]
    public void Constructor_InitializesRegionResourceKeyToDkIsles()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal("dk_isles_label", viewModel.RegionResourceKey);
    }

    [Fact]
    public void Constructor_InitializesShowRegionImageToFalse()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.False(viewModel.ShowRegionImage);
    }

    #endregion

    #region UpdatePointsData Tests - No Spoiler

    [Fact]
    public void UpdatePointsData_WithNoSpoilerData_ShowsNoPoints()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns((ParsedSpoilerData?)null);
        var viewModel = CreateViewModel();

        // Act
        // Simulate data reset
        var eventArgs = new EventArgs();
        viewModel.OnDataReset(null, eventArgs);

        // Assert
        Assert.False(viewModel.ShowRegionImage);
        Assert.False(viewModel.ShowRemainingPoints);
        Assert.Equal(-1, viewModel.Points);
        Assert.Equal("RegionInProgress", viewModel.PointColorResource);
    }

    #endregion

    #region UpdatePointsData Tests - Points Only

    [Fact]
    public void UpdatePointsData_WithPointsEnabledOnly_ShowsItemPoints()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(pointsEnabled: true),
            PointSpread = new Dictionary<PointCategory, int>
            {
                { PointCategory.Kong, 5 },
                { PointCategory.Key, 3 }
            }
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointsForRegion(RegionName.DK_ISLES)).Returns(10);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointSpread()).Returns(spoilerData.PointSpread);
        _itemTrackingServiceMock.Setup(x => x.GetItemsInRegion(RegionName.DK_ISLES)).Returns(new List<SavedItem>());

        var viewModel = CreateViewModel();

        // Act
        var eventArgs = new EventArgs();
        viewModel.OnDataReset(null, eventArgs);

        // Assert
        Assert.True(viewModel.ShowRegionImage);
        Assert.True(viewModel.ShowRemainingPoints);
        Assert.Equal(10, viewModel.Points); // No items collected, so full points remaining
        Assert.Equal("RegionInProgress", viewModel.PointColorResource);
    }

    [Fact]
    public void UpdatePointsData_WithPointsEnabledAndItemsCollected_ReducesRemainingPoints()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(pointsEnabled: true),
            PointSpread = new Dictionary<PointCategory, int>
            {
                { PointCategory.Kong, 5 },
                { PointCategory.Key, 3 }
            }
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointsForRegion(RegionName.DK_ISLES)).Returns(10);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointSpread()).Returns(spoilerData.PointSpread);

        var collectedItem = new SavedItem(ItemName.KEY_1, RegionName.DK_ISLES);
        _itemTrackingServiceMock.Setup(x => x.GetItemsInRegion(RegionName.DK_ISLES))
            .Returns(new List<SavedItem> { collectedItem });

        var viewModel = CreateViewModel();

        // Act
        var eventArgs = new EventArgs();
        viewModel.OnDataReset(null, eventArgs);

        // Assert
        Assert.True(viewModel.ShowRegionImage);
        Assert.True(viewModel.ShowRemainingPoints);
        Assert.Equal(7, viewModel.Points); // 10 - 3 (Key points)
        Assert.Equal("RegionInProgress", viewModel.PointColorResource);
    }

    [Fact]
    public void UpdatePointsData_WithPointsEnabledAndAllItemsCollected_ShowsCompleteColor()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(pointsEnabled: true),
            PointSpread = new Dictionary<PointCategory, int>
            {
                { PointCategory.Kong, 5 },
                { PointCategory.Key, 3 }
            }
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointsForRegion(RegionName.DK_ISLES)).Returns(8);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointSpread()).Returns(spoilerData.PointSpread);

        var kongItem = new SavedItem(ItemName.DONKEY, RegionName.DK_ISLES); // Kong item (5 points)
        var keyItem = new SavedItem(ItemName.KEY_7, RegionName.DK_ISLES); // Key item (3 points)
        _itemTrackingServiceMock.Setup(x => x.GetItemsInRegion(RegionName.DK_ISLES))
            .Returns(new List<SavedItem> { kongItem, keyItem });

        var viewModel = CreateViewModel();

        // Act
        var eventArgs = new EventArgs();
        viewModel.OnDataReset(null, eventArgs);

        // Assert
        Assert.True(viewModel.ShowRegionImage);
        Assert.True(viewModel.ShowRemainingPoints);
        Assert.Equal(0, viewModel.Points); // 8 - 5 - 3 = 0
        Assert.Equal("RegionComplete", viewModel.PointColorResource); // Points <= 0 shows RegionComplete
    }

    #endregion

    #region UpdatePointsData Tests - Woth Only

    [Fact]
    public void UpdatePointsData_WithWothEnabledOnly_ShowsHoardPoints()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(wOTHEnabled: true)
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetWothPointsForRegion(RegionName.DK_ISLES)).Returns(5);
        _itemTrackingServiceMock.Setup(x => x.GetItemsInRegion(RegionName.DK_ISLES)).Returns(new List<SavedItem>());

        var viewModel = CreateViewModel();

        // Act
        var eventArgs = new EventArgs();
        viewModel.OnDataReset(null, eventArgs);

        // Assert
        Assert.True(viewModel.ShowRegionImage);
        Assert.True(viewModel.ShowRemainingPoints);
        Assert.Equal(5, viewModel.Points); // No starred items, so full hoard points
        Assert.Equal("RequiredChecksColor", viewModel.PointColorResource);
    }

    [Fact]
    public void UpdatePointsData_WithWothEnabledAndStarredItems_ReducesHoardPoints()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(wOTHEnabled: true)
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetWothPointsForRegion(RegionName.DK_ISLES)).Returns(5);

        var starredItem = new SavedItem(ItemName.RAREWARE_COIN, RegionName.DK_ISLES) { Starred = true };
        var unstarredItem = new SavedItem(ItemName.KEY_1, RegionName.DK_ISLES) { Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemsInRegion(RegionName.DK_ISLES))
            .Returns(new List<SavedItem> { starredItem, unstarredItem });

        var viewModel = CreateViewModel();

        // Act
        var eventArgs = new EventArgs();
        viewModel.OnDataReset(null, eventArgs);

        // Assert
        Assert.True(viewModel.ShowRegionImage);
        Assert.True(viewModel.ShowRemainingPoints);
        Assert.Equal(4, viewModel.Points); // 5 - 1 starred = 4
        Assert.Equal("RequiredChecksColor", viewModel.PointColorResource);
    }

    #endregion

    #region UpdatePointsData Tests - Both Points and Woth

    [Fact]
    public void UpdatePointsData_WithBothPointsAndWothEnabled_UsesItemCountWhenSet()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(pointsEnabled: true, wOTHEnabled: true),
            PointSpread = new Dictionary<PointCategory, int>
            {
                { PointCategory.Kong, 5 },
                { PointCategory.Key, 3 }
            }
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);
        _userSettingsServiceMock.Setup(x => x.BroadcastNumberLabel).Returns(BroadcastNumberLabel.Points);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointsForRegion(RegionName.DK_ISLES)).Returns(10);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointSpread()).Returns(spoilerData.PointSpread);
        _itemTrackingServiceMock.Setup(x => x.GetItemsInRegion(RegionName.DK_ISLES)).Returns(new List<SavedItem>());

        var viewModel = CreateViewModel();

        // Act
        var eventArgs = new EventArgs();
        viewModel.OnDataReset(null, eventArgs);

        // Assert
        Assert.True(viewModel.ShowRegionImage);
        Assert.True(viewModel.ShowRemainingPoints);
        Assert.Equal(10, viewModel.Points); // Uses item points because BroadcastNumberLabel is Points
        Assert.Equal("RegionInProgress", viewModel.PointColorResource);
    }

    [Fact]
    public void UpdatePointsData_WithBothPointsAndWothEnabled_UsesWothCountWhenSet()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(pointsEnabled: true, wOTHEnabled: true),
            PointSpread = new Dictionary<PointCategory, int>
            {
                { PointCategory.Kong, 5 },
                { PointCategory.Key, 3 }
            }
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);
        _userSettingsServiceMock.Setup(x => x.BroadcastNumberLabel).Returns(BroadcastNumberLabel.WothCount);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetWothPointsForRegion(RegionName.DK_ISLES)).Returns(5);
        _itemTrackingServiceMock.Setup(x => x.GetItemsInRegion(RegionName.DK_ISLES)).Returns(new List<SavedItem>());

        var viewModel = CreateViewModel();

        // Act
        var eventArgs = new EventArgs();
        viewModel.OnDataReset(null, eventArgs);

        // Assert
        Assert.True(viewModel.ShowRegionImage);
        Assert.True(viewModel.ShowRemainingPoints);
        Assert.Equal(5, viewModel.Points); // Uses woth points because BroadcastNumberLabel is WothCount
        Assert.Equal("RequiredChecksColor", viewModel.PointColorResource);
    }

    #endregion

    #region User Settings Changed Tests

    [Fact]
    public void OnUserSettingsChanged_WithBroadcastNumberLabelChange_UpdatesRegionData()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(pointsEnabled: true, wOTHEnabled: true),
            PointSpread = new Dictionary<PointCategory, int>
            {
                { PointCategory.Kong, 5 },
                { PointCategory.Key, 3 }
            }
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointsForRegion(RegionName.DK_ISLES)).Returns(10);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointSpread()).Returns(spoilerData.PointSpread);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetWothPointsForRegion(RegionName.DK_ISLES)).Returns(5);
        _itemTrackingServiceMock.Setup(x => x.GetItemsInRegion(RegionName.DK_ISLES)).Returns(new List<SavedItem>());

        _userSettingsServiceMock.Setup(x => x.BroadcastNumberLabel).Returns(BroadcastNumberLabel.Points);
        var viewModel = CreateViewModel();
        var initialPoints = viewModel.Points;

        // Act
        _userSettingsServiceMock.Setup(x => x.BroadcastNumberLabel).Returns(BroadcastNumberLabel.WothCount);
        var eventArgs = new PropertyChangedEventArgs(nameof(IUserSettingsService.BroadcastNumberLabel));
        viewModel.OnUserSettingsChanged(null, eventArgs);

        // Assert
        Assert.Equal(5, viewModel.Points); // Changed to use Woth points
        Assert.NotEqual(initialPoints, viewModel.Points);
    }

    [Fact]
    public void OnUserSettingsChanged_WithOtherPropertyChange_DoesNotUpdateRegionData()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(pointsEnabled: true)
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointsForRegion(RegionName.DK_ISLES)).Returns(10);
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointSpread()).Returns(spoilerData.PointSpread);
        _itemTrackingServiceMock.Setup(x => x.GetItemsInRegion(RegionName.DK_ISLES)).Returns(new List<SavedItem>());

        var viewModel = CreateViewModel();
        var initialPoints = viewModel.Points;

        // Act
        var eventArgs = new PropertyChangedEventArgs("SomeOtherProperty");
        viewModel.OnUserSettingsChanged(null, eventArgs);

        // Assert
        Assert.Equal(initialPoints, viewModel.Points); // Points should not have changed
    }

    #endregion

    #region Object Override Tests

    [Fact]
    public void Equals_WithSameSelectedLevel_ReturnsTrue()
    {
        // Arrange
        var viewModel1 = CreateViewModel();
        var viewModel2 = CreateViewModel();

        // Act
        var areEqual = viewModel1.Equals(viewModel2);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Equals_WithDifferentSelectedLevel_ReturnsFalse()
    {
        // Arrange
        var viewModel1 = CreateViewModel();
        var viewModel2 = new IslesViewModel(
            5,
            _itemTrackingServiceMock.Object,
            _levelOrderServiceMock.Object,
            _parsedSpoilerDataServiceMock.Object,
            _userSettingsServiceMock.Object,
            _dataSaverServiceMock.Object
        );

        // Act
        var areEqual = viewModel1.Equals(viewModel2);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void GetHashCode_WithSameSelectedLevel_ReturnsSameHashCode()
    {
        // Arrange
        var viewModel1 = CreateViewModel();
        var viewModel2 = CreateViewModel();

        // Act
        var hash1 = viewModel1.GetHashCode();
        var hash2 = viewModel2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_WithDifferentSelectedLevel_ReturnsDifferentHashCode()
    {
        // Arrange
        var viewModel1 = CreateViewModel();
        var viewModel2 = new IslesViewModel(
            5,
            _itemTrackingServiceMock.Object,
            _levelOrderServiceMock.Object,
            _parsedSpoilerDataServiceMock.Object,
            _userSettingsServiceMock.Object,
            _dataSaverServiceMock.Object
        );

        // Act
        var hash1 = viewModel1.GetHashCode();
        var hash2 = viewModel2.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
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
            () => _levelOrderServiceMock.VerifyRemove(x => x.LevelOrderChanged -= It.IsAny<EventHandler<LevelOrderStateChangedEventArgs>>(), Times.Once),
            () => _parsedSpoilerDataServiceMock.VerifyRemove(x => x.ParsedSpoilerDataChanged -= It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once)
        );
    }

    #endregion
}
