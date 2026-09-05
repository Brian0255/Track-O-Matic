using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public partial class ProgressiveSlamViewModel : ObservableObject, IDisposable
{
    private static readonly ItemName _slam1 = ItemName.PROGRESSIVE_SLAM_1;
    private static readonly ItemName _slam2 = ItemName.PROGRESSIVE_SLAM_2;
    private static readonly ItemName _slam3 = ItemName.PROGRESSIVE_SLAM_3;

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

    public ProgressiveSlamViewModel(
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

    ~ProgressiveSlamViewModel()
    {
        Dispose(false);
    }
    #endregion

    protected virtual void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        if (e.UpdatedItem.ItemName == _slam1 || e.UpdatedItem.ItemName == _slam2 || e.UpdatedItem.ItemName == _slam3)
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
            var targetCategory = ItemName.PROGRESSIVE_SLAM_1.ToPointCategory();
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
        var slam1 = _itemTrackingService.GetItemState(_slam1);
        var slam2 = _itemTrackingService.GetItemState(_slam2);
        var slam3 = _itemTrackingService.GetItemState(_slam3);
        IsStarred =
            (slam1 is not null && slam1.Starred) ||
            (slam2 is not null && slam2.Starred) ||
            (slam3 is not null && slam3.Starred);
        }

    protected virtual void UpdateImageResourceKey()
    {
        ItemName[] slamArray = [_slam1, _slam2, _slam3];
        var counts = slamArray.Count(slam =>
        {
            // Check if the slam is in spoiler starting items
            var inSpoilerData = _parsedSpoilerDataService.CurrentData?.StartingItems.ContainsKey(slam) == true
                            && _parsedSpoilerDataService.CurrentData.StartingItems[slam] != RegionName.UNKNOWN;

            // Check if the slam is tracked in a known region
            var trackedItem = _itemTrackingService.GetItemState(slam);
            var inTracking = trackedItem is not null && trackedItem.Region != RegionName.UNKNOWN;

            // Count this slam if it's in either source
            return inSpoilerData || inTracking;
        });

        var newKey = counts switch
        {
            1 => "progressive_slam_1_bc",
            2 => "progressive_slam_2_bc",
            3 => "progressive_slam_3_bc",
            _ => "progressive_slam_1_bc_bw",
        }
    ;

        // Force WPF to re-evaluate the binding by temporarily clearing the property
        ImageResourceKey = string.Empty;
        ImageResourceKey = newKey;
    }
}
