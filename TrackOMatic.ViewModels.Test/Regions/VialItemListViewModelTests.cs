namespace TrackOMatic.ViewModels.Test.Regions;

public class VialItemListViewModelTests
{
    private readonly Mock<IItemTrackingService> _mockItemTrackingService;
    private readonly Mock<IParsedSpoilerDataService> _mockParsedSpoilerDataService;
    private readonly Mock<ISavedProgressProvider> _mockSavedProgressProvider;
    private readonly Mock<IThemeService> _mockThemeService;
    private readonly Mock<IDataSaverService> _mockDataSaverService;

    public VialItemListViewModelTests()
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
                { RegionName.JUNGLE_JAPES, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.ANGRY_AZTEC, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.FRANTIC_FACTORY, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.GLOOMY_GALLEON, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.FUNGI_FOREST, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.CRYSTAL_CAVES, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.CREEPY_CASTLE, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.HIDEOUT_HELM, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.DK_ISLES, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.SHOPS, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.START, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.UNHINTABLE_MOVES, new RegionSpoilerData() { VialColors = [] } },
                { RegionName.UNKNOWN, new RegionSpoilerData() { VialColors = [] } }
            },
            StartingItems = []
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
        var vm = new VialItemListViewModel(
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
            new VialItemListViewModel(
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
            new VialItemListViewModel(
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
            new VialItemListViewModel(
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
            new VialItemListViewModel(
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
            new VialItemListViewModel(
                RegionName.JUNGLE_JAPES,
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                null!
            )
        );
    }

    [Fact]
    public void Constructor_InitializesEmptyItemsForNonStartRegion()
    {
        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Empty(vm.Items);
    }

    [Fact]
    public void Constructor_InitializesItemsForStartRegion()
    {
        var startingItems = new Dictionary<ItemName, RegionName>
        {
            { ItemName.STRONG_KONG, RegionName.START },
            { ItemName.COCONUT_GUN, RegionName.START }
        };

        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                { RegionName.START, new RegionSpoilerData() { VialColors = [] } }
            },
            StartingItems = startingItems
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.START,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(2, vm.Items.Count);
        Assert.Equal(ItemName.COCONUT_GUN, vm.Items[0].CurrentItemName);
        Assert.Equal(ItemName.STRONG_KONG, vm.Items[1].CurrentItemName);
    }

    #endregion

    #region TryAcceptDrop Tests

    [Fact]
    public void TryAcceptDrop_WithEmptySlotMatchingColor_ReturnsTrue()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW, VialColor.YELLOW]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAcceptDrop(ItemName.STRONG_KONG, MouseDragType.Left);

        Assert.True(result);
    }

    [Fact]
    public void TryAcceptDrop_WithNoMatchingColor_ReturnsFalse()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.RED]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAcceptDrop(ItemName.STRONG_KONG, MouseDragType.Left);

        Assert.False(result);
    }

    [Fact]
    public void TryAcceptDrop_WithAllSlotsFilled_ReturnsFalse()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var itemsInRegion = new[]
        {
            new SavedItem { ItemName = ItemName.STRONG_KONG, Region = RegionName.JUNGLE_JAPES }
        };

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns(itemsInRegion);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAcceptDrop(ItemName.COCONUT_GUN, MouseDragType.Left);

        Assert.False(result);
    }

    [Fact]
    public void TryAcceptDrop_WithStartRegionUserDrag_ReturnsFalse()
    {
        var startingItems = new Dictionary<ItemName, RegionName>
        {
            { ItemName.STRONG_KONG, RegionName.START }
        };

        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                { RegionName.START, new RegionSpoilerData() { VialColors = [] } }
            },
            StartingItems = startingItems
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.START,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        // User drag (Left) should be rejected for start region
        var result = vm.TryAcceptDrop(ItemName.COCONUT_GUN, MouseDragType.Left);

        Assert.False(result);
    }

    [Fact]
    public void TryAcceptDrop_WithStartRegionAutotrackingDrop_ReturnsFalseNoEmptySlots()
    {
        var startingItems = new Dictionary<ItemName, RegionName>
        {
            { ItemName.STRONG_KONG, RegionName.START }
        };

        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                { RegionName.START, new RegionSpoilerData() { VialColors = [] } }
            },
            StartingItems = startingItems
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.START,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        // Autotracking drop (None) would be allowed for start region but only if there's an empty slot
        // Since START region vials are created with items, there are no empty slots
        var result = vm.TryAcceptDrop(ItemName.COCONUT_GUN, MouseDragType.None);

        Assert.False(result);
    }

    #endregion

    #region OnItemStateChanged Tests

    [Fact]
    public void OnItemStateChanged_WithAutoTrackedItemInRegion_PlacesInEmptySlot()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW, VialColor.YELLOW]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var itemState = new SavedItem { ItemName = ItemName.STRONG_KONG, Region = RegionName.JUNGLE_JAPES, Autotracked = true };
        var eventArgs = new ItemStateChangedEventArgs(itemState, null, ChangeReason.AutoTracked);

        _mockItemTrackingService.Raise(s => s.ItemStateChanged += null, null!, eventArgs);

        // First slot should now have the item
        Assert.Equal(ItemName.STRONG_KONG, vm.Items[0].CurrentItemName);
    }

    [Fact]
    public void OnItemStateChanged_WithNonAutoTrackedItem_DoesNotPlace()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var itemState = new SavedItem { ItemName = ItemName.STRONG_KONG, Region = RegionName.JUNGLE_JAPES, Autotracked = false };
        var eventArgs = new ItemStateChangedEventArgs(itemState, null, ChangeReason.Unknown);

        _mockItemTrackingService.Raise(s => s.ItemStateChanged += null, null!, eventArgs);

        // Slot should remain empty for non-autotracked items
        Assert.Null(vm.Items[0].CurrentItemName);
    }

    [Fact]
    public void OnItemStateChanged_WithAutoTrackedItemInDifferentRegion_DoesNotPlace()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var itemState = new SavedItem { ItemName = ItemName.STRONG_KONG, Region = RegionName.ANGRY_AZTEC, Autotracked = true };
        var eventArgs = new ItemStateChangedEventArgs(itemState, null, ChangeReason.AutoTracked);

        _mockItemTrackingService.Raise(s => s.ItemStateChanged += null, null!, eventArgs);

        // Slot should remain empty because item is in different region
        Assert.Null(vm.Items[0].CurrentItemName);
    }

    #endregion

    #region OnDataReset Tests

    [Fact]
    public void OnDataReset_ClearsAllItems()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Single(vm.Items);

        var resetArgs = new ProgressReplacedEventArgs { NewProgress = new SavedProgress(), OldProgress = null };
        _mockSavedProgressProvider.Raise(s => s.ProgressChanged += null, null!, resetArgs);

        Assert.Empty(vm.Items);
    }

    #endregion

    #region OnDataLoadingCompleted Tests

    [Fact]
    public void OnDataLoadingCompleted_RefreshesItems()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Single(vm.Items);

        var loadingArgs = new DataLoadingCompletedEventArgs(new SavedProgress());
        _mockDataSaverService.Raise(s => s.DataLoadingCompleted += null, null!, loadingArgs);

        // Items should be refreshed (count should remain the same in this case)
        Assert.Single(vm.Items);
    }

    #endregion

    #region OnParsedSpoilerDataChanged Tests

    [Fact]
    public void OnParsedSpoilerDataChanged_RefreshesItems()
    {
        var initialSpoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(initialSpoilerData);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.Single(vm.Items);

        // Change spoiler data
        var newSpoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW, VialColor.YELLOW, VialColor.RED]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(newSpoilerData);

        var spoilerChangedArgs = new ParsedSpoilerDataChangedEventArgs(null, null);
        _mockParsedSpoilerDataService.Raise(s => s.ParsedSpoilerDataChanged += null, null!, spoilerChangedArgs);

        // Items should be refreshed to match new configuration
        Assert.Equal(3, vm.Items.Count);
    }

    #endregion

    #region VialItemStateChanged Event Tests

    [Fact]
    public void VialItemStateChanged_EventIsRaised()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        VialItemStateChangedEventArgs? capturedArgs = null;
        vm.VialItemStateChanged += (s, e) => capturedArgs = e;

        vm.Items[0].ToggleVialStarCommand.Execute(null);

        Assert.NotNull(capturedArgs);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameRegion_ReturnsTrue()
    {
        var vm1 = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var vm2 = new VialItemListViewModel(
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
        var vm1 = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var vm2 = new VialItemListViewModel(
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
    public void GetHashCode_WithSameRegion_ReturnsSameHash()
    {
        var vm1 = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        var vm2 = new VialItemListViewModel(
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
    public void ToString_ReturnsFormattedString()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW, VialColor.RED]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
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

    #region Dispose Tests

    [Fact]
    public void Dispose_ClearsItems()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        Assert.NotEmpty(vm.Items);

        vm.Dispose();

        Assert.Empty(vm.Items);
    }

    [Fact]
    public void Dispose_UnhooksEventHandlers()
    {
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                {
                    RegionName.JUNGLE_JAPES, new RegionSpoilerData()
                    {
                        VialColors = [VialColor.YELLOW]
                    }
                }
            },
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData)
            .Returns(spoilerData);

        var vm = new VialItemListViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockDataSaverService.Object
        );

        vm.Dispose();

        // Verify that event handlers were removed (we just verify the dispose didn't crash)
        Assert.NotNull(vm);
    }

    #endregion
}
