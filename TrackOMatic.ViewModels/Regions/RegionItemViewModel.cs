using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Regions;

/// <summary>
/// A view model representing an item in a specific region, providing properties and methods for UI binding and interaction.
/// </summary>
public partial class RegionItemViewModel : ObservableObject, IEquatable<RegionItemViewModel>, IRegionItemViewModel, IVialSlot
{
    protected ItemName? _itemName;
    protected readonly RegionName _regionName;
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly IThemeService _themeService;

    public virtual VialColor VialColor => VialColor.NONE; // Not used in this view model but must be implemented.
    public virtual ItemName? CurrentItemName
    {
        get => _itemName;
        protected set => SetProperty(ref _itemName, value);
    }
    protected internal IItemTrackingService ItemTrackingService => _itemTrackingService;
    public virtual RegionName RegionName => _regionName;

    #region Proxy Properties

    [ObservableProperty]
    public partial string ImageResourceKey { get; set; } = "";

    [ObservableProperty]
    public partial bool IsStarred { get; set; } = false;

    [ObservableProperty]
    public partial double Opacity { get; set; } = 1.0;

    [ObservableProperty]
    public partial string HoverText { get; set; } = "";

    [ObservableProperty]
    public partial bool HasHoverText { get; set; } = false;

    #endregion

    public RegionItemViewModel(
        ItemName? itemName,
        RegionName regionName,
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IThemeService themeService
    )
    {
        _itemName = itemName;
        _regionName = regionName;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        // Subscribe to events
        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        if (_itemName.HasValue && EndGameMappings.BarrelPadItems.Contains(_itemName.Value))
        {
            _themeService.BarrelPadThemeChanged += OnBarrelPadThemeChanged;
        }
        // Initialize state from service (if item exists)
        InitializeState();
    }

    private void InitializeState()
    {
        var itemState = _itemName.HasValue ? _itemTrackingService.GetItemState(_itemName.Value) : null;
        UpdateProperties(itemState);
    }

    /// <summary>
    /// A no-op implementation since this slot is already occupied with an item and cannot accept any drops.
    /// </summary>
    /// <param name="itemToPlace"></param>
    /// <param name="dragType"></param>
    /// <returns></returns>
    public virtual bool CanAcceptDrop(ItemName itemToPlace, MouseDragType dragType) => false;

    /// <summary>
    /// A no-op implementation since this slot is already occupied with an item and cannot accept any drops.
    /// </summary>
    /// <param name="itemToPlace"></param>
    /// <param name="dragType"></param>
    /// <returns></returns>
    public virtual bool AcceptDropAndMutate(ItemName itemToPlace, MouseDragType dragType) => false;

    [RelayCommand(CanExecute = nameof(CanRemoveFromRegion))]
    private void RemoveFromRegion()
    {
        var current = _itemName.HasValue ? _itemTrackingService.GetItemState(_itemName.Value) : null;
        if (current is not null)
        {
            _itemTrackingService.SetItemState(_itemName!.Value, current with
            {
                Region = RegionName.UNKNOWN,
                Opacity = 1,
            });
        }
    }

    private bool CanRemoveFromRegion()
    {
        if (!CurrentItemName.HasValue)
        {
            return false;
        }
        var currentState = _itemTrackingService.GetItemState(CurrentItemName.Value);
        if (currentState is null)
        {
            return false;
        }
        if (currentState.Autotracked)
        {
            return false;
        }
        return true;
    }

    [RelayCommand(CanExecute = nameof(CanToggleStar))]
    private void ToggleStar()
    {
        if (CurrentItemName is null)
        {
            return;
        }
        _itemTrackingService.ToggleStar(CurrentItemName.Value);
    }

    private bool CanToggleStar()
    {
        return CurrentItemName is not null && CurrentItemName.HasValue;
    }

    public int GetHoardPoints()
    {
        if (CurrentItemName is null)
        {
            return 0;
        }
        var itemState = _itemTrackingService.GetItemState(CurrentItemName.Value);
        if (itemState is null)
        {
            return 0;
        }
        return itemState.Starred ? 1 : 0;
    }

