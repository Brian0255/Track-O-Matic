using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services.Test.TrackerState;

/// <summary>
/// Unit tests for SavedProgressProvider.
/// Tests progress lifecycle management and event firing.
/// </summary>
public class SavedProgressProviderTests
{
    [Fact]
    public void Constructor_StoresInitialProgress()
    {
        // Arrange
        var initial = new SavedProgress();

        // Act
        var provider = new SavedProgressProvider(initial);

        // Assert
        Assert.Same(initial, provider.CurrentProgress);
    }

    [Fact]
    public void Constructor_WithNullProgress_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SavedProgressProvider(null!));
    }

    [Fact]
    public void UpdateProgress_WithNullProgress_ThrowsArgumentNullException()
    {
        // Arrange
        var provider = new SavedProgressProvider(new SavedProgress());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => provider.UpdateProgress(null!));
    }

    [Fact]
    public void UpdateProgress_ReplacesCurrentProgress()
    {
        // Arrange
        var initial = new SavedProgress();
        var provider = new SavedProgressProvider(initial);
        var newProgress = new SavedProgress { SpoilerPath = "test.txt" };

        // Act
        provider.UpdateProgress(newProgress);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Same(newProgress, provider.CurrentProgress);
            Assert.Equal("test.txt", provider.CurrentProgress.SpoilerPath);
        });
    }

    [Fact]
    public void UpdateProgress_RaisesProgressChangedEvent()
    {
        // Arrange
        var initial = new SavedProgress();
        var provider = new SavedProgressProvider(initial);
        var newProgress = new SavedProgress();

        var eventFired = false;
        ProgressReplacedEventArgs? eventArgs = null;

        provider.ProgressChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        // Act
        provider.UpdateProgress(newProgress);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(eventFired);
            Assert.NotNull(eventArgs);
            Assert.Same(initial, eventArgs.OldProgress);
            Assert.Same(newProgress, eventArgs.NewProgress);
        });
    }

    [Fact]
    public void UpdateProgress_DeterminesReasonAsReset_WhenOldProgressEmpty()
    {
        // Arrange
        var initialEmpty = new SavedProgress();
        var provider = new SavedProgressProvider(initialEmpty);
        var newProgress = new SavedProgress();

        ProgressReplacedEventArgs? eventArgs = null;
        provider.ProgressChanged += (sender, args) => eventArgs = args;

        // Act
        provider.UpdateProgress(newProgress);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(eventArgs);
            Assert.Equal(ChangeReason.Reset, eventArgs.ChangeReason);
        });
    }

    [Fact]
    public void UpdateProgress_DeterminesReasonAsLoadedFromFile_WhenOldProgressHasData()
    {
        // Arrange
        var initialWithData = new SavedProgress();
        // Add an item to SavedItems to simulate loaded data
        initialWithData.SavedItems[ItemName.DONKEY] =
            new SavedItem(ItemName.DONKEY, RegionName.JUNGLE_JAPES, true, false, 1.0, false);

        var provider = new SavedProgressProvider(initialWithData);
        var newProgress = new SavedProgress();

        ProgressReplacedEventArgs? eventArgs = null;
        provider.ProgressChanged += (sender, args) => eventArgs = args;

        // Act
        provider.UpdateProgress(newProgress);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(eventArgs);
            Assert.Equal(ChangeReason.LoadedFromFile, eventArgs.ChangeReason);
        });
    }

    [Fact]
    public void MultipleUpdates_AllRaiseEvents()
    {
        // Arrange
        var initial = new SavedProgress();
        var provider = new SavedProgressProvider(initial);
        var eventCount = 0;

        provider.ProgressChanged += (sender, args) => eventCount++;

        // Act
        provider.UpdateProgress(new SavedProgress());
        provider.UpdateProgress(new SavedProgress());
        provider.UpdateProgress(new SavedProgress());

        // Assert
        Assert.Equal(3, eventCount);
    }

    [Fact]
    public void CurrentProgress_PropertyAccess_DoesNotConsumeEvent()
    {
        // Arrange
        var initial = new SavedProgress();
        var provider = new SavedProgressProvider(initial);
        var eventCount = 0;

        provider.ProgressChanged += (sender, args) => eventCount++;

        // Act
        var progress1 = provider.CurrentProgress;
        var progress2 = provider.CurrentProgress;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Same(progress1, progress2);
            Assert.Equal(0, eventCount);
        });
    }
}
