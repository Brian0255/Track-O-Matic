using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// Implements ISavedProgressProvider to manage SavedProgress lifecycle and notify services of changes.
/// </summary>
public class SavedProgressProvider(SavedProgress initialProgress) : ISavedProgressProvider
{
    private SavedProgress _currentProgress = initialProgress ?? throw new ArgumentNullException(nameof(initialProgress));

    public SavedProgress CurrentProgress => _currentProgress;

    public event EventHandler<ProgressReplacedEventArgs>? ProgressChanged;

    public void UpdateProgress(SavedProgress newProgress)
    {
        ArgumentNullException.ThrowIfNull(newProgress);

        var oldProgress = _currentProgress;
        _currentProgress = newProgress;

        // Determine reason based on whether old had data
        var reason = oldProgress?.SavedItems.Count > 0 ? ChangeReason.LoadedFromFile : ChangeReason.Reset;

        ProgressChanged?.Invoke(this, new ProgressReplacedEventArgs
        {
            OldProgress = oldProgress,
            NewProgress = newProgress,
            ChangeReason = reason
        });
    }
}
