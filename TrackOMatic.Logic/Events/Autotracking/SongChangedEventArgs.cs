namespace TrackOMatic.Logic.Events.Autotracking;

public class SongChangedEventArgs(string songGame, string songName) : EventArgs
{
    public string SongGame { get; } = songGame;
    public string SongName { get; } = songName;
}
