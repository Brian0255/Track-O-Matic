namespace TrackOMatic.ViewModels.Test.Broadcast;

using Moq;

using System;

using Xunit;

using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

public class BossSlotViewModelTests
{
    private static Mock<IEndGameProgressionService> SetupEndGameProgressionServiceMock()
    {
        var mock = new Mock<IEndGameProgressionService>();
        mock.Setup(x => x.GetBossKongs()).Returns([0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]);
        return mock;
    }

    private static Mock<IParsedSpoilerDataService> SetupParsedSpoilerDataServiceMock()
    {
        var mock = new Mock<IParsedSpoilerDataService>();
        mock.Setup(x => x.HasBossOrder()).Returns(false);
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
            new BossSlotViewModel(
                0,
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            )
        );

        Assert.NotNull(viewModel);
        Assert.Equal(0, viewModel.BossValue);
        Assert.True(viewModel.ShowsSlot);
    }

    [Fact]
    public void Constructor_WithInvalidSlotNumber_ThrowsArgumentOutOfRangeException()
    {
        // Arrange, Act & Assert
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new BossSlotViewModel(
                    -1,
                    SetupEndGameProgressionServiceMock().Object,
                    SetupParsedSpoilerDataServiceMock().Object,
                    SetupSavedProgressProviderMock().Object,
                    SetupDataSaverServiceMock().Object
                )
            );

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new BossSlotViewModel(
                    13,
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
                new BossSlotViewModel(
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
            new BossSlotViewModel(
                0,
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            )
        );

        // Assert
        endGameProgressionServiceMock.VerifyAdd(x => x.GauntletStateChanged += It.IsAny<EventHandler<GauntletStateChangedEventArgs>>(), Times.Once);
        parsedSpoilerDataServiceMock.VerifyAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
        savedProgressProviderMock.VerifyAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
        dataSaverServiceMock.VerifyAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    #endregion

    #region Event Handling Tests

    [Fact]
    public void OnBossStateChanged_SameContext_UpdatesBossValue()
    {
        // Arrange, Act, and Assert - all in the same UI context
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
            var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
            var savedProgressProviderMock = SetupSavedProgressProviderMock();
            var dataSaverServiceMock = SetupDataSaverServiceMock();

            var viewModel = new BossSlotViewModel(
                3,
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            );

            // Initial value should be 3
            Assert.Equal(3, viewModel.BossValue);

            // Raise event with new boss kongs
            var newBossKongs = new List<int> { 0, 1, 2, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2 };
            endGameProgressionServiceMock.Raise(x => x.GauntletStateChanged += null,
                new GauntletStateChangedEventArgs([], newBossKongs));

            // Assert - the value for slot 3 should now be 5
            Assert.Equal(5, viewModel.BossValue);
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

            var viewModel = new BossSlotViewModel(
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
            Assert.Equal(0, viewModel.BossValue);
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

            var viewModel = new BossSlotViewModel(
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
            Assert.Equal(2, viewModel.BossValue);
        });
    }

    [Fact]
    public void OnDataLoadingCompleted_WithSpoilerAndBossOrder_UpdatesFromSpoilerData()
    {
        // Arrange, Act, and Assert - all in the same UI context
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
            var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
            var savedProgressProviderMock = SetupSavedProgressProviderMock();
            var dataSaverServiceMock = SetupDataSaverServiceMock();

            // Setup spoiler data with boss order
            var spoilerData = new ParsedSpoilerData
            {
                LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
                FinalBossOrder = new List<int> { 1, 2, 3, 4, 5, 6 } // Donkey, Diddy, Lanky, Tiny, Chunky, Army
            };

            parsedSpoilerDataServiceMock.Setup(x => x.HasBossOrder()).Returns(true);
            parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

            var viewModel = new BossSlotViewModel(
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

            // Assert - slot 2 should now have the value from FinalBossOrder[2] = 3 (Lanky)
            Assert.Equal(3, viewModel.BossValue);
            Assert.True(viewModel.ShowsSlot);
        });
    }

    [Fact]
    public void OnDataLoadingCompleted_WithSpoilerSlotBeyondBossCount_HidesSlot()
    {
        // Arrange, Act, and Assert - all in the same UI context
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            var endGameProgressionServiceMock = SetupEndGameProgressionServiceMock();
            var parsedSpoilerDataServiceMock = SetupParsedSpoilerDataServiceMock();
            var savedProgressProviderMock = SetupSavedProgressProviderMock();
            var dataSaverServiceMock = SetupDataSaverServiceMock();

            // Setup spoiler data with only 3 bosses
            var spoilerData = new ParsedSpoilerData
            {
                LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
                FinalBossOrder = new List<int> { 1, 2, 3 } // Only 3 bosses
            };

            parsedSpoilerDataServiceMock.Setup(x => x.HasBossOrder()).Returns(true);
            parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

            var viewModel = new BossSlotViewModel(
                5, // Slot 5 is beyond the 3 bosses
                endGameProgressionServiceMock.Object,
                parsedSpoilerDataServiceMock.Object,
                savedProgressProviderMock.Object,
                dataSaverServiceMock.Object
            );

            // Raise event with spoiler loaded
            var progressWithSpoiler = new SavedProgress { SpoilerPath = "test.spoiler" };
            dataSaverServiceMock.Raise(x => x.DataLoadingCompleted += null,
                new DataLoadingCompletedEventArgs(progressWithSpoiler));

            // Assert - slot 5 should be hidden since there are only 3 bosses
            Assert.Equal(0, viewModel.BossValue);
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

            // Setup spoiler data with UNKNOWN_KONG (value 0) in the boss order
            var spoilerData = new ParsedSpoilerData
            {
                LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
                FinalBossOrder = new List<int> { 0, 2, 3 } // Unknown, Diddy, Lanky
            };

            parsedSpoilerDataServiceMock.Setup(x => x.HasBossOrder()).Returns(true);
            parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

            var viewModel = new BossSlotViewModel(
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
            Assert.Equal(0, viewModel.BossValue);
            Assert.True(viewModel.ShowsSlot);
        });
    }

    #endregion

    #region Object Override Tests

    [Fact]
    public void Equals_WithSameSotNumber_ReturnsTrue()
    {
        // Arrange
        BossSlotViewModel viewModel1 = null!;
        BossSlotViewModel viewModel2 = null!;

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            viewModel1 = new BossSlotViewModel(
                5,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            );

            viewModel2 = new BossSlotViewModel(
                5,
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
        BossSlotViewModel viewModel1 = null!;
        BossSlotViewModel viewModel2 = null!;

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            viewModel1 = new BossSlotViewModel(
                3,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            );

            viewModel2 = new BossSlotViewModel(
                7,
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
            new BossSlotViewModel(
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
    public void GetHashCode_WithSameSlotNumber_ReturnsSameHashCode()
    {
        // Arrange
        BossSlotViewModel viewModel1 = null!;
        BossSlotViewModel viewModel2 = null!;

        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            viewModel1 = new BossSlotViewModel(
                4,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            );

            viewModel2 = new BossSlotViewModel(
                4,
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
    [InlineData(6, "Army 1")]
    [InlineData(7, "Doga 1")]
    [InlineData(8, "Mad Jack")]
    [InlineData(9, "Puftoss")]
    [InlineData(10, "Doga 2")]
    [InlineData(11, "Army 2")]
    [InlineData(12, "Kutout")]
    public void ToString_ReturnsFormattedString(int bossNumber, string expectedName)
    {
        // Arrange
        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new BossSlotViewModel(
                0,
                SetupEndGameProgressionServiceMock().Object,
                SetupParsedSpoilerDataServiceMock().Object,
                SetupSavedProgressProviderMock().Object,
                SetupDataSaverServiceMock().Object
            )
        );

        // Act
        viewModel.BossValue = bossNumber;
        var result = viewModel.ToString();

        // Assert
        Assert.Contains("Broadcast:", result);
        Assert.Contains("1st", result);
        Assert.Contains(expectedName, result);
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
            new BossSlotViewModel(
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
        endGameProgressionServiceMock.VerifyRemove(x => x.GauntletStateChanged -= It.IsAny<EventHandler<GauntletStateChangedEventArgs>>(), Times.Once);
        parsedSpoilerDataServiceMock.VerifyRemove(x => x.ParsedSpoilerDataChanged -= It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
        savedProgressProviderMock.VerifyRemove(x => x.ProgressChanged -= It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
        dataSaverServiceMock.VerifyRemove(x => x.DataLoadingCompleted -= It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var viewModel = SynchronizationContextTestHelper.RunWithUIContext(() =>
            new BossSlotViewModel(
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
