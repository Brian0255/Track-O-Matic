using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public class MainHintViewModel : ObservableObject
{
    private readonly IHintService _hintService;
    private readonly IShortcutService _shortcutService;
    private readonly IHintItemSelectionService _hintItemSelectionService;
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IUserSettingsService _userSettingsService;
    private readonly ISavedProgressProvider _savedProgressProvider;

    #region Lazy-loaded HintPanelViewModels

    // Eventually have properties for each panel's view model here as well.

    private HintPanelViewModel? _pathsHintPanelViewModel;
    public HintPanelViewModel PathsHintPanelViewModel
    {
        get
        {
            _pathsHintPanelViewModel ??= CreateViewModel(HintPanelType.Paths, "#dba512");
            return _pathsHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _foolishHintPanelViewModel;
    public HintPanelViewModel FoolishHintPanelViewModel
    {
        get
        {
            _foolishHintPanelViewModel ??= CreateViewModel(HintPanelType.Foolish, "#e02f2f");
            return _foolishHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _kongsHintPanelViewModel;
    public HintPanelViewModel KongsHintPanelViewModel
    {
        get
        {
            _kongsHintPanelViewModel ??= CreateViewModel(HintPanelType.Kongs, "#ff59e1");
            return _kongsHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _hoardHintPanelViewModel;
    public HintPanelViewModel HoardHintPanelViewModel
    {
        get
        {
            _hoardHintPanelViewModel ??= CreateViewModel(HintPanelType.WayOfTheHoard, "#2652ff");
            return _hoardHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _potionHintPanelViewModel;
    public HintPanelViewModel PotionHintPanelViewModel
    {
        get
        {
            _potionHintPanelViewModel ??= CreateViewModel(HintPanelType.PotionCounts, "#d0d0d0");
            return _potionHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _unhintedHintPanelViewModel;
    public HintPanelViewModel UnhintedHintPanelViewModel
    {
        get
        {
            _unhintedHintPanelViewModel ??= CreateViewModel(HintPanelType.Unhinted, "#36f77d");
            return _unhintedHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _islesHintPanelViewModel;
    public HintPanelViewModel IslesHintPanelViewModel
    {
        get
        {
            _islesHintPanelViewModel ??= CreateViewModel(HintPanelType.Isles, "#fff67a");
            return _islesHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _japesHintPanelViewModel;
    public HintPanelViewModel JapesHintPanelViewModel
    {
        get
        {
            _japesHintPanelViewModel ??= CreateViewModel(HintPanelType.Japes, "#59ff64");
            return _japesHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _aztecHintPanelViewModel;
    public HintPanelViewModel AztecHintPanelViewModel
    {
        get
        {
            _aztecHintPanelViewModel ??= CreateViewModel(HintPanelType.Aztec, "#ffb01f");
            return _aztecHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _factoryHintPanelViewModel;
    public HintPanelViewModel FactoryHintPanelViewModel
    {
        get
        {
            _factoryHintPanelViewModel ??= CreateViewModel(HintPanelType.Factory, "#eeeeee");
            return _factoryHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _galleonHintPanelViewModel;
    public HintPanelViewModel GalleonHintPanelViewModel
    {
        get
        {
            _galleonHintPanelViewModel ??= CreateViewModel(HintPanelType.Galleon, "#216fff");
            return _galleonHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _forestHintPanelViewModel;
    public HintPanelViewModel ForestHintPanelViewModel
    {
        get
        {
            _forestHintPanelViewModel ??= CreateViewModel(HintPanelType.Forest, "#006b3e");
            return _forestHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _cavesHintPanelViewModel;
    public HintPanelViewModel CavesHintPanelViewModel
    {
        get
        {
            _cavesHintPanelViewModel ??= CreateViewModel(HintPanelType.Caves, "#42ddf5");
            return _cavesHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _castleHintPanelViewModel;
    public HintPanelViewModel CastleHintPanelViewModel
    {
        get
        {
            _castleHintPanelViewModel ??= CreateViewModel(HintPanelType.Castle, "#99a3ad");
            return _castleHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _helmHintPanelViewModel;
    public HintPanelViewModel HelmHintPanelViewModel
    {
        get
        {
            _helmHintPanelViewModel ??= CreateViewModel(HintPanelType.Helm, "#b580ff");
            return _helmHintPanelViewModel;
        }
    }

    #endregion

    public BLockerPanelViewModel BLockerPanelViewModel { get; init; }

    public HelmDoorPanelViewModel HelmDoorPanelViewModel { get; init; }

    #region Proxy Properties

    public bool DisplayNoHints => _userSettingsService.HintDisplay == HintDisplayMode.Off;

    public bool DisplayDirectHints => _userSettingsService.HintDisplay == HintDisplayMode.DirectItemHints;

    public bool DisplayPathHints => _userSettingsService.HintDisplay == HintDisplayMode.MultipathHints;

    public bool IsHelmDoorPanelEnabled => _userSettingsService.HelmDoors;

    public bool IsMultipathCompactMode => _userSettingsService.CompactMode && _userSettingsService.HintDisplay == HintDisplayMode.MultipathHints;

    public bool IsDirectHintCompactMode => _userSettingsService.CompactMode && _userSettingsService.HintDisplay == HintDisplayMode.DirectItemHints;

    public bool IsMultipathStandardMode => !_userSettingsService.CompactMode && _userSettingsService.HintDisplay == HintDisplayMode.MultipathHints;

    public bool IsDirectHintStandardMode => !_userSettingsService.CompactMode && _userSettingsService.HintDisplay == HintDisplayMode.DirectItemHints;

    /// <summary>
    /// Returns a unique key representing the current mode combination.
    /// This property changes whenever any mode property changes, forcing the ContentTemplateSelector to re-evaluate.
    /// </summary>
    public string CurrentModeKey
    {
        get
        {
            if (IsDirectHintCompactMode) return "DirectCompact";
            if (IsDirectHintStandardMode) return "DirectStandard";
            if (IsMultipathCompactMode) return "MultipathCompact";
            if (IsMultipathStandardMode) return "MultipathStandard";
            return "None";
        }
    }

    #endregion

    public MainHintViewModel(
        IHintService hintService,
        IShortcutService shortcutService,
        IHintItemSelectionService hintItemSelectionService,
        IItemTrackingService itemTrackingService,
        IUserSettingsService userSettingsService,
        ISavedProgressProvider savedProgressProvider,
        IBarrierService barrierService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IItemSelectorService itemSelectorService
    )
    {
        _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
        _shortcutService = shortcutService ?? throw new ArgumentNullException(nameof(shortcutService));
        _hintItemSelectionService = hintItemSelectionService ?? throw new ArgumentNullException(nameof(hintItemSelectionService));
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));

        BLockerPanelViewModel = new(
            "#f0f000",
            barrierService,
            parsedSpoilerDataService,
            _savedProgressProvider,
            itemSelectorService
        );
        HelmDoorPanelViewModel = new(
            "#7d3cbd",
            userSettingsService,
            barrierService,
            savedProgressProvider,
            itemSelectorService
        );

        _userSettingsService.PropertyChanged += OnSettingChanged;
    }

    private HintPanelViewModel CreateViewModel(HintPanelType hintPanelType, string lineColor) => new(
        hintPanelType,
        lineColor,
        _hintService,
        _shortcutService,
        _hintItemSelectionService,
        _itemTrackingService,
        _userSettingsService,
        _savedProgressProvider
    );

    private void OnSettingChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IUserSettingsService.HintDisplay))
        {
            OnPropertyChanged(nameof(DisplayNoHints));
            OnPropertyChanged(nameof(DisplayDirectHints));
            OnPropertyChanged(nameof(DisplayPathHints));

            // Do other things with hint display.
            OnPropertyChanged(nameof(IsDirectHintCompactMode));
            OnPropertyChanged(nameof(IsDirectHintStandardMode));
            OnPropertyChanged(nameof(IsMultipathCompactMode));
            OnPropertyChanged(nameof(IsMultipathStandardMode));
            OnPropertyChanged(nameof(CurrentModeKey));
        }
        else if (e.PropertyName == nameof(IUserSettingsService.CompactMode))
        {
            // Do something with compact mode.
            OnPropertyChanged(nameof(IsDirectHintCompactMode));
            OnPropertyChanged(nameof(IsDirectHintStandardMode));
            OnPropertyChanged(nameof(IsMultipathCompactMode));
            OnPropertyChanged(nameof(IsMultipathStandardMode));
            OnPropertyChanged(nameof(CurrentModeKey));
        }
        else if (e.PropertyName == nameof(IUserSettingsService.HelmDoors))
        {
            OnPropertyChanged(nameof(IsHelmDoorPanelEnabled));
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
                // Unsubscribe from events
                _userSettingsService.PropertyChanged -= OnSettingChanged;
            }
            _disposed = true;
        }
    }

    ~MainHintViewModel()
    {
        Dispose(false);
    }

    #endregion
}
