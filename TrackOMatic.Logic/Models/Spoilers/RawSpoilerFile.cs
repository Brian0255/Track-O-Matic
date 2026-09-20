using System.Text.Json.Serialization;

namespace TrackOMatic.Logic.Models.Spoilers;

public class RawSpoilerFile
{
    /// <summary>
    /// Gets the settings used to generate the seed from the spoiler file.
    /// </summary>
    /// <remarks>
    /// Only a few settings are parsed for this program.
    /// Other tracking programs may want to parse additional settings.
    /// </remarks>
    [JsonPropertyName("Settings")]
    public SimplifiedSettings? Settings { get; init; }
    /// <summary>
    /// Gets the spoiler hints data from the spoiler file.
    /// </summary>
    /// <remarks>The structure was cleaned up to use stronger types for consistency.</remarks>
    [JsonPropertyName("Spoiler Hints Data")]
    public SpoilerHintData? HintData { get; init; }

    /// <summary>
    /// Gets the item pool from the spoiler file.
    /// </summary>
    /// <remarks>Surprisingly, this does not list every single move that can be gotten.</remarks>
    [JsonPropertyName("Item Pool")]
    public List<string>? ItemPool { get; init; }

    /// <summary>
    /// Gets the randomizer version used to generate the spoiler file.
    /// </summary>
    [JsonPropertyName("Randomizer Version")]
    public string? RandomizerVersion { get; init; }
}
