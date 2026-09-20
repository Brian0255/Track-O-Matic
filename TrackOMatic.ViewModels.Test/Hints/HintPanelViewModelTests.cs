namespace TrackOMatic.ViewModels.Test.Hints;

public class HintPanelViewModelTests
{
    private readonly Mock<IHintService> _hintServiceMock = new();
    private readonly Mock<IShortcutService> _shortcutServiceMock = new();
    private readonly Mock<IHintItemSelectionService> _hintItemSelectionServiceMock = new();
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock = new();
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock = new();
    private readonly Mock<IDataSaverService> _dataSaverServiceMock = new();

    private HintPanelViewModel CreateViewModel(
        HintPanelType panelType = HintPanelType.Paths,
        string lineColor = "#FF0000",
        IHintService? hintService = null,
        IShortcutService? shortcutService = null,
        IHintItemSelectionService? hintItemSelectionService = null,
        IItemTrackingService? itemTrackingService = null,
        ISavedProgressProvider? savedProgressProvider = null,
        IDataSaverService? dataSaverService = null,
        SavedProgress? currentProgress = null
    )
    {
        // Setup CurrentProgress - use provided progress or create empty one
        var progress = currentProgress ?? new SavedProgress();
        _savedProgressProviderMock.SetupGet(p => p.CurrentProgress).Returns(progress);

        return new HintPanelViewModel(
            panelType,
            lineColor,
            hintService ?? _hintServiceMock.Object,
            shortcutService ?? _shortcutServiceMock.Object,
            hintItemSelectionService ?? _hintItemSelectionServiceMock.Object,
            itemTrackingService ?? _itemTrackingServiceMock.Object,
            savedProgressProvider ?? _savedProgressProviderMock.Object,
            dataSaverService ?? _dataSaverServiceMock.Object
        );
    }

    #region Constructor Validation Tests

    [Fact]
    public void Constructor_WithValidServices_Succeeds()
    {
        var vm = CreateViewModel();
        Assert.NotNull(vm);
    }

