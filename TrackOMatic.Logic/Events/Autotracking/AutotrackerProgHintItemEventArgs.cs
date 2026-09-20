using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events.Autotracking;

/// <summary>
/// Arguments for progressive hint item update events.
/// </summary>
public class AutotrackerProgHintItemEventArgs : EventArgs
{
    public ItemType ProgHintItem { get; set; }
}
