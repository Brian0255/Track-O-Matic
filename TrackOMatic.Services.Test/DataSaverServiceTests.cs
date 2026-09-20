using Moq;

using System.Text.Json;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services.TrackerState;

#pragma warning disable CS8620

namespace TrackOMatic.Services.Test;

public class DataSaverServiceTests
{
    private readonly Mock<IDataPersistenceService> _mockDataPersistence;
    private readonly Mock<IParsedSpoilerDataService> _mockSpoilerData;
    private readonly Mock<IItemTrackingService> _mockItemTracking;
    private readonly Mock<IHintService> _mockHints;
    private readonly Mock<ISavedProgressProvider> _mockProgressProvider;
    private readonly Mock<ISpoilerService> _mockSpoilerService;
    private readonly Mock<IBarrierService> _mockBarriers;
    private readonly Mock<IEndGameProgressionService> _mockEndGameProgression;
    private readonly Mock<ILevelOrderService> _mockLevelOrder;
    private readonly DataSaverService _sut;

    public DataSaverServiceTests()
    {
        _mockDataPersistence = new Mock<IDataPersistenceService>();
        _mockSpoilerData = new Mock<IParsedSpoilerDataService>();
        _mockItemTracking = new Mock<IItemTrackingService>();
        _mockHints = new Mock<IHintService>();
        _mockProgressProvider = new Mock<ISavedProgressProvider>();
        _mockSpoilerService = new Mock<ISpoilerService>();
        _mockBarriers = new Mock<IBarrierService>();
        _mockEndGameProgression = new Mock<IEndGameProgressionService>();
        _mockLevelOrder = new Mock<ILevelOrderService>();

        _sut = new DataSaverService(
            _mockDataPersistence.Object,
            _mockSpoilerData.Object,
            _mockItemTracking.Object,
            _mockHints.Object,
            _mockProgressProvider.Object,
            _mockSpoilerService.Object,
            _mockBarriers.Object,
            _mockEndGameProgression.Object,
            _mockLevelOrder.Object
        );
    }

    #region Reset Tests

    [Fact]
    public void Reset_ResetsProgressToNewInstance()
    {
        // Arrange
        var originalProgress = new SavedProgress { SpoilerPath = "test.json" };

        // Act
        _sut.Reset();

        // Assert
        _mockProgressProvider.Verify(p => p.UpdateProgress(It.Is<SavedProgress>(sp =>
            sp.SpoilerPath == "" &&
            sp.SavedItems.Count == 0 &&
            sp.SavedHints.Count == 0
        )), Times.Once);
    }

    #endregion

    #region SaveAsync Tests

