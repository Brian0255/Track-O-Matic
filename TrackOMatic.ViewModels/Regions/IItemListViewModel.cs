using TrackOMatic.Logic.Enums;

namespace TrackOMatic.ViewModels.Regions;

/// <summary>
/// A common interface for the item list view models (both region and vial).
/// </summary>
public interface IItemListViewModel
{
    /// <summary>
    /// Gets the name of the region associated with the items or vials.
    /// </summary>
    RegionName RegionName { get; }

    /// <summary>
    /// Attempts to accept a dropped item into the list.
    /// </summary>
    /// <param name="itemName">The item to drop.</param>
    /// <param name="dragType">The type of mouse drag operation.</param>
    /// <returns>True if the drop was accepted; otherwise, false.</returns>
    bool TryAcceptDrop(ItemName itemName, MouseDragType dragType);
}
