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
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IHintService _hintService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly ISpoilerService _spoilerService;

    private SavedProgress CurrentSavedProgress { get; set; }

    public DataSaverService(
        IDataPersistenceService dataPersistenceService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IItemTrackingService itemTrackingService,
        IHintService hintService,
        ISavedProgressProvider savedProgressProvider,
        ISpoilerService spoilerService
    )
    {
        _dataPersistenceService = dataPersistenceService;
        _parsedSpoilerDataService = parsedSpoilerDataService;
        _itemTrackingService = itemTrackingService;
        _hintService = hintService;
        _savedProgressProvider = savedProgressProvider;
        _spoilerService = spoilerService;
        CurrentSavedProgress = new();
    }

    public event EventHandler<SavedProgressChangedEventArgs>? SaveCompleted;
    public event EventHandler<string>? SaveFailed;

    public async Task InitWithSpoilerAsync(string spoilerFilePath)
    {
        CurrentSavedProgress = new();

        try
        {
            // Perform batch updates to minimize issues with a new load.
            using (_parsedSpoilerDataService.BeginBatchUpdate())
            using (_itemTrackingService.BeginBatchUpdate())
            using (_hintService.BeginBatchUpdate())
            {
                var spoilerData = await ParseSpoilerLogAsync(spoilerFilePath);
                _parsedSpoilerDataService.UpdateParsedData(spoilerData);
                CurrentSavedProgress.SpoilerPath = spoilerFilePath;

                // Sync fresh progress WITHIN the batch scopes
                // This ensures OnProgressChanged doesn't trigger secondary UpdateParsedData(null) or item-change events
                _savedProgressProvider.UpdateProgress(CurrentSavedProgress);

                foreach (var item in _parsedSpoilerDataService.GetStartingItems())
                {
                    _itemTrackingService.SetItemState(item.Key, new(item.Key, item.Value, autotracked: true));
                }
            }

            // We probably do want the AutotrackerService to observe here...
        }
        catch (Exception)
        {
            CurrentSavedProgress = new();
            _savedProgressProvider.UpdateProgress(CurrentSavedProgress);
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

        // Perform all initializations in batch mode FIRST, then notify subscribers
        // This ensures all caches are populated before OnProgressChanged handlers fire
        using (_parsedSpoilerDataService.BeginBatchUpdate())
        using (_itemTrackingService.BeginBatchUpdate())
        using (_hintService.BeginBatchUpdate())
        {
            if (!string.IsNullOrEmpty(CurrentSavedProgress.SpoilerPath))
            {
                try
                {
                    var spoilerData = await ParseSpoilerLogAsync(CurrentSavedProgress.SpoilerPath);
                    _parsedSpoilerDataService.UpdateParsedData(spoilerData);

                    foreach (var item in _parsedSpoilerDataService.GetStartingItems())
                    {
                        _itemTrackingService.SetItemState(item.Key, new(item.Key, item.Value, autotracked: true));
                    }
                }
                catch (Exception)
                {
                    // Handle the exception or log it as needed
                }
            }
            else
            {
                // No spoiler path set: this is fine. Just make sure the parsed data is cleared.
                _parsedSpoilerDataService.UpdateParsedData(null);
            }
        }
        // Exit batch scopes here - all caches are now populated

        // NOW notify subscribers that progress has been replaced
        // This ensures HintPanelViewModel.OnProgressChanged fires AFTER HintService cache is ready
        _savedProgressProvider.UpdateProgress(CurrentSavedProgress);
    }

    public void Reset()
    {
        CurrentSavedProgress = new();
        _savedProgressProvider.UpdateProgress(CurrentSavedProgress);
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
            // Ensure the order of saved items is consistent before saving
            _itemTrackingService.SyncSavedItemsOrder();
            // If only a filename is provided, resolve it to the app data directory
            string resolvedPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(AppDataDirectory, filePath);
            await _dataPersistenceService.SaveAsync(CreateFilteredProgressSnapshot(), resolvedPath);
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

    private SavedProgress CreateFilteredProgressSnapshot()
    {
        // Start with a shallow copy of CurrentSavedProgress (new collection objects, same items)
        var snapshot = CurrentSavedProgress.CreateShallowCopy();

        // Rebuild SavedItems in insertion order with filtering applied
        snapshot.SavedItems.Clear();
        var itemsInOrder = _itemTrackingService.GetAllItemsInOrder();
        foreach (var item in itemsInOrder)
        {
            // Filter: exclude non-starred autotracked items
            if (!item.IsStartingAutotracked || item.Starred)
            {
                snapshot.SavedItems[item.ItemName] = item;
            }
        }

        return snapshot;
    }
}
