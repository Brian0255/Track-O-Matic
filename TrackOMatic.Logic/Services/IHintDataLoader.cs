using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Services;

/// <summary>
/// Abstraction for loading hint data from persistent sources (files, resources, etc.).
///
/// This decouples the provider from loading/initialization logic, allowing:
/// - Production: Load from embedded resources and files via HintData.Init()
/// - Tests: Inject pre-built data directly to the provider
/// </summary>
public interface IHintDataLoader
{
    /// <summary>
    /// Load and initialize all hint data from sources.
    /// </summary>
    /// <returns>A result containing all 5 prepared hint data collections.</returns>
    HintDataLoadResult LoadHintData();
}

/// <summary>
/// Container for all hint data collections returned by a loader.
/// </summary>
public class HintDataLoadResult
{
    /// <summary>
    /// User-defined shortcuts, organized by category.
    /// Key: category name (e.g., "Regions", "Moves")
    /// Value: dictionary of shortcut key to expanded value
    /// </summary>
    public required IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> UserShortcuts { get; set; }

    /// <summary>
    /// Items grouped by playable region (without level name in the check name).
    /// </summary>
    public required IReadOnlyDictionary<RegionName, IReadOnlyList<string>> RegionsWithoutLevelName { get; set; }

    /// <summary>
    /// Sorted list of all region names.
    /// </summary>
    public required IReadOnlyList<string> SortedRegions { get; set; }

    /// <summary>
    /// Sorted list of all move names.
    /// </summary>
    public required IReadOnlyList<string> SortedMoves { get; set; }

    /// <summary>
    /// Sorted list of all check names.
    /// </summary>
    public required IReadOnlyList<string> SortedChecks { get; set; }
}
