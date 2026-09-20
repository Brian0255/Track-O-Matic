using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events.Autotracking;

/// <summary>
/// Arguments for region lighting change events.
/// </summary>
public class AutotrackerRegionEventArgs : EventArgs
{
    public RegionName Region { get; set; }
    public bool LightUp { get; set; }
}
