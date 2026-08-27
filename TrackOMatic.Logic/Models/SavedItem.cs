using System.Text.Json.Serialization;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models;
/// <summary>
/// Represents a saved item with its associated data, including the item name, region, visibility state, autotracked status, opacity, and hinted status.
/// </summary>
/// <param name="ItemName">The name of the item.</param>
/// <param name="Region">The region where the item is located.</param>
/// <param name="Starred">The visibility state of the item.</param>
/// <param name="Autotracked">Indicates whether the item is autotracked.</param>
/// <param name="Opacity">The opacity of the item.</param>
/// <param name="Hinted">Indicates whether the item is hinted.</param>
public record SavedItem(
    ItemName ItemName,
    RegionName Region = RegionName.UNKNOWN,
    ItemVisibilityState Starred = ItemVisibilityState.Hidden,
    bool Autotracked = false,
    double Opacity = 1.0,
    bool Hinted = false
)
{
    [JsonIgnore]
    public bool IsEmpty => Starred == ItemVisibilityState.Hidden && Region == RegionName.UNKNOWN && !Hinted;

    /// <summary>
    /// Creates a default <see cref="SavedItem"/> for a given <see cref="ItemName"/>
    /// This is useful for initializing items that have not yet been saved or tracked.
    /// </summary>
    /// <param name="itemName">The name of the item to create a default instance for.</param>
    /// <returns>A new SavedItem with sensible defaults: hidden, not autotracked, full opacity, not in any region, not hinted.</returns>
    public static SavedItem CreateEmpty(ItemName itemName)
        => new(itemName);

    #region Object Overrides

    public override string ToString()
    {
        return $"{ItemName}{(Starred == ItemVisibilityState.Visible ? "*" : "")} in the {Region} Region, Autotracked: {Autotracked}, Opacity: {Opacity}, Hinted: {Hinted}";
    }

    #endregion
}
