namespace TrackOMatic.ViewModels.Test.Hints;

public class MainHintViewModelTests
{
    private readonly Mock<IHintService> _hintServiceMock = new();
    private readonly Mock<IShortcutService> _shortcutServiceMock = new();
    private readonly Mock<IHintItemSelectionService> _hintItemSelectionServiceMock = new();
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock = new();
    private readonly Mock<IUserSettingsService> _userSettingsServiceMock = new();
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock = new();
    private readonly Mock<IBarrierService> _barrierServiceMock = new();
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock = new();
    private readonly Mock<IItemSelectorService> _itemSelectorServiceMock = new();
    private readonly Mock<IDataSaverService> _dataSaverServiceMock = new();

    public MainHintViewModelTests()
    {
        // Default to a valid SavedProgress for eager loading to work
        var savedProgress = new SavedProgress();
        _savedProgressProviderMock.SetupGet(x => x.CurrentProgress).Returns(savedProgress);
    }

    private MainHintViewModel CreateViewModelInternal(
        IHintService? hintService = null,
        IShortcutService? shortcutService = null,
        IHintItemSelectionService? hintItemSelectionService = null,
        IItemTrackingService? itemTrackingService = null,
        IUserSettingsService? userSettingsService = null,
        ISavedProgressProvider? savedProgressProvider = null,
        IBarrierService? barrierService = null,
        IParsedSpoilerDataService? parsedSpoilerDataService = null,
        IItemSelectorService? itemSelectorService = null,
        IDataSaverService? dataSaverService = null
    )
    {
        return new MainHintViewModel(
            hintService ?? _hintServiceMock.Object,
            shortcutService ?? _shortcutServiceMock.Object,
            hintItemSelectionService ?? _hintItemSelectionServiceMock.Object,
            itemTrackingService ?? _itemTrackingServiceMock.Object,
            userSettingsService ?? _userSettingsServiceMock.Object,
            savedProgressProvider ?? _savedProgressProviderMock.Object,
            barrierService ?? _barrierServiceMock.Object,
            parsedSpoilerDataService ?? _parsedSpoilerDataServiceMock.Object,
            itemSelectorService ?? _itemSelectorServiceMock.Object,
            dataSaverService ?? _dataSaverServiceMock.Object
        );
    }

