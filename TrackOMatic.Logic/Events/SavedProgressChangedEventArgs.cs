using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Events;

public class SavedProgressChangedEventArgs: EventChangeData
{
    /// <summary>
    /// Gets the file path where the data was saved.
    /// </summary>
    public string? FilePath { get; init; }

    /// <summary>
    /// Gets the error message if the operation failed. Successful saves should have this as null.
    /// </summary>
    public string? ErrorMessage { get; init; }

    public bool Success { get; init; }
}
