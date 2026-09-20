using TrackOMatic.Logic.Events.Autotracking;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services.Test.TrackerState;

/// <summary>
/// Unit tests for SongService.
/// Tests song state management, property exposure, and event propagation.
/// </summary>
public class SongServiceTests
{
    private readonly SongService _sut;

    public SongServiceTests()
    {
        _sut = new SongService();
    }

    #region Initial State Tests

    [Fact]
    public void Constructor_InitializesEmptyState()
    {
        // Act
        var songGame = _sut.SongGame;
        var songName = _sut.SongName;

        // Assert
        Assert.Equal(string.Empty, songGame);
        Assert.Equal("Waiting for a 4.0+ ROM...", songName);
    }

    [Fact]
    public void SongName_WithEmptySongGame_ReturnsDefaultMessage()
    {
        // Act
        var songName = _sut.SongName;

        // Assert
        Assert.Equal("Waiting for a 4.0+ ROM...", songName);
    }

    #endregion

    #region SetSong Tests

    [Fact]
    public void SetSong_WithValidValues_UpdatesProperties()
    {
        // Act
        _sut.SetSong("DK64", "Jungle Japes");

        // Assert
        Assert.Equal("DK64", _sut.SongGame);
        Assert.Equal("Jungle Japes", _sut.SongName);
    }

    [Fact]
    public void SetSong_WithNullGameValue_TreatsAsEmpty()
    {
        // Act
        _sut.SetSong(null!, "Test Song");

        // Assert
        Assert.Equal(string.Empty, _sut.SongGame);
        Assert.Equal("Waiting for a 4.0+ ROM...", _sut.SongName);
    }

    [Fact]
    public void SetSong_WithNullSongValue_TreatsAsEmpty()
    {
        // Act
        _sut.SetSong("DK64", null!);

        // Assert
        Assert.Equal("DK64", _sut.SongGame);
        Assert.Equal(string.Empty, _sut.SongName);
    }

    [Fact]
    public void SetSong_WithBothNullValues_TreatsAsEmpty()
    {
        // Act
        _sut.SetSong(null!, null!);

        // Assert
        Assert.Equal(string.Empty, _sut.SongGame);
        Assert.Equal("Waiting for a 4.0+ ROM...", _sut.SongName);
    }

    [Fact]
    public void SetSong_WithEmptyStrings_UpdatesProperties()
    {
        // Arrange
        _sut.SetSong("DK64", "Jungle Japes");

        // Act - Reset to empty
        _sut.SetSong(string.Empty, string.Empty);

        // Assert
        Assert.Equal(string.Empty, _sut.SongGame);
        Assert.Equal("Waiting for a 4.0+ ROM...", _sut.SongName);
    }

    #endregion

    #region Event Tests

    [Fact]
    public void SetSong_WithChangedValues_RaisesEvent()
    {
        // Arrange
        var eventFired = false;
        SongChangedEventArgs? eventArgs = null;

        _sut.SongChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        // Act
        _sut.SetSong("DK64", "Jungle Japes");

        // Assert
        Assert.True(eventFired);
        Assert.NotNull(eventArgs);
        Assert.Equal("DK64", eventArgs.SongGame);
        Assert.Equal("Jungle Japes", eventArgs.SongName);
    }

    [Fact]
    public void SetSong_WithUnchangedValues_DoesNotRaiseEvent()
    {
        // Arrange
        _sut.SetSong("DK64", "Jungle Japes");
        var eventFiredCount = 0;

        _sut.SongChanged += (sender, args) => eventFiredCount++;

        // Act
        _sut.SetSong("DK64", "Jungle Japes");

        // Assert
        Assert.Equal(0, eventFiredCount);
    }

    [Fact]
    public void SetSong_WithChangedGameOnly_RaisesEvent()
    {
        // Arrange
        _sut.SetSong("DK64", "Jungle Japes");
        var eventFiredCount = 0;

        _sut.SongChanged += (sender, args) => eventFiredCount++;

        // Act
        _sut.SetSong("DKC", "Jungle Japes");

        // Assert
        Assert.Equal(1, eventFiredCount);
    }

