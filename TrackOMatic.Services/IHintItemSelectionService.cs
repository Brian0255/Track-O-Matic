using System;
using System.Collections.Generic;
using System.Text;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.Services;

/// <summary>
/// A service for displaying a grid of items and allowing the user to select multiples at a time.
/// This interface is framework-agnostic to support WPF initially and Avalonia in the future.
/// </summary>
/// <remarks>Ensure that any implementations go in the executable projects.</remarks>
public interface IHintItemSelectionService
{
    /// <summary>
    /// Displays a dialog for selecting and deselecting multiple items for hint purposes.
    /// </summary>
    /// <remarks>At this time, it does not matter how the window is closed.</remarks>
    /// <param name="items">The collection of items to display.</param>
    /// <param name="itemsFoundAlready">The list of items that were already found (or claimed to be) previously.</param>
    /// <param name="title">The title of the selection dialog.</param>
    /// <param name="preferredX">The preferred X position of the dialog.</param>
    /// <param name="preferredY">The preferred Y position of the dialog.</param>
    /// <returns>The selected items after finishing with the dialog window.</returns>
    Task<IList<HintGridCell>> SelectItemsAsync(
        IReadOnlyList<HintGridCell> items,
        IList<ItemName> itemsFoundAlready,
        string title,
        double? preferredX = null,
        double? preferredY = null
    );
}
