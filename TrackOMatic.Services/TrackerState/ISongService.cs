using TrackOMatic.Logic.Events.Autotracking;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// A service to manage the current song state from an attached emulator.
/// This is meant to provide a centralized way to access song information.
/// </summary>
/// <remarks>This data is intentionally not persisted to the hard drive.</remarks>
public interface ISongService
{
    /// <summary>
    /// Raised when song information changes.
    /// </summary>
    event EventHandler<SongChangedEventArgs>? SongChanged;

    /// <summary>
    /// Gets the current game name associated with the song.
    /// </summary>
    string SongGame { get; }

    /// <summary>
    /// Gets the current song name.
    /// </summary>
    string SongName { get; }

    /// <summary>
    /// Sets the current song information.
    /// Raises SongChanged event only if values actually change.
    /// </summary>
    /// <param name="songGame">The game/context name for the song</param>
    /// <param name="songName">The name of the song</param>
    void SetSong(string songGame, string songName);
}
