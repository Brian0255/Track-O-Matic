using System.Collections.ObjectModel;
using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class PathHintItemListViewModel : ObservableObject
{
    private readonly IHintItemSelectionService _hintItemSelectionService;
    private readonly IHintService _hintService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly HintPanelType _hintPanelType;
    private readonly HintId _hintId;
    private readonly string _dialogTitle;

    #region Proxy Properties

    private ObservableCollection<PathHintItemViewModel> _pathItems;
    public ObservableCollection<PathHintItemViewModel> PathItems
    {
        get => _pathItems;
        set
        {
            if (SetProperty(ref _pathItems, value))
            {
                RefreshRow();
            }
        }
    }

    public bool IsHintInfoPath => _hintPanelType == HintPanelType.Paths;

    #endregion

    /// <summary>
    /// Fired when path items collection changes (items added/removed/cleared).
    /// Parent HintPanel listens to this to trigger re-sorting and filtering.
    /// </summary>
    public event EventHandler? PathItemsChanged;

    public PathHintItemListViewModel(
        HintPanelType hintPanelType,
        HintId hintId,
        string dialogTitle,
        IHintService hintService,
        IHintItemSelectionService hintItemSelectionService,
        ISavedProgressProvider savedProgressProvider
    )
    {
        _hintId = hintId;
        _hintPanelType = hintPanelType;
        _dialogTitle = dialogTitle ?? throw new ArgumentNullException(nameof(dialogTitle));
        _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
        _hintItemSelectionService = hintItemSelectionService ?? throw new ArgumentNullException(nameof(hintItemSelectionService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _pathItems = [];

        // TODO: SavedProgressProvider initializes the hinted/found items.
        RefreshRow();
    }

    private void RefreshRow()
    {
        if (PathItems.Count == 0)
        {
            // Add ItemName.NONE as a placeholder to allow the UI to be clickable.
            PathItems.Add(new PathHintItemViewModel(_hintId, ItemName.NONE, false, _hintService));
        }
    }

    /// <summary>
    /// Hook into PropertyChanged to notify when PathItems collection changes.
    /// </summary>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(PathItems))
        {
            PathItemsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [RelayCommand]
    private async Task SelectPathItems((double X, double Y) screenMousePosition)
    {
        var currentlyFound = PathItems.Select(h => h.ItemName).ToList(); // Separate list to be safe.

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
                var existing = PathItems.FirstOrDefault(i => i.ItemName == h.ItemName);
                return existing ?? new PathHintItemViewModel(_hintId, h.ItemName, false, _hintService);
            })
            .ToList();

        PathItems = new ObservableCollection<PathHintItemViewModel>(newHintedItems);
    }

    #region Object Overrides

    public override string ToString() => "Items on path: " + string.Join(", ", PathItems.Select(i => i.ItemName));

    #endregion
}
