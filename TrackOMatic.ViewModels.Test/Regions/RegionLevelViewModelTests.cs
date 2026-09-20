namespace TrackOMatic.ViewModels.Test.Regions;

public class RegionLevelViewModelTests
{
    private readonly Mock<ILevelOrderService> _levelOrderServiceMock = new();
    private readonly Mock<IUserSettingsService> _userSettingsServiceMock = new();
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock = new();
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock = new();
    private readonly Mock<IDataSaverService> _dataSaverServiceMock = new();

    public RegionLevelViewModelTests()
    {
        // Default setup
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([0, 1, 2, 3, 4, 5, 6, 7]);
        _userSettingsServiceMock.SetupGet(x => x.HelmInLevelOrder).Returns(false);
        _parsedSpoilerDataServiceMock.SetupGet(x => x.CurrentData).Returns((ParsedSpoilerData?)null);
    }

    private RegionLevelViewModel CreateViewModel(
        RegionName regionName = RegionName.JUNGLE_JAPES,
        ILevelOrderService? levelOrderService = null,
        IUserSettingsService? userSettingsService = null,
        IParsedSpoilerDataService? parsedSpoilerDataService = null,
        ISavedProgressProvider? savedProgressProvider = null,
        IDataSaverService? dataSaverService = null
    )
    {
        return new RegionLevelViewModel(
            regionName,
            levelOrderService ?? _levelOrderServiceMock.Object,
            userSettingsService ?? _userSettingsServiceMock.Object,
            parsedSpoilerDataService ?? _parsedSpoilerDataServiceMock.Object,
            savedProgressProvider ?? _savedProgressProviderMock.Object,
            dataSaverService ?? _dataSaverServiceMock.Object
        );
    }

    #region Constructor Validation Tests

    [Fact]
    public void Constructor_WithValidDependencies_Succeeds()
    {
        var vm = CreateViewModel();
        Assert.NotNull(vm);
    }

    [Fact]
    public void Constructor_WithNullLevelOrderService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionLevelViewModel(
                RegionName.JUNGLE_JAPES,
                null!,
                _userSettingsServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("levelOrderService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullUserSettingsService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionLevelViewModel(
                RegionName.JUNGLE_JAPES,
                _levelOrderServiceMock.Object,
                null!,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("userSettingsService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionLevelViewModel(
                RegionName.JUNGLE_JAPES,
                _levelOrderServiceMock.Object,
                _userSettingsServiceMock.Object,
                null!,
                _savedProgressProviderMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionLevelViewModel(
                RegionName.JUNGLE_JAPES,
                _levelOrderServiceMock.Object,
                _userSettingsServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
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
            new RegionLevelViewModel(
                RegionName.JUNGLE_JAPES,
                _levelOrderServiceMock.Object,
                _userSettingsServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                null!
            )
        );
        Assert.Equal("dataSaverService", ex.ParamName);
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void Constructor_InitializesLevelOrderNumber_FromService()
    {
        // Arrange
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([0, 5, 2, 3, 4, 5, 6, 7]);

        // Act
        var vm = CreateViewModel(RegionName.ANGRY_AZTEC);

        // Assert
        Assert.Equal(5, vm.LevelOrderNumber); // Angry Aztec is at index 1 (RegionName - 2)
    }

    [Theory]
    [InlineData(0, "?")]
    [InlineData(1, "1")]
    [InlineData(2, "2")]
    [InlineData(3, "3")]
    [InlineData(4, "4")]
    [InlineData(5, "5")]
    [InlineData(6, "6")]
    [InlineData(7, "7")]
    [InlineData(8, "8")]
    public void Constructor_InitializesLevelText_BasedOnLevelOrderNumber(int levelOrderNumber, string expectedText)
    {
        // Arrange
        var levelOrders = new int[8];
        levelOrders[0] = levelOrderNumber;
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns(levelOrders);

        // Act
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(expectedText, vm.LevelText);
    }

    [Fact]
    public void Constructor_SubscribesToLevelOrderService()
    {
        // Arrange
        EventHandler<LevelOrderStateChangedEventArgs>? capturedHandler = null;
        _levelOrderServiceMock.SetupAdd(x => x.LevelOrderChanged += It.IsAny<EventHandler<LevelOrderStateChangedEventArgs>>())
            .Callback((EventHandler<LevelOrderStateChangedEventArgs> handler) => capturedHandler = handler);

        // Act
        var vm = CreateViewModel();

        // Assert
        Assert.NotNull(capturedHandler);
    }

    [Fact]
    public void Constructor_SubscribesToParsedSpoilerDataService()
    {
        // Arrange
        EventHandler<ParsedSpoilerDataChangedEventArgs>? capturedHandler = null;
        _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
            .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => capturedHandler = handler);

        // Act
        var vm = CreateViewModel();

        // Assert
        Assert.NotNull(capturedHandler);
    }

    [Fact]
    public void Constructor_SubscribesToSavedProgressProvider()
    {
        // Arrange
        EventHandler<ProgressReplacedEventArgs>? capturedHandler = null;
        _savedProgressProviderMock.SetupAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>())
            .Callback((EventHandler<ProgressReplacedEventArgs> handler) => capturedHandler = handler);

        // Act
        var vm = CreateViewModel();

        // Assert
        Assert.NotNull(capturedHandler);
    }

    [Fact]
    public void Constructor_SubscribesToDataSaverService()
    {
        // Arrange
        EventHandler<DataLoadingCompletedEventArgs>? capturedHandler = null;
        _dataSaverServiceMock.SetupAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>())
            .Callback((EventHandler<DataLoadingCompletedEventArgs> handler) => capturedHandler = handler);

        // Act
        var vm = CreateViewModel();

        // Assert
        Assert.NotNull(capturedHandler);
    }

