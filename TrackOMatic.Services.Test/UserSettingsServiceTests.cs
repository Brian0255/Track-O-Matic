using System.Text.Json;

using Moq;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Services.Test;

public class UserSettingsServiceTests
{
    #region Test Fixture & Helpers

    /// <summary>
    /// Creates a UserSettingsService with mocked IDataPersistenceService and optional custom file path.
    /// </summary>
    private static UserSettingsService CreateService(
        string settingsFilePath = "test-settings.json",
        UserSettingsData? dataToLoad = null,
        bool shouldThrowOnLoad = false)
    {
        var mockPersistence = new Mock<IDataPersistenceService>();

        if (shouldThrowOnLoad)
        {
            mockPersistence
                .Setup(x => x.LoadAsync<UserSettingsData>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new IOException("Simulated load error"));
        }
        else
        {
            mockPersistence
                .Setup(x => x.LoadAsync<UserSettingsData>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataToLoad ?? new UserSettingsData());
        }

        // Mock SaveAsync to complete successfully by default
        mockPersistence
            .Setup(x => x.SaveAsync(It.IsAny<UserSettingsData>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new UserSettingsService(settingsFilePath, mockPersistence.Object);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithCustomPath_CreatesServiceWithCustomPath()
    {
        // Act
        var service = CreateService(settingsFilePath: "custom-path.json");

        // Assert
        // Verify defaults are loaded
        Assert.False(service.TopMost);
        Assert.False(service.SongDisplay);
        Assert.True(service.CompactMode); // default true
        Assert.True(service.Autotracking); // default true
    }

    [Fact]
    public void Constructor_WithSuccessfulLoad_LoadsDataFromFile()
    {
        // Arrange
        var customData = new UserSettingsData
        {
            TopMost = true,
            SongDisplay = false,
            CompactMode = false,
            Autotracking = false,
            ShowTotalBPs = true,
            ShowKRoolOrder = false,
            BroadcastViewActive = true,
            BroadcastHelmKRool = true,
            HintDisplay = "AllHints",
            BroadcastNumberLabel = "Deaths"
        };

        // Act
        var service = CreateService(dataToLoad: customData);

        // Assert
        Assert.True(service.TopMost);
        Assert.False(service.SongDisplay);
        Assert.False(service.CompactMode);
        Assert.False(service.Autotracking);
        Assert.True(service.ShowTotalBPs);
        Assert.False(service.ShowKRoolOrder);
        Assert.True(service.BroadcastViewActive);
        Assert.True(service.BroadcastHelmKRool);
    }

    [Fact]
    public void Constructor_WithLoadFailure_FallsBackToDefaults()
    {
        // Act
        var service = CreateService(shouldThrowOnLoad: true);

        // Assert - All defaults should be loaded
        Assert.False(service.TopMost);
        Assert.False(service.SongDisplay);
        Assert.True(service.CompactMode);
        Assert.False(service.HelmInLevelOrder);
        Assert.False(service.HelmDoors);
        Assert.False(service.AutoSortPathHints);
        Assert.False(service.EnemiesInAutofill);
        Assert.False(service.ColoredBarrelPadMoves);
        Assert.True(service.Autotracking);
        Assert.False(service.ShowTotalBPs);
        Assert.False(service.ShowCompanyCoins);
        Assert.True(service.ShowKRoolOrder);
        Assert.True(service.ShowHelmOrder);
        Assert.False(service.BroadcastViewActive);
        Assert.False(service.BroadcastHelmKRool);
        Assert.False(service.BroadcastShopkeepers);
        Assert.False(service.BroadcastSongDisplay);
        Assert.False(service.ShowAmountForHints);
    }

    [Fact]
    public void Constructor_InitializesWithDefaultBooleanValues()
    {
        // Act
        var service = CreateService();

        // Assert - verify all 21 boolean property defaults
        Assert.False(service.TopMost);
        Assert.False(service.SongDisplay);
        Assert.True(service.CompactMode);
        Assert.False(service.HelmInLevelOrder);
        Assert.False(service.HelmDoors);
        Assert.False(service.AutoSortPathHints);
        Assert.False(service.EnemiesInAutofill);
        Assert.False(service.ColoredBarrelPadMoves);
        Assert.True(service.Autotracking);
        Assert.False(service.ShowTotalBPs);
        Assert.False(service.ShowCompanyCoins);
        Assert.True(service.ShowKRoolOrder);
        Assert.True(service.ShowHelmOrder);
        Assert.False(service.BroadcastViewActive);
        Assert.False(service.BroadcastHelmKRool);
        Assert.False(service.BroadcastShopkeepers);
        Assert.False(service.BroadcastSongDisplay);
        Assert.False(service.ShowAmountForHints);
    }

    [Fact]
    public void Constructor_InitializesWithDefaultEnumValues()
    {
        // Act
        var service = CreateService();

        // Assert
        Assert.Equal(BroadcastNumberLabel.WothCount, service.BroadcastNumberLabel);
        Assert.Equal(HintDisplayMode.MultipathHints, service.HintDisplay);
    }

    #endregion

    #region Boolean Property Tests (21 properties)

    [Theory]
    [InlineData(nameof(IUserSettingsService.TopMost), false)]
    [InlineData(nameof(IUserSettingsService.SongDisplay), false)]
    [InlineData(nameof(IUserSettingsService.CompactMode), true)]
    [InlineData(nameof(IUserSettingsService.HelmInLevelOrder), false)]
    [InlineData(nameof(IUserSettingsService.HelmDoors), false)]
    [InlineData(nameof(IUserSettingsService.AutoSortPathHints), false)]
    [InlineData(nameof(IUserSettingsService.EnemiesInAutofill), false)]
    [InlineData(nameof(IUserSettingsService.ColoredBarrelPadMoves), false)]
    [InlineData(nameof(IUserSettingsService.Autotracking), true)]
    [InlineData(nameof(IUserSettingsService.ShowTotalBPs), false)]
    [InlineData(nameof(IUserSettingsService.ShowCompanyCoins), false)]
    [InlineData(nameof(IUserSettingsService.ShowKRoolOrder), true)]
    [InlineData(nameof(IUserSettingsService.ShowHelmOrder), true)]
    [InlineData(nameof(IUserSettingsService.BroadcastHelmKRool), false)]
    [InlineData(nameof(IUserSettingsService.BroadcastShopkeepers), false)]
    [InlineData(nameof(IUserSettingsService.BroadcastSongDisplay), false)]
    [InlineData(nameof(IUserSettingsService.BroadcastViewActive), false)]
    [InlineData(nameof(IUserSettingsService.ShowAmountForHints), false)]
    public void BooleanProperty_Get_ReturnsValueFromSettingsData(string propertyName, bool expectedDefault)
    {
        // Arrange
        var service = CreateService();
        var propertyInfo = typeof(IUserSettingsService).GetProperty(propertyName);
        Assert.NotNull(propertyInfo);

        // Act
        var value = (bool)propertyInfo.GetValue(service)!;

        // Assert - should get expected default value
        Assert.Equal(expectedDefault, value);
    }

    [Theory]
    [InlineData(nameof(IUserSettingsService.TopMost), true)]
    [InlineData(nameof(IUserSettingsService.SongDisplay), true)]
    [InlineData(nameof(IUserSettingsService.CompactMode), false)]
    [InlineData(nameof(IUserSettingsService.HelmInLevelOrder), true)]
    [InlineData(nameof(IUserSettingsService.HelmDoors), true)]
    [InlineData(nameof(IUserSettingsService.AutoSortPathHints), true)]
    [InlineData(nameof(IUserSettingsService.EnemiesInAutofill), true)]
    [InlineData(nameof(IUserSettingsService.ColoredBarrelPadMoves), true)]
    [InlineData(nameof(IUserSettingsService.Autotracking), false)]
    [InlineData(nameof(IUserSettingsService.ShowTotalBPs), true)]
    [InlineData(nameof(IUserSettingsService.ShowCompanyCoins), true)]
    [InlineData(nameof(IUserSettingsService.ShowKRoolOrder), false)]
    [InlineData(nameof(IUserSettingsService.ShowHelmOrder), false)]
    [InlineData(nameof(IUserSettingsService.BroadcastHelmKRool), true)]
    [InlineData(nameof(IUserSettingsService.BroadcastShopkeepers), true)]
    [InlineData(nameof(IUserSettingsService.BroadcastSongDisplay), true)]
    [InlineData(nameof(IUserSettingsService.BroadcastViewActive), true)]
    [InlineData(nameof(IUserSettingsService.ShowAmountForHints), true)]
    public void BooleanProperty_Set_UpdatesValueAndFiresPropertyChanged(string propertyName, bool newValue)
    {
        // Arrange
        var service = CreateService();
        var propertyInfo = typeof(IUserSettingsService).GetProperty(propertyName);
        Assert.NotNull(propertyInfo);
        var eventFired = false;

        service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == propertyName)
            {
                eventFired = true;
            }
        };

        // Act
        propertyInfo.SetValue(service, newValue);

        // Assert
        var retrievedValue = (bool)propertyInfo.GetValue(service)!;
        Assert.Equal(newValue, retrievedValue);
        Assert.True(eventFired);
    }

    [Theory]
    [InlineData(nameof(IUserSettingsService.TopMost), false)]
    [InlineData(nameof(IUserSettingsService.SongDisplay), false)]
    [InlineData(nameof(IUserSettingsService.CompactMode), true)]
    [InlineData(nameof(IUserSettingsService.HelmInLevelOrder), false)]
    [InlineData(nameof(IUserSettingsService.HelmDoors), false)]
    [InlineData(nameof(IUserSettingsService.AutoSortPathHints), false)]
    [InlineData(nameof(IUserSettingsService.EnemiesInAutofill), false)]
    [InlineData(nameof(IUserSettingsService.ColoredBarrelPadMoves), false)]
    [InlineData(nameof(IUserSettingsService.Autotracking), true)]
    [InlineData(nameof(IUserSettingsService.ShowTotalBPs), false)]
    [InlineData(nameof(IUserSettingsService.ShowCompanyCoins), false)]
    [InlineData(nameof(IUserSettingsService.ShowKRoolOrder), true)]
    [InlineData(nameof(IUserSettingsService.ShowHelmOrder), true)]
    [InlineData(nameof(IUserSettingsService.BroadcastHelmKRool), false)]
    [InlineData(nameof(IUserSettingsService.BroadcastShopkeepers), false)]
    [InlineData(nameof(IUserSettingsService.BroadcastSongDisplay), false)]
    [InlineData(nameof(IUserSettingsService.BroadcastViewActive), false)]
    [InlineData(nameof(IUserSettingsService.ShowAmountForHints), false)]
    public void BooleanProperty_SetSameValue_DoesNotFirePropertyChanged(string propertyName, bool currentValue)
    {
        // Arrange
        var service = CreateService();
        var propertyInfo = typeof(IUserSettingsService).GetProperty(propertyName);
        Assert.NotNull(propertyInfo);

        // Set initial value if needed (to match the currentValue)
        if ((bool)propertyInfo.GetValue(service)! != currentValue)
        {
            propertyInfo.SetValue(service, currentValue);
        }

        var eventFired = false;
        service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == propertyName)
            {
                eventFired = true;
            }
        };

