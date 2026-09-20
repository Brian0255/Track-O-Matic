using System.Windows;
using TrackOMatic.Services;
using TrackOMatic.ViewModels;

namespace TrackOMatic;

/// <summary>
/// Dialog window for selecting an item from a flexible grid layout.
/// Displays items grouped by row/column and returns the selected item or its index.
/// </summary>
public partial class FlexibleGridItemSelector : Window
{
    private readonly FlexibleGridItemSelectorViewModel _viewModel;
    private readonly IReadOnlyList<ItemGridCell> _originalItems;

    /// <summary>
    /// Initializes a new instance of the FlexibleGridItemSelector dialog.
    /// </summary>
    /// <param name="items">The collection of items to display in the grid.</param>
    /// <param name="title">The title for the dialog window.</param>
    public FlexibleGridItemSelector(IReadOnlyList<ItemGridCell> items, string title)
    {
        if (items == null || items.Count == 0)
        {
            throw new ArgumentException("Items collection cannot be null or empty.", nameof(items));
        }

        _originalItems = items;
        _viewModel = new FlexibleGridItemSelectorViewModel(items);

        InitializeComponent();

        DataContext = _viewModel;
        Title = title;
        SelectedItem = null;
    }

    /// <summary>
    /// Gets the item selected by the user, or null if cancelled.
    /// </summary>
    public ItemGridCell? SelectedItem { get; private set; }

    /// <summary>
    /// Gets the zero-based index of the selected item within the original items collection.
    /// Returns -1 if no selection was made.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            if (SelectedItem == null)
            {
                return -1;
            }

            return _originalItems.ToList().IndexOf(SelectedItem);
        }
    }

    private void ItemButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element || element.Tag is not ItemGridCell item)
        {
            return;
        }

        SelectedItem = item;
        Close();
    }
}
