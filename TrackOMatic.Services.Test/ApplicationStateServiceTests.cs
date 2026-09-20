using Moq;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Services.Test;

public class ApplicationStateServiceTests
{
    private readonly Mock<IDataPersistenceService> _dataPersistenceServiceMock;
    private readonly string _testFilePath;

    public ApplicationStateServiceTests()
    {
        _dataPersistenceServiceMock = new Mock<IDataPersistenceService>();
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test-app-state-{Guid.NewGuid()}.json");
    }

    private ApplicationStateService CreateService(
        ApplicationStateData? dataToLoad = null,
        bool throwOnLoad = false,
        string? filePath = null)
    {
        filePath ??= _testFilePath;

        if (throwOnLoad)
        {
            _dataPersistenceServiceMock
                .Setup(d => d.LoadAsync<ApplicationStateData>(filePath, null, default))
                .Throws(new IOException("Test load failure"));
        }
        else
        {
            var dataToReturn = dataToLoad ?? new ApplicationStateData();
            _dataPersistenceServiceMock
                .Setup(d => d.LoadAsync<ApplicationStateData>(filePath, null, default))
                .ReturnsAsync(dataToReturn);
        }

        _dataPersistenceServiceMock
            .Setup(d => d.SaveAsync(It.IsAny<ApplicationStateData>(), filePath))
            .Returns(Task.CompletedTask);

        return new ApplicationStateService(filePath, _dataPersistenceServiceMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithDefaultPath_CreatesServiceWithDefaults()
    {
        // Arrange - Use internal constructor to avoid needing real IDataPersistenceService for default path
        var filePath = Path.Combine(Path.GetTempPath(), $"test-app-state-default-{Guid.NewGuid()}.json");
        _dataPersistenceServiceMock
            .Setup(d => d.LoadAsync<ApplicationStateData>(filePath, null, default))
            .ReturnsAsync(new ApplicationStateData());

        _dataPersistenceServiceMock
            .Setup(d => d.SaveAsync(It.IsAny<ApplicationStateData>(), filePath))
            .Returns(Task.CompletedTask);

        // Act
        var sut = new ApplicationStateService(filePath, _dataPersistenceServiceMock.Object);

        // Assert
        Assert.NotNull(sut);
        // Default values from ApplicationStateData
        Assert.Equal(100, sut.WindowX);
        Assert.Equal(100, sut.WindowY);
        Assert.Equal(1800, sut.DesiredWidth);
        Assert.Equal(820, sut.DesiredHeight);
    }

    [Fact]
    public void Constructor_WithCustomPath_CreatesServiceWithCustomPath()
    {
        // Arrange & Act
        var sut = CreateService();

        // Assert
        Assert.NotNull(sut);
        _dataPersistenceServiceMock.Verify(
            d => d.LoadAsync<ApplicationStateData>(_testFilePath, null, default),
            Times.Once
        );
    }

    [Fact]
    public void Constructor_WithSuccessfulLoad_LoadsDataFromFile()
    {
        // Arrange
        var loadedData = new ApplicationStateData
        {
            WindowX = 200,
            WindowY = 300,
            DesiredWidth = 1600,
            DesiredHeight = 900
        };

        // Act
        var sut = CreateService(dataToLoad: loadedData);

        // Assert
        Assert.Equal(200, sut.WindowX);
        Assert.Equal(300, sut.WindowY);
        Assert.Equal(1600, sut.DesiredWidth);
        Assert.Equal(900, sut.DesiredHeight);
    }

    [Fact]
    public void Constructor_WithLoadFailure_FallsBackToDefaults()
    {
        // Arrange & Act
        var sut = CreateService(throwOnLoad: true);

        // Assert - Falls back to defaults
        Assert.Equal(100, sut.WindowX);
        Assert.Equal(100, sut.WindowY);
        Assert.Equal(1800, sut.DesiredWidth);
        Assert.Equal(820, sut.DesiredHeight);
    }

    [Fact]
    public void Constructor_InitializesWithDefaultLastFolderPath()
    {
        // Arrange & Act
        var sut = CreateService();

        // Assert
        var expectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        Assert.Equal(expectedPath, sut.LastFolderPath);
    }

    [Fact]
    public void Constructor_InitializesWithDefaultProgressiveHintValues()
    {
        // Arrange & Act
        var sut = CreateService();

        // Assert
        Assert.Equal((ItemType)9, sut.ProgressiveHintItem);
        Assert.Equal(50, sut.ProgressiveHintCap);
    }

    #endregion

    #region Window Position/Size Property Tests

    [Fact]
    public void WindowX_Get_ReturnsValueFromStateData()
    {
        // Arrange
        var loadedData = new ApplicationStateData { WindowX = 250 };
        var sut = CreateService(dataToLoad: loadedData);

        // Act
        var value = sut.WindowX;

        // Assert
        Assert.Equal(250, value);
    }

    [Fact]
    public void WindowX_Set_UpdatesValueAndFiresPropertyChanged()
    {
        // Arrange
        var sut = CreateService();
        var propertyChangedFired = false;
        string? changedPropertyName = null;

        sut.PropertyChanged += (sender, args) =>
        {
            propertyChangedFired = true;
            changedPropertyName = args.PropertyName;
        };

        // Act
        sut.WindowX = 350;

        // Assert
        Assert.True(propertyChangedFired);
        Assert.Equal(nameof(sut.WindowX), changedPropertyName);
        Assert.Equal(350, sut.WindowX);
    }

    [Fact]
    public void WindowX_SetSameValue_DoesNotFirePropertyChanged()
    {
        // Arrange
        var sut = CreateService();
        var propertyChangedCount = 0;

        sut.PropertyChanged += (sender, args) => propertyChangedCount++;

        // Act
        sut.WindowX = 100; // Same as default

        // Assert
        Assert.Equal(0, propertyChangedCount);
    }

    [Fact]
    public void WindowY_Get_ReturnsValueFromStateData()
    {
        // Arrange
        var loadedData = new ApplicationStateData { WindowY = 350 };
        var sut = CreateService(dataToLoad: loadedData);

        // Act
        var value = sut.WindowY;

        // Assert
        Assert.Equal(350, value);
    }

    [Fact]
    public void WindowY_Set_UpdatesValueAndFiresPropertyChanged()
    {
        // Arrange
        var sut = CreateService();
        var propertyChangedFired = false;
        string? changedPropertyName = null;

        sut.PropertyChanged += (sender, args) =>
        {
            propertyChangedFired = true;
            changedPropertyName = args.PropertyName;
        };

        // Act
        sut.WindowY = 450;

        // Assert
        Assert.True(propertyChangedFired);
        Assert.Equal(nameof(sut.WindowY), changedPropertyName);
        Assert.Equal(450, sut.WindowY);
    }

    [Fact]
    public void DesiredWidth_Get_ReturnsValueFromStateData()
    {
        // Arrange
        var loadedData = new ApplicationStateData { DesiredWidth = 1920 };
        var sut = CreateService(dataToLoad: loadedData);

        // Act
        var value = sut.DesiredWidth;

        // Assert
        Assert.Equal(1920, value);
    }

    [Fact]
    public void DesiredWidth_Set_UpdatesValueAndFiresPropertyChanged()
    {
        // Arrange
        var sut = CreateService();
        var propertyChangedFired = false;
        string? changedPropertyName = null;

        sut.PropertyChanged += (sender, args) =>
        {
            propertyChangedFired = true;
            changedPropertyName = args.PropertyName;
        };

        // Act
        sut.DesiredWidth = 2560;

        // Assert
        Assert.True(propertyChangedFired);
        Assert.Equal(nameof(sut.DesiredWidth), changedPropertyName);
        Assert.Equal(2560, sut.DesiredWidth);
    }

    [Fact]
    public void DesiredHeight_Get_ReturnsValueFromStateData()
    {
        // Arrange
        var loadedData = new ApplicationStateData { DesiredHeight = 1080 };
        var sut = CreateService(dataToLoad: loadedData);

        // Act
        var value = sut.DesiredHeight;

        // Assert
        Assert.Equal(1080, value);
    }

    [Fact]
    public void DesiredHeight_Set_UpdatesValueAndFiresPropertyChanged()
    {
        // Arrange
        var sut = CreateService();
        var propertyChangedFired = false;
        string? changedPropertyName = null;

        sut.PropertyChanged += (sender, args) =>
        {
            propertyChangedFired = true;
            changedPropertyName = args.PropertyName;
        };

        // Act
        sut.DesiredHeight = 1440;

        // Assert
        Assert.True(propertyChangedFired);
        Assert.Equal(nameof(sut.DesiredHeight), changedPropertyName);
        Assert.Equal(1440, sut.DesiredHeight);
    }

    #endregion

    #region LastFolderPath Property Tests

    [Fact]
    public void LastFolderPath_Get_ReturnsValueFromStateData()
    {
        // Arrange
        var testPath = @"C:\Test\Folder";
        var loadedData = new ApplicationStateData { LastFolderPath = testPath };
        var sut = CreateService(dataToLoad: loadedData);

        // Act
        var value = sut.LastFolderPath;

        // Assert
        Assert.Equal(testPath, value);
    }

    [Fact]
    public void LastFolderPath_Set_UpdatesValueAndFiresPropertyChanged()
    {
        // Arrange
        var sut = CreateService();
        var propertyChangedFired = false;
        string? changedPropertyName = null;

        sut.PropertyChanged += (sender, args) =>
        {
            propertyChangedFired = true;
            changedPropertyName = args.PropertyName;
        };

        var newPath = @"C:\New\Path";

        // Act
        sut.LastFolderPath = newPath;

        // Assert
        Assert.True(propertyChangedFired);
        Assert.Equal(nameof(sut.LastFolderPath), changedPropertyName);
        Assert.Equal(newPath, sut.LastFolderPath);
    }

    [Fact]
    public void LastFolderPath_SetSameValue_DoesNotFirePropertyChanged()
    {
        // Arrange
        var defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var sut = CreateService();
        var propertyChangedCount = 0;

        sut.PropertyChanged += (sender, args) => propertyChangedCount++;

        // Act
        sut.LastFolderPath = defaultPath; // Same as default

        // Assert
        Assert.Equal(0, propertyChangedCount);
    }

    #endregion

    #region Progressive Hint Properties Tests

    [Fact]
    public void ProgressiveHintItem_Get_ReturnsValueFromStateData()
    {
        // Arrange
        var loadedData = new ApplicationStateData { ProgressiveHintItem = 5 };
        var sut = CreateService(dataToLoad: loadedData);

        // Act
        var value = sut.ProgressiveHintItem;

        // Assert
        Assert.Equal((ItemType)5, value);
    }

    [Fact]
    public void ProgressiveHintItem_Set_UpdatesValueAndFiresPropertyChanged()
    {
        // Arrange
        var sut = CreateService();
        var propertyChangedFired = false;
        string? changedPropertyName = null;

        sut.PropertyChanged += (sender, args) =>
        {
            propertyChangedFired = true;
            changedPropertyName = args.PropertyName;
        };

        // Act
        sut.ProgressiveHintItem = (ItemType)7;

        // Assert
        Assert.True(propertyChangedFired);
        Assert.Equal(nameof(sut.ProgressiveHintItem), changedPropertyName);
        Assert.Equal((ItemType)7, sut.ProgressiveHintItem);
    }

    [Fact]
    public void ProgressiveHintItem_SetSameValue_DoesNotFirePropertyChanged()
    {
        // Arrange
        var sut = CreateService();
        var propertyChangedCount = 0;

        sut.PropertyChanged += (sender, args) => propertyChangedCount++;

        // Act
        sut.ProgressiveHintItem = (ItemType)9; // Same as default

        // Assert
        Assert.Equal(0, propertyChangedCount);
    }

    [Fact]
    public void ProgressiveHintCap_Get_ReturnsValueFromStateData()
    {
        // Arrange
        var loadedData = new ApplicationStateData { ProgressiveHintCap = 75 };
        var sut = CreateService(dataToLoad: loadedData);

        // Act
        var value = sut.ProgressiveHintCap;

        // Assert
        Assert.Equal(75, value);
    }

    [Fact]
    public void ProgressiveHintCap_Set_UpdatesValueAndFiresPropertyChanged()
    {
        // Arrange
        var sut = CreateService();
        var propertyChangedFired = false;
        string? changedPropertyName = null;

        sut.PropertyChanged += (sender, args) =>
        {
            propertyChangedFired = true;
            changedPropertyName = args.PropertyName;
        };

        // Act
        sut.ProgressiveHintCap = 100;

        // Assert
        Assert.True(propertyChangedFired);
        Assert.Equal(nameof(sut.ProgressiveHintCap), changedPropertyName);
        Assert.Equal(100, sut.ProgressiveHintCap);
    }

    [Fact]
    public void ProgressiveHintCap_SetSameValue_DoesNotFirePropertyChanged()
    {
        // Arrange
        var sut = CreateService();
        var propertyChangedCount = 0;

        sut.PropertyChanged += (sender, args) => propertyChangedCount++;

        // Act
        sut.ProgressiveHintCap = 50; // Same as default

        // Assert
        Assert.Equal(0, propertyChangedCount);
    }

    #endregion

    #region SaveToFileAsync Tests

    [Fact]
    public void PropertyChange_CallsSaveToFileAsync()
    {
        // Arrange
        var sut = CreateService();

        // Act
        sut.WindowX = 500;

        // Assert - Wait a bit for async operation to complete
        System.Threading.Thread.Sleep(100);
        _dataPersistenceServiceMock.Verify(
            d => d.SaveAsync(It.IsAny<ApplicationStateData>(), _testFilePath),
            Times.Once
        );
    }

    [Fact]
    public void SaveToFileAsync_OnSuccess_CompletesWithoutError()
    {
        // Arrange
        var sut = CreateService();
        _dataPersistenceServiceMock
            .Setup(d => d.SaveAsync(It.IsAny<ApplicationStateData>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act - Should not throw
        sut.WindowX = 600;
        System.Threading.Thread.Sleep(100);

        // Assert
        _dataPersistenceServiceMock.Verify(
            d => d.SaveAsync(It.IsAny<ApplicationStateData>(), It.IsAny<string>()),
            Times.Once
        );
    }

    [Fact]
    public void SaveToFileAsync_OnException_SilentlyHandlesError()
    {
        // Arrange
        var sut = CreateService();
        _dataPersistenceServiceMock
            .Setup(d => d.SaveAsync(It.IsAny<ApplicationStateData>(), It.IsAny<string>()))
            .Throws(new IOException("Save failed"));

        // Act - Should not throw
        var ex = Record.Exception(() => sut.WindowX = 700);

        // Assert - No exception should be thrown to caller
        Assert.Null(ex);
    }

    #endregion

    #region PropertyChanged Event Tests

    [Theory]
    [InlineData(nameof(ApplicationStateService.WindowX))]
    [InlineData(nameof(ApplicationStateService.WindowY))]
    [InlineData(nameof(ApplicationStateService.DesiredWidth))]
    [InlineData(nameof(ApplicationStateService.DesiredHeight))]
    [InlineData(nameof(ApplicationStateService.LastFolderPath))]
    [InlineData(nameof(ApplicationStateService.ProgressiveHintItem))]
    [InlineData(nameof(ApplicationStateService.ProgressiveHintCap))]
    public void PropertyChanged_FiresWithCorrectPropertyName(string propertyName)
    {
        // Arrange
        var sut = CreateService();
        string? firedPropertyName = null;

        sut.PropertyChanged += (sender, args) => firedPropertyName = args.PropertyName;

        // Act
        switch (propertyName)
        {
            case nameof(ApplicationStateService.WindowX):
                sut.WindowX = 999;
                break;
            case nameof(ApplicationStateService.WindowY):
                sut.WindowY = 999;
                break;
            case nameof(ApplicationStateService.DesiredWidth):
                sut.DesiredWidth = 999;
                break;
            case nameof(ApplicationStateService.DesiredHeight):
                sut.DesiredHeight = 999;
                break;
            case nameof(ApplicationStateService.LastFolderPath):
                sut.LastFolderPath = "C:\\NewPath";
                break;
            case nameof(ApplicationStateService.ProgressiveHintItem):
                sut.ProgressiveHintItem = (ItemType)15;
                break;
            case nameof(ApplicationStateService.ProgressiveHintCap):
                sut.ProgressiveHintCap = 999;
                break;
        }

        // Assert
        Assert.Equal(propertyName, firedPropertyName);
    }

    #endregion
}
