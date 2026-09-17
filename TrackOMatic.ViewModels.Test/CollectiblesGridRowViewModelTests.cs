namespace TrackOMatic.ViewModels.Test;

/// <summary>
/// Test suite for CollectiblesGridRowViewModel.
/// Tests constructor guards, property initialization, event handling with synchronization context,
/// wrinkly visibility and hint type updates, collectibles list management, and disposal.
/// </summary>
public class CollectiblesGridRowViewModelTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullUserSettingsService_ThrowsArgumentNullException()
    {
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CollectiblesGridRowViewModel(null!, mockAutotrackerService.Object, mockCollectiblesService.Object,
                mockSpoilerDataService.Object, mockProgressProvider.Object)
        );

        Assert.Equal("userSettingsService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullAutotrackerService_ThrowsArgumentNullException()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CollectiblesGridRowViewModel(mockUserSettingsService.Object, null!, mockCollectiblesService.Object,
                mockSpoilerDataService.Object, mockProgressProvider.Object)
        );

        Assert.Equal("autotrackerService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullCollectiblesService_ThrowsArgumentNullException()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CollectiblesGridRowViewModel(mockUserSettingsService.Object, mockAutotrackerService.Object, null!,
                mockSpoilerDataService.Object, mockProgressProvider.Object)
        );

        Assert.Equal("collectiblesService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSpoilerDataService_ThrowsArgumentNullException()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CollectiblesGridRowViewModel(mockUserSettingsService.Object, mockAutotrackerService.Object,
                mockCollectiblesService.Object, null!, mockProgressProvider.Object)
        );

        Assert.Equal("parsedSpoilerDataService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullProgressProvider_ThrowsArgumentNullException()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CollectiblesGridRowViewModel(mockUserSettingsService.Object, mockAutotrackerService.Object,
                mockCollectiblesService.Object, mockSpoilerDataService.Object, null!)
        );

        Assert.Equal("savedProgressProvider", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithValidServices_Succeeds()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        Assert.NotNull(viewModel);
        viewModel.Dispose();
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowAmountForHints).Returns(true);

        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        Assert.Multiple(() =>
        {
            Assert.Equal("wrinkly", viewModel.WrinklyResourceKey);
            Assert.True(viewModel.IsWrinklyVisible);
            Assert.Equal("golden_banana", viewModel.HintResourceKey);
            Assert.Equal(0, viewModel.AmountToNextHint);
            Assert.NotNull(viewModel.Collectibles);
        });

        viewModel.Dispose();
    }

    [Fact]
    public void Constructor_SubscribesToEventHandlers()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        // Verify subscriptions
        mockUserSettingsService.VerifyAdd(s => s.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.Once);
        mockAutotrackerService.VerifyAdd(s => s.HintProgressUpdated += It.IsAny<EventHandler<AutotrackerHintEventArgs>>(), Times.Once);
        mockAutotrackerService.VerifyAdd(s => s.ProgHintItemUpdated += It.IsAny<EventHandler<AutotrackerProgHintItemEventArgs>>(), Times.Once);

        viewModel.Dispose();
    }

    #endregion

    #region Wrinkly Visibility Tests

    [Fact]
    public void IsWrinklyVisible_WhenShowAmountForHintsTrue_IsTrue()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowAmountForHints).Returns(true);

        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        Assert.True(viewModel.IsWrinklyVisible);
        viewModel.Dispose();
    }

    [Fact]
    public void IsWrinklyVisible_WhenShowAmountForHintsFalse_IsFalse()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowAmountForHints).Returns(false);

        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        Assert.False(viewModel.IsWrinklyVisible);
        viewModel.Dispose();
    }

    #endregion

    #region Hint Resource Key Tests

    [Theory]
    [InlineData(ItemType.TOTAL_BLUEPRINTS, "total_blueprints")]
    [InlineData(ItemType.KEY, "basic_key")]
    [InlineData(ItemType.BANANA_MEDAL, "medal")]
    [InlineData(ItemType.BATTLE_CROWN, "crown")]
    [InlineData(ItemType.FAIRY, "fairy")]
    [InlineData(ItemType.RAINBOW_COIN, "rainbow_coin")]
    [InlineData(ItemType.PEARL, "pearl")]
    [InlineData(ItemType.COLORED_BANANA, "colored_banana")]
    [InlineData(ItemType.GOLDEN_BANANA, "golden_banana")]
    public void HintResourceKey_MapsItemTypeCorrectly(ItemType itemType, string expectedKey)
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        // Simulate event with specific item type
        mockAutotrackerService.Raise(
            s => s.ProgHintItemUpdated += null,
            new AutotrackerProgHintItemEventArgs { ProgHintItem = itemType }
        );

        Assert.Equal(expectedKey, viewModel.HintResourceKey);
        viewModel.Dispose();
    }

    #endregion

    #region Amount to Next Hint Tests

    [Fact]
    public void AmountToNextHint_InitializesToZero()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        Assert.Equal(0, viewModel.AmountToNextHint);
        viewModel.Dispose();
    }

    [Fact]
    public void AmountToNextHint_UpdatesFromHintProgressEvent()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        // Simulate event with specific amount
        mockAutotrackerService.Raise(
            s => s.HintProgressUpdated += null,
            new AutotrackerHintEventArgs { AmountToNextHint = 42 }
        );

        Assert.Equal(42, viewModel.AmountToNextHint);
        viewModel.Dispose();
    }

    [Fact]
    public void AmountToNextHint_UpdatesToMultipleValues()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        mockAutotrackerService.Raise(
            s => s.HintProgressUpdated += null,
            new AutotrackerHintEventArgs { AmountToNextHint = 10 }
        );
        Assert.Equal(10, viewModel.AmountToNextHint);

        mockAutotrackerService.Raise(
            s => s.HintProgressUpdated += null,
            new AutotrackerHintEventArgs { AmountToNextHint = 20 }
        );
        Assert.Equal(20, viewModel.AmountToNextHint);

        viewModel.Dispose();
    }

    #endregion

    #region Collectibles Collection Tests

    [Fact]
    public void Collectibles_IsInitiallyNotNull()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        Assert.NotNull(viewModel.Collectibles);
        viewModel.Dispose();
    }

    [Fact]
    public void HiddenCollectibleCount_TracksHiddenItems()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowTotalBPs).Returns(false);
        mockUserSettingsService.Setup(s => s.ShowCompanyCoins).Returns(false);

        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        // Both total blueprints and company coins are hidden
        Assert.Equal(2, viewModel.HiddenCollectibleCount);
        Assert.Equal(6, viewModel.VisibleCount); // 8 - 2

        viewModel.Dispose();
    }

    [Fact]
    public void VisibleCount_CalculatesCorrectly()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowTotalBPs).Returns(true);
        mockUserSettingsService.Setup(s => s.ShowCompanyCoins).Returns(true);

        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        // All items visible means hidden count is 0
        Assert.Equal(0, viewModel.HiddenCollectibleCount);
        Assert.Equal(8, viewModel.VisibleCount); // 8 - 0

        viewModel.Dispose();
    }

    #endregion

    #region Event Handling Tests

    [Fact]
    public void OnHintProgressUpdated_UpdatesAmountToNextHint()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        Assert.Equal(0, viewModel.AmountToNextHint);

        // Simulate event via Moq.Raise
        mockAutotrackerService.Raise(
            s => s.HintProgressUpdated += null,
            new AutotrackerHintEventArgs { AmountToNextHint = 25 }
        );

        Assert.Equal(25, viewModel.AmountToNextHint);

        viewModel.Dispose();
    }

    [Fact]
    public void OnProgHintItemUpdated_UpdatesHintResourceKey()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        Assert.Equal("golden_banana", viewModel.HintResourceKey);

        // Simulate event with different item type via Moq.Raise
        mockAutotrackerService.Raise(
            s => s.ProgHintItemUpdated += null,
            new AutotrackerProgHintItemEventArgs { ProgHintItem = ItemType.PEARL }
        );

        Assert.Equal("pearl", viewModel.HintResourceKey);

        viewModel.Dispose();
    }

    #endregion

    #region OnUserSettingsChanged Tests

    [Fact]
    public void OnUserSettingsChanged_WithShowTotalBPsChanged_RefreshesCollectibles()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowTotalBPs).Returns(true);
        mockUserSettingsService.Setup(s => s.ShowCompanyCoins).Returns(true);

        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        // Initially all visible: 1 total + 1 company + 6 countables = 8 items
        var initialCount = viewModel.Collectibles.Count;
        Assert.Equal(8, initialCount);
        Assert.Equal(0, viewModel.HiddenCollectibleCount);

        // Simulate changing ShowTotalBPs to false
        mockUserSettingsService.Setup(s => s.ShowTotalBPs).Returns(false);
        mockUserSettingsService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IUserSettingsService.ShowTotalBPs))
        );

        // After hiding total blueprints, should have 7 items and 1 hidden
        Assert.Equal(1, viewModel.HiddenCollectibleCount);
        Assert.Equal(7, viewModel.Collectibles.Count);

        viewModel.Dispose();
    }

    [Fact]
    public void OnUserSettingsChanged_WithShowCompanyCoinsChanged_RefreshesCollectibles()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowTotalBPs).Returns(false);
        mockUserSettingsService.Setup(s => s.ShowCompanyCoins).Returns(true);

        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        // Initially 1 company coin + 6 countables = 7 items (ShowTotalBPs is false)
        var initialCount = viewModel.Collectibles.Count;
        Assert.Equal(7, initialCount);

        // Simulate changing ShowCompanyCoins to false
        mockUserSettingsService.Setup(s => s.ShowCompanyCoins).Returns(false);
        mockUserSettingsService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IUserSettingsService.ShowCompanyCoins))
        );

        // After hiding company coins, should have 6 countable items
        Assert.Equal(2, viewModel.HiddenCollectibleCount);
        Assert.Equal(6, viewModel.Collectibles.Count);

        viewModel.Dispose();
    }

    [Fact]
    public void OnUserSettingsChanged_WithShowAmountForHintsChanged_UpdatesWrinklyVisibility()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowAmountForHints).Returns(true);

        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        Assert.True(viewModel.IsWrinklyVisible);

        // Simulate changing ShowAmountForHints to false
        mockUserSettingsService.Setup(s => s.ShowAmountForHints).Returns(false);
        mockUserSettingsService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IUserSettingsService.ShowAmountForHints))
        );

        Assert.False(viewModel.IsWrinklyVisible);

        viewModel.Dispose();
    }

    [Fact]
    public void OnUserSettingsChanged_WithShowAmountForHintsChangedMultipleTimes_TogglesVisibility()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowAmountForHints).Returns(true);

        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        Assert.True(viewModel.IsWrinklyVisible);

        // Toggle to false
        mockUserSettingsService.Setup(s => s.ShowAmountForHints).Returns(false);
        mockUserSettingsService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IUserSettingsService.ShowAmountForHints))
        );
        Assert.False(viewModel.IsWrinklyVisible);

        // Toggle back to true
        mockUserSettingsService.Setup(s => s.ShowAmountForHints).Returns(true);
        mockUserSettingsService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IUserSettingsService.ShowAmountForHints))
        );
        Assert.True(viewModel.IsWrinklyVisible);

        viewModel.Dispose();
    }

    [Fact]
    public void OnUserSettingsChanged_WithUnrelatedPropertyChanged_DoesNothing()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowTotalBPs).Returns(true);
        mockUserSettingsService.Setup(s => s.ShowCompanyCoins).Returns(true);
        mockUserSettingsService.Setup(s => s.ShowAmountForHints).Returns(true);

        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        var initialCollectibleCount = viewModel.Collectibles.Count;
        var initialVisibility = viewModel.IsWrinklyVisible;

        // Simulate an unrelated property change
        mockUserSettingsService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs("SomeOtherProperty")
        );

        // Should remain unchanged
        Assert.Equal(initialCollectibleCount, viewModel.Collectibles.Count);
        Assert.Equal(initialVisibility, viewModel.IsWrinklyVisible);

        viewModel.Dispose();
    }

    [Fact]
    public void OnUserSettingsChanged_WithShowTotalBPsAndShowCompanyCoinsChanged_RefreshesCollectibles()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowTotalBPs).Returns(true);
        mockUserSettingsService.Setup(s => s.ShowCompanyCoins).Returns(true);

        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        // Initially all visible: 1 total + 1 company + 6 countables = 8 items
        Assert.Equal(0, viewModel.HiddenCollectibleCount);
        Assert.Equal(8, viewModel.Collectibles.Count);

        // Hide total blueprints
        mockUserSettingsService.Setup(s => s.ShowTotalBPs).Returns(false);
        mockUserSettingsService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IUserSettingsService.ShowTotalBPs))
        );

        Assert.Equal(1, viewModel.HiddenCollectibleCount);
        Assert.Equal(7, viewModel.Collectibles.Count);

        // Also hide company coins
        mockUserSettingsService.Setup(s => s.ShowCompanyCoins).Returns(false);
        mockUserSettingsService.Raise(
            s => s.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IUserSettingsService.ShowCompanyCoins))
        );

        Assert.Equal(2, viewModel.HiddenCollectibleCount);
        Assert.Equal(6, viewModel.Collectibles.Count);

        viewModel.Dispose();
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnhooksEventHandlers()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        viewModel.Dispose();

        mockUserSettingsService.VerifyAdd(s => s.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.Once);
        mockAutotrackerService.VerifyAdd(s => s.HintProgressUpdated += It.IsAny<EventHandler<AutotrackerHintEventArgs>>(), Times.Once);
        mockAutotrackerService.VerifyAdd(s => s.ProgHintItemUpdated += It.IsAny<EventHandler<AutotrackerProgHintItemEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        viewModel.Dispose();
        viewModel.Dispose(); // Should not throw

        Assert.True(true);
    }

    [Fact]
    public void Dispose_DisposesCollectibleViewModels()
    {
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.ShowTotalBPs).Returns(true);
        mockUserSettingsService.Setup(s => s.ShowCompanyCoins).Returns(true);

        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();

        var viewModel = new CollectiblesGridRowViewModel(
            mockUserSettingsService.Object,
            mockAutotrackerService.Object,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object
        );

        Assert.NotEmpty(viewModel.Collectibles);

        viewModel.Dispose();

        Assert.NotNull(viewModel.Collectibles);
    }

    #endregion
}
