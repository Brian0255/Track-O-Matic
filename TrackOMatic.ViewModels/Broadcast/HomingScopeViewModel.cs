using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public partial class HomingScopeViewModel : ObservableObject, IDisposable
{
    private static readonly ItemName _leftItem = ItemName.HOMING_AMMO;
    private static readonly ItemName _rightItem = ItemName.SNIPER_SCOPE;

    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IDataSaverService _dataSaverService;


    #region Proxy Properties

    [ObservableProperty]
    public partial string ImageResourceKey { get; set; } = "";

    [ObservableProperty]
    public partial bool IsStarred { get; set; } = false;

    [ObservableProperty]
    public partial string HoverText { get; set; } = "";

    [ObservableProperty]
    public partial bool HasHoverText { get; set; } = false;

    #endregion

    public HomingScopeViewModel(
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IDataSaverService dataSaverService
    )
    {
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _dataSaverService = dataSaverService ?? throw new ArgumentNullException(nameof(dataSaverService));
        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnProgressChanged;
        _dataSaverService.DataLoadingCompleted += OnDataLoadingCompleted;

        InitializeState();
    }

    protected virtual void InitializeState()
    {
        SetStarred();
        UpdateImageResourceKey();
        UpdateHoverText();
    }

    protected virtual void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        if (e.UpdatedItem.ItemName == _leftItem || e.UpdatedItem.ItemName == _rightItem)
        {
            SetStarred();
            UpdateImageResourceKey();
        }
    }

    protected virtual void OnParsedSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e) => InitializeState();

    protected virtual void OnProgressChanged(object? sender, ProgressReplacedEventArgs e) => InitializeState();

    protected virtual void OnDataLoadingCompleted(object? sender, DataLoadingCompletedEventArgs e) => InitializeState();

    protected virtual void UpdateHoverText()
    {
        var spoilerData = _parsedSpoilerDataService.CurrentData;
        if (spoilerData is null || !spoilerData.SpoilerSettings.PointsEnabled)
        {
            HoverText = "";
            HasHoverText = false;
        }
        else
        {
            var pointSpread = spoilerData.PointSpread;
            var targetCategory = ItemName.HOMING_AMMO.ToPointCategory();
            var pointValue = pointSpread[targetCategory];
            var suffix = pointValue == 1 ? "Point" : "Points";
            HoverText = $"{pointValue} {suffix}";
            HasHoverText = true;
        }

        OnPropertyChanged(nameof(HoverText));
        OnPropertyChanged(nameof(HasHoverText));
    }

    protected virtual void SetStarred()
    {
        IsStarred = IsEitherItemStarred();
    }

    protected virtual void UpdateImageResourceKey()
    {
        var leftRegion = RegionName.UNKNOWN;
        if (_parsedSpoilerDataService.CurrentData?.StartingItems.ContainsKey(_leftItem) == true)
        {
            leftRegion = _parsedSpoilerDataService.CurrentData.StartingItems[_leftItem];
        }
        var leftItem = _itemTrackingService.GetItemState(_leftItem);
        var isLeftKnown = leftRegion != RegionName.UNKNOWN || (leftItem is not null && leftItem.Region != RegionName.UNKNOWN);

        var rightRegion = RegionName.UNKNOWN;
        if (_parsedSpoilerDataService.CurrentData?.StartingItems.ContainsKey(_rightItem) == true)
        {
            rightRegion = _parsedSpoilerDataService.CurrentData.StartingItems[_rightItem];
        }
        var rightItem = _itemTrackingService.GetItemState(_rightItem);
        var isRightKnown = rightRegion != RegionName.UNKNOWN || (rightItem is not null && rightItem.Region != RegionName.UNKNOWN);

        ImageResourceKey = (isLeftKnown, isRightKnown) switch
        {
            (false, false) => "homing_scope_bw",
            (true, false) => "homingonly",
            (false, true) => "scopeonly",
            (true, true) => "homing_scope",
        };
    }

    private bool IsEitherItemStarred()
    {
        var leftItem = _itemTrackingService.GetItemState(_leftItem);
        var rightItem = _itemTrackingService.GetItemState(_rightItem);
        return
            (leftItem is not null && leftItem.Starred) ||
            (rightItem is not null && rightItem.Starred);
    }

    #region IDisposable

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Unsubscribe from events
                _itemTrackingService.ItemStateChanged -= OnItemStateChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnParsedSpoilerDataChanged;
                _savedProgressProvider.ProgressChanged -= OnProgressChanged;
                _dataSaverService.DataLoadingCompleted -= OnDataLoadingCompleted;
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~HomingScopeViewModel()
    {
        Dispose(false);
    }
    #endregion
}
