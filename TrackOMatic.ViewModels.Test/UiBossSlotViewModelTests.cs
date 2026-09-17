namespace TrackOMatic.ViewModels.Test;

/// <summary>
/// Test suite for UiBossSlotViewModel.
/// Tests constructor validation, command gate logic, cycling behavior with modulo arithmetic,
/// item selection via dialog, ToString override, and event handling with synchronization context.
/// </summary>
public class UiBossSlotViewModelTests
{
    #region Constructor Tests

    [Theory]
    [InlineData(-1)]
    [InlineData(13)]
    [InlineData(14)]
    public void Constructor_WithInvalidSlotNumber_ThrowsArgumentOutOfRangeException(int invalidSlot)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        // Set up a known initial boss kong state
        mockEndGameService.Setup(s => s.GetBossKongs()).Returns([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new UiBossSlotViewModel(
                invalidSlot,
                mockEndGameService.Object,
                mockSpoilerDataService.Object,
                mockProgressProvider.Object,
                mockItemSelectorService.Object,
                mockDataSaverService.Object
            )
        );

        Assert.Equal("slotNumber", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(12)]
    public void Constructor_WithValidSlotNumber_Succeeds(int validSlot)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetBossKongs()).Returns([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);

        var viewModel = new UiBossSlotViewModel(
            validSlot,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        Assert.NotNull(viewModel);
        viewModel.Dispose();
    }

    [Fact]
    public void Constructor_WithNullEndGameProgressionService_ThrowsArgumentNullException()
    {
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            new UiBossSlotViewModel(
                0,
                null!,
                mockSpoilerDataService.Object,
                mockProgressProvider.Object,
                mockItemSelectorService.Object,
                mockDataSaverService.Object
            )
        );

        Assert.Equal("endGameProgressionService", exception.ParamName);
    }

    [Fact]
    public void Constructor_InitializesPropertiesFromService()
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var bossKongs = new List<int> { 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0 };
        mockEndGameService.Setup(s => s.GetBossKongs()).Returns(bossKongs);

        var viewModel = new UiBossSlotViewModel(
            5,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // BossValue should be 3 (from slot 5)
        Assert.Equal(3, viewModel.BossValue);
        // ImageResourceKey should map to boss value 3 (Lanky)
        Assert.Equal("lanky", viewModel.ImageResourceKey);

        viewModel.Dispose();
    }

    #endregion

    #region CanCycle Gate Tests