    [Fact]
    public void Constructor_WithNullHintService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HintPanelViewModel(
                HintPanelType.Paths,
                "#FF0000",
                null!,
                _shortcutServiceMock.Object,
                _hintItemSelectionServiceMock.Object,
                _itemTrackingServiceMock.Object,
                _savedProgressProviderMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("hintService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullShortcutService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HintPanelViewModel(
                HintPanelType.Paths,
                "#FF0000",
                _hintServiceMock.Object,
                null!,
                _hintItemSelectionServiceMock.Object,
                _itemTrackingServiceMock.Object,
                _savedProgressProviderMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("shortcutService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullHintItemSelectionService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HintPanelViewModel(
                HintPanelType.Paths,
                "#FF0000",
                _hintServiceMock.Object,
                _shortcutServiceMock.Object,
                null!,
                _itemTrackingServiceMock.Object,
                _savedProgressProviderMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("hintItemSelectionService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HintPanelViewModel(
                HintPanelType.Paths,
                "#FF0000",
                _hintServiceMock.Object,
                _shortcutServiceMock.Object,
                _hintItemSelectionServiceMock.Object,
                null!,
                _savedProgressProviderMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("itemTrackingService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HintPanelViewModel(
                HintPanelType.Paths,
                "#FF0000",
                _hintServiceMock.Object,
                _shortcutServiceMock.Object,
                _hintItemSelectionServiceMock.Object,
                _itemTrackingServiceMock.Object,
                null!,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("savedProgressProvider", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullDataSaverService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HintPanelViewModel(
                HintPanelType.Paths,
                "#FF0000",
                _hintServiceMock.Object,
                _shortcutServiceMock.Object,
                _hintItemSelectionServiceMock.Object,
                _itemTrackingServiceMock.Object,
                _savedProgressProviderMock.Object,
                null!
            )
        );
        Assert.Equal("dataSaverService", ex.ParamName);
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void Constructor_InitializesLineColor()
    {
        var vm = CreateViewModel(lineColor: "#ABCDEF");
        Assert.Equal("#ABCDEF", vm.LineColor);
    }

    [Fact]
    public void Constructor_InitializesHintInfoViewModelsEmpty()
    {
        var vm = CreateViewModel();
        Assert.Empty(vm.HintInfoViewModels);
    }

    [Fact]
    public void Constructor_InitializesFilterResourceKeyAsEmpty()
    {
        var vm = CreateViewModel();
        Assert.Equal("filter_empty", vm.FilterResourceKey);
    }

    [Fact]
    public void Constructor_InitializesSortResourceKey()
    {
        var vm = CreateViewModel();
        Assert.Equal("sort", vm.SortResourceKey);
    }

    [Theory]
    [InlineData(HintPanelType.Paths)]
    [InlineData(HintPanelType.Foolish)]
    [InlineData(HintPanelType.Kongs)]
    [InlineData(HintPanelType.Isles)]
    [InlineData(HintPanelType.PotionCounts)]
    public void Constructor_SetsHeadingBasedOnPanelType(HintPanelType panelType)
    {
        var vm = CreateViewModel(panelType: panelType);
        var expectedHeading = panelType.ToHeading();
        Assert.Equal(expectedHeading, vm.Heading);
    }

    [Theory]
    [InlineData(HintPanelType.Paths, true)]
    [InlineData(HintPanelType.Foolish, false)]
    [InlineData(HintPanelType.Kongs, false)]
    [InlineData(HintPanelType.Isles, false)]
    [InlineData(HintPanelType.PotionCounts, false)]
    public void IsPathHintPanel_ReflectsPanelType(HintPanelType panelType, bool expected)
    {
        var vm = CreateViewModel(panelType: panelType);
        Assert.Equal(expected, vm.IsPathHintPanel);
    }

    [Fact]
    public void Constructor_SubscribesToProgressChangedEvent()
    {
        EventHandler<ProgressReplacedEventArgs>? capturedHandler = null;
        _savedProgressProviderMock.SetupAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>())
            .Callback((EventHandler<ProgressReplacedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel();

        Assert.NotNull(capturedHandler);
    }

    [Fact]
    public void Constructor_SubscribesToDataLoadingCompletedEvent()
    {
        EventHandler<DataLoadingCompletedEventArgs>? capturedHandler = null;
        _dataSaverServiceMock.SetupAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>())
            .Callback((EventHandler<DataLoadingCompletedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel();

        Assert.NotNull(capturedHandler);
    }

    [Fact]
    public void Constructor_SubscribesToHintCreatedEvent()
    {
        EventHandler<HintCreatedEventArgs>? capturedHandler = null;
        _hintServiceMock.SetupAdd(x => x.HintCreated += It.IsAny<EventHandler<HintCreatedEventArgs>>())
            .Callback((EventHandler<HintCreatedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel();

        Assert.NotNull(capturedHandler);
    }

    [Fact]
    public void Constructor_SubscribesToHintRemovedEvent()
    {
        EventHandler<HintRemovedEventArgs>? capturedHandler = null;
        _hintServiceMock.SetupAdd(x => x.HintRemoved += It.IsAny<EventHandler<HintRemovedEventArgs>>())
            .Callback((EventHandler<HintRemovedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel();

        Assert.NotNull(capturedHandler);
    }

    #endregion

    #region HintCount and Computed Properties Tests

    [Fact]
    public void HintCount_ReturnsNumberOfHintInfoViewModels()
    {
        var vm = CreateViewModel();
        Assert.Equal(0, vm.HintCount);

        // Add a real hint to test the count
        var hint = new SavedHint
        {
            HintId = new HintId("00000002"),
            HintPanelType = HintPanelType.Paths,
            LocationText = "New Hint"
        };
        _hintServiceMock.Setup(s => s.GetHint(hint.HintId)).Returns(hint);

        var hintVm = new HintInfoViewModel(
            hint.HintId,
            _hintServiceMock.Object,
            _shortcutServiceMock.Object,
            _hintItemSelectionServiceMock.Object,
            _savedProgressProviderMock.Object,
            _itemTrackingServiceMock.Object
        );
        vm.HintInfoViewModels.Add(hintVm);
        Assert.Equal(1, vm.HintCount);
    }

    #endregion

    #region CreateHint Command Tests

    [Fact]
    public void CreateHintCommand_CallsHintServiceCreateHint()
    {
        var vm = CreateViewModel(panelType: HintPanelType.Paths);

        vm.CreateHintCommand.Execute(null);

        _hintServiceMock.Verify(s => s.CreateHint(HintPanelType.Paths), Times.Once);
    }

    [Fact]
    public void CreateHintCommand_WithDifferentPanelTypes_CallsWithCorrectType()
    {
        var vm = CreateViewModel(panelType: HintPanelType.PotionCounts);

        vm.CreateHintCommand.Execute(null);

        _hintServiceMock.Verify(s => s.CreateHint(HintPanelType.PotionCounts), Times.Once);
    }

    #endregion

    #region Event Handling Tests

    [Fact]
    public void OnProgressChanged_LoadsHintsForPanel()
    {
        // Arrange
        var savedHint = new SavedHint
        {
            HintId = new HintId("00000001"),
            HintPanelType = HintPanelType.Paths,
            LocationText = "Test Location"
        };
        _hintServiceMock.Setup(s => s.GetHint(It.IsAny<HintId>())).Returns(savedHint);

        var progress = new SavedProgress();
        progress.SavedHints.Add(savedHint);

        EventHandler<ProgressReplacedEventArgs>? capturedHandler = null;
        _savedProgressProviderMock.SetupAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>())
            .Callback((EventHandler<ProgressReplacedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel(panelType: HintPanelType.Paths, currentProgress: progress);

        // Act
        var newProgress = new SavedProgress();
        newProgress.SavedHints.Add(savedHint);
        _savedProgressProviderMock.SetupGet(p => p.CurrentProgress).Returns(newProgress);
        capturedHandler?.Invoke(_savedProgressProviderMock.Object, new ProgressReplacedEventArgs { NewProgress = newProgress });

        // Assert
        Assert.Single(vm.HintInfoViewModels);
    }

    [Fact]
    public void OnDataLoaded_LoadsHintsForPanel()
    {
        // Arrange - we need real hint service setup to construct HintInfoViewModels successfully
        var savedHint = new SavedHint
        {
            HintId = new HintId("00000001"),
            HintPanelType = HintPanelType.Paths,
            LocationText = "Test Location"
        };
        _hintServiceMock.Setup(s => s.GetHint(It.IsAny<HintId>())).Returns(savedHint);

        var progress = new SavedProgress();
        progress.SavedHints.Add(savedHint);

        EventHandler<DataLoadingCompletedEventArgs>? capturedHandler = null;
        _dataSaverServiceMock.SetupAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>())
            .Callback((EventHandler<DataLoadingCompletedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel(panelType: HintPanelType.Paths, currentProgress: progress);

        // Act
        capturedHandler?.Invoke(_dataSaverServiceMock.Object, new DataLoadingCompletedEventArgs(progress));

        // Assert
        Assert.Single(vm.HintInfoViewModels);
    }

    [Fact]
    public void OnHintCreated_ForMatchingPanelType_AddsHintToCollection()
    {
        // Arrange
        var newHint = new SavedHint
        {
            HintId = new HintId("00000002"),
            HintPanelType = HintPanelType.Paths,
            LocationText = "New Location"
        };
        _hintServiceMock.Setup(s => s.GetHint(It.IsAny<HintId>())).Returns(newHint);

        EventHandler<HintCreatedEventArgs>? capturedHandler = null;
        _hintServiceMock.SetupAdd(x => x.HintCreated += It.IsAny<EventHandler<HintCreatedEventArgs>>())
            .Callback((EventHandler<HintCreatedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel(panelType: HintPanelType.Paths);

        // Act
        capturedHandler?.Invoke(_hintServiceMock.Object, new HintCreatedEventArgs(newHint.HintId, newHint));

        // Assert
        Assert.Single(vm.HintInfoViewModels);
        Assert.Equal("00000002", vm.HintInfoViewModels[0].HintId.ToString());
    }

    [Fact]
    public void OnHintCreated_ForMismatchedPanelType_DoesNotAddHint()
    {
        // Arrange
        var newHint = new SavedHint
        {
            HintId = new HintId("00000002"),
            HintPanelType = HintPanelType.Foolish,
            LocationText = "New Location"
        };

        EventHandler<HintCreatedEventArgs>? capturedHandler = null;
        _hintServiceMock.SetupAdd(x => x.HintCreated += It.IsAny<EventHandler<HintCreatedEventArgs>>())
            .Callback((EventHandler<HintCreatedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel(panelType: HintPanelType.Paths);

        // Act
        capturedHandler?.Invoke(_hintServiceMock.Object, new HintCreatedEventArgs(newHint.HintId, newHint));

        // Assert
        Assert.Empty(vm.HintInfoViewModels);
    }

    [Fact]
    public void OnHintCreated_SetsShouldFocusLocationOnNewHint()
    {
        // Arrange
        var newHint = new SavedHint
        {
            HintId = new HintId("00000002"),
            HintPanelType = HintPanelType.Paths,
            LocationText = ""
        };
        _hintServiceMock.Setup(s => s.GetHint(It.IsAny<HintId>())).Returns(newHint);

        EventHandler<HintCreatedEventArgs>? capturedHandler = null;
        _hintServiceMock.SetupAdd(x => x.HintCreated += It.IsAny<EventHandler<HintCreatedEventArgs>>())
            .Callback((EventHandler<HintCreatedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel(panelType: HintPanelType.Paths);

        // Act
        capturedHandler?.Invoke(_hintServiceMock.Object, new HintCreatedEventArgs(newHint.HintId, newHint));

        // Assert
        Assert.True(vm.HintInfoViewModels[0].ShouldFocusLocation);
    }

    [Fact]
    public void OnHintRemoved_RemovesHintFromCollection()
    {
        // Arrange
        var savedHint = new SavedHint
        {
            HintId = new HintId("00000001"),
            HintPanelType = HintPanelType.Paths,
            LocationText = "Test Location"
        };
        _hintServiceMock.Setup(s => s.GetHint(It.IsAny<HintId>())).Returns(savedHint);

        var progress = new SavedProgress();
        progress.SavedHints.Add(savedHint);

        EventHandler<HintRemovedEventArgs>? capturedHandler = null;
        _hintServiceMock.SetupAdd(x => x.HintRemoved += It.IsAny<EventHandler<HintRemovedEventArgs>>())
            .Callback((EventHandler<HintRemovedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel(panelType: HintPanelType.Paths, currentProgress: progress);
        Assert.Single(vm.HintInfoViewModels);

        // Act
        capturedHandler?.Invoke(_hintServiceMock.Object, new HintRemovedEventArgs(savedHint.HintId, savedHint));

        // Assert
        Assert.Empty(vm.HintInfoViewModels);
    }

    #endregion

    #region Load Hints Tests

    [Fact]
    public void LoadHintsForPanel_LoadsHintsWithMatchingPanelType()
    {
        // Arrange
        var pathHint = new SavedHint
        {
            HintId = new HintId("00000001"),
            HintPanelType = HintPanelType.Paths,
            LocationText = "Path Location"
        };
        var foolishHint = new SavedHint
        {
            HintId = new HintId("00000002"),
            HintPanelType = HintPanelType.Foolish,
            LocationText = "Foolish Location"
        };
        _hintServiceMock.Setup(s => s.GetHint(It.IsAny<HintId>())).Returns((HintId id) =>
            id.ToString() == "00000001" ? pathHint : foolishHint
        );

        var progress = new SavedProgress();
        progress.SavedHints.Add(pathHint);
        progress.SavedHints.Add(foolishHint);

        // Act
        var vm = CreateViewModel(panelType: HintPanelType.Paths, currentProgress: progress);

        // Assert
        Assert.Single(vm.HintInfoViewModels);
        Assert.Equal("00000001", vm.HintInfoViewModels[0].HintId.ToString());
    }

    [Fact]
    public void LoadHintsForPanel_UpdatesFilterResourceKeyToOn_WhenHintsExist()
    {
        // Arrange
        var savedHint = new SavedHint
        {
            HintId = new HintId("00000001"),
            HintPanelType = HintPanelType.Paths,
            LocationText = "Test Location"
        };
        _hintServiceMock.Setup(s => s.GetHint(It.IsAny<HintId>())).Returns(savedHint);

        var progress = new SavedProgress();
        progress.SavedHints.Add(savedHint);

        // Act
        var vm = CreateViewModel(panelType: HintPanelType.Paths, currentProgress: progress);

        // Assert
        Assert.Equal("filter_on", vm.FilterResourceKey);
    }

    #endregion

    #region Sort and Filter Tests

    [Fact]
    public void SortHintsCommand_CanExecute()
    {
        var vm = CreateViewModel();
        Assert.True(vm.SortHintsCommand.CanExecute(null));
    }

    [Fact]
    public void SortHintsCommand_Execute_ExecutesWithoutError()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act
        vm.SortHintsCommand.Execute(null);

        // Assert - just verify it executes without error
        Assert.NotNull(vm);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromProgressChangedEvent()
    {
        // Arrange
        EventHandler<ProgressReplacedEventArgs>? capturedHandler = null;
        _savedProgressProviderMock.SetupAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>())
            .Callback((EventHandler<ProgressReplacedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel();

        // Act
        vm.Dispose();

        // Assert
        _savedProgressProviderMock.VerifyRemove(
            x => x.ProgressChanged -= capturedHandler!,
            Times.Once
        );
    }

    [Fact]
    public void Dispose_UnsubscribesFromDataLoadingCompletedEvent()
    {
        // Arrange
        EventHandler<DataLoadingCompletedEventArgs>? capturedHandler = null;
        _dataSaverServiceMock.SetupAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>())
            .Callback((EventHandler<DataLoadingCompletedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel();

        // Act
        vm.Dispose();

        // Assert
        _dataSaverServiceMock.VerifyRemove(
            x => x.DataLoadingCompleted -= capturedHandler!,
            Times.Once
        );
    }

    [Fact]
    public void Dispose_UnsubscribesFromHintCreatedEvent()
    {
        // Arrange
        EventHandler<HintCreatedEventArgs>? capturedHandler = null;
        _hintServiceMock.SetupAdd(x => x.HintCreated += It.IsAny<EventHandler<HintCreatedEventArgs>>())
            .Callback((EventHandler<HintCreatedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel();

        // Act
        vm.Dispose();

        // Assert
        _hintServiceMock.VerifyRemove(
            x => x.HintCreated -= capturedHandler!,
            Times.Once
        );
    }

    [Fact]
    public void Dispose_UnsubscribesFromHintRemovedEvent()
    {
        // Arrange
        EventHandler<HintRemovedEventArgs>? capturedHandler = null;
        _hintServiceMock.SetupAdd(x => x.HintRemoved += It.IsAny<EventHandler<HintRemovedEventArgs>>())
            .Callback((EventHandler<HintRemovedEventArgs> handler) => capturedHandler = handler);

        var vm = CreateViewModel();

        // Act
        vm.Dispose();

        // Assert
        _hintServiceMock.VerifyRemove(
            x => x.HintRemoved -= capturedHandler!,
            Times.Once
        );
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var vm = CreateViewModel();

        // Act & Assert - should not throw
        vm.Dispose();
        vm.Dispose();
    }

    #endregion

    #region Object Override Tests

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var vm = CreateViewModel(panelType: HintPanelType.Paths);
        var result = vm.ToString();

        Assert.Multiple(() =>
        {
            Assert.Contains("HintPanelViewModel", result);
            Assert.Contains("Paths", result);
            Assert.Contains("0", result); // HintsCount
        });
    }

    [Fact]
    public void Equals_WithSamePanelType_ReturnsTrue()
    {
        var vm1 = CreateViewModel(panelType: HintPanelType.Paths);
        var vm2 = CreateViewModel(panelType: HintPanelType.Paths);

        Assert.True(vm1.Equals((object)vm2));
    }

    [Fact]
    public void Equals_WithDifferentPanelType_ReturnsFalse()
    {
        var vm1 = CreateViewModel(panelType: HintPanelType.Paths);
        var vm2 = CreateViewModel(panelType: HintPanelType.Foolish);

        Assert.False(vm1.Equals(vm2));
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var vm = CreateViewModel();
        Assert.False(vm.Equals(null));
    }

    [Fact]
    public void GetHashCode_WithSamePanelType_AreEqual()
    {
        var vm1 = CreateViewModel(panelType: HintPanelType.Paths);
        var vm2 = CreateViewModel(panelType: HintPanelType.Paths);

        Assert.Equal(vm1.GetHashCode(), vm2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentPanelType_AreDifferent()
    {
        var vm1 = CreateViewModel(panelType: HintPanelType.Paths);
        var vm2 = CreateViewModel(panelType: HintPanelType.Foolish);

        Assert.NotEqual(vm1.GetHashCode(), vm2.GetHashCode());
    }

    #endregion
}
