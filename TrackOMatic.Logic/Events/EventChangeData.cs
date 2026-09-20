using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events;

public abstract class EventChangeData : EventArgs
{
    /// <summary>
    /// Gets the reason for the change.
    /// </summary>
    public ChangeReason ChangeReason { get; init; }

    /// <summary>
    /// Gets the timestamp when the value change occurred.
    /// </summary>
    public DateTime ChangedAt { get; init; } = DateTime.UtcNow;
}
