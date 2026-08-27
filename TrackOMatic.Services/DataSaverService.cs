using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services;

public class DataSaverService : IDataSaverService
{
    private static readonly string AppDataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Track-O-Matic"
    );

    private readonly IDataPersistenceService _dataPersistenceService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISpoilerService _spoilerService;

    private SavedProgress CurrentSavedProgress { get; set; }

    public DataSaverService(IDataPersistenceService dataPersistenceService, IParsedSpoilerDataService parsedSpoilerDataService, ISpoilerService spoilerService)
    {
        _dataPersistenceService = dataPersistenceService;
        _parsedSpoilerDataService = parsedSpoilerDataService;
        _spoilerService = spoilerService;
        CurrentSavedProgress = new();
    }

    public event EventHandler<SavedProgressChangedEventArgs>? SaveCompleted;
    public event EventHandler<string>? SaveFailed;

    public async Task InitWithSpoilerAsync(string spoilerFilePath)
    {
        Reset();

        try
        {
            var spoilerData = await ParseSpoilerLogAsync(spoilerFilePath);
            _parsedSpoilerDataService.UpdateParsedData(spoilerData);
            CurrentSavedProgress.SpoilerPath = spoilerFilePath;

            // We probably do want the AutotrackerService to observe here...
        }
        catch (Exception)
        {
            Reset(); // safety precaution in case the data got corrupted.
            throw;
        }
    }

    public async Task LoadAsync(string filePath)
    {
        SavedProgress? loadedProgress = await _dataPersistenceService.LoadAsync<SavedProgress>(filePath);
        if (loadedProgress is null)
        {
            return;
        }

        CurrentSavedProgress = loadedProgress;
        if (!string.IsNullOrEmpty(CurrentSavedProgress.SpoilerPath))
        {
            try
            {
                var spoilerData = await ParseSpoilerLogAsync(CurrentSavedProgress.SpoilerPath);
            }
            catch (Exception)
            {
                // Handle the exception or log it as needed
            }
        }
        // Sync SavedProgress with provider after loading from file
        var provider = ServiceLocator.GetService<ISavedProgressProvider>();
        provider?.UpdateProgress(CurrentSavedProgress);
    }

    public void Reset()
    {
        CurrentSavedProgress = new();
        // Sync fresh SavedProgress with provider. ...should this be initialized via the constructor?
        var provider = ServiceLocator.GetService<ISavedProgressProvider>();
        if (provider != null && CurrentSavedProgress != null)
        {
            provider.UpdateProgress(CurrentSavedProgress);
        }
    }

    public async Task SaveAsync(string filePath = "autosave.json")
    {
        if (CurrentSavedProgress is null)
        {
            SaveFailed?.Invoke(this, "CurrentSavedProgress is null. Cannot save progress.");
            return;
        }

        try
        {
            // If only a filename is provided, resolve it to the app data directory
            string resolvedPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(AppDataDirectory, filePath);

            await _dataPersistenceService.SaveAsync(CurrentSavedProgress, resolvedPath);
            SaveCompleted?.Invoke(this, new() { FilePath = resolvedPath, Success = true });
        }
        catch (Exception ex)
        {
            SaveFailed?.Invoke(this, $"Failed to save progress: {ex.Message}");
        }
    }

    private async Task<ParsedSpoilerData> ParseSpoilerLogAsync(string spoilerFilePath)
    {
        var result = await _spoilerService.DeserializeAndParseAsync(spoilerFilePath);
        if (!result.IsSuccess || result.Data == null)
        {
            throw new InvalidOperationException($"Failed to parse spoiler: {result.ErrorMessage}");
        }
        return result.Data;
    }
}
