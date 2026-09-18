using TrackOMatic.Logic.Services;
using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.Services;

/// <summary>
/// Production implementation of IHintDataLoader.
///
/// Wraps the static HintData.Init() logic and returns prepared data in a HintDataLoadResult.
/// This class exists to isolate initialization concerns from the provider, making both testable
/// and the data flow explicit in dependency injection.
/// </summary>
public class HintDataLoader : IHintDataLoader
{
    /// <summary>
    /// Load hint data by calling HintData.Init() and collecting the results.
    /// </summary>
    public HintDataLoadResult LoadHintData()
    {
        // Initialize static HintData (loads resources, parses JSON, builds structures)
        HintData.Init();

        // Collect the initialized data into a result object
        return new HintDataLoadResult
        {
            UserShortcuts = HintData.UserShortcuts.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyDictionary<string, string>)kvp.Value
            ),
            RegionsWithoutLevelName = HintData.REGIONS_WITHOUT_LEVEL_NAME.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyList<string>)kvp.Value
            ),
            SortedRegions = HintData.SortedRegions.AsReadOnly(),
            SortedMoves = HintData.SortedMoves.AsReadOnly(),
            SortedChecks = HintData.SortedChecks.AsReadOnly()
        };
    }
}
