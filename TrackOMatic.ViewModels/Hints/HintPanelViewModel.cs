using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Events.Hints;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class HintPanelViewModel : ObservableObject, IDisposable
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

    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e) => LoadHintsForPanel();

    private void LoadHintsForPanel()
    {
        HintInfoViewModels.Clear();
        var hintsForThisPanel = _savedProgressProvider.CurrentProgress.SavedHints.Where(h => h.HintPanelType == _hintPanelType);
        foreach (var hint in hintsForThisPanel)
        {
            HintInfoViewModel vm = new(
                _hintPanelType.ToHintType(),
                _hintPanelType,
                _hintPanelType.ToRegion(),
                hint,
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
        var hint = _hintService.GetHint(e.HintId);
        if (hint is not null && hint.HintPanelType == _hintPanelType)
        {
            HintInfoViewModel vm = new(
                _hintPanelType.ToHintType(),
                _hintPanelType,
                _hintPanelType.ToRegion(),
                hint,
                _shortcutService,
                _hintItemSelectionService,
                _savedProgressProvider,
                _itemTrackingService,
                _userSettingsService
            );
            HintInfoViewModels.Add(vm);
            UpdateFilterResourceKey();
        }
    }

    private void OnHintRemoved(object? sender, HintRemovedEventArgs e)
    {
        var vm = HintInfoViewModels.FirstOrDefault(h => h.SavedHint.HintId == e.HintId);
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

    public override bool Equals(object? obj)
    {
        return obj is HintPanelViewModel other && _hintPanelType == other._hintPanelType;
    }

    public override int GetHashCode() => _hintPanelType.GetHashCode();

    #endregion
}
