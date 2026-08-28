using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Test;

public class RegionLevelViewModelTests
{
    private static Mock<ILevelOrderService> CreateMockLevelOrderService(List<int>? levelOrder = null)
    {
        var mock = new Mock<ILevelOrderService>();
        levelOrder ??= [0, 0, 0, 0, 0, 0, 0, 0];

        mock.Setup(s => s.GetLevelOrder()).Returns(levelOrder);
        mock.Setup(s => s.SetLevelOrder(It.IsAny<IEnumerable<int>>()))
            .Callback<IEnumerable<int>>(newOrder =>
            {
                levelOrder.Clear();
                levelOrder.AddRange(newOrder);
            });

        return mock;
    }

    private static Mock<IUserSettingsService> CreateMockUserSettingsService(bool helmInLevelOrder = false)
    {
        var mock = new Mock<IUserSettingsService>();
        mock.Setup(s => s.HelmInLevelOrder).Returns(helmInLevelOrder);
        return mock;
    }

    private static Mock<IParsedSpoilerDataService> CreateMockParsedSpoilerDataService(bool hasLevelOrder = false)
    {
        var mock = new Mock<IParsedSpoilerDataService>();

        ParsedSpoilerData? setupData = hasLevelOrder
            ? new ParsedSpoilerData { LevelOrder = [] }
            : null;

        mock.Setup(s => s.CurrentData).Returns(setupData);

        return mock;
    }

    private static Mock<ISavedProgressProvider> CreateMockSavedProgressProvider()
    {
        var mock = new Mock<ISavedProgressProvider>();
        return mock;
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidDependencies_Succeeds()
    {
        var levelOrderService = CreateMockLevelOrderService().Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.NotNull(viewModel);
    }

    [Fact]
    public void Constructor_WithNullLevelOrderService_ThrowsArgumentNullException()
    {
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        Assert.Throws<ArgumentNullException>(() =>
            new RegionLevelViewModel(
                RegionName.JUNGLE_JAPES,
                null!,
                userSettingsService,
                parsedSpoilerDataService,
                savedProgressProvider));
    }

    [Fact]
    public void Constructor_WithNullUserSettingsService_ThrowsArgumentNullException()
    {
        var levelOrderService = CreateMockLevelOrderService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        Assert.Throws<ArgumentNullException>(() =>
            new RegionLevelViewModel(
                RegionName.JUNGLE_JAPES,
                levelOrderService,
                null!,
                parsedSpoilerDataService,
                savedProgressProvider));
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        var levelOrderService = CreateMockLevelOrderService().Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        Assert.Throws<ArgumentNullException>(() =>
            new RegionLevelViewModel(
                RegionName.JUNGLE_JAPES,
                levelOrderService,
                userSettingsService,
                null!,
                savedProgressProvider));
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        var levelOrderService = CreateMockLevelOrderService().Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;

        Assert.Throws<ArgumentNullException>(() =>
            new RegionLevelViewModel(
                RegionName.JUNGLE_JAPES,
                levelOrderService,
                userSettingsService,
                parsedSpoilerDataService,
                null!));
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void LevelOrderNumber_InitializesToZero()
    {
        var levelOrderService = CreateMockLevelOrderService(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 }).Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal(0, viewModel.LevelOrderNumber);
    }

    [Fact]
    public void LevelText_InitializesToQuestionMark()
    {
        var levelOrderService = CreateMockLevelOrderService(new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 }).Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal("?", viewModel.LevelText);
    }

    #endregion

    #region Level Text Generation Tests

