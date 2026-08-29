using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.ViewModels.Test;

public class RegionViewModelTests
{
    private static Mock<IItemTrackingService> CreateMockItemTrackingService(Dictionary<ItemName, SavedItem>? items = null)
    {
        var mock = new Mock<IItemTrackingService>();
        items ??= [];

        mock.Setup(s => s.GetItemState(It.IsAny<ItemName>()))
            .Returns<ItemName>(itemName => items.TryGetValue(itemName, out var item) ? item : null);

        mock.Setup(s => s.GetItemsInRegion(It.IsAny<RegionName>()))
            .Returns<RegionName>(region => items.Values.Where(i => i.Region == region).ToList());

        mock.Setup(s => s.SetItemState(It.IsAny<ItemName>(), It.IsAny<SavedItem>()))
            .Callback<ItemName, SavedItem>((itemName, item) => items[itemName] = item);

        mock.Setup(s => s.ToggleStar(It.IsAny<ItemName>())).Callback<ItemName>(itemName =>
        {
            if (items.TryGetValue(itemName, out var item))
            {
                var newStarred = item.Starred switch
                {
                    ItemVisibilityState.Visible => ItemVisibilityState.Hidden,
                    _ => ItemVisibilityState.Visible
                };
                items[itemName] = item with { Starred = newStarred };
            }
            else
            {
                items[itemName] = SavedItem.CreateEmpty(itemName) with { Starred = ItemVisibilityState.Visible };
            }
        });

        return mock;
    }

    private static Mock<IParsedSpoilerDataService> CreateMockSpoilerDataService()
    {
        var mock = new Mock<IParsedSpoilerDataService>();
        mock.Setup(s => s.CurrentData).Returns((ParsedSpoilerData?)null);
        mock.Setup(s => s.GetPointsForRegion(It.IsAny<RegionName>())).Returns(0);
        mock.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        mock.Setup(s => s.GetWothPointsForRegion(It.IsAny<RegionName>())).Returns(-1);
        return mock;
    }

    private static Mock<ISavedProgressProvider> CreateMockSavedProgressProvider()
    {
        var mock = new Mock<ISavedProgressProvider>();
        var progress = new SavedProgress();
        mock.Setup(s => s.CurrentProgress).Returns(progress);
        return mock;
    }

    private static Mock<IThemeService> CreateMockThemeService() => new();

