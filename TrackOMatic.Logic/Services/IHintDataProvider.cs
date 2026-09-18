using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Services;

/// <summary>
/// Abstraction for accessing hint data sources (shortcuts, regions, sorted collections).
/// Replaces direct static HintData access to enable dependency injection and testability.
/// </summary>
public interface IHintDataProvider
{
    /// <summary>
    /// Gets user-defined and default shortcut mappings.
    /// Structure: { "Hint Regions" -> { "dk" -> "DK", ... }, "Item Locations" -> {...}, "Moves" -> {...} }
    /// </summary>
    IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> UserShortcuts { get; }

    /// <summary>
    /// Gets region-specific direct hint items (items that can be hinted as direct items within a region).
    /// Excludes region names; includes misc hint types.
    /// </summary>
    IReadOnlyDictionary<RegionName, IReadOnlyList<string>> RegionsWithoutLevelName { get; }

    /// <summary>
    /// Gets all region short names in sorted order.
    /// </summary>
    IReadOnlyList<string> SortedRegions { get; }

    /// <summary>
    /// Gets all move short names in sorted order.
    /// </summary>
    IReadOnlyList<string> SortedMoves { get; }

    /// <summary>
    /// Gets all check location short names in sorted order.
    /// </summary>
    IReadOnlyList<string> SortedChecks { get; }
}