    [Fact]
    public void CanCycle_WhenHasBossOrder_ReturnsFalse()
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetBossKongs()).Returns([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasBossOrder()).Returns(true);

        var viewModel = new UiBossSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        Assert.False(viewModel.CycleNextCommand.CanExecute(null));
        Assert.False(viewModel.CyclePreviousCommand.CanExecute(null));

        viewModel.Dispose();
    }

    [Fact]
    public void CanCycle_WhenNoSpoilerData_ReturnsTrue()
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetBossKongs()).Returns([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasBossOrder()).Returns(false);

        var viewModel = new UiBossSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        Assert.True(viewModel.CycleNextCommand.CanExecute(null));
        Assert.True(viewModel.CyclePreviousCommand.CanExecute(null));

        viewModel.Dispose();
    }

    [Fact]
    public void SelectItemCommand_CanExecute_SameAsCanCycle()
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetBossKongs()).Returns([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasBossOrder()).Returns(false);

        var viewModel = new UiBossSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        Assert.True(viewModel.SelectItemCommand.CanExecute((100, 100)));

        viewModel.Dispose();
    }

    #endregion

    #region CycleNext Tests

    [Theory]
    [InlineData(0, 1)]
    [InlineData(5, 6)]
    [InlineData(11, 12)]
    [InlineData(12, 0)] // Wraps around
    public void CycleNext_CyclesValueWithModuloArithmetic(int currentValue, int expectedValue)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var bossKongs = new List<int> { currentValue, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        mockEndGameService.Setup(s => s.GetBossKongs()).Returns(bossKongs);
        mockSpoilerDataService.Setup(s => s.HasBossOrder()).Returns(false);

        var viewModel = new UiBossSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // Execute the command
        viewModel.CycleNextCommand.Execute(null);

        // Verify SetBossKongs was called with updated value
        mockEndGameService.Verify(s => s.SetBossKongs(It.Is<List<int>>(list => list[0] == expectedValue)), Times.Once);

        viewModel.Dispose();
    }

    [Fact]
    public void CycleNext_WhenCannotCycle_DoesNotExecute()
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetBossKongs()).Returns([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasBossOrder()).Returns(true);

        var viewModel = new UiBossSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // Should not execute
        Assert.False(viewModel.CycleNextCommand.CanExecute(null));

        viewModel.Dispose();
    }

    #endregion

    #region CyclePrevious Tests

    [Theory]
    [InlineData(1, 0)]
    [InlineData(5, 4)]
    [InlineData(12, 11)]
    [InlineData(0, 12)] // Wraps around
    public void CyclePrevious_CyclesValueWithModuloArithmetic(int currentValue, int expectedValue)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var bossKongs = new List<int> { currentValue, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        mockEndGameService.Setup(s => s.GetBossKongs()).Returns(bossKongs);
        mockSpoilerDataService.Setup(s => s.HasBossOrder()).Returns(false);

        var viewModel = new UiBossSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // Execute the command
        viewModel.CyclePreviousCommand.Execute(null);

        // Verify SetBossKongs was called with updated value
        mockEndGameService.Verify(s => s.SetBossKongs(It.Is<List<int>>(list => list[0] == expectedValue)), Times.Once);

        viewModel.Dispose();
    }

    [Fact]
    public void CyclePrevious_WhenCannotCycle_DoesNotExecute()
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetBossKongs()).Returns([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasBossOrder()).Returns(true);

        var viewModel = new UiBossSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // Should not execute
        Assert.False(viewModel.CyclePreviousCommand.CanExecute(null));

        viewModel.Dispose();
    }

    #endregion

    #region SelectItem Tests

    [Fact]
    public async Task SelectItemCommand_WhenUserSelectsItem_UpdatesBossValue()
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var bossKongs = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        mockEndGameService.Setup(s => s.GetBossKongs()).Returns(bossKongs);
        mockSpoilerDataService.Setup(s => s.HasBossOrder()).Returns(false);

        // User selects Diddy (index 2 in mappings)
        var selectedCell = new ItemGridCell(0, 1, "diddy");
        mockItemSelectorService
            .Setup(s => s.SelectItemAsync(It.IsAny<IReadOnlyList<ItemGridCell>>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(selectedCell);

        var viewModel = new UiBossSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // Execute the command with screen coordinates
        await viewModel.SelectItemCommand.ExecuteAsync((100.0, 200.0));

        // Verify dialog was called with proper parameters
        mockItemSelectorService.Verify(
            s => s.SelectItemAsync(
                It.IsAny<IReadOnlyList<ItemGridCell>>(),
                "Select Phase/Boss",
                100.0,
                200.0
            ),
            Times.Once
        );

        // Verify SetBossKongs was called with index 2 (Diddy)
        mockEndGameService.Verify(s => s.SetBossKongs(It.Is<List<int>>(list => list[0] == 2)), Times.Once);

        viewModel.Dispose();
    }

    [Fact]
    public async Task SelectItemCommand_WhenUserCancels_DoesNothing()
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetBossKongs()).Returns([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasBossOrder()).Returns(false);

        // User cancels dialog (returns null)
        mockItemSelectorService
            .Setup(s => s.SelectItemAsync(It.IsAny<IReadOnlyList<ItemGridCell>>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((ItemGridCell?)null);

        var viewModel = new UiBossSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // Execute the command
        await viewModel.SelectItemCommand.ExecuteAsync((100.0, 200.0));

        // Verify SetBossKongs was NOT called
        mockEndGameService.Verify(s => s.SetBossKongs(It.IsAny<List<int>>()), Times.Never);

        viewModel.Dispose();
    }

    [Theory]
    [InlineData(2, "diddy")]
    [InlineData(8, "madjack")]
    [InlineData(12, "kutout")]
    public async Task SelectItemCommand_WithVariousItems_CorrectlyIndexesMappings(int expectedIndex, string resourceKey)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetBossKongs()).Returns([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasBossOrder()).Returns(false);

        var selectedCell = new ItemGridCell(0, 0, resourceKey);
        mockItemSelectorService
            .Setup(s => s.SelectItemAsync(It.IsAny<IReadOnlyList<ItemGridCell>>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(selectedCell);

        var viewModel = new UiBossSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        await viewModel.SelectItemCommand.ExecuteAsync((100.0, 200.0));

        mockEndGameService.Verify(s => s.SetBossKongs(It.Is<List<int>>(list => list[0] == expectedIndex)), Times.Once);

        viewModel.Dispose();
    }

    #endregion

    #region ToString Tests

    [Theory]
    [InlineData(0, "The 1st final boss is Unknown.")]
    [InlineData(1, "The 2nd final boss is Donkey Phase.")]
    [InlineData(2, "The 3rd final boss is Diddy Phase.")]
    [InlineData(12, "The 13th final boss is Kutout.")]
    public void ToString_FormatsOutputCorrectly(int slotNumber, string expectedOutput)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var bossKongs = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        // For slot X, use corresponding boss value
        if (slotNumber == 1)
        {
            bossKongs[slotNumber] = 1;
        }
        else if (slotNumber == 2)
        {
            bossKongs[slotNumber] = 2;
        }
        else if (slotNumber == 12)
        {
            bossKongs[slotNumber] = 12;
        }

        mockEndGameService.Setup(s => s.GetBossKongs()).Returns(bossKongs);

        var viewModel = new UiBossSlotViewModel(
            slotNumber,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        var result = viewModel.ToString();

        Assert.Equal(expectedOutput, result);

        viewModel.Dispose();
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnhooksEventHandlers()
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetBossKongs()).Returns([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);

        var viewModel = new UiBossSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        viewModel.Dispose();

        mockEndGameService.VerifyAdd(s => s.GauntletStateChanged += It.IsAny<EventHandler<GauntletStateChangedEventArgs>>(), Times.Once);
        mockSpoilerDataService.VerifyAdd(s => s.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
        mockProgressProvider.VerifyAdd(s => s.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
        mockDataSaverService.VerifyAdd(s => s.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    #endregion
}