    private MainHintViewModel CreateViewModelWithNull(string parameterName)
    {
        // Use reflection to call the internal creator with appropriate null for the parameter
        _ = typeof(MainHintViewModelTests).GetMethod("CreateViewModelInternal",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var parameters = new object?[10];
        for (int i = 0; i < parameters.Length; i++)
        {
            parameters[i] = null!; // Pass nulls for all, then selectively fill in the mocks
        }

        return parameterName switch
        {
            "IHintService" => new MainHintViewModel(
                null!,
                _shortcutServiceMock.Object,
                _hintItemSelectionServiceMock.Object,
                _itemTrackingServiceMock.Object,
                _userSettingsServiceMock.Object,
                _savedProgressProviderMock.Object,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            ),
            "IShortcutService" => new MainHintViewModel(
                _hintServiceMock.Object,
                null!,
                _hintItemSelectionServiceMock.Object,
                _itemTrackingServiceMock.Object,
                _userSettingsServiceMock.Object,
                _savedProgressProviderMock.Object,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            ),
            "IHintItemSelectionService" => new MainHintViewModel(
                _hintServiceMock.Object,
                _shortcutServiceMock.Object,
                null!,
                _itemTrackingServiceMock.Object,
                _userSettingsServiceMock.Object,
                _savedProgressProviderMock.Object,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            ),
            "IItemTrackingService" => new MainHintViewModel(
                _hintServiceMock.Object,
                _shortcutServiceMock.Object,
                _hintItemSelectionServiceMock.Object,
                null!,
                _userSettingsServiceMock.Object,
                _savedProgressProviderMock.Object,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            ),
            "IUserSettingsService" => new MainHintViewModel(
                _hintServiceMock.Object,
                _shortcutServiceMock.Object,
                _hintItemSelectionServiceMock.Object,
                _itemTrackingServiceMock.Object,
                null!,
                _savedProgressProviderMock.Object,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            ),
            "ISavedProgressProvider" => new MainHintViewModel(
                _hintServiceMock.Object,
                _shortcutServiceMock.Object,
                _hintItemSelectionServiceMock.Object,
                _itemTrackingServiceMock.Object,
                _userSettingsServiceMock.Object,
                null!,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            ),
            "IDataSaverService" => new MainHintViewModel(
                _hintServiceMock.Object,
                _shortcutServiceMock.Object,
                _hintItemSelectionServiceMock.Object,
                _itemTrackingServiceMock.Object,
                _userSettingsServiceMock.Object,
                _savedProgressProviderMock.Object,
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _itemSelectorServiceMock.Object,
                null!
            ),
            _ => throw new ArgumentException($"Unknown parameter: {parameterName}")
        };
    }

    private MainHintViewModel CreateViewModel()
    {
        return CreateViewModelInternal();
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
            CreateViewModelWithNull(nameof(IHintService))
        );
        Assert.Equal("hintService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullShortcutService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            CreateViewModelWithNull(nameof(IShortcutService))
        );
        Assert.Equal("shortcutService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullHintItemSelectionService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            CreateViewModelWithNull(nameof(IHintItemSelectionService))
        );
        Assert.Equal("hintItemSelectionService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            CreateViewModelWithNull(nameof(IItemTrackingService))
        );
        Assert.Equal("itemTrackingService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullUserSettingsService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            CreateViewModelWithNull(nameof(IUserSettingsService))
        );
        Assert.Equal("userSettingsService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            CreateViewModelWithNull(nameof(ISavedProgressProvider))
        );
        Assert.Equal("savedProgressProvider", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullDataSaverService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            CreateViewModelWithNull(nameof(IDataSaverService))
        );
        Assert.Equal("dataSaverService", ex.ParamName);
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void Constructor_InitializesBLockerPanelViewModel()
    {
        var vm = CreateViewModel();
        Assert.NotNull(vm.BLockerPanelViewModel);
    }

    [Fact]
    public void Constructor_InitializesHelmDoorPanelViewModel()
    {
        var vm = CreateViewModel();
        Assert.NotNull(vm.HelmDoorPanelViewModel);
    }

    [Fact]
    public void Constructor_SubscribesToUserSettingsServicePropertyChanged()
    {
        PropertyChangedEventHandler? capturedHandler = null;
        _userSettingsServiceMock.SetupAdd(x => x.PropertyChanged += It.IsAny<PropertyChangedEventHandler>())
            .Callback((PropertyChangedEventHandler handler) => capturedHandler = handler);

        var vm = CreateViewModel();

        Assert.NotNull(capturedHandler);
    }

    #endregion

    #region Eager-Loaded HintPanel ViewModels Tests

    [Fact]
    public void AllHintPanelViewModels_CreatedOnConstruction()
    {
        var vm = CreateViewModel();

        Assert.Multiple(() =>
        {
            Assert.NotNull(vm.PathsHintPanelViewModel);
            Assert.NotNull(vm.FoolishHintPanelViewModel);
            Assert.NotNull(vm.KongsHintPanelViewModel);
            Assert.NotNull(vm.HoardHintPanelViewModel);
            Assert.NotNull(vm.PotionHintPanelViewModel);
            Assert.NotNull(vm.UnhintedHintPanelViewModel);
            Assert.NotNull(vm.IslesHintPanelViewModel);
            Assert.NotNull(vm.JapesHintPanelViewModel);
            Assert.NotNull(vm.AztecHintPanelViewModel);
            Assert.NotNull(vm.FactoryHintPanelViewModel);
            Assert.NotNull(vm.GalleonHintPanelViewModel);
            Assert.NotNull(vm.ForestHintPanelViewModel);
            Assert.NotNull(vm.CavesHintPanelViewModel);
            Assert.NotNull(vm.CastleHintPanelViewModel);
            Assert.NotNull(vm.HelmHintPanelViewModel);
        });
    }

    [Fact]
    public void SpecialPanels_BLockerAndHelmDoor_CreatedOnConstruction()
    {
        var vm = CreateViewModel();
        Assert.NotNull(vm.BLockerPanelViewModel);
        Assert.NotNull(vm.HelmDoorPanelViewModel);
    }

    [Fact]
    public void EachHintPanelViewModel_IsDistinctInstance()
    {
        var vm = CreateViewModel();

        Assert.Multiple(() =>
        {
            // Verify that each property returns the same instance (no re-creation on access)
            Assert.Same(vm.PathsHintPanelViewModel, vm.PathsHintPanelViewModel);
            Assert.Same(vm.FoolishHintPanelViewModel, vm.FoolishHintPanelViewModel);
            Assert.Same(vm.KongsHintPanelViewModel, vm.KongsHintPanelViewModel);

            // Verify that different panels are different instances
            Assert.NotSame(vm.PathsHintPanelViewModel, vm.FoolishHintPanelViewModel);
            Assert.NotSame(vm.FoolishHintPanelViewModel, vm.KongsHintPanelViewModel);
        });
    }

    #endregion

    #region Proxy Properties Tests

    [Theory]
    [InlineData(HintDisplayMode.Off, true, false, false)]
    [InlineData(HintDisplayMode.DirectItemHints, false, true, false)]
    [InlineData(HintDisplayMode.MultipathHints, false, false, true)]
    public void DisplayProperties_ReflectHintDisplayMode(HintDisplayMode mode, bool expectNoHints, bool expectDirect, bool expectPath)
    {
        _userSettingsServiceMock.SetupGet(s => s.HintDisplay).Returns(mode);
        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);

        Assert.Multiple(() =>
        {
            Assert.Equal(expectNoHints, vm.DisplayNoHints);
            Assert.Equal(expectDirect, vm.DisplayDirectHints);
            Assert.Equal(expectPath, vm.DisplayPathHints);
        });
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsHelmDoorPanelEnabled_ReflectsUserSetting(bool enabled)
    {
        _userSettingsServiceMock.SetupGet(s => s.HelmDoors).Returns(enabled);
        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);

        Assert.Equal(enabled, vm.IsHelmDoorPanelEnabled);
    }

