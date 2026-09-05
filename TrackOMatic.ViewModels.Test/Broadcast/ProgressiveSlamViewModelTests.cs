using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels.Test.Broadcast;

public class ProgressiveSlamViewModelTests
{
    private static Mock<IItemTrackingService> CreateMockItemTrackingService(
        SavedItem? slam1State = null,
        SavedItem? slam2State = null,
        SavedItem? slam3State = null)
    {
        var mock = new Mock<IItemTrackingService>();

        mock.Setup(s => s.GetItemState(ItemName.PROGRESSIVE_SLAM_1))
            .Returns(slam1State);
        mock.Setup(s => s.GetItemState(ItemName.PROGRESSIVE_SLAM_2))
            .Returns(slam2State);
        mock.Setup(s => s.GetItemState(ItemName.PROGRESSIVE_SLAM_3))
            .Returns(slam3State);

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

        var viewModel = new ProgressiveSlamViewModel(
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
            new ProgressiveSlamViewModel(
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
            new ProgressiveSlamViewModel(
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
            new ProgressiveSlamViewModel(
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

        var viewModel = new ProgressiveSlamViewModel(
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

        var viewModel = new ProgressiveSlamViewModel(
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

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.NotNull(viewModel.HoverText);
    }

    #endregion

    #region ImageResourceKey Tests

    [Fact]
    public void ImageResourceKey_NoneKnown_ReturnsBwImage()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("progressive_slam_1_bc_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void ImageResourceKey_OneKnown_Returns1Image()
    {
        var slam1State = CreateItemState(ItemName.PROGRESSIVE_SLAM_1, RegionName.JUNGLE_JAPES);
        var itemTrackingService = CreateMockItemTrackingService(slam1State: slam1State).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("progressive_slam_1_bc", viewModel.ImageResourceKey);
    }

    [Fact]
    public void ImageResourceKey_TwoKnown_Returns2Image()
    {
        var slam1State = CreateItemState(ItemName.PROGRESSIVE_SLAM_1, RegionName.JUNGLE_JAPES);
        var slam2State = CreateItemState(ItemName.PROGRESSIVE_SLAM_2, RegionName.ANGRY_AZTEC);
        var itemTrackingService = CreateMockItemTrackingService(
            slam1State: slam1State,
            slam2State: slam2State).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("progressive_slam_2_bc", viewModel.ImageResourceKey);
    }

    [Fact]
    public void ImageResourceKey_ThreeKnown_Returns3Image()
    {
        var slam1State = CreateItemState(ItemName.PROGRESSIVE_SLAM_1, RegionName.JUNGLE_JAPES);
        var slam2State = CreateItemState(ItemName.PROGRESSIVE_SLAM_2, RegionName.ANGRY_AZTEC);
        var slam3State = CreateItemState(ItemName.PROGRESSIVE_SLAM_3, RegionName.FUNGI_FOREST);
        var itemTrackingService = CreateMockItemTrackingService(
            slam1State: slam1State,
            slam2State: slam2State,
            slam3State: slam3State).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("progressive_slam_3_bc", viewModel.ImageResourceKey);
    }

    [Fact]
    public void ImageResourceKey_Slam1InSpoilerData_IsKnown()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerData = new ParsedSpoilerData
        {
            StartingItems = new Dictionary<ItemName, RegionName>
            {
                { ItemName.PROGRESSIVE_SLAM_1, RegionName.JUNGLE_JAPES }
            },
            LevelOrder = []
        };
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(spoilerData).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("progressive_slam_1_bc", viewModel.ImageResourceKey);
    }

    [Fact]
    public void ImageResourceKey_Slam2InSpoilerData_IsKnown()
    {
        var itemTrackingService = CreateMockItemTrackingService().Object;
        var spoilerData = new ParsedSpoilerData
        {
            StartingItems = new Dictionary<ItemName, RegionName>
            {
                { ItemName.PROGRESSIVE_SLAM_1, RegionName.JUNGLE_JAPES },
                { ItemName.PROGRESSIVE_SLAM_2, RegionName.ANGRY_AZTEC }
            },
            LevelOrder = []
        };
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(spoilerData).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("progressive_slam_2_bc", viewModel.ImageResourceKey);
    }

    [Fact]
    public void ImageResourceKey_MixedSpoilerAndTrackedItems_CountsCorrectly()
    {
        var slam1State = CreateItemState(ItemName.PROGRESSIVE_SLAM_1, RegionName.JUNGLE_JAPES);
        var itemTrackingService = CreateMockItemTrackingService(slam1State: slam1State).Object;
        var spoilerData = new ParsedSpoilerData
        {
            StartingItems = new Dictionary<ItemName, RegionName>
            {
                { ItemName.PROGRESSIVE_SLAM_2, RegionName.ANGRY_AZTEC }
            },
            LevelOrder = []
        };
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(spoilerData).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("progressive_slam_2_bc", viewModel.ImageResourceKey);
    }

    #endregion

    #region IsStarred Tests

    [Fact]
    public void IsStarred_NoneStarred_ReturnsFalse()
    {
        var slam1State = CreateItemState(ItemName.PROGRESSIVE_SLAM_1, starred: false);
        var slam2State = CreateItemState(ItemName.PROGRESSIVE_SLAM_2, starred: false);
        var slam3State = CreateItemState(ItemName.PROGRESSIVE_SLAM_3, starred: false);
        var itemTrackingService = CreateMockItemTrackingService(
            slam1State: slam1State,
            slam2State: slam2State,
            slam3State: slam3State).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.False(viewModel.IsStarred);
    }

    [Fact]
    public void IsStarred_Slam1Starred_ReturnsTrue()
    {
        var slam1State = CreateItemState(ItemName.PROGRESSIVE_SLAM_1, starred: true);
        var slam2State = CreateItemState(ItemName.PROGRESSIVE_SLAM_2, starred: false);
        var slam3State = CreateItemState(ItemName.PROGRESSIVE_SLAM_3, starred: false);
        var itemTrackingService = CreateMockItemTrackingService(
            slam1State: slam1State,
            slam2State: slam2State,
            slam3State: slam3State).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.True(viewModel.IsStarred);
    }

    [Fact]
    public void IsStarred_Slam2Starred_ReturnsTrue()
    {
        var slam1State = CreateItemState(ItemName.PROGRESSIVE_SLAM_1, starred: false);
        var slam2State = CreateItemState(ItemName.PROGRESSIVE_SLAM_2, starred: true);
        var slam3State = CreateItemState(ItemName.PROGRESSIVE_SLAM_3, starred: false);
        var itemTrackingService = CreateMockItemTrackingService(
            slam1State: slam1State,
            slam2State: slam2State,
            slam3State: slam3State).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.True(viewModel.IsStarred);
    }

    [Fact]
    public void IsStarred_Slam3Starred_ReturnsTrue()
    {
        var slam1State = CreateItemState(ItemName.PROGRESSIVE_SLAM_1, starred: false);
        var slam2State = CreateItemState(ItemName.PROGRESSIVE_SLAM_2, starred: false);
        var slam3State = CreateItemState(ItemName.PROGRESSIVE_SLAM_3, starred: true);
        var itemTrackingService = CreateMockItemTrackingService(
            slam1State: slam1State,
            slam2State: slam2State,
            slam3State: slam3State).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.True(viewModel.IsStarred);
    }

    [Fact]
    public void IsStarred_MultipleStarred_ReturnsTrue()
    {
        var slam1State = CreateItemState(ItemName.PROGRESSIVE_SLAM_1, starred: true);
        var slam2State = CreateItemState(ItemName.PROGRESSIVE_SLAM_2, starred: true);
        var slam3State = CreateItemState(ItemName.PROGRESSIVE_SLAM_3, starred: false);
        var itemTrackingService = CreateMockItemTrackingService(
            slam1State: slam1State,
            slam2State: slam2State,
            slam3State: slam3State).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(
            new ParsedSpoilerData { StartingItems = [], LevelOrder = [] }).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new ProgressiveSlamViewModel(
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

        var viewModel = new ProgressiveSlamViewModel(
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

        var viewModel = new ProgressiveSlamViewModel(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider);

        viewModel.Dispose();
        viewModel.Dispose(); // Should not throw
    }

    #endregion
}
