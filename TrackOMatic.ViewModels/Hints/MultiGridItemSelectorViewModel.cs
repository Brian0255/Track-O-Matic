using System.Collections.ObjectModel;
using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.ViewModels.Hints;

public partial class MultiGridItemSelectorViewModel : ObservableObject, IDisposable
{
    private IList<HintDialogItemViewModel> _selectedItems;

    public ObservableCollection<HintDialogItemViewModel> AllItems { get; private init; }

    public ObservableCollection<HintDialogItemViewModel> FinalBossPhases { get; private init; } = [];
    public ObservableCollection<HintDialogItemViewModel> KongMoves {  get; private init; } = [];
    public ObservableCollection<HintDialogItemViewModel> Keys { get; private init; } = [];
    public ObservableCollection<HintDialogItemViewModel> Shopkeepers { get; private init; } = [];
    public ObservableCollection<HintDialogItemViewModel> TrainingMoves { get; private init; } = [];
    public ObservableCollection<HintDialogItemViewModel> SharedMoves { get; private init; } = [];
    public ObservableCollection<HintDialogItemViewModel> Potions { get; private init; } = [];
    public ObservableCollection<HintDialogItemViewModel> LevelBosses { get; private init; } = [];
    public ObservableCollection<HintDialogItemViewModel> MiscItems { get; private init; } = [];

    public event EventHandler? CloseRequested;

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
        PartitionItemsByRegion();

        foreach (var item in items)
        {
            item.PropertyChanged += OnHintItemSelected;
        }
    }

    private void PartitionItemsByRegion()
    {
        foreach (var item in AllItems)
        {
            var name = item.Cell.ItemName;
            switch (name)
            {
                case ItemName.DONKEY_KROOL:
                case ItemName.DIDDY_KROOL:
                case ItemName.LANKY_KROOL:
                case ItemName.TINY_KROOL:
                case ItemName.CHUNKY_KROOL:
                    FinalBossPhases.Add(item);
                    break;
                case ItemName.ARMY:
                case ItemName.DOGA:
                case ItemName.MADJACK:
                case ItemName.PUFFTOSS:
                case ItemName.DOGA2:
                case ItemName.ARMY2:
                case ItemName.KUTOUT:
                    LevelBosses.Add(item);
                    break;
                case ItemName.KEY_1:
                case ItemName.KEY_2:
                case ItemName.KEY_3:
                case ItemName.KEY_4:
                case ItemName.KEY_5:
                case ItemName.KEY_6:
                case ItemName.KEY_7:
                case ItemName.KEY_8:
                    Keys.Add(item);
                    break;
                case ItemName.UNKNOWN_VIAL:
                case ItemName.UNKNOWN_KEY:
                case ItemName.NINTENDO_COIN:
                case ItemName.RAREWARE_COIN:
                case ItemName.HINT:
                    MiscItems.Add(item);
                    break;
                case ItemName.PROGRESSIVE_SLAM_1:
                case ItemName.PROGRESSIVE_SLAM_2:
                case ItemName.PROGRESSIVE_SLAM_3:
                case ItemName.BEAN:
                case ItemName.SNIPER_SCOPE:
                case ItemName.HOMING_AMMO:
                case ItemName.FAIRY_CAMERA:
                case ItemName.SHOCKWAVE:
                    SharedMoves.Add(item);
                    break;
                case ItemName.VINE_SWINGING:
                case ItemName.BARREL_THROWING:
                case ItemName.DIVING:
                case ItemName.ORANGE_THROWING:
                case ItemName.CLIMBING:
                    TrainingMoves.Add(item);
                    break;
                case ItemName.KONG_FAMILY:
                case ItemName.POTION_SHARED:
                case ItemName.POTION_DK:
                case ItemName.POTION_DIDDY:
                case ItemName.POTION_LANKY:
                case ItemName.POTION_TINY:
                case ItemName.POTION_CHUNKY:
                    Potions.Add(item);
                    break;
                case ItemName.CRANKY:
                case ItemName.CANDY:
                case ItemName.FUNKY:
                case ItemName.SNIDE:
                    Shopkeepers.Add(item);
                    break;
                default:
                    KongMoves.Add(item);
                    break;
            }
        }
    }

    private void OnHintItemSelected(object? s, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HintDialogItemViewModel.IsSelected))
        {
            if (s is HintDialogItemViewModel item)
            {
                UpdatedSelectedItem(item);
            }
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
    private void ConfirmSelection()
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
                foreach (var item in AllItems)
                {
                    item.PropertyChanged -= OnHintItemSelected;
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
