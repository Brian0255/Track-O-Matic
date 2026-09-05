using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

/// <summary>
/// A view model for an item that users can see with the dedicated broadcast view.
/// </summary>
/// <remarks>
/// This is meant to be a base class version.
///Additional view models can extend this for write functionality.
/// </remarks>
public partial class BroadcastItemViewModel : ObservableObject, IEquatable<BroadcastItemViewModel>, IDisposable
{
    private static readonly HashSet<ItemName> _barrelPadItems =
    [
        ItemName.STRONG_KONG,
        ItemName.ROCKETBARREL_BOOST,
        ItemName.ORANGSTAND,
        ItemName.MINI_MONKEY,
        ItemName.HUNKY_CHUNKY,
        ItemName.BABOON_BLAST,
        ItemName.SIMIAN_SPRING,
        ItemName.MONKEYPORT,
        ItemName.GORILLA_GONE
    ];

    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IThemeService _themeService;
    private readonly ItemName _itemName;

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

    public BroadcastItemViewModel(
        ItemName itemName,
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IThemeService themeService
    )
    {
        _itemName = itemName;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnSavedProgressChanged;

        if (_barrelPadItems.Contains(_itemName))
        {
            _themeService.BarrelPadThemeChanged += OnBarrelPadThemeChanged;
        }

        // Initialize state from service (if item exists)
        InitializeState();
    }

    /// <summary>
    /// Initialize properties from current service state.
    /// Called on construction and when state needs to be refreshed.
    /// </summary>
    private void InitializeState()
    {
        var itemState = _itemTrackingService.GetItemState(_itemName);

        if (itemState != null)
        {
            UpdateImageResourceKey(itemState);
            IsStarred = itemState.IsStarred;
        }
        else
        {
            // No saved state yet - show default B&W variant
            UpdateImageResourceKey(null);
            IsStarred = false;
        }

        UpdateHoverText();
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unsubscribe from events to prevent memory leaks
            _itemTrackingService.ItemStateChanged -= OnItemStateChanged;
            _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnParsedSpoilerDataChanged;
            if (_barrelPadItems.Contains(_itemName))
            {
                _themeService.BarrelPadThemeChanged -= OnBarrelPadThemeChanged;
            }
        }
    }

    ~BroadcastItemViewModel()
    {
        Dispose(false);
    }

    #endregion

    private void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        // Handle the item state change event here.
        if (e.UpdatedItem.ItemName == _itemName)
        {
            // Update the ImageResourceKey based on the new state of the item.
            UpdateImageResourceKey(e.UpdatedItem);
            IsStarred = e.UpdatedItem.IsStarred;

            // Update hover text from spoiler service if available
            UpdateHoverText();
        }
    }

    /// <summary>
    /// Handles changes to the parsed spoiler data (load, reload, or clear).
    /// Reinitializes the image based on the new spoiler data.
    /// </summary>
    protected virtual void OnParsedSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e)
    {
        // When parsed spoiler data changes, reinitialize to reflect the new spoiler state
        InitializeState();
    }

    /// <summary>
    /// Handles changes to the saved progress (primarily on load or reset).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnSavedProgressChanged(object? sender, ProgressReplacedEventArgs e)
    {
        InitializeState();
    }

    private void OnBarrelPadThemeChanged(object? sender, EventArgs e)
    {
        // Force an update of the image resource key to reflect the new theme setting
        var itemState = _itemTrackingService.GetItemState(_itemName);
        ImageResourceKey = string.Empty;
        UpdateImageResourceKey(itemState);
    }

    protected void UpdateHoverText()
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
            var targetCategory = _itemName.ToPointCategory();
            var pointValue = pointSpread[targetCategory];
            var suffix = pointValue == 1 ? "Point" : "Points";
            HoverText = $"{pointValue} {suffix}";
            HasHoverText = true;
        }

        OnPropertyChanged(nameof(HoverText));
        OnPropertyChanged(nameof(HasHoverText));
    }

    private void UpdateImageResourceKey(SavedItem? item)
    {
        string baseKey = _itemName.ToString().ToLower();
        RegionName spoiledRegion = RegionName.UNKNOWN;
        if (_parsedSpoilerDataService.CurrentData?.StartingItems.ContainsKey(_itemName) == true)
        {
            spoiledRegion = _parsedSpoilerDataService.CurrentData.StartingItems[_itemName];
        }

        // If the spoiled region is a valid move region, show full-color
        if (spoiledRegion != RegionName.UNKNOWN)
        {
            ImageResourceKey = baseKey;
            return;
        }

        // If no item state, show default B&W variant
        if (item == null)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }

        // If item has a non standard opacity, it's user hinted. Show as B&W.
        if (item.Opacity != 1.0)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }

        // If item is in a valid region, show full-color
        if (EndGameMappings.ValidMoveRegions.Contains(item.Region))
        {
            ImageResourceKey = baseKey;
            return;
        }

        // If item is hinted, always show B&W
        // This logic may need to be revised.
        if (item.Hinted == true)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }

        // Otherwise, item is in ItemGrid or unknown region, show B&W
        ImageResourceKey = $"{baseKey}_bw";
    }

    #region Object Overrides

    public bool Equals(BroadcastItemViewModel? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return _itemName == other._itemName;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as BroadcastItemViewModel);
    }

    public override int GetHashCode()
    {
        return _itemName.GetHashCode();
    }

    public override string ToString()
    {
        return $"BroadcastItemViewModel: {_itemName}{(IsStarred ? "*" : "")}";
    }

    #endregion
}
