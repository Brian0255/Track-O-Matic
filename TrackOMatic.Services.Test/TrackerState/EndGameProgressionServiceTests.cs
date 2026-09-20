using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services.Test.TrackerState;

/// <summary>
/// Unit tests for EndGameProgressionService.
/// Tests Helm Kongs and Boss Kongs state management.
/// </summary>
public class EndGameProgressionServiceTests
{
    private readonly SavedProgress _progress;
    private readonly TestSavedProgressProvider _provider;
    private readonly EndGameProgressionService _sut;

    public EndGameProgressionServiceTests()
    {
        _progress = new SavedProgress
        {
            HelmKongs = [0, 0, 0, 0, 0],
            BossKongs = [0, 0, 0, 0, 0]
        };
        _provider = new TestSavedProgressProvider(_progress);
        _sut = new EndGameProgressionService(_provider);
    }

    #region GetHelmKongs Tests

    [Fact]
    public void GetHelmKongs_ReturnsReadOnlyList()
    {
        // Arrange
        _progress.HelmKongs = [1, 2, 3, 4, 5];

        // Act
        var kongs = _sut.GetHelmKongs();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal([1, 2, 3, 4, 5], kongs);
            Assert.IsType<IReadOnlyList<int>>(kongs, exactMatch: false);
        });
    }

    [Fact]
    public void GetHelmKongs_WithEmptyList_ReturnsEmpty()
    {
        // Arrange
        _progress.HelmKongs = [];

        // Act
        var kongs = _sut.GetHelmKongs();

        // Assert
        Assert.Empty(kongs);
    }

    #endregion

    #region SetHelmKongs Tests

    [Fact]
    public void SetHelmKongs_UpdatesList()
    {
        // Arrange
        var newKongs = new List<int> { 1, 0, 1, 0, 1 };

        // Act
        _sut.SetHelmKongs(newKongs);

        // Assert
        Assert.Equal(newKongs, _sut.GetHelmKongs());
    }

    [Fact]
    public void SetHelmKongs_RaisesEvent()
    {
        // Arrange
        var eventFired = false;
        BlastStateChangedEventArgs? eventArgs = null;

        _sut.BlastStateChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        var newKongs = new List<int> { 1, 1, 1, 1, 1 };

        // Act
        _sut.SetHelmKongs(newKongs);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(eventFired);
            Assert.NotNull(eventArgs);
            Assert.Equal([0, 0, 0, 0, 0], eventArgs.OldValue);
            Assert.Equal([1, 1, 1, 1, 1], eventArgs.NewValue);
        });
    }

    [Fact]
    public void SetHelmKongs_AllowsEnumerable()
    {
        // Arrange
        IEnumerable<int> newKongs = Enumerable.Range(1, 5);

        // Act
        _sut.SetHelmKongs(newKongs);

        // Assert
        Assert.Equal([1, 2, 3, 4, 5], _sut.GetHelmKongs());
    }

    #endregion

    #region GetBossKongs Tests

    [Fact]
    public void GetBossKongs_ReturnsReadOnlyList()
    {
        // Arrange
        _progress.BossKongs = [1, 1, 1, 1, 1];

        // Act
        var kongs = _sut.GetBossKongs();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal([1, 1, 1, 1, 1], kongs);
            Assert.IsType<IReadOnlyList<int>>(kongs, exactMatch: false);
        });
    }

    [Fact]
    public void GetBossKongs_WithEmptyList_ReturnsEmpty()
    {
        // Arrange
        _progress.BossKongs = [];

        // Act
        var kongs = _sut.GetBossKongs();

        // Assert
        Assert.Empty(kongs);
    }

    #endregion

    #region SetBossKongs Tests

    [Fact]
    public void SetBossKongs_UpdatesList()
    {
        // Arrange
        var newKongs = new List<int> { 0, 1, 0, 1, 0 };

        // Act
        _sut.SetBossKongs(newKongs);

        // Assert
        Assert.Equal(newKongs, _sut.GetBossKongs());
    }

    [Fact]
    public void SetBossKongs_RaisesEvent()
    {
        // Arrange
        var eventFired = false;
        GauntletStateChangedEventArgs? eventArgs = null;

        _sut.GauntletStateChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        var newKongs = new List<int> { 1, 1, 0, 0, 1 };

        // Act
        _sut.SetBossKongs(newKongs);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(eventFired);
            Assert.NotNull(eventArgs);
            Assert.Equal([0, 0, 0, 0, 0], eventArgs.OldValue);
            Assert.Equal([1, 1, 0, 0, 1], eventArgs.NewValue);
        });
    }

    #endregion

    #region Progress Replacement Tests

    [Fact]
    public void OnProgressChanged_ReinitializesWithNewProgress()
    {
        // Arrange
        var newProgress = new SavedProgress
        {
            HelmKongs = [1, 1, 1, 1, 1],
            BossKongs = [0, 0, 0, 0, 0]
        };

        // Act
        _provider.UpdateProgress(newProgress);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal([1, 1, 1, 1, 1], _sut.GetHelmKongs());
            Assert.Equal([0, 0, 0, 0, 0], _sut.GetBossKongs());
        });
    }

    #endregion
}
