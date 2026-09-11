using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Events;

public class DataLoadingCompletedEventArgs(SavedProgress loadedProgress) : EventArgs
{
    /// <summary>
    /// Gets the <see cref="SavedProgress"/> that was just loaded.
    /// </summary>
    public SavedProgress LoadedProgress { get; } = loadedProgress ?? throw new ArgumentNullException(nameof(loadedProgress));

    /// <summary>
    /// Gets a flag indicating whether a spoiler file was loaded.
    /// </summary>
    public bool HasSpoiler => !string.IsNullOrEmpty(LoadedProgress.SpoilerPath);
}
