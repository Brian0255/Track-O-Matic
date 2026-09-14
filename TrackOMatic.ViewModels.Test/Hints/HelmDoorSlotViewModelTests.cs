namespace TrackOMatic.ViewModels.Test.Hints;

/// <summary>
/// Test coverage for <see cref="HelmDoorSlotViewModel"/> class.
/// Tests constructor validation, event subscriptions, command execution, synchronization context handling, and state management.
/// </summary>
public class HelmDoorSlotViewModelTests
{
    private readonly Mock<IBarrierService> _barrierServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;
    private readonly Mock<IItemSelectorService> _itemSelectorServiceMock;

    public HelmDoorSlotViewModelTests()
    {
        _barrierServiceMock = new Mock<IBarrierService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
        _itemSelectorServiceMock = new Mock<IItemSelectorService>();

        // Setup default returns
        _barrierServiceMock.Setup(x => x.GetHelmDoorItemType(It.IsAny<HelmDoor>())).Returns(BarrierItems.GOLDEN_BANANA);
    }

    private HelmDoorSlotViewModel CreateViewModel(HelmDoor helmDoor = HelmDoor.First)
    {
        return new HelmDoorSlotViewModel(
            helmDoor,
            _barrierServiceMock.Object,
            _savedProgressProviderMock.Object,
            _itemSelectorServiceMock.Object
        );
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidDependencies_Succeeds()
    {
        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Assert
            Assert.NotNull(viewModel);
        });
    }

    [Theory]
    [InlineData(HelmDoor.First)]
    [InlineData(HelmDoor.Second)]
    public void Constructor_WithAllowedHelmDoors_Succeeds(HelmDoor helmDoor)
    {
        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(helmDoor);

            // Assert
            Assert.NotNull(viewModel);
        });
    }

    [Fact]
    public void Constructor_WithNullBarrierService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new HelmDoorSlotViewModel(
                    HelmDoor.First,
                    null!,
                    _savedProgressProviderMock.Object,
                    _itemSelectorServiceMock.Object
                )
            )
        );
        Assert.Equal("barrierService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new HelmDoorSlotViewModel(
                    HelmDoor.First,
                    _barrierServiceMock.Object,
                    null!,
                    _itemSelectorServiceMock.Object
                )
            )
        );
        Assert.Equal("savedProgressProvider", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullItemSelectorService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new HelmDoorSlotViewModel(
                    HelmDoor.First,
                    _barrierServiceMock.Object,
                    _savedProgressProviderMock.Object,
                    null!
                )
            )
        );
        Assert.Equal("itemSelectorService", ex.ParamName);
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void Constructor_InitializesBarrierValue_FromService()
    {
        // Arrange
        _barrierServiceMock.Setup(x => x.GetHelmDoorItemType(HelmDoor.First)).Returns(BarrierItems.BLUEPRINT);

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);

            // Assert
            Assert.Equal(1, viewModel.BarrierValue);
            _barrierServiceMock.Verify(x => x.GetHelmDoorItemType(HelmDoor.First), Times.Once);
        });
    }

    [Fact]
    public void Constructor_InitializesImageResourceKey_FromBarrierValue()
    {
        // Arrange
        _barrierServiceMock.Setup(x => x.GetHelmDoorItemType(HelmDoor.First)).Returns(BarrierItems.PEARL);

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);

            // Assert
            Assert.Equal("pearl", viewModel.ImageResourceKey);
        });
    }

    [Theory]
    [InlineData(BarrierItems.GOLDEN_BANANA, 0, "golden_banana")]
    [InlineData(BarrierItems.BLUEPRINT, 1, "blueprint")]
    [InlineData(BarrierItems.PEARL, 2, "pearl")]
    [InlineData(BarrierItems.CROWN, 3, "crown")]
    [InlineData(BarrierItems.MEDAL, 4, "medal")]
    [InlineData(BarrierItems.RAINBOW_COIN, 5, "rainbow_coin")]
    [InlineData(BarrierItems.FAIRY, 6, "fairy")]
    [InlineData(BarrierItems.COMPANY_COIN, 7, "company_coin")]
    [InlineData(BarrierItems.BEAN, 8, "bean")]
    public void InitializeState_MapsBarrierItemToCorrectValue_And_ResourceKey(
        BarrierItems barrierItem, int expectedValue, string expectedResourceKey)
    {
        // Arrange
        _barrierServiceMock.Setup(x => x.GetHelmDoorItemType(HelmDoor.First)).Returns(barrierItem);

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.Equal(expectedValue, viewModel.BarrierValue);
                Assert.Equal(expectedResourceKey, viewModel.ImageResourceKey);
            });
        });
    }

    #endregion

    #region Command Tests

    [Fact]
    public void SelectItemCommand_WithValidSelection_UpdatesBarrierService()
    {
        // Arrange
        var selectedCell = new ItemGridCell(0, 1, "blueprint");
        _itemSelectorServiceMock.Setup(x => x.SelectItemAsync(It.IsAny<IReadOnlyList<ItemGridCell>>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(selectedCell);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);

            // Act
            viewModel.SelectItemCommand.Execute((100.0, 100.0));

            // Give the async operation time to complete
            Thread.Sleep(100);

            // Assert
            _barrierServiceMock.Verify(
                x => x.SetHelmDoorItemType(HelmDoor.First, BarrierItems.BLUEPRINT),
                Times.Once
            );
        });
    }

    [Fact]
    public void SelectItemCommand_WithNullSelection_DoesNotUpdateService()
    {
        // Arrange
        _itemSelectorServiceMock.Setup(x => x.SelectItemAsync(It.IsAny<IReadOnlyList<ItemGridCell>>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((ItemGridCell?)null);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);

            // Act
            viewModel.SelectItemCommand.Execute((100.0, 100.0));

            // Give the async operation time to complete
            Thread.Sleep(100);

            // Assert
            _barrierServiceMock.Verify(
                x => x.SetHelmDoorItemType(It.IsAny<HelmDoor>(), It.IsAny<BarrierItems>()),
                Times.Never
            );
        });
    }

    [Theory]
    [InlineData(BarrierItems.GOLDEN_BANANA, BarrierItems.BLUEPRINT)]
    [InlineData(BarrierItems.BLUEPRINT, BarrierItems.PEARL)]
    [InlineData(BarrierItems.BEAN, BarrierItems.GOLDEN_BANANA)] // Wrap-around
    public void CycleNextCommand_AdvancesBarrier_And_UpdatesService(BarrierItems currentItem, BarrierItems expectedNextItem)
    {
        // Arrange
        _barrierServiceMock.Setup(x => x.GetHelmDoorItemType(HelmDoor.First)).Returns(currentItem);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);
            _barrierServiceMock.Reset();
            _barrierServiceMock.Setup(x => x.GetHelmDoorItemType(HelmDoor.First)).Returns(currentItem);

            // Act
            viewModel.CycleNextCommand.Execute(null);

            // Assert
            _barrierServiceMock.Verify(
                x => x.SetHelmDoorItemType(HelmDoor.First, expectedNextItem),
                Times.Once
            );
        });
    }

    [Theory]
    [InlineData(BarrierItems.BLUEPRINT, BarrierItems.GOLDEN_BANANA)]
    [InlineData(BarrierItems.PEARL, BarrierItems.BLUEPRINT)]
    [InlineData(BarrierItems.GOLDEN_BANANA, BarrierItems.BEAN)] // Wrap-around backward
    public void CyclePreviousCommand_DecreasesBarrier_And_UpdatesService(BarrierItems currentItem, BarrierItems expectedPreviousItem)
    {
        // Arrange
        _barrierServiceMock.Setup(x => x.GetHelmDoorItemType(HelmDoor.First)).Returns(currentItem);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);
            _barrierServiceMock.Reset();
            _barrierServiceMock.Setup(x => x.GetHelmDoorItemType(HelmDoor.First)).Returns(currentItem);

            // Act
            viewModel.CyclePreviousCommand.Execute(null);

            // Assert
            _barrierServiceMock.Verify(
                x => x.SetHelmDoorItemType(HelmDoor.First, expectedPreviousItem),
                Times.Once
            );
        });
    }

    #endregion

    #region Event Subscription Tests

    [Fact]
    public void Constructor_SubscribesToHelmBarrierTypeChangedEvent()
    {
        // Arrange
        EventHandler<HelmBarrierTypeChangedEventArgs>? capturedHandler = null;
        _barrierServiceMock.SetupAdd(x => x.HelmBarrierTypeChanged += It.IsAny<EventHandler<HelmBarrierTypeChangedEventArgs>>())
            .Callback((EventHandler<HelmBarrierTypeChangedEventArgs> handler) => capturedHandler = handler);

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);
            // Assert - event was subscribed to
            Assert.NotNull(capturedHandler);
        });
    }

    [Fact]
    public void Constructor_SubscribesToProgressChangedEvent()
    {
        // Arrange
        EventHandler<ProgressReplacedEventArgs>? capturedHandler = null;
        _savedProgressProviderMock.SetupAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>())
            .Callback((EventHandler<ProgressReplacedEventArgs> handler) => capturedHandler = handler);

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();
            // Assert - event was subscribed to
            Assert.NotNull(capturedHandler);
        });
    }

    [Fact]
    public void OnBarrierChange_WithMatchingHelmDoor_UpdatesBarrierValue()
    {
        // Arrange
        EventHandler<HelmBarrierTypeChangedEventArgs> barrierChangeHandler = null!;
        _barrierServiceMock.SetupAdd(x => x.HelmBarrierTypeChanged += It.IsAny<EventHandler<HelmBarrierTypeChangedEventArgs>>())
            .Callback((EventHandler<HelmBarrierTypeChangedEventArgs> handler) => barrierChangeHandler = handler);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);

            // Act
            var eventArgs = new HelmBarrierTypeChangedEventArgs(HelmDoor.First, BarrierItems.GOLDEN_BANANA, BarrierItems.PEARL);
            barrierChangeHandler?.Invoke(_barrierServiceMock.Object, eventArgs);

            // Assert
            Assert.Equal(2, viewModel.BarrierValue);
            Assert.Equal("pearl", viewModel.ImageResourceKey);
        });
    }

    [Fact]
    public void OnBarrierChange_WithNonMatchingHelmDoor_IgnoresEvent()
    {
        // Arrange
        EventHandler<HelmBarrierTypeChangedEventArgs> barrierChangeHandler = null!;
        _barrierServiceMock.SetupAdd(x => x.HelmBarrierTypeChanged += It.IsAny<EventHandler<HelmBarrierTypeChangedEventArgs>>())
            .Callback((EventHandler<HelmBarrierTypeChangedEventArgs> handler) => barrierChangeHandler = handler);
        _barrierServiceMock.Setup(x => x.GetHelmDoorItemType(HelmDoor.First)).Returns(BarrierItems.GOLDEN_BANANA);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);
            var initialValue = viewModel.BarrierValue;

            // Act
            var eventArgs = new HelmBarrierTypeChangedEventArgs(HelmDoor.Second, BarrierItems.GOLDEN_BANANA, BarrierItems.PEARL);
            barrierChangeHandler?.Invoke(_barrierServiceMock.Object, eventArgs);

            // Assert
            Assert.Equal(initialValue, viewModel.BarrierValue);
        });
    }

    [Fact]
    public void OnDataReset_ReinitializesState()
    {
        // Arrange
        EventHandler<ProgressReplacedEventArgs> progressChangeHandler = null!;
        var newProgress = new SavedProgress { SavedGBCounts = [], BLockerImageIndexes = [], HelmDoorCounts = ["?", "?"], HelmDoorImageIndexes = [0, 0] };
        _savedProgressProviderMock.SetupAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>())
            .Callback((EventHandler<ProgressReplacedEventArgs> handler) => progressChangeHandler = handler);
        _barrierServiceMock.Setup(x => x.GetHelmDoorItemType(HelmDoor.First))
            .Returns(BarrierItems.GOLDEN_BANANA);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);

            // Change the mock to return a different value
            _barrierServiceMock.Setup(x => x.GetHelmDoorItemType(HelmDoor.First)).Returns(BarrierItems.BLUEPRINT);

            // Act
            var eventArgs = new ProgressReplacedEventArgs { NewProgress = newProgress };
            progressChangeHandler?.Invoke(_savedProgressProviderMock.Object, eventArgs);

            // Assert
            Assert.Equal(1, viewModel.BarrierValue);
            Assert.Equal("blueprint", viewModel.ImageResourceKey);
        });
    }

    #endregion

    #region Cross-Thread Marshaling Tests

    [Fact]
    public void OnBarrierChange_FromBackgroundThread_MarshalesToUIContext()
    {
        // Arrange
        EventHandler<HelmBarrierTypeChangedEventArgs> barrierChangeHandler = null!;
        _barrierServiceMock.SetupAdd(x => x.HelmBarrierTypeChanged += It.IsAny<EventHandler<HelmBarrierTypeChangedEventArgs>>())
            .Callback((EventHandler<HelmBarrierTypeChangedEventArgs> handler) => barrierChangeHandler = handler);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(HelmDoor.First);

            // Simulate background thread calling the event handler
            var backgroundTask = Task.Run(() =>
            {
                var eventArgs = new HelmBarrierTypeChangedEventArgs(HelmDoor.First, BarrierItems.GOLDEN_BANANA, BarrierItems.PEARL);
                barrierChangeHandler?.Invoke(_barrierServiceMock.Object, eventArgs);
            });

            backgroundTask.Wait();

            // Assert
            Assert.Equal(2, viewModel.BarrierValue);
            Assert.Equal("pearl", viewModel.ImageResourceKey);
        });
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromHelmBarrierTypeChangedEvent()
    {
        // Arrange
        EventHandler<HelmBarrierTypeChangedEventArgs> capturedHandler = null!;
        _barrierServiceMock.SetupAdd(x => x.HelmBarrierTypeChanged += It.IsAny<EventHandler<HelmBarrierTypeChangedEventArgs>>())
            .Callback((EventHandler<HelmBarrierTypeChangedEventArgs> handler) => capturedHandler = handler);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Act
            viewModel.Dispose();

            // Assert
            _barrierServiceMock.VerifyRemove(
                x => x.HelmBarrierTypeChanged -= capturedHandler,
                Times.Once
            );
        });
    }

    [Fact]
    public void Dispose_UnsubscribesFromProgressChangedEvent()
    {
        // Arrange
        EventHandler<ProgressReplacedEventArgs> capturedHandler = null!;
        _savedProgressProviderMock.SetupAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>())
            .Callback((EventHandler<ProgressReplacedEventArgs> handler) => capturedHandler = handler);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Act
            viewModel.Dispose();

            // Assert
            _savedProgressProviderMock.VerifyRemove(
                x => x.ProgressChanged -= capturedHandler,
                Times.Once
            );
        });
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        // Arrange
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Act & Assert
            viewModel.Dispose();
            viewModel.Dispose(); // Should not throw
            Assert.True(true); // Basic assertion to ensure no exception
        });
    }

    #endregion
}
