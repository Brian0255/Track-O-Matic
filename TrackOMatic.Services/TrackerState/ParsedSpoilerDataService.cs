using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// A service for managing the currently loaded parsed spoiler data.
/// This service maintains state across tracker resets and load operations via <see cref="ISavedProgressProvider"/>.
/// </summary>
public class ParsedSpoilerDataService : IParsedSpoilerDataService
{
    private readonly ISavedProgressProvider _progressProvider;
    private ParsedSpoilerData? _currentData;
    private bool _isBatchingUpdates;
    private ParsedSpoilerData? _deferredOldData;
    private ParsedSpoilerData? _deferredNewData;

    public event EventHandler<ParsedSpoilerDataChangedEventArgs>? ParsedSpoilerDataChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedSpoilerDataService"/> class.
    /// </summary>
    /// <param name="progressProvider">The provider for saved progress state changes.</param>
    public ParsedSpoilerDataService(ISavedProgressProvider progressProvider)
    {
        _progressProvider = progressProvider ?? throw new ArgumentNullException(nameof(progressProvider));
        _currentData = null;
        _isBatchingUpdates = false;
        _deferredOldData = null;
        _deferredNewData = null;
        // Set up listener for SavedProgress changes (e.g., reset or load)
        _progressProvider.ProgressChanged += OnProgressChanged;
    }

    /// <summary>
    /// Handles progress state changes by clearing the parsed spoiler data.
    /// However, if we're already in a batch update scope (e.g., during spoiler load),
    /// this is handled by the LoadAsync/InitWithSpoilerAsync methods and we should not
    /// interfere with their deferred updates.
    /// </summary>
    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e)
    {
        if (!_isBatchingUpdates)
        {
            UpdateParsedData(null);
        }
    }

    #region IParsedSpoilerDataService Implementation

    public ParsedSpoilerData? CurrentData => _currentData;

    public void UpdateParsedData(ParsedSpoilerData? data)
    {
        var oldData = _currentData;
        _currentData = data;

        if (_isBatchingUpdates)
        {
            _deferredOldData = oldData;
            _deferredNewData = data;
        }
        else
        {
            ParsedSpoilerDataChanged?.Invoke(this, new ParsedSpoilerDataChangedEventArgs(oldData, data));
        }
    }

    public int GetPointsForItem(ItemName itemName)
    {
        var pointSpread = _currentData?.PointSpread ?? [];
        if (pointSpread is null || pointSpread.Count == 0)
        {
            return 0; // No data available, return 0 points
        }
        var pointCategory = itemName.ToPointCategory();
        return pointSpread.TryGetValue(pointCategory, out var points) ? points : 0;
    }

    public int GetPointsForRegion(RegionName regionName)
    {
        if (CurrentData is null)
        {
            return int.MinValue; // Any negative value will do since 0 may mean no points.
        }
        var kvp = CurrentData.RegionData.FirstOrDefault(kvp => kvp.Key == regionName);
        return kvp.Value?.Points ?? 0;
    }

    public IDictionary<PointCategory, int> GetPointSpread() => _currentData?.PointSpread ?? [];

    public int GetWothPointsForRegion(RegionName regionName)
    {
        if (CurrentData is null)
        {
            return int.MinValue; // Any negative value will do since 0 may mean no WOTH items.
        }
        var kvp = CurrentData.RegionData.FirstOrDefault(kvp => kvp.Key == regionName);
        var wothPoints = kvp.Value?.WothCount ?? int.MinValue;
        return wothPoints;
    }

    public bool HasItemPoints()
    {
        if (CurrentData is null)
        {
            return false;
        }
        return CurrentData.RegionData.Any(kvp => kvp.Value.Points > 0);
    }

    public bool HasHoardPoints()
    {
        if (CurrentData is null)
        {
            return false;
        }
        return CurrentData.RegionData.Any(kvp => kvp.Value.WothCount > 0);
    }

    public SpoilerSettings GetSpoilerSettings() => _currentData?.SpoilerSettings ?? new();

    public bool HasHelmOrder() => CurrentData?.HelmOrder is not null && CurrentData.HelmOrder.Count > 0;

    public bool HasBossOrder() => CurrentData?.FinalBossOrder is not null && CurrentData.FinalBossOrder.Count > 0;

    public bool HasRegionBarrier() => CurrentData?.RegionBarrierInfo is not null && CurrentData.RegionBarrierInfo.Count > 0;

    public IDisposable BeginBatchUpdate()
    {
        return new BatchUpdateScope(this);
    }

    #endregion

    /// <summary>
    /// A private helper class that manages the batch update scope lifecycle for parsed spoiler data.
    /// When this is disposed, all of the changes fire in one <see cref="ParsedSpoilerDataChanged"/> event.
    /// </summary>
    private class BatchUpdateScope : IDisposable
    {
        private readonly ParsedSpoilerDataService _service;
        private bool _disposed;
        public BatchUpdateScope(ParsedSpoilerDataService service)
        {
            _service = service;
            _service._isBatchingUpdates = true;
            _service._deferredOldData = null;
            _service._deferredNewData = null;
        }
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            // Exit batch mode and process all deferred updates
            _service._isBatchingUpdates = false;
            if (_service._deferredOldData != null || _service._deferredNewData != null)
            {
                var oldData = _service._deferredOldData;
                var newData = _service._deferredNewData;
                // Clear deferred data before firing the event to avoid re-entrancy issues
                _service._deferredOldData = null;
                _service._deferredNewData = null;
                _service.ParsedSpoilerDataChanged?.Invoke(_service, new ParsedSpoilerDataChangedEventArgs(oldData, newData));
            }
        }
    }
}
