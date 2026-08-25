using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

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
            _pathsHintPanelViewModel ??= CreateViewModel(HintPanelType.Paths);
            return _pathsHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _foolishHintPanelViewModel;
    public HintPanelViewModel FoolishHintPanelViewModel
    {
        get
        {
            _foolishHintPanelViewModel ??= CreateViewModel(HintPanelType.Foolish);
            return _foolishHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _kongsHintPanelViewModel;
    public HintPanelViewModel KongsHintPanelViewModel
    {
        get
        {
            _kongsHintPanelViewModel ??= CreateViewModel(HintPanelType.Kongs);
            return _kongsHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _hoardHintPanelViewModel;
    public HintPanelViewModel HoardHintPanelViewModel
    {
        get
        {
            _hoardHintPanelViewModel ??= CreateViewModel(HintPanelType.WayOfTheHoard);
            return _hoardHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _potionHintPanelViewModel;
    public HintPanelViewModel PotionHintPanelViewModel
    {
        get
        {
            _potionHintPanelViewModel ??= CreateViewModel(HintPanelType.PotionCounts);
            return _potionHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _unhintedHintPanelViewModel;
    public HintPanelViewModel UnhintedHintPanelViewModel
    {
        get
        {
            _unhintedHintPanelViewModel ??= CreateViewModel(HintPanelType.Unhinted);
            return _unhintedHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _islesHintPanelViewModel;
    public HintPanelViewModel IslesHintPanelViewModel
    {
        get
        {
            _islesHintPanelViewModel ??= CreateViewModel(HintPanelType.Isles);
            return _islesHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _japesHintPanelViewModel;
    public HintPanelViewModel JapesHintPanelViewModel
    {
        get
        {
            _japesHintPanelViewModel ??= CreateViewModel(HintPanelType.Japes);
            return _japesHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _aztecHintPanelViewModel;
    public HintPanelViewModel AztecHintPanelViewModel
    {
        get
        {
            _aztecHintPanelViewModel ??= CreateViewModel(HintPanelType.Aztec);
            return _aztecHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _factoryHintPanelViewModel;
    public HintPanelViewModel FactoryHintPanelViewModel
    {
        get
        {
            _factoryHintPanelViewModel ??= CreateViewModel(HintPanelType.Factory);
            return _factoryHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _galleonHintPanelViewModel;
    public HintPanelViewModel GalleonHintPanelViewModel
    {
        get
        {
            _galleonHintPanelViewModel ??= CreateViewModel(HintPanelType.Galleon);
            return _galleonHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _forestHintPanelViewModel;
    public HintPanelViewModel ForestHintPanelViewModel
    {
        get
        {
            _forestHintPanelViewModel ??= CreateViewModel(HintPanelType.Forest);
            return _forestHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _cavesHintPanelViewModel;
    public HintPanelViewModel CavesHintPanelViewModel
    {
        get
        {
            _cavesHintPanelViewModel ??= CreateViewModel(HintPanelType.Caves);
            return _cavesHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _castleHintPanelViewModel;
    public HintPanelViewModel CastleHintPanelViewModel
    {
        get
        {
            _castleHintPanelViewModel ??= CreateViewModel(HintPanelType.Castle);
            return _castleHintPanelViewModel;
        }
    }

    private HintPanelViewModel? _helmHintPanelViewModel;
    public HintPanelViewModel HelmHintPanelViewModel
    {
        get
        {
            _helmHintPanelViewModel ??= CreateViewModel(HintPanelType.Helm);
            return _helmHintPanelViewModel;
        }
    }

    #endregion

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
        ISavedProgressProvider savedProgressProvider
    )
    {
        _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
        _shortcutService = shortcutService ?? throw new ArgumentNullException(nameof(shortcutService));
        _hintItemSelectionService = hintItemSelectionService ?? throw new ArgumentNullException(nameof(hintItemSelectionService));
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));

        _userSettingsService.PropertyChanged += OnSettingChanged;
    }

    private HintPanelViewModel CreateViewModel(HintPanelType hintPanelType) => new(
        hintPanelType,
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
