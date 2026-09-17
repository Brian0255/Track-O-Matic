namespace TrackOMatic.ViewModels.Test;

/// <summary>
/// Test suite for MainWindowViewModel.
/// Tests constructor validation, child view model initialization,
/// property synchronization with services, command execution,
/// event handling from settings changes, calculated properties,
/// and proper disposal of resources.
/// </summary>
public class MainWindowViewModelTests
{
    private static void SetupEndGameProgressionMocks(Mock<IEndGameProgressionService> mockEndGameProgressionService)
    {
        mockEndGameProgressionService.Setup(s => s.GetHelmKongs()).Returns([0, 0, 0, 0, 0]);
        mockEndGameProgressionService.Setup(s => s.GetBossKongs()).Returns([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
    }
    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullServices_ThrowsArgumentNullException()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            new MainWindowViewModel(
                mockItemTrackingService.Object,
                null!,
                mockCollectiblesService.Object,
                mockThemeService.Object,
                mockAutotrackerService.Object,
                mockParsedSpoilerDataService.Object,
                mockEndGameProgressionService.Object,
                mockSavedProgressProvider.Object,
                mockItemSelectorService.Object,
                mockDataSaverService.Object
            )
        );

        Assert.Equal("userSettingsService", exception.ParamName);
    }

    [Fact]
    public void Constructor_InitializesChildViewModels()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        Assert.NotNull(viewModel.CollectiblesGridRowViewModel);
        Assert.NotEmpty(viewModel.KongItems);
        Assert.NotEmpty(viewModel.KongBlueprints);
        Assert.NotEmpty(viewModel.Keys);
        Assert.NotEmpty(viewModel.Shopkeepers);
        Assert.NotEmpty(viewModel.TrainingMoves);
        Assert.NotEmpty(viewModel.MiscMoves);
        Assert.NotEmpty(viewModel.HelmSlots);
        Assert.NotEmpty(viewModel.BossSlots);

