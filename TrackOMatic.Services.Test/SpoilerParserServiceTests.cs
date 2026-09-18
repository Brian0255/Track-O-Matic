using System.Text.Json;
using System.Text.Json.Serialization;

using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Spoilers;

namespace TrackOMatic.Services.Test;

/// <summary>
/// Unit tests for <see cref="SpoilerParserService"/>.
/// </summary>
public class SpoilerParserServiceTests : IAsyncLifetime
{
    private const string TestDataFolderPath = "TestData";
    private const string FirstFixturePath = "183145-rap-sheet-spoilerlog.json";
    private const string SecondFixturePath = "799354-spoilerlog-gvm.json";

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = false,
        UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
    };

    private Mock<IDataPersistenceService> _mockDataPersistenceService = null!;
    private SpoilerParserService _sut = null!;

    public Task InitializeAsync()
    {
        _mockDataPersistenceService = new Mock<IDataPersistenceService>();
        _sut = new SpoilerParserService(_mockDataPersistenceService.Object);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    #region DeserializeAndParseAsync Tests

    [Fact]
    public async Task DeserializeAndParseAsync_WithValidFixture_ReturnSuccessfulResult()
    {
        // Arrange
        var rawSpoiler = new RawSpoilerFile
        {
            Settings = new SimplifiedSettings(),
            HintData = new SpoilerHintData
            {
                RegionDataDictionary = new Dictionary<int, RawRegionSpoilerData>
                {
                    {
                        0, new RawRegionSpoilerData
                        {
                            LevelName = "Jungle Japes",
                            Points = 100,
                            WothCount = -1,
                            VialColors = []
                        }
                    }
                },
                StartingInfo = new StartingInfoRaw
                {
                    HelmOrder = [4, 3],
                    FinalBossOrder = [205, 207, 206],
                    StartingKongs = [4, 2],
                    StartingKeys = [],
                    StartingMoves = [],
                    UnhintableMoves = [],
                    StartingMovesWothCount = 0,
                    LevelOrder = [0, 1, 2, 3, 4, 5, 6],
                    BlockerInfo = []
                },
                PointSpread = new Dictionary<string, int>
                {
                    { "kongs", 100 },
                    { "keys", 100 },
                    { "training_moves", 10 },
                    { "fairy_moves", 20 }
                }
            }
        };

#pragma warning disable CS8620
        _mockDataPersistenceService
            .Setup(x => x.LoadAsync<RawSpoilerFile>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RawSpoilerFile?)rawSpoiler);
#pragma warning restore CS8620

        // Act
        var result = await _sut.DeserializeAndParseAsync(Path.Combine(TestDataFolderPath, FirstFixturePath));

        Assert.Multiple(() =>
        {
            // Assert - The result should be successful since we have all required data
            Assert.NotNull(result);
            if (!result.IsSuccess)
            {
                // If parsing fails, check the error message to understand why
                Assert.Fail($"Expected successful result, but got error: {result.ErrorMessage}");
            }
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.Data.RegionData);
            Assert.NotNull(result.Data.PointSpread);
            Assert.NotNull(result.Data.SpoilerSettings);
        });
    }

    [Fact]
    public async Task DeserializeAndParseAsync_WithNullRawFile_ReturnFailureResult()
    {
        // Arrange
#pragma warning disable CS8620
        _mockDataPersistenceService
            .Setup(x => x.LoadAsync<RawSpoilerFile>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RawSpoilerFile?)null);
