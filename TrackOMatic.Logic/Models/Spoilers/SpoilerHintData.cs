using System.Text.Json.Serialization;
using TrackOMatic.Logic.Models.Spoilers.JsonConverters;

namespace TrackOMatic.Logic.Models.Spoilers;

/// <summary>
/// Represents the "Spoiler Hints Data" section of a spoiler file.
/// </summary>
/// <remarks>
/// This class uses a custom JSON converter to transform numeric string keys ("0"-"8")
/// into a strongly-typed dictionary indexed by region number.
///
/// Region Index Mapping:
/// - 0: Jungle Japes
/// - 1: Angry Aztec
/// - 2: Frantic Factory
/// - 3: Gloomy Galleon
/// - 4: Fungi Forest
/// - 5: Crystal Caves
/// - 6: Castle Crush
/// - 7: Helm's Deep
/// - 8: DK Isles
/// </remarks>
[JsonConverter(typeof(SpoilerHintDataJsonConverter))]
public class SpoilerHintData
{
    /// <summary>
    /// Gets a dictionary of region spoiler data indexed by region number (0-8).
    /// </summary>
    public Dictionary<int, RawRegionSpoilerData>? RegionDataDictionary { get; init; }

    /// <summary>
    /// Gets the starting info for the game, including starting items and other relevant information.
    /// </summary>
    [JsonPropertyName("starting_info")]
    public StartingInfoRaw? StartingInfo { get; init; }

    /// <summary>
    /// Gets the point spread for the game, which indicates how many points each item is worth.
    /// </summary>
    /// <remarks>The string should be converted to an enum for better typing.</remarks>
    [JsonPropertyName("point_spread")]
    public Dictionary<string, int>? PointSpread { get; init; }
}
