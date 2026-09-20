namespace TrackOMatic.ViewModels.Test.Broadcast;

public class HelmSlotViewModelTests
{
    private static Mock<IEndGameProgressionService> SetupEndGameProgressionServiceMock()
    {
        var mock = new Mock<IEndGameProgressionService>();
        mock.Setup(x => x.GetHelmKongs()).Returns([0, 1, 2, 3, 4]);
        return mock;
    }

    private static Mock<IParsedSpoilerDataService> SetupParsedSpoilerDataServiceMock()
    {
        var mock = new Mock<IParsedSpoilerDataService>();
        mock.Setup(x => x.HasHelmOrder()).Returns(false);
        return mock;
    }

    private static Mock<ISavedProgressProvider> SetupSavedProgressProviderMock()
    {
        var mock = new Mock<ISavedProgressProvider>();
        return mock;
    }

    private static Mock<IDataSaverService> SetupDataSaverServiceMock()
    {
        var mock = new Mock<IDataSaverService>();
        return mock;
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_Succeeds()
    {
        // Arrange
        var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
        var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
        var savedProgressProviderMock = SetupSavedProgressProviderMock();
        var dataSaverServiceMock = SetupDataSaverServiceMock();

        // Act & Assert
        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new HelmSlotViewModel(
                0,
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            )
        );

        Assert.NotNull(viewModel);
        Assert.Equal(0, viewModel.HelmValue);
        Assert.True(viewModel.ShowsSlot);
    }

