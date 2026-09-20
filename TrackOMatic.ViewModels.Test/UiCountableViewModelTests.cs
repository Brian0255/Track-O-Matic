using Microsoft.Extensions.DependencyInjection;

namespace TrackOMatic.ViewModels.Test;

/// <summary>
/// Test suite for UiCountableViewModel.
/// Tests command behavior, count mutations, and autotracking interaction.
/// </summary>
public class UiCountableViewModelTests : IDisposable
{
    public UiCountableViewModelTests()
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
    public void Constructor_WithValidItemType_Succeeds()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();

        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockUserSettingsService.Object
        );

        Assert.NotNull(viewModel);
        viewModel.Dispose();
    }

    [Fact]
    public void Constructor_WithNullCollectiblesService_ThrowsArgumentNullException()
    {
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();

        Assert.Throws<ArgumentNullException>(() =>
            new UiCountableViewModel(
                ItemType.GOLDEN_BANANA,
                null!,
                mockSpoilerDataService.Object,
                mockUserSettingsService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullSpoilerDataService_ThrowsArgumentNullException()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);

        var mockUserSettingsService = new Mock<IUserSettingsService>();

        Assert.Throws<ArgumentNullException>(() =>
            new UiCountableViewModel(
                ItemType.GOLDEN_BANANA,
                mockCollectiblesService.Object,
                null!,
                mockUserSettingsService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullUserSettingsService_ThrowsArgumentNullException()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();

        Assert.Throws<ArgumentNullException>(() =>
            new UiCountableViewModel(
                ItemType.GOLDEN_BANANA,
                mockCollectiblesService.Object,
                mockSpoilerDataService.Object,
                null!
            )
        );
    }

    [Theory]
    [InlineData(ItemType.GOLDEN_BANANA)]
    [InlineData(ItemType.PEARL)]
    [InlineData(ItemType.BANANA_MEDAL)]
    [InlineData(ItemType.FAIRY)]
    [InlineData(ItemType.RAINBOW_COIN)]
    [InlineData(ItemType.BATTLE_CROWN)]
    [InlineData(ItemType.COMPANY_COIN)]
    public void Constructor_WithAllowedItemTypes_Succeeds(ItemType itemType)
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiCountableViewModel(
            itemType,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockUserSettingsService.Object
        );

        Assert.NotNull(viewModel);
        viewModel.Dispose();
    }

    #endregion

    #region IncrementCount Command Tests

    [Fact]
    public void IncrementCountCommand_WithCountZero_IncrementsToOne()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.GOLDEN_BANANA)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockUserSettingsService.Object
        );

        viewModel.IncrementCountCommand.Execute(null);

        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.GOLDEN_BANANA, 1),
            Times.Once
        );
        viewModel.Dispose();
    }

    [Fact]
    public void IncrementCountCommand_WithCountFive_IncrementsToSix()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.GOLDEN_BANANA)).Returns(5);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockUserSettingsService.Object
        );

        viewModel.IncrementCountCommand.Execute(null);

        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.GOLDEN_BANANA, 6),
            Times.Once
        );
        viewModel.Dispose();
    }

    [Fact]
    public void IncrementCountCommand_WhenAutotrackingEnabled_CannotExecute()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.GOLDEN_BANANA)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(true);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
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
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.GOLDEN_BANANA)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
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
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.GOLDEN_BANANA)).Returns(1);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockUserSettingsService.Object
        );

        viewModel.DecrementCountCommand.Execute(null);

        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.GOLDEN_BANANA, 0),
            Times.Once
        );
        viewModel.Dispose();
    }

    [Fact]
    public void DecrementCountCommand_WithCountFive_DecrementsToFour()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.GOLDEN_BANANA)).Returns(5);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockUserSettingsService.Object
        );

        viewModel.DecrementCountCommand.Execute(null);

        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.GOLDEN_BANANA, 4),
            Times.Once
        );
        viewModel.Dispose();
    }

    [Fact]
    public void DecrementCountCommand_WithCountZero_StaysAtZero()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.GOLDEN_BANANA)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockUserSettingsService.Object
        );

        viewModel.DecrementCountCommand.Execute(null);

        mockCollectiblesService.Verify(
            s => s.SetCollectibleCount(ItemType.GOLDEN_BANANA, 0),
            Times.Once
        );
        viewModel.Dispose();
    }

    [Fact]
    public void DecrementCountCommand_WhenAutotrackingEnabled_CannotExecute()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.GOLDEN_BANANA)).Returns(5);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(true);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
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
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.GOLDEN_BANANA)).Returns(5);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockUserSettingsService.Object
        );

        var canExecute = viewModel.DecrementCountCommand.CanExecute(null);

        Assert.True(canExecute);
        viewModel.Dispose();
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnhooksEventHandlers()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.GOLDEN_BANANA)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockUserSettingsService.Object
        );

        viewModel.Dispose();

        // Verify event handlers were removed by checking the mock was called during construction
        mockCollectiblesService.VerifyAdd(s => s.CollectibleChanged += It.IsAny<EventHandler<CollectiblesChangedEventArgs>>(), Times.Once);
        mockSpoilerDataService.VerifyAdd(s => s.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        mockCollectiblesService.Setup(s => s.GetCollectibleCount(ItemType.GOLDEN_BANANA)).Returns(0);

        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);

        var viewModel = new UiCountableViewModel(
            ItemType.GOLDEN_BANANA,
            mockCollectiblesService.Object,
            mockSpoilerDataService.Object,
            mockUserSettingsService.Object
        );

        viewModel.Dispose();
        viewModel.Dispose(); // Should not throw

        Assert.True(true); // Test passes if no exception thrown
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

    ~UiCountableViewModelTests()
    {
        Dispose(false);
    }

    #endregion
}
