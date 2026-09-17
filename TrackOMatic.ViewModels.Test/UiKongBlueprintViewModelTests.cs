namespace TrackOMatic.ViewModels.Test;

/// <summary>
/// Test suite for UiKongBlueprintViewModel.
/// Tests constructor validation, blueprint type pairing, count increment/decrement with clamping,
/// command gating based on autotracking, event handling with synchronization context,
/// ToString override, and equality/hashing.
/// </summary>
public class UiKongBlueprintViewModelTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullUserSettingsService_ThrowsArgumentNullException()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            new UiKongBlueprintViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED,
                mockCollectiblesService.Object,
                mockSpoilerDataService.Object,
                mockProgressProvider.Object,
                null!
            )
        );

        Assert.Equal("userSettingsService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullCollectiblesService_ThrowsArgumentNullException()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            new UiKongBlueprintViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED,
                null!,
                mockSpoilerDataService.Object,
                mockProgressProvider.Object,
                mockUserSettingsService.Object
            )
        );

        Assert.Equal("collectiblesService", exception.ParamName);
    }

    [Theory]
    [InlineData(ItemType.COMPANY_COIN, ItemType.DONKEY_BLUEPRINT_TURNED)]
    [InlineData(ItemType.PEARL, ItemType.DONKEY_BLUEPRINT_TURNED)]
    [InlineData(ItemType.DONKEY_BLUEPRINT, ItemType.COMPANY_COIN)]
    [InlineData(ItemType.DONKEY_BLUEPRINT, ItemType.PEARL)]
    public void Constructor_WithInvalidItemTypes_ThrowsArgumentException(ItemType collectedType, ItemType turnedInType)
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var exception = Assert.Throws<ArgumentException>(() =>
            new UiKongBlueprintViewModel(
                collectedType,
                turnedInType,
                mockCollectiblesService.Object,
                mockSpoilerDataService.Object,
                mockProgressProvider.Object,
                mockUserSettingsService.Object
            )
        );

        Assert.NotNull(exception);
    }

    [Theory]
    [InlineData(ItemType.DONKEY_BLUEPRINT, ItemType.DONKEY_BLUEPRINT_TURNED)]
    [InlineData(ItemType.DIDDY_BLUEPRINT, ItemType.DIDDY_BLUEPRINT_TURNED)]
    [InlineData(ItemType.LANKY_BLUEPRINT, ItemType.LANKY_BLUEPRINT_TURNED)]
    [InlineData(ItemType.TINY_BLUEPRINT, ItemType.TINY_BLUEPRINT_TURNED)]
    [InlineData(ItemType.CHUNKY_BLUEPRINT, ItemType.CHUNKY_BLUEPRINT_TURNED)]
    public void Constructor_WithValidItemTypes_Succeeds(ItemType collectedType, ItemType turnedInType)
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);

        var viewModel = new UiKongBlueprintViewModel(
            collectedType,
            turnedInType,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.NotNull(viewModel);
        viewModel.Dispose();
    }

    [Fact]
    public void Constructor_InitializesCountFromService()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT)).Returns(7);
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT_TURNED)).Returns(3);

        var viewModel = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        // Count should be 7 - 3 = 4
        Assert.Equal(4, viewModel.Count);

        viewModel.Dispose();
    }

    #endregion

    #region CanChangeCount Gate Tests

    [Fact]
    public void CanChangeCount_WhenAutotrackingEnabled_ReturnsFalse()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(true);

        var viewModel = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.False(viewModel.IncrementCountCommand.CanExecute(null));
        Assert.False(viewModel.DecrementCountCommand.CanExecute(null));

        viewModel.Dispose();
    }

    [Fact]
    public void CanChangeCount_WhenAutotrackingDisabled_ReturnsTrue()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.True(viewModel.IncrementCountCommand.CanExecute(null));
        Assert.True(viewModel.DecrementCountCommand.CanExecute(null));

        viewModel.Dispose();
    }

    #endregion

    #region IncrementCount Tests

    [Theory]
    [InlineData(0, 1)]
    [InlineData(4, 5)]
    [InlineData(7, 8)]
    [InlineData(8, 8)] // Clamped at 8
    public void IncrementCount_IncrementsAndClamps(int currentCount, int expectedCount)
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var getCount = 0;
        mockCollectiblesService
            .Setup(s => s.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT))
            .Returns(() => getCount);
        mockCollectiblesService
            .Setup(s => s.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT_TURNED))
            .Returns(0);
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        // Set initial count
        getCount = currentCount;

        var viewModel = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        // Execute the command
        viewModel.IncrementCountCommand.Execute(null);

        // Verify SetCollectibleCount was called with the correct clamped value
        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.DONKEY_BLUEPRINT, expectedCount),
            Times.Once
        );

        viewModel.Dispose();
    }

    [Fact]
    public void IncrementCount_WhenAutotrackingEnabled_DoesNotExecute()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(true);

        var viewModel = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.False(viewModel.IncrementCountCommand.CanExecute(null));

        viewModel.Dispose();
    }

    #endregion

    #region DecrementCount Tests

    [Theory]
    [InlineData(8, 7)]
    [InlineData(4, 3)]
    [InlineData(1, 0)]
    [InlineData(0, 0)] // Clamped at 0
    public void DecrementCount_DecrementsAndClamps(int currentCount, int expectedCount)
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var getCount = 0;
        mockCollectiblesService
            .Setup(s => s.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT))
            .Returns(() => getCount);
        mockCollectiblesService
            .Setup(s => s.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT_TURNED))
            .Returns(0);
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        // Set initial count
        getCount = currentCount;

        var viewModel = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        // Execute the command
        viewModel.DecrementCountCommand.Execute(null);

        // Verify SetCollectibleCount was called with the correct clamped value
        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.DONKEY_BLUEPRINT, expectedCount),
            Times.Once
        );

        viewModel.Dispose();
    }

    [Fact]
    public void DecrementCount_WhenAutotrackingEnabled_DoesNotExecute()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(true);

        var viewModel = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.False(viewModel.DecrementCountCommand.CanExecute(null));

        viewModel.Dispose();
    }

    #endregion

    #region ToString Tests

    [Theory]
    [InlineData(ItemType.DONKEY_BLUEPRINT, 3)]
    [InlineData(ItemType.DIDDY_BLUEPRINT, 5)]
    [InlineData(ItemType.LANKY_BLUEPRINT, 0)]
    [InlineData(ItemType.CHUNKY_BLUEPRINT, 8)]
    public void ToString_FormatsOutputCorrectly(ItemType blueprintType, int count)
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(blueprintType)).Returns(count);
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsNotIn(blueprintType))).Returns(0);

        var viewModel = new UiKongBlueprintViewModel(
            blueprintType,
            GetMatchingTurnedInType(blueprintType),
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        var result = viewModel.ToString();

        Assert.Contains(count.ToString(), result);
        Assert.Contains(blueprintType.ToString(), result);
        Assert.Contains("blueprints on hand", result);

        viewModel.Dispose();
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameBlueprintType_ReturnsTrue()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);

        var viewModel1 = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        var viewModel2 = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.Equal(viewModel1, viewModel2);

        viewModel1.Dispose();
        viewModel2.Dispose();
    }

    [Fact]
    public void Equals_WithDifferentBlueprintType_ReturnsFalse()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);

        var viewModel1 = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        var viewModel2 = new UiKongBlueprintViewModel(
            ItemType.DIDDY_BLUEPRINT,
            ItemType.DIDDY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.NotEqual(viewModel1, viewModel2);

        viewModel1.Dispose();
        viewModel2.Dispose();
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);

        var viewModel = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.False(viewModel.Equals(null));

        viewModel.Dispose();
    }

    [Fact]
    public void GetHashCode_WithSameBlueprintType_IsSame()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);

        var viewModel1 = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        var viewModel2 = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.Equal(viewModel1.GetHashCode(), viewModel2.GetHashCode());

        viewModel1.Dispose();
        viewModel2.Dispose();
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnhooksEventHandlers()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);

        var viewModel = new UiKongBlueprintViewModel(
            ItemType.DONKEY_BLUEPRINT,
            ItemType.DONKEY_BLUEPRINT_TURNED,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        viewModel.Dispose();

        mockCollectiblesService.VerifyAdd(s => s.CollectibleChanged += It.IsAny<EventHandler<CollectiblesChangedEventArgs>>(), Times.Once);
        mockSpoilerDataService.VerifyAdd(s => s.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
        mockProgressProvider.VerifyAdd(s => s.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
    }

    #endregion

    #region Helper Methods

    private static ItemType GetMatchingTurnedInType(ItemType collectedType)
    {
        return collectedType switch
        {
            ItemType.DONKEY_BLUEPRINT => ItemType.DONKEY_BLUEPRINT_TURNED,
            ItemType.DIDDY_BLUEPRINT => ItemType.DIDDY_BLUEPRINT_TURNED,
            ItemType.LANKY_BLUEPRINT => ItemType.LANKY_BLUEPRINT_TURNED,
            ItemType.TINY_BLUEPRINT => ItemType.TINY_BLUEPRINT_TURNED,
            ItemType.CHUNKY_BLUEPRINT => ItemType.CHUNKY_BLUEPRINT_TURNED,
            _ => throw new ArgumentException("Invalid blueprint type", nameof(collectedType))
        };
    }

    #endregion
}
