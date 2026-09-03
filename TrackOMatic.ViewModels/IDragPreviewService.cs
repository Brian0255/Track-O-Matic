namespace TrackOMatic.ViewModels;

/// <summary>
/// An itnerface for clearing the drag preview when a drop is completed or canceled.
/// </summary>
/// <remarks>This interface must be implemented in an executable project.</remarks>
public interface IDragPreviewService
{
    /// <summary>
    /// Registers a drag preview for the given item.
    /// </summary>
    /// <remarks>The drag preview will be displayed in the UI until ClearDragPreview is called.</remarks>
    /// <param name="item"></param>
    void RegisterDragPreview(UiItemViewModel item);

    /// <summary>
    /// Updates the drag preview in the UI.
    /// </summary>
    /// <remarks>This method should be called when the item being dragged changes its state or appearance.</remarks>
    void UpdateDragPreview();

    /// <summary>
    /// Clears the drag preview from the UI.
    /// </summary>
    void ClearDragPreview();
}
