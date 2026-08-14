using System.Text.Json.Serialization;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models.Spoilers;

/// <summary>
/// Represents the raw information about a B. Locker door from the spoiler log.
/// </summary>
public class BlockerInfoRaw
{
    /// <summary>
    /// Gets the raw item ID of the B. Locker door.
    /// </summary>
    /// <remarks>
    /// This is _not_ the same as the <see cref="BarrierItems"/> enum value.
    /// The fact that the value can convert over to it is purely coincidental.
    /// A separate mapping function will convert this to the fully parsed output.
    /// </remarks>
    [JsonPropertyName("item")]
    public int Item { get; init; }

    /// <summary>
    /// Gets the cost of the B. Locker door.
    /// </summary>
    [JsonPropertyName("cost")]
    public int Cost { get; init; }
}
