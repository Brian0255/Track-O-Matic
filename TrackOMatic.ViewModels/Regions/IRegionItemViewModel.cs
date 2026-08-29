using System.ComponentModel;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.ViewModels.Regions;

/// <summary>
/// A contract interface indicating what each specialized view model is allowed to do.
/// </summary>
public interface IRegionItemViewModel: INotifyPropertyChanged, IDisposable
{
    /// <summary>
    /// Gets the name of the item currently present in the region, or null if no item is present.
    /// </summary>
    ItemName? CurrentItemName { get; }

    /// <summary>
    /// Gets the resource key for the image to display for the item in the region.
    /// </summary>
    string ImageResourceKey { get; }

    /// <summary>
    /// Gets a value indicating whether the item is starred (i.e., important) in the region.
    /// </summary>
    bool IsStarred { get; }

    /// <summary>
    /// Gets the item tracking service used to manage the state of items in the region.
    /// </summary>
    // IItemTrackingService ItemTrackingService { get; }

    /// <summary>
    /// Determines whether the region can accept a drop of the specified item and drag type.
    /// </summary>
    /// <param name="itemToPlace">The item dragged into the region.</param>
    /// <param name="dragType">The type of drag operation being performed.</param>
    /// <returns>True if the region can accept the drop; otherwise, false.</returns>
    bool CanAcceptDrop(ItemName itemToPlace, MouseDragType dragType);

    /// <summary>
    /// Attempts to accept the dropped item and mutate the region state accordingly.
    /// </summary>
    /// <param name="itemToPlace">The item dragged into the region.</param>
    /// <param name="dragType">The type of drag operation being performed.</param>
    /// <returns>True if the drop was accepted and the region state was mutated; otherwise, false.</returns>
    bool AcceptDropAndMutate(ItemName itemToPlace, MouseDragType dragType);

    /// <summary>
    /// Removes the item from the region, if it is currently present.
    /// </summary>
    void RemoveFromRegion();

    /// <summary>
    /// Gets the hoard points associated with the item in the region, if any.
    /// </summary>
    /// <returns>The hoard points.</returns>
    int GetHoardPoints();

    /// <summary>
    /// Gets the point value associated with the item in the region, if any.
    /// </summary>
    /// <returns>The point value.</returns>
    int GetItemPoints();

    /// <summary>
    /// Toggles the starred state of the item in the region.
    /// </summary>
    void ToggleStar();
}
