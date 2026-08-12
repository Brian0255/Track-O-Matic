using System.Text.Json;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services.Test;

/// <summary>
/// Unit tests for DataPersistenceService.
/// Tests file I/O, JSON serialization, event firing, and error handling.
/// </summary>
public sealed class DataPersistenceServiceTests : IDisposable
{
    private readonly string _testDataDir;
    private readonly string _testFilePath;

    public DataPersistenceServiceTests()
    {
        _testDataDir = Path.Combine(Path.GetTempPath(), $"TrackOMatic-Persistence-Tests-{Guid.NewGuid()}");
        _testFilePath = Path.Combine(_testDataDir, "test-progress.json");
        Directory.CreateDirectory(_testDataDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDataDir))
        {
            Directory.Delete(_testDataDir, recursive: true);
        }
        GC.SuppressFinalize(this);
    }

    #region Load Tests

    [Fact]
    public void Load_WithValidJsonFile_ReturnsDeserializedProgress()
    {
        // Arrange
        var service = new DataPersistenceService();
        var progress = new SavedProgress();
        progress.SavedGBCounts[RegionName.JUNGLE_JAPES] = "5";
        progress.BLockerImageIndexes[RegionName.JUNGLE_JAPES] = 1;
        progress.HelmDoorImageIndexes.Add(4);
        progress.HelmDoorCounts.Add("3");
        progress.HelmKongs.Add(0);
        progress.BossKongs.Add(1);
        progress.LevelOrder.Add(0);

        // Act - save then load
        service.Save(progress, _testFilePath);
        var loaded = service.Load(_testFilePath);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(loaded);
            Assert.Equal("5", loaded.SavedGBCounts[RegionName.JUNGLE_JAPES]);
            Assert.Equal(1, loaded.BLockerImageIndexes[RegionName.JUNGLE_JAPES]);
            Assert.Single(loaded.HelmDoorImageIndexes);
        });
    }

    [Fact]
    public void Load_WithNonexistentFile_ReturnsNull()
    {
        // Arrange
        var service = new DataPersistenceService();
        var nonexistentPath = Path.Combine(_testDataDir, "nonexistent.json");

        // Act
        var result = service.Load(nonexistentPath);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Load_WithInvalidJson_ReturnsNullAndFiresSaveFailed()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "{ invalid json }");
        var service = new DataPersistenceService();
        var failedEventFired = false;
        string? failureMessage = null;

        service.SaveFailed += (sender, message) =>
        {
            failedEventFired = true;
            failureMessage = message;
        };

        // Act
        var result = service.Load(_testFilePath);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Null(result);
            Assert.True(failedEventFired);
            Assert.NotNull(failureMessage);
            Assert.Contains("Failed to deserialize", failureMessage);
        });
    }

    [Fact]
    public void Load_WithFileReadError_ReturnsNullAndFiresSaveFailed()
    {
        // Arrange
        var service = new DataPersistenceService();
        var lockedFilePath = Path.Combine(_testDataDir, "locked.json");
        File.WriteAllText(lockedFilePath, "{}");

        var failedEventFired = false;
        service.SaveFailed += (sender, message) =>
        {
            failedEventFired = true;
        };

        // Use a file stream to lock the file
        using var fs = File.Open(lockedFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
        // Act
        var result = service.Load(lockedFilePath);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Null(result);
            Assert.True(failedEventFired);
        });
    }

    #endregion

    #region Save Tests

    [Fact]
    public void Save_WithValidProgress_CreatesFileAndFiresEvent()
    {
        // Arrange
        var service = new DataPersistenceService();
        var progress = new SavedProgress();
        progress.SavedGBCounts[RegionName.JUNGLE_JAPES] = "5";

        var eventFired = false;
        SavedProgressChangedEventArgs? eventArgs = null;

        service.SaveCompleted += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        // Act
        service.Save(progress, _testFilePath);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(File.Exists(_testFilePath));
            Assert.True(eventFired);
            Assert.NotNull(eventArgs);
            Assert.Equal(_testFilePath, eventArgs.FilePath);
            Assert.True(eventArgs.Success);
        });
    }

    [Fact]
    public void Save_WithNullProgress_FiresSaveFailedEvent()
    {
        // Arrange
        var service = new DataPersistenceService();
        var failedEventFired = false;
        string? failureMessage = null;

        service.SaveFailed += (sender, message) =>
        {
            failedEventFired = true;
            failureMessage = message;
        };

        // Act
        service.Save(null!, _testFilePath);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(failedEventFired);
            Assert.NotNull(failureMessage);
            Assert.Contains("null", failureMessage);
        });
    }

    [Fact]
    public void Save_SuccessfullyWritesValidJson()
    {
        // Arrange
        var service = new DataPersistenceService();
        var progress = new SavedProgress();
        progress.SavedGBCounts[RegionName.START] = "1";
        progress.HelmKongs.Add(0);

        var successFired = false;
        service.SaveCompleted += (sender, args) => successFired = true;

        // Act
        service.Save(progress, _testFilePath);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(successFired && File.Exists(_testFilePath));
            var content = File.ReadAllText(_testFilePath);
            Assert.NotEmpty(content);
            // JSON serializer uses PropertyNameCaseInsensitive, so check lowercase version
            Assert.Contains("savedgbcounts", content.ToLower());
        });
    }

    [Fact]
    public void Save_WithInvalidPath_FiresSaveFailedEvent()
    {
        // Arrange
        var service = new DataPersistenceService();
        var invalidPath = "\0invalid.json"; // Null character makes path invalid
        var progress = new SavedProgress();
        var failedEventFired = false;

        service.SaveFailed += (sender, message) =>
        {
            failedEventFired = true;
        };

        // Act
        service.Save(progress, invalidPath);

        // Assert
        Assert.True(failedEventFired);
    }

    #endregion

    #region Reset Tests

    [Fact]
    public void Reset_DoesNotThrowForStatelessService()
    {
        // Arrange
        var service = new DataPersistenceService();

        // Act & Assert
        service.Reset(); // Should not throw
    }

    #endregion

    #region Event Tests

    [Fact]
    public void SaveCompleted_IncludesCorrectEventData()
    {
        // Arrange
        var service = new DataPersistenceService();
        var progress = new SavedProgress();
        SavedProgressChangedEventArgs? capturedArgs = null;

        service.SaveCompleted += (sender, args) =>
        {
            capturedArgs = args;
        };

        // Act
        service.Save(progress, _testFilePath);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(capturedArgs);
            Assert.Equal(_testFilePath, capturedArgs.FilePath);
            Assert.True(capturedArgs.Success);
        });
    }

    [Fact]
    public void SaveFailed_IncludesErrorMessage()
    {
        // Arrange
        var service = new DataPersistenceService();
        string? capturedMessage = null;

        service.SaveFailed += (sender, message) =>
        {
            capturedMessage = message;
        };

        // Act
        service.Save(null!, _testFilePath);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(capturedMessage);
            Assert.NotEmpty(capturedMessage);
        });
    }

    [Fact]
    public void MultipleSubscribersCanListenToEvents()
    {
        // Arrange
        var service = new DataPersistenceService();
        var subscriber1Called = false;
        var subscriber2Called = false;
        var progress = new SavedProgress();

        service.SaveCompleted += (sender, args) => subscriber1Called = true;
        service.SaveCompleted += (sender, args) => subscriber2Called = true;

        // Act
        service.Save(progress, _testFilePath);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(subscriber1Called);
            Assert.True(subscriber2Called);
        });
    }

    #endregion
}