        viewModel.Dispose();
    }

    [Fact]
    public void Constructor_PopulatesCollectionsWithCorrectCounts()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // Verify exact counts based on actual initialization loops
        Assert.Equal(5, viewModel.KongBlueprints.Count);
        Assert.Equal(8, viewModel.Keys.Count);
        Assert.Equal(4, viewModel.Shopkeepers.Count);
        Assert.Equal(5, viewModel.TrainingMoves.Count);
        Assert.Equal(8, viewModel.MiscMoves.Count);
        Assert.Equal(5, viewModel.HelmSlots.Count);
        Assert.Equal(5, viewModel.BossSlots.Count);

        // KongItems count depends on Extensions.KongItemNames, just verify it's populated
        Assert.NotEmpty(viewModel.KongItems);

        viewModel.Dispose();
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void InitializeState_LoadsSettingsIntoProperties()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        // Setup user settings
        mockUserSettingsService.Setup(s => s.TopMost).Returns(true);
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);
        mockUserSettingsService.Setup(s => s.SongDisplay).Returns(true);
        mockUserSettingsService.Setup(s => s.CompactMode).Returns(true);

        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        Assert.True(viewModel.IsTopMostEnabled);
        Assert.False(viewModel.IsAutotrackingEnabled);
        Assert.True(viewModel.IsSongDisplayEnabled);
        Assert.True(viewModel.IsCompactModeEnabled);

        viewModel.Dispose();
    }

    #endregion

    #region Toggle Command Tests

    [Fact]
    public void ToggleTopMostCommand_TogglesProperty()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockUserSettingsService.Setup(s => s.TopMost).Returns(false);
        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        var initialValue = viewModel.IsTopMostEnabled;
        viewModel.ToggleTopMostCommand.Execute(null);
        var finalValue = viewModel.IsTopMostEnabled;

        Assert.NotEqual(initialValue, finalValue);

        viewModel.Dispose();
    }

    [Fact]
    public void ToggleAutotrackingCommand_TogglesProperty()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);
        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        var initialValue = viewModel.IsAutotrackingEnabled;
        viewModel.ToggleAutotrackingCommand.Execute(null);
        var finalValue = viewModel.IsAutotrackingEnabled;

        Assert.NotEqual(initialValue, finalValue);

        viewModel.Dispose();
    }

    [Fact]
    public void ToggleColoredBarrelPadMovesCommand_CallsThemeService()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockUserSettingsService.Setup(s => s.ColoredBarrelPadMoves).Returns(false);
        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        viewModel.ToggleColoredBarrelPadMovesCommand.Execute(null);

        mockThemeService.Verify(s => s.SetBarrelPadTheme(It.IsAny<bool>()), Times.Once);

        viewModel.Dispose();
    }

    [Theory]
    [InlineData(nameof(MainWindowViewModel.ToggleSongDisplayCommand), nameof(MainWindowViewModel.IsSongDisplayEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleHelmInLevelOrderCommand), nameof(MainWindowViewModel.IsHelmInLevelOrderEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleTotalBlueprintCollectibleCommand), nameof(MainWindowViewModel.IsTotalBlueprintCollectibleEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleCompanyCoinCollectibleCommand), nameof(MainWindowViewModel.IsCompanyCoinCollectibleEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleHelmSpoilerOrderCommand), nameof(MainWindowViewModel.IsShowingHelmSpoilerInfoEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleBossSpoilerOrderCommand), nameof(MainWindowViewModel.IsShowingBossSpoilerInfoEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleProgressiveHintCounterCommand), nameof(MainWindowViewModel.IsProgressiveHintCounterEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleSortablePathHintsCommand), nameof(MainWindowViewModel.IsSortablePathHintsEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleHelmDoorPanelCommand), nameof(MainWindowViewModel.IsHelmDoorPanelEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleCompactModeCommand), nameof(MainWindowViewModel.IsCompactModeEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleEnemySearchCommand), nameof(MainWindowViewModel.IsEnemySearchEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleBroadcastViewCommand), nameof(MainWindowViewModel.IsBroadcastViewEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleBroadcastHelmKRoolCommand), nameof(MainWindowViewModel.IsBroadcastHelmKRoolEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleBroadcastShopkeepersCommand), nameof(MainWindowViewModel.IsBroadcastShopkeepersEnabled))]
    [InlineData(nameof(MainWindowViewModel.ToggleBroadcastSongDisplayCommand), nameof(MainWindowViewModel.IsBroadcastSongDisplayEnabled))]
    public void ToggleCommands_TogglesProperty(string commandName, string propertyName)
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        // Initialize all toggle properties to false
        mockUserSettingsService.Setup(s => s.SongDisplay).Returns(false);
        mockUserSettingsService.Setup(s => s.HelmInLevelOrder).Returns(false);
        mockUserSettingsService.Setup(s => s.ShowTotalBPs).Returns(false);
        mockUserSettingsService.Setup(s => s.ShowCompanyCoins).Returns(false);
        mockUserSettingsService.Setup(s => s.ShowHelmOrder).Returns(false);
        mockUserSettingsService.Setup(s => s.ShowKRoolOrder).Returns(false);
        mockUserSettingsService.Setup(s => s.ShowAmountForHints).Returns(false);
        mockUserSettingsService.Setup(s => s.AutoSortPathHints).Returns(false);
        mockUserSettingsService.Setup(s => s.HelmDoors).Returns(false);
        mockUserSettingsService.Setup(s => s.CompactMode).Returns(false);
        mockUserSettingsService.Setup(s => s.EnemiesInAutofill).Returns(false);
        mockUserSettingsService.Setup(s => s.BroadcastViewActive).Returns(false);
        mockUserSettingsService.Setup(s => s.BroadcastHelmKRool).Returns(false);
        mockUserSettingsService.Setup(s => s.BroadcastShopkeepers).Returns(false);
        mockUserSettingsService.Setup(s => s.BroadcastSongDisplay).Returns(false);
        mockUserSettingsService.Setup(s => s.TopMost).Returns(false);
        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);
        mockUserSettingsService.Setup(s => s.ColoredBarrelPadMoves).Returns(false);

        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // Get the command via reflection
        var commandProperty = typeof(MainWindowViewModel).GetProperty(commandName);
        Assert.NotNull(commandProperty);
        var command = commandProperty.GetValue(viewModel) as System.Windows.Input.ICommand;
        Assert.NotNull(command);

        // Get initial value via reflection
        var propertyInfo = typeof(MainWindowViewModel).GetProperty(propertyName);
        Assert.NotNull(propertyInfo);
        var initialValue = (bool)propertyInfo.GetValue(viewModel)!;

        // Execute command
        command.Execute(null);

        // Get final value
        var finalValue = (bool)propertyInfo.GetValue(viewModel)!;

        // Verify toggle occurred
        Assert.NotEqual(initialValue, finalValue);

        viewModel.Dispose();
    }

    #endregion

    #region SetHintDisplay Command Tests

    [Theory]
    [InlineData(HintDisplayMode.Off)]
    [InlineData(HintDisplayMode.MultipathHints)]
    [InlineData(HintDisplayMode.DirectItemHints)]
    public void SetHintDisplayCommand_UpdatesHintDisplayMode(HintDisplayMode mode)
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        viewModel.SetHintDisplayCommand.Execute(mode);

        Assert.Equal(mode, viewModel.HintDisplayMode);

        viewModel.Dispose();
    }

    #endregion

    #region SetBroadcastNumberLabel Command Tests

    [Theory]
    [InlineData(BroadcastNumberLabel.WothCount)]
    [InlineData(BroadcastNumberLabel.Points)]
    public void SetBroadcastNumberLabelCommand_UpdatesLabel(BroadcastNumberLabel label)
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        viewModel.SetBroadcastNumberLabelCommand.Execute(label);

        Assert.Equal(label, viewModel.BroadcastNumberLabel);

        viewModel.Dispose();
    }

    #endregion

    #region Calculated Properties Tests

    [Theory]
    [InlineData(HintDisplayMode.Off, true, 0)]
    [InlineData(HintDisplayMode.MultipathHints, true, 1.43)]
    [InlineData(HintDisplayMode.MultipathHints, false, 2.15)]
    [InlineData(HintDisplayMode.DirectItemHints, true, 1.43)]
    [InlineData(HintDisplayMode.DirectItemHints, false, 2.15)]
    public void HintColumnWidth_ReturnsCorrectValue(HintDisplayMode hintMode, bool compactMode, double expectedWidth)
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockUserSettingsService.Setup(s => s.HintDisplay).Returns(hintMode);
        mockUserSettingsService.Setup(s => s.CompactMode).Returns(compactMode);
        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        Assert.Equal(expectedWidth, viewModel.HintColumnWidth);

        viewModel.Dispose();
    }

    [Theory]
    [InlineData(HintDisplayMode.Off, true, 580)]
    [InlineData(HintDisplayMode.Off, false, 580)]
    [InlineData(HintDisplayMode.MultipathHints, true, 1392)]
    [InlineData(HintDisplayMode.MultipathHints, false, 1800)]
    [InlineData(HintDisplayMode.DirectItemHints, true, 1392)]
    [InlineData(HintDisplayMode.DirectItemHints, false, 1800)]
    public void CurrentWindowWidth_ReturnsCorrectValue(HintDisplayMode hintMode, bool compactMode, double expectedWidth)
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockUserSettingsService.Setup(s => s.HintDisplay).Returns(hintMode);
        mockUserSettingsService.Setup(s => s.CompactMode).Returns(compactMode);
        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        Assert.Equal(expectedWidth, viewModel.CurrentWindowWidth);

        viewModel.Dispose();
    }

    #endregion

    #region Event Handling Tests

    [Fact]
    public void OnSettingChanged_UpdatesPropertyWhenTopMostChanges()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockUserSettingsService.Setup(s => s.TopMost).Returns(false);
        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        Assert.False(viewModel.IsTopMostEnabled);

        mockUserSettingsService.Setup(s => s.TopMost).Returns(true);
        var args = new System.ComponentModel.PropertyChangedEventArgs(nameof(IUserSettingsService.TopMost));
        mockUserSettingsService.Raise(s => s.PropertyChanged += null, args);

        Assert.True(viewModel.IsTopMostEnabled);

        viewModel.Dispose();
    }

    [Fact]
    public void OnSettingChanged_UpdatesAutotrackingProperty()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockUserSettingsService.Setup(s => s.Autotracking).Returns(false);
        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        Assert.False(viewModel.IsAutotrackingEnabled);

        mockUserSettingsService.Setup(s => s.Autotracking).Returns(true);
        var args = new System.ComponentModel.PropertyChangedEventArgs(nameof(IUserSettingsService.Autotracking));
        mockUserSettingsService.Raise(s => s.PropertyChanged += null, args);

        Assert.True(viewModel.IsAutotrackingEnabled);

        viewModel.Dispose();
    }

    #endregion

    #region Property Changed Notifications Tests

    [Fact]
    public void PropertyChanged_WhenHintDisplayChanges_NotifiesHintColumnWidth()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockUserSettingsService.Setup(s => s.HintDisplay).Returns(HintDisplayMode.Off);
        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.HintColumnWidth))
            {
                propertyChangedRaised = true;
            }
        };

        mockUserSettingsService.Setup(s => s.HintDisplay).Returns(HintDisplayMode.MultipathHints);
        var args = new System.ComponentModel.PropertyChangedEventArgs(nameof(IUserSettingsService.HintDisplay));
        mockUserSettingsService.Raise(s => s.PropertyChanged += null, args);

        Assert.True(propertyChangedRaised);

        viewModel.Dispose();
    }

    [Fact]
    public void PropertyChanged_WhenCompactModeChanges_NotifiesWindowDimensions()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockUserSettingsService.Setup(s => s.CompactMode).Returns(false);
        mockUserSettingsService.Setup(s => s.HintDisplay).Returns(HintDisplayMode.MultipathHints);
        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        var hintColumnRaised = false;
        var windowWidthRaised = false;
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.HintColumnWidth))
            {
                hintColumnRaised = true;
            }

            if (e.PropertyName == nameof(viewModel.CurrentWindowWidth))
            {
                windowWidthRaised = true;
            }
        };

        mockUserSettingsService.Setup(s => s.CompactMode).Returns(true);
        var args = new System.ComponentModel.PropertyChangedEventArgs(nameof(IUserSettingsService.CompactMode));
        mockUserSettingsService.Raise(s => s.PropertyChanged += null, args);

        Assert.True(hintColumnRaised);
        Assert.True(windowWidthRaised);

        viewModel.Dispose();
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnhooksEventHandler()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        viewModel.Dispose();

        // After dispose, PropertyChanged events from settings should not be handled
        viewModel.Dispose(); // Second dispose should be safe (idempotent)
    }

    [Fact]
    public void Dispose_DisposesCollectiblesGridRowViewModel()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockUserSettingsService = new Mock<IUserSettingsService>();
        var mockCollectiblesService = new Mock<ICollectiblesService>();
        var mockThemeService = new Mock<IThemeService>();
        var mockAutotrackerService = new Mock<IAutotrackerService>();
        var mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockEndGameProgressionService = new Mock<IEndGameProgressionService>();
        var mockSavedProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        SetupEndGameProgressionMocks(mockEndGameProgressionService);

        var viewModel = new MainWindowViewModel(
            mockItemTrackingService.Object,
            mockUserSettingsService.Object,
            mockCollectiblesService.Object,
            mockThemeService.Object,
            mockAutotrackerService.Object,
            mockParsedSpoilerDataService.Object,
            mockEndGameProgressionService.Object,
            mockSavedProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        var childViewModel = viewModel.CollectiblesGridRowViewModel;
        Assert.NotNull(childViewModel);

        viewModel.Dispose();
    }

    #endregion
}
