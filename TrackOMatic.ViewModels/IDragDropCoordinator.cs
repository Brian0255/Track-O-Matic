using TrackOMatic.Logic.Enums;
using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.ViewModels;

/// <summary>
/// Defines a contract for coordinating drag-and-drop operations between items and regions in the application.
/// </summary>
/// <remarks>This interface must be implemented in the executable project.</remarks>
public interface IDragDropCoordinator
{
    /// <summary>
    /// Validates and executes a drop operation for a given item onto a target region.
    /// </summary>
    /// <remarks>The drag type determines how the item will be rendered.</remarks>
    /// <param name="itemName">The name of the item being dragged.</param>
    /// <param name="viewModel">The view model of the target region where the item is being dropped.</param>
    /// <param name="dragType">The type of drag operation being performed.</param>
    /// <returns>True if the drop operation was successful; otherwise, false.</returns>
    bool TryExecuteDrop(ItemName itemName, RegionViewModel viewModel, MouseDragType dragType);
}
