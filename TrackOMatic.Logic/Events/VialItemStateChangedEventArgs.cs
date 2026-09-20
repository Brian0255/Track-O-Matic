using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events;

/// <summary>
/// Arguments for when a vial item state changes.
/// </summary>
/// <param name="regionName"></param>
/// <param name="vialColor"></param>
/// <param name="itemName"></param>
/// <param name="isStarred"></param>
public class VialItemStateChangedEventArgs(RegionName regionName, VialColor vialColor, ItemName? itemName, bool isStarred) : EventArgs
{
    public RegionName RegionName { get; init; } = regionName;

    public VialColor VialColor { get; init; } = vialColor;

    public ItemName? ItemName { get; init; } = itemName;

    public bool IsStarred { get; init; } = isStarred;
}
