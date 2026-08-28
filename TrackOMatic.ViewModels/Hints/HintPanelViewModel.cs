
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Events.Hints;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class HintPanelViewModel : ObservableObject, IEquatable<HintPanelViewModel>, IDisposable
{
    private readonly IHintService _hintService;
    private readonly IShortcutService _shortcutService;
    private readonly IHintItemSelectionService _hintItemSelectionService;
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IUserSettingsService _userSettingsService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly HintPanelType _hintPanelType;

    #region Proxy Properties

    private ObservableCollection<HintInfoViewModel> _hintInfoViewModels = [];
    public ObservableCollection<HintInfoViewModel> HintInfoViewModels
    {
        get => _hintInfoViewModels;
        private set => SetProperty(ref _hintInfoViewModels, value);
    }

    private string _filterResourceKey = "";
    public string FilterResourceKey
    {
        get => _filterResourceKey;
        private set => SetProperty(ref _filterResourceKey, value);
    }

    private string _sortResourceKey = "";
    public string SortResourceKey
    {
        get => _sortResourceKey;
        private set => SetProperty(ref _sortResourceKey, value);
    }

    private string _heading = "";
    public string Heading
    {
        get => _heading;
        set => SetProperty(ref _heading, value);
    }

    public bool IsPathHintPanel => _hintPanelType == HintPanelType.Paths;

    public int HintCount => HintInfoViewModels.Count;

    private ObservableCollection<ItemName> _itemsToFilterBy = [];
    public ObservableCollection<ItemName> ItemsToFilterBy
    {
        get => _itemsToFilterBy;
        private set => SetProperty(ref _itemsToFilterBy, value);
    }

    #endregion

    public HintPanelViewModel(
        HintPanelType hintPanelType,
        IHintService hintService,
        IShortcutService shortcutService,
        IHintItemSelectionService hintItemSelectionService,
        IItemTrackingService itemTrackingService,
        IUserSettingsService userSettingsService,
        ISavedProgressProvider savedProgressProvider
    )
    {
        _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
        _shortcutService = shortcutService ?? throw new ArgumentNullException(nameof(shortcutService));
        _hintItemSelectionService = hintItemSelectionService ?? throw new ArgumentNullException(nameof(hintItemSelectionService));
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _hintPanelType = hintPanelType;
        // Subscribe to progress changes to update hints when the game state changes.
        _savedProgressProvider.ProgressChanged += OnProgressChanged;

        _hintService.HintCreated += OnHintCreated;
        _hintService.HintRemoved += OnHintRemoved;

        // Initialize the hints for the current progress.
        LoadHintsForPanel();
        OnPropertyChanged(nameof(IsPathHintPanel));
    }

    #region Event Handling

    [RelayCommand]
    private void CreateHint()
    {
        _hintService.CreateHint(_hintPanelType);
    }

    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e)
    {
        LoadHintsForPanel();
    }

    private void LoadHintsForPanel()
    {
        HintInfoViewModels.Clear();
        var hintsForThisPanel = _savedProgressProvider.CurrentProgress.SavedHints.Where(h => h.HintPanelType == _hintPanelType);
        foreach (var hint in hintsForThisPanel)
        {
            HintInfoViewModel vm = new(
                hint.HintId,
                _hintService,
                _shortcutService,
                _hintItemSelectionService,
                _savedProgressProvider,
                _itemTrackingService,
                _userSettingsService
            );
            HintInfoViewModels.Add(vm);
        }
        UpdateFilterResourceKey();
        SortResourceKey = "sort";
        Heading = _hintPanelType.ToHeading();
    }

    private void OnHintCreated(object? sender, HintCreatedEventArgs e)
    {
        if (e.Hint.HintPanelType != _hintPanelType)
        {
            return;
        }

        var hint = _hintService.GetHint(e.HintId);
        if (hint is not null && hint.HintPanelType == _hintPanelType)
        {
            HintInfoViewModel vm = new(
                hint.HintId,
                _hintService,
                _shortcutService,
                _hintItemSelectionService,
                _savedProgressProvider,
                _itemTrackingService,
                _userSettingsService
            )
            {
                // Focus the Location TextBox immediately when a new hint is created
                ShouldFocusLocation = true
            };
            HintInfoViewModels.Add(vm);

            UpdateFilterResourceKey();
        }
    }

    private void OnHintRemoved(object? sender, HintRemovedEventArgs e)
    {
        var vm = HintInfoViewModels.FirstOrDefault(h => h.HintId == e.HintId);
        if (vm is not null)
        {
            HintInfoViewModels.Remove(vm);
            UpdateFilterResourceKey();
        }
    }

    private void UpdateFilterResourceKey()
    {
        FilterResourceKey = HintInfoViewModels.Count > 0 ? "filter_on" : "filter_empty";
    }

    #endregion

    #region Sort & Filter

    [RelayCommand]
    private void SortHints()
    {
        RefreshDisplayedHints();
    }

    [RelayCommand]
    private void ApplyFilter(List<ItemName> selectedItems)
    {
        ItemsToFilterBy = new ObservableCollection<ItemName>(selectedItems);
        RefreshDisplayedHints();
    }

    private void RefreshDisplayedHints()
    {
        var allHints = HintInfoViewModels.ToList();
        allHints.Sort((hint1, hint2) =>
            hint2.PathHintItemListViewModel.PathItems.Count -
            hint1.PathHintItemListViewModel.PathItems.Count);

        var (matchingHints, rejectedHints) = PartitionByFilter(allHints);
        var combined = matchingHints.Concat(rejectedHints).ToList();
        RebuildHintCollection(combined);
    }

    private (List<HintInfoViewModel>, List<HintInfoViewModel>) PartitionByFilter(List<HintInfoViewModel> hints)
    {
        var matching = new List<HintInfoViewModel>();
        var rejected = new List<HintInfoViewModel>();

        foreach (var hint in hints)
        {
            if (ItemsToFilterBy.Count == 0)
            {
                matching.Add(hint);
            }
            else
            {
                var pathItemNames = hint.PathHintItemListViewModel.PathItems
                    .Select(vm => vm.ItemName)
                    .ToHashSet();

                if (ItemsToFilterBy.All(filterItem => pathItemNames.Contains(filterItem)))
                {
                    matching.Add(hint);
                }
                else
                {
                    rejected.Add(hint);
                }
            }
        }

        return (matching, rejected);
    }

    private void RebuildHintCollection(List<HintInfoViewModel> orderedHints)
    {
        HintInfoViewModels.Clear();
        foreach (var hint in orderedHints)
        {
            HintInfoViewModels.Add(hint);
        }
    }

    #endregion

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
                // Unsubscribe from events
                _savedProgressProvider.ProgressChanged -= OnProgressChanged;
                _hintService.HintCreated -= OnHintCreated;
                _hintService.HintRemoved -= OnHintRemoved;
            }
            _disposed = true;
        }
    }

    ~HintPanelViewModel()
    {
        Dispose(false);
    }

    #endregion

    #region Object Overrides

    public override string ToString()
    {
        return $"HintPanelViewModel: Type={_hintPanelType}, HintsCount={HintInfoViewModels.Count}";
    }

    public override bool Equals(object? obj) => Equals(obj as HintPanelViewModel);

    public bool Equals(HintPanelViewModel? other) => other is not null && _hintPanelType == other._hintPanelType;

    public override int GetHashCode() => _hintPanelType.GetHashCode();

    #endregion
}
