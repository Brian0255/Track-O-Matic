using System.ComponentModel;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public class KeyViewModel : ObservableObject, IDisposable
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly int _selectedKey;

    public KeyViewModel(
        int selectedKey,
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider)
    {
        _selectedKey = selectedKey;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));

        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnDataReset;
        _savedProgressProvider.ProgressChanged += OnDataReset;

        InitializeState();
    }

    private void InitializeState()
    {
        var targetKey = $"KEY_{_selectedKey}";
        if (!Enum.TryParse<ItemName>(targetKey, out var itemName))
        {
            return;
        }

        var itemState = _itemTrackingService.GetItemState(itemName);
        UpdateImageResourceKey(itemState, itemName);

        IsStarred = itemState is not null && itemState.IsStarred;
        UpdateHoverText();

    }

    #region Proxy Properties
    private string _imageResourceKey = "";
    public string ImageResourceKey
    {
        get => _imageResourceKey;
        private set => SetProperty(ref _imageResourceKey, value);
    }

    private bool _isStarred = false;
    public bool IsStarred
    {
        get => _isStarred;
        private set => SetProperty(ref _isStarred, value);
    }

    private string _hoverText = "";
    public string HoverText
    {
        get => _hoverText;
        private set => SetProperty(ref _hoverText, value);
    }

    private bool _hasHoverText = false;
    public bool HasHoverText
    {
        get => _hasHoverText;
        protected set => SetProperty(ref _hasHoverText, value);
    }

    #endregion

    #region IDisposable

    private bool _disposed = false;

    // Cleanup
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
                _itemTrackingService.ItemStateChanged -= OnItemStateChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnDataReset;
                _savedProgressProvider.ProgressChanged -= OnDataReset;
            }
            _disposed = true;
        }
    }

    ~KeyViewModel()
    {
        Dispose(false);
    }

    #endregion

    private void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        var targetKey = $"KEY_{_selectedKey}";
        if (!Enum.TryParse<ItemName>(targetKey, out var itemName))
        {
            return;
        }
        if (e.UpdatedItem.ItemName == itemName)
        {
            UpdateImageResourceKey(e.UpdatedItem, itemName);
            IsStarred = e.UpdatedItem.IsStarred;
            UpdateHoverText();
        }
    }
    private void OnDataReset(object? sender, EventArgs e)
    {
        InitializeState();
    }

    private void UpdateHoverText()
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
            var targetCategory = ItemName.BASIC_KEY.ToPointCategory();
            var pointValue = pointSpread[targetCategory];
            var suffix = pointValue == 1 ? "Point" : "Points";
            HoverText = $"{pointValue} {suffix}";
            HasHoverText = true;
        }

        OnPropertyChanged(nameof(HoverText));
        OnPropertyChanged(nameof(HasHoverText));
    }

    private void UpdateImageResourceKey(SavedItem? item, ItemName itemName)
    {
        string baseKey = "basic_key";
        RegionName spoiledRegion = RegionName.UNKNOWN;
        if (_parsedSpoilerDataService.CurrentData?.StartingItems.ContainsKey(itemName) == true)
        {
            spoiledRegion = _parsedSpoilerDataService.CurrentData.StartingItems[itemName];
        }

        // If no item state, show default B&W variant
        if (item == null && spoiledRegion == RegionName.UNKNOWN)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }

        // If item is hinted, always show B&W
        // This logic may need to be revised.
        if (item?.Hinted == true)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }

        // If the item is user hinted (opacity not 1, always show B&W)
        if (item is not null && item.Opacity < 1.0)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }

        // If item is in a valid region, show full-color
        if (spoiledRegion != RegionName.UNKNOWN || EndGameMappings.ValidMoveRegions.Contains(item?.Region ?? RegionName.UNKNOWN))
        {
            ImageResourceKey = baseKey;
            return;
        }

        // Otherwise, item is in ItemGrid or unknown region, show B&W
        ImageResourceKey = $"{baseKey}_bw";
    }

    #region Object Overrides

    public override bool Equals(object? obj) => obj is KeyViewModel other && other._selectedKey == _selectedKey;

    public override int GetHashCode() => _selectedKey.GetHashCode();

    #endregion
}
