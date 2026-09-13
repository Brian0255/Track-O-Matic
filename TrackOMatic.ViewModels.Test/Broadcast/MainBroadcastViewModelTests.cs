namespace TrackOMatic.ViewModels.Test.Broadcast;

public class MainBroadcastViewModelTests
{
    private readonly Mock<IUserSettingsService> _userSettingsServiceMock;
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock;
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock;
    private readonly Mock<ILevelOrderService> _levelOrderServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;
    private readonly Mock<IEndGameProgressionService> _endGameProgressionServiceMock;
    private readonly Mock<IThemeService> _themeServiceMock;
    private readonly Mock<ICollectiblesService> _collectiblesServiceMock;
    private readonly Mock<ISongService> _songServiceMock;
    private readonly Mock<IDataSaverService> _dataSaverServiceMock;

    public MainBroadcastViewModelTests()
    {
        _userSettingsServiceMock = new Mock<IUserSettingsService>();
        _parsedSpoilerDataServiceMock = new Mock<IParsedSpoilerDataService>();
        _itemTrackingServiceMock = new Mock<IItemTrackingService>();
        _levelOrderServiceMock = new Mock<ILevelOrderService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
        _endGameProgressionServiceMock = new Mock<IEndGameProgressionService>();
        _themeServiceMock = new Mock<IThemeService>();
        _collectiblesServiceMock = new Mock<ICollectiblesService>();
        _songServiceMock = new Mock<ISongService>();
        _dataSaverServiceMock = new Mock<IDataSaverService>();
    }

    private MainBroadcastViewModel CreateViewModel()
    {
        return new MainBroadcastViewModel(
            _userSettingsServiceMock.Object,
            _parsedSpoilerDataServiceMock.Object,
            _itemTrackingServiceMock.Object,
            _levelOrderServiceMock.Object,
            _savedProgressProviderMock.Object,
            _endGameProgressionServiceMock.Object,
            _themeServiceMock.Object,
            _collectiblesServiceMock.Object,
            _songServiceMock.Object,
            _dataSaverServiceMock.Object
        );
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidDependencies_Succeeds()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel);
    }

    [Fact]
    public void Constructor_WithNullUserSettingsService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new MainBroadcastViewModel(
                null!,
                _parsedSpoilerDataServiceMock.Object,
                _itemTrackingServiceMock.Object,
                _levelOrderServiceMock.Object,
                _savedProgressProviderMock.Object,
                _endGameProgressionServiceMock.Object,
                _themeServiceMock.Object,
                _collectiblesServiceMock.Object,
                _songServiceMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("userSettingsService", ex.ParamName);
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void Constructor_CreatesLevelViewModels()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel.Levels);
        Assert.NotEmpty(viewModel.Levels);
    }

    [Fact]
    public void Constructor_CreatesHelmSlotViewModels()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel.HelmSlots);
        Assert.NotEmpty(viewModel.HelmSlots);
    }

    [Fact]
    public void Constructor_CreatesBossSlotViewModels()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel.BossSlots);
        Assert.NotEmpty(viewModel.BossSlots);
    }

    [Fact]
    public void Constructor_CreatesKongItemViewModels()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel.KongItems);
        Assert.NotEmpty(viewModel.KongItems);
    }

    [Fact]
    public void Constructor_CreatesShopkeepItemViewModels()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel.ShopkeepItems);
        Assert.NotEmpty(viewModel.ShopkeepItems);
    }

    [Fact]
    public void Constructor_CreatesCountableViewModels()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel.Countables);
        Assert.NotEmpty(viewModel.Countables);
    }

    [Fact]
    public void Constructor_CreatesKongBlueprintViewModels()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel.KongBlueprints);
        Assert.NotEmpty(viewModel.KongBlueprints);
    }

    #endregion

    #region UserSettings Subscription Tests

    [Fact]
    public void Constructor_SubscribesToUserSettingsPropertyChanged()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _userSettingsServiceMock.VerifyAdd(x => x.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.Once);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromUserSettings()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.Dispose();

        // Assert
        _userSettingsServiceMock.VerifyRemove(x => x.PropertyChanged -= It.IsAny<PropertyChangedEventHandler>(), Times.Once);
    }

    [Fact]
    public void Dispose_DisposesAllNestedViewModels()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.Dispose();

        // Assert - verify that nested viewmodels unsubscribe from services
        Assert.Multiple(
            () => _itemTrackingServiceMock.VerifyRemove(x => x.ItemStateChanged -= It.IsAny<EventHandler<ItemStateChangedEventArgs>>(), Times.AtLeastOnce),
            () => _parsedSpoilerDataServiceMock.VerifyRemove(x => x.ParsedSpoilerDataChanged -= It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.AtLeastOnce)
        );
    }

    #endregion
}
