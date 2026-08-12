namespace TrackOMatic.Services;

/// <summary>
/// Service for displaying a flexible grid item selector dialog and returning the user's selection.
/// This interface is framework-agnostic to support WPF initially and Avalonia in the future.
/// </summary>
public interface IItemSelectorService
{
    /// <summary>
    /// Displays a flexible grid item selector dialog and returns the selected item asynchronously.
    /// </summary>
    /// <param name="items">The collection of items to display in the grid.</param>
    /// <param name="title">The title for the dialog window.</param>
    /// <returns>
    /// A task that completes with the selected <see cref="IItemGridCell"/> if the user made a selection,
    /// or null if the user cancelled the dialog.
    /// </returns>
    Task<ItemGridCell?> SelectItemAsync(IReadOnlyList<ItemGridCell> items, string title);
}
