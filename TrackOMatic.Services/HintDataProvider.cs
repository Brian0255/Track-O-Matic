using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Services;

namespace TrackOMatic.Services;

/// <summary>
/// Implementation of IHintDataProvider that acts as a simple data container.
///
/// Accepts pre-built hint data via constructor (typically from IHintDataLoader).
/// This design makes HintDataProvider testable without initialization logic,
/// while keeping the actual loading/initialization logic in HintDataLoader.
/// </summary>
public class HintDataProvider : IHintDataProvider
{
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> UserShortcuts { get; }

    public IReadOnlyDictionary<RegionName, IReadOnlyList<string>> RegionsWithoutLevelName { get; }

    public IReadOnlyList<string> SortedRegions { get; }

    public IReadOnlyList<string> SortedMoves { get; }

    public IReadOnlyList<string> SortedChecks { get; }

    /// <summary>
    /// Initializes a new instance of HintDataProvider with pre-built data.
    /// </summary>
    /// <param name="userShortcuts">User-defined shortcuts by category</param>
    /// <param name="regionsWithoutLevelName">Items grouped by region (without level name)</param>
    /// <param name="sortedRegions">Sorted region list</param>
    /// <param name="sortedMoves">Sorted move list</param>
    /// <param name="sortedChecks">Sorted check list</param>
    public HintDataProvider(
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> userShortcuts,
        IReadOnlyDictionary<RegionName, IReadOnlyList<string>> regionsWithoutLevelName,
        IReadOnlyList<string> sortedRegions,
        IReadOnlyList<string> sortedMoves,
        IReadOnlyList<string> sortedChecks)
    {
        UserShortcuts = userShortcuts;
        RegionsWithoutLevelName = regionsWithoutLevelName;
        SortedRegions = sortedRegions;
        SortedMoves = sortedMoves;
        SortedChecks = sortedChecks;
    }
}
