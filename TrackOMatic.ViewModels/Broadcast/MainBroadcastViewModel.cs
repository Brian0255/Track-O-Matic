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

    public ObservableCollection<LevelViewModel> Levels { get; } = [];

    public ObservableCollection<HelmSlotViewModel> HelmSlots { get; } = [];

    public ObservableCollection<BossSlotViewModel> BossSlots { get; } = [];

    #endregion

    public MainBroadcastViewModel(
        IUserSettingsService userSettingsService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IItemTrackingService itemTrackingService,
        ILevelOrderService levelOrderService,
        ISavedProgressProvider savedProgressProvider,
        IEndGameProgressionService endGameProgressionService
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
