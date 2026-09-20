namespace TrackOMatic.Services.Test;

using System.ComponentModel;

using Moq;

public class BarrelPadThemeServiceTests
{
    private static Mock<IUserSettingsService> CreateMockSettingsService(bool initialValue = false)
    {
        var mockSettings = new Mock<IUserSettingsService>();
        var currentValue = initialValue;

        // Setup getter to return current value
        mockSettings.Setup(s => s.ColoredBarrelPadMoves)
            .Returns(() => currentValue);

        // Setup setter to update current value and raise PropertyChanged
        mockSettings.SetupSet(s => s.ColoredBarrelPadMoves = It.IsAny<bool>())
            .Callback<bool>(value =>
            {
                currentValue = value;
                // Raise PropertyChanged event
                mockSettings.Raise(m => m.PropertyChanged += null,
                    new PropertyChangedEventArgs(nameof(IUserSettingsService.ColoredBarrelPadMoves)));
            });

        // Make mock implement INotifyPropertyChanged
        mockSettings.As<INotifyPropertyChanged>();

        return mockSettings;
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenSettingsServiceIsNull()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new BarrelPadThemeService(null!));
            Assert.Equal("settingsService", exception.ParamName);
        });
    }

    [Fact]
    public void Constructor_InitializesFromSettingsService()
    {
        // Arrange
        var mockSettings = CreateMockSettingsService(initialValue: true);

        // Act
        var service = new BarrelPadThemeService(mockSettings.Object);

        // Assert
        Assert.True(service.UseColoredBarrelPadMoves);
    }

    [Fact]
    public void UseColoredBarrelPadMoves_ReturnsInitialValue()
    {
        // Arrange
        var mockSettings = CreateMockSettingsService(initialValue: false);

        // Act
        var service = new BarrelPadThemeService(mockSettings.Object);

        // Assert
        Assert.False(service.UseColoredBarrelPadMoves);
    }

    [Fact]
    public void SetBarrelPadTheme_UpdatesProperty_WhenValueChanges()
    {
        // Arrange
        var mockSettings = CreateMockSettingsService(initialValue: false);
        var service = new BarrelPadThemeService(mockSettings.Object);
        var eventRaised = false;
        service.BarrelPadThemeChanged += (s, e) => eventRaised = true;

        // Act
        service.SetBarrelPadTheme(true);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(service.UseColoredBarrelPadMoves);
            Assert.True(eventRaised);
            mockSettings.VerifySet(s => s.ColoredBarrelPadMoves = true, Times.Once);
        });
    }

    [Fact]
    public void SetBarrelPadTheme_UpdatesPropertyBackToFalse()
    {
        // Arrange
        var mockSettings = CreateMockSettingsService(initialValue: true);
        var service = new BarrelPadThemeService(mockSettings.Object);

        var eventCount = 0;
        service.BarrelPadThemeChanged += (s, e) => eventCount++;

        // Act
        service.SetBarrelPadTheme(false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.False(service.UseColoredBarrelPadMoves);
            Assert.Equal(1, eventCount);
            mockSettings.VerifySet(s => s.ColoredBarrelPadMoves = false, Times.Once);
        });
    }

    [Fact]
    public void BarrelPadThemeChanged_NotRaised_WhenSettingSameValue()
    {
        // Arrange
        var mockSettings = CreateMockSettingsService(initialValue: false);
        var service = new BarrelPadThemeService(mockSettings.Object);
        var eventCount = 0;
        service.BarrelPadThemeChanged += (s, e) => eventCount++;

        // Act
        service.SetBarrelPadTheme(false); // Setting to same value

        // Assert
        Assert.Equal(0, eventCount);
    }

    [Fact]
    public void SubscribesToSettingsPropertyChanged()
    {
        // Arrange
        var mockSettings = CreateMockSettingsService(initialValue: false);
        var service = new BarrelPadThemeService(mockSettings.Object);

        var eventRaised = false;
        service.BarrelPadThemeChanged += (s, e) => eventRaised = true;

        // Act - Trigger property change on settings
        service.SetBarrelPadTheme(true);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(eventRaised);
            Assert.True(service.UseColoredBarrelPadMoves);
        });
    }

    [Fact]
    public void BarrelPadThemeChanged_EventArgsAreEmpty()
    {
        // Arrange
        var mockSettings = CreateMockSettingsService(initialValue: false);
        var service = new BarrelPadThemeService(mockSettings.Object);
        EventArgs? capturedArgs = null;
        service.BarrelPadThemeChanged += (s, e) => capturedArgs = e;

        // Act
        service.SetBarrelPadTheme(true);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(service.UseColoredBarrelPadMoves);
            Assert.NotNull(capturedArgs);
            Assert.Same(EventArgs.Empty, capturedArgs);
        });
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void SetBarrelPadTheme_TogglesValue(bool initialValue, bool newValue)
    {
        // Arrange
        var mockSettings = CreateMockSettingsService(initialValue: initialValue);
        var service = new BarrelPadThemeService(mockSettings.Object);

        // Act
        service.SetBarrelPadTheme(newValue);

        // Assert
        Assert.Equal(newValue, service.UseColoredBarrelPadMoves);
    }
}