    #endregion

    #region HelmInLevelOrder Special Handling Tests

    [Fact]
    public void Constructor_WithHelmInLevelOrder_DisplaysHelmAtCorrectLevel()
    {
        // Arrange
        _userSettingsServiceMock.SetupGet(x => x.HelmInLevelOrder).Returns(true);
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([0, 1, 2, 3, 4, 5, 6, 5]);

        // Act
        var vm = CreateViewModel(RegionName.HIDEOUT_HELM);

        // Assert
        Assert.Equal(5, vm.LevelOrderNumber);
        Assert.Equal("5", vm.LevelText);
    }

    [Fact]
    public void Constructor_WithoutHelmInLevelOrder_ForcesHelmToLevel8()
    {
        // Arrange
        _userSettingsServiceMock.SetupGet(x => x.HelmInLevelOrder).Returns(false);
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([0, 1, 2, 3, 4, 5, 6, 99]);

        // Act
        var vm = CreateViewModel(RegionName.HIDEOUT_HELM);

        // Assert
        Assert.Equal(8, vm.LevelOrderNumber);
        Assert.Equal("8", vm.LevelText);
    }

    #endregion

    #region Spoiler Data Preference Tests

    [Fact]
    public void UpdateLevelOrderNumber_PrefersSpoiledLevelOrder_OverLocalLevelOrder()
    {
        // Arrange
        var spoiledData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>
            {
                { RegionName.JUNGLE_JAPES, 3 }
            }
        };
        _parsedSpoilerDataServiceMock.SetupGet(x => x.CurrentData).Returns(spoiledData);
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([0, 1, 2, 3, 4, 5, 6, 7]);