        // Act - set to same value
        propertyInfo.SetValue(service, currentValue);

        // Assert
        Assert.False(eventFired);
    }

    #endregion

    #region Enum Property Tests

    [Fact]
    public void BroadcastNumberLabel_Get_ReturnsValueFromSettingsData()
    {
        // Arrange
        var service = CreateService();

        // Act
        var value = service.BroadcastNumberLabel;

        // Assert
        Assert.Equal(BroadcastNumberLabel.WothCount, value);
    }

    [Theory]
    [InlineData(BroadcastNumberLabel.Points)]
    public void BroadcastNumberLabel_Set_UpdatesValueAndFiresPropertyChanged(BroadcastNumberLabel newValue)
    {
        // Arrange
        var service = CreateService();
        var eventFired = false;

        service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(IUserSettingsService.BroadcastNumberLabel))
            {
                eventFired = true;
            }
        };

        // Act
        service.BroadcastNumberLabel = newValue;

        // Assert
        Assert.Equal(newValue, service.BroadcastNumberLabel);
        Assert.True(eventFired);
    }

    [Fact]
    public void BroadcastNumberLabel_SetSameValue_DoesNotFirePropertyChanged()
    {
        // Arrange
        var service = CreateService();
        var eventFired = false;

        service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName != nameof(IUserSettingsService.BroadcastNumberLabel))
            {
                return;
            }
            eventFired = true;
        };

        // Act - set to same value as default (WothCount)
        service.BroadcastNumberLabel = BroadcastNumberLabel.WothCount;

        // Assert
        Assert.False(eventFired);
    }

    [Fact]
    public void HintDisplay_Get_ReturnsValueFromSettingsData()
    {
        // Arrange
        var service = CreateService();

        // Act
        var value = service.HintDisplay;

        // Assert
        Assert.Equal(HintDisplayMode.MultipathHints, value);
    }

    [Theory]
    [InlineData(HintDisplayMode.Off)]
    [InlineData(HintDisplayMode.DirectItemHints)]
    public void HintDisplay_Set_UpdatesValueAndFiresPropertyChanged(HintDisplayMode newValue)
    {
        // Arrange
        var service = CreateService();
        var eventFired = false;

        service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(IUserSettingsService.HintDisplay))
            {
                eventFired = true;
            }
        };

        // Act
        service.HintDisplay = newValue;

        // Assert
        Assert.Equal(newValue, service.HintDisplay);
        Assert.True(eventFired);
    }

    [Fact]
    public void HintDisplay_SetSameValue_DoesNotFirePropertyChanged()
    {
        // Arrange
        var service = CreateService();
        var eventFired = false;

        service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(IUserSettingsService.HintDisplay))
            {
                eventFired = true;
            }
        };

        // Act - set to same value as default (MultipathHints)
        service.HintDisplay = HintDisplayMode.MultipathHints;

        // Assert
        Assert.False(eventFired);
    }

    #endregion

    #region PropertyChanged Event Tests

    [Theory]
    [InlineData(nameof(IUserSettingsService.TopMost))]
    [InlineData(nameof(IUserSettingsService.SongDisplay))]
    [InlineData(nameof(IUserSettingsService.CompactMode))]
    [InlineData(nameof(IUserSettingsService.HelmInLevelOrder))]
    [InlineData(nameof(IUserSettingsService.HelmDoors))]
    [InlineData(nameof(IUserSettingsService.AutoSortPathHints))]
    [InlineData(nameof(IUserSettingsService.EnemiesInAutofill))]
    [InlineData(nameof(IUserSettingsService.ColoredBarrelPadMoves))]
    [InlineData(nameof(IUserSettingsService.Autotracking))]
    [InlineData(nameof(IUserSettingsService.ShowTotalBPs))]
    [InlineData(nameof(IUserSettingsService.ShowCompanyCoins))]
    [InlineData(nameof(IUserSettingsService.ShowKRoolOrder))]
    [InlineData(nameof(IUserSettingsService.ShowHelmOrder))]
    [InlineData(nameof(IUserSettingsService.BroadcastHelmKRool))]
    [InlineData(nameof(IUserSettingsService.BroadcastShopkeepers))]
    [InlineData(nameof(IUserSettingsService.BroadcastSongDisplay))]
    [InlineData(nameof(IUserSettingsService.BroadcastViewActive))]
    [InlineData(nameof(IUserSettingsService.ShowAmountForHints))]
    public void PropertyChanged_FiresWithCorrectPropertyName_BooleanProperties(string propertyName)
    {
        // Arrange
        var service = CreateService();
        var propertyInfo = typeof(IUserSettingsService).GetProperty(propertyName);
        Assert.NotNull(propertyInfo);
        var receivedPropertyName = string.Empty;

        service.PropertyChanged += (s, e) => receivedPropertyName = e.PropertyName ?? string.Empty;

        // Act
        var currentValue = (bool)propertyInfo.GetValue(service)!;
        propertyInfo.SetValue(service, !currentValue);

        // Assert
        Assert.Equal(propertyName, receivedPropertyName);
    }

    [Theory]
    [InlineData(nameof(IUserSettingsService.BroadcastNumberLabel))]
    [InlineData(nameof(IUserSettingsService.HintDisplay))]
    public void PropertyChanged_FiresWithCorrectPropertyName_EnumProperties(string propertyName)
    {
        // Arrange
        var service = CreateService();
        var receivedPropertyName = string.Empty;

        service.PropertyChanged += (s, e) => receivedPropertyName = e.PropertyName ?? string.Empty;

        // Act
        if (propertyName == nameof(IUserSettingsService.BroadcastNumberLabel))
        {
            service.BroadcastNumberLabel = BroadcastNumberLabel.Points;
        }
        else
        {
            service.HintDisplay = HintDisplayMode.Off;
        }

        // Assert
        Assert.Equal(propertyName, receivedPropertyName);
    }

    #endregion

    #region Persistence Tests

    [Fact]
    public void PropertyChange_CallsSaveToFileAsync()
    {
        // Arrange
        var mockPersistence = new Mock<IDataPersistenceService>();
        mockPersistence
            .Setup(x => x.LoadAsync<UserSettingsData>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserSettingsData());
        mockPersistence
            .Setup(x => x.SaveAsync(It.IsAny<UserSettingsData>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new UserSettingsService("test-path.json", mockPersistence.Object)
        {
            // Act - trigger a property change
            TopMost = true
        };

        // Assert - wait a bit for the fire-and-forget save
        Thread.Sleep(100);
        mockPersistence.Verify(
            x => x.SaveAsync(It.IsAny<UserSettingsData>(), "test-path.json", It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void SaveToFileAsync_OnSuccess_CompletesWithoutError()
    {
        // Arrange
        var mockPersistence = new Mock<IDataPersistenceService>();
        mockPersistence
            .Setup(x => x.LoadAsync<UserSettingsData>(It.IsAny<string>(), options: It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserSettingsData());
        mockPersistence
            .Setup(x => x.SaveAsync(It.IsAny<UserSettingsData>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new UserSettingsService("test-path.json", mockPersistence.Object)
        {
            // Act - no exception should be thrown
            TopMost = true
        };

        // Assert
        Thread.Sleep(100);
        Assert.True(service.TopMost);
    }

    [Fact]
    public void SaveToFileAsync_OnException_SilentlyHandlesError()
    {
        // Arrange
        var mockPersistence = new Mock<IDataPersistenceService>();
        mockPersistence
            .Setup(x => x.LoadAsync<UserSettingsData>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserSettingsData());
        mockPersistence
            .Setup(x => x.SaveAsync(It.IsAny<UserSettingsData>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("Simulated save error"));

        var service = new UserSettingsService("test-path.json", mockPersistence.Object);

        // Act - exception should be silently handled
        var exceptionThrown = false;
        try
        {
            service.TopMost = true;
            Thread.Sleep(100); // Wait for async save task
        }
        catch
        {
            exceptionThrown = true;
        }

        // Assert
        Assert.False(exceptionThrown);
        Assert.True(service.TopMost); // Value should still be set
    }

    #endregion
}