    [Theory]
    [InlineData(1, "1")]
    [InlineData(2, "2")]
    [InlineData(3, "3")]
    [InlineData(4, "4")]
    [InlineData(5, "5")]
    [InlineData(6, "6")]
    [InlineData(7, "7")]
    [InlineData(8, "8")]
    [InlineData(0, "?")]
    public void LevelText_MatchesLevelOrderNumber(int levelNumber, string expectedText)
    {
        var levelOrders = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 };
        levelOrders[0] = levelNumber; // Set for JUNGLE_JAPES (index 0)
        var levelOrderService = CreateMockLevelOrderService(levelOrders).Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal(expectedText, viewModel.LevelText);
    }

    #endregion

    #region Increment/Decrement Level Order Tests (Without spoiler data - should not apply)

    [Fact]
    public void IncrementLevelOrder_WithoutSpoilerData_DoesNothing()
    {
        var levelOrders = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 };
        var levelOrderService = CreateMockLevelOrderService(levelOrders).Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(hasLevelOrder: false).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        viewModel.IncrementLevelOrderCommand.Execute(null);

        Assert.Equal(0, viewModel.LevelOrderNumber);
    }

    [Fact]
    public void DecrementLevelOrder_WithoutSpoilerData_DoesNothing()
    {
        var levelOrders = new List<int> { 1, 0, 0, 0, 0, 0, 0, 0 };
        var levelOrderService = CreateMockLevelOrderService(levelOrders).Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(hasLevelOrder: false).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        viewModel.DecrementLevelOrderCommand.Execute(null);

        Assert.Equal(1, viewModel.LevelOrderNumber);
    }

    #endregion

    #region Helmet-Block Tests

    [Fact]
    public void IncrementLevelOrder_HelmBlockedWhenNotInOrder()
    {
        var levelOrders = new List<int> { 8, 0, 0, 0, 0, 0, 0, 8 };
        var levelOrderService = CreateMockLevelOrderService(levelOrders);
        var userSettingsService = CreateMockUserSettingsService(helmInLevelOrder: false).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(hasLevelOrder: true).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new RegionLevelViewModel(
            RegionName.HIDEOUT_HELM,
            levelOrderService.Object,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        viewModel.IncrementLevelOrderCommand.Execute(null);

        Assert.Equal(8, viewModel.LevelOrderNumber);
    }

    [Fact]
    public void HelmForcedToEightOnInitialization_WhenHelmInLevelOrderIsFalse()
    {
        // Arrange: Level order array with Helm (index 7) set to 4 (not 8)
        var levelOrders = new List<int> { 1, 2, 3, 8, 5, 6, 7, 4 }; // Helm at index 7 is 4
        var levelOrderService = CreateMockLevelOrderService(levelOrders);
        var userSettingsService = CreateMockUserSettingsService(helmInLevelOrder: false).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(hasLevelOrder: false).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        // Act: Create ViewModel for HIDEOUT_HELM with setting disabled
        var viewModel = new RegionLevelViewModel(
            RegionName.HIDEOUT_HELM,
            levelOrderService.Object,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        // Assert: Helm should be forced to 8 despite level order saying 4
        Assert.Multiple(() =>
        {
            Assert.Equal(8, viewModel.LevelOrderNumber);
            Assert.Equal("8", viewModel.LevelText);
        });
    }

    [Fact]
    public void HelmNotForcedToEight_WhenHelmInLevelOrderIsTrue()
    {
        // Arrange: Level order array with Helm (index 7) set to 3
        var levelOrders = new List<int> { 1, 2, 8, 4, 5, 6, 7, 3 }; // Helm at index 7 is 3
        var levelOrderService = CreateMockLevelOrderService(levelOrders);
        var userSettingsService = CreateMockUserSettingsService(helmInLevelOrder: true).Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService(hasLevelOrder: false).Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        // Act: Create ViewModel for HIDEOUT_HELM with setting enabled
        var viewModel = new RegionLevelViewModel(
            RegionName.HIDEOUT_HELM,
            levelOrderService.Object,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        // Assert: Helm should use the actual level order value (3), not forced to 8
        Assert.Multiple(() =>
        {
            Assert.Equal(3, viewModel.LevelOrderNumber);
            Assert.Equal("3", viewModel.LevelText);
        });
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_CanBeCalled()
    {
        var levelOrderService = CreateMockLevelOrderService().Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        viewModel.Dispose(); // Should not throw
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var levelOrderService = CreateMockLevelOrderService().Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        viewModel.Dispose();
        viewModel.Dispose(); // Should not throw
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameRegionName_ReturnsTrue()
    {
        var levelOrderService = CreateMockLevelOrderService().Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel1 = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        var viewModel2 = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.True(viewModel1.Equals(viewModel2));
    }

    [Fact]
    public void Equals_WithDifferentRegionName_ReturnsFalse()
    {
        var levelOrderService = CreateMockLevelOrderService().Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel1 = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        var viewModel2 = new RegionLevelViewModel(
            RegionName.ANGRY_AZTEC,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.False(viewModel1.Equals(viewModel2));
    }

    [Fact]
    public void GetHashCode_WithSameRegionName_ReturnsSameHashCode()
    {
        var levelOrderService = CreateMockLevelOrderService().Object;
        var userSettingsService = CreateMockUserSettingsService().Object;
        var parsedSpoilerDataService = CreateMockParsedSpoilerDataService().Object;
        var savedProgressProvider = CreateMockSavedProgressProvider().Object;

        var viewModel1 = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        var viewModel2 = new RegionLevelViewModel(
            RegionName.JUNGLE_JAPES,
            levelOrderService,
            userSettingsService,
            parsedSpoilerDataService,
            savedProgressProvider);

        Assert.Equal(viewModel1.GetHashCode(), viewModel2.GetHashCode());
    }

    #endregion
}
