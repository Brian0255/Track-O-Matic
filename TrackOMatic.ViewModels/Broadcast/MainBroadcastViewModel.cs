using System.Collections.ObjectModel;
using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public partial class MainBroadcastViewModel : ObservableObject, IDisposable
{
    private readonly IUserSettingsService _userSettingsService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;

    #region Proxy Properties

    [ObservableProperty]
    public partial bool TopMostSetting { get; set; }

    [ObservableProperty]
    public partial bool BroadcastHelmKRoolSetting { get; set; }

    [ObservableProperty]
    public partial bool BroadcastShopkeepersSetting { get; set; }

    [ObservableProperty]
    public partial bool BroadcastSongDisplaySetting { get; set; }

    [ObservableProperty]
    public partial double MovesWidth { get; set; }

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

    public MainBroadcastViewModel(
        IUserSettingsService userSettingsService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IItemTrackingService itemTrackingService,
        ILevelOrderService levelOrderService,
        ISavedProgressProvider savedProgressProvider,
        IEndGameProgressionService endGameProgressionService,
        IThemeService themeService,
        ICollectiblesService collectiblesService,
        ISongService songService,
        IDataSaverService dataSaverService
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
            userSettingsService,
            dataSaverService
        );
        for (int i = 1; i <= 8; ++i)
        {
            Levels.Add(new(
                i,
                itemTrackingService,
                levelOrderService,
                parsedSpoilerDataService,
                userSettingsService,
                savedProgressProvider,
                dataSaverService
            ));
        }
        for (int i = 0; i <= 4; ++i)
        {
            HelmSlots.Add(new(
                i,
                endGameProgressionService,
                parsedSpoilerDataService,
                savedProgressProvider,
                dataSaverService
            ));
            BossSlots.Add(new(
                i,
                endGameProgressionService,
                parsedSpoilerDataService,
                savedProgressProvider,
                dataSaverService
            ));
        }
        foreach (var itemName in Extensions.KongItemNames)
        {
            KongItems.Add(new(
                itemName,
                itemTrackingService,
                parsedSpoilerDataService,
                savedProgressProvider,
                themeService,
                dataSaverService
            ));
        }

        foreach (var itemName in new List<ItemName> { ItemName.CRANKY, ItemName.FUNKY, ItemName.CANDY, ItemName.SNIDE, ItemName.CLIMBING })
        {
            ShopkeepItems.Add(new(
                itemName,
                itemTrackingService,
                parsedSpoilerDataService,
                savedProgressProvider,
                themeService,
                dataSaverService
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
            themeService,
            dataSaverService
        );
        Diving = new(
            ItemName.DIVING,
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            themeService,
            dataSaverService
        );
        Oranges = new(
            ItemName.ORANGE_THROWING,
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            themeService,
            dataSaverService
        );
        Barrels = new(
            ItemName.BARREL_THROWING,
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            themeService,
            dataSaverService
        );
        Vines = new(
            ItemName.VINE_SWINGING,
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            themeService,
            dataSaverService
        );
        Slams = new(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            dataSaverService
        );
        CameraShockwave = new(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            dataSaverService
        );
        HomingScope = new(
            itemTrackingService,
            parsedSpoilerDataService,
            savedProgressProvider,
            dataSaverService
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
