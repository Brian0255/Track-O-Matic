namespace TrackOMatic.ViewModels.Test.Broadcast;

/// <summary>
/// Test coverage for KongBlueprintViewModel.
/// Tests constructor validation, event subscriptions, and state initialization.
/// </summary>
public class KongBlueprintViewModelTests
{
    private readonly Mock<ICollectiblesService> _collectiblesServiceMock;
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;

    public KongBlueprintViewModelTests()
    {
        _collectiblesServiceMock = new Mock<ICollectiblesService>();
        _parsedSpoilerDataServiceMock = new Mock<IParsedSpoilerDataService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
    }

    private KongBlueprintViewModel CreateViewModel(
        ItemType collectedType = ItemType.DONKEY_BLUEPRINT,
        ItemType turnedInType = ItemType.DONKEY_BLUEPRINT_TURNED)
    {
        return new KongBlueprintViewModel(
            collectedType,
            turnedInType,
            _collectiblesServiceMock.Object,
            _parsedSpoilerDataServiceMock.Object,
            _savedProgressProviderMock.Object
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

    [Theory]
    [InlineData(ItemType.GOLDEN_BANANA)]
    public void Constructor_WithDisallowedCollectedType_ThrowsArgumentException(ItemType disallowedType)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            CreateViewModel(disallowedType, ItemType.DONKEY_BLUEPRINT_TURNED)
        );
        Assert.Equal("collectedType", ex.ParamName);
    }

    [Theory]
    [InlineData(ItemType.GOLDEN_BANANA)]
    public void Constructor_WithDisallowedTurnedInType_ThrowsArgumentException(ItemType disallowedType)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            CreateViewModel(ItemType.DONKEY_BLUEPRINT, disallowedType)
        );
        Assert.Equal("turnedInType", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullCollectiblesService_ThrowsArgumentNullException(ICollectiblesService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new KongBlueprintViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED,
                service!,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object
            )
        );
        Assert.Equal("collectiblesService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException(IParsedSpoilerDataService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new KongBlueprintViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED,
                _collectiblesServiceMock.Object,
                service!,
                _savedProgressProviderMock.Object
            )
        );
        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException(ISavedProgressProvider? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new KongBlueprintViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED,
                _collectiblesServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                service!
            )
        );
        Assert.Equal("savedProgressProvider", ex.ParamName);
    }

    #endregion

    #region Event Subscription Tests

    [Fact]
    public void Constructor_SubscribesToCollectiblesServiceCollectibleChanged()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _collectiblesServiceMock.VerifyAdd(x => x.CollectibleChanged += It.IsAny<EventHandler<CollectiblesChangedEventArgs>>(), Times.Once);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromAllEvents()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.Dispose();

        // Assert
        _collectiblesServiceMock.VerifyRemove(x => x.CollectibleChanged -= It.IsAny<EventHandler<CollectiblesChangedEventArgs>>(), Times.Once);
    }

    #endregion
}