    [Fact]
    public void Constructor_WithInvalidSlotNumber_ThrowsArgumentOutOfRangeException()
    {
        // Arrange, Act & Assert
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new HelmSlotViewModel(
                    -1,
                    SetupEndGameProgressionServiceMock().Object,
                    SetupParsedSpoilerDataServiceMock().Object,
                    SetupSavedProgressProviderMock().Object,
                    SetupDataSaverServiceMock().Object
                )
            );

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new HelmSlotViewModel(
                    5,
                    SetupEndGameProgressionServiceMock().Object,
                    SetupParsedSpoilerDataServiceMock().Object,
                    SetupSavedProgressProviderMock().Object,
                    SetupDataSaverServiceMock().Object
                )
            );
        });
    }

    [Fact]
    public void Constructor_WithNullEndGameProgressionService_ThrowsArgumentNullException()
    {
        // Act & Assert
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new HelmSlotViewModel(
                    0,
                    null!,
                    SetupParsedSpoilerDataServiceMock().Object,
                    SetupSavedProgressProviderMock().Object,
                    SetupDataSaverServiceMock().Object
                )
            );

            Assert.Equal("endGameProgressionService", ex.ParamName);
        });
    }

    [Fact]
    public void Constructor_SubscribesToServices()
    {
        // Arrange
        var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
        var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
        var savedProgressProviderMock = SetupSavedProgressProviderMock();
        var dataSaverServiceMock = SetupDataSaverServiceMock();

        // Act
        SynchronizationContextTestHelper.RunWithUIContext(() =>
            new HelmSlotViewModel(
                0,
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            )
        );

        // Assert
        endGameProgressionServiceMock.VerifyAdd(x => x.BlastStateChanged += It.IsAny<EventHandler<BlastStateChangedEventArgs>>(), Times.Once);
        parsedSpoilerDataServiceMock.VerifyAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
        savedProgressProviderMock.VerifyAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
        dataSaverServiceMock.VerifyAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    #endregion

    #region Event Handling Tests

    [Fact]
    public void OnBlastStateChanged_SameContext_UpdatesHelmValue()
    {
        // Arrange, Act, and Assert - all in the same UI context
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
            var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
            var savedProgressProviderMock = SetupSavedProgressProviderMock();
            var dataSaverServiceMock = SetupDataSaverServiceMock();

            var viewModel = new HelmSlotViewModel(
                2,
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            );

            // Initial value should be 2
            Assert.Equal(2, viewModel.HelmValue);

            // Raise event with new helm kongs
            var newHelmKongs = new List<int> { 0, 5, 4, 3, 1 };
            endGameProgressionServiceMock.Raise(x => x.BlastStateChanged += null,
                new BlastStateChangedEventArgs([], newHelmKongs));

            // Assert - the value for slot 2 should now be 4
            Assert.Equal(4, viewModel.HelmValue);
        });
    }

    [Fact]
    public void OnSpoilerDataChanged_WithNoData_ResetsValues()
    {
        // Arrange, Act, and Assert - all in the same UI context
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
            var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
            var savedProgressProviderMock = SetupSavedProgressProviderMock();
            var dataSaverServiceMock = SetupDataSaverServiceMock();

            var viewModel = new HelmSlotViewModel(
                1,
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            );

            // Raise event with no data
            parsedSpoilerDataServiceMock.Raise(x => x.ParsedSpoilerDataChanged += null,
                new ParsedSpoilerDataChangedEventArgs(null, null));

            // Assert
            Assert.Equal(0, viewModel.HelmValue);
            Assert.True(viewModel.ShowsSlot);
        });
    }

    [Fact]
    public void OnDataLoadingCompleted_WithoutSpoiler_ReinitializesState()
    {
        // Arrange, Act, and Assert - all in the same UI context
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
            var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
            var savedProgressProviderMock = SetupSavedProgressProviderMock();
            var dataSaverServiceMock = SetupDataSaverServiceMock();

            var viewModel = new HelmSlotViewModel(
                2,
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            );

            // Raise event with no spoiler
            var mockProgress = new SavedProgress();
            dataSaverServiceMock.Raise(x => x.DataLoadingCompleted += null,
                new DataLoadingCompletedEventArgs(mockProgress));

            // Assert - should maintain initial state
            Assert.Equal(2, viewModel.HelmValue);
        });
    }

    [Fact]
    public void OnDataLoadingCompleted_WithSpoilerAndHelmOrder_UpdatesFromSpoilerData()
    {
        // Arrange, Act, and Assert - all in the same UI context
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
            var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
            var savedProgressProviderMock = SetupSavedProgressProviderMock();
            var dataSaverServiceMock = SetupDataSaverServiceMock();

            // Setup spoiler data with helm order
            var spoilerData = new ParsedSpoilerData
            {
                LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
                HelmOrder = [5, 3, 1, 2, 4] // Chunky, Lanky, Donkey, Diddy, Tiny
            };

            parsedSpoilerDataServiceMock.Setup(x => x.HasHelmOrder()).Returns(true);
            parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

            var viewModel = new HelmSlotViewModel(
                2,
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            );

            // Raise event with spoiler loaded (simulating SpoilerPath being set)
            var progressWithSpoiler = new SavedProgress { SpoilerPath = "test.spoiler" };
            dataSaverServiceMock.Raise(x => x.DataLoadingCompleted += null,
                new DataLoadingCompletedEventArgs(progressWithSpoiler));

            // Assert - slot 2 should now have the value from HelmOrder[2] = 1 (Donkey)
            Assert.Equal(1, viewModel.HelmValue);
            Assert.True(viewModel.ShowsSlot);
        });
    }

    [Fact]
    public void OnDataLoadingCompleted_WithSpoilerSlotBeyondHelmCount_HidesSlot()
    {
        // Arrange, Act, and Assert - all in the same UI context
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
            var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
            var savedProgressProviderMock = SetupSavedProgressProviderMock();
            var dataSaverServiceMock = SetupDataSaverServiceMock();

            // Setup spoiler data with only 2 helm kongs
            var spoilerData = new ParsedSpoilerData
            {
                LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
                HelmOrder = [1, 2] // Only 2 kongs
            };

            parsedSpoilerDataServiceMock.Setup(x => x.HasHelmOrder()).Returns(true);
            parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

            var viewModel = new HelmSlotViewModel(
                3, // Slot 3 is beyond the 2 kongs
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            );

            // Raise event with spoiler loaded
            var progressWithSpoiler = new SavedProgress { SpoilerPath = "test.spoiler" };
            dataSaverServiceMock.Raise(x => x.DataLoadingCompleted += null,
                new DataLoadingCompletedEventArgs(progressWithSpoiler));

            // Assert - slot 3 should be hidden since there are only 2 kongs
            Assert.Equal(0, viewModel.HelmValue);
            Assert.False(viewModel.ShowsSlot);
        });
    }

    [Fact]
    public void OnDataLoadingCompleted_WithSpoilerAndUnknownKong_ShowsUnknownSlot()
    {
        // Arrange, Act, and Assert - all in the same UI context
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
            var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
            var savedProgressProviderMock = SetupSavedProgressProviderMock();
            var dataSaverServiceMock = SetupDataSaverServiceMock();

            // Setup spoiler data with UNKNOWN_KONG (value 0) in the helm order
            var spoilerData = new ParsedSpoilerData
            {
                LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
                HelmOrder = [0, 2, 3] // Unknown Kong, Diddy, Lanky
            };

            parsedSpoilerDataServiceMock.Setup(x => x.HasHelmOrder()).Returns(true);
            parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

            var viewModel = new HelmSlotViewModel(
                0, // First slot has UNKNOWN_KONG
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            );

            // Raise event with spoiler loaded
            var progressWithSpoiler = new SavedProgress { SpoilerPath = "test.spoiler" };
            dataSaverServiceMock.Raise(x => x.DataLoadingCompleted += null,
                new DataLoadingCompletedEventArgs(progressWithSpoiler));

            // Assert - slot 0 should show UNKNOWN_KONG (value 0)
            Assert.Equal(0, viewModel.HelmValue);
            Assert.True(viewModel.ShowsSlot);
        });
    }

    #endregion

    #region Object Override Tests

    [Fact]
    public void Equals_WithSameSlotNumber_ReturnsTrue()
    {
        // Arrange
        HelmSlotViewModel viewModel1 = null!;
        HelmSlotViewModel viewModel2 = null!;

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            viewModel1 = new HelmSlotViewModel(
                3,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            );

            viewModel2 = new HelmSlotViewModel(
                3,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            );
        });

        // Act & Assert
        Assert.True(viewModel1.Equals((object)viewModel2));
    }

    [Fact]
    public void Equals_WithDifferentSlotNumber_ReturnsFalse()
    {
        // Arrange
        HelmSlotViewModel viewModel1 = null!;
        HelmSlotViewModel viewModel2 = null!;

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            viewModel1 = new HelmSlotViewModel(
                1,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            );

            viewModel2 = new HelmSlotViewModel(
                3,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            );
        });

        // Act & Assert
        Assert.False(viewModel1.Equals(viewModel2));
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new HelmSlotViewModel(
                0,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            )
        );

        // Act & Assert
        Assert.False(viewModel.Equals(null));
    }

    [Fact]
    public void Equals_ReflexiveEquality_ReturnsTrueForSameInstance()
    {
        // Arrange
        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new HelmSlotViewModel(
                0,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            )
        );

        // Act & Assert - verifies ReferenceEquals path
        Assert.True(viewModel.Equals(viewModel));
    }

    [Fact]
    public void GetHashCode_WithSameSlotNumber_ReturnsSameHashCode()
    {
        // Arrange
        HelmSlotViewModel viewModel1 = null!;
        HelmSlotViewModel viewModel2 = null!;

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            viewModel1 = new HelmSlotViewModel(
                2,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            );

            viewModel2 = new HelmSlotViewModel(
                2,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            );
        });

        // Act & Assert
        Assert.Equal(viewModel1.GetHashCode(), viewModel2.GetHashCode());
    }

    [Theory]
    [InlineData(0, "Unknown")]
    [InlineData(1, "Donkey")]
    [InlineData(2, "Diddy")]
    [InlineData(3, "Lanky")]
    [InlineData(4, "Tiny")]
    [InlineData(5, "Chunky")]
    public void ToString_ReturnsFormattedString(int helmNumber, string expectedName)
    {
        // Arrange
        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new HelmSlotViewModel(
                0,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            )
        );

        // Act
        viewModel.HelmValue = helmNumber;
        var result = viewModel.ToString();

        // Assert
        Assert.Contains("Broadcast:", result);
        Assert.Contains("1st", result);
        Assert.Contains(expectedName, result);
        Assert.Contains("Blast-O-Matic", result);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromAllServices()
    {
        // Arrange
        var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
        var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
        var savedProgressProviderMock = SetupSavedProgressProviderMock();
        var dataSaverServiceMock = SetupDataSaverServiceMock();

        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new HelmSlotViewModel(
                0,
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            )
        );

        // Act
        viewModel.Dispose();

        // Assert
        endGameProgressionServiceMock.VerifyRemove(x => x.BlastStateChanged -= It.IsAny<EventHandler<BlastStateChangedEventArgs>>(), Times.Once);
        parsedSpoilerDataServiceMock.VerifyRemove(x => x.ParsedSpoilerDataChanged -= It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
        savedProgressProviderMock.VerifyRemove(x => x.ProgressChanged -= It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
        dataSaverServiceMock.VerifyRemove(x => x.DataLoadingCompleted -= It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new HelmSlotViewModel(
                0,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            )
        );

        // Act & Assert - should not throw
        viewModel.Dispose();
        viewModel.Dispose();
        viewModel.Dispose();
    }

    #endregion
}
