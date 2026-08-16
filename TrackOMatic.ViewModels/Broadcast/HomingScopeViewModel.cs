using System.ComponentModel;
using System.Runtime.CompilerServices;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public class HomingScopeViewModel : INotifyPropertyChanged, IDisposable
{
    private static readonly ItemName _leftItem = ItemName.HOMING_AMMO;
    private static readonly ItemName _rightItem = ItemName.SNIPER_SCOPE;

    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;

    public HomingScopeViewModel(
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

    #region Proxy Properties

    private string _imageResourceKey = "";
    public string ImageResourceKey
    {
        get => _imageResourceKey;
        protected set
        {
            if (_imageResourceKey != value)
            {
                _imageResourceKey = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isStarred = false;
    public bool IsStarred
    {
        get => _isStarred;
        protected set
        {
            if (_isStarred != value)
            {
                _isStarred = value;
                OnPropertyChanged();
            }
        }
    }

    private string _hoverText = "";
    public string HoverText
    {
        get => _hoverText;
        protected set
        {
            if (_hoverText != value)
            {
                _hoverText = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _hasHoverText = false;
    public bool HasHoverText
    {
        get => _hasHoverText;
        protected set
        {
            if (_hasHoverText != value)
            {
                _hasHoverText = value;
                OnPropertyChanged();
            }
        }
    }

    #endregion

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

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

    ~HomingScopeViewModel()
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
        var leftItem = _itemTrackingService.GetItemState(_leftItem);
        var rightItem = _itemTrackingService.GetItemState(_rightItem);

        IsStarred = (leftItem is not null && leftItem.Starred == ItemVisibilityState.Visible) ||
        (rightItem is not null && rightItem.Starred == ItemVisibilityState.Visible);
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
}
