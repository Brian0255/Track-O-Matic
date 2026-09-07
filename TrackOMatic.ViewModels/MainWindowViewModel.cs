using System.Collections.ObjectModel;
using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic;
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
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;

    #region Child ViewModels

    public CollectiblesGridRowViewModel CollectiblesGridRowViewModel { get; private init; }

    public ObservableCollection<UiItemViewModel> KongItems { get; private init; } = [];

    public ObservableCollection<UiKongBlueprintViewModel> KongBlueprints { get; private init; } = [];

    public ObservableCollection<UiItemViewModel> Keys { get; private init; } = [];

    public ObservableCollection<UiItemViewModel> Shopkeepers { get; private init; } = [];

    public ObservableCollection<UiItemViewModel> TrainingMoves { get; private init; } = [];

    public ObservableCollection<UiItemViewModel> MiscMoves { get; private init; } = [];

    #endregion

    #region Proxy Properties

    [ObservableProperty]
    public partial bool IsTopMostEnabled { get; set; }

    partial void OnIsTopMostEnabledChanged(bool value)
    {
        _userSettingsService.TopMost = value;
    }

    [ObservableProperty]
    public partial bool IsAutotrackingEnabled { get; set; }

    partial void OnIsAutotrackingEnabledChanged(bool value)
    {
        _userSettingsService.Autotracking = value;
    }

    [ObservableProperty]
    public partial bool IsSongDisplayEnabled { get; set; }

    partial void OnIsSongDisplayEnabledChanged(bool value)
    {
        _userSettingsService.SongDisplay = value;
    }

    [ObservableProperty]
    public partial bool IsHelmInLevelOrderEnabled { get; set; }

    partial void OnIsHelmInLevelOrderEnabledChanged(bool value)
    {
        _userSettingsService.HelmInLevelOrder = value;
    }

    [ObservableProperty]
    public partial bool IsColoredBarrelPadMovesEnabled { get; set; }

    partial void OnIsColoredBarrelPadMovesEnabledChanged(bool value)
    {
        _themeService.SetBarrelPadTheme(value);
    }

    [ObservableProperty]
    public partial bool IsTotalBlueprintCollectibleEnabled { get; set; }

    partial void OnIsTotalBlueprintCollectibleEnabledChanged(bool value)
    {
        _userSettingsService.ShowTotalBPs = value;
    }

    [ObservableProperty]
    public partial bool IsCompanyCoinCollectibleEnabled { get; set; }

    partial void OnIsCompanyCoinCollectibleEnabledChanged(bool value)
    {
        _userSettingsService.ShowCompanyCoins = value;
    }

    [ObservableProperty]
    public partial bool IsShowingHelmSpoilerInfoEnabled { get; set; }

    partial void OnIsShowingHelmSpoilerInfoEnabledChanged(bool value)
    {
        _userSettingsService.ShowHelmOrder = value;
    }

    [ObservableProperty]
    public partial bool IsShowingBossSpoilerInfoEnabled { get; set; }

    partial void OnIsShowingBossSpoilerInfoEnabledChanged(bool value)
    {
        _userSettingsService.ShowKRoolOrder = value;
    }

    [ObservableProperty]
    public partial bool IsProgressiveHintCounterEnabled { get; set; }

    partial void OnIsProgressiveHintCounterEnabledChanged(bool value)
    {
        _userSettingsService.ShowAmountForHints = value;
    }

    [ObservableProperty]
    public partial HintDisplayMode HintDisplayMode { get; set; }

    partial void OnHintDisplayModeChanged(HintDisplayMode oldValue, HintDisplayMode newValue)
    {
        bool wasVisible = oldValue != HintDisplayMode.Off;
        bool isVisible = newValue != HintDisplayMode.Off;
        _userSettingsService.HintDisplay = newValue;
        if (wasVisible != isVisible)
        {
            OnPropertyChanged(nameof(CurrentWindowWidth));
        }
    }

    [ObservableProperty]
    public partial bool IsHelmDoorPanelEnabled { get; set; }

    partial void OnIsHelmDoorPanelEnabledChanged(bool value)
    {
        _userSettingsService.HelmDoors = value;
    }

    [ObservableProperty]
    public partial bool IsCompactModeEnabled { get; set; }

    partial void OnIsCompactModeEnabledChanged(bool value)
    {
        _userSettingsService.CompactMode = value;
        OnPropertyChanged(nameof(HintColumnWidth));
        OnPropertyChanged(nameof(CurrentWindowWidth));
    }

    [ObservableProperty]
    public partial bool IsSortablePathHintsEnabled { get; set; } = false;

    partial void OnIsSortablePathHintsEnabledChanged(bool value)
    {
        _userSettingsService.AutoSortPathHints = value;
    }

    [ObservableProperty]
    public partial bool IsEnemySearchEnabled { get; set; } = false;

    partial void OnIsEnemySearchEnabledChanged(bool value)
    {
        _userSettingsService.EnemiesInAutofill = value;
    }

    [ObservableProperty]
    public partial bool IsBroadcastViewEnabled { get; set; } = false;

    partial void OnIsBroadcastViewEnabledChanged(bool value)
    {
        _userSettingsService.BroadcastViewActive = value;
    }

    [ObservableProperty]
    public partial bool IsBroadcastHelmKRoolEnabled { get; set; } = false;

    partial void OnIsBroadcastHelmKRoolEnabledChanged(bool value)
    {
        _userSettingsService.BroadcastHelmKRool = value;
    }

    [ObservableProperty]
    public partial bool IsBroadcastShopkeepersEnabled { get; set; } = false;

    partial void OnIsBroadcastShopkeepersEnabledChanged(bool value)
    {
        _userSettingsService.BroadcastShopkeepers = value;
    }

    [ObservableProperty]
    public partial bool IsBroadcastSongDisplayEnabled { get; set; } = false;

    partial void OnIsBroadcastSongDisplayEnabledChanged(bool value)
    {
        _userSettingsService.BroadcastSongDisplay = value;
    }

    [ObservableProperty]
    public partial BroadcastNumberLabel BroadcastNumberLabel { get; set; }

    partial void OnBroadcastNumberLabelChanged(BroadcastNumberLabel value)
    {
        _userSettingsService.BroadcastNumberLabel = value;
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
        IItemTrackingService itemTrackingService,
        IUserSettingsService userSettingsService,
        ICollectiblesService collectiblesService,
        IThemeService themeService,
        IAutotrackerService autotrackerService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider
    )
    {
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _autotrackerService = autotrackerService ?? throw new ArgumentNullException(nameof(autotrackerService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));

        CollectiblesGridRowViewModel = new CollectiblesGridRowViewModel(
            _userSettingsService,
            _autotrackerService,
            _savedProgressProvider
        );

        InitializeState();

        foreach (var itemName in Extensions.KongItemNames)
        {
            KongItems.Add(new(
                itemName,
                _itemTrackingService,
                _parsedSpoilerDataService,
                _savedProgressProvider,
                _themeService
            ));
        }

        var startingItem = (int)ItemType.DONKEY_BLUEPRINT;
        for (int i = startingItem; i < startingItem + 10; i += 2)
        {
            KongBlueprints.Add(new(
                (ItemType)i,
                (ItemType)(i + 1),
                collectiblesService,
                _parsedSpoilerDataService,
                _savedProgressProvider,
                _userSettingsService
            ));
        }

        var startingKey = (int)ItemName.KEY_1;
        for (int i = startingKey; i < startingKey + 8; ++i)
        {
            Keys.Add(new(
                (ItemName)i,
                _itemTrackingService,
                _parsedSpoilerDataService,
                _savedProgressProvider,
                _themeService
            ));
        }

        var startingShopkeep = (int)ItemName.CRANKY;
        for (int i = startingShopkeep; i < startingShopkeep + 4; ++i)
        {
            Shopkeepers.Add(new(
                (ItemName)i,
                _itemTrackingService,
                _parsedSpoilerDataService,
                _savedProgressProvider,
                _themeService
            ));
        }

        List<ItemName> trainingMoves = [ItemName.DIVING, ItemName.ORANGE_THROWING, ItemName.BARREL_THROWING, ItemName.VINE_SWINGING, ItemName.CLIMBING];
        foreach (var item in trainingMoves)
        {
            TrainingMoves.Add(new(
                item,
                _itemTrackingService,
                _parsedSpoilerDataService,
                _savedProgressProvider,
                _themeService
            ));
        }

        List<ItemName> miscMoves = [
            ItemName.PROGRESSIVE_SLAM_1,
            ItemName.PROGRESSIVE_SLAM_2,
            ItemName.PROGRESSIVE_SLAM_3,
            ItemName.BEAN,
            ItemName.SNIPER_SCOPE,
            ItemName.HOMING_AMMO,
            ItemName.FAIRY_CAMERA,
            ItemName.SHOCKWAVE
        ];
        foreach (var item in miscMoves)
        {
            MiscMoves.Add(new(
                item,
                _itemTrackingService,
                _parsedSpoilerDataService,
                _savedProgressProvider,
                _themeService
            ));
        }

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
                CollectiblesGridRowViewModel.Dispose();
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
