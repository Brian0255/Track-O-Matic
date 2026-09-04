using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public partial class CameraShockwaveViewModel : ObservableObject, IDisposable
{
    private static readonly ItemName _leftItem = ItemName.FAIRY_CAMERA;
    private static readonly ItemName _rightItem = ItemName.SHOCKWAVE;

    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;


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

    public CameraShockwaveViewModel(
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider
    )
    {
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnProgressChanged;

        InitializeState();
    }

    protected virtual void InitializeState()
    {
        SetStarred();
        UpdateImageResourceKey();
        UpdateHoverText();
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
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CameraShockwaveViewModel()
    {
        Dispose(false);
    }
    #endregion

    protected virtual void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        if (e.UpdatedItem.ItemName == _leftItem || e.UpdatedItem.ItemName == _rightItem)
        {
            SetStarred();
            UpdateImageResourceKey();
        }
    }

    protected virtual void OnParsedSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e)
    {
        InitializeState();
    }

    protected virtual void OnProgressChanged(object? sender, ProgressReplacedEventArgs e)
    {
        InitializeState();
    }

    protected virtual void UpdateHoverText()
    {
        var spoilerData = _parsedSpoilerDataService.CurrentData;
        if (spoilerData is null)
        {
            HoverText = "";
            HasHoverText = false;
        }
        else
        {
            var pointSpread = spoilerData.PointSpread;
            var targetCategory = ItemName.SHOCKWAVE.ToPointCategory();
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
        var leftItem = _itemTrackingService.GetItemState(_leftItem);
        var rightItem = _itemTrackingService.GetItemState(_rightItem);

        IsStarred =
            (leftItem is not null && leftItem.IsStarred) ||
            (rightItem is not null && rightItem.IsStarred);
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
            (false, false) => "camera_shockwave_bw",
            (true, false) => "fairycamonly",
            (false, true) => "shockwaveonly",
            (true, true) => "camera_shockwave",
        };
    }
}