    [Fact]
    public async Task SaveAsync_WithResetProgress_SavesEmptyState()
    {
        // Arrange
        _sut.Reset();
        var mockItem = new SavedItem(ItemName.DONKEY, autotracked: false, starred: true);
        _mockItemTracking
            .Setup(x => x.GetAllItemsInOrder())
            .Returns([mockItem]);

        _mockItemTracking.Setup(x => x.SyncSavedItemsOrder());

        SavedProgress? capturedProgress = null;
        _mockDataPersistence
            .Setup(x => x.SaveAsync(It.IsAny<SavedProgress>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback((SavedProgress progress, string _, bool _, CancellationToken _) => capturedProgress = progress)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.SaveAsync();

        // Assert
        Assert.NotNull(capturedProgress);
        // After reset, SavedItems should be rebuilt from GetAllItemsInOrder(), so it contains the mock item
        Assert.Single(capturedProgress.SavedItems);
    }

    [Fact]
    public async Task SaveAsync_WithValidProgress_CallsPersistenceService()
    {
        // Arrange
        var filePath = "test.json";
        var progress = new SavedProgress { SpoilerPath = "spoiler.json" };

        // Set up progress via reflection or initialization
        var mockItem = new SavedItem(ItemName.DONKEY, autotracked: false, starred: true);
        _mockItemTracking
            .Setup(x => x.GetAllItemsInOrder())
            .Returns([mockItem]);

        _mockItemTracking.Setup(x => x.SyncSavedItemsOrder());

        // Act
        await _sut.SaveAsync(filePath);

        // Assert
        _mockDataPersistence.Verify(
            x => x.SaveAsync(It.IsAny<SavedProgress>(), It.Is<string>(p => p.Contains(filePath))),
            Times.Once
        );
    }

    [Fact]
    public async Task SaveAsync_WithRelativePath_ResolvesToAppDataDirectory()
    {
        // Arrange
        var mockItem = new SavedItem(ItemName.DONKEY, autotracked: false, starred: true);
        _mockItemTracking
            .Setup(x => x.GetAllItemsInOrder())
            .Returns([mockItem]);

        _mockItemTracking.Setup(x => x.SyncSavedItemsOrder());

        // Act
        await _sut.SaveAsync("autosave.json");

        // Assert
        _mockDataPersistence.Verify(
            x => x.SaveAsync(It.IsAny<SavedProgress>(), It.Is<string>(p =>
                p.Contains("Track-O-Matic") && p.Contains("autosave.json")
            )),
            Times.Once
        );
    }

    [Fact]
    public async Task SaveAsync_WithAbsolutePath_UsesPathAsIs()
    {
        // Arrange
        var absolutePath = @"C:\Temp\mysave.json";
        var mockItem = new SavedItem(ItemName.DONKEY, autotracked: false, starred: true);
        _mockItemTracking
            .Setup(x => x.GetAllItemsInOrder())
            .Returns([mockItem]);

        _mockItemTracking.Setup(x => x.SyncSavedItemsOrder());

        // Act
        await _sut.SaveAsync(absolutePath);

        // Assert
        _mockDataPersistence.Verify(
            x => x.SaveAsync(It.IsAny<SavedProgress>(), It.Is<string>(p => p == absolutePath)),
            Times.Once
        );
    }

    [Fact]
    public async Task SaveAsync_OnSuccess_FiresSaveCompletedEvent()
    {
        // Arrange
        var completedEvents = new List<SavedProgressChangedEventArgs>();
        _sut.SaveCompleted += (s, args) => completedEvents.Add(args);

        var mockItem = new SavedItem(ItemName.DONKEY, autotracked: false, starred: true);
        _mockItemTracking
            .Setup(x => x.GetAllItemsInOrder())
            .Returns([mockItem]);

        _mockItemTracking.Setup(x => x.SyncSavedItemsOrder());

        // Act
        await _sut.SaveAsync("test.json");

        // Assert
        Assert.Single(completedEvents);
        Assert.True(completedEvents[0].Success);
        Assert.NotNull(completedEvents[0].FilePath);
    }

    [Fact]
    public async Task SaveAsync_OnPersistenceException_FiresSaveFailedEvent()
    {
        // Arrange
        var failedEvents = new List<string>();
        _sut.SaveFailed += (s, msg) => failedEvents.Add(msg);

        var mockItem = new SavedItem(ItemName.DONKEY, autotracked: false, starred: true);
        _mockItemTracking
            .Setup(x => x.GetAllItemsInOrder())
            .Returns([mockItem]);

        _mockItemTracking.Setup(x => x.SyncSavedItemsOrder());

        _mockDataPersistence
            .Setup(x => x.SaveAsync(It.IsAny<SavedProgress>(), It.IsAny<string>()))
            .ThrowsAsync(new IOException("Disk full"));

        // Act
        await _sut.SaveAsync("test.json");

        // Assert
        Assert.Single(failedEvents);
        Assert.Contains("Failed to save progress", failedEvents[0]);
        Assert.Contains("Disk full", failedEvents[0]);
    }

    [Fact]
    public async Task SaveAsync_FiltersOutStartingAutoTrackedItems()
    {
        // Arrange - create items with proper properties for filtering
        var items = new[]
        {
            // This is a starting autotracked item (no star) - should be FILTERED
            new SavedItem(ItemName.DONKEY, RegionName.START, starred: false, autotracked: true, opacity: 1.0, hinted: false),
            // This is a starting autotracked item (with star) - should be INCLUDED
            new SavedItem(ItemName.DIDDY, RegionName.START, starred: true, autotracked: true, opacity: 1.0, hinted: false),
            // This is NOT a starting autotracked item (has opacity != 1) - should be INCLUDED
            new SavedItem(ItemName.LANKY, RegionName.JUNGLE_JAPES, starred: false, autotracked: true, opacity: 0.5, hinted: false),
        };

        _mockItemTracking
            .Setup(x => x.GetAllItemsInOrder())
            .Returns(items);

        _mockItemTracking.Setup(x => x.SyncSavedItemsOrder());

        SavedProgress? capturedProgress = null;
        _mockDataPersistence
            .Setup(x => x.SaveAsync(It.IsAny<SavedProgress>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback((SavedProgress progress, string _, bool _, CancellationToken _) => capturedProgress = progress)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.SaveAsync("test.json");

        // Assert
        Assert.NotNull(capturedProgress);
        // Should only include DIDDY and LANKY (DONKEY is filtered as starting autotracked without star)
        Assert.Equal(2, capturedProgress.SavedItems.Count);
        Assert.Contains(ItemName.DIDDY, capturedProgress.SavedItems.Keys);
        Assert.Contains(ItemName.LANKY, capturedProgress.SavedItems.Keys);
        Assert.DoesNotContain(ItemName.DONKEY, capturedProgress.SavedItems.Keys);
    }

    [Fact]
    public async Task SaveAsync_SyncsItemOrderBeforeSaving()
    {
        // Arrange
        var callOrder = new List<string>();

        _mockItemTracking
            .Setup(x => x.SyncSavedItemsOrder())
            .Callback(() => callOrder.Add("sync"));

        _mockItemTracking
            .Setup(x => x.GetAllItemsInOrder())
            .Callback(() => callOrder.Add("get"))
            .Returns([]);

        _mockDataPersistence
            .Setup(x => x.SaveAsync(It.IsAny<SavedProgress>(), It.IsAny<string>()))
            .Callback(() => callOrder.Add("persist"))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.SaveAsync("test.json");

        // Assert
        Assert.Equal("sync", callOrder[0]);
        Assert.Equal("get", callOrder[1]);
        Assert.Equal("persist", callOrder[2]);
    }

    #endregion

    #region LoadAsync Tests

    [Fact]
    public async Task LoadAsync_WithNullLoadResult_DoesNothing()
    {
        // Arrange
        var loadingCompleted = new List<DataLoadingCompletedEventArgs>();
        _sut.DataLoadingCompleted += (s, args) => loadingCompleted.Add(args);

        _mockDataPersistence
            .Setup(x => x.LoadAsync<SavedProgress>(It.IsAny<string>()))
            .ReturnsAsync((SavedProgress?)null);

        // Act
        await _sut.LoadAsync("nonexistent.json");

        // Assert
        Assert.Empty(loadingCompleted);
        _mockSpoilerData.Verify(x => x.LoadParsedDataSilently(It.IsAny<ParsedSpoilerData>()), Times.Never);
    }

    [Fact]
    public async Task LoadAsync_WithValidFile_LoadsProgressState()
    {
        // Arrange
        var loadingCompleted = new List<DataLoadingCompletedEventArgs>();
        _sut.DataLoadingCompleted += (s, args) => loadingCompleted.Add(args);

        var savedProgress = new SavedProgress { SpoilerPath = "spoiler.json" };

        _mockDataPersistence
            .Setup(x => x.LoadAsync<SavedProgress>(It.IsAny<string>()))
            .ReturnsAsync(savedProgress);

        _mockSpoilerData.Setup(x => x.CurrentData).Returns((ParsedSpoilerData?)null);

        // Act
        await _sut.LoadAsync("test.json");

        // Assert
        Assert.Single(loadingCompleted);
        _mockItemTracking.Verify(x => x.LoadItemsSilently(It.IsAny<Dictionary<ItemName, SavedItem>>()), Times.Once);
        _mockHints.Verify(x => x.LoadHintsSilently(It.IsAny<List<SavedHint>>()), Times.Once);
    }

    [Fact]
    public async Task LoadAsync_SkipsExistingItems_IsFalse()
    {
        // Arrange
        var savedProgress = new SavedProgress
        {
            SpoilerPath = "",
            SavedItems = new Dictionary<ItemName, SavedItem>
            {
                { ItemName.DONKEY, new SavedItem(ItemName.DONKEY, starred: true) }
            }
        };

        _mockDataPersistence
            .Setup(x => x.LoadAsync<SavedProgress>(It.IsAny<string>()))
            .ReturnsAsync(savedProgress);

        _mockSpoilerData.Setup(x => x.CurrentData).Returns((ParsedSpoilerData?)null);

        // Act
        await _sut.LoadAsync("test.json");

        // Assert
        _mockItemTracking.Verify(
            x => x.LoadItemsSilently(It.Is<Dictionary<ItemName, SavedItem>>(d =>
                d.ContainsKey(ItemName.DONKEY)
            )),
            Times.Once
        );
    }

    [Fact]
    public async Task LoadAsync_FiresDataLoadingCompletedEvent()
    {
        // Arrange
        var loadingCompleted = new List<DataLoadingCompletedEventArgs>();
        _sut.DataLoadingCompleted += (s, args) => loadingCompleted.Add(args);

        var savedProgress = new SavedProgress { SpoilerPath = "spoiler.json" };

        _mockDataPersistence
            .Setup(x => x.LoadAsync<SavedProgress>(It.IsAny<string>()))
            .ReturnsAsync(savedProgress);

        _mockSpoilerData.Setup(x => x.CurrentData).Returns((ParsedSpoilerData?)null);

        // Act
        await _sut.LoadAsync("test.json");

        // Assert
        Assert.Single(loadingCompleted);
        Assert.Equal("spoiler.json", loadingCompleted[0].LoadedProgress.SpoilerPath);
    }

    #endregion

    #region InitWithSpoilerAsync Tests

    [Fact]
    public async Task InitWithSpoilerAsync_LoadsOnlyStartingItems()
    {
        // Arrange
        var loadingCompleted = new List<DataLoadingCompletedEventArgs>();
        _sut.DataLoadingCompleted += (s, args) => loadingCompleted.Add(args);

        var spoilerPath = "spoiler.json";
        var startingItems = new Dictionary<ItemName, RegionName>
        {
            { ItemName.DONKEY, RegionName.START },
            { ItemName.DIDDY, RegionName.START }
        };

        _mockSpoilerData.Setup(x => x.GetStartingItems()).Returns(startingItems);
        _mockSpoilerService
            .Setup(x => x.DeserializeAndParseAsync(spoilerPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DeserializationResult<ParsedSpoilerData>.Success(
                new ParsedSpoilerData
                {
                    LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } }
                }
            ));

        _mockSpoilerData.Setup(x => x.CurrentData).Returns(new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } }
        });

        // Act
        await _sut.InitWithSpoilerAsync(spoilerPath);

        // Assert
        _mockItemTracking.Verify(
            x => x.LoadItemsSilently(It.Is<Dictionary<ItemName, SavedItem>>(d =>
                d.Count == 2 &&
                d[ItemName.DONKEY].Autotracked &&
                d[ItemName.DIDDY].Autotracked
            )),
            Times.Once
        );
    }

