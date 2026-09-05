using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels.Test.Broadcast;

public class HomingScopeViewModelTests
{
    private static Mock<IItemTrackingService> CreateMockItemTrackingService(
        SavedItem? homingState = null,
        SavedItem? scopeState = null)
    {
        var mock = new Mock<IItemTrackingService>();

        mock.Setup(s => s.GetItemState(ItemName.HOMING_AMMO))
            .Returns(homingState);
        mock.Setup(s => s.GetItemState(ItemName.SNIPER_SCOPE))
            .Returns(scopeState);

        return mock;
    }

    private static Mock<IParsedSpoilerDataService> CreateMockParsedSpoilerDataService(
        ParsedSpoilerData? spoilerData = null)
    {
        var mock = new Mock<IParsedSpoilerDataService>();
        mock.Setup(s => s.CurrentData).Returns(spoilerData);
        return mock;
    }

    private static Mock<ISavedProgressProvider> CreateMockSavedProgressProvider()
    {
        var mock = new Mock<ISavedProgressProvider>();
        return mock;
    }

    private static SavedItem CreateItemState(ItemName itemName, RegionName region = RegionName.JUNGLE_JAPES, bool starred = false)
    {
        return new SavedItem(itemName) with
        {
            Region = region,
            Starred = starred
        };
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidDependencies_Succeeds()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.NotNull(viewModel);
    }

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        Assert.Throws<ArgumentNullException>(() =>
            new HomingScopeViewModel(
                null!,
                parsedSpoilerDataService,
                savedProgressProvider));
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        Assert.Throws<ArgumentNullException>(() =>
            new HomingScopeViewModel(
                itemTrackingService,
                null!,
                savedProgressProvider));
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;

        Assert.Throws<ArgumentNullException>(() =>
            new HomingScopeViewModel(
                itemTrackingService,
                parsedSpoilerDataService,
                null!));
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void Constructor_InitializesImageResourceKey()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.NotNull(viewModel.ImageResourceKey);
    }

    [Fact]
    public void Constructor_InitializesIsStarred()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.False(viewModel.IsStarred);
    }

    [Fact]
    public void Constructor_InitializesHoverText()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.NotNull(viewModel.HoverText);
    }

    #endregion

    #region ImageResourceKey Tests

    [Fact]
    public void ImageResourceKey_BothUnknown_ReturnsBwImage()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("homing_scope_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void ImageResourceKey_HomingKnown_ReturnsHomingOnly()
    {
        var homingState = CreateItemState(ItemName.HOMING_AMMO, RegionName.JUNGLE_JAPES);
        var itemTrackingService = CreateMockItemTrackingService(
            homingState: homingState).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("homingonly", viewModel.ImageResourceKey);
    }

    [Fact]
    public void ImageResourceKey_ScopeKnown_ReturnsScopeOnly()
    {
        var scopeState = CreateItemState(ItemName.SNIPER_SCOPE, RegionName.JUNGLE_JAPES);
        var itemTrackingService = CreateMockItemTrackingService(
            scopeState: scopeState).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("scopeonly", viewModel.ImageResourceKey);
    }

    [Fact]
    public void ImageResourceKey_BothKnown_ReturnsFullImage()
    {
        var homingState = CreateItemState(ItemName.HOMING_AMMO, RegionName.JUNGLE_JAPES);
        var scopeState = CreateItemState(ItemName.SNIPER_SCOPE, RegionName.ANGRY_AZTEC);
        var itemTrackingService = CreateMockItemTrackingService(
            homingState: homingState,
            scopeState: scopeState).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("homing_scope", viewModel.ImageResourceKey);
    }

    [Fact]
    public void ImageResourceKey_HomingInSpoilerData_IsKnown()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerData = new ParsedSpoilerData
        {
            StartingItems = new Dictionary<ItemName, RegionName>
            {
                { ItemName.HOMING_AMMO, RegionName.JUNGLE_JAPES }
            },
            LevelOrder = []
        };
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(spoilerData).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("homingonly", viewModel.ImageResourceKey);
    }

    #endregion

    #region IsStarred Tests

    [Fact]
    public void IsStarred_NeitherStarred_ReturnsFalse()
    {
        var homingState = CreateItemState(ItemName.HOMING_AMMO, starred: false);
        var scopeState = CreateItemState(ItemName.SNIPER_SCOPE, starred: false);
        var itemTrackingService = CreateMockItemTrackingService(
            homingState: homingState,
            scopeState: scopeState).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.False(viewModel.IsStarred);
    }

    [Fact]
    public void IsStarred_HomingStarred_ReturnsTrue()
    {
        var homingState = CreateItemState(ItemName.HOMING_AMMO, starred: true);
        var scopeState = CreateItemState(ItemName.SNIPER_SCOPE, starred: false);
        var itemTrackingService = CreateMockItemTrackingService(
            homingState: homingState,
            scopeState: scopeState).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.True(viewModel.IsStarred);
    }

    [Fact]
    public void IsStarred_ScopeStarred_ReturnsTrue()
    {
        var homingState = CreateItemState(ItemName.HOMING_AMMO, starred: false);
        var scopeState = CreateItemState(ItemName.SNIPER_SCOPE, starred: true);
        var itemTrackingService = CreateMockItemTrackingService(
            homingState: homingState,
            scopeState: scopeState).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.True(viewModel.IsStarred);
    }

    [Fact]
    public void IsStarred_BothStarred_ReturnsTrue()
    {
        var homingState = CreateItemState(ItemName.HOMING_AMMO, starred: true);
        var scopeState = CreateItemState(ItemName.SNIPER_SCOPE, starred: true);
        var itemTrackingService = CreateMockItemTrackingService(
            homingState: homingState,
            scopeState: scopeState).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.True(viewModel.IsStarred);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_CanBeCalled()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        viewModel.Dispose(); // Should not throw
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new HomingScopeViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        viewModel.Dispose();
        viewModel.Dispose(); // Should not throw
    }

    #endregion
}
