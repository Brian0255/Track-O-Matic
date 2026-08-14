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

    public event EventHandler<ParsedSpoilerDataChangedEventArgs>? ParsedSpoilerDataChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedSpoilerDataService"/> class.
    /// </summary>
    /// <param name="progressProvider">The provider for saved progress state changes.</param>
    public ParsedSpoilerDataService(ISavedProgressProvider progressProvider)
    {
        _progressProvider = progressProvider ?? throw new ArgumentNullException(nameof(progressProvider));
        _currentData = null;
        // Set up listener for SavedProgress changes (e.g., reset or load)
        _progressProvider.ProgressChanged += OnProgressChanged;
    }

    /// <summary>
    /// Handles progress state changes by clearing the parsed spoiler data.
    /// This ensures that when tracker state is reset or loaded, the parsed data
    /// reflects the new state (empty until a new spoiler is loaded).
    /// </summary>
    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e)
    {
        // Clear the parsed data when progress changes (reset or load from file)
        UpdateParsedData(null);
    }

    #region IParsedSpoilerDataService Implementation

    public ParsedSpoilerData? CurrentData => _currentData;

    public void UpdateParsedData(ParsedSpoilerData? data)
    {
        var oldData = _currentData;
        _currentData = data;

        // Notify subscribers that the data has changed
        ParsedSpoilerDataChanged?.Invoke(this, new ParsedSpoilerDataChangedEventArgs(oldData, data));
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

    #endregion
}
