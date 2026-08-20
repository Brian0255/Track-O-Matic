using System.Collections.ObjectModel;
using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.ViewModels.Hints;

public partial class MultiGridItemSelectorViewModel : ObservableObject, IDisposable
{
    private IList<HintDialogItemViewModel> _selectedItems;

    public ObservableCollection<HintDialogItemViewModel> AllItems { get; private init; }

    public event EventHandler? CloseRequested;

    private PropertyChangedEventHandler? _itemPropertyChangedHandler;

    public MultiGridItemSelectorViewModel(IReadOnlyList<HintDialogItemViewModel> items)
    {
        if (items == null || items.Count == 0)
        {
            AllItems = [];
            _selectedItems = [];
            return;
        }

        AllItems = new ObservableCollection<HintDialogItemViewModel>(items);
        _selectedItems = [];

        _itemPropertyChangedHandler = (s, e) =>
        {
            if (e.PropertyName == nameof(HintDialogItemViewModel.IsSelected))
            {
                if (s is HintDialogItemViewModel item)
                {
                    UpdatedSelectedItem(item);
                }
            }
        };

        foreach (var item in items)
        {
            item.PropertyChanged += _itemPropertyChangedHandler;
        }
    }

    public IList<HintDialogItemViewModel> SelectedItems
    {
        get => _selectedItems;
        set => SetProperty(ref _selectedItems, value);
    }

    private void UpdatedSelectedItem(HintDialogItemViewModel item)
    {
        if (item.IsSelected && !_selectedItems.Contains(item))
        {
            _selectedItems.Add(item);
        }
        else if (!item.IsSelected && _selectedItems.Contains(item))
        {
            _selectedItems.Remove(item);
        }
    }

    /// <summary>
    /// Command to close the dialog and finalize the selection.
    /// </summary>
    [RelayCommand]
    public void ConfirmSelection()
    {
        // Collect all selected items
        var selected = new List<HintDialogItemViewModel>();
        foreach (var item in AllItems)
        {
            selected.Add(item);
        }
        _selectedItems = selected;

        // Signal the dialog to close
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Gets the selected ItemGridCells from the current selection state.
    /// This method should be called when the dialog closes, regardless of how it closed.
    /// </summary>
    public IList<HintGridCell> GetSelectedCells()
    {
        var result = new List<HintGridCell>();
        foreach (var item in AllItems)
        {
            if (item.IsSelected)
            {
                result.Add(item.Cell);
            }
        }
        return result;
    }

    #region IDisposable

    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!_disposed)
            {
                if (_itemPropertyChangedHandler is not null)
                {
                    foreach (var item in AllItems)
                    {
                        item.PropertyChanged -= _itemPropertyChangedHandler;
                    }
                }
            }
            _disposed = true;
        }
    }

    ~MultiGridItemSelectorViewModel()
    {
        Dispose(false);
    }

    #endregion
}
