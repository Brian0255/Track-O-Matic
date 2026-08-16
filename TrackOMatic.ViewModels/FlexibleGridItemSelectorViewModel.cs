using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Services;

namespace TrackOMatic.ViewModels;

/// <summary>
/// Represents a row of items in the flexible grid selector, grouped by row index.
/// </summary>
public class ItemGridRowGroup : ObservableObject
{
    private readonly int _rowIndex;
    private ObservableCollection<ItemGridCell> _items;

    public ItemGridRowGroup(int rowIndex, IEnumerable<ItemGridCell> items)
    {
        _rowIndex = rowIndex;
        _items = new ObservableCollection<ItemGridCell>(items);
    }

    public int RowIndex => _rowIndex;
    public ObservableCollection<ItemGridCell> Items => _items;
}

/// <summary>
/// ViewModel for the flexible grid item selector dialog.
/// Handles grouping of items by row and managing the selection state.
/// </summary>
public class FlexibleGridItemSelectorViewModel : ObservableObject
{
    private ItemGridCell? _selectedItem;
    private ObservableCollection<ItemGridRowGroup> _itemGroups;

    public FlexibleGridItemSelectorViewModel(IReadOnlyList<ItemGridCell> items)
    {
        if (items == null || items.Count == 0)
        {
            _itemGroups = [];
            return;
        }

        // Group items by row index
        var groupedByRow = items
            .GroupBy(item => item.Row)
            .OrderBy(group => group.Key)
            .Select(group => new ItemGridRowGroup(group.Key, group.OrderBy(item => item.Column)))
            .ToList();

        _itemGroups = new ObservableCollection<ItemGridRowGroup>(groupedByRow);
    }

    /// <summary>
    /// Gets the grouped collection of items organized by row.
    /// </summary>
    public ObservableCollection<ItemGridRowGroup> ItemGroups => _itemGroups;

    /// <summary>
    /// Gets or sets the currently selected item.
    /// </summary>
    public ItemGridCell? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }
}
