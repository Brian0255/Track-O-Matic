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
/// A view model representing a vial item by default, only to be covered by a real item when found in a region.
/// </summary>
public partial class VialItemViewModel : RegionItemViewModel, IEquatable<VialItemViewModel>, IVialSlot
{
    private readonly VialColor _vialColor;
    private readonly bool _isStartRegion;

    /// <summary>
    /// Gets the vial color in this slot.
    /// </summary>
    public override VialColor VialColor => _vialColor;

    public event EventHandler<VialItemStateChangedEventArgs>? VialItemStateChanged;

    #region Proxy Properties

    [ObservableProperty]
    public partial bool IsVialStarred { get; set; } = false;

    #endregion

    public VialItemViewModel(
        ItemName? itemName,
        RegionName regionName,
        VialColor vialColor,
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IThemeService themeService
    ) : base(itemName, regionName, itemTrackingService, parsedSpoilerDataService, themeService)
    {
        if (regionName == RegionName.START && !itemName.HasValue)
        {
            throw new ArgumentException("Start region vial must have an item name.", nameof(itemName));
        }
        if (regionName != RegionName.START && itemName.HasValue)
        {
            throw new ArgumentException("Non-start region vial cannot have an item name.", nameof(itemName));
        }
        _vialColor = vialColor;
        _isStartRegion = regionName == RegionName.START;

        // Re-initialize properties now that _vialColor is set.
        // The base constructor calls InitializeState which calls UpdateProperties,
        // but at that time _vialColor was not yet initialized, so we need to call it again.
        var itemState = itemName.HasValue ? itemTrackingService.GetItemState(itemName.Value) : null;
        UpdateProperties(itemState);

        // Initialize the vial starred state based on the item state
        if (_isStartRegion && itemName.HasValue)
        {
            itemTrackingService.SetItemState(itemName.Value, SavedItem.CreateEmpty(itemName.Value) with
            {
                Region = regionName,
                Starred = false,
                Autotracked = false,
                Opacity = 1.0,
                Hinted = false
            });
        }
    }

    #region IVialSlot Implementation

    public override bool CanAcceptAutoTrackedItem(ItemName itemToPlace, SavedItem itemState)
    {
        // Vial color must match the item type.
        if (itemToPlace.ToVialColor() != _vialColor)
        {
            return false;
        }

        // Start region: only the specific item can be placed in the start region.
        if (_isStartRegion)
        {
            return CurrentItemName == itemToPlace;
        }

        if (!CurrentItemName.HasValue)
        {
            return true;
        }

        var currentState = ItemTrackingService.GetItemState(CurrentItemName.Value);
        return currentState is not null && !currentState.Autotracked;
    }

    public override void AcceptAutoTrackedItem(ItemName itemToPlace, SavedItem itemState)
    {
        // Prepare the itemState with any vial-specific adjustments (e.g., transfer vial star to item star)
        var hasStarToTransfer = IsVialStarred;
        var cleanState = itemState with
        {
            Starred = hasStarToTransfer || itemState.Starred,
        };

        // Clear the vial star since we're transferring it to the item
        IsVialStarred = false;

        // Set the item as the current occupant
        CurrentItemName = itemToPlace;

        // Update the service state. This will eventually fire ItemStateChanged,
        // but we handle the UI update synchronously to ensure no race condition
        // between setting CurrentItemName and receiving the event.
        ItemTrackingService.SetItemState(itemToPlace, cleanState, ChangeReason.AutoTracked);

        // Update properties immediately on the calling thread. This is safe because:
        // 1. CurrentItemName is now set, so the binding is established
        // 2. ItemStateChanged will be marshaled to the UI thread, ensuring consistency
        // 3. Any subsequent OnItemStateChanged will simply re-apply the same state
        UpdateProperties(cleanState);
    }

    #endregion

    #region Overridden Methods

