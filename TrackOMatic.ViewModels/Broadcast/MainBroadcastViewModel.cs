using System.Collections.ObjectModel;
using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public partial class MainBroadcastViewModel : ObservableObject, IDisposable
{
    private readonly IUserSettingsService _userSettingsService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;

    #region Proxy Properties

    private bool _topMostSetting;
    public bool TopMostSetting
    {
        get => _topMostSetting;
        private set => SetProperty(ref _topMostSetting, value);
    }

    private bool _broadcastHelmKRoolSetting;
    public bool BroadcastHelmKRoolSetting
    {
        get => _broadcastHelmKRoolSetting;
        private set => SetProperty(ref _broadcastHelmKRoolSetting, value);
    }

    private bool _broadcastShopkeepersSetting;
    public bool BroadcastShopkeepersSetting
    {
        get => _broadcastShopkeepersSetting;
        private set => SetProperty(ref _broadcastShopkeepersSetting, value);
    }

    private bool _broadcastSongDisplaySetting;
    public bool BroadcastSongDisplaySetting
    {
        get => _broadcastSongDisplaySetting;
        private set => SetProperty(ref _broadcastSongDisplaySetting, value);
    }

    private double _movesWidth;
    public double MovesWidth
    {
        get => _movesWidth;
        private set => SetProperty(ref _movesWidth, value);
    }

    public IslesViewModel Isles { get; init; }
    public ObservableCollection<LevelViewModel> Levels { get; } = [];

    public ObservableCollection<HelmSlotViewModel> HelmSlots { get; } = [];

    public ObservableCollection<BossSlotViewModel> BossSlots { get; } = [];

    public ObservableCollection<BroadcastItemViewModel> KongItems { get; } = [];

    public ObservableCollection<KongBlueprintViewModel> KongBlueprints { get; } = [];

    public ObservableCollection<BroadcastItemViewModel> ShopkeepItems { get; } = [];

    public ObservableCollection<CountableViewModel> Countables { get; } = [];

    public BroadcastItemViewModel Bean { get; init; }
    public BroadcastItemViewModel Diving { get; init; }
    public BroadcastItemViewModel Oranges { get; init; }
    public BroadcastItemViewModel Barrels { get; init; }
    public BroadcastItemViewModel Vines { get; init; }
    public ProgressiveSlamViewModel Slams { get; init; }
    public CameraShockwaveViewModel CameraShockwave { get; init; }
    public HomingScopeViewModel HomingScope { get; init; }
    public SongViewModel Song { get; init; }


    #endregion

    protected ItemName[] KongItemNames { get; } =
    [
        ItemName.DONKEY,
        ItemName.COCONUT_GUN,
        ItemName.BONGO_BLAST,
        ItemName.GORILLA_GRAB,
        ItemName.STRONG_KONG,
        ItemName.BABOON_BLAST,
        ItemName.DIDDY,
        ItemName.PEANUT_POPGUNS,
        ItemName.GUITAR_GAZUMP,
        ItemName.CHIMPY_CHARGE,
        ItemName.ROCKETBARREL_BOOST,
        ItemName.SIMIAN_SPRING,
        ItemName.LANKY,
        ItemName.GRAPE_SHOOTER,
        ItemName.TROMBONE_TREMOR,
        ItemName.ORANGSTAND,
        ItemName.ORANGSTAND_SPRINT,
        ItemName.BABOON_BALLOON,
        ItemName.TINY,
        ItemName.FEATHER_BOW,
        ItemName.SAXOPHONE_SLAM,
        ItemName.PONYTAIL_TWIRL,
        ItemName.MINI_MONKEY,
        ItemName.MONKEYPORT,
        ItemName.CHUNKY,
        ItemName.PINEAPPLE_LAUNCHER,
        ItemName.TRIANGLE_TRAMPLE,
        ItemName.PRIMATE_PUNCH,
        ItemName.HUNKY_CHUNKY,
        ItemName.GORILLA_GONE
    ];

    public MainBroadcastViewModel(
        IUserSettingsService userSettingsService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IItemTrackingService itemTrackingService,
        ILevelOrderService levelOrderService,
        ISavedProgressProvider savedProgressProvider,
        IEndGameProgressionService endGameProgressionService,
        IThemeService themeService,
        ICollectiblesService collectiblesService,
        ISongService songService
    )
    {
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));

        // Initialize all proxy properties from the service before subscribing to events
        TopMostSetting = _userSettingsService.TopMost;
        BroadcastHelmKRoolSetting = _userSettingsService.BroadcastHelmKRool;
        BroadcastShopkeepersSetting = _userSettingsService.BroadcastShopkeepers;
        BroadcastSongDisplaySetting = _userSettingsService.BroadcastSongDisplay;

        _userSettingsService.PropertyChanged += OnSettingChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;

        Isles = new(
            itemTrackingService,
            levelOrderService,
            parsedSpoilerDataService,
            userSettingsService
        );
        for (int i = 1; i <= 8; ++i)
        {
            Levels.Add(new(
                i,
                itemTrackingService,
                levelOrderService,
                parsedSpoilerDataService,
                userSettingsService,
                savedProgressProvider
            ));
        }
        for (int i = 0; i <= 4; ++i)
        {
            HelmSlots.Add(new(
                i,
                endGameProgressionService,
                parsedSpoilerDataService,
                savedProgressProvider
            ));
            BossSlots.Add(new(
                i,
                endGameProgressionService,
                parsedSpoilerDataService,
                savedProgressProvider
            ));
        }
        foreach (var itemName in KongItemNames)
        {
            KongItems.Add(new(
                itemName,
                itemTrackingService,
                parsedSpoilerDataService,
                savedProgressProvider,
                themeService
            ));
        }

        foreach (var itemName in new List<ItemName> { ItemName.CRANKY, ItemName.FUNKY, ItemName.CANDY, ItemName.SNIDE, ItemName.CLIMBING })
        {
            ShopkeepItems.Add(new(
                itemName,
                itemTrackingService,
                parsedSpoilerDataService,
                savedProgressProvider,
                themeService
            ));
        }

        foreach (var itemName in new List<ItemType> {  ItemType.PEARL, ItemType.BATTLE_CROWN, ItemType.BANANA_MEDAL, ItemType.RAINBOW_COIN, ItemType.FAIRY, ItemType.GOLDEN_BANANA})
        {
            Countables.Add(new(
                itemName,
                collectiblesService,
                parsedSpoilerDataService
            ));
        }

        var startingItem = (int)ItemType.DONKEY_BLUEPRINT;
        for (int i = startingItem; i < startingItem + 10; i += 2)
        {
            KongBlueprints.Add(new(
                (ItemType)i,
                (ItemType)(i + 1),
                collectiblesService,
                parsedSpoilerDataService,
                savedProgressProvider
            ));
        }

        Bean = new(
            ItemName.BEAN,
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            themeService
        );
        Diving = new(
            ItemName.DIVING,
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            themeService
        );
        Oranges = new(
            ItemName.ORANGE_THROWING,
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            themeService
        );
        Barrels = new(
            ItemName.BARREL_THROWING,
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            themeService
        );
        Vines = new(
            ItemName.VINE_SWINGING,
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            themeService
        );
        Slams = new(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider
        );
        CameraShockwave = new(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider
        );
        HomingScope = new(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider
        );

        Song = new(songService);

        AdjustLayout();
    }

    private void OnParsedSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e) => AdjustLayout();

    private void OnSettingChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IUserSettingsService.TopMost))
        {
            TopMostSetting = _userSettingsService.TopMost;
        }
        else if (e.PropertyName == nameof(IUserSettingsService.BroadcastHelmKRool))
        {
            BroadcastHelmKRoolSetting = _userSettingsService.BroadcastHelmKRool;
        }
        else if (e.PropertyName == nameof(IUserSettingsService.BroadcastShopkeepers))
        {
            BroadcastShopkeepersSetting = _userSettingsService.BroadcastShopkeepers;
        }
        else if (e.PropertyName == nameof(IUserSettingsService.BroadcastSongDisplay))
        {
            BroadcastSongDisplaySetting = _userSettingsService.BroadcastSongDisplay;
        }
    }

    private void AdjustLayout()
    {
        MovesWidth = _parsedSpoilerDataService.HasItemPoints() ? 315 : 345;
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
            }
            _disposed = true;
        }
    }

    ~MainBroadcastViewModel()
    {
        Dispose(false);
    }

    #endregion
}
