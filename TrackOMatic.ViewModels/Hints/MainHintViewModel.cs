using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public class MainHintViewModel : ObservableObject, IDisposable
{
    private readonly IHintService _hintService;
    private readonly IShortcutService _shortcutService;
    private readonly IHintItemSelectionService _hintItemSelectionService;
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IUserSettingsService _userSettingsService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IDataSaverService _dataSaverService;

    #region HintPanelViewModels

    public HintPanelViewModel PathsHintPanelViewModel { get; init; }
    public HintPanelViewModel FoolishHintPanelViewModel { get; init; }
    public HintPanelViewModel KongsHintPanelViewModel { get; init; }
    public HintPanelViewModel HoardHintPanelViewModel { get; init; }
    public HintPanelViewModel PotionHintPanelViewModel { get; init; }
    public HintPanelViewModel UnhintedHintPanelViewModel { get; init; }
    public HintPanelViewModel IslesHintPanelViewModel { get; init; }
    public HintPanelViewModel JapesHintPanelViewModel { get; init; }
    public HintPanelViewModel AztecHintPanelViewModel { get; init; }
    public HintPanelViewModel FactoryHintPanelViewModel { get; init; }
    public HintPanelViewModel GalleonHintPanelViewModel { get; init; }
    public HintPanelViewModel ForestHintPanelViewModel { get; init; }
    public HintPanelViewModel CavesHintPanelViewModel { get; init; }
    public HintPanelViewModel CastleHintPanelViewModel { get; init; }
    public HintPanelViewModel HelmHintPanelViewModel { get; init; }

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
            if (IsDirectHintCompactMode)
            {
                return "DirectCompact";
            }
            if (IsDirectHintStandardMode)
            {
                return "DirectStandard";
            }
            if (IsMultipathCompactMode)
            {
                return "MultipathCompact";
            }
            if (IsMultipathStandardMode)
            {
                return "MultipathStandard";
            }
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
        IItemSelectorService itemSelectorService,
        IDataSaverService dataSaverService
    )
    {
        _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
        _shortcutService = shortcutService ?? throw new ArgumentNullException(nameof(shortcutService));
        _hintItemSelectionService = hintItemSelectionService ?? throw new ArgumentNullException(nameof(hintItemSelectionService));
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _dataSaverService = dataSaverService ?? throw new ArgumentNullException(nameof(dataSaverService));

        // Eagerly instantiate all hint panel view models
        PathsHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Paths, "#dba512");
        FoolishHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Foolish, "#e02f2f");
        KongsHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Kongs, "#ff59e1");
        HoardHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.WayOfTheHoard, "#2652ff");
        PotionHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.PotionCounts, "#d0d0d0");
        UnhintedHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Unhinted, "#36f77d");
        IslesHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Isles, "#fff67a");
        JapesHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Japes, "#59ff64");
        AztecHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Aztec, "#ffb01f");
        FactoryHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Factory, "#eeeeee");
        GalleonHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Galleon, "#216fff");
        ForestHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Forest, "#006b3e");
        CavesHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Caves, "#42ddf5");
        CastleHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Castle, "#99a3ad");
        HelmHintPanelViewModel = CreateHintPanelViewModel(HintPanelType.Helm, "#b580ff");

        BLockerPanelViewModel = new(
            "#f0f000",
            barrierService,
            parsedSpoilerDataService,
            _savedProgressProvider,
            itemSelectorService,
            dataSaverService
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

    private HintPanelViewModel CreateHintPanelViewModel(HintPanelType hintPanelType, string lineColor) => new(
        hintPanelType,
        lineColor,
        _hintService,
        _shortcutService,
        _hintItemSelectionService,
        _itemTrackingService,
        _savedProgressProvider,
        _dataSaverService
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
