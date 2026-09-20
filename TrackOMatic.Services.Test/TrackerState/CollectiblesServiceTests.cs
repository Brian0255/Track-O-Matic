using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services.Test.TrackerState;

/// <summary>
/// Unit tests for CollectiblesService.
/// Tests collectible state management and event firing.
/// </summary>
public class CollectiblesServiceTests
{
    private readonly SavedProgress _progress;
    private readonly TestSavedProgressProvider _provider;
    private readonly CollectiblesService _sut;

    public CollectiblesServiceTests()
    {
        _progress = new SavedProgress();
        _provider = new TestSavedProgressProvider(_progress);
        _sut = new CollectiblesService(_provider);
    }

    #region GetCollectibleCount Tests

    [Fact]
    public void GetCollectibleCount_WithNoData_ReturnsZero()
    {
        // Act
        var count = _sut.GetCollectibleCount(ItemType.GOLDEN_BANANA);

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public void GetCollectibleCount_WithExistingValue_ReturnsValue()
    {
        // Arrange
        _progress.Collectibles[ItemType.DONKEY_BLUEPRINT] = 5;

        // Act
        var count = _sut.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT);

        // Assert
        Assert.Equal(5, count);
    }

    [Fact]
    public void GetCollectibleCount_WithNegativeValue_ReturnsZero()
    {
        // Arrange
        _progress.Collectibles[ItemType.RAINBOW_COIN] = -3;

        // Act
        var count = _sut.GetCollectibleCount(ItemType.RAINBOW_COIN);

        // Assert
        Assert.Equal(0, count);
    }

    #endregion

    #region IncrementCollectible Tests

