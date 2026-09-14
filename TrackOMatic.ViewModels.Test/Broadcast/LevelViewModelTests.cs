namespace TrackOMatic.ViewModels.Test.Broadcast;

/// <summary>
/// Test coverage for the <see cref="LevelViewModel"/> class.
/// </summary>
public class LevelViewModelTests
{
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock;
    private readonly Mock<ILevelOrderService> _levelOrderServiceMock;
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock;
    private readonly Mock<IUserSettingsService> _userSettingsServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;
    private readonly Mock<IDataSaverService> _dataSaverServiceMock;

    public LevelViewModelTests()
    {
        _itemTrackingServiceMock = new Mock<IItemTrackingService>();
        _levelOrderServiceMock = new Mock<ILevelOrderService>();
        _parsedSpoilerDataServiceMock = new Mock<IParsedSpoilerDataService>();
        _userSettingsServiceMock = new Mock<IUserSettingsService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
        _dataSaverServiceMock = new Mock<IDataSaverService>();

        // Setup default returns
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns((ParsedSpoilerData?)null);
        _userSettingsServiceMock.Setup(x => x.BroadcastNumberLabel).Returns(BroadcastNumberLabel.Points);
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns(new List<int> { 1, 2, 3, 4, 5 });
    }

    private LevelViewModel CreateViewModel(int selectedLevel = 1)
    {
        return new LevelViewModel(
            selectedLevel,
            _itemTrackingServiceMock.Object,
            _levelOrderServiceMock.Object,
            _parsedSpoilerDataServiceMock.Object,
            _userSettingsServiceMock.Object,
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

    [Fact]
    public void Constructor_WithValidDependencies_CreatesKeyViewModel()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel.KeyViewModel);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException(IItemTrackingService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new LevelViewModel(
                1,
                service!,
                _levelOrderServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _userSettingsServiceMock.Object,
                _savedProgressProviderMock.Object,
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
            new LevelViewModel(
                1,
                _itemTrackingServiceMock.Object,
                service!,
                _parsedSpoilerDataServiceMock.Object,
                _userSettingsServiceMock.Object,
                _savedProgressProviderMock.Object,
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
            new LevelViewModel(
                1,
                _itemTrackingServiceMock.Object,
                _levelOrderServiceMock.Object,
                service!,
                _userSettingsServiceMock.Object,
                _savedProgressProviderMock.Object,
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
            new LevelViewModel(
                1,
                _itemTrackingServiceMock.Object,
                _levelOrderServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                service!,
                _savedProgressProviderMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("userSettingsService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException(ISavedProgressProvider? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new LevelViewModel(
                1,
                _itemTrackingServiceMock.Object,
                _levelOrderServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _userSettingsServiceMock.Object,
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
            new LevelViewModel(
                1,
                _itemTrackingServiceMock.Object,
                _levelOrderServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _userSettingsServiceMock.Object,
                _savedProgressProviderMock.Object,
                service!
            )
        );
        Assert.Equal("dataSaverService", ex.ParamName);
    }

    #endregion

    #region Initial State Tests

    [Fact]
    public void Constructor_InitializesNumberResourceKeyWithLevel()
    {
        // Act
        var viewModel = CreateViewModel(selectedLevel: 3);

        // Assert
        Assert.Equal("number_3", viewModel.NumberResourceKey);
    }

    [Fact]
    public void Constructor_InitializesImageResourceKeyToBWKey()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal("basic_key_bw", viewModel.ImageResourceKey);
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

    #region OnItemStateChanged Override Tests

    [Fact]
    public void OnItemStateChanged_WithUnrelatedItemChange_DoesNotUpdate()
    {
        // Arrange
        var viewModel = CreateViewModel(selectedLevel: 2);
        var initialImageKey = viewModel.ImageResourceKey;

        // Act - Change a different item (KEY_1 instead of KEY_2)
        var unrelatedItem = new SavedItem(ItemName.KEY_1, RegionName.GLOOMY_GALLEON);
        var eventArgs = new ItemStateChangedEventArgs(unrelatedItem);
        viewModel.OnItemStateChanged(null, eventArgs);

        // Assert
        Assert.Equal(initialImageKey, viewModel.ImageResourceKey);
    }

    [Fact]
    public void OnItemStateChanged_WithMatchingKeyChange_UpdatesImage()
    {
        // Arrange
        var viewModel = CreateViewModel(selectedLevel: 2);

        // Act - Change the matching KEY_2
        var matchingItem = new SavedItem(ItemName.KEY_2, RegionName.JUNGLE_JAPES);
        var eventArgs = new ItemStateChangedEventArgs(matchingItem);
        viewModel.OnItemStateChanged(null, eventArgs);

        // Assert
        Assert.Equal("basic_key", viewModel.ImageResourceKey); // Item is now in a known region
    }

    [Fact]
    public void OnItemStateChanged_WithMatchingKeyHinted_ShowsBWImage()
    {
        // Arrange
        var viewModel = CreateViewModel(selectedLevel: 1);

        // Act - Mark KEY_1 as hinted
        var hintedItem = new SavedItem(ItemName.KEY_1, RegionName.JUNGLE_JAPES) { Hinted = true };
        var eventArgs = new ItemStateChangedEventArgs(hintedItem);
        viewModel.OnItemStateChanged(null, eventArgs);

        // Assert
        Assert.Equal("basic_key_bw", viewModel.ImageResourceKey); // Hinted items always B&W
    }

    [Fact]
    public void OnItemStateChanged_WithMatchingKeyStarred_UpdatesIsStarred()
    {
        // Arrange
        var viewModel = CreateViewModel(selectedLevel: 1);
        Assert.False(viewModel.IsStarred);

        // Act
        var starredItem = new SavedItem(ItemName.KEY_1, RegionName.JUNGLE_JAPES) { Starred = true };
        var eventArgs = new ItemStateChangedEventArgs(starredItem);
        viewModel.OnItemStateChanged(null, eventArgs);

        // Assert
        Assert.True(viewModel.IsStarred);
    }

    #endregion

    #region UpdateRegionData Override Tests

    [Fact]
    public void UpdateRegionData_WithoutSpoilerData_ShowsUnknownRegion()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns((ParsedSpoilerData?)null);
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns(new List<int>());  // Empty level order

        // Act
        var viewModel = CreateViewModel(selectedLevel: 2);

        // Assert
        Assert.Equal("unknown_label", viewModel.RegionResourceKey);
    }

    [Fact]
    public void UpdateRegionData_WithSpoilerDataAndLevelOrder_UsesLevelOrder()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>
            {
                { RegionName.JUNGLE_JAPES, 1 },
                { RegionName.ANGRY_AZTEC, 2 },
                { RegionName.FRANTIC_FACTORY, 3 }
            },
            SpoilerSettings = new SpoilerSettings()
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        // Act
        var viewModel = CreateViewModel(selectedLevel: 2);

        // Assert
        Assert.Equal("angry_aztec_label", viewModel.RegionResourceKey);
    }

    [Fact]
    public void UpdateRegionData_WithoutSpoilerDataUsesLevelOrderService()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings()
        });
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns(new List<int> { 1, 2, 3, 4, 5 });

        // Act
        var viewModel = CreateViewModel(selectedLevel: 2);

        // Assert
        Assert.Equal("angry_aztec_label", viewModel.RegionResourceKey);
    }

