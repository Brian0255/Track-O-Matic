namespace TrackOMatic.ViewModels.Test.Hints;

public class BLockerHintViewModelTests
{
    private readonly Mock<IBarrierService> _barrierServiceMock;
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;
    private readonly Mock<IItemSelectorService> _itemSelectorServiceMock;
    private readonly Mock<IDataSaverService> _dataSaverServiceMock;

    public BLockerHintViewModelTests()
    {
        _barrierServiceMock = new Mock<IBarrierService>();
        _parsedSpoilerDataServiceMock = new Mock<IParsedSpoilerDataService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
        _itemSelectorServiceMock = new Mock<IItemSelectorService>();
        _dataSaverServiceMock = new Mock<IDataSaverService>();

        // Default mock setup
        _barrierServiceMock.Setup(x => x.GetBLockerCount(It.IsAny<RegionName>())).Returns("0");
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);
    }

    private BLockerHintViewModel CreateViewModel(RegionName regionName)
    {
        return SynchronizationContextTestHelper.RunWithUIContext(() =>
            new BLockerHintViewModel(
                regionName,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            )
        );
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullBarrierService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new BLockerHintViewModel(
                    RegionName.JUNGLE_JAPES,
                    null!,
                    _parsedSpoilerDataServiceMock.Object,
                    _savedProgressProviderMock.Object,
                    _itemSelectorServiceMock.Object,
                    _dataSaverServiceMock.Object
                )
            )
        );
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new BLockerHintViewModel(
                    RegionName.JUNGLE_JAPES,
                    _barrierServiceMock.Object,
                    null!,
                    _savedProgressProviderMock.Object,
                    _itemSelectorServiceMock.Object,
                    _dataSaverServiceMock.Object
                )
            )
        );
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new BLockerHintViewModel(
                    RegionName.JUNGLE_JAPES,
                    _barrierServiceMock.Object,
                    _parsedSpoilerDataServiceMock.Object,
                    null!,
                    _itemSelectorServiceMock.Object,
                    _dataSaverServiceMock.Object
                )
            )
        );
    }

    [Fact]
    public void Constructor_WithNullItemSelectorService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new BLockerHintViewModel(
                    RegionName.JUNGLE_JAPES,
                    _barrierServiceMock.Object,
                    _parsedSpoilerDataServiceMock.Object,
                    _savedProgressProviderMock.Object,
                    null!,
                    _dataSaverServiceMock.Object
                )
            )
        );
    }

    [Fact]
    public void Constructor_WithNullDataSaverService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new BLockerHintViewModel(
                    RegionName.JUNGLE_JAPES,
                    _barrierServiceMock.Object,
                    _parsedSpoilerDataServiceMock.Object,
                    _savedProgressProviderMock.Object,
                    _itemSelectorServiceMock.Object,
                    null!
                )
            )
        );
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
        var vm = CreateViewModel(validRegion);
        Assert.NotNull(vm);
        Assert.Equal(validRegion.ToString().ToLowerInvariant(), vm.RegionResourceKey);
    }

    [Theory]
    [InlineData(RegionName.DK_ISLES)]
    [InlineData(RegionName.START)]
    [InlineData(RegionName.SHOPS)]
    [InlineData(RegionName.UNHINTABLE_MOVES)]
    public void Constructor_WithNonLobbyRegion_ThrowsArgumentException(RegionName invalidRegion)
    {
        Assert.Throws<ArgumentException>(() => CreateViewModel(invalidRegion));
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void Constructor_InitializesBarrierCount_FromService()
    {
        _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("5");

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        Assert.Equal("5", vm.BarrierCount);
    }

    [Fact]
    public void Constructor_InitializesRegionResourceKey()
    {
        var vm = CreateViewModel(RegionName.ANGRY_AZTEC);

        Assert.Equal("angry_aztec", vm.RegionResourceKey);
    }

    #endregion

    #region Command Execute Guard Tests

    [Fact]
    public void ClearPlaceholderCommand_WithGuardPassing_CanExecute()
    {
        _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("?");
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        Assert.True(vm.ClearPlaceholderCommand.CanExecute(null));
    }

    [Fact]
    public void ClearPlaceholderCommand_WithGuardFailing_CannotExecute()
    {
        _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("5");
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        Assert.False(vm.ClearPlaceholderCommand.CanExecute(null));
    }

    [Fact]
    public void SetBarrierCountCommand_WithGuardPassing_CanExecute()
    {
        _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("5");
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(false);

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        Assert.True(vm.SetBarrierCountCommand.CanExecute(null));
    }

    [Fact]
    public void SetBarrierCountCommand_WithGuardFailing_CannotExecute()
    {
        _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("?");
        _parsedSpoilerDataServiceMock.Setup(x => x.HasRegionBarrier()).Returns(true);

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        Assert.False(vm.SetBarrierCountCommand.CanExecute(null));
    }

    #endregion

    #region Command Execution Tests

    [Fact]
    public void ClearPlaceholderCommand_ClearsBarrierCount()
    {
        _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("?");

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);
        vm.ClearPlaceholderCommand.Execute(null);

        Assert.Equal("", vm.BarrierCount);
    }

    [Fact]
    public void SetBarrierCountCommand_CallsService_WithCurrentBarrierCount()
    {
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);
        vm.BarrierCount = "5";

        vm.SetBarrierCountCommand.Execute(null);

        _barrierServiceMock.Verify(x => x.SetBLockerCount(RegionName.JUNGLE_JAPES, "5"), Times.Once);
    }

    #endregion

    #region Slot View Model Lazy Loading Tests

    [Fact]
    public void SlotViewModel_CreatesInstance_WhenAccessed()
    {
        _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("0");

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);
        var slot = vm.SlotViewModel;

        Assert.NotNull(slot);
    }

    [Fact]
    public void SlotViewModel_ReusesInstance_WhenAccessedMultipleTimes()
    {
        _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("0");

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);
        var slot1 = vm.SlotViewModel;
        var slot2 = vm.SlotViewModel;

        Assert.Same(slot1, slot2);
    }

    #endregion

    #region Event Subscription Tests

    [Fact]
    public void Constructor_SubscribesToBlockerBarrierAmountChangedEvent()
    {
        _barrierServiceMock.SetupAdd(x => x.BlockerBarrierAmountChanged += It.IsAny<EventHandler<BlockerBarrierAmountChangedEventArgs>>())
            .Verifiable();

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        _barrierServiceMock.Verify();
    }

    [Fact]
    public void Constructor_SubscribesToParsedSpoilerDataChangedEvent()
    {
        _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
            .Verifiable();

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        _parsedSpoilerDataServiceMock.Verify();
    }

    [Fact]
    public void Constructor_SubscribesToProgressChangedEvent()
    {
        _savedProgressProviderMock.SetupAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>())
            .Verifiable();

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        _savedProgressProviderMock.Verify();
    }

    [Fact]
    public void Constructor_SubscribesToDataLoadingCompletedEvent()
    {
        _dataSaverServiceMock.SetupAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>())
            .Verifiable();

        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        _dataSaverServiceMock.Verify();
    }

    #endregion

    #region Barrier Amount Changed Event Tests

    [Fact]
    public void OnBarrierAmountChanged_WithMatchingRegion_UpdatesBarrierCount()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<BlockerBarrierAmountChangedEventArgs>? barrierChangeHandler = null;
            _barrierServiceMock.SetupAdd(x => x.BlockerBarrierAmountChanged += It.IsAny<EventHandler<BlockerBarrierAmountChangedEventArgs>>())
                .Callback((EventHandler<BlockerBarrierAmountChangedEventArgs> handler) => barrierChangeHandler = handler);

            var vm = new BLockerHintViewModel(
                RegionName.JUNGLE_JAPES,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            );

            // Verify the handler was captured during construction
            Assert.NotNull(barrierChangeHandler);

            barrierChangeHandler!.Invoke(_barrierServiceMock.Object,
                new BlockerBarrierAmountChangedEventArgs(RegionName.JUNGLE_JAPES, "0", "5"));

            Thread.Sleep(50); // Allow UI context post to execute

            Assert.Equal("5", vm.BarrierCount);
        });
    }

    [Fact]
    public void OnBarrierAmountChanged_WithNonMatchingRegion_IgnoresEvent()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<BlockerBarrierAmountChangedEventArgs>? barrierChangeHandler = null;
            _barrierServiceMock.SetupAdd(x => x.BlockerBarrierAmountChanged += It.IsAny<EventHandler<BlockerBarrierAmountChangedEventArgs>>())
                .Callback((EventHandler<BlockerBarrierAmountChangedEventArgs> handler) => barrierChangeHandler = handler);

            _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("0");
            var vm = new BLockerHintViewModel(
                RegionName.JUNGLE_JAPES,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            );

            barrierChangeHandler?.Invoke(_barrierServiceMock.Object,
                new BlockerBarrierAmountChangedEventArgs(RegionName.ANGRY_AZTEC, "0", "5"));

            Assert.Equal("0", vm.BarrierCount);
        });
    }

    #endregion

    #region Spoiler Data Changed Event Tests

    [Fact]
    public void OnSpoilerDataChanged_WithNullData_IgnoresEvent()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<ParsedSpoilerDataChangedEventArgs>? spoilerChangeHandler = null;
            _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
                .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => spoilerChangeHandler = handler);

            var vm = new BLockerHintViewModel(
                RegionName.JUNGLE_JAPES,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            );

            spoilerChangeHandler?.Invoke(_parsedSpoilerDataServiceMock.Object,
                new ParsedSpoilerDataChangedEventArgs(null, null));

            // Should not crash, BarrierCount unchanged
            Assert.Equal("0", vm.BarrierCount);
        });
    }

    [Fact]
    public void OnSpoilerDataChanged_WithValidCost_UpdatesBarrierCount()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<ParsedSpoilerDataChangedEventArgs>? spoilerChangeHandler = null;
            _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
                .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => spoilerChangeHandler = handler);

            var vm = new BLockerHintViewModel(
                RegionName.JUNGLE_JAPES,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            );

            // Create spoiler data with valid barrier info
            var mockSpoilerData = new ParsedSpoilerData
            {
                LevelOrder = [],
                RegionBarrierInfo =
                [
                    new BlockerInfoRaw { Item = 1, Cost = 5 },  // JUNGLE_JAPES
                    new BlockerInfoRaw { Item = 2, Cost = 4 },  // ANGRY_AZTEC
                    new BlockerInfoRaw { Item = 3, Cost = 3 },  // FRANTIC_FACTORY
                ]
            };

            spoilerChangeHandler?.Invoke(_parsedSpoilerDataServiceMock.Object,
                new ParsedSpoilerDataChangedEventArgs(null, mockSpoilerData));

            Thread.Sleep(50); // Allow UI context post to complete

            // Should update BarrierCount with the cost
            Assert.Equal("5", vm.BarrierCount);
        });
    }

    [Fact]
    public void OnSpoilerDataChanged_WithNegativeCost_SetsToQuestionMark()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<ParsedSpoilerDataChangedEventArgs>? spoilerChangeHandler = null;
            _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
                .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => spoilerChangeHandler = handler);

            var vm = new BLockerHintViewModel(
                RegionName.JUNGLE_JAPES,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            );

            // Create spoiler data with negative cost (barrier not needed)
            var mockSpoilerData = new ParsedSpoilerData
            {
                LevelOrder = [],
                RegionBarrierInfo =
                [
                    new BlockerInfoRaw { Item = 0, Cost = -1 },  // JUNGLE_JAPES has no barrier
                    new BlockerInfoRaw { Item = 2, Cost = 4 },   // ANGRY_AZTEC
                ]
            };

            spoilerChangeHandler?.Invoke(_parsedSpoilerDataServiceMock.Object,
                new ParsedSpoilerDataChangedEventArgs(null, mockSpoilerData));

            Thread.Sleep(50); // Allow UI context post to complete

            // Should set BarrierCount to "?" for negative cost
            Assert.Equal("?", vm.BarrierCount);
        });
    }

    [Fact]
    public void OnSpoilerDataChanged_WithZeroCost_UpdatesToZero()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<ParsedSpoilerDataChangedEventArgs>? spoilerChangeHandler = null;
            _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
                .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => spoilerChangeHandler = handler);

            var vm = new BLockerHintViewModel(
                RegionName.JUNGLE_JAPES,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            );

            // Create spoiler data with zero cost
            var mockSpoilerData = new ParsedSpoilerData
            {
                LevelOrder = [],
                RegionBarrierInfo =
                [
                    new BlockerInfoRaw { Item = 1, Cost = 0 },  // JUNGLE_JAPES costs 0
                ]
            };

            spoilerChangeHandler?.Invoke(_parsedSpoilerDataServiceMock.Object,
                new ParsedSpoilerDataChangedEventArgs(null, mockSpoilerData));

            Thread.Sleep(50); // Allow UI context post to complete

            // Should update BarrierCount to "0"
            Assert.Equal("0", vm.BarrierCount);
        });
    }

    [Fact]
    public void OnSpoilerDataChanged_WithDifferentRegion_UpdatesCorrectCost()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<ParsedSpoilerDataChangedEventArgs>? spoilerChangeHandler = null;
            _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
                .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => spoilerChangeHandler = handler);

            var vm = new BLockerHintViewModel(
                RegionName.ANGRY_AZTEC,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            );

            // Create spoiler data with different costs per region
            var mockSpoilerData = new ParsedSpoilerData
            {
                LevelOrder = [],
                RegionBarrierInfo =
                [
                    new BlockerInfoRaw { Item = 1, Cost = 5 },  // JUNGLE_JAPES
                    new BlockerInfoRaw { Item = 2, Cost = 8 },  // ANGRY_AZTEC
                    new BlockerInfoRaw { Item = 3, Cost = 3 },  // FRANTIC_FACTORY
                ]
            };

            spoilerChangeHandler?.Invoke(_parsedSpoilerDataServiceMock.Object,
                new ParsedSpoilerDataChangedEventArgs(null, mockSpoilerData));

            Thread.Sleep(50); // Allow UI context post to complete

            // Should update with ANGRY_AZTEC's cost (index 1), which is 8
            Assert.Equal("8", vm.BarrierCount);
        });
    }

    [Fact]
    public void OnSpoilerDataChanged_OnUIContext_DoesNotPost()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<ParsedSpoilerDataChangedEventArgs>? spoilerChangeHandler = null;
            _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
                .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => spoilerChangeHandler = handler);

            var vm = new BLockerHintViewModel(
                RegionName.JUNGLE_JAPES,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            );

            // Create spoiler data
            var mockSpoilerData = new ParsedSpoilerData
            {
                LevelOrder = [],
                RegionBarrierInfo =
                [
                    new BlockerInfoRaw { Item = 1, Cost = 7 },
                ]
            };

            // Invoke on UI context (no background thread)
            spoilerChangeHandler?.Invoke(_parsedSpoilerDataServiceMock.Object,
                new ParsedSpoilerDataChangedEventArgs(null, mockSpoilerData));

            // Should update immediately without needing Post delay
            Assert.Equal("7", vm.BarrierCount);
        });
    }

    #endregion

    #region Progress Changed Event Tests

    [Fact]
    public void OnDataReset_ReinitializesState()
    {
        _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("5");
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("10");
        _savedProgressProviderMock.Raise(x => x.ProgressChanged += null,
            new ProgressReplacedEventArgs { OldProgress = null, NewProgress = null! });

        Assert.Equal("10", vm.BarrierCount);
    }

    #endregion

    #region Data Loading Completed Event Tests

    [Fact]
    public void OnDataLoaded_WithValidCost_UpdatesBarrierCount()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<DataLoadingCompletedEventArgs>? dataLoadedHandler = null;
            _dataSaverServiceMock.SetupAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>())
                .Callback((EventHandler<DataLoadingCompletedEventArgs> handler) => dataLoadedHandler = handler);

            _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("0");

            var mockBarrierInfo = new BlockerInfoRaw { Item = 1, Cost = 3 };
            _parsedSpoilerDataServiceMock.Setup(x => x.GetBarrierInfo(RegionName.JUNGLE_JAPES))
                .Returns(mockBarrierInfo);

            var vm = new BLockerHintViewModel(
                RegionName.JUNGLE_JAPES,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            );

            var mockProgress = new SavedProgress();
            dataLoadedHandler?.Invoke(_dataSaverServiceMock.Object,
                new DataLoadingCompletedEventArgs(mockProgress));

            Thread.Sleep(50);

            Assert.Equal("3", vm.BarrierCount);
        });
    }

    [Fact]
    public void OnDataLoaded_WithNegativeCost_ReinitializesState()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<DataLoadingCompletedEventArgs>? dataLoadedHandler = null;
            _dataSaverServiceMock.SetupAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>())
                .Callback((EventHandler<DataLoadingCompletedEventArgs> handler) => dataLoadedHandler = handler);

            _barrierServiceMock.Setup(x => x.GetBLockerCount(RegionName.JUNGLE_JAPES)).Returns("0");

            var mockBarrierInfo = new BlockerInfoRaw { Item = 1, Cost = -1 };
            _parsedSpoilerDataServiceMock.Setup(x => x.GetBarrierInfo(RegionName.JUNGLE_JAPES))
                .Returns(mockBarrierInfo);

            var vm = new BLockerHintViewModel(
                RegionName.JUNGLE_JAPES,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            );

            var mockProgress = new SavedProgress();
            dataLoadedHandler?.Invoke(_dataSaverServiceMock.Object,
                new DataLoadingCompletedEventArgs(mockProgress));

            Assert.Equal("0", vm.BarrierCount);
        });
    }

    #endregion

    #region Cross-Thread Marshaling Tests

    [Fact]
    public void OnSpoilerDataChanged_FromBackgroundThread_MarshalesToUIContext()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<ParsedSpoilerDataChangedEventArgs>? spoilerChangeHandler = null;
            _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
                .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => spoilerChangeHandler = handler);

            var vm = new BLockerHintViewModel(
                RegionName.JUNGLE_JAPES,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            );

            var backgroundTask = Task.Run(() =>
            {
                spoilerChangeHandler?.Invoke(_parsedSpoilerDataServiceMock.Object,
                    new ParsedSpoilerDataChangedEventArgs(null, null));
            });

            backgroundTask.Wait();
            Thread.Sleep(50);

            Assert.Equal("0", vm.BarrierCount); // Should remain unchanged with null data
        });
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromBlockerBarrierAmountChangedEvent()
    {
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);
        vm.Dispose();

        _barrierServiceMock.VerifyRemove(x => x.BlockerBarrierAmountChanged -= It.IsAny<EventHandler<BlockerBarrierAmountChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_UnsubscribesFromParsedSpoilerDataChangedEvent()
    {
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);
        vm.Dispose();

        _parsedSpoilerDataServiceMock.VerifyRemove(x => x.ParsedSpoilerDataChanged -= It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_UnsubscribesFromProgressChangedEvent()
    {
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);
        vm.Dispose();

        _savedProgressProviderMock.VerifyRemove(x => x.ProgressChanged -= It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_UnsubscribesFromDataLoadingCompletedEvent()
    {
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);
        vm.Dispose();

        _dataSaverServiceMock.VerifyRemove(x => x.DataLoadingCompleted -= It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        vm.Dispose();
        vm.Dispose();

        // Should not throw
        _barrierServiceMock.VerifyRemove(x => x.BlockerBarrierAmountChanged -= It.IsAny<EventHandler<BlockerBarrierAmountChangedEventArgs>>(), Times.Once);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameRegion_ReturnsTrue()
    {
        var vm1 = CreateViewModel(RegionName.JUNGLE_JAPES);
        var vm2 = CreateViewModel(RegionName.JUNGLE_JAPES);

        Assert.Equal(vm1, vm2);
    }

    [Fact]
    public void Equals_WithDifferentRegion_ReturnsFalse()
    {
        var vm1 = CreateViewModel(RegionName.JUNGLE_JAPES);
        var vm2 = CreateViewModel(RegionName.ANGRY_AZTEC);

        Assert.NotEqual(vm1, vm2);
    }

    [Fact]
    public void GetHashCode_WithSameRegion_ReturnsSameHash()
    {
        var vm1 = CreateViewModel(RegionName.JUNGLE_JAPES);
        var vm2 = CreateViewModel(RegionName.JUNGLE_JAPES);

        Assert.Equal(vm1.GetHashCode(), vm2.GetHashCode());
    }

    #endregion
}
