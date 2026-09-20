using System.Text.Json.Serialization;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Converters;

namespace TrackOMatic.Logic.Models;

/// <summary>
/// Represents a saved item with its associated data, including the item name, region, visibility state, autotracked status, opacity, and hinted status.
/// </summary>
public record SavedItem
{
    /// <summary>
    /// The name of the item.
    /// </summary>
    public ItemName ItemName { get; init; }

    /// <summary>
    /// The region where the item is located.
    /// </summary>
    public RegionName Region { get; init; } = RegionName.UNKNOWN;

    /// <summary>
    /// Whether the item is visible (true=Visible, false=Hidden/Collapsed).
    /// Uses a custom converter to handle backward compatibility with old enum-based save files.
    /// </summary>
    [JsonConverter(typeof(StarredStateJsonConverter))]
    public bool Starred { get; init; } = false;

    /// <summary>
    /// Indicates whether the item is autotracked.
    /// </summary>
    public bool Autotracked { get; init; } = false;

    /// <summary>
    /// The opacity of the item.
    /// </summary>
    public double Opacity { get; init; } = 1.0;

    /// <summary>
    /// Indicates whether the item is hinted.
    /// </summary>
    public bool Hinted { get; init; } = false;

    /// <summary>
    /// Initializes a new instance of the SavedItem record.
    /// </summary>
    public SavedItem(
        ItemName itemName,
        RegionName region = RegionName.UNKNOWN,
        bool starred = false,
        bool autotracked = false,
        double opacity = 1.0,
        bool hinted = false
    )
    {
        ItemName = itemName;
        Region = region;
        Starred = starred;
        Autotracked = autotracked;
        Opacity = opacity;
        Hinted = hinted;
    }

    /// <summary>
    /// Initializes a new instance of the SavedItem record with no parameters.
    /// Used by the JSON deserializer.
    /// </summary>
    public SavedItem()
    {
    }

    /// <summary>
    /// A helper porperty to see if the item is empty (not starred, not in any region, and not hinted).
    /// </summary>
    [JsonIgnore]
    public bool IsEmpty => !Starred && Region == RegionName.UNKNOWN && !Hinted;

    /// <summary>
    /// A helper property to see if we have a starting item provided by a spoiler log.
    /// </summary>
    [JsonIgnore]
    public bool IsStartingAutotracked
    {
        get
        {
            if (Region != RegionName.START && Region != RegionName.UNHINTABLE_MOVES)
            {
                return false;
            }
            if (!Autotracked)
            {
                return false;
            }
            if (Hinted)
            {
                return false;
            }
            if (Opacity != 1)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Creates a default <see cref="SavedItem"/> for a given <see cref="ItemName"/>
    /// This is useful for initializing items that have not yet been saved or tracked.
    /// </summary>
    /// <param name="itemName">The name of the item to create a default instance for.</param>
    /// <returns>A new SavedItem with sensible defaults: hidden, not autotracked, full opacity, not in any region, not hinted.</returns>
    public static SavedItem CreateEmpty(ItemName itemName) => new(itemName);

    #region Object Overrides

    public override string ToString()
    {
        return $"{ItemName}{(Starred ? "*" : "")} in the {Region} Region, Autotracked: {Autotracked}, Opacity: {Opacity}, Hinted: {Hinted}";
    }

    #endregion
}
