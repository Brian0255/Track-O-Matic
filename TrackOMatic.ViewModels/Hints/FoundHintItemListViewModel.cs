using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Events.Hints;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class FoundHintItemListViewModel : ObservableObject, IDisposable
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

    [ObservableProperty]
    public partial ObservableCollection<FoundHintItemViewModel> TopRowItems { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<FoundHintItemViewModel> BottomRowItems { get; set; } = [];

    [ObservableProperty]
    public partial bool ShouldShowSecondRow { get; set; }

    [ObservableProperty]
    public partial int RowsToShow { get; set; } = 0;

    public bool IsHintInfoNotFoolish => _hintPanelType != HintPanelType.Foolish;

    [ObservableProperty]
    public partial bool ShouldShowFocusList { get; set; } = false;

    #endregion

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

        UpdateItemsFromService(hintId);

        _hintService.HintFoundItemsUpdated += OnFoundItemsUpdated;
        _savedProgressProvider.ProgressChanged += OnProgressChanged;

        RefreshRowSplits();

        ShouldShowFocusList = _hintPanelType switch
        {
            HintPanelType.Foolish => false,
            HintPanelType.PotionCounts => false,
            _ => true
        };
    }

    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e) => UpdateItemsFromService(_hintId);

    private void OnFoundItemsUpdated(object? sender, HintFoundItemsChangedEventArgs e)
    {
        if (_hintId != e.HintId || _hintPanelType != e.HintPanelType)
        {
            return;
        }

        UpdateItemsFromService(_hintId);
    }

    private void UpdateItemsFromService(HintId hintId)
    {
        var foundItems = (_hintService.GetHint(hintId) ?? new SavedHint()).FoundItems;
        var foundList = foundItems.Select(kvp => new FoundHintItemViewModel(hintId, kvp.Key, kvp.Value, _hintService)).ToList();
        FoundItems = new ObservableCollection<FoundHintItemViewModel>(foundList);
    }

    #region Commands

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

        var resultItems = result.Select(c => c.ItemName).ToDictionary(itemName => itemName, itemName => false);
        var targetRegion = _hintPanelType.ToRegion();

        if (targetRegion.IsPlayableRegion())
        {
            // We should add the item to the item tracking service as hinted via direct hint.
            foreach (var itemName in resultItems.Keys)
            {
                var itemState = _itemTrackingService.GetItemState(itemName) ?? SavedItem.CreateEmpty(itemName);
                if (itemState.Region == RegionName.UNKNOWN)
                {
                    _itemTrackingService.SetItemState(itemName, itemState with
                    {
                        Region = targetRegion,
                        Hinted = true
                    });
                }
            }
        }

        // Update the list with the old item checkmarks if they exist.
        foreach (var item in FoundItems)
        {
            if (resultItems.ContainsKey(item.ItemName))
            {
                resultItems[item.ItemName] = item.IsFound;
            }
        }

        _hintService.SetFoundItems(_hintId, resultItems, _hintPanelType);
    }

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

        if (_hintPanelType != HintPanelType.Paths)
        {
            ShouldShowSecondRow = false;
            RowsToShow = 1;
        }
        else
        {
            ShouldShowSecondRow = FoundItems.Count > 2;
            RowsToShow = 2;
        }
        if (FoundItems.Count == 0)
        {
            // Add ItemName.NONE as a placeholder to allow the UI to be clickable
            TopRowItems.Add(new FoundHintItemViewModel(_hintId, ItemName.NONE, false, _hintService));
            return;
        }

        int midpoint = (FoundItems.Count + 1) / 2;

        for (int i = 0; i < FoundItems.Count; i++)
        {
            if (i < midpoint || FoundItems.Count <= 2)
            {
                TopRowItems.Add(FoundItems[i]);
            }
            else
            {
                BottomRowItems.Add(FoundItems[i]);
            }
        }
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
        if (!_disposed)
        {
            if (disposing)
            {
                _hintService.HintFoundItemsUpdated -= OnFoundItemsUpdated;
                _savedProgressProvider.ProgressChanged -= OnProgressChanged;
            }
            _disposed = true;
        }
    }

    ~FoundHintItemListViewModel()
    {
        Dispose(false);
    }

    #endregion

    #region Object Overrides

    public override string ToString() => "Items found: " + string.Join(", ", FoundItems.Select(i => i.ItemName));

    #endregion
}
