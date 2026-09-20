namespace TrackOMatic.Logic.Events.Autotracking;

/// <summary>
/// Arguments for hint progress update events.
/// </summary>
public class AutotrackerHintEventArgs : EventArgs
{
    public int AmountToNextHint { get; set; }
}
