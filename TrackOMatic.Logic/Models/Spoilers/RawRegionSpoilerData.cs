using System.Text.Json.Serialization;

namespace TrackOMatic.Logic.Models.Spoilers;

public class RawRegionSpoilerData
{
    [JsonPropertyName("level_name")]
    public string? LevelName { get; init; }

    [JsonPropertyName("level_order")]
    public int? LevelOrder { get; init; }

    [JsonPropertyName("vial_colors")]
    public List<string>? VialColors { get; init; }

    [JsonPropertyName("points")]
    public int Points { get; init; }

    [JsonPropertyName("woth_count")]
    public int WothCount { get; init; }
}