    public int GetItemPoints()
    {
        if (CurrentItemName is null)
        {
            return 0;
        }

        return _parsedSpoilerDataService.GetPointsForItem(CurrentItemName.Value);
    }

    protected virtual void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        if (e.UpdatedItem.ItemName == _itemName)
        {
            UpdateProperties(e.UpdatedItem);
        }
    }

    protected virtual void OnBarrelPadThemeChanged(object? sender, EventArgs e)
    {
        // If the item is a barrel pad, we need to update the image resource key to reflect the new theme.
        if (_itemName.HasValue && EndGameMappings.BarrelPadItems.Contains(_itemName.Value))
        {
            var itemState = _itemTrackingService.GetItemState(_itemName.Value);
            // Force the frontend to re-evaluate the binding via temporary property change.
            ImageResourceKey = "";
            UpdateProperties(itemState);
        }
    }

    protected virtual void UpdateProperties(SavedItem? itemState)
    {
        if (itemState == null || !_itemName.HasValue)
        {
            ImageResourceKey = "";
            IsStarred = false;
            Opacity = 1.0;
        }
        else
        {
            // Set image based on item state
            string baseKey = _itemName.Value.ToString().ToLower();
            ImageResourceKey = itemState.Hinted ? $"{baseKey}_bw" : baseKey;
            IsStarred = itemState.Starred;
            Opacity = itemState.Opacity;
        }

        UpdateHoverText();
    }

    protected virtual void UpdateHoverText()
    {
        var spoilerData = _parsedSpoilerDataService.CurrentData;
        if (spoilerData is null || _itemName is null || !spoilerData.SpoilerSettings.PointsEnabled)
        {
            HoverText = "";
            HasHoverText = false;
        }
        else
        {
            var pointSpread = spoilerData.PointSpread;
            var targetCategory = _itemName.Value.ToPointCategory();
            var pointValue = pointSpread[targetCategory];
            var suffix = pointValue == 1 ? "Point" : "Points";
            HoverText = $"{pointValue} {suffix}";
            HasHoverText = true;
        }

        OnPropertyChanged(nameof(HoverText));
        OnPropertyChanged(nameof(HasHoverText));
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
        if (disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            _itemTrackingService.ItemStateChanged -= OnItemStateChanged;
            if (_itemName.HasValue && EndGameMappings.BarrelPadItems.Contains(_itemName.Value))
            {
                _themeService.BarrelPadThemeChanged -= OnBarrelPadThemeChanged;
            }
        }
    }

    ~RegionItemViewModel()
    {
        Dispose(false);
    }

    #endregion

    #region IVialSlot Implementation

    /// <summary>
    /// Checks if an autotracked item can be placed in this slot.
    /// RegionItemViewModel slots don't have the same restrictions as vial slots,
    /// so this typically returns true if the slot is empty or can accept replacement.
    /// </summary>
    public virtual bool CanAcceptAutoTrackedItem(ItemName itemToPlace, SavedItem itemState)
    {
        // In non-spoiler regions, any item can be displaced by autotracking
        // unless it's already autotracked
        if (_itemName.HasValue)
        {
            var currentItemState = _itemTrackingService.GetItemState(_itemName.Value);
            if (currentItemState?.Autotracked == true)
            {
                return false; // Can't displace autotracked items
            }
        }
        return true;
    }

    /// <summary>
    /// Accepts an autotracked item into this slot and updates ItemTrackingService.
    /// The view model will receive the updated state via ItemStateChanged event.
    /// </summary>
    public virtual void AcceptAutoTrackedItem(ItemName itemToPlace, SavedItem itemState)
    {
        // Write the autotracked item to the service; this will trigger ItemStateChanged
        _itemTrackingService.SetItemState(itemToPlace, itemState, ChangeReason.AutoTracked);
    }

    #endregion

    #region Object Overrides

    public bool Equals(RegionItemViewModel? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return _regionName == other._regionName;
    }

    public override bool Equals(object? obj) => Equals(obj as RegionItemViewModel);

    public override int GetHashCode()
    {
        return _regionName.GetHashCode();
    }

    public override string ToString()
    {
        return $"{CurrentItemName}{(IsStarred ? "*" : "")} in {RegionName}";
    }

    #endregion
}