    [Fact]
    public void UpdateRegionData_WithLevelBeyondOrderBounds_ShowsUnknown()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings()
        });
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns(new List<int> { 1, 2 }); // Only 2 levels

        // Act
        var viewModel = CreateViewModel(selectedLevel: 5); // Level 5 doesn't exist

        // Assert
        Assert.Equal("unknown_label", viewModel.RegionResourceKey);
    }

    #endregion

    #region UpdatePointsData Override Tests

    [Fact]
    public void UpdatePointsData_WithUnknownRegion_HidesPointsDisplay()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns((ParsedSpoilerData?)null);

        // Act
        var viewModel = CreateViewModel(selectedLevel: 5);

        // Assert
        Assert.False(viewModel.ShowRemainingPoints);
        Assert.Equal(-1, viewModel.Points);
        Assert.Equal("RegionInProgress", viewModel.PointColorResource);
    }

    [Fact]
    public void UpdatePointsData_WithKnownRegionAndNoSpoiler_ShowsPoints()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>
            {
                { RegionName.JUNGLE_JAPES, 1 }
            },
            SpoilerSettings = new SpoilerSettings()
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        // Act
        var viewModel = CreateViewModel(selectedLevel: 1);

        // Assert
        Assert.False(viewModel.ShowRemainingPoints);
    }

    #endregion

    #region UpdateImageResourceKey Tests

    [Fact]
    public void UpdateImageResourceKey_WithNoItemStateAndNoSpoiler_ShowsBWImage()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings()
        });
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns((SavedItem?)null);

        // Act
        var viewModel = CreateViewModel(selectedLevel: 1);

        // Assert
        Assert.Equal("basic_key_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithHintedItem_ShowsBWImage()
    {
        // Arrange
        var hintedItem = new SavedItem(ItemName.KEY_1, RegionName.JUNGLE_JAPES) { Hinted = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(hintedItem);

        // Act
        var viewModel = CreateViewModel(selectedLevel: 1);

        // Assert
        Assert.Equal("basic_key_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithUserHintedItem_ShowsBWImage()
    {
        // Arrange
        var userHintedItem = new SavedItem(ItemName.KEY_1, RegionName.JUNGLE_JAPES) { Opacity = 0.5 };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(userHintedItem);

        // Act
        var viewModel = CreateViewModel(selectedLevel: 1);

        // Assert
        Assert.Equal("basic_key_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithSpoiledRegion_ShowsBWImage()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(),
            StartingItems = new Dictionary<ItemName, RegionName>
            {
                { ItemName.KEY_1, RegionName.JUNGLE_JAPES } // Spoiled to level 1's region
            }
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns((SavedItem?)null);

        // Act
        var viewModel = CreateViewModel(selectedLevel: 1);

        // Assert
        // B&W because spoiled region is item's starting location (not a valid move region)
        Assert.Equal("basic_key_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithValidMoveRegion_ShowsBWImage()
    {
        // Arrange
        // KEY_1 placed in JUNGLE_JAPES (level 1's region)
        var item = new SavedItem(ItemName.KEY_1, RegionName.JUNGLE_JAPES);
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(item);

        // Act
        var viewModel = CreateViewModel(selectedLevel: 1);

        // Assert
        // B&W because item is in level's region (not a valid move region)
        Assert.Equal("basic_key_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithFactoryRegion_ShowsBWImage()
    {
        // Arrange
        var item = new SavedItem(ItemName.KEY_1, RegionName.FRANTIC_FACTORY);
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(item);

        // Act
        var viewModel = CreateViewModel(selectedLevel: 1);

        // Assert
        Assert.Equal("basic_key_bw", viewModel.ImageResourceKey);
    }

    #endregion

    #region Object Override Tests

    [Fact]
    public void Equals_WithSameSelectedLevel_ReturnsTrue()
    {
        // Arrange
        var viewModel1 = CreateViewModel(selectedLevel: 1);
        var viewModel2 = CreateViewModel(selectedLevel: 1);

        // Act
        var areEqual = viewModel1.Equals((object)viewModel2);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Equals_WithDifferentSelectedLevel_ReturnsFalse()
    {
        // Arrange
        var viewModel1 = CreateViewModel(selectedLevel: 1);
        var viewModel2 = CreateViewModel(selectedLevel: 2);

        // Act
        var areEqual = viewModel1.Equals(viewModel2);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void GetHashCode_WithSameSelectedLevel_ReturnsSameHashCode()
    {
        // Arrange
        var viewModel1 = CreateViewModel(selectedLevel: 1);
        var viewModel2 = CreateViewModel(selectedLevel: 1);

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
        var viewModel1 = CreateViewModel(selectedLevel: 1);
        var viewModel2 = CreateViewModel(selectedLevel: 2);

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