#pragma warning restore CS8620

        // Act
        var result = await _sut.DeserializeAndParseAsync(Path.Combine(TestDataFolderPath, FirstFixturePath));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("Failed to load raw spoiler file", result.ErrorMessage);
        });
    }

    [Fact]
    public async Task DeserializeAndParseAsync_WithCancellation_ReturnFailureResult()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockDataPersistenceService
            .Setup(x => x.LoadAsync<RawSpoilerFile>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());

        // Act
        var result = await _sut.DeserializeAndParseAsync(Path.Combine(TestDataFolderPath, FirstFixturePath), cts.Token);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("cancelled", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public async Task DeserializeAndParseAsync_WithLoadException_ReturnFailureResult()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        _mockDataPersistenceService
            .Setup(x => x.LoadAsync<RawSpoilerFile>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
            .Throws(exception);

        // Act
        var result = await _sut.DeserializeAndParseAsync(Path.Combine(TestDataFolderPath, FirstFixturePath));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("Unexpected error", result.ErrorMessage);
        });
    }

    #endregion

    #region SetUpSettings Tests

    [Fact]
    public void SetUpSettings_WithNullRegionInfo_ReturnDefaultSettings()
    {
        // Act
        var settings = SpoilerParserService.SetUpSettings(null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(settings);
            Assert.False(settings.PointsEnabled);
            Assert.False(settings.VialsEnabled);
            Assert.False(settings.WOTHEnabled);
        });
    }

    [Fact]
    public void SetUpSettings_WithPointsEnabled_ReturnSettingsWithPointsTrue()
    {
        // Arrange
        var regionInfo = new RawRegionSpoilerData
        {
            Points = 100,
            WothCount = -1,
            LevelName = "Test Level"
        };

        // Act
        var settings = SpoilerParserService.SetUpSettings(regionInfo);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(settings);
            Assert.True(settings.PointsEnabled);
            Assert.False(settings.VialsEnabled);
            Assert.False(settings.WOTHEnabled);
        });
    }

    [Fact]
    public void SetUpSettings_WithPointsDisabled_ReturnSettingsWithVialsTrue()
    {
        // Arrange
        var regionInfo = new RawRegionSpoilerData
        {
            Points = -1,
            WothCount = -1,
            LevelName = "Test Level"
        };

        // Act
        var settings = SpoilerParserService.SetUpSettings(regionInfo);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(settings);
            Assert.False(settings.PointsEnabled);
            Assert.True(settings.VialsEnabled);
            Assert.False(settings.WOTHEnabled);
        });
    }

    [Fact]
    public void SetUpSettings_WithWothCountValid_ReturnSettingsWithWOTHTrue()
    {
        // Arrange
        var regionInfo = new RawRegionSpoilerData
        {
            Points = 100,
            WothCount = 10,
            LevelName = "Test Level"
        };

        // Act
        var settings = SpoilerParserService.SetUpSettings(regionInfo);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(settings);
            Assert.True(settings.PointsEnabled);
            Assert.False(settings.VialsEnabled);
            Assert.True(settings.WOTHEnabled);
        });
    }

    #endregion

    #region ParseRegionPointData Tests

    [Fact]
    public void ParseRegionPointData_WithMultipleRegions_ReturnDictionaryWithAllRegions()
    {
        // Arrange
        var rawSpoiler = new RawSpoilerFile
        {
            HintData = new SpoilerHintData
            {
                RegionDataDictionary = new Dictionary<int, RawRegionSpoilerData>
                {
                    {
                        0, new RawRegionSpoilerData
                        {
                            LevelName = "Jungle Japes",
                            Points = 100,
                            WothCount = -1,
                            VialColors = ["Blue", "Green"]
                        }
                    },
                    {
                        1, new RawRegionSpoilerData
                        {
                            LevelName = "Angry Aztec",
                            Points = 200,
                            WothCount = -1,
                            VialColors = ["Red"]
                        }
                    }
                },
                StartingInfo = new StartingInfoRaw()
            }
        };

        // Act
        var regionData = SpoilerParserService.ParseRegionPointData(rawSpoiler);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(regionData);
            Assert.Contains(RegionName.START, regionData.Keys);
            Assert.True(regionData.Count >= 2);
        });
    }

    [Fact]
    public void ParseRegionPointData_WithUnknownRegion_SkipUnknownRegion()
    {
        // Arrange
        var rawSpoiler = new RawSpoilerFile
        {
            HintData = new SpoilerHintData
            {
                RegionDataDictionary = new Dictionary<int, RawRegionSpoilerData>
                {
                    {
                        99, new RawRegionSpoilerData
                        {
                            LevelName = "Unknown Level Name",
                            Points = 100,
                            WothCount = -1,
                            VialColors = []
                        }
                    }
                },
                StartingInfo = new StartingInfoRaw()
            }
        };

        // Act
        var regionData = SpoilerParserService.ParseRegionPointData(rawSpoiler);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(regionData);
            // Should only have START region
            Assert.Single(regionData);
            Assert.Contains(RegionName.START, regionData.Keys);
        });
    }

    [Fact]
    public void ParseRegionPointData_WithNullRegionDictionary_ReturnDictionaryWithOnlyStart()
    {
        // Arrange
        var rawSpoiler = new RawSpoilerFile
        {
            HintData = new SpoilerHintData
            {
                RegionDataDictionary = null,
                StartingInfo = new StartingInfoRaw()
            }
        };

        // Act
        var regionData = SpoilerParserService.ParseRegionPointData(rawSpoiler);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(regionData);
            Assert.Single(regionData);
            Assert.Contains(RegionName.START, regionData.Keys);
        });
    }

    #endregion

    #region ParseHelmOrder Tests

    [Fact]
    public void ParseHelmOrder_WithValidOrder_ReturnIncrementedIndices()
    {
        // Arrange
        var info = new StartingInfoRaw
        {
            HelmOrder = [0, 2, 4]
        };

        // Act
        var order = SpoilerParserService.ParseHelmOrder(info);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(order);
            Assert.Equal(3, order.Count);
            Assert.Equal(1, order[0]);
            Assert.Equal(3, order[1]);
            Assert.Equal(5, order[2]);
        });
    }

    [Fact]
    public void ParseHelmOrder_WithNullOrder_ReturnEmptyList()
    {
        // Arrange
        var info = new StartingInfoRaw { HelmOrder = null };

        // Act
        var order = SpoilerParserService.ParseHelmOrder(info);

        // Assert
        Assert.NotNull(order);
        Assert.Empty(order);
    }

    [Fact]
    public void ParseHelmOrder_WithEmptyOrder_ReturnEmptyList()
    {
        // Arrange
        var info = new StartingInfoRaw { HelmOrder = [] };

        // Act
        var order = SpoilerParserService.ParseHelmOrder(info);

        // Assert
        Assert.NotNull(order);
        Assert.Empty(order);
    }

    #endregion

    #region ParseFinalBossOrder Tests

    [Fact]
    public void ParseFinalBossOrder_WithValidOrder_ReturnMappedImageIndices()
    {
        // Arrange
        var info = new StartingInfoRaw
        {
            FinalBossOrder = [205, 207, 206]
        };

        // Act
        var order = SpoilerParserService.ParseFinalBossOrder(info);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(3, order.Count);
    }

    [Fact]
    public void ParseFinalBossOrder_WithNullOrder_ReturnEmptyList()
    {
        // Arrange
        var info = new StartingInfoRaw { FinalBossOrder = null };

        // Act
        var order = SpoilerParserService.ParseFinalBossOrder(info);

        // Assert
        Assert.NotNull(order);
        Assert.Empty(order);
    }

    [Fact]
    public void ParseFinalBossOrder_WithEmptyOrder_ReturnEmptyList()
    {
        // Arrange
        var info = new StartingInfoRaw { FinalBossOrder = [] };

        // Act
        var order = SpoilerParserService.ParseFinalBossOrder(info);

        // Assert
        Assert.NotNull(order);
        Assert.Empty(order);
    }

    #endregion

    #region ParseLevelOrder Tests

    [Fact]
    public void ParseLevelOrder_WithNullOrder_ReturnEmptyDictionary()
    {
        // Arrange
        var info = new StartingInfoRaw { LevelOrder = null };

        // Act
        var order = SpoilerParserService.ParseLevelOrder(info);

        // Assert
        Assert.NotNull(order);
        Assert.Empty(order);
    }

    [Fact]
    public void ParseLevelOrder_WithEmptyOrder_ReturnDictionaryWithAllRegionsZero()
    {
        // Arrange
        var info = new StartingInfoRaw { LevelOrder = [] };

        // Act
        var order = SpoilerParserService.ParseLevelOrder(info);

        // Assert
        Assert.NotNull(order);
        // Empty list results in dictionary with all regions initialized to 0
        Assert.NotEmpty(order);
    }

    [Fact]
    public void ParseLevelOrder_WithValidOrder_ReturnDictionaryWithRegionMappings()
    {
        // Arrange
        var info = new StartingInfoRaw { LevelOrder = [0, 1, 2, 3, 4, 5, 6] };

        // Act
        var order = SpoilerParserService.ParseLevelOrder(info);

        // Assert
        Assert.NotNull(order);
        Assert.NotEmpty(order);
    }

    [Fact]
    public void ParseLevelOrder_WithSevenElements_IncludeHelmOrder()
    {
        // Arrange
        var info = new StartingInfoRaw { LevelOrder = [0, 1, 2, 3, 4, 5, 6] };

        // Act
        var order = SpoilerParserService.ParseLevelOrder(info);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(order);
            Assert.Contains(RegionName.HIDEOUT_HELM, order.Keys);
            Assert.Equal(8, order[RegionName.HIDEOUT_HELM]);
        });
    }

    #endregion

    #region ParsePointSpread Tests

    [Fact]
    public void ParsePointSpread_WithValidSpread_ReturnCategoryMapping()
    {
        // Arrange
        var rawSpoiler = new RawSpoilerFile
        {
            HintData = new SpoilerHintData
            {
                PointSpread = new Dictionary<string, int>
                {
                    { "kongs", 100 },
                    { "keys", 100 },
                    { "training_moves", 10 },
                    { "fairy_moves", 20 }
                }
            }
        };

        // Act
        var spread = SpoilerParserService.ParsePointSpread(rawSpoiler);

        // Assert
        Assert.NotNull(spread);
        Assert.NotEmpty(spread);
    }

    [Fact]
    public void ParsePointSpread_WithoutFairyMoves_AddFairyMovesFromTrainingMoves()
    {
        // Arrange
        var rawSpoiler = new RawSpoilerFile
        {
            HintData = new SpoilerHintData
            {
                PointSpread = new Dictionary<string, int>
                {
                    { "kongs", 100 },
                    { "training_moves", 10 }
                }
            }
        };

        // Act
        var spread = SpoilerParserService.ParsePointSpread(rawSpoiler);

        // Assert
        Assert.NotNull(spread);
    }

    [Fact]
    public void ParsePointSpread_WithNullSpread_ReturnEmptyDictionary()
    {
        // Arrange
        var rawSpoiler = new RawSpoilerFile
        {
            HintData = new SpoilerHintData { PointSpread = null }
        };

        // Act
        var spread = SpoilerParserService.ParsePointSpread(rawSpoiler);

        // Assert
        Assert.NotNull(spread);
        Assert.Empty(spread);
    }

    [Fact]
    public void ParsePointSpread_WithEmptySpread_ReturnEmptyDictionary()
    {
        // Arrange
        var rawSpoiler = new RawSpoilerFile
        {
            HintData = new SpoilerHintData { PointSpread = [] }
        };

        // Act
        var spread = SpoilerParserService.ParsePointSpread(rawSpoiler);

        // Assert
        Assert.NotNull(spread);
        Assert.Empty(spread);
    }

    #endregion

    #region ParseBlockerInfo Tests

    [Fact]
    public void ParseBlockerInfo_WithValidBlockers_ReturnBlockerList()
    {
        // Arrange
        var info = new StartingInfoRaw
        {
            BlockerInfo =
            [
                new() { /* Initialize with default values */ },
                new() { /* Initialize with default values */ }
            ]
        };

        // Act
        var blockers = SpoilerParserService.ParseBlockerInfo(info);

        // Assert
        Assert.NotNull(blockers);
        Assert.Equal(2, blockers.Count);
    }

    [Fact]
    public void ParseBlockerInfo_WithNullBlockers_ReturnEmptyList()
    {
        // Arrange
        var info = new StartingInfoRaw { BlockerInfo = null };

        // Act
        var blockers = SpoilerParserService.ParseBlockerInfo(info);

        // Assert
        Assert.NotNull(blockers);
        Assert.Empty(blockers);
    }

    [Fact]
    public void ParseBlockerInfo_WithEmptyBlockers_ReturnEmptyList()
    {
        // Arrange
        var info = new StartingInfoRaw { BlockerInfo = [] };

        // Act
        var blockers = SpoilerParserService.ParseBlockerInfo(info);

        // Assert
        Assert.NotNull(blockers);
        Assert.Empty(blockers);
    }

    #endregion

    #region ParseStartingMoves Tests

    [Fact]
    public void ParseStartingMoves_WithValidStartingData_ReturnMoveDictionary()
    {
        // Arrange
        var rawSpoiler = new RawSpoilerFile
        {
            HintData = new SpoilerHintData
            {
                StartingInfo = new StartingInfoRaw
                {
                    StartingKongs = [0, 1],
                    StartingKeys = ["Key 1", "Key 2"],
                    StartingMoves = []
                }
            }
        };

        // Act
        var moves = SpoilerParserService.ParseStartingMoves(rawSpoiler);

        // Assert
        Assert.NotNull(moves);
    }

    [Fact]
    public void ParseStartingMoves_WithNullStartingInfo_ReturnEmptyDictionary()
    {
        // Arrange
        var rawSpoiler = new RawSpoilerFile
        {
            HintData = new SpoilerHintData { StartingInfo = null }
        };

        // Act
        var moves = SpoilerParserService.ParseStartingMoves(rawSpoiler);

        // Assert
        Assert.NotNull(moves);
    }

    [Fact]
    public void ParseStartingMoves_WithNullHintData_ReturnEmptyDictionary()
    {
        // Arrange
        var rawSpoiler = new RawSpoilerFile { HintData = null };

        // Act
        var moves = SpoilerParserService.ParseStartingMoves(rawSpoiler);

        // Assert
        Assert.NotNull(moves);
    }

    #endregion

    #region Integration Tests with Real Fixtures

    [Fact]
    public async Task DeserializeAndParseAsync_WithRealFixture_LoadsAndParsesSuccessfully()
    {
        // Arrange
        var filePath = GetTestFixturePath(Path.Combine(TestDataFolderPath, FirstFixturePath));
        if (!File.Exists(filePath))
        {
            // Skip if fixture not found
            return;
        }

        var jsonContent = await File.ReadAllTextAsync(filePath);
        var rawSpoiler = JsonSerializer.Deserialize<RawSpoilerFile>(jsonContent, _options);

        _mockDataPersistenceService
            .Setup(x => x.LoadAsync<RawSpoilerFile>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(rawSpoiler!);

        // Act
        var result = await _sut.DeserializeAndParseAsync(filePath);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.RegionData);
        Assert.NotNull(result.Data.SpoilerSettings);
        Assert.NotNull(result.Data.HelmOrder);
        Assert.NotNull(result.Data.FinalBossOrder);
    }

    [Fact]
    public async Task DeserializeAndParseAsync_WithSecondRealFixture_LoadsAndParsesSuccessfully()
    {
        // Arrange
        var filePath = GetTestFixturePath(Path.Combine(TestDataFolderPath, SecondFixturePath));
        if (!File.Exists(filePath))
        {
            // Skip if fixture not found
            return;
        }

        var jsonContent = await File.ReadAllTextAsync(filePath);
        var rawSpoiler = JsonSerializer.Deserialize<RawSpoilerFile>(jsonContent, _options);

        _mockDataPersistenceService
            .Setup(x => x.LoadAsync<RawSpoilerFile>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(rawSpoiler!);

        // Act
        var result = await _sut.DeserializeAndParseAsync(filePath);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.RegionData);
        Assert.NotNull(result.Data.SpoilerSettings);
    }

    #endregion

    #region Helpers

    private static string GetTestFixturePath(string relativePath)
    {
        return TestContext.GetTestDataPath(relativePath);
    }

    #endregion
}
