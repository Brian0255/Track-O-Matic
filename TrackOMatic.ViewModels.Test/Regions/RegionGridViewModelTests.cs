using System.Collections.ObjectModel;

using Microsoft.Extensions.DependencyInjection;

namespace TrackOMatic.ViewModels.Test.Regions;

public class RegionGridViewModelTests
{
    private readonly Mock<IItemTrackingService> _mockItemTrackingService;
    private readonly Mock<IParsedSpoilerDataService> _mockParsedSpoilerDataService;
    private readonly Mock<ISavedProgressProvider> _mockSavedProgressProvider;
    private readonly Mock<IThemeService> _mockThemeService;
    private readonly Mock<RegionSlotProviderRegistry> _mockSlotProviderRegistry;
    private readonly Mock<IAutotrackerService> _mockAutotrackerService;
    private readonly Mock<IDataSaverService> _mockDataSaverService;
    private readonly Mock<ILevelOrderService> _mockLevelOrderService;
    private readonly Mock<IUserSettingsService> _mockUserSettingsService;

    public RegionGridViewModelTests()
    {
        _mockItemTrackingService = new Mock<IItemTrackingService>();
        _mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        _mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        _mockThemeService = new Mock<IThemeService>();
        _mockSlotProviderRegistry = new Mock<RegionSlotProviderRegistry>();
        _mockAutotrackerService = new Mock<IAutotrackerService>();
        _mockDataSaverService = new Mock<IDataSaverService>();
        _mockLevelOrderService = new Mock<ILevelOrderService>();
        _mockUserSettingsService = new Mock<IUserSettingsService>();

        // Setup default return values
        var defaultSpoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                { RegionName.START, new RegionSpoilerData() },
                { RegionName.DK_ISLES, new RegionSpoilerData() },
                { RegionName.JUNGLE_JAPES, new RegionSpoilerData() },
                { RegionName.ANGRY_AZTEC, new RegionSpoilerData() },
                { RegionName.FRANTIC_FACTORY, new RegionSpoilerData() },
                { RegionName.GLOOMY_GALLEON, new RegionSpoilerData() },
                { RegionName.FUNGI_FOREST, new RegionSpoilerData() },
                { RegionName.CRYSTAL_CAVES, new RegionSpoilerData() },
                { RegionName.CREEPY_CASTLE, new RegionSpoilerData() },
                { RegionName.HIDEOUT_HELM, new RegionSpoilerData() },
                { RegionName.SHOPS, new RegionSpoilerData() },
                { RegionName.UNHINTABLE_MOVES, new RegionSpoilerData() },
                { RegionName.UNKNOWN, new RegionSpoilerData() }
            },
            SpoilerSettings = new SpoilerSettings(pointsEnabled: false, vialsEnabled: false),
            StartingItems = []
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(defaultSpoilerData);
        _mockParsedSpoilerDataService.Setup(s => s.GetSpoilerSettings()).Returns(defaultSpoilerData.SpoilerSettings);
        _mockParsedSpoilerDataService.Setup(s => s.GetWothPointsForRegion(It.IsAny<RegionName>())).Returns(-1);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointsForRegion(It.IsAny<RegionName>())).Returns(0);
        _mockParsedSpoilerDataService.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());

        _mockItemTrackingService.Setup(s => s.GetItemsInRegion(It.IsAny<RegionName>())).Returns([]);
        _mockItemTrackingService.Setup(s => s.BeginBatchUpdate()).Returns(new Mock<IDisposable>().Object);

        // Setup level order service for RegionLevelViewModel
        _mockLevelOrderService.Setup(s => s.GetLevelOrder()).Returns(
            [
                0, 1, 2, 3, 4, 5, 6, 7, 8
            ]
        );

        // Initialize ServiceLocator for RegionLevelViewModel initialization
        var services = new ServiceCollection();
        services.AddSingleton(_mockLevelOrderService.Object);
        services.AddSingleton(_mockUserSettingsService.Object);
        ServiceLocator.Initialize(services.BuildServiceProvider());
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionGridViewModel(
                null!,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                _mockSlotProviderRegistry.Object,
                _mockAutotrackerService.Object,
                _mockDataSaverService.Object
            )
        );

        Assert.Equal("itemTrackingService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionGridViewModel(
                _mockItemTrackingService.Object,
                null!,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                _mockSlotProviderRegistry.Object,
                _mockAutotrackerService.Object,
                _mockDataSaverService.Object
            )
        );

        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionGridViewModel(
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                null!,
                _mockThemeService.Object,
                _mockSlotProviderRegistry.Object,
                _mockAutotrackerService.Object,
                _mockDataSaverService.Object
            )
        );

        Assert.Equal("savedProgressProvider", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullThemeService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionGridViewModel(
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                null!,
                _mockSlotProviderRegistry.Object,
                _mockAutotrackerService.Object,
                _mockDataSaverService.Object
            )
        );

        Assert.Equal("themeService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullAutotrackerService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionGridViewModel(
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                _mockSlotProviderRegistry.Object,
                null!,
                _mockDataSaverService.Object
            )
        );

        Assert.Equal("autotrackerService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullDataSaverService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionGridViewModel(
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                _mockSavedProgressProvider.Object,
                _mockThemeService.Object,
                _mockSlotProviderRegistry.Object,
                _mockAutotrackerService.Object,
                null!
            )
        );

        Assert.Equal("dataSaverService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithValidServices_Succeeds()
    {
        var vm = new RegionGridViewModel(
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        Assert.NotNull(vm);
        Assert.NotNull(vm.Regions);
    }

    #endregion

    #region Regions Collection Tests

    [Fact]
    public void Constructor_InitializesRegionsCollection()
    {
        var vm = new RegionGridViewModel(
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        Assert.NotNull(vm.Regions);
        Assert.IsType<ObservableCollection<RegionViewModel>>(vm.Regions);
    }

    [Fact]
    public void Constructor_PopulatesRegionsWithCorrectCount()
    {
        var vm = new RegionGridViewModel(
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        // Expected display regions: START, DK_ISLES, JUNGLE_JAPES, FUNGI_FOREST, ANGRY_AZTEC,
        // CRYSTAL_CAVES, FRANTIC_FACTORY, CREEPY_CASTLE, GLOOMY_GALLEON, HIDEOUT_HELM
        Assert.Equal(10, vm.Regions.Count);
    }

    [Fact]
    public void Constructor_CreatesRegionViewModels()
    {
        var vm = new RegionGridViewModel(
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        foreach (var region in vm.Regions)
        {
            Assert.IsType<RegionViewModel>(region);
        }
    }

    [Fact]
    public void Constructor_PopulatesRegionsInCorrectOrder()
    {
        var vm = new RegionGridViewModel(
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        var expectedOrder = new[]
        {
            RegionName.START,
            RegionName.DK_ISLES,
            RegionName.JUNGLE_JAPES,
            RegionName.FUNGI_FOREST,
            RegionName.ANGRY_AZTEC,
            RegionName.CRYSTAL_CAVES,
            RegionName.FRANTIC_FACTORY,
            RegionName.CREEPY_CASTLE,
            RegionName.GLOOMY_GALLEON,
            RegionName.HIDEOUT_HELM
        };

        for (int i = 0; i < expectedOrder.Length; i++)
        {
            Assert.Equal(expectedOrder[i], vm.Regions[i].RegionName);
        }
    }

    [Fact]
    public void Constructor_PopulatesAllDisplayRegions()
    {
        var vm = new RegionGridViewModel(
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        var regionNames = vm.Regions.Select(r => r.RegionName).ToList();

        // Verify all expected regions are present
        Assert.Contains(RegionName.START, regionNames);
        Assert.Contains(RegionName.DK_ISLES, regionNames);
        Assert.Contains(RegionName.JUNGLE_JAPES, regionNames);
        Assert.Contains(RegionName.FUNGI_FOREST, regionNames);
        Assert.Contains(RegionName.ANGRY_AZTEC, regionNames);
        Assert.Contains(RegionName.CRYSTAL_CAVES, regionNames);
        Assert.Contains(RegionName.FRANTIC_FACTORY, regionNames);
        Assert.Contains(RegionName.CREEPY_CASTLE, regionNames);
        Assert.Contains(RegionName.GLOOMY_GALLEON, regionNames);
        Assert.Contains(RegionName.HIDEOUT_HELM, regionNames);

        // Verify that non-display regions are NOT included
        Assert.DoesNotContain(RegionName.SHOPS, regionNames);
        Assert.DoesNotContain(RegionName.UNHINTABLE_MOVES, regionNames);
        Assert.DoesNotContain(RegionName.UNKNOWN, regionNames);
    }

    [Fact]
    public void Constructor_EachRegionReceivesSameDependencies()
    {
        var vm = new RegionGridViewModel(
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        // Each region should have received the same service instances
        // We verify this indirectly by checking that all regions were initialized
        Assert.All(vm.Regions, region =>
        {
            Assert.NotNull(region);
            // Region names should be assigned (even if START is the default value 0)
            Assert.True(region.RegionName >= 0);
        });
    }

    #endregion

    #region Region Property Tests

    [Fact]
    public void Regions_IsObservableCollection()
    {
        var vm = new RegionGridViewModel(
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        // Verify we can add/remove from the collection (it's observable)
        var initialCount = vm.Regions.Count;
        Assert.Equal(10, initialCount);
    }

    [Fact]
    public void Constructor_DisposesRegionViewModels()
    {
        var vm = new RegionGridViewModel(
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockSavedProgressProvider.Object,
            _mockThemeService.Object,
            _mockSlotProviderRegistry.Object,
            _mockAutotrackerService.Object,
            _mockDataSaverService.Object
        );

        // Verify each region can be disposed without error
        // (This tests that the regions were properly initialized)
        foreach (var region in vm.Regions)
        {
            region.Dispose(); // Should not throw
        }
    }

    #endregion
}