    /// <summary>
    /// Determine if this vial slot can accept the dropped item.
    /// </summary>
    /// <remarks>This checks the item type, vial color, and autotracked state.</remarks>
    /// <param name="itemToPlace">The item to place</param>
    /// <param name="dragType">The type of mouse drag</param>
    /// <returns>True if the drop can be accepted; otherwise, false.</returns>
    public override bool CanAcceptDrop(ItemName itemToPlace, MouseDragType dragType)
    {
        // Start region: user drag/drops forbidden, but autotracking is allowed
        if (_isStartRegion && dragType != MouseDragType.None)
        {
            return false;
        }

        // Vial color must match the item type.
        if (itemToPlace.ToVialColor() != _vialColor)
        {
            return false;
        }

        // If the slot is occupied by an autotracked item, it must be protected.
        if (CurrentItemName.HasValue && IsItemAutotracked(CurrentItemName.Value))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Accept the dropped item and mutate the state accordingly.
    /// </summary>
    /// <param name="itemToPlace">The item to place</param>
    /// <param name="dragType">The type of mouse drag</param>
    /// <returns>True if the drop was accepted and the state was mutated; otherwise, false.</returns>
    public override bool AcceptDropAndMutate(ItemName itemToPlace, MouseDragType dragType)
    {
        if (!CanAcceptDrop(itemToPlace, dragType))
        {
            return false;
        }

        // Transfer the star over.
        var hasStarToTransfer = IsVialStarred;
        IsVialStarred = false;

        var oldItemState = ItemTrackingService.GetItemState(itemToPlace) ?? SavedItem.CreateEmpty(itemToPlace);
        var newItemState = oldItemState with
        {
            Region = RegionName,
            Starred = hasStarToTransfer || oldItemState.Starred,
        };
        ItemTrackingService.SetItemState(itemToPlace, newItemState);
        CurrentItemName = itemToPlace;
        IsVialStarred = false;
        UpdateProperties(newItemState);
        return true;
    }

    /// <summary>
    /// Removes the item from the region if possible.
    /// </summary>
    /// <remarks>Any failures to remove the item are silently ignored.</remarks>
    [RelayCommand(CanExecute = nameof(CanRemoveVialFromRegion))]
    private void RemoveVialFromRegion()
    {
        var currentState = ItemTrackingService.GetItemState(CurrentItemName!.Value);
        ItemTrackingService.SetItemState(CurrentItemName.Value, currentState! with
        {
            Region = RegionName.UNKNOWN,
            Opacity = 1,
        });
        CurrentItemName = null;
        UpdateProperties(null);
        RaiseVialItemStateChanged();
    }

    private bool CanRemoveVialFromRegion()
    {
        if (_isStartRegion)
        {
            return false;
        }
        if (!CurrentItemName.HasValue)
        {
            return false;
        }
        var currentState = ItemTrackingService.GetItemState(CurrentItemName.Value);
        if (currentState is null)
        {
            return false;
        }
        if (IsItemAutotracked(CurrentItemName.Value))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Toggles the star state of the vial or the item, depending on whether an item is present.
    /// </summary>
    [RelayCommand]
    private void ToggleVialStar()
    {
        if (CurrentItemName.HasValue)
        {
            ItemTrackingService.ToggleStar(CurrentItemName.Value);
        }
        else
        {
            IsVialStarred = !IsVialStarred;
            // Update the IsStarred property to reflect the vial star state
            UpdateProperties(null);
        }

        RaiseVialItemStateChanged();
    }

    /// <summary>
    /// Handles item state changes. When the current item in this vial is moved to a different region,
    /// notify the change to force hoard recalculation. This ensures the hoard updates whether removal 
    /// happens via clicking the item in the UiGrid or left-clicking the vial directly.
    /// </summary>
    protected override void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        var wasItemInVial = CurrentItemName.HasValue && e.UpdatedItem.ItemName == CurrentItemName.Value;
        var itemMovedAway = wasItemInVial && e.UpdatedItem.Region != RegionName;

        // If the item in this vial changed, update its state
        if (CurrentItemName.HasValue && e.UpdatedItem.ItemName == CurrentItemName.Value)
        {
            UpdateProperties(e.UpdatedItem);
        }

        // If the item was moved away, clear vial star and force a property notification
        // to trigger hoard recalculation in RegionViewModel
        if (itemMovedAway)
        {
            IsVialStarred = false;
            // Force a PropertyChanged notification for IsVialStarred to trigger hoard update,
            // even if IsVialStarred was already false before
            OnPropertyChanged(nameof(IsVialStarred));
        }
        RaiseVialItemStateChanged();
    }

    protected override void UpdateProperties(SavedItem? itemState)
    {
        if (itemState is null || !CurrentItemName.HasValue)
        {
            IsStarred = IsVialStarred;
            ImageResourceKey = "vial_" + _vialColor.ToString().ToLower();
            Opacity = 1;
        }
        else if (itemState.Region == RegionName.UNKNOWN)
        {
            // The item was removed: restore the vial.
            CurrentItemName = null;
            IsStarred = false;
            ImageResourceKey = "vial_" + _vialColor.ToString().ToLower();
            Opacity = 1;
        }
        else
        {
            // Set image based on item state
            string baseKey = CurrentItemName.Value.ToString().ToLower();
            ImageResourceKey = itemState.Hinted ? $"{baseKey}_bw" : baseKey;
            IsStarred = itemState.Starred;
            Opacity = itemState.Opacity;
        }
    }

    #endregion

    #region Helper Methods

    private void RaiseVialItemStateChanged()
    {
        VialItemStateChanged?.Invoke(this, new(
            RegionName,
            _vialColor,
            CurrentItemName,
            CurrentItemName.HasValue ? IsStarred : IsVialStarred
        ));
    }

    private bool IsItemAutotracked(ItemName itemName)
    {
        var itemState = ItemTrackingService.GetItemState(itemName);
        return itemState?.Autotracked ?? false;
    }

    #endregion

    #region Object Overrides

    public bool Equals(VialItemViewModel? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        if (RegionName != other.RegionName)
        {
            return false;
        }

        return _vialColor == other._vialColor;
    }

    public override bool Equals(object? obj) => Equals(obj as VialItemViewModel);

    public override int GetHashCode()
    {
        return HashCode.Combine(_vialColor, _regionName);
    }

    public override string ToString()
    {
        if (CurrentItemName.HasValue)
        {
            return $"{CurrentItemName.Value}{(IsStarred ? "*" : "")} ({VialColor}) in {RegionName}";
        }

        return $"{VialColor}{(IsVialStarred ? "*" : "")} in {RegionName}";
    }

    #endregion
}
