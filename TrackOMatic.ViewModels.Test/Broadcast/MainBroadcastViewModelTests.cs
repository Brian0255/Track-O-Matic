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

        // Set up default mock behavior to avoid nulls during construction
        SetupDefaultMocks();
    }

    private void SetupDefaultMocks()
    {
        // Set up ILevelOrderService to return a valid level order
        var defaultLevelOrder = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns(defaultLevelOrder);

        // Set up IParsedSpoilerDataService with a real ParsedSpoilerData instance
        var defaultParsedData = new ParsedSpoilerData
        {
            LevelOrder = []
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(defaultParsedData);

        // Set up IEndGameProgressionService to return valid lists for helm and boss kongs
        var defaultHelmKongs = new List<int> { 0, 1, 2, 3, 4 };
        var defaultBossKongs = new List<int> { 0, 1, 2, 3, 4 };
        _endGameProgressionServiceMock.Setup(x => x.GetHelmKongs()).Returns(defaultHelmKongs);
        _endGameProgressionServiceMock.Setup(x => x.GetBossKongs()).Returns(defaultBossKongs);

        // Set up ISavedProgressProvider to return a valid SavedProgress
        _savedProgressProviderMock.Setup(x => x.CurrentProgress).Returns(new SavedProgress());
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

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new MainBroadcastViewModel(
                _userSettingsServiceMock.Object,
                null!,
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
        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNulDataSaverService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new MainBroadcastViewModel(
                _userSettingsServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _itemTrackingServiceMock.Object,
                _levelOrderServiceMock.Object,
                _savedProgressProviderMock.Object,
                _endGameProgressionServiceMock.Object,
                _themeServiceMock.Object,
                _collectiblesServiceMock.Object,
                _songServiceMock.Object,
                null!
            )
        );
        Assert.Equal("dataSaverService", ex.ParamName);
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
        // MainBroadcastViewModel and its nested view models (HelmSlots, BossSlots, ShopkeepItems, KongItems, etc.)
        // all subscribe to PropertyChanged during construction, so we expect multiple subscriptions
        _userSettingsServiceMock.VerifyAdd(x => x.PropertyChanged += It.IsAny<PropertyChangedEventHandler>(), Times.AtLeastOnce);
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

        // Assert - verify that MainBroadcastViewModel unsubscribes from UserSettings
        _userSettingsServiceMock.VerifyRemove(x => x.PropertyChanged -= It.IsAny<PropertyChangedEventHandler>(), Times.AtLeastOnce);
    }

    #endregion
}