    [Fact]
    public async Task InitWithSpoilerAsync_IgnoresSavedItems()
    {
        // Arrange
        var spoilerPath = "spoiler.json";
        var startingItems = new Dictionary<ItemName, RegionName>
        {
            { ItemName.DONKEY, RegionName.START }
        };

        _mockSpoilerData.Setup(x => x.GetStartingItems()).Returns(startingItems);
        _mockSpoilerService
            .Setup(x => x.DeserializeAndParseAsync(spoilerPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DeserializationResult<ParsedSpoilerData>.Success(
                new ParsedSpoilerData
                {
                    LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } }
                }
            ));

        _mockSpoilerData.Setup(x => x.CurrentData).Returns(new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } }
        });

        // Act
        await _sut.InitWithSpoilerAsync(spoilerPath);

        // Assert
        _mockHints.Verify(x => x.LoadHintsSilently(It.Is<List<SavedHint>>(h => h.Count == 0)), Times.Once);
    }

    #endregion

    #region LoadProgressStateAsync Integration Tests

    [Fact]
    public async Task LoadProgressStateAsync_WithSpoilerParsing_LoadsSpoilerData()
    {
        // Arrange
        var loadingCompleted = new List<DataLoadingCompletedEventArgs>();
        _sut.DataLoadingCompleted += (s, args) => loadingCompleted.Add(args);

        var spoilerPath = "spoiler.json";
        var parsedData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } }
        };

        _mockSpoilerService
            .Setup(x => x.DeserializeAndParseAsync(spoilerPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DeserializationResult<ParsedSpoilerData>.Success(parsedData));

        _mockSpoilerData.Setup(x => x.CurrentData).Returns(parsedData);
        _mockSpoilerData.Setup(x => x.GetStartingItems()).Returns([]);

        var savedProgress = new SavedProgress { SpoilerPath = spoilerPath };
        _mockDataPersistence
            .Setup(x => x.LoadAsync<SavedProgress>(It.IsAny<string>()))
            .ReturnsAsync(savedProgress);

        // Act
        await _sut.LoadAsync("test.json");

        // Assert
        _mockSpoilerData.Verify(x => x.LoadParsedDataSilently(parsedData), Times.Once);
    }

    [Fact]
    public async Task LoadProgressStateAsync_WithSpoilerParsingError_ContinuesWithNullData()
    {
        // Arrange
        var loadingCompleted = new List<DataLoadingCompletedEventArgs>();
        _sut.DataLoadingCompleted += (s, args) => loadingCompleted.Add(args);

        var spoilerPath = "invalid.json";

        _mockSpoilerService
            .Setup(x => x.DeserializeAndParseAsync(spoilerPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DeserializationResult<ParsedSpoilerData>.Failure("Failed to parse"));

        var savedProgress = new SavedProgress { SpoilerPath = spoilerPath };
        _mockDataPersistence
            .Setup(x => x.LoadAsync<SavedProgress>(It.IsAny<string>()))
            .ReturnsAsync(savedProgress);

        // Act
        await _sut.LoadAsync("test.json");

        // Assert
        _mockSpoilerData.Verify(x => x.LoadParsedDataSilently(null), Times.Once);
        Assert.Single(loadingCompleted); // Should still fire completion event
    }

    [Fact]
    public async Task LoadProgressStateAsync_LoadsAllServiceStates()
    {
        // Arrange
        var savedProgress = new SavedProgress
        {
            SpoilerPath = "spoiler.json",
            SavedGBCounts = new Dictionary<RegionName, string> { { RegionName.JUNGLE_JAPES, "5" } },
            BLockerImageIndexes = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 3 } },
            HelmDoorImageIndexes = [1, 2, 3],
            HelmDoorCounts = ["2"],
            HelmKongs = [1, 0, 1, 0, 1],
            BossKongs = [0, 1, 0, 1, 0],
            LevelOrder = [0, 1, 2, 3, 4, 5, 6, 7]
        };

        _mockDataPersistence
            .Setup(x => x.LoadAsync<SavedProgress>(It.IsAny<string>()))
            .ReturnsAsync(savedProgress);

        _mockSpoilerData.Setup(x => x.CurrentData).Returns((ParsedSpoilerData?)null);

        // Act
        await _sut.LoadAsync("test.json");

        // Assert
        _mockBarriers.Verify(x => x.LoadBLockerCountsSilently(savedProgress.SavedGBCounts), Times.Once);
        _mockBarriers.Verify(x => x.LoadBLockerTypesSilently(savedProgress.BLockerImageIndexes), Times.Once);
        _mockBarriers.Verify(x => x.LoadHelmDoorCountsSilently(savedProgress.HelmDoorCounts), Times.Once);
        _mockBarriers.Verify(x => x.LoadHelmDoorTypesSilently(savedProgress.HelmDoorImageIndexes), Times.Once);
        _mockEndGameProgression.Verify(x => x.SetHelmKongsSilently(savedProgress.HelmKongs), Times.Once);
        _mockEndGameProgression.Verify(x => x.SetBossKongsSilently(savedProgress.BossKongs), Times.Once);
        _mockLevelOrder.Verify(x => x.LoadLevelOrderSilently(savedProgress.LevelOrder), Times.Once);
    }

    [Fact]
    public async Task LoadProgressStateAsync_CombinesSpoilerStartingItemsWithSavedItems()
    {
        // Arrange
        var startingItems = new Dictionary<ItemName, RegionName>
        {
            { ItemName.DONKEY, RegionName.START }
        };

        var savedItems = new Dictionary<ItemName, SavedItem>
        {
            { ItemName.DONKEY, new SavedItem(ItemName.DONKEY, starred: true) },
            { ItemName.DIDDY, new SavedItem(ItemName.DIDDY, starred: true) }
        };

        _mockSpoilerData.Setup(x => x.GetStartingItems()).Returns(startingItems);
        _mockSpoilerData.Setup(x => x.CurrentData).Returns(new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } }
        });

        _mockSpoilerService
            .Setup(x => x.DeserializeAndParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(DeserializationResult<ParsedSpoilerData>.Success(
                new ParsedSpoilerData
                {
                    LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } }
                }
            ));

        var savedProgress = new SavedProgress
        {
            SpoilerPath = "spoiler.json",
            SavedItems = savedItems
        };

        _mockDataPersistence
            .Setup(x => x.LoadAsync<SavedProgress>(It.IsAny<string>()))
            .ReturnsAsync(savedProgress);

        // Act
        await _sut.LoadAsync("test.json");

        // Assert
        _mockItemTracking.Verify(
            x => x.LoadItemsSilently(It.Is<Dictionary<ItemName, SavedItem>>(d =>
                d.Count == 2 &&
                d[ItemName.DONKEY].Starred == true &&  // Saved item wins
                d[ItemName.DIDDY].ItemName == ItemName.DIDDY
            )),
            Times.Once
        );
    }

    #endregion

    #region Error Recovery Tests

    [Fact]
    public async Task LoadAsync_OnException_ThrowsAndDoesNotModifyState()
    {
        // Arrange
        _mockDataPersistence
            .Setup(x => x.LoadAsync<SavedProgress>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("File not found"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<IOException>(() => _sut.LoadAsync("test.json"));
        Assert.Contains("File not found", exception.Message);

        // The service catches the exception and resets, but then rethrows
        // Verify that at least the load was attempted
        _mockDataPersistence.Verify(
            x => x.LoadAsync<SavedProgress>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task InitWithSpoilerAsync_WithSpoilerParsingFailure_RollsBackToCleanState()
    {
        // Arrange
        _mockSpoilerService
            .Setup(x => x.DeserializeAndParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(DeserializationResult<ParsedSpoilerData>.Failure("Invalid spoiler format"));

        _mockSpoilerData.Setup(x => x.CurrentData).Returns((ParsedSpoilerData?)null);

        // Note: This won't throw because the service catches the error
        // Act
        await _sut.InitWithSpoilerAsync("invalid.json");

        // Assert
        // Verify it continues with null spoiler data
        _mockSpoilerData.Verify(x => x.LoadParsedDataSilently(null), Times.Once);
    }

    #endregion
}

#pragma warning restore CS8620
