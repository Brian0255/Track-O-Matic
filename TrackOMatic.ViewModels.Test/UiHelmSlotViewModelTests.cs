namespace TrackOMatic.ViewModels.Test;

/// <summary>
/// Test suite for UiHelmSlotViewModel.
/// Tests constructor validation, command gate logic, cycling behavior with modulo arithmetic (0-5),
/// item selection via dialog, ToString override, and event handling with synchronization context.
/// </summary>
public class UiHelmSlotViewModelTests
{
    #region Constructor Tests

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    [InlineData(6)]
    public void Constructor_WithInvalidSlotNumber_ThrowsArgumentOutOfRangeException(int invalidSlot)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        // Set up a known initial helm kong state
        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns([0, 0, 0, 0, 0]);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new UiHelmSlotViewModel(
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
    [InlineData(2)]
    [InlineData(4)]
    public void Constructor_WithValidSlotNumber_Succeeds(int validSlot)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns([0, 0, 0, 0, 0]);

        var viewModel = new UiHelmSlotViewModel(
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
            new UiHelmSlotViewModel(
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

        var helmKongs = new List<int> { 0, 0, 2, 0, 0 };
        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns(helmKongs);

        var viewModel = new UiHelmSlotViewModel(
            2,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // HelmValue should be 2 (from slot 2)
        Assert.Equal(2, viewModel.HelmValue);
        // ImageResourceKey should map to helm value 2 (Diddy)
        Assert.Equal("diddy", viewModel.ImageResourceKey);

        viewModel.Dispose();
    }

    #endregion

    #region CanCycle Gate Tests

    [Fact]
    public void CanCycle_WhenHasHelmOrder_ReturnsFalse()
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns([0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasHelmOrder()).Returns(true);

        var viewModel = new UiHelmSlotViewModel(
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

        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns([0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasHelmOrder()).Returns(false);

        var viewModel = new UiHelmSlotViewModel(
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

        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns([0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasHelmOrder()).Returns(false);

        var viewModel = new UiHelmSlotViewModel(
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
    [InlineData(2, 3)]
    [InlineData(4, 5)]
    [InlineData(5, 0)] // Wraps around (modulo 6)
    public void CycleNext_CyclesValueWithModuloArithmetic(int currentValue, int expectedValue)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var helmKongs = new List<int> { currentValue, 0, 0, 0, 0 };
        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns(helmKongs);
        mockSpoilerDataService.Setup(s => s.HasHelmOrder()).Returns(false);

        var viewModel = new UiHelmSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // Execute the command
        viewModel.CycleNextCommand.Execute(null);

        // Verify SetHelmKongs was called with updated value
        mockEndGameService.Verify(s => s.SetHelmKongs(It.Is<List<int>>(list => list[0] == expectedValue)), Times.Once);

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

        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns([0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasHelmOrder()).Returns(true);

        var viewModel = new UiHelmSlotViewModel(
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
    [InlineData(3, 2)]
    [InlineData(5, 4)]
    [InlineData(0, 5)] // Wraps around (modulo 6)
    public void CyclePrevious_CyclesValueWithModuloArithmetic(int currentValue, int expectedValue)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var helmKongs = new List<int> { currentValue, 0, 0, 0, 0 };
        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns(helmKongs);
        mockSpoilerDataService.Setup(s => s.HasHelmOrder()).Returns(false);

        var viewModel = new UiHelmSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // Execute the command
        viewModel.CyclePreviousCommand.Execute(null);

        // Verify SetHelmKongs was called with updated value
        mockEndGameService.Verify(s => s.SetHelmKongs(It.Is<List<int>>(list => list[0] == expectedValue)), Times.Once);

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

        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns([0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasHelmOrder()).Returns(true);

        var viewModel = new UiHelmSlotViewModel(
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
    public async Task SelectItemCommand_WhenUserSelectsItem_UpdatesHelmValue()
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var helmKongs = new List<int> { 0, 0, 0, 0, 0 };
        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns(helmKongs);
        mockSpoilerDataService.Setup(s => s.HasHelmOrder()).Returns(false);

        // User selects Lanky (index 3 in mappings)
        var selectedCell = new ItemGridCell(0, 3, "lanky");
        mockItemSelectorService
            .Setup(s => s.SelectItemAsync(It.IsAny<IReadOnlyList<ItemGridCell>>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(selectedCell);

        var viewModel = new UiHelmSlotViewModel(
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
                "Select Kong",
                100.0,
                200.0
            ),
            Times.Once
        );

        // Verify SetHelmKongs was called with index 3 (Lanky)
        mockEndGameService.Verify(s => s.SetHelmKongs(It.Is<List<int>>(list => list[0] == 3)), Times.Once);

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

        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns([0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasHelmOrder()).Returns(false);

        // User cancels dialog (returns null)
        mockItemSelectorService
            .Setup(s => s.SelectItemAsync(It.IsAny<IReadOnlyList<ItemGridCell>>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync((ItemGridCell?)null);

        var viewModel = new UiHelmSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        // Execute the command
        await viewModel.SelectItemCommand.ExecuteAsync((100.0, 200.0));

        // Verify SetHelmKongs was NOT called
        mockEndGameService.Verify(s => s.SetHelmKongs(It.IsAny<List<int>>()), Times.Never);

        viewModel.Dispose();
    }

    [Theory]
    [InlineData(1, "donkey")]
    [InlineData(3, "lanky")]
    [InlineData(5, "chunky")]
    public async Task SelectItemCommand_WithVariousItems_CorrectlyIndexesMappings(int expectedIndex, string resourceKey)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns([0, 0, 0, 0, 0]);
        mockSpoilerDataService.Setup(s => s.HasHelmOrder()).Returns(false);

        var selectedCell = new ItemGridCell(0, 0, resourceKey);
        mockItemSelectorService
            .Setup(s => s.SelectItemAsync(It.IsAny<IReadOnlyList<ItemGridCell>>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
            .ReturnsAsync(selectedCell);

        var viewModel = new UiHelmSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        await viewModel.SelectItemCommand.ExecuteAsync((100.0, 200.0));

        mockEndGameService.Verify(s => s.SetHelmKongs(It.Is<List<int>>(list => list[0] == expectedIndex)), Times.Once);

        viewModel.Dispose();
    }

    #endregion

    #region ToString Tests

    [Theory]
    [InlineData(0, "The 1st Kong to destroy the Blast-O-Matic is Unknown.")]
    [InlineData(1, "The 2nd Kong to destroy the Blast-O-Matic is Donkey.")]
    [InlineData(2, "The 3rd Kong to destroy the Blast-O-Matic is Diddy.")]
    [InlineData(4, "The 5th Kong to destroy the Blast-O-Matic is Tiny.")]
    public void ToString_FormatsOutputCorrectly(int slotNumber, string expectedOutput)
    {
        var mockEndGameService = new Mock<IEndGameProgressionService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockItemSelectorService = new Mock<IItemSelectorService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var helmKongs = new List<int> { 0, 0, 0, 0, 0 };
        // For slot X, use corresponding helm value
        if (slotNumber == 1)
        {
            helmKongs[slotNumber] = 1;
        }
        else if (slotNumber == 2)
        {
            helmKongs[slotNumber] = 2;
        }
        else if (slotNumber == 4)
        {
            helmKongs[slotNumber] = 4;
        }

        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns(helmKongs);

        var viewModel = new UiHelmSlotViewModel(
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

        mockEndGameService.Setup(s => s.GetHelmKongs()).Returns([0, 0, 0, 0, 0]);

        var viewModel = new UiHelmSlotViewModel(
            0,
            mockEndGameService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockItemSelectorService.Object,
            mockDataSaverService.Object
        );

        viewModel.Dispose();

        mockEndGameService.VerifyAdd(s => s.BlastStateChanged += It.IsAny<EventHandler<BlastStateChangedEventArgs>>(), Times.Once);
        mockSpoilerDataService.VerifyAdd(s => s.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
        mockProgressProvider.VerifyAdd(s => s.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
        mockDataSaverService.VerifyAdd(s => s.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    #endregion
}
