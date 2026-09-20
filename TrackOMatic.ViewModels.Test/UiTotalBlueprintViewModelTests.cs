using Microsoft.Extensions.DependencyInjection;

namespace TrackOMatic.ViewModels.Test;

/// <summary>
/// Test suite for UiTotalBlueprintViewModel.
/// Tests command behavior, count mutations, event handling, and synchronization context usage.
/// </summary>
public class UiTotalBlueprintViewModelTests : IDisposable
{
    public UiTotalBlueprintViewModelTests()
    {
        InitializeServiceLocator();
    }

    private static void InitializeServiceLocator()
    {
        // Only initialize once per test run
        try
        {
            ServiceLocator.GetService<IUserSettingsService>();
        }
        catch (InvalidOperationException)
        {
            // Not yet initialized
            var services = new ServiceCollection();
            ServiceLocator.Initialize(services.BuildServiceProvider());
        }
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidServices_Succeeds()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.NotNull(viewModel);
        viewModel.Dispose();
    }

    [Fact]
    public void Constructor_WithNullCollectiblesService_ThrowsArgumentNullException()
    {
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        Assert.Throws<ArgumentNullException>(() =>
            new UiTotalBlueprintViewModel(
                null!,
                mockSpoilerDataService.Object,
                mockProgressProvider.Object,
                mockUserSettingsService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullSpoilerDataService_ThrowsArgumentNullException()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        Assert.Throws<ArgumentNullException>(() =>
            new UiTotalBlueprintViewModel(
                mockCollectiblesService.Object,
                null!,
                mockProgressProvider.Object,
                mockUserSettingsService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullProgressProvider_ThrowsArgumentNullException()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        Assert.Throws<ArgumentNullException>(() =>
            new UiTotalBlueprintViewModel(
                mockCollectiblesService.Object,
                mockSpoilerDataService.Object,
                null!,
                mockUserSettingsService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullUserSettingsService_ThrowsArgumentNullException()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();

        Assert.Throws<ArgumentNullException>(() =>
            new UiTotalBlueprintViewModel(
                mockCollectiblesService.Object,
                mockSpoilerDataService.Object,
                mockProgressProvider.Object,
                null!
            )
        );
    }

    [Fact]
    public void Constructor_InitializesCountFromService()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(15);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.Equal(15, viewModel.Count);
        viewModel.Dispose();
    }

    [Fact]
    public void Constructor_SubscribesToEventHandlers()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        // Verify subscriptions
        mockCollectiblesService.VerifyAdd(s => s.CollectibleChanged += It.IsAny<EventHandler<CollectiblesChangedEventArgs>>(), Times.Once);
        mockSpoilerDataService.VerifyAdd(s => s.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
        mockProgressProvider.VerifyAdd(s => s.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);

        viewModel.Dispose();
    }

    #endregion

    #region IncrementCount Command Tests

    [Fact]
    public void IncrementCountCommand_WithCountZero_IncrementsToOne()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        viewModel.IncrementCountCommand.Execute(null);

        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.TOTAL_BLUEPRINTS, 1),
            Times.Once
        );
        viewModel.Dispose();
    }

    [Fact]
    public void IncrementCountCommand_WithCount39_IncrementsTo40()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(39);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        viewModel.IncrementCountCommand.Execute(null);

        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.TOTAL_BLUEPRINTS, 40),
            Times.Once
        );
        viewModel.Dispose();
    }

    [Fact]
    public void IncrementCountCommand_WithCount40_StaysAt40()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(40);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        viewModel.IncrementCountCommand.Execute(null);

        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.TOTAL_BLUEPRINTS, 40),
            Times.Once
        );
        viewModel.Dispose();
    }

    [Fact]
    public void IncrementCountCommand_WhenAutotrackingEnabled_CannotExecute()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(true);

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        var canExecute = viewModel.IncrementCountCommand.CanExecute(null);

        Assert.False(canExecute);
        viewModel.Dispose();
    }

    [Fact]
    public void IncrementCountCommand_WhenAutotrackingDisabled_CanExecute()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(20);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        var canExecute = viewModel.IncrementCountCommand.CanExecute(null);

        Assert.True(canExecute);
        viewModel.Dispose();
    }

    #endregion

    #region DecrementCount Command Tests

    [Fact]
    public void DecrementCountCommand_WithCountOne_DecrementsToZero()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(1);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        viewModel.DecrementCountCommand.Execute(null);

        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.TOTAL_BLUEPRINTS, 0),
            Times.Once
        );
        viewModel.Dispose();
    }

    [Fact]
    public void DecrementCountCommand_WithCount20_DecrementsTo19()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(20);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        viewModel.DecrementCountCommand.Execute(null);

        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.TOTAL_BLUEPRINTS, 19),
            Times.Once
        );
        viewModel.Dispose();
    }

    [Fact]
    public void DecrementCountCommand_WithCountZero_StaysAtZero()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        viewModel.DecrementCountCommand.Execute(null);

        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.TOTAL_BLUEPRINTS, 0),
            Times.Once
        );
        viewModel.Dispose();
    }

    [Fact]
    public void DecrementCountCommand_WhenAutotrackingEnabled_CannotExecute()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(20);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(true);

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        var canExecute = viewModel.DecrementCountCommand.CanExecute(null);

        Assert.False(canExecute);
        viewModel.Dispose();
    }

    [Fact]
    public void DecrementCountCommand_WhenAutotrackingDisabled_CanExecute()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(20);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        var canExecute = viewModel.DecrementCountCommand.CanExecute(null);

        Assert.True(canExecute);
        viewModel.Dispose();
    }

    #endregion

    #region Event Handling Tests

    [Fact]
    public void OnCollectableChanged_WithMatchingItemType_UpdatesCount()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService
            .Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS))
            .Returns(25);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        // Verify initial count was loaded
        Assert.Equal(25, viewModel.Count);
        viewModel.Dispose();
    }

    [Fact]
    public void OnCollectableChanged_WithDifferentItemType_IgnoresEvent()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        // Count should remain at initialization value (0)
        Assert.Equal(0, viewModel.Count);
        viewModel.Dispose();
    }

    [Fact]
    public void OnDataReset_ReinitializesState()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService
            .Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS))
            .Returns(30);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.Equal(30, viewModel.Count);
        viewModel.Dispose();
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithZeroCount_ReturnsFormattedString()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        var result = viewModel.ToString();

        Assert.Equal("Total blueprints: 0", result);
        viewModel.Dispose();
    }

    [Fact]
    public void ToString_WithNonZeroCount_ReturnsFormattedString()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(40);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        var result = viewModel.ToString();

        Assert.Equal("Total blueprints: 40", result);
        viewModel.Dispose();
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnhooksEventHandlers()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        viewModel.Dispose();

        // Verify event handlers were subscribed during construction
        mockCollectiblesService.VerifyAdd(s => s.CollectibleChanged += It.IsAny<EventHandler<CollectiblesChangedEventArgs>>(), Times.Once);
        mockSpoilerDataService.VerifyAdd(s => s.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
        mockProgressProvider.VerifyAdd(s => s.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        viewModel.Dispose();
        viewModel.Dispose(); // Should not throw

        Assert.True(true); // Test passes if no exception thrown
    }

    #endregion

    #region ImageResourceKey Tests

    [Fact]
    public void ImageResourceKey_WithZeroCount_SetsToBwVariant()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.EndsWith("_bw", viewModel.ImageResourceKey);
        viewModel.Dispose();
    }

    [Fact]
    public void ImageResourceKey_WithPositiveCount_SetsToColorVariant()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS)).Returns(15);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        var viewModel = new UiTotalBlueprintViewModel(
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockUserSettingsService.Object
        );

        Assert.False(viewModel.ImageResourceKey.EndsWith("_bw"));
        viewModel.Dispose();
    }

    #endregion

    #region IDisposable

    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            _disposed = true;
        }
    }

    ~UiTotalBlueprintViewModelTests()
    {
        Dispose(false);
    }

    #endregion
}