    private static Mock<IRegionPlacementOrchestrator> CreateMockRegionPlacementOrchestrator() => new();

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidDependencies_Succeeds()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var orchestrator = CreateMockRegionPlacementOrchestrator().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider,
            orchestrator,
            themeService);

        Assert.NotNull(viewModel);
    }

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var orchestrator = CreateMockRegionPlacementOrchestrator().Object;
        var themeService = CreateMockThemeService().Object;

        Assert.Throws<ArgumentNullException>(() =>
            new RegionViewModel(
                RegionName.JUNGLE_JAPES,
                null!,
                spoilerDataService,
                savedProgressProvider,
                orchestrator,
                themeService));
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var orchestrator = CreateMockRegionPlacementOrchestrator().Object;
        var themeService = CreateMockThemeService().Object;

        Assert.Throws<ArgumentNullException>(() =>
            new RegionViewModel(
                RegionName.JUNGLE_JAPES,
                itemTrackingService,
                null!,
                savedProgressProvider,
                orchestrator,
                themeService));
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var orchestrator = CreateMockRegionPlacementOrchestrator().Object;
        var themeService = CreateMockThemeService().Object;

        Assert.Throws<ArgumentNullException>(() =>
            new RegionViewModel(
                RegionName.JUNGLE_JAPES,
                itemTrackingService,
                spoilerDataService,
                null!,
                orchestrator,
                themeService));
    }

    [Fact]
    public void Constructor_InitializesPlacedItemsCollection()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var orchestrator = CreateMockRegionPlacementOrchestrator().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider,
            orchestrator,
            themeService);

        Assert.Multiple(() =>
        {
            Assert.NotNull(viewModel.PlacedItems);
            Assert.IsType<System.Collections.ObjectModel.ObservableCollection<IRegionItemViewModel>>(viewModel.PlacedItems);
        });
    }

    #endregion

    #region TryAcceptDrop Tests - Non-Spoiler Mode

    [Fact]
    public void TryAcceptDrop_NonSpoilerMode_EmptyRegion_ReturnsTrue()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        var result = viewModel.TryAcceptDrop(ItemName.STRONG_KONG, MouseDragType.Left);

        Assert.True(result);
    }

    [Fact]
    public void TryAcceptDrop_NonSpoilerMode_WithExistingItem_ReturnsTrue()
    {
        var items = new Dictionary<ItemName, SavedItem>
        {
            { ItemName.STRONG_KONG, new SavedItem(ItemName.STRONG_KONG, RegionName.JUNGLE_JAPES) }
        };
        var itemTrackingService = CreateMockItemTrackingService(items).Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        // Non-spoiler mode should always accept drops, even with existing items
        var result = viewModel.TryAcceptDrop(ItemName.DIDDY, MouseDragType.Left);

        Assert.True(result);
    }

    #endregion

    #region TryAutoPlaceItem Tests - Non-Spoiler Mode

    [Fact]
    public void TryAutoPlaceItem_NonSpoilerMode_PlacesItemDirectly()
    {
        var items = new Dictionary<ItemName, SavedItem>();
        var itemTrackingService = CreateMockItemTrackingService(items).Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        var result = viewModel.TryAutoPlaceItem(ItemName.STRONG_KONG);

        Assert.Multiple(() =>
        {
            Assert.True(result);
            Assert.True(items.ContainsKey(ItemName.STRONG_KONG));
            Assert.Equal(RegionName.JUNGLE_JAPES, items[ItemName.STRONG_KONG].Region);
            Assert.True(items[ItemName.STRONG_KONG].Autotracked);
        });
    }

    #endregion

    #region TryAutoPlaceItem Tests - Spoiler Mode

    [Fact]
    public void TryAutoPlaceItem_SpoilerMode_NoMatchingColor_ReturnsFalse()
    {
        var items = new Dictionary<ItemName, SavedItem>();
        var itemTrackingService = CreateMockItemTrackingService(items).Object;

        var spoilerDataMock = new Mock<IParsedSpoilerDataService>();
        var regionSpoilerData = new RegionSpoilerData
        {
            VialColors = [VialColor.YELLOW]
        };
        var parsedData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                { RegionName.JUNGLE_JAPES, regionSpoilerData }
            }
        };
        spoilerDataMock.Setup(s => s.CurrentData).Returns(parsedData);
        spoilerDataMock.Setup(s => s.GetPointsForRegion(It.IsAny<RegionName>())).Returns(0);
        spoilerDataMock.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        spoilerDataMock.Setup(s => s.GetWothPointsForRegion(It.IsAny<RegionName>())).Returns(-1);
        spoilerDataMock.Setup(s => s.GetSpoilerSettings()).Returns(new SpoilerSettings(vialsEnabled: true));

        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var orchestrator = CreateMockRegionPlacementOrchestrator().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataMock.Object,
            savedProgressProvider,
            orchestrator,
            themeService);

        // CRANKY maps to VialColor.ORANGE, but we only have YELLOW vials
        var result = viewModel.TryAutoPlaceItem(ItemName.CRANKY);

        Assert.False(result);
    }

    [Fact]
    public void TryAutoPlaceItem_SpoilerMode_EmptySlotAvailable_PlacesInEmptySlot()
    {
        var items = new Dictionary<ItemName, SavedItem>();
        var itemTrackingService = CreateMockItemTrackingService(items).Object;

        var spoilerDataMock = new Mock<IParsedSpoilerDataService>();
        var regionSpoilerData = new RegionSpoilerData
        {
            VialColors = [VialColor.YELLOW, VialColor.YELLOW]
        };
        var parsedData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                { RegionName.JUNGLE_JAPES, regionSpoilerData }
            }
        };
        spoilerDataMock.Setup(s => s.CurrentData).Returns(parsedData);
        spoilerDataMock.Setup(s => s.GetPointsForRegion(It.IsAny<RegionName>())).Returns(0);
        spoilerDataMock.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        spoilerDataMock.Setup(s => s.GetWothPointsForRegion(It.IsAny<RegionName>())).Returns(-1);

        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var orchestrator = CreateMockRegionPlacementOrchestrator().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataMock.Object,
            savedProgressProvider,
            orchestrator,
            themeService);

        // First slot should be empty, autoplace STRONG_KONG (which is YELLOW)
        var result = viewModel.TryAutoPlaceItem(ItemName.STRONG_KONG);

        Assert.Multiple(() =>
        {
            Assert.True(result);
            Assert.True(items.ContainsKey(ItemName.STRONG_KONG));
        });
    }

    [Fact]
    public void TryAutoPlaceItem_SpoilerMode_SkipsAutotrackingItems()
    {
        // First item is autotracked, second item is not—should place in second slot
        var items = new Dictionary<ItemName, SavedItem>
        {
            { ItemName.STRONG_KONG, new SavedItem(ItemName.STRONG_KONG, RegionName.JUNGLE_JAPES, Autotracked: true) },
            { ItemName.CANDY, new SavedItem(ItemName.CANDY, RegionName.JUNGLE_JAPES, Autotracked: false) }
        };
        var itemTrackingService = CreateMockItemTrackingService(items).Object;

        var spoilerDataMock = new Mock<IParsedSpoilerDataService>();
        var regionSpoilerData = new RegionSpoilerData
        {
            VialColors = [VialColor.YELLOW, VialColor.YELLOW]
        };
        var parsedData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                { RegionName.JUNGLE_JAPES, regionSpoilerData }
            }
        };
        spoilerDataMock.Setup(s => s.CurrentData).Returns(parsedData);
        spoilerDataMock.Setup(s => s.GetPointsForRegion(It.IsAny<RegionName>())).Returns(0);
        spoilerDataMock.Setup(s => s.GetPointSpread()).Returns(new Dictionary<PointCategory, int>());
        spoilerDataMock.Setup(s => s.GetWothPointsForRegion(It.IsAny<RegionName>())).Returns(-1);

        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var orchestrator = CreateMockRegionPlacementOrchestrator().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataMock.Object,
            savedProgressProvider,
            orchestrator,
            themeService);

        // Try to place STRONG_KONG (YELLOW), should skip the first autotracked STRONG_KONG
        // and place in slot with non-autotracked CANDY
        var result = viewModel.TryAutoPlaceItem(ItemName.STRONG_KONG);

        Assert.True(result);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void RegionResourceKey_InitializesFromRegionName()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        Assert.Equal("jungle_japes", viewModel.RegionResourceKey);
    }

    [Fact]
    public void HasWothPoints_InitializesToFalse()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        Assert.False(viewModel.HasWothPoints);
    }

    [Fact]
    public void HasItemPoints_InitializesToFalse()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        Assert.False(viewModel.HasItemPoints);
    }

    #endregion

    #region Visibility Property Tests

    [Fact]
    public void ShouldShowItemPoints_StartRegion_ReturnsFalse()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.START,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        Assert.False(viewModel.ShouldShowItemPoints);
    }

    [Fact]
    public void ShouldShowItemPoints_NoItemPoints_ReturnsFalse()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        Assert.Multiple(() =>
        {
            Assert.False(viewModel.HasItemPoints);
            Assert.False(viewModel.ShouldShowItemPoints);
        });
    }

    [Fact]
    public void ShouldShowItemPoints_WithItemPoints_ReturnsTrue()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        Assert.Multiple(() =>
        {
            Assert.False(viewModel.HasItemPoints);
            Assert.False(viewModel.ShouldShowItemPoints);
        });
    }

    [Fact]
    public void ShouldShowItemPoints_WithZeroPoints_ReturnsTrue()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        Assert.Multiple(() =>
        {
            Assert.False(viewModel.HasItemPoints);
            Assert.False(viewModel.ShouldShowItemPoints);
        });
    }

    [Theory]
    [InlineData(RegionName.START, false)]
    [InlineData(RegionName.DK_ISLES, false)]
    [InlineData(RegionName.JUNGLE_JAPES, true)]
    [InlineData(RegionName.ANGRY_AZTEC, true)]
    [InlineData(RegionName.FRANTIC_FACTORY, true)]
    [InlineData(RegionName.GLOOMY_GALLEON, true)]
    [InlineData(RegionName.FUNGI_FOREST, true)]
    [InlineData(RegionName.CRYSTAL_CAVES, true)]
    [InlineData(RegionName.CREEPY_CASTLE, true)]
    [InlineData(RegionName.HIDEOUT_HELM, true)]
    public void ShouldShowRegionLevel_ByRegionType_ReturnsCorrectValue(RegionName regionName, bool expectedResult)
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var orchestrator = CreateMockRegionPlacementOrchestrator().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            regionName,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider,
            orchestrator,
            themeService);

        Assert.Equal(expectedResult, viewModel.ShouldShowRegionLevel);
    }

    #endregion

    #region PropertyChanged Tests

    [Fact]
    public void RegionResourceKey_RaisesPropertyChanged()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        var propertyChanged = false;
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(RegionViewModel.RegionResourceKey))
            {
                propertyChanged = true;
            }
        };

        viewModel.RegionResourceKey = "new_value";

        Assert.True(propertyChanged);
    }

    [Fact]
    public void WothPoints_RaisesPropertyChanged()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        var propertyChanged = false;
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(RegionViewModel.WothPoints))
            {
                propertyChanged = true;
            }
        };

        viewModel.WothPoints = 10;

        Assert.True(propertyChanged);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameRegionName_ReturnsTrue()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel1 = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        var viewModel2 = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        Assert.Equal(viewModel1, viewModel2);
    }

    [Fact]
    public void Equals_WithDifferentRegionName_ReturnsFalse()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel1 = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        var viewModel2 = new RegionViewModel(
            RegionName.ANGRY_AZTEC,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        Assert.NotEqual(viewModel1, viewModel2);
    }

    [Fact]
    public void GetHashCode_WithSameRegionName_ReturnsSameHashCode()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel1 = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        var viewModel2 = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        Assert.Equal(viewModel1.GetHashCode(), viewModel2.GetHashCode());
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_CanBeCalled()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        viewModel.Dispose(); // Should not throw
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerDataService = CreateMockSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;
        var themeService = CreateMockThemeService().Object;

        var viewModel = new RegionViewModel(
            RegionName.JUNGLE_JAPES,
            itemTrackingService,
            spoilerDataService,
            savedProgressProvider, CreateMockRegionPlacementOrchestrator().Object,
            themeService);

        viewModel.Dispose();
        viewModel.Dispose(); // Should not throw
    }

    #endregion
}
