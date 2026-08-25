using System.Collections.ObjectModel;
using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Events.Hints;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class PathHintItemListViewModel : ObservableObject, IDisposable
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
        _hintService.HintPathItemsUpdated += OnPathItemsUpdated;
        _savedProgressProvider.ProgressChanged += OnProgressChanged;

        RefreshRow();
    }

    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e) => UpdateItemsFromService(_hintId);

    private void OnPathItemsUpdated(object? sender, HintPathItemsChangedEventArgs e)
    {
        if (_hintId != e.HintId)
        {
            return;
        }

        UpdateItemsFromService(_hintId);
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

        var resultItems = result.Select(c => c.ItemName).ToDictionary(i => i, i => true);
        foreach (var item in PathItems)
        {
            if (resultItems.ContainsKey(item.ItemName))
            {
                resultItems[item.ItemName] = item.IsFound;
            }
        }

        _hintService.SetPathItems(_hintId, resultItems);
    }

    private void UpdateItemsFromService(HintId hintId)
    {
        var pathItems = (_hintService.GetHint(hintId) ?? new SavedHint()).PathItems;
        var pathList = pathItems.Select(kvp => new PathHintItemViewModel(hintId, kvp.Key, kvp.Value, _hintService)).ToList();
        PathItems = new ObservableCollection<PathHintItemViewModel>(pathList);
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
                _hintService.HintPathItemsUpdated -= OnPathItemsUpdated;
                _savedProgressProvider.ProgressChanged -= OnProgressChanged;
            }
            _disposed = true;
        }
    }

    ~PathHintItemListViewModel()
    {
        Dispose(false);
    }

    #endregion

    #region Object Overrides

    public override string ToString() => "Items on path: " + string.Join(", ", PathItems.Select(i => i.ItemName));

    #endregion
}
