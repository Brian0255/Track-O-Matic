using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services.Test.TrackerState;

/// <summary>
/// Unit tests for LevelOrderService.
/// Tests level order state management and event firing.
/// </summary>
public class LevelOrderServiceTests
{
    private readonly SavedProgress _progress;
    private readonly TestSavedProgressProvider _provider;
    private readonly LevelOrderService _sut;

    public LevelOrderServiceTests()
    {
        _progress = new SavedProgress
        {
            LevelOrder = [0, 1, 2, 3, 4, 5, 6, 7]
        };
        _provider = new TestSavedProgressProvider(_progress);
        // TODO: Mock this in the future.
        _sut = new LevelOrderService(_provider, new UserSettingsService());
    }

    #region GetLevelOrder Tests

    [Fact]
    public void GetLevelOrder_ReturnsReadOnlyList()
    {
        // Act
        var order = _sut.GetLevelOrder();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal([0, 1, 2, 3, 4, 5, 6, 7], order);
            Assert.IsType<IReadOnlyList<int>>(order, exactMatch: false);
        });
    }

    [Fact]
    public void GetLevelOrder_WithCustomOrder_ReturnsCustomOrder()
    {
        // Arrange
        _progress.LevelOrder = [7, 6, 5, 4, 3, 2, 1, 0];

        // Act
        var order = _sut.GetLevelOrder();

        // Assert
        Assert.Equal([7, 6, 5, 4, 3, 2, 1, 0], order);
    }

    #endregion

    #region SetLevelOrder Tests

    [Fact]
    public void SetLevelOrder_UpdatesOrder()
    {
        // Arrange
        var newOrder = new List<int> { 1, 0, 3, 2, 5, 4, 7, 6 };

        // Act
        _sut.SetLevelOrder(newOrder);

        // Assert
        Assert.Equal(newOrder, _sut.GetLevelOrder());
    }

    [Fact]
    public void SetLevelOrder_RaisesEvent()
    {
        // Arrange
        var eventFired = false;
        LevelOrderStateChangedEventArgs? eventArgs = null;

        _sut.LevelOrderChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        var newOrder = new List<int> { 7, 6, 5, 4, 3, 2, 1, 0 };

        // Act
        _sut.SetLevelOrder(newOrder);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(eventFired);
            Assert.NotNull(eventArgs);
            Assert.Equal([0, 1, 2, 3, 4, 5, 6, 7], eventArgs.OldValue);
            Assert.Equal([7, 6, 5, 4, 3, 2, 1, 0], eventArgs.NewValue);
        });
    }

    [Fact]
    public void SetLevelOrder_AllowsEnumerable()
    {
        // Arrange
        IEnumerable<int> newOrder = [2, 0, 1, 4, 5, 3, 6, 7];

        // Act
        _sut.SetLevelOrder(newOrder);

        // Assert
        Assert.Equal([2, 0, 1, 4, 5, 3, 6, 7], _sut.GetLevelOrder());
    }

    #endregion

    #region Progress Replacement Tests

    [Fact]
    public void OnProgressChanged_ReinitializesWithNewProgress()
    {
        // Arrange
        var newProgress = new SavedProgress
        {
            LevelOrder = [3, 1, 4, 1, 5, 9, 2, 6]
        };

        // Act
        _provider.UpdateProgress(newProgress);

        // Assert
        Assert.Equal([3, 1, 4, 1, 5, 9, 2, 6], _sut.GetLevelOrder());
    }

    #endregion
}
