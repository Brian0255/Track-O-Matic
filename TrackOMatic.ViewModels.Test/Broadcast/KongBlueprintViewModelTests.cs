namespace TrackOMatic.ViewModels.Test.Broadcast;

using TrackOMatic.ViewModels.Test.Helpers;

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

    #region Equals Tests

    [Fact]
    public void Equals_WithSameInstance_ReturnsTrue()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            var result = viewModel.Equals(viewModel);

            // Assert
            Assert.True(result);
        });
    }

    [Fact]
    public void Equals_WithSameCollectedAndTurnedInTypes_ReturnsTrue()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            var viewModel1 = CreateViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED);
            var viewModel2 = CreateViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED);

            // Act
            var result = viewModel1.Equals(viewModel2);

            // Assert
            Assert.True(result);
        });
    }

    [Fact]
    public void Equals_WithDifferentCollectedType_ReturnsFalse()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            var viewModel1 = CreateViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED);
            var viewModel2 = CreateViewModel(
                ItemType.DIDDY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED);

            // Act
            var result = viewModel1.Equals(viewModel2);

            // Assert
            Assert.False(result);
        });
    }

    [Fact]
    public void Equals_WithDifferentTurnedInType_ReturnsFalse()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            var viewModel1 = CreateViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED);
            var viewModel2 = CreateViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DIDDY_BLUEPRINT_TURNED);

            // Act
            var result = viewModel1.Equals(viewModel2);

            // Assert
            Assert.False(result);
        });
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            var result = viewModel.Equals(null);

            // Assert
            Assert.False(result);
        });
    }

    [Fact]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            var viewModel = CreateViewModel();
            var otherObject = "not a KongBlueprintViewModel";

            // Act
            var result = viewModel.Equals(otherObject);

            // Assert
            Assert.False(result);
        });
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    public void GetHashCode_WithSameTypes_ReturnsSame()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            var viewModel1 = CreateViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED);
            var viewModel2 = CreateViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED);

            // Act
            var hash1 = viewModel1.GetHashCode();
            var hash2 = viewModel2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        });
    }

    [Fact]
    public void GetHashCode_WithDifferentCollectedTypes_ReturnsDifferent()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            var viewModel1 = CreateViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED);
            var viewModel2 = CreateViewModel(
                ItemType.DIDDY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED);

            // Act
            var hash1 = viewModel1.GetHashCode();
            var hash2 = viewModel2.GetHashCode();

            // Assert
            Assert.NotEqual(hash1, hash2);
        });
    }

    [Fact]
    public void GetHashCode_WithDifferentTurnedInTypes_ReturnsDifferent()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            var viewModel1 = CreateViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DONKEY_BLUEPRINT_TURNED);
            var viewModel2 = CreateViewModel(
                ItemType.DONKEY_BLUEPRINT,
                ItemType.DIDDY_BLUEPRINT_TURNED);

            // Act
            var hash1 = viewModel1.GetHashCode();
            var hash2 = viewModel2.GetHashCode();

            // Assert
            Assert.NotEqual(hash1, hash2);
        });
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithZeroCount_IncludesCountAndCollectedType()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT)).Returns(0);
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT_TURNED)).Returns(0);
            var viewModel = CreateViewModel();

            // Act
            var result = viewModel.ToString();

            // Assert
            Assert.Contains("0", result);
            Assert.Contains("DONKEY_BLUEPRINT", result);
        });
    }

    [Fact]
    public void ToString_WithPositiveCount_IncludesCountAndCollectedType()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DIDDY_BLUEPRINT)).Returns(7);
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DIDDY_BLUEPRINT_TURNED)).Returns(2);
            var viewModel = CreateViewModel(
                ItemType.DIDDY_BLUEPRINT,
                ItemType.DIDDY_BLUEPRINT_TURNED);

            // Act
            var result = viewModel.ToString();

            // Assert
            Assert.Contains("5", result); // 7 - 2
            Assert.Contains("DIDDY_BLUEPRINT", result);
        });
    }

    [Fact]
    public void ToString_ReturnsNonEmptyString()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);
            var viewModel = CreateViewModel();

            // Act
            var result = viewModel.ToString();

            // Assert
            Assert.False(string.IsNullOrEmpty(result));
            Assert.NotEmpty(result);
        });
    }

    [Fact]
    public void ToString_ContainsKongBlueprintViewModelClassName()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(It.IsAny<ItemType>())).Returns(0);
            var viewModel = CreateViewModel();

            // Act
            var result = viewModel.ToString();

            // Assert
            Assert.Contains("KongBlueprintViewModel", result);
        });
    }

    #endregion

    #region Context-Aware Event Tests

    [Fact]
    public void OnCollectibleChanged_WhenCalledOnUIContext_UpdatesStateDirectly()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT)).Returns(5);
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT_TURNED)).Returns(2);
            var viewModel = CreateViewModel();
            Assert.Equal(3, viewModel.Count); // 5 - 2 = 3

            // Re-setup for the event
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT)).Returns(6);
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT_TURNED)).Returns(2);

            // Act
            _collectiblesServiceMock.Raise(x => x.CollectibleChanged += null,
                new CollectiblesChangedEventArgs(ItemType.DONKEY_BLUEPRINT, 1, 6));

            // Assert
            Assert.Equal(4, viewModel.Count); // 6 - 2 = 4
        });
    }

    [Fact]
    public void OnCollectibleChanged_WhenCalledFromBackgroundThread_QueuesCallback()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT)).Returns(5);
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT_TURNED)).Returns(2);
            var viewModel = CreateViewModel();
            Assert.Equal(3, viewModel.Count); // 5 - 2 = 3

            // Re-setup for the event
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT)).Returns(7);
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT_TURNED)).Returns(2);

            // Simulate raising event from background thread by clearing the context temporarily
            var savedContext = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);

                // Act
                _collectiblesServiceMock.Raise(x => x.CollectibleChanged += null,
                    new CollectiblesChangedEventArgs(ItemType.DONKEY_BLUEPRINT, 1, 7));

                // Assert: The state should not immediately update (posted to context)
                // So it's still at old value 3
                Assert.Equal(3, viewModel.Count);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(savedContext);
            }
        });
    }

    [Fact]
    public void OnCollectibleChanged_WithIrrelevantItemType_IgnoresEvent()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT)).Returns(5);
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT_TURNED)).Returns(2);
            var viewModel = CreateViewModel();
            var initialCount = viewModel.Count;

            // Act: Raise event with unrelated item type
            _collectiblesServiceMock.Raise(x => x.CollectibleChanged += null,
                new CollectiblesChangedEventArgs(ItemType.GOLDEN_BANANA, 1, 10));

            // Assert: Count should remain unchanged
            Assert.Equal(initialCount, viewModel.Count);
        });
    }

    [Fact]
    public void OnCollectibleChanged_WithTurnedInType_UpdatesStateCorrectly()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            // Arrange
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT)).Returns(5);
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT_TURNED)).Returns(2);
            var viewModel = CreateViewModel();
            Assert.Equal(3, viewModel.Count); // 5 - 2 = 3

            // Re-setup: turned-in count increases
            _collectiblesServiceMock.Setup(x => x.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT_TURNED)).Returns(3);

            // Act
            _collectiblesServiceMock.Raise(x => x.CollectibleChanged += null,
                new CollectiblesChangedEventArgs(ItemType.DONKEY_BLUEPRINT_TURNED, 1, 3));

            // Assert
            Assert.Equal(2, viewModel.Count); // 5 - 3 = 2
        });
    }

    #endregion
}
