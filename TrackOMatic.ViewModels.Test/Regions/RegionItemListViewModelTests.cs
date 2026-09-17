namespace TrackOMatic.ViewModels.Test.Regions;

public class RegionItemListViewModelTests
{
    private readonly Mock<IItemTrackingService> _mockItemTrackingService;
    private readonly Mock<IParsedSpoilerDataService> _mockParsedSpoilerDataService;
    private readonly Mock<ISavedProgressProvider> _mockSavedProgressProvider;
    private readonly Mock<IThemeService> _mockThemeService;
    private readonly Mock<IDataSaverService> _mockDataSaverService;

    public RegionItemListViewModelTests()
    {
        _mockItemTrackingService = new Mock<IItemTrackingService>();
        _mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        _mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        _mockThemeService = new Mock<IThemeService>();
        _mockDataSaverService = new Mock<IDataSaverService>();

        // Setup default mock behaviors
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(It.IsAny<RegionName>()))
            .Returns([]);

        var defaultSpoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                { RegionName.JUNGLE_JAPES, new RegionSpoilerData() },
                { RegionName.ANGRY_AZTEC, new RegionSpoilerData() },
                { RegionName.FRANTIC_FACTORY, new RegionSpoilerData() },
                { RegionName.GLOOMY_GALLEON, new RegionSpoilerData() },
                { RegionName.FUNGI_FOREST, new RegionSpoilerData() },
                { RegionName.CRYSTAL_CAVES, new RegionSpoilerData() },
                { RegionName.CREEPY_CASTLE, new RegionSpoilerData() },
                { RegionName.HIDEOUT_HELM, new RegionSpoilerData() },
                { RegionName.DK_ISLES, new RegionSpoilerData() },
                { RegionName.SHOPS, new RegionSpoilerData() },
                { RegionName.START, new RegionSpoilerData() },
                { RegionName.UNHINTABLE_MOVES, new RegionSpoilerData() },
                { RegionName.UNKNOWN, new RegionSpoilerData() }
            }
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(defaultSpoilerData);
        _mockItemTrackingService.Setup(s => s.BeginBatchUpdate())
            .Returns(new Mock<IDisposable>().Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidServices_Succeeds()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.NotNull(vm);
        Assert.Equal(RegionName.JUNGLE_JAPES, vm.RegionName);
    }

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RegionItemListViewModel(
                RegionName.JUNGLE_JAPES,
                null!,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                _mockDataSaverService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RegionItemListViewModel(
                RegionName.JUNGLE_JAPES,
                _mockItemTrackingService.Object,
                null!,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                _mockDataSaverService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RegionItemListViewModel(
                RegionName.JUNGLE_JAPES,
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                null!,
                _mockThemeService.Object,
                _mockDataSaverService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullThemeService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RegionItemListViewModel(
                RegionName.JUNGLE_JAPES,
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                null!,
                _mockDataSaverService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullDataSaverService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RegionItemListViewModel(
                RegionName.JUNGLE_JAPES,
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                null!
            )
        );
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void Constructor_SubscribesToItemStateChanged()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        _mockItemTrackingService.VerifyAdd(
            s => s.ItemStateChanged += It.IsAny<EventHandler<ItemStateChangedEventArgs>>(),
            Times.Once
        );
    }

    [Fact]
    public void Constructor_SubscribesToProgressChanged()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        _mockSavedProgressProvider.VerifyAdd(
            s => s.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>(),
            Times.Once
        );
    }

    [Fact]
    public void Constructor_SubscribesToParsedSpoilerDataChanged()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        _mockParsedSpoilerDataService.VerifyAdd(
            s => s.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(),
            Times.Once
        );
    }

    [Fact]
    public void Constructor_SubscribesToDataLoadingCompleted()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        _mockDataSaverService.VerifyAdd(
            s => s.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(),
            Times.Once
        );
    }

    [Fact]
    public void Constructor_WithNoItems_ItemsCollectionIsEmpty()
    {
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([]);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Empty(vm.Items);
    }

    #endregion

    #region Items Population Tests

    [Fact]
    public void Constructor_WithItemsInRegion_PopulatesItemsCollection()
    {
        var savedItems = new[]
        {
            new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES },
            new SavedItem { ItemName = ItemName.KEY_2, Region = RegionName.JUNGLE_JAPES }
        };

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns(savedItems);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(2, vm.Items.Count);
        Assert.All(vm.Items, item => Assert.IsType<RegionItemViewModel>(item));
    }

    [Fact]
    public void Constructor_CreatesRegionItemVMsWithCorrectItemNames()
    {
        var key1 = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES };
        var key2 = new SavedItem { ItemName = ItemName.KEY_2, Region = RegionName.JUNGLE_JAPES };

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1, key2]);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(ItemName.KEY_1, vm.Items[0].CurrentItemName);
        Assert.Equal(ItemName.KEY_2, vm.Items[1].CurrentItemName);
    }

    [Fact]
    public void Constructor_BeginsBatchUpdate()
    {
        var mockBatchDisposable = new Mock<IDisposable>();
        _mockItemTrackingService.Setup(s => s.BeginBatchUpdate())
            .Returns(mockBatchDisposable.Object);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        _mockItemTrackingService.Verify(s => s.BeginBatchUpdate(), Times.Once);
        mockBatchDisposable.Verify(d => d.Dispose(), Times.Once);
    }

    #endregion

    #region TryAcceptDrop Tests

    [Fact]
    public void TryAcceptDrop_WithMissingItemState_ReturnsTrue()
    {
        _mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>()))
            .Returns((SavedItem?)null);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAcceptDrop(ItemName.KEY_1, MouseDragType.Left);

        Assert.True(result);
    }

    [Fact]
    public void TryAcceptDrop_WithAutoTrackedItem_ReturnsFalse()
    {
        var autoTrackedItem = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES, Autotracked = true };
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.KEY_1))
            .Returns(autoTrackedItem);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAcceptDrop(ItemName.KEY_1, MouseDragType.Left);

        Assert.False(result);
    }

    [Theory]
    [InlineData(MouseDragType.Left)]
    [InlineData(MouseDragType.Right)]
    public void TryAcceptDrop_WithItemInStartRegion_ReturnsFalseForNonNoneDragType(MouseDragType dragType)
    {
        var startItem = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.START, Autotracked = false };
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.KEY_1))
            .Returns(startItem);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAcceptDrop(ItemName.KEY_1, dragType);

        Assert.False(result);
    }

    [Fact]
    public void TryAcceptDrop_WithItemInStartRegion_ReturnsTrueForNoneDragType()
    {
        var startItem = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.START, Autotracked = false };
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.KEY_1))
            .Returns(startItem);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAcceptDrop(ItemName.KEY_1, MouseDragType.None);

        Assert.True(result);
    }

    [Theory]
    [InlineData(MouseDragType.Left)]
    [InlineData(MouseDragType.Right)]
    public void TryAcceptDrop_WithItemInUnhintableMovesRegion_ReturnsFalseForNonNoneDragType(MouseDragType dragType)
    {
        var unhintableItem = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.UNHINTABLE_MOVES, Autotracked = false };
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.KEY_1))
            .Returns(unhintableItem);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAcceptDrop(ItemName.KEY_1, dragType);

        Assert.False(result);
    }

    [Fact]
    public void TryAcceptDrop_WithItemInUnhintableMovesRegion_ReturnsTrueForNoneDragType()
    {
        var unhintableItem = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.UNHINTABLE_MOVES, Autotracked = false };
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.KEY_1))
            .Returns(unhintableItem);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAcceptDrop(ItemName.KEY_1, MouseDragType.None);

        Assert.True(result);
    }

    [Theory]
    [InlineData(MouseDragType.Left)]
    [InlineData(MouseDragType.Right)]
    [InlineData(MouseDragType.None)]
    public void TryAcceptDrop_WithItemInNormalRegion_ReturnsTrue(MouseDragType dragType)
    {
        var normalItem = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES, Autotracked = false };
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.KEY_1))
            .Returns(normalItem);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAcceptDrop(ItemName.KEY_1, dragType);

        Assert.True(result);
    }

    #endregion

    #region Event Handler Tests

    [Fact]
    public void OnItemStateChanged_WithItemMovingToThisRegion_RefreshesItems()
    {
        var key1 = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES };
        var key2 = new SavedItem { ItemName = ItemName.KEY_2, Region = RegionName.JUNGLE_JAPES };

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1]);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Single(vm.Items);

        // Update mock to return two items
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1, key2]);

        // Simulate item moved to this region
        var eventArgs = new ItemStateChangedEventArgs(
            previousState: new SavedItem { ItemName = ItemName.KEY_2, Region = RegionName.ANGRY_AZTEC },
            updatedItem: new SavedItem { ItemName = ItemName.KEY_2, Region = RegionName.JUNGLE_JAPES }
        );

        _mockItemTrackingService.Raise(s => s.ItemStateChanged += null, null!, eventArgs);

        Assert.Equal(2, vm.Items.Count);
    }

    [Fact]
    public void OnItemStateChanged_WithItemMovingFromThisRegion_RefreshesItems()
    {
        var key1 = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES };
        var key2 = new SavedItem { ItemName = ItemName.KEY_2, Region = RegionName.JUNGLE_JAPES };

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1, key2]);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(2, vm.Items.Count);

        // Update mock to return one item
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1]);

        // Simulate item moved away from this region
        var eventArgs = new ItemStateChangedEventArgs(
            previousState: new SavedItem { ItemName = ItemName.KEY_2, Region = RegionName.JUNGLE_JAPES },
            updatedItem: new SavedItem { ItemName = ItemName.KEY_2, Region = RegionName.ANGRY_AZTEC }
        );

        _mockItemTrackingService.Raise(s => s.ItemStateChanged += null, null!, eventArgs);

        Assert.Single(vm.Items);
    }

    [Fact]
    public void OnItemStateChanged_WithItemNotAffectingThisRegion_DoesNotRefresh()
    {
        var key1 = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES };

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1]);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var initialCount = vm.Items.Count;

        // Simulate item moved between two other regions
        var eventArgs = new ItemStateChangedEventArgs(
            previousState: new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.ANGRY_AZTEC },
            updatedItem: new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.FRANTIC_FACTORY }
        );

        _mockItemTrackingService.Raise(s => s.ItemStateChanged += null, null!, eventArgs);

        // Items should remain unchanged
        Assert.Equal(initialCount, vm.Items.Count);
    }

    [Fact]
    public void OnDataReset_RefreshesItems()
    {
        var key1 = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES };

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1]);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        // Clear the initial items mock setup
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([]);

        var eventArgs = new ProgressReplacedEventArgs { NewProgress = new SavedProgress(), OldProgress = null };
        _mockSavedProgressProvider.Raise(s => s.ProgressChanged += null, null!, eventArgs);

        Assert.Empty(vm.Items);
    }

    [Fact]
    public void OnDataLoadingCompleted_RefreshesItems()
    {
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([]);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var key1 = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES };
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1]);

        var eventArgs = new DataLoadingCompletedEventArgs(new SavedProgress());
        _mockDataSaverService.Raise(s => s.DataLoadingCompleted += null, null!, eventArgs);

        Assert.Single(vm.Items);
    }

    [Fact]
    public void OnParsedSpoilerDataChanged_RefreshesItems()
    {
        var key1 = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES };

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1]);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([]);

        var eventArgs = new ParsedSpoilerDataChangedEventArgs(null, null);
        _mockParsedSpoilerDataService.Raise(s => s.ParsedSpoilerDataChanged += null, null!, eventArgs);

        Assert.Empty(vm.Items);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameRegion_ReturnsTrue()
    {
        var vm1 = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var vm2 = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(vm1, vm2);
    }

    [Fact]
    public void Equals_WithDifferentRegion_ReturnsFalse()
    {
        var vm1 = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var vm2 = new RegionItemListViewModel(
            RegionName.ANGRY_AZTEC,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.NotEqual(vm1, vm2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.False(vm.Equals((RegionItemListViewModel?)null));
    }

    [Fact]
    public void Equals_WithSameReference_ReturnsTrue()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(vm, vm);
    }

    [Fact]
    public void EqualsObject_WithSameRegion_ReturnsTrue()
    {
        var vm1 = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var vm2 = (object)new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(vm1, vm2);
    }

    [Fact]
    public void EqualsObject_WithNull_ReturnsFalse()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.False(vm.Equals((object?)null));
    }

    [Fact]
    public void EqualsObject_WithDifferentType_ReturnsFalse()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.False(vm.Equals("not a RegionItemListViewModel"));
    }

    [Fact]
    public void GetHashCode_WithSameRegion_ReturnsSameHash()
    {
        var vm1 = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var vm2 = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(vm1.GetHashCode(), vm2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentRegion_ReturnsDifferentHash()
    {
        var vm1 = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var vm2 = new RegionItemListViewModel(
            RegionName.ANGRY_AZTEC,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.NotEqual(vm1.GetHashCode(), vm2.GetHashCode());
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithNoItems_IncludesZeroCount()
    {
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([]);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.ToString();

        Assert.Contains("JUNGLE_JAPES", result);
        Assert.Contains("0", result);
    }

    [Fact]
    public void ToString_WithItems_IncludesItemCount()
    {
        var items = new[]
        {
            new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES },
            new SavedItem { ItemName = ItemName.KEY_2, Region = RegionName.JUNGLE_JAPES }
        };

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns(items);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.ToString();

        Assert.Contains("JUNGLE_JAPES", result);
        Assert.Contains("2", result);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromItemStateChanged()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        vm.Dispose();

        _mockItemTrackingService.VerifyRemove(
            s => s.ItemStateChanged -= It.IsAny<EventHandler<ItemStateChangedEventArgs>>(),
            Times.Once
        );
    }

    [Fact]
    public void Dispose_UnsubscribesFromProgressChanged()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        vm.Dispose();

        _mockSavedProgressProvider.VerifyRemove(
            s => s.ProgressChanged -= It.IsAny<EventHandler<ProgressReplacedEventArgs>>(),
            Times.Once
        );
    }

    [Fact]
    public void Dispose_UnsubscribesFromParsedSpoilerDataChanged()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        vm.Dispose();

        _mockParsedSpoilerDataService.VerifyRemove(
            s => s.ParsedSpoilerDataChanged -= It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(),
            Times.Once
        );
    }

    [Fact]
    public void Dispose_UnsubscribesFromDataLoadingCompleted()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        vm.Dispose();

        _mockDataSaverService.VerifyRemove(
            s => s.DataLoadingCompleted -= It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(),
            Times.Once
        );
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        vm.Dispose();
        vm.Dispose(); // Should not throw

        Assert.True(true);
    }

    #endregion

    #region Interface Tests

    [Fact]
    public void ImplementsIItemListViewModel()
    {
        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.IsType<IItemListViewModel>(vm, false);
    }

    [Fact]
    public void IItemListViewModel_RegionName_ReturnsCorrectRegion()
    {
        RegionItemListViewModel vm = new(
            RegionName.FRANTIC_FACTORY,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(RegionName.FRANTIC_FACTORY, vm.RegionName);
    }

    [Fact]
    public void IItemListViewModel_TryAcceptDrop_CallsImplementation()
    {
        var normalItem = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES, Autotracked = false };
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.KEY_1))
            .Returns(normalItem);

        RegionItemListViewModel vm = new(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAcceptDrop(ItemName.KEY_1, MouseDragType.Left);

        Assert.True(result);
    }

    #endregion

    #region Edge Cases and Stress Tests

    [Fact]
    public void RefreshPlacedItems_WithMultipleRefreshes_HandlesCorrectly()
    {
        var key1 = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES };
        var key2 = new SavedItem { ItemName = ItemName.KEY_2, Region = RegionName.JUNGLE_JAPES };

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1]);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Single(vm.Items);

        // Refresh with two items
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1, key2]);
        var eventArgs = new ParsedSpoilerDataChangedEventArgs(null, null);
        _mockParsedSpoilerDataService.Raise(s => s.ParsedSpoilerDataChanged += null, null!, eventArgs);

        Assert.Equal(2, vm.Items.Count);

        // Refresh with one item
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([key1]);
        _mockSavedProgressProvider.Raise(s => s.ProgressChanged += null, null!, new ProgressReplacedEventArgs { NewProgress = new SavedProgress(), OldProgress = null });

        Assert.Single(vm.Items);
    }

    [Fact]
    public void Constructor_WithLargeItemList_PopulatesCorrectly()
    {
        var manyItems = Enumerable.Range(0, 100)
            .Select(i => new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES })
            .ToList();

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns(manyItems);

        var vm = new RegionItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(100, vm.Items.Count);
    }

    #endregion
}
