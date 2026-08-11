using TrackOMatic.Logic.Events.Autotracking;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// An implementation of the <see cref="ISongService"/> that holds the current song state in memory.
/// </summary>
/// <remarks>This data is intentionally not persisted to the hard drive.</remarks>
public class SongService : ISongService
{
    private static readonly string _defaultMessage = "Waiting for a 4.0+ ROM...";

    private string _songGame = string.Empty;
    private string _songName = string.Empty;

    public event EventHandler<SongChangedEventArgs>? SongChanged;

    public string SongGame => _songGame;
    public string SongName => string.IsNullOrEmpty(_songGame) ? _defaultMessage : _songName;

    public void SetSong(string songGame, string songName)
    {
        var newGame = songGame ?? string.Empty;
        var newName = songName ?? string.Empty;

        // Only raise event if values actually changed
        if (_songGame != newGame || _songName != newName)
        {
            _songGame = newGame;
            _songName = newName;
            SongChanged?.Invoke(this, new(newGame, newName));
        }
    }
}