    [Theory]
    [InlineData(true, HintDisplayMode.DirectItemHints, true)]
    [InlineData(true, HintDisplayMode.MultipathHints, false)]
    [InlineData(false, HintDisplayMode.DirectItemHints, false)]
    [InlineData(false, HintDisplayMode.MultipathHints, false)]
    public void IsDirectHintCompactMode_ReflectsSettings(bool compactModeOn, HintDisplayMode displayMode, bool expected)
    {
        _userSettingsServiceMock.SetupGet(s => s.CompactMode).Returns(compactModeOn);
        _userSettingsServiceMock.SetupGet(s => s.HintDisplay).Returns(displayMode);
        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);

        Assert.Equal(expected, vm.IsDirectHintCompactMode);
    }

    [Theory]
    [InlineData(true, HintDisplayMode.DirectItemHints, false)]
    [InlineData(true, HintDisplayMode.MultipathHints, false)]
    [InlineData(false, HintDisplayMode.DirectItemHints, true)]
    [InlineData(false, HintDisplayMode.MultipathHints, false)]
    public void IsDirectHintStandardMode_ReflectsSettings(bool compactModeOn, HintDisplayMode displayMode, bool expected)
    {
        _userSettingsServiceMock.SetupGet(s => s.CompactMode).Returns(compactModeOn);
        _userSettingsServiceMock.SetupGet(s => s.HintDisplay).Returns(displayMode);
        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);

        Assert.Equal(expected, vm.IsDirectHintStandardMode);
    }

    [Theory]
    [InlineData(true, HintDisplayMode.MultipathHints, true)]
    [InlineData(true, HintDisplayMode.DirectItemHints, false)]
    [InlineData(false, HintDisplayMode.MultipathHints, false)]
    [InlineData(false, HintDisplayMode.DirectItemHints, false)]
    public void IsMultipathCompactMode_ReflectsSettings(bool compactModeOn, HintDisplayMode displayMode, bool expected)
    {
        _userSettingsServiceMock.SetupGet(s => s.CompactMode).Returns(compactModeOn);
        _userSettingsServiceMock.SetupGet(s => s.HintDisplay).Returns(displayMode);
        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);

        Assert.Equal(expected, vm.IsMultipathCompactMode);
    }

    [Theory]
    [InlineData(true, HintDisplayMode.MultipathHints, false)]
    [InlineData(true, HintDisplayMode.DirectItemHints, false)]
    [InlineData(false, HintDisplayMode.MultipathHints, true)]
    [InlineData(false, HintDisplayMode.DirectItemHints, false)]
    public void IsMultipathStandardMode_ReflectsSettings(bool compactModeOn, HintDisplayMode displayMode, bool expected)
    {
        _userSettingsServiceMock.SetupGet(s => s.CompactMode).Returns(compactModeOn);
        _userSettingsServiceMock.SetupGet(s => s.HintDisplay).Returns(displayMode);
        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);

        Assert.Equal(expected, vm.IsMultipathStandardMode);
    }

    #endregion

    #region CurrentModeKey Tests

    [Theory]
    [InlineData(true, HintDisplayMode.DirectItemHints, "DirectCompact")]
    [InlineData(false, HintDisplayMode.DirectItemHints, "DirectStandard")]
    [InlineData(true, HintDisplayMode.MultipathHints, "MultipathCompact")]
    [InlineData(false, HintDisplayMode.MultipathHints, "MultipathStandard")]
    [InlineData(false, HintDisplayMode.Off, "None")]
    public void CurrentModeKey_ReturnsCorrectModeString(bool compactMode, HintDisplayMode displayMode, string expectedKey)
    {
        _userSettingsServiceMock.SetupGet(s => s.CompactMode).Returns(compactMode);
        _userSettingsServiceMock.SetupGet(s => s.HintDisplay).Returns(displayMode);
        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);

        Assert.Equal(expectedKey, vm.CurrentModeKey);
    }

    #endregion

    #region OnPropertyChanged Event Handling Tests

    [Fact]
    public void OnHintDisplayChanged_NotifiesPropertyChanges()
    {
        PropertyChangedEventHandler? capturedHandler = null;
        _userSettingsServiceMock.SetupAdd(x => x.PropertyChanged += It.IsAny<PropertyChangedEventHandler>())
            .Callback((PropertyChangedEventHandler handler) => capturedHandler = handler);

        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);
        var changedProperties = new List<string?>();
        vm.PropertyChanged += (s, e) => changedProperties.Add(e.PropertyName);

        // Act
        capturedHandler?.Invoke(_userSettingsServiceMock.Object, new PropertyChangedEventArgs(nameof(IUserSettingsService.HintDisplay)));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Contains(nameof(MainHintViewModel.DisplayNoHints), changedProperties);
            Assert.Contains(nameof(MainHintViewModel.DisplayDirectHints), changedProperties);
            Assert.Contains(nameof(MainHintViewModel.DisplayPathHints), changedProperties);
            Assert.Contains(nameof(MainHintViewModel.IsDirectHintCompactMode), changedProperties);
            Assert.Contains(nameof(MainHintViewModel.IsDirectHintStandardMode), changedProperties);
            Assert.Contains(nameof(MainHintViewModel.IsMultipathCompactMode), changedProperties);
            Assert.Contains(nameof(MainHintViewModel.IsMultipathStandardMode), changedProperties);
            Assert.Contains(nameof(MainHintViewModel.CurrentModeKey), changedProperties);
        });
    }

    [Fact]
    public void OnCompactModeChanged_NotifiesPropertyChanges()
    {
        PropertyChangedEventHandler? capturedHandler = null;
        _userSettingsServiceMock.SetupAdd(x => x.PropertyChanged += It.IsAny<PropertyChangedEventHandler>())
            .Callback((PropertyChangedEventHandler handler) => capturedHandler = handler);

        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);
        var changedProperties = new List<string?>();
        vm.PropertyChanged += (s, e) => changedProperties.Add(e.PropertyName);

        // Act
        capturedHandler?.Invoke(_userSettingsServiceMock.Object, new PropertyChangedEventArgs(nameof(IUserSettingsService.CompactMode)));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Contains(nameof(MainHintViewModel.IsDirectHintCompactMode), changedProperties);
            Assert.Contains(nameof(MainHintViewModel.IsDirectHintStandardMode), changedProperties);
            Assert.Contains(nameof(MainHintViewModel.IsMultipathCompactMode), changedProperties);
            Assert.Contains(nameof(MainHintViewModel.IsMultipathStandardMode), changedProperties);
            Assert.Contains(nameof(MainHintViewModel.CurrentModeKey), changedProperties);
        });
    }

    [Fact]
    public void OnHelmDoorsChanged_NotifiesPropertyChange()
    {
        PropertyChangedEventHandler? capturedHandler = null;
        _userSettingsServiceMock.SetupAdd(x => x.PropertyChanged += It.IsAny<PropertyChangedEventHandler>())
            .Callback((PropertyChangedEventHandler handler) => capturedHandler = handler);

        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);
        var changedProperties = new List<string?>();
        vm.PropertyChanged += (s, e) => changedProperties.Add(e.PropertyName);

        // Act
        capturedHandler?.Invoke(_userSettingsServiceMock.Object, new PropertyChangedEventArgs(nameof(IUserSettingsService.HelmDoors)));

        // Assert
        Assert.Contains(nameof(MainHintViewModel.IsHelmDoorPanelEnabled), changedProperties);
    }

    [Fact]
    public void OnUnrelatedPropertyChanged_NoNotification()
    {
        PropertyChangedEventHandler? capturedHandler = null;
        _userSettingsServiceMock.SetupAdd(x => x.PropertyChanged += It.IsAny<PropertyChangedEventHandler>())
            .Callback((PropertyChangedEventHandler handler) => capturedHandler = handler);

        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);
        var changedProperties = new List<string?>();
        vm.PropertyChanged += (s, e) => changedProperties.Add(e.PropertyName);

        // Act
        capturedHandler?.Invoke(_userSettingsServiceMock.Object, new PropertyChangedEventArgs("SomeOtherProperty"));

        // Assert
        Assert.Empty(changedProperties);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromUserSettingsService()
    {
        PropertyChangedEventHandler? capturedHandler = null;
        _userSettingsServiceMock.SetupAdd(x => x.PropertyChanged += It.IsAny<PropertyChangedEventHandler>())
            .Callback((PropertyChangedEventHandler handler) => capturedHandler = handler);

        var vm = CreateViewModelInternal(userSettingsService: _userSettingsServiceMock.Object);

        // Act
        vm.Dispose();

        // Assert
        _userSettingsServiceMock.VerifyRemove(
            x => x.PropertyChanged -= capturedHandler!,
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
}
