using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// Provides access to SavedProgress and notifies services when it changes.
/// Used to synchronize services when Reset or Load operations create a new SavedProgress instance.
/// </summary>
public interface ISavedProgressProvider
{
    /// <summary>
    /// Gets the current SavedProgress instance.
    /// </summary>
    SavedProgress CurrentProgress { get; }

    /// <summary>
    /// Called when SavedProgress is replaced (e.g., during Reset or Load).
    /// Services holding a reference should subscribe to this event and re-initialize.
    /// </summary>
    event EventHandler<ProgressReplacedEventArgs>? ProgressChanged;

    /// <summary>
    /// Updates the current SavedProgress instance and fires ProgressChanged event.
    /// Called by SavedProgressProvider implementation when Reset() or Load() replaces the instance.
    /// </summary>
    void UpdateProgress(SavedProgress newProgress);
}
