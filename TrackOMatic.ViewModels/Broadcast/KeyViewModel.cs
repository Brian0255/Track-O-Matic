using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public partial class KeyViewModel : ObservableObject, IEquatable<KeyViewModel>, IDisposable
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IDataSaverService _dataSaverService;
    private readonly int _selectedKey;

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

    public KeyViewModel(
        int selectedKey,
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IDataSaverService dataSaverService
    )
    {
        _selectedKey = selectedKey;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _dataSaverService = dataSaverService ?? throw new ArgumentNullException(nameof(dataSaverService));

        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnDataReset;
        _savedProgressProvider.ProgressChanged += OnDataReset;
        _dataSaverService.DataLoadingCompleted += OnDataReset;

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

        IsStarred = itemState is not null && itemState.Starred;
        UpdateHoverText();
    }

    internal void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        var targetKey = $"KEY_{_selectedKey}";
        if (!Enum.TryParse<ItemName>(targetKey, out var itemName))
        {
            return;
        }
        if (e.UpdatedItem.ItemName == itemName)
        {
            UpdateImageResourceKey(e.UpdatedItem, itemName);
            IsStarred = e.UpdatedItem.Starred;
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
        if (spoilerData is null || !spoilerData.SpoilerSettings.PointsEnabled)
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

    public bool Equals(KeyViewModel? other) => other is not null && other._selectedKey == _selectedKey;

    public override bool Equals(object? obj) => Equals(obj as KeyViewModel);

    public override int GetHashCode() => _selectedKey.GetHashCode();

    public override string ToString()
    {
        return $"Key {_selectedKey}{(IsStarred ? "*" : "")}";
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
                _dataSaverService.DataLoadingCompleted -= OnDataReset;
            }
            _disposed = true;
        }
    }

    ~KeyViewModel()
    {
        Dispose(false);
    }

    #endregion
}