    [Fact]
    public void IncrementCollectible_FromZero_RaisesEvent()
    {
        // Arrange
        var eventFired = false;
        CollectiblesChangedEventArgs? eventArgs = null;

        _sut.CollectibleChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        // Act
        _sut.IncrementCollectible(ItemType.GOLDEN_BANANA);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(eventFired);
            Assert.NotNull(eventArgs);
            Assert.Equal(ItemType.GOLDEN_BANANA, eventArgs.ItemType);
            Assert.Equal(0, eventArgs.OldValue);
            Assert.Equal(1, eventArgs.NewValue);
        });
    }

    [Fact]
    public void IncrementCollectible_MultipleTimesIncrementsCorrectly()
    {
        // Arrange
        _progress.Collectibles[ItemType.DONKEY_BLUEPRINT] = 3;

        // Act
        _sut.IncrementCollectible(ItemType.DONKEY_BLUEPRINT);
        _sut.IncrementCollectible(ItemType.DONKEY_BLUEPRINT);

        // Assert
        Assert.Equal(5, _progress.Collectibles[ItemType.DONKEY_BLUEPRINT]);
    }

    [Fact]
    public void IncrementCollectible_RaisesEventEachTime()
    {
        // Arrange
        var eventCount = 0;
        _sut.CollectibleChanged += (sender, args) => eventCount++;

        // Act
        _sut.IncrementCollectible(ItemType.RAINBOW_COIN);
        _sut.IncrementCollectible(ItemType.RAINBOW_COIN);

        // Assert
        Assert.Equal(2, eventCount);
    }

    #endregion

    #region DecrementCollectible Tests

    [Fact]
    public void DecrementCollectible_FromPositive_Decreases()
    {
        // Arrange
        _progress.Collectibles[ItemType.DONKEY_BLUEPRINT] = 5;

        // Act
        _sut.DecrementCollectible(ItemType.DONKEY_BLUEPRINT);

        // Assert
        Assert.Equal(4, _progress.Collectibles[ItemType.DONKEY_BLUEPRINT]);
    }

    [Fact]
    public void DecrementCollectible_NeverGoesNegative()
    {
        // Arrange
        _progress.Collectibles[ItemType.RAINBOW_COIN] = 1;

        // Act
        _sut.DecrementCollectible(ItemType.RAINBOW_COIN);
        _sut.DecrementCollectible(ItemType.RAINBOW_COIN);
        _sut.DecrementCollectible(ItemType.RAINBOW_COIN);

        // Assert
        Assert.Equal(0, _progress.Collectibles[ItemType.RAINBOW_COIN]);
    }

    [Fact]
    public void DecrementCollectible_RaisesEvent()
    {
        // Arrange
        _progress.Collectibles[ItemType.GOLDEN_BANANA] = 3;
        var eventFired = false;

        _sut.CollectibleChanged += (sender, args) => eventFired = true;

        // Act
        _sut.DecrementCollectible(ItemType.GOLDEN_BANANA);

        // Assert
        Assert.True(eventFired);
    }

    #endregion

    #region SetCollectibleCount Tests

    [Fact]
    public void SetCollectibleCount_ToNewValue_Updates()
    {
        // Arrange
        _progress.Collectibles[ItemType.DONKEY_BLUEPRINT] = 2;

        // Act
        _sut.SetCollectibleCount(ItemType.DONKEY_BLUEPRINT, 7);

        // Assert
        Assert.Equal(7, _progress.Collectibles[ItemType.DONKEY_BLUEPRINT]);
    }

    [Fact]
    public void SetCollectibleCount_ToNegative_ClampsToZero()
    {
        // Act
        _sut.SetCollectibleCount(ItemType.RAINBOW_COIN, -5);

        // Assert
        Assert.Equal(0, _sut.GetCollectibleCount(ItemType.RAINBOW_COIN));
    }

    [Fact]
    public void SetCollectibleCount_ToSameValue_NoEvent()
    {
        // Arrange
        _progress.Collectibles[ItemType.DONKEY_BLUEPRINT] = 5;
        var eventCount = 0;

        _sut.CollectibleChanged += (sender, args) => eventCount++;

        // Act
        _sut.SetCollectibleCount(ItemType.DONKEY_BLUEPRINT, 5);

        // Assert
        Assert.Equal(0, eventCount);
    }

    [Fact]
    public void SetCollectibleCount_ToNewValue_RaisesEvent()
    {
        // Arrange
        var eventFired = false;
        CollectiblesChangedEventArgs? eventArgs = null;

        _sut.CollectibleChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        // Act
        _sut.SetCollectibleCount(ItemType.GOLDEN_BANANA, 42);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(eventFired);
            Assert.NotNull(eventArgs);
            Assert.Equal(0, eventArgs.OldValue);
            Assert.Equal(42, eventArgs.NewValue);
        });
    }

    #endregion

    #region Multiple Collectibles

    [Fact]
    public void MultipleCollectibles_MaintainIndependentState()
    {
        // Arrange & Act
        _sut.SetCollectibleCount(ItemType.DONKEY_BLUEPRINT, 3);
        _sut.SetCollectibleCount(ItemType.RAINBOW_COIN, 7);
        _sut.SetCollectibleCount(ItemType.GOLDEN_BANANA, 5);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(3, _sut.GetCollectibleCount(ItemType.DONKEY_BLUEPRINT));
            Assert.Equal(7, _sut.GetCollectibleCount(ItemType.RAINBOW_COIN));
            Assert.Equal(5, _sut.GetCollectibleCount(ItemType.GOLDEN_BANANA));
        });
    }

    #endregion
}

/// <summary>
/// Test implementation of ISavedProgressProvider for testing.
/// </summary>
internal class TestSavedProgressProvider(SavedProgress initialProgress) : ISavedProgressProvider
{
    private SavedProgress _currentProgress = initialProgress;

    public SavedProgress CurrentProgress => _currentProgress;

    public event EventHandler<ProgressReplacedEventArgs>? ProgressChanged;

    public void UpdateProgress(SavedProgress newProgress)
    {
        _currentProgress = newProgress;
        ProgressChanged?.Invoke(this, new ProgressReplacedEventArgs { NewProgress = newProgress });
    }
}
