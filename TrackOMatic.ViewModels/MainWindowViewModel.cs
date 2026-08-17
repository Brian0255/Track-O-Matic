using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private readonly IUserSettingsService _userSettingsService;
    private readonly IThemeService _themeService;
    private readonly IAutotrackerService _autotrackerService;
    private readonly ISavedProgressProvider _savedProgressProvider;

    #region Child ViewModels

    private CollectiblesGridRowViewModel? _collectiblesGridRowViewModel;
    public CollectiblesGridRowViewModel CollectiblesGridRowViewModel
    {
        get
        {
            _collectiblesGridRowViewModel ??= new CollectiblesGridRowViewModel(
                _userSettingsService,
                _autotrackerService,
                _savedProgressProvider
            );
            return _collectiblesGridRowViewModel;
        }
    }

    #endregion

    #region Proxy Properties

    private bool _isTopMostEnabled;
    public bool IsTopMostEnabled
    {
        get => _isTopMostEnabled;
        set
        {
            if (SetProperty(ref _isTopMostEnabled, value))
            {
                _userSettingsService.TopMost = value;
            }
        }
    }

    private bool _isAutotrackingEnabled;
    public bool IsAutotrackingEnabled
    {
        get => _isAutotrackingEnabled;
        set
        {
            if (SetProperty(ref _isAutotrackingEnabled, value))
            {
                _userSettingsService.Autotracking = value;
            }
        }
    }

    private bool _isSongDisplayEnabled;
    public bool IsSongDisplayEnabled
    {
        get => _isSongDisplayEnabled;
        set
        {
            if (SetProperty(ref _isSongDisplayEnabled, value))
            {
                _userSettingsService.SongDisplay = value;
            }
        }
    }

    private bool _isHelmInLevelOrderEnabled;
    public bool IsHelmInLevelOrderEnabled
    {
        get => _isHelmInLevelOrderEnabled;
        set
        {
            if (SetProperty(ref _isHelmInLevelOrderEnabled, value))
            {
                _userSettingsService.HelmInLevelOrder = value;
            }
        }
    }

    private bool _isColoredBarrelPadMovesEnabled;
    public bool IsColoredBarrelPadMovesEnabled
    {
        get => _isColoredBarrelPadMovesEnabled;
        set
        {
            if (SetProperty(ref _isColoredBarrelPadMovesEnabled, value))
            {
                // The theme service handles updating the colored barrel/pad moves setting.
                _themeService.SetBarrelPadTheme(value);
            }
        }
    }

    private bool _isTotalBlueprintCollectibleEnabled;
    public bool IsTotalBlueprintCollectibleEnabled
    {
        get => _isTotalBlueprintCollectibleEnabled;
        set
        {
            if (SetProperty(ref _isTotalBlueprintCollectibleEnabled, value))
            {
                _userSettingsService.ShowTotalBPs = value;
            }
        }
    }

    private bool _isCompanyCoinCollectibleEnabled;
    public bool IsCompanyCoinCollectibleEnabled
    {
        get => _isCompanyCoinCollectibleEnabled;
        set
        {
            if (SetProperty(ref _isCompanyCoinCollectibleEnabled, value))
            {
                _userSettingsService.ShowCompanyCoins = value;
            }
        }
    }

    private bool _isShowingHelmSpoilerInfoEnabled;
    public bool IsShowingHelmSpoilerInfoEnabled
    {
        get => _isShowingHelmSpoilerInfoEnabled;
        set
        {
            if (SetProperty(ref _isShowingHelmSpoilerInfoEnabled, value))
            {
                _userSettingsService.ShowHelmOrder = value;
            }
        }
    }

    private bool _isShowingBossSpoilerInfoEnabled;
    public bool IsShowingBossSpoilerInfoEnabled
    {
        get => _isShowingBossSpoilerInfoEnabled;
        set
        {
            if (SetProperty(ref _isShowingBossSpoilerInfoEnabled, value))
            {
                _userSettingsService.ShowKRoolOrder = value;
            }
        }
    }

    private bool _isProgressiveHintCounterEnabled;
    public bool IsProgressiveHintCounterEnabled
    {
        get => _isProgressiveHintCounterEnabled;
        set
        {
            if (SetProperty(ref _isProgressiveHintCounterEnabled, value))
            {
                _userSettingsService.ShowAmountForHints = value;
            }
        }
    }

    private HintDisplayMode _hintDisplayMode;
    public HintDisplayMode HintDisplayMode
    {
        get => _hintDisplayMode;
        set
        {
            bool wasVisible = _hintDisplayMode != HintDisplayMode.Off;
            bool isVisible = value != HintDisplayMode.Off;

            if (SetProperty(ref _hintDisplayMode, value))
            {
                _userSettingsService.HintDisplay = value;
                if (wasVisible != isVisible)
                {
                    OnPropertyChanged(nameof(CurrentWindowWidth));
                }
            }
        }
    }

    private bool _isHelmDoorPanelEnabled;
    public bool IsHelmDoorPanelEnabled
    {
        get => _isHelmDoorPanelEnabled;
        set
        {
            if (SetProperty(ref _isHelmDoorPanelEnabled, value))
            {
                _userSettingsService.HelmDoors = value;
            }
        }
    }

    private bool _isCompactModeEnabled;
    public bool IsCompactModeEnabled
    {
        get => _isCompactModeEnabled;
        set
        {
            if (SetProperty(ref _isCompactModeEnabled, value))
            {
                _userSettingsService.CompactMode = value;
            }
        }
    }

    private bool _isSortablePathHintsEnabled = false;
    public bool IsSortablePathHintsEnabled
    {
        get => _isSortablePathHintsEnabled;
        set
        {
            if (SetProperty(ref _isSortablePathHintsEnabled, value))
            {
                _userSettingsService.AutoSortPathHints = value;
            }
        }
    }

    private bool _isEnemySearchEnabled = false;
    public bool IsEnemySearchEnabled
    {
        get => _isEnemySearchEnabled;
        set
        {
            if (SetProperty(ref _isEnemySearchEnabled, value))
            {
                _userSettingsService.EnemiesInAutofill = value;
            }
        }
    }

    private bool _isBroadcastViewEnabled = false;
    public bool IsBroadcastViewEnabled
    {
        get => _isBroadcastViewEnabled;
        set
        {
            if (SetProperty(ref _isBroadcastViewEnabled, value))
            {
                _userSettingsService.BroadcastViewActive = value;
            }
        }
    }

    private bool _isBroadcastHelmKRoolEnabled = false;
    public bool IsBroadcastHelmKRoolEnabled
    {
        get => _isBroadcastHelmKRoolEnabled;
        set
        {
            if (SetProperty(ref _isBroadcastHelmKRoolEnabled, value))
            {
                _userSettingsService.BroadcastHelmKRool = value;
            }
        }
    }

    private bool _isBroadcastShopkeepersEnabled = false;
    public bool IsBroadcastShopkeepersEnabled
    {
        get => _isBroadcastShopkeepersEnabled;
        set
        {
            if (SetProperty(ref _isBroadcastShopkeepersEnabled, value))
            {
                _userSettingsService.BroadcastShopkeepers = value;
            }
        }
    }

    private bool _isBroadcastSongDisplayEnabled = false;
    public bool IsBroadcastSongDisplayEnabled
    {
        get => _isBroadcastSongDisplayEnabled;
        set
        {
            if (SetProperty(ref _isBroadcastSongDisplayEnabled, value))
            {
                _userSettingsService.BroadcastSongDisplay = value;
            }
        }
    }

    private BroadcastNumberLabel _broadcastNumberLabel;
    public BroadcastNumberLabel BroadcastNumberLabel
    {
        get => _broadcastNumberLabel;
        set
        {
            if (SetProperty(ref _broadcastNumberLabel, value))
            {
                _userSettingsService.BroadcastNumberLabel = value;
            }
        }
    }

    /// <summary>
    /// Gets the width of the hint column based on the current hint display mode and compact mode setting.
    /// </summary>
    public double HintColumnWidth
    {
        get => _userSettingsService.HintDisplay == HintDisplayMode.Off ? 0 : _userSettingsService.CompactMode ? 1.43 : 2.15;
    }

    public double CurrentWindowWidth
    {
        get
        {
            if (HintDisplayMode == HintDisplayMode.Off)
            {
                return 580;
            }

            return IsCompactModeEnabled ? 1392 : 1800;
        }
    }

    #endregion

    public MainWindowViewModel(
        IUserSettingsService userSettingsService,
        IThemeService themeService,
        IAutotrackerService autotrackerService,
        ISavedProgressProvider savedProgressProvider
    )
    {
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _autotrackerService = autotrackerService ?? throw new ArgumentNullException(nameof(autotrackerService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        InitializeState();
        _userSettingsService.PropertyChanged += OnSettingChanged;
    }

    private void InitializeState()
    {
        // load the settings so the menus are set up correctly.
        IsTopMostEnabled = _userSettingsService.TopMost;
        IsAutotrackingEnabled = _userSettingsService.Autotracking;
        IsSongDisplayEnabled = _userSettingsService.SongDisplay;
        IsHelmInLevelOrderEnabled = _userSettingsService.HelmInLevelOrder;
        IsColoredBarrelPadMovesEnabled = _userSettingsService.ColoredBarrelPadMoves;
        IsTotalBlueprintCollectibleEnabled = _userSettingsService.ShowTotalBPs;
        IsCompanyCoinCollectibleEnabled = _userSettingsService.ShowCompanyCoins;
        IsShowingHelmSpoilerInfoEnabled = _userSettingsService.ShowHelmOrder;
        IsShowingBossSpoilerInfoEnabled = _userSettingsService.ShowKRoolOrder;
        IsProgressiveHintCounterEnabled = _userSettingsService.ShowAmountForHints;
        HintDisplayMode = _userSettingsService.HintDisplay;
        IsHelmDoorPanelEnabled = _userSettingsService.HelmDoors;
        IsCompactModeEnabled = _userSettingsService.CompactMode;
        IsSortablePathHintsEnabled = _userSettingsService.AutoSortPathHints;
        IsEnemySearchEnabled = _userSettingsService.EnemiesInAutofill;
        IsBroadcastViewEnabled = _userSettingsService.BroadcastViewActive;
        IsBroadcastHelmKRoolEnabled = _userSettingsService.BroadcastHelmKRool;
        IsBroadcastShopkeepersEnabled = _userSettingsService.BroadcastShopkeepers;
        IsBroadcastSongDisplayEnabled = _userSettingsService.BroadcastSongDisplay;
        BroadcastNumberLabel = _userSettingsService.BroadcastNumberLabel;
    }

    #region Commands

    [RelayCommand]
    private void ToggleTopMost()
    {
        IsTopMostEnabled = !IsTopMostEnabled;
    }

    [RelayCommand]
    private void ToggleAutotracking()
    {
        IsAutotrackingEnabled = !IsAutotrackingEnabled;
    }

    [RelayCommand]
    private void ToggleSongDisplay()
    {
        IsSongDisplayEnabled = !IsSongDisplayEnabled;
    }

    [RelayCommand]
    private void ToggleHelmInLevelOrder()
    {
        IsHelmInLevelOrderEnabled = !IsHelmInLevelOrderEnabled;
    }

    [RelayCommand]
    private void ToggleColoredBarrelPadMoves()
    {
        IsColoredBarrelPadMovesEnabled = !IsColoredBarrelPadMovesEnabled;
    }

    [RelayCommand]
    private void ToggleTotalBlueprintCollectible()
    {
        IsTotalBlueprintCollectibleEnabled = !IsTotalBlueprintCollectibleEnabled;
    }

    [RelayCommand]
    private void ToggleCompanyCoinCollectible()
    {
        IsCompanyCoinCollectibleEnabled = !IsCompanyCoinCollectibleEnabled;
    }

    [RelayCommand]
    private void ToggleHelmSpoilerOrder()
    {
        IsShowingHelmSpoilerInfoEnabled = !IsShowingHelmSpoilerInfoEnabled;
    }

    [RelayCommand]
    private void ToggleBossSpoilerOrder()
    {
        IsShowingBossSpoilerInfoEnabled = !IsShowingBossSpoilerInfoEnabled;
    }

    [RelayCommand]
    private void ToggleProgressiveHintCounter()
    {
        IsProgressiveHintCounterEnabled = !IsProgressiveHintCounterEnabled;
    }

    [RelayCommand]
    private void SetHintDisplay(HintDisplayMode mode)
    {
        HintDisplayMode = mode;
    }

    [RelayCommand]
    private void ToggleSortablePathHints()
    {
        IsSortablePathHintsEnabled = !IsSortablePathHintsEnabled;
    }

    [RelayCommand]
    private void ToggleHelmDoorPanel()
    {
        IsHelmDoorPanelEnabled = !IsHelmDoorPanelEnabled;
    }

    [RelayCommand]
    private void ToggleCompactMode()
    {
        IsCompactModeEnabled = !IsCompactModeEnabled;
    }

    [RelayCommand]
    private void ToggleEnemySearch()
    {
        IsEnemySearchEnabled = !IsEnemySearchEnabled;
    }

    [RelayCommand]
    private void ToggleBroadcastView()
    {
        IsBroadcastViewEnabled = !IsBroadcastViewEnabled;
    }

    [RelayCommand]
    private void ToggleBroadcastHelmKRool()
    {
        IsBroadcastHelmKRoolEnabled = !IsBroadcastHelmKRoolEnabled;
    }

    [RelayCommand]
    private void ToggleBroadcastShopkeepers()
    {
        IsBroadcastShopkeepersEnabled = !IsBroadcastShopkeepersEnabled;
    }

    [RelayCommand]
    private void ToggleBroadcastSongDisplay()
    {
        IsBroadcastSongDisplayEnabled = !IsBroadcastSongDisplayEnabled;
    }

    [RelayCommand]
    private void SetBroadcastNumberLabel(BroadcastNumberLabel label)
    {
        BroadcastNumberLabel = label;
    }

    #endregion

    private void OnSettingChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_userSettingsService.TopMost))
        {
            IsTopMostEnabled = _userSettingsService.TopMost;
        }
        else if (e.PropertyName == nameof(_userSettingsService.Autotracking))
        {
            IsAutotrackingEnabled = _userSettingsService.Autotracking;
        }
        else if (e.PropertyName == nameof(_userSettingsService.SongDisplay))
        {
            IsSongDisplayEnabled = _userSettingsService.SongDisplay;
        }
        else if (e.PropertyName == nameof(_userSettingsService.HelmInLevelOrder))
        {
            IsHelmInLevelOrderEnabled = _userSettingsService.HelmInLevelOrder;
        }
        else if (e.PropertyName == nameof(_userSettingsService.ColoredBarrelPadMoves))
        {
            IsColoredBarrelPadMovesEnabled = _userSettingsService.ColoredBarrelPadMoves;
        }
        else if (e.PropertyName == nameof(_userSettingsService.ShowTotalBPs))
        {
            IsTotalBlueprintCollectibleEnabled = _userSettingsService.ShowTotalBPs;
        }
        else if (e.PropertyName == nameof(_userSettingsService.ShowCompanyCoins))
        {
            IsCompanyCoinCollectibleEnabled = _userSettingsService.ShowCompanyCoins;
        }
        else if (e.PropertyName == nameof(_userSettingsService.ShowHelmOrder))
        {
            IsShowingHelmSpoilerInfoEnabled = _userSettingsService.ShowHelmOrder;
        }
        else if (e.PropertyName == nameof(_userSettingsService.ShowKRoolOrder))
        {
            IsShowingBossSpoilerInfoEnabled = _userSettingsService.ShowKRoolOrder;
        }
        else if (e.PropertyName == nameof(_userSettingsService.ShowAmountForHints))
        {
            IsProgressiveHintCounterEnabled = _userSettingsService.ShowAmountForHints;
        }
        else if (e.PropertyName == nameof(_userSettingsService.HintDisplay))
        {
            HintDisplayMode = _userSettingsService.HintDisplay;
            OnPropertyChanged(nameof(HintColumnWidth));
        }
        else if (e.PropertyName == nameof(_userSettingsService.HelmDoors))
        {
            IsHelmDoorPanelEnabled = _userSettingsService.HelmDoors;
        }
        else if (e.PropertyName == nameof(_userSettingsService.CompactMode))
        {
            IsCompactModeEnabled = _userSettingsService.CompactMode;
            OnPropertyChanged(nameof(HintColumnWidth));
            OnPropertyChanged(nameof(CurrentWindowWidth));
        }
        else if (e.PropertyName == nameof(_userSettingsService.AutoSortPathHints))
        {
            IsSortablePathHintsEnabled = _userSettingsService.AutoSortPathHints;
        }
        else if (e.PropertyName == nameof(_userSettingsService.EnemiesInAutofill))
        {
            IsEnemySearchEnabled = _userSettingsService.EnemiesInAutofill;
        }
        else if (e.PropertyName == nameof(_userSettingsService.BroadcastViewActive))
        {
            IsBroadcastViewEnabled = _userSettingsService.BroadcastViewActive;
        }
        else if (e.PropertyName == nameof(_userSettingsService.BroadcastHelmKRool))
        {
            IsBroadcastHelmKRoolEnabled = _userSettingsService.BroadcastHelmKRool;
        }
        else if (e.PropertyName == nameof(_userSettingsService.BroadcastShopkeepers))
        {
            IsBroadcastShopkeepersEnabled = _userSettingsService.BroadcastShopkeepers;
        }
        else if (e.PropertyName == nameof(_userSettingsService.BroadcastSongDisplay))
        {
            IsBroadcastSongDisplayEnabled = _userSettingsService.BroadcastSongDisplay;
        }
        else if (e.PropertyName == nameof(_userSettingsService.BroadcastNumberLabel))
        {
            BroadcastNumberLabel = _userSettingsService.BroadcastNumberLabel;
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
                _userSettingsService.PropertyChanged -= OnSettingChanged;
                (_collectiblesGridRowViewModel as IDisposable)?.Dispose();
            }
            _disposed = true;
        }
    }

    ~MainWindowViewModel()
    {
        Dispose(false);
    }

    #endregion
}
