namespace TrackOMatic.Services;

/// <summary>
/// A service for loading and saving any JSON files the app uses.
/// </summary>
public interface IDataPersistenceService
{
    /// <summary>
    /// Asynchronously load data from a JSON file.
    /// </summary>
    /// <typeparam name="T">The type of the data to load.</typeparam>
    /// <param name="filePath">The path to the JSON file.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the loaded data,
    /// or null if the file does not exist or could not be loaded.
    /// </returns>
    Task<T?> LoadAsync<T>(string filePath) where T : class;

    /// <summary>
    /// Asynchronously save data to a JSON file.
    /// </summary>
    /// <typeparam name="T">The type of the data to save.</typeparam>
    /// <param name="data">The data to save.</param>
    /// <param name="filePath">The path to the JSON file.</param>
    /// <param name="ensureDirectory">Whether to ensure the directory exists before saving.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="IOException">Thrown if an I/O error occurs while saving the file.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the data to save is null.</exception>
    Task SaveAsync<T>(T data, string filePath, bool ensureDirectory = true) where T : class;
}
