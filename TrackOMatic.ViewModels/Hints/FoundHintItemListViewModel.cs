using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class FoundHintItemListViewModel : ObservableObject
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IHintItemSelectionService _hintItemSelectionService;
    private readonly IHintService _hintService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly HintPanelType _hintPanelType;
    private readonly HintId _hintId;
    private readonly string _dialogTitle;

    #region Proxy Properties

    private ObservableCollection<FoundHintItemViewModel> _foundItems;
    public ObservableCollection<FoundHintItemViewModel> FoundItems
    {
        get => _foundItems;
        set 
        { 
            if (SetProperty(ref _foundItems, value))
            {
                RefreshRowSplits();
            }
        }
    }

    private ObservableCollection<FoundHintItemViewModel> _topRowItems = [];
    public ObservableCollection<FoundHintItemViewModel> TopRowItems
    {
        get => _topRowItems;
        private set => SetProperty(ref _topRowItems, value);
    }

    private ObservableCollection<FoundHintItemViewModel> _bottomRowItems = [];
    public ObservableCollection<FoundHintItemViewModel> BottomRowItems
    {
        get => _bottomRowItems;
        private set => SetProperty(ref _bottomRowItems, value);
    }

    private bool _shouldShowSecondRow;
    public bool ShouldShowSecondRow
    {
        get => _shouldShowSecondRow;
        private set => SetProperty(ref _shouldShowSecondRow, value);
    }

    public bool IsHintInfoNotFoolish => _hintPanelType != HintPanelType.Foolish;

    #endregion

    /// <summary>
    /// Splits FoundItems into top and bottom rows based on the old split logic.
    /// If count > 2, shows bottom row; otherwise hides it.
    /// Items are split at the midpoint: top row gets first half, bottom row gets second half.
    /// If no items exist, adds ItemName.NONE as a placeholder to allow UI interaction.
    /// </summary>
    private void RefreshRowSplits()
    {
        TopRowItems.Clear();
        BottomRowItems.Clear();

        if (FoundItems.Count == 0)
        {
            ShouldShowSecondRow = false;
            // Add ItemName.NONE as a placeholder to allow the UI to be clickable
            TopRowItems.Add(new FoundHintItemViewModel(_hintId, ItemName.NONE, false, _hintService));
            return;
        }

        // Use old split logic: if count > 2, enable second row
        ShouldShowSecondRow = FoundItems.Count > 2;
        int midpoint = (FoundItems.Count + 1) / 2;

        for (int i = 0; i < FoundItems.Count; i++)
        {
            if (i < midpoint)
            {
                TopRowItems.Add(FoundItems[i]);
            }
            else
            {
                BottomRowItems.Add(FoundItems[i]);
            }
        }
    }

    public FoundHintItemListViewModel(
        HintPanelType hintPanelType,
        HintId hintId,
        string dialogTitle,
        IItemTrackingService itemTrackingService,
        IHintItemSelectionService hintItemSelectionService,
        IHintService hintService,
        ISavedProgressProvider savedProgressProvider
    )
    {
        _hintId = hintId;
        _hintPanelType = hintPanelType;
        _dialogTitle = dialogTitle ?? throw new ArgumentNullException(nameof(dialogTitle));
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _hintItemSelectionService = hintItemSelectionService ?? throw new ArgumentNullException(nameof(hintItemSelectionService));
        _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _foundItems = [];

        // TODO: SavedProgressProvider initializes the hinted/found items.
        RefreshRowSplits();
    }

    [RelayCommand]
    private async Task SelectFoundItems((double X, double Y) screenMousePosition)
    {
        var currentlyFound = FoundItems.Select(h => h.ItemName).ToList(); // Separate list to be safe.

        var result = await _hintItemSelectionService.SelectItemsAsync(
            HintDialogMapping.HintDialogMappings,
            currentlyFound,
            _dialogTitle,
            screenMousePosition.X,
            screenMousePosition.Y
        );

        UpdateItemsFromSelection(result);
    }

    /// <summary>
    /// A clean place to update the hinted items from a selection.
    /// </summary>
    /// <remarks>
    /// This method will eventually populate the saved hints so the
    /// <see cref="ISavedProgressProvider"/> can update the display.
    /// </remarks>
    /// <param name="cells"></param>
    private void UpdateItemsFromSelection(IEnumerable<HintGridCell> cells)
    {
        var selectedSet = new HashSet<ItemName>(
            cells.Select(c => c.ItemName)
        );

        // Compare what we found with what we had. If the item is marked found, transfer it.
        var newHintedItems = HintDialogMapping.HintDialogMappings
            .Where(h => selectedSet.Contains(h.ItemName))
            .Select(h =>
            {
                var existing = FoundItems.FirstOrDefault(i => i.ItemName == h.ItemName);
                return existing ?? new FoundHintItemViewModel(_hintId, h.ItemName, false, _hintService);
            })
            .ToList();

        FoundItems = new ObservableCollection<FoundHintItemViewModel>(newHintedItems);
    }

    #region Object Overrides

    public override string ToString() => "Items found: " + string.Join(", ", FoundItems.Select(i => i.ItemName));

    #endregion
}