        // Act
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(3, vm.LevelOrderNumber);
    }

    #endregion

    #region ToLevelOrderIndex Tests

    [Theory]
    [InlineData(RegionName.JUNGLE_JAPES, 0)]
    [InlineData(RegionName.ANGRY_AZTEC, 1)]
    [InlineData(RegionName.FRANTIC_FACTORY, 2)]
    [InlineData(RegionName.GLOOMY_GALLEON, 3)]
    [InlineData(RegionName.FUNGI_FOREST, 4)]
    [InlineData(RegionName.CRYSTAL_CAVES, 5)]
    [InlineData(RegionName.CREEPY_CASTLE, 6)]
    [InlineData(RegionName.HIDEOUT_HELM, 7)]
    public void ToLevelOrderIndex_ReturnsCorrectIndex(RegionName region, int expectedIndex)
    {
        // Act
        var vm = CreateViewModel(region);

        // Assert
        Assert.Equal(expectedIndex, vm.ToLevelOrderIndex());
    }

    #endregion

    #region Command Tests - IncrementLevelOrder

    [Fact]
    public void IncrementLevelOrderCommand_WithValidRegion_IncrementsBySetting()
    {
        // Arrange
        var levelOrders = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([.. levelOrders]);
        _userSettingsServiceMock.SetupGet(x => x.HelmInLevelOrder).Returns(false);
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Act
        vm.IncrementLevelOrderCommand.Execute(null);

        // Assert
        var expectedLevelOrders = new[] { 1, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Verify(x => x.SetLevelOrder(It.Is<IEnumerable<int>>(orders =>
            orders.SequenceEqual(expectedLevelOrders)
        )), Times.Once);
    }

    [Fact]
    public void IncrementLevelOrderCommand_WhenAtMax_WrapsToZero()
    {
        // Arrange
        var levelOrders = new[] { 7, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([.. levelOrders]);
        _userSettingsServiceMock.SetupGet(x => x.HelmInLevelOrder).Returns(false);
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Act
        vm.IncrementLevelOrderCommand.Execute(null);

        // Assert
        var expectedLevelOrders = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Verify(x => x.SetLevelOrder(It.Is<IEnumerable<int>>(orders =>
            orders.SequenceEqual(expectedLevelOrders)
        )), Times.Once);
    }

    [Fact]
    public void IncrementLevelOrderCommand_WithHelmInLevelOrderEnabled_AllowsIncrementTo8()
    {
        // Arrange
        var levelOrders = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([.. levelOrders]);
        _userSettingsServiceMock.SetupGet(x => x.HelmInLevelOrder).Returns(true);
        var vm = CreateViewModel(RegionName.HIDEOUT_HELM);

        // Act
        vm.IncrementLevelOrderCommand.Execute(null);

        // Assert
        var expectedLevelOrders = new[] { 0, 1, 2, 3, 4, 5, 6, 8 };
        _levelOrderServiceMock.Verify(x => x.SetLevelOrder(It.Is<IEnumerable<int>>(orders =>
            orders.SequenceEqual(expectedLevelOrders)
        )), Times.Once);
    }

    #endregion

    #region Command Tests - DecrementLevelOrder

    [Fact]
    public void DecrementLevelOrderCommand_WithValidRegion_DecrementsBySetting()
    {
        // Arrange
        var levelOrders = new[] { 5, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([.. levelOrders]);
        _userSettingsServiceMock.SetupGet(x => x.HelmInLevelOrder).Returns(false);
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Act
        vm.DecrementLevelOrderCommand.Execute(null);

        // Assert
        var expectedLevelOrders = new[] { 4, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Verify(x => x.SetLevelOrder(It.Is<IEnumerable<int>>(orders =>
            orders.SequenceEqual(expectedLevelOrders)
        )), Times.Once);
    }

    [Fact]
    public void DecrementLevelOrderCommand_WhenAtZero_WrapsToMax()
    {
        // Arrange
        var levelOrders = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([.. levelOrders]);
        _userSettingsServiceMock.SetupGet(x => x.HelmInLevelOrder).Returns(false);
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Act
        vm.DecrementLevelOrderCommand.Execute(null);

        // Assert
        var expectedLevelOrders = new[] { 7, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Verify(x => x.SetLevelOrder(It.Is<IEnumerable<int>>(orders =>
            orders.SequenceEqual(expectedLevelOrders)
        )), Times.Once);
    }

    #endregion

    #region Command Tests - HandleMouseWheel

    [Fact]
    public void HandleMouseWheelCommand_WithPositiveDelta_IncrementsBySetting()
    {
        // Arrange
        var levelOrders = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([.. levelOrders]);
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Act
        vm.HandleMouseWheelCommand.Execute(1);

        // Assert
        var expectedLevelOrders = new[] { 1, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Verify(x => x.SetLevelOrder(It.Is<IEnumerable<int>>(orders =>
            orders.SequenceEqual(expectedLevelOrders)
        )), Times.Once);
    }

    [Fact]
    public void HandleMouseWheelCommand_WithNegativeDelta_DecrementsBySetting()
    {
        // Arrange
        var levelOrders = new[] { 5, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([.. levelOrders]);
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Act
        vm.HandleMouseWheelCommand.Execute(-1);

        // Assert
        var expectedLevelOrders = new[] { 4, 1, 2, 3, 4, 5, 6, 7 };
        _levelOrderServiceMock.Verify(x => x.SetLevelOrder(It.Is<IEnumerable<int>>(orders =>
            orders.SequenceEqual(expectedLevelOrders)
        )), Times.Once);
    }

    #endregion

    #region CanExecute Tests

    [Fact]
    public void CanChangeLevelOrder_WhenSpoilerDataLacksLevelOrder_ReturnsFalse()
    {
        // Arrange
        var spoiledData = new ParsedSpoilerData
        {
            LevelOrder = [] // Empty, HasLevelOrder will be false
        };
        _parsedSpoilerDataServiceMock.SetupGet(x => x.CurrentData).Returns(spoiledData);
        var vm = CreateViewModel();

        // Assert
        Assert.False(vm.IncrementLevelOrderCommand.CanExecute(null));
        Assert.False(vm.DecrementLevelOrderCommand.CanExecute(null));
        Assert.False(vm.HandleMouseWheelCommand.CanExecute(0));
    }

    [Fact]
    public void CanChangeLevelOrder_WithHelmAt8AndNoSetting_ReturnsFalse()
    {
        // Arrange
        var spoiledData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } }
        };
        _parsedSpoilerDataServiceMock.SetupGet(x => x.CurrentData).Returns(spoiledData);
        _userSettingsServiceMock.SetupGet(x => x.HelmInLevelOrder).Returns(false);
        var levelOrders = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns(levelOrders);
        var vm = CreateViewModel(RegionName.HIDEOUT_HELM);

        // Assert
        Assert.False(vm.IncrementLevelOrderCommand.CanExecute(null));
    }

    [Fact]
    public void CanChangeLevelOrder_WithHelmAt8AndSettingEnabled_ReturnsTrue()
    {
        // Arrange
        var spoiledData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } }
        };
        _parsedSpoilerDataServiceMock.SetupGet(x => x.CurrentData).Returns(spoiledData);
        _userSettingsServiceMock.SetupGet(x => x.HelmInLevelOrder).Returns(true);
        var levelOrders = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns(levelOrders);
        var vm = CreateViewModel(RegionName.HIDEOUT_HELM);

        // Assert
        Assert.True(vm.IncrementLevelOrderCommand.CanExecute(null));
    }

    #endregion

    #region Event Handling Tests

    [Fact]
    public void OnLevelOrderChanged_UpdatesLevelOrderNumber()
    {
        // Arrange
        EventHandler<LevelOrderStateChangedEventArgs>? capturedHandler = null;
        _levelOrderServiceMock.SetupAdd(x => x.LevelOrderChanged += It.IsAny<EventHandler<LevelOrderStateChangedEventArgs>>())
            .Callback((EventHandler<LevelOrderStateChangedEventArgs> handler) => capturedHandler = handler);
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([0, 1, 2, 3, 4, 5, 6, 7]);
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Update mock to return new level order
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([3, 1, 2, 3, 4, 5, 6, 7]);

        // Act
        capturedHandler?.Invoke(_levelOrderServiceMock.Object, new LevelOrderStateChangedEventArgs([], [], ChangeReason.Unknown));

        // Assert
        Assert.Equal(3, vm.LevelOrderNumber);
        Assert.Equal("3", vm.LevelText);
    }

    [Fact]
    public void OnDataLoadingCompleted_RefreshesLevelOrderNumber()
    {
        // Arrange
        EventHandler<DataLoadingCompletedEventArgs>? capturedHandler = null;
        _dataSaverServiceMock.SetupAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>())
            .Callback((EventHandler<DataLoadingCompletedEventArgs> handler) => capturedHandler = handler);
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([0, 1, 2, 3, 4, 5, 6, 7]);
        var vm = CreateViewModel();

        // Update mock to return new level order
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([5, 1, 2, 3, 4, 5, 6, 7]);

        // Create a mock SavedProgress for the event
        var savedProgress = new SavedProgress();

        // Act
        capturedHandler?.Invoke(_dataSaverServiceMock.Object, new DataLoadingCompletedEventArgs(savedProgress));

        // Assert
        Assert.Equal(5, vm.LevelOrderNumber);
    }

    #endregion

    #region Equality/Hashing Tests

    [Fact]
    public void Equals_WithSameRegion_ReturnsTrue()
    {
        // Arrange
        var vm1 = CreateViewModel(RegionName.JUNGLE_JAPES);
        var vm2 = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(vm1, vm2);
    }

    [Fact]
    public void Equals_WithDifferentRegion_ReturnsFalse()
    {
        // Arrange
        var vm1 = CreateViewModel(RegionName.JUNGLE_JAPES);
        var vm2 = CreateViewModel(RegionName.ANGRY_AZTEC);

        // Assert
        Assert.NotEqual(vm1, vm2);
    }

    [Fact]
    public void GetHashCode_WithSameRegion_ReturnsSameHash()
    {
        // Arrange
        var vm1 = CreateViewModel(RegionName.JUNGLE_JAPES);
        var vm2 = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(vm1.GetHashCode(), vm2.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        _levelOrderServiceMock.Setup(x => x.GetLevelOrder()).Returns([3, 1, 2, 3, 4, 5, 6, 7]);
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal("JUNGLE_JAPES at Level 3", vm.ToString());
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromLevelOrderService()
    {
        // Arrange
        EventHandler<LevelOrderStateChangedEventArgs>? capturedHandler = null;
        _levelOrderServiceMock.SetupAdd(x => x.LevelOrderChanged += It.IsAny<EventHandler<LevelOrderStateChangedEventArgs>>())
            .Callback((EventHandler<LevelOrderStateChangedEventArgs> handler) => capturedHandler = handler);
        var vm = CreateViewModel();

        // Act
        vm.Dispose();

        // Assert
        _levelOrderServiceMock.VerifyRemove(x => x.LevelOrderChanged -= capturedHandler!, Times.Once);
    }

    [Fact]
    public void Dispose_UnsubscribesFromParsedSpoilerDataService()
    {
        // Arrange
        EventHandler<ParsedSpoilerDataChangedEventArgs>? capturedHandler = null;
        _parsedSpoilerDataServiceMock.SetupAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>())
            .Callback((EventHandler<ParsedSpoilerDataChangedEventArgs> handler) => capturedHandler = handler);
        var vm = CreateViewModel();

        // Act
        vm.Dispose();

        // Assert
        _parsedSpoilerDataServiceMock.VerifyRemove(x => x.ParsedSpoilerDataChanged -= capturedHandler!, Times.Once);
    }

    [Fact]
    public void Dispose_UnsubscribesFromSavedProgressProvider()
    {
        // Arrange
        EventHandler<ProgressReplacedEventArgs>? capturedHandler = null;
        _savedProgressProviderMock.SetupAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>())
            .Callback((EventHandler<ProgressReplacedEventArgs> handler) => capturedHandler = handler);
        var vm = CreateViewModel();

        // Act
        vm.Dispose();

        // Assert
        _savedProgressProviderMock.VerifyRemove(x => x.ProgressChanged -= capturedHandler!, Times.Once);
    }

    [Fact]
    public void Dispose_UnsubscribesFromDataSaverService()
    {
        // Arrange
        EventHandler<DataLoadingCompletedEventArgs>? capturedHandler = null;
        _dataSaverServiceMock.SetupAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>())
            .Callback((EventHandler<DataLoadingCompletedEventArgs> handler) => capturedHandler = handler);
        var vm = CreateViewModel();

        // Act
        vm.Dispose();

        // Assert
        _dataSaverServiceMock.VerifyRemove(x => x.DataLoadingCompleted -= capturedHandler!, Times.Once);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act & Assert
        vm.Dispose();
        vm.Dispose(); // Should not throw
    }

    #endregion

    #region Operator Overload Tests

    [Fact]
    public void EqualityOperator_WithSameRegion_ReturnsTrue()
    {
        // Arrange
        var vm1 = CreateViewModel(RegionName.JUNGLE_JAPES);
        var vm2 = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.True(vm1 == vm2);
    }

    [Fact]
    public void InequalityOperator_WithDifferentRegion_ReturnsTrue()
    {
        // Arrange
        var vm1 = CreateViewModel(RegionName.JUNGLE_JAPES);
        var vm2 = CreateViewModel(RegionName.ANGRY_AZTEC);

        // Assert
        Assert.True(vm1 != vm2);
    }

    [Fact]
    public void EqualityOperator_WithNull_ReturnsFalse()
    {
        // Arrange
        var vm = CreateViewModel(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.NotNull(vm);
    }

    #endregion
}
