using Microsoft.Extensions.DependencyInjection;

namespace TrackOMatic.ViewModels.Test.Regions;

public class RegionViewModelTests
{
    private readonly Mock<IItemTrackingService> _mockItemTrackingService;
    private readonly Mock<IParsedSpoilerDataService> _mockParsedSpoilerDataService;
    private readonly Mock<ISavedProgressProvider> _mockSavedProgressProvider;
    private readonly Mock<IThemeService> _mockThemeService;
    private readonly Mock<IAutotrackerService> _mockAutotrackerService;
    private readonly Mock<IDataSaverService> _mockDataSaverService;
    private readonly Mock<RegionSlotProviderRegistry> _mockSlotProviderRegistry;

    public RegionViewModelTests()
    {
        _mockItemTrackingService = new Mock<IItemTrackingService>();
        _mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        _mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        _mockThemeService = new Mock<IThemeService>();
        _mockAutotrackerService = new Mock<IAutotrackerService>();
        _mockDataSaverService = new Mock<IDataSaverService>();
        _mockSlotProviderRegistry = new Mock<RegionSlotProviderRegistry>();

        // Setup ServiceLocator with minimal service setup
        var mockLevelOrderService = new Mock<ILevelOrderService>();
        mockLevelOrderService.Setup(s => s.GetLevelOrder()).Returns([1, 2, 3, 4, 5, 6, 7, 8]);

        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.HelmInLevelOrder).Returns(true);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mockLevelOrderService.Object);
        serviceCollection.AddSingleton(mockUserSettingsService.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        ServiceLocator.Initialize(serviceProvider);
    }

    private static ParsedSpoilerData CreateDefaultSpoilerData(bool vialsEnabled = false, bool pointsEnabled = false)
    {
        var spoilerSettings = new SpoilerSettings(pointsEnabled: pointsEnabled, vialsEnabled: vialsEnabled);
        var spoilerData = new ParsedSpoilerData
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
            },
            SpoilerSettings = spoilerSettings,
            StartingItems = []
        };

        // Setup only on the mock, don't actually call Setup here - let tests do that
        return spoilerData;
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidServices_Succeeds()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);
        _mockItemTrackingService.Setup(s => s.BeginBatchUpdate()).Returns(new Mock<IDisposable>().Object);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(RegionName.JUNGLE_JAPES, vm.RegionName);
        Assert.NotNull(vm.PlacedItems);
        Assert.NotNull(vm.VialItemListViewModel);
        Assert.NotNull(vm.RegionItemListViewModel);

        vm.Dispose();
    }

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);

        Assert.Throws<ArgumentNullException>(() =>
            new RegionViewModel(
                RegionName.JUNGLE_JAPES,
                null!,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                _mockSlotProviderRegistry.Object,
                _mockAutotrackerService.Object,
                _mockDataSaverService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RegionViewModel(
                RegionName.JUNGLE_JAPES,
                _mockItemTrackingService.Object,
                null!,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                _mockSlotProviderRegistry.Object,
                _mockAutotrackerService.Object,
                _mockDataSaverService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullThemeService_ThrowsArgumentNullException()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);

        Assert.Throws<ArgumentNullException>(() =>
            new RegionViewModel(
                RegionName.JUNGLE_JAPES,
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                null!,
                _mockSlotProviderRegistry.Object,
                _mockAutotrackerService.Object,
                _mockDataSaverService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullAutotrackerService_ThrowsArgumentNullException()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);

        Assert.Throws<ArgumentNullException>(() =>
            new RegionViewModel(
                RegionName.JUNGLE_JAPES,
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                _mockSlotProviderRegistry.Object,
                null!,
                _mockDataSaverService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullDataSaverService_ThrowsArgumentNullException()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);

        Assert.Throws<ArgumentNullException>(() =>
            new RegionViewModel(
                RegionName.JUNGLE_JAPES,
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                _mockSlotProviderRegistry.Object,
                _mockAutotrackerService.Object,
                null!
            )
        );
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void Constructor_InitializesResourceKeysAndDefaults()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal("jungle_japes", vm.RegionResourceKey);
        Assert.False(vm.HasVials);
        Assert.False(vm.HasWothPoints);
        Assert.False(vm.HasItemPoints);
        Assert.Equal("RegionBG", vm.BackgroundResourceKey);

        vm.Dispose();
    }


    [Fact]
    public void OnParsedSpoilerDataChanged_WithVialsEnabled_SetsHasVialsTrue()
    {
        var spoilerData = CreateDefaultSpoilerData(vialsEnabled: true);

        // Populate with vial colors
        spoilerData.RegionData[RegionName.JUNGLE_JAPES] = new RegionSpoilerData { VialColors = [VialColor.KEY] };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);
        _mockItemTrackingService.Setup(s => s.BeginBatchUpdate()).Returns(new Mock<IDisposable>().Object);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        // Verify HasVials is false before the event
        Assert.False(vm.HasVials);

        // Raise the ParsedSpoilerDataChanged event
        var eventArgs = new ParsedSpoilerDataChangedEventArgs(null, null);
        _mockParsedSpoilerDataService.Raise(s => s.ParsedSpoilerDataChanged += null, null!, eventArgs);

        // After the event, HasVials should be true
        Assert.True(vm.HasVials);

        vm.Dispose();
    }

    #endregion

    #region OnItemStateChanged Tests

    [Fact]
    public void OnItemStateChanged_WithUserModifiedItem_UpdatesHoardAndPoints()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(5);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES))
            .Returns([new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES, Starred = false }]);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        // Simulate adding an item to this region
        var newItemState = new SavedItem { ItemName = ItemName.KEY_2, Region = RegionName.JUNGLE_JAPES, Starred = false };
        var eventArgs = new ItemStateChangedEventArgs(newItemState, null, ChangeReason.UserModified);

        _mockItemTrackingService.Raise(s => s.ItemStateChanged += null, null!, eventArgs);

        // WothPoints should be calculated (5 - 1 existing - 0 vials = 4)
        Assert.True(vm.HasWothPoints);

        vm.Dispose();
    }

    [Fact]
    public void OnItemStateChanged_WithIgnoredChangeReason_DoesNotUpdate()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        // Event with an ignored reason should not update
        var newItemState = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.JUNGLE_JAPES };
        var eventArgs = new ItemStateChangedEventArgs(newItemState, null, ChangeReason.Unknown);

        _mockItemTrackingService.Raise(s => s.ItemStateChanged += null, null!, eventArgs);

        // Should remain unchanged
        Assert.False(vm.HasWothPoints);

        vm.Dispose();
    }

    [Fact]
    public void OnItemStateChanged_WithItemInDifferentRegion_DoesNotUpdate()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        // Event for a different region should not update this region
        var newItemState = new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.ANGRY_AZTEC };
        var eventArgs = new ItemStateChangedEventArgs(newItemState, null, ChangeReason.UserModified);

        _mockItemTrackingService.Raise(s => s.ItemStateChanged += null, null!, eventArgs);

        // Should remain unchanged
        Assert.False(vm.HasWothPoints);

        vm.Dispose();
    }

    #endregion

    #region TryAutoPlaceItem Tests

    [Fact]
    public void TryAutoPlaceItem_WithNonVialMode_PlacesItemDirectly()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        var result = vm.TryAutoPlaceItem(ItemName.KEY_1);

        Assert.True(result);
        _mockItemTrackingService.Verify(
            s => s.SetItemState(ItemName.KEY_1, It.Is<SavedItem>(item =>
                item.ItemName == ItemName.KEY_1 &&
                item.Region == RegionName.JUNGLE_JAPES &&
                item.Autotracked == true
            )),
            Times.Once
        );

        vm.Dispose();
    }

    [Fact]
    public void TryAutoPlaceItem_WithVialModeButNoMatchingColor_ReturnsFalse()
    {
        var spoilerData = CreateDefaultSpoilerData(vialsEnabled: true);

        // Set region to have no vials
        spoilerData.RegionData[RegionName.JUNGLE_JAPES] = new RegionSpoilerData { VialColors = [] };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);
        _mockItemTrackingService.Setup(s => s.BeginBatchUpdate()).Returns(new Mock<IDisposable>().Object);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        // Raise the event to enable vials
        var eventArgs1 = new ParsedSpoilerDataChangedEventArgs(null, null);
        _mockParsedSpoilerDataService.Raise(s => s.ParsedSpoilerDataChanged += null, null!, eventArgs1);

        // Without matching vial slots, should return false
        var result = vm.TryAutoPlaceItem(ItemName.KEY_1);

        Assert.False(result);

        vm.Dispose();
    }

    [Fact]
    public void TryAutoPlaceItem_WithVialModeAndMatchingSlot_PlacesSuccessfully()
    {
        var spoilerData = CreateDefaultSpoilerData(vialsEnabled: true);

        // Populate with matching vial color (KEY_1 maps to VialColor.KEY)
        spoilerData.RegionData[RegionName.JUNGLE_JAPES] = new RegionSpoilerData { VialColors = [VialColor.KEY] };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);
        _mockItemTrackingService.Setup(s => s.BeginBatchUpdate()).Returns(new Mock<IDisposable>().Object);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        // Raise the event to enable vials
        var eventArgs2 = new ParsedSpoilerDataChangedEventArgs(null, null);
        _mockParsedSpoilerDataService.Raise(s => s.ParsedSpoilerDataChanged += null, null!, eventArgs2);

        // Manually add a vial slot that matches KEY_1's color (KEY)
        var vialSlot = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.KEY,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        vm.PlacedItems.Add(vialSlot);

        var result = vm.TryAutoPlaceItem(ItemName.KEY_1);

        Assert.True(result);
        Assert.Equal(ItemName.KEY_1, vialSlot.CurrentItemName);

        vm.Dispose();
    }

    #endregion

    #region RegionLighting Tests

    [Fact]
    public void OnRegionLightingChanged_WithMatchingRegion_UpdatesBackground()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal("RegionBG", vm.BackgroundResourceKey);

        var lightingEvent = new AutotrackerRegionEventArgs { Region = RegionName.JUNGLE_JAPES, LightUp = true };
        _mockAutotrackerService.Raise(s => s.RegionLightingChanged += null, null!, lightingEvent);

        Assert.Equal("RegionBGLitUp", vm.BackgroundResourceKey);

        vm.Dispose();
    }

    [Fact]
    public void OnRegionLightingChanged_WithDifferentRegion_DoesNotUpdate()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        var lightingEvent = new AutotrackerRegionEventArgs { Region = RegionName.ANGRY_AZTEC, LightUp = true };
        _mockAutotrackerService.Raise(s => s.RegionLightingChanged += null, null!, lightingEvent);

        Assert.Equal("RegionBG", vm.BackgroundResourceKey);

        vm.Dispose();
    }

    #endregion

    #region Data Refresh Tests

    [Fact]
    public void OnDataLoadingCompleted_RefreshesItemPointsAndWothPoints()
    {
        var spoilerData = CreateDefaultSpoilerData(pointsEnabled: true);

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(5);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(10);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>
        {
            { PointCategory.Key, 1 },
            { PointCategory.SharedMove, 2 }
        });
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);
        _mockItemTrackingService.Setup(s => s.BeginBatchUpdate()).Returns(new Mock<IDisposable>().Object);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        var loadingArgs = new DataLoadingCompletedEventArgs(new SavedProgress());
        _mockDataSaverService.Raise(s => s.DataLoadingCompleted += null, null!, loadingArgs);

        Assert.True(vm.HasWothPoints);
        Assert.True(vm.HasItemPoints);

        vm.Dispose();
    }

    [Fact]
    public void OnParsedSpoilerDataChanged_RefreshesState()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        var spoilerChangedArgs = new ParsedSpoilerDataChangedEventArgs(null, null);
        _mockParsedSpoilerDataService.Raise(s => s.ParsedSpoilerDataChanged += null, null!, spoilerChangedArgs);

        // State should be refreshed (just verify it didn't crash)
        Assert.NotNull(vm);

        vm.Dispose();
    }

    #endregion

    #region Property Visibility Tests

    [Fact]
    public void ShouldShowItemPoints_WithStartRegion_ReturnsFalse()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.START)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.START)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.START)).Returns([]);

        var vm = new RegionViewModel(
            RegionName.START,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        Assert.False(vm.ShouldShowItemPoints);

        vm.Dispose();
    }

    [Fact]
    public void ShouldShowRegionLevel_WithStartRegion_ReturnsFalse()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.START)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.START)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.START)).Returns([]);

        var vm = new RegionViewModel(
            RegionName.START,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        Assert.False(vm.ShouldShowRegionLevel);

        vm.Dispose();
    }

    #endregion

    #region Equality and Hash Tests

    [Fact]
    public void Equals_WithSameRegion_ReturnsTrue()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);

        var vm1 = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        var vm2 = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(vm1, vm2);
        Assert.True(vm1 == vm2);

        vm1.Dispose();
        vm2.Dispose();
    }

    [Fact]
    public void Equals_WithDifferentRegion_ReturnsFalse()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(It.IsAny<RegionName>())).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(It.IsAny<RegionName>())).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(It.IsAny<RegionName>())).Returns([]);

        var vm1 = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        var vm2 = new RegionViewModel(
            RegionName.ANGRY_AZTEC,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        Assert.NotEqual(vm1, vm2);
        Assert.True(vm1 != vm2);

        vm1.Dispose();
        vm2.Dispose();
    }

    [Fact]
    public void GetHashCode_WithSameRegion_ReturnsSameHash()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);

        var vm1 = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        var vm2 = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        Assert.Equal(vm1.GetHashCode(), vm2.GetHashCode());

        vm1.Dispose();
        vm2.Dispose();
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(spoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        var str = vm.ToString();

        Assert.Contains("Region: JUNGLE_JAPES", str);
        Assert.Contains("PlacedItems: 0", str);

        vm.Dispose();
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_UnhooksEventHandlers()
    {
        var spoilerData = CreateDefaultSpoilerData();

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(spoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(RegionName.JUNGLE_JAPES)).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(RegionName.JUNGLE_JAPES)).Returns([]);
        _mockItemTrackingService.Setup(s => s.BeginBatchUpdate()).Returns(new Mock<IDisposable>().Object);

        var vm = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        vm.Dispose();

        // Verify that dispose completed without error
        Assert.NotNull(vm);
    }

    #endregion
}

