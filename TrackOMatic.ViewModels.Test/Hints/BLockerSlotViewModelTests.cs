namespace TrackOMatic.ViewModels.Test.Hints;

/// <summary>
/// Test coverage for <see cref="BLockerSlotViewModel"/> class.
/// Tests constructor validation (including region validation), event subscriptions, command execution with execute guards,
/// synchronization context handling, and state management.
/// </summary>
public class BLockerSlotViewModelTests
{
    private readonly Mock<IBarrierService> _barrierServiceMock;
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;
    private readonly Mock<IItemSelectorService> _itemSelectorServiceMock;

    public BLockerSlotViewModelTests()
    {
        _barrierServiceMock = new Mock<IBarrierService>();
        _parsedSpoilerDataServiceMock = new Mock<IParsedSpoilerDataService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
        _itemSelectorServiceMock = new Mock<IItemSelectorService>();

        // Setup default returns
        _barrierServiceMock.Setup(x => x.GetBLockerItemType(It.IsAny<RegionName>())).Returns(BarrierItems.GOLDEN_BANANA);
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);
    }

    private BLockerSlotViewModel CreateViewModel(RegionName regionName = RegionName.JUNGLE_JAPES)
    {
        return new BLockerSlotViewModel(
            regionName,
            _barrierServiceMock.Object,
            _parsedSpoilerDataServiceMock.Object,
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
    [InlineData(RegionName.JUNGLE_JAPES)]
    [InlineData(RegionName.ANGRY_AZTEC)]
    [InlineData(RegionName.FRANTIC_FACTORY)]
    [InlineData(RegionName.GLOOMY_GALLEON)]
    [InlineData(RegionName.FUNGI_FOREST)]
    [InlineData(RegionName.CRYSTAL_CAVES)]
    [InlineData(RegionName.CREEPY_CASTLE)]
    [InlineData(RegionName.HIDEOUT_HELM)]
    public void Constructor_WithValidLobbyRegion_Succeeds(RegionName validRegion)
    {
        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(validRegion);

            // Assert
            Assert.NotNull(viewModel);
        });
    }

    [Theory]
    [InlineData(RegionName.DK_ISLES)]
    [InlineData(RegionName.START)]
    [InlineData(RegionName.SHOPS)]
    [InlineData(RegionName.UNHINTABLE_MOVES)]
    public void Constructor_WithNonLobbyRegion_ThrowsArgumentOutOfRangeException(RegionName invalidRegion)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new BLockerSlotViewModel(
                    invalidRegion,
                    _barrierServiceMock.Object,
                    _parsedSpoilerDataServiceMock.Object,
                    _savedProgressProviderMock.Object,
                    _itemSelectorServiceMock.Object
                )
            )
        );
        Assert.Equal("regionName", ex.ParamName);
        Assert.Contains("valid lobby region", ex.Message);
    }

    [Fact]
    public void Constructor_WithNullBarrierService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new BLockerSlotViewModel(
                    RegionName.JUNGLE_JAPES,
                    null!,
                    _parsedSpoilerDataServiceMock.Object,
                    _savedProgressProviderMock.Object,
                    _itemSelectorServiceMock.Object
                )
            )
        );
        Assert.Equal("barrierService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new BLockerSlotViewModel(
                    RegionName.JUNGLE_JAPES,
                    _barrierServiceMock.Object,
                    null!,
                    _savedProgressProviderMock.Object,
                    _itemSelectorServiceMock.Object
                )
            )
        );
        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new BLockerSlotViewModel(
                    RegionName.JUNGLE_JAPES,
                    _barrierServiceMock.Object,
                    _parsedSpoilerDataServiceMock.Object,
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
                new BLockerSlotViewModel(
                    RegionName.JUNGLE_JAPES,
                    _barrierServiceMock.Object,
                    _parsedSpoilerDataServiceMock.Object,
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
    public void Constructor_InitializesBarrierValue_FromService_ForSpecificRegion()
    {
        // Arrange
        _barrierServiceMock.Setup(x => x.GetBLockerItemType(RegionName.ANGRY_AZTEC)).Returns(BarrierItems.BLUEPRINT);

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.ANGRY_AZTEC);

            // Assert
            Assert.Equal(1, viewModel.BarrierValue);
            _barrierServiceMock.Verify(x => x.GetBLockerItemType(RegionName.ANGRY_AZTEC), Times.Once);
        });
    }

    [Fact]
    public void Constructor_InitializesImageResourceKey_FromBarrierValue()
    {
        // Arrange
        _barrierServiceMock.Setup(x => x.GetBLockerItemType(RegionName.JUNGLE_JAPES)).Returns(BarrierItems.PEARL);

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);

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
        _barrierServiceMock.Setup(x => x.GetBLockerItemType(RegionName.JUNGLE_JAPES)).Returns(barrierItem);

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.Equal(expectedValue, viewModel.BarrierValue);
                Assert.Equal(expectedResourceKey, viewModel.ImageResourceKey);
            });
        });
    }

    #endregion

    #region Command Execution Tests

    [Fact]
    public void SelectItemCommand_WithExecuteGuardPassing_CanExecute()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Act & Assert
            Assert.True(viewModel.SelectItemCommand.CanExecute((100.0, 100.0)));
        });
    }

    [Fact]
    public void SelectItemCommand_WithExecuteGuardFailing_CannotExecute()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(true);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Act & Assert
            Assert.False(viewModel.SelectItemCommand.CanExecute((100.0, 100.0)));
        });
    }

    [Fact]
    public void SelectItemCommand_WithValidSelection_UpdatesBarrierService()
    {
        // Arrange
        var selectedCell = new ItemGridCell(0, 1, "blueprint");
        _itemSelectorServiceMock.Setup(x => x.SelectItemAsync(It.IsAny<IReadOnlyList<ItemGridCell>>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(selectedCell);
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);

            // Act
            viewModel.SelectItemCommand.Execute((100.0, 100.0));

            // Give the async operation time to complete
            Thread.Sleep(100);

            // Assert
            _barrierServiceMock.Verify(
                x => x.SetBLockerItemType(RegionName.JUNGLE_JAPES, BarrierItems.BLUEPRINT),
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
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);

            // Act
            viewModel.SelectItemCommand.Execute((100.0, 100.0));

            // Give the async operation time to complete
            Thread.Sleep(100);

            // Assert
            _barrierServiceMock.Verify(
                x => x.SetBLockerItemType(It.IsAny<RegionName>(), It.IsAny<BarrierItems>()),
                Times.Never
            );
        });
    }

    [Fact]
    public void CycleNextCommand_WithExecuteGuardPassing_CanExecute()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Act & Assert
            Assert.True(viewModel.CycleNextCommand.CanExecute(null));
        });
    }

    [Fact]
    public void CycleNextCommand_WithExecuteGuardFailing_CannotExecute()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(true);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Act & Assert
            Assert.False(viewModel.CycleNextCommand.CanExecute(null));
        });
    }

    [Theory]
    [InlineData(BarrierItems.GOLDEN_BANANA, BarrierItems.BLUEPRINT)]
    [InlineData(BarrierItems.BLUEPRINT, BarrierItems.PEARL)]
    [InlineData(BarrierItems.BEAN, BarrierItems.GOLDEN_BANANA)] // Wrap-around
    public void CycleNextCommand_AdvancesBarrier_And_UpdatesService(BarrierItems currentItem, BarrierItems expectedNextItem)
    {
        // Arrange
        _barrierServiceMock.Setup(x => x.GetBLockerItemType(RegionName.JUNGLE_JAPES)).Returns(currentItem);
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);
            _barrierServiceMock.Reset();
            _barrierServiceMock.Setup(x => x.GetBLockerItemType(RegionName.JUNGLE_JAPES)).Returns(currentItem);

            // Act
            viewModel.CycleNextCommand.Execute(null);

            // Assert
            _barrierServiceMock.Verify(
                x => x.SetBLockerItemType(RegionName.JUNGLE_JAPES, expectedNextItem),
                Times.Once
            );
        });
    }

    [Fact]
    public void CyclePreviousCommand_WithExecuteGuardPassing_CanExecute()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Act & Assert
            Assert.True(viewModel.CyclePreviousCommand.CanExecute(null));
        });
    }

    [Fact]
    public void CyclePreviousCommand_WithExecuteGuardFailing_CannotExecute()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(true);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Act & Assert
            Assert.False(viewModel.CyclePreviousCommand.CanExecute(null));
        });
    }

    [Theory]
    [InlineData(BarrierItems.BLUEPRINT, BarrierItems.GOLDEN_BANANA)]
    [InlineData(BarrierItems.PEARL, BarrierItems.BLUEPRINT)]
    [InlineData(BarrierItems.GOLDEN_BANANA, BarrierItems.BEAN)] // Wrap-around backward
    public void CyclePreviousCommand_DecreasesBarrier_And_UpdatesService(BarrierItems currentItem, BarrierItems expectedPreviousItem)
    {
        // Arrange
        _barrierServiceMock.Setup(x => x.GetBLockerItemType(RegionName.JUNGLE_JAPES)).Returns(currentItem);
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);
            _barrierServiceMock.Reset();
            _barrierServiceMock.Setup(x => x.GetBLockerItemType(RegionName.JUNGLE_JAPES)).Returns(currentItem);

            // Act
            viewModel.CyclePreviousCommand.Execute(null);

            // Assert
            _barrierServiceMock.Verify(
                x => x.SetBLockerItemType(RegionName.JUNGLE_JAPES, expectedPreviousItem),
                Times.Once
            );
        });
    }

    #endregion

    #region Event Subscription Tests

    [Fact]
    public void Constructor_SubscribesToBlockerBarrierTypeChangedEvent()
    {
        // Arrange
        EventHandler<BlockerBarrierTypeChangedEventArgs>? capturedHandler = null;
        _barrierServiceMock.SetupAdd(x => x.BlockerBarrierTypeChanged += It.IsAny<EventHandler<BlockerBarrierTypeChangedEventArgs>>())
            .Callback((EventHandler<BlockerBarrierTypeChangedEventArgs> handler) => capturedHandler = handler);

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);
            // Assert - event was subscribed to
            Assert.NotNull(capturedHandler);
        });
    }

    [Fact]
    public void Constructor_SubscribesToParsedSpoilerDataChangedEvent()
    {
        // Arrange
        EventHandler<ParsedSpoilerDataChangedEventArgs>? capturedHandler = null;
        _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
            .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => capturedHandler = handler);

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();
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
    public void OnBarrierChange_WithMatchingRegion_UpdatesBarrierValue()
    {
        // Arrange
        EventHandler<BlockerBarrierTypeChangedEventArgs> barrierChangeHandler = null!;
        _barrierServiceMock.SetupAdd(x => x.BlockerBarrierTypeChanged += It.IsAny<EventHandler<BlockerBarrierTypeChangedEventArgs>>())
            .Callback((EventHandler<BlockerBarrierTypeChangedEventArgs> handler) => barrierChangeHandler = handler);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);

            // Act
            var eventArgs = new BlockerBarrierTypeChangedEventArgs(RegionName.JUNGLE_JAPES, BarrierItems.GOLDEN_BANANA, BarrierItems.PEARL);
            barrierChangeHandler?.Invoke(_barrierServiceMock.Object, eventArgs);

            // Assert
            Assert.Equal(2, viewModel.BarrierValue);
            Assert.Equal("pearl", viewModel.ImageResourceKey);
        });
    }

    [Fact]
    public void OnBarrierChange_WithNonMatchingRegion_IgnoresEvent()
    {
        // Arrange
        EventHandler<BlockerBarrierTypeChangedEventArgs> barrierChangeHandler = null!;
        _barrierServiceMock.SetupAdd(x => x.BlockerBarrierTypeChanged += It.IsAny<EventHandler<BlockerBarrierTypeChangedEventArgs>>())
            .Callback((EventHandler<BlockerBarrierTypeChangedEventArgs> handler) => barrierChangeHandler = handler);
        _barrierServiceMock.Setup(x => x.GetBLockerItemType(RegionName.JUNGLE_JAPES)).Returns(BarrierItems.GOLDEN_BANANA);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);
            var initialValue = viewModel.BarrierValue;

            // Act
            var eventArgs = new BlockerBarrierTypeChangedEventArgs(RegionName.ANGRY_AZTEC, BarrierItems.GOLDEN_BANANA, BarrierItems.PEARL);
            barrierChangeHandler?.Invoke(_barrierServiceMock.Object, eventArgs);

            // Assert
            Assert.Equal(initialValue, viewModel.BarrierValue);
        });
    }

    [Fact]
    public void OnSpoilerDataChanged_WithNullData_UpdatesToGoldenBanana()
    {
        // Arrange
        EventHandler<ParsedSpoilerDataChangedEventArgs> spoilerChangeHandler = null!;
        _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
            .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => spoilerChangeHandler = handler);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);

            // Act - null data should default to GOLDEN_BANANA
            var eventArgs = new ParsedSpoilerDataChangedEventArgs(null, null);
            spoilerChangeHandler?.Invoke(_parsedSpoilerDataServiceMock.Object, eventArgs);

            // Assert
            Assert.Equal(0, viewModel.BarrierValue);
            Assert.Equal("golden_banana", viewModel.ImageResourceKey);
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
        _barrierServiceMock.Setup(x => x.GetBLockerItemType(RegionName.JUNGLE_JAPES))
            .Returns(BarrierItems.GOLDEN_BANANA);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);

            // Change the mock to return a different value
            _barrierServiceMock.Setup(x => x.GetBLockerItemType(RegionName.JUNGLE_JAPES)).Returns(BarrierItems.BLUEPRINT);

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
        EventHandler<BlockerBarrierTypeChangedEventArgs> barrierChangeHandler = null!;
        _barrierServiceMock.SetupAdd(x => x.BlockerBarrierTypeChanged += It.IsAny<EventHandler<BlockerBarrierTypeChangedEventArgs>>())
            .Callback((EventHandler<BlockerBarrierTypeChangedEventArgs> handler) => barrierChangeHandler = handler);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);

            // Simulate background thread calling the event handler
            var backgroundTask = Task.Run(() =>
            {
                var eventArgs = new BlockerBarrierTypeChangedEventArgs(RegionName.JUNGLE_JAPES, BarrierItems.GOLDEN_BANANA, BarrierItems.PEARL);
                barrierChangeHandler?.Invoke(_barrierServiceMock.Object, eventArgs);
            });

            backgroundTask.Wait();

            // Give the UI context post callback time to execute
            Thread.Sleep(50);

            // Assert
            Assert.Equal(2, viewModel.BarrierValue);
            Assert.Equal("pearl", viewModel.ImageResourceKey);
        });
    }

    [Fact]
    public void OnSpoilerDataChanged_FromBackgroundThread_MarshalesToUIContext()
    {
        // Arrange
        EventHandler<ParsedSpoilerDataChangedEventArgs> spoilerChangeHandler = null!;
        _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
            .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => spoilerChangeHandler = handler);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel(RegionName.JUNGLE_JAPES);

            // Simulate background thread calling the event handler with null data
            var backgroundTask = Task.Run(() =>
            {
                var eventArgs = new ParsedSpoilerDataChangedEventArgs(null, null);
                spoilerChangeHandler?.Invoke(_parsedSpoilerDataServiceMock.Object, eventArgs);
            });

            backgroundTask.Wait();

            // Assert
            Assert.Equal(0, viewModel.BarrierValue);
            Assert.Equal("golden_banana", viewModel.ImageResourceKey);
        });
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromBlockerBarrierTypeChangedEvent()
    {
        // Arrange
        EventHandler<BlockerBarrierTypeChangedEventArgs> capturedHandler = null!;
        _barrierServiceMock.SetupAdd(x => x.BlockerBarrierTypeChanged += It.IsAny<EventHandler<BlockerBarrierTypeChangedEventArgs>>())
            .Callback((EventHandler<BlockerBarrierTypeChangedEventArgs> handler) => capturedHandler = handler);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Act
            viewModel.Dispose();

            // Assert
            _barrierServiceMock.VerifyRemove(
                x => x.BlockerBarrierTypeChanged -= capturedHandler,
                Times.Once
            );
        });
    }

    [Fact]
    public void Dispose_UnsubscribesFromParsedSpoilerDataChangedEvent()
    {
        // Arrange
        EventHandler<ParsedSpoilerDataChangedEventArgs> capturedHandler = null!;
        _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
            .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => capturedHandler = handler);

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var viewModel = CreateViewModel();

            // Act
            viewModel.Dispose();

            // Assert
            _parsedSpoilerDataServiceMock.VerifyRemove(
                x => x.ParsedSpoilerDataChanged -= capturedHandler,
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
