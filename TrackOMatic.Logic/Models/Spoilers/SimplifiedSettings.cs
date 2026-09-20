using System.Text.Json.Serialization;

namespace TrackOMatic.Logic.Models.Spoilers;

public class SimplifiedSettings
{
    /// <summary>
    /// Gets the Shockwave Shuffle setting from the spoiler file.
    /// </summary>
    [JsonPropertyName("Shockwave Shuffle")]
    public string? ShockwaveShuffle { get; init; }
}
