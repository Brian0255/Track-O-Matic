using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services;

/// <summary>
/// An interface to handle saving and loading tracker data.
/// </summary>
public interface IDataSaverService
{
    /// <summary>
    /// Resets the saved progress to its default state.
    /// </summary>
    void Reset();

    /// <summary>
    /// Initializes the saved progress with a spoiler file to parse.
    /// </summary>
    /// <param name="spoilerFilePath">The path to the spoiler file.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InitWithSpoilerAsync(string spoilerFilePath);

    /// <summary>
    /// Saves the current tracker state to a JSON file.
    /// </summary>
    /// <param name="filePath">The path to the file where the tracker state will be saved.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SaveAsync(string filePath = "autosave.json");

    /// <summary>
    /// Loads the tracker state from a JSON file.
    /// </summary>
    /// <param name="filePath">The path to the file from which the tracker state will be loaded.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task LoadAsync(string filePath);

    /// <summary>
    /// An event that is triggered when the save operation is completed successfully.
    /// </summary>
    event EventHandler<SavedProgressChangedEventArgs>? SaveCompleted;
    /// <summary>
    /// An event that is triggered when the save operation fails.
    /// </summary>
    event EventHandler<string>? SaveFailed;
}
