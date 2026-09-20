using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Events;

/// <summary>
/// Fired when the SavedProgress instance is replaced (e.g., during Reset or Load).
/// Services that hold a reference to SavedProgress should subscribe to this event
/// and re-initialize when the instance changes.
/// </summary>
public class ProgressReplacedEventArgs : EventChangeData
{
    /// <summary>
    /// Gets the previous SavedProgress instance (may be null if this is the initial load).
    /// </summary>
    public SavedProgress? OldProgress { get; init; }

    /// <summary>
    /// Gets the new SavedProgress instance.
    /// </summary>
    public required SavedProgress NewProgress { get; init; }
}