    [Fact]
    public void SetSong_WithChangedNameOnly_RaisesEvent()
    {
        // Arrange
        _sut.SetSong("DK64", "Jungle Japes");
        var eventFiredCount = 0;

        _sut.SongChanged += (sender, args) => eventFiredCount++;

        // Act
        _sut.SetSong("DK64", "Angry Aztec");

        // Assert
        Assert.Equal(1, eventFiredCount);
    }

    [Fact]
    public void SetSong_EventArgs_ContainsNewValues()
    {
        // Arrange
        SongChangedEventArgs? capturedArgs = null;
        _sut.SongChanged += (sender, args) => capturedArgs = args;

        // Act
        _sut.SetSong("New Game", "New Song");

        // Assert
        Assert.NotNull(capturedArgs);
        Assert.Equal("New Game", capturedArgs.SongGame);
        Assert.Equal("New Song", capturedArgs.SongName);
    }

    [Fact]
    public void SetSong_MultipleSubscribers_AllReceiveEvent()
    {
        // Arrange
        var subscriber1Fired = false;
        var subscriber2Fired = false;

        _sut.SongChanged += (sender, args) => subscriber1Fired = true;
        _sut.SongChanged += (sender, args) => subscriber2Fired = true;

        // Act
        _sut.SetSong("DK64", "Song");

        // Assert
        Assert.True(subscriber1Fired);
        Assert.True(subscriber2Fired);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void SetSong_WithWhitespaceValues_TreatsAsNonEmpty()
    {
        // Act
        _sut.SetSong("   ", "   ");

        // Assert
        Assert.Equal("   ", _sut.SongGame);
        Assert.Equal("   ", _sut.SongName);
    }

    [Fact]
    public void SetSong_MultipleUpdates_TracksLatestState()
    {
        // Act
        _sut.SetSong("Game1", "Song1");
        _sut.SetSong("Game2", "Song2");
        _sut.SetSong("Game3", "Song3");

        // Assert
        Assert.Equal("Game3", _sut.SongGame);
        Assert.Equal("Song3", _sut.SongName);
    }

    [Fact]
    public void SetSong_EventFiringSequence_OrderCorrect()
    {
        // Arrange
        var eventSequence = new List<string>();

        _sut.SongChanged += (sender, args) => eventSequence.Add($"{args.SongGame}|{args.SongName}");

        // Act
        _sut.SetSong("Game1", "Song1");
        _sut.SetSong("Game2", "Song2");
        _sut.SetSong("Game3", "Song3");

        // Assert
        Assert.Equal(3, eventSequence.Count);
        Assert.Equal("Game1|Song1", eventSequence[0]);
        Assert.Equal("Game2|Song2", eventSequence[1]);
        Assert.Equal("Game3|Song3", eventSequence[2]);
    }

    #endregion

    #region Event Unsubscription Tests

    [Fact]
    public void SetSong_AfterUnsubscribe_DoesNotRaiseEvent()
    {
        // Arrange
        var eventFired = false;
        void handler(object? sender, SongChangedEventArgs args) => eventFired = true;
        _sut.SongChanged += handler;

        // Act
        _sut.SetSong("Game1", "Song1");
        Assert.True(eventFired);

        eventFired = false;
        _sut.SongChanged -= handler;
        _sut.SetSong("Game2", "Song2");

        // Assert
        Assert.False(eventFired);
    }

    #endregion

    #region SongName Property Logic Tests

    [Fact]
    public void SongName_WhenGameIsSet_ReturnsActualSongName()
    {
        // Arrange
        _sut.SetSong("DK64", "Custom Song");

        // Act
        var songName = _sut.SongName;

        // Assert
        Assert.Equal("Custom Song", songName);
    }

    [Fact]
    public void SongName_WhenGameIsEmpty_ReturnsDefaultMessage()
    {
        // Arrange
        _sut.SetSong(string.Empty, "Some Song");

        // Act
        var songName = _sut.SongName;

        // Assert
        Assert.Equal("Waiting for a 4.0+ ROM...", songName);
    }

    [Fact]
    public void SongName_WhenGameCleared_ReturnsDefaultMessage()
    {
        // Arrange
        _sut.SetSong("DK64", "Jungle Japes");
        Assert.Equal("Jungle Japes", _sut.SongName);

        // Act
        _sut.SetSong(string.Empty, "Jungle Japes");

        // Assert
        Assert.Equal("Waiting for a 4.0+ ROM...", _sut.SongName);
    }

    #endregion
}
