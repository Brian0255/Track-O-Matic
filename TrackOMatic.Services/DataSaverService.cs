using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services;

public class DataSaverService(
    IDataPersistenceService dataPersistenceService,
    IParsedSpoilerDataService parsedSpoilerDataService,
    IItemTrackingService itemTrackingService,
    IHintService hintService,
    ISavedProgressProvider savedProgressProvider,
    ISpoilerService spoilerService,
    IBarrierService BarrierService,
    IEndGameProgressionService endGameProgressionService,
    ILevelOrderService levelOrderService
) : IDataSaverService
{
    private static readonly string _appDataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Track-O-Matic"
    );

    private readonly IDataPersistenceService _dataPersistenceService = dataPersistenceService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService = parsedSpoilerDataService;
    private readonly IItemTrackingService _itemTrackingService = itemTrackingService;
    private readonly IHintService _hintService = hintService;
    private readonly ISavedProgressProvider _savedProgressProvider = savedProgressProvider;
    private readonly ISpoilerService _spoilerService = spoilerService;
    private readonly IBarrierService _barrierService = BarrierService;
    private readonly IEndGameProgressionService _endGameProgressionService = endGameProgressionService;
    private readonly ILevelOrderService _levelOrderService = levelOrderService;

    private SavedProgress CurrentSavedProgress { get; set; } = new();

    public event EventHandler<SavedProgressChangedEventArgs>? SaveCompleted;
    public event EventHandler<DataLoadingCompletedEventArgs>? DataLoadingCompleted;
    public event EventHandler<string>? SaveFailed;

    public async Task InitWithSpoilerAsync(string spoilerFilePath)
    {
        SavedProgress progress = new() { SpoilerPath = spoilerFilePath };
        await LoadProgressStateAsync(progress, skipExistingItems: true);
    }

    public async Task LoadAsync(string filePath)
    {
        SavedProgress? loadedProgress = await _dataPersistenceService.LoadAsync<SavedProgress>(filePath);
        if (loadedProgress is null)
        {
            return;
        }

        await LoadProgressStateAsync(loadedProgress, skipExistingItems: false);
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
            string resolvedPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(_appDataDirectory, filePath);
            await _dataPersistenceService.SaveAsync(CreateFilteredProgressSnapshot(), resolvedPath);
            SaveCompleted?.Invoke(this, new() { FilePath = resolvedPath, Success = true });
        }
        catch (Exception ex)
        {
            SaveFailed?.Invoke(this, $"Failed to save progress: {ex.Message}");
        }
    }

    /// <summary>
    /// Common logic for loading progress state into services silently.
    /// Used by both InitWithSpoilerAsync (new spoiler only) and LoadAsync (with merge).
    /// </summary>
    /// <param name="progress">The SavedProgress to load (either new or from file)</param>
    /// <param name="skipExistingItems">If true, ignore progress.SavedItems and only use spoiler starting items</param>
    private async Task LoadProgressStateAsync(SavedProgress progress, bool skipExistingItems)
    {
        try
        {
            // PHASE 1: Load spoiler data silently (if present)
            if (!string.IsNullOrEmpty(progress.SpoilerPath))
            {
                try
                {
                    var spoilerData = await ParseSpoilerLogAsync(progress.SpoilerPath);
                    _parsedSpoilerDataService.LoadParsedDataSilently(spoilerData);
                }
                catch (Exception)
                {
                    // Spoiler failed to load, continue without it
                    _parsedSpoilerDataService.LoadParsedDataSilently(null);
                }
            }
            else
            {
                _parsedSpoilerDataService.LoadParsedDataSilently(null);
            }

            // PHASE 2: Prepare items (spoiler starting items + existing items if skipExistingItems is false)
            var itemsToLoad = new Dictionary<ItemName, SavedItem>();

            // Start with spoiler starting items
            if (_parsedSpoilerDataService.CurrentData != null)
            {
                foreach (var (itemName, count) in _parsedSpoilerDataService.GetStartingItems())
                {
                    itemsToLoad[itemName] = new SavedItem(itemName, count, autotracked: true);
                }
            }

            // Layer existing items if we're loading a full save file (manual data wins on conflicts)
            if (!skipExistingItems)
            {
                foreach (var (itemName, savedItem) in progress.SavedItems)
                {
                    itemsToLoad[itemName] = savedItem;
                }
            }

            // PHASE 3: Load all items silently (no events fired)
            _itemTrackingService.LoadItemsSilently(itemsToLoad);

            // PHASE 4: Load hints silently (no events fired)
            var hintsToLoad = skipExistingItems ? [] : progress.SavedHints;
            _hintService.LoadHintsSilently(hintsToLoad);

            _barrierService.LoadBLockerCountsSilently(progress.SavedGBCounts);
            _barrierService.LoadBLockerTypesSilently(progress.BLockerImageIndexes);
            _barrierService.LoadHelmDoorCountsSilently(progress.HelmDoorCounts);
            _barrierService.LoadHelmDoorTypesSilently(progress.HelmDoorImageIndexes);

            _endGameProgressionService.SetHelmKongsSilently(progress.HelmKongs);
            _endGameProgressionService.SetBossKongsSilently(progress.BossKongs);

            _levelOrderService.LoadLevelOrderSilently(progress.LevelOrder);

            // PHASE 5: Update saved progress
            CurrentSavedProgress = progress;

            // PHASE 6: Alert UI that loading is complete
            DataLoadingCompleted?.Invoke(this, new DataLoadingCompletedEventArgs(progress));
        }
        catch (Exception)
        {
            // On error, reset to clean state
            CurrentSavedProgress = new();
            _parsedSpoilerDataService.LoadParsedDataSilently(null);
            _itemTrackingService.LoadItemsSilently([]);
            _hintService.LoadHintsSilently([]);
            throw;
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
