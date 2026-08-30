using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;

namespace TrackOMatic.Services;

/// <summary>
/// Provides access to the currently loaded parsed spoiler data.
/// This service maintains state across tracker resets and load operations.
/// </summary>
public interface IParsedSpoilerDataService
{
    /// <summary>
    /// Event raised when the parsed spoiler data changes (loaded, reloaded, or cleared).
    /// </summary>
    event EventHandler<ParsedSpoilerDataChangedEventArgs>? ParsedSpoilerDataChanged;

    /// <summary>
    /// Gets the current parsed spoiler data, or null if no spoiler has been loaded.
    /// </summary>
    ParsedSpoilerData? CurrentData { get; }

    /// <summary>
    /// Updates the parsed spoiler data (e.g., when a new spoiler file is loaded).
    /// </summary>
    /// <param name="data">The new parsed spoiler data, or null to clear.</param>
    void UpdateParsedData(ParsedSpoilerData? data);

    /// <summary>
    /// Gets the point value for a specific item based on the current parsed spoiler data.
    /// </summary>
    /// <param name="itemName">The name of the item.</param>
    /// <returns>The point value for the specified item.</returns>
    int GetPointsForItem(ItemName itemName);

    /// <summary>
    /// Gets the point value for all items in a specific region based on the current parsed spoiler data.
    /// </summary>
    /// <param name="regionName">The name of the region.</param>
    /// <returns>The point value for all items in the specified region.</returns>
    int GetPointsForRegion(RegionName regionName);

    /// <summary>
    /// Gets the point spread for all items in the current parsed spoiler data, categorized by PointCategory.
    /// </summary>
    /// <returns>The point spread for all items, categorized by PointCategory.</returns>
    IDictionary<PointCategory, int> GetPointSpread();

    /// <summary>
    /// Gets the WOTH point value for all items in a specific region based on the current parsed spoiler data.
    /// </summary>
    /// <param name="regionName">The name of the region.</param>
    /// <returns>The WOTH point value for all items in the specified region.</returns>
    int GetWothPointsForRegion(RegionName regionName);

    /// <summary>
    /// Determines whether the current parsed spoiler data contains any item points.
    /// </summary>
    /// <returns></returns>
    bool HasItemPoints();

    /// <summary>
    /// Determines whether the current parsed spoiler data contains any WOTH points.
    /// </summary>
    /// <returns></returns>
    bool HasHoardPoints();

    /// <summary>
    /// Get the spoiler settings from the current parsed spoiler data.
    /// </summary>
    /// <remarks>If there is no current data, a new instance of <see cref="SpoilerSettings"/> will be returned.</remarks>
    /// <returns>The spoiler settings from the current parsed spoiler data.</returns>
    SpoilerSettings GetSpoilerSettings();

    /// <summary>
    /// Determines whether the current parsed spoiler data contains a helm order.
    /// </summary>
    /// <returns></returns>
    bool HasHelmOrder();

    /// <summary>
    /// Determines whether the current parsed spoiler data contains a boss order.
    /// </summary>
    /// <returns></returns>
    bool HasBossOrder();

    /// <summary>
    /// Determines whether the current parsed spoiler data contains any region barrier information.
    /// </summary>
    /// <returns></returns>
    bool HasRegionBarrier();

    /// <summary>
    /// Beigns a new scope where parsed data changes are batched.
    /// When the return scope is disposed, all collected changes will fire a single
    /// <see cref="ParsedSpoilerDataChanged"/> event.
    /// </summary>
    /// <returns></returns>
    IDisposable BeginBatchUpdate();
}

/// <summary>
/// Event args for when parsed spoiler data changes.
/// </summary>
public class ParsedSpoilerDataChangedEventArgs(ParsedSpoilerData? oldData, ParsedSpoilerData? newData) : EventArgs
{
    public ParsedSpoilerData? OldData { get; } = oldData;
    public ParsedSpoilerData? NewData { get; } = newData;
}
