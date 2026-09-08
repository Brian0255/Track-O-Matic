using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services;

/// <summary>
/// Orchestrates placement of autotracked items into vial slots.
/// This implementation is stateless and query-driven; it queries regions for available slots
/// at placement time rather than maintaining registrations.
/// </summary>
public class RegionPlacementOrchestrator(
    IRegionSlotProvider slotProvider,
    IItemTrackingService itemTrackingService) : IRegionPlacementOrchestrator
{
    private readonly IRegionSlotProvider _slotProvider = slotProvider ?? throw new ArgumentNullException(nameof(slotProvider));
    private readonly IItemTrackingService _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));

    public bool TryPlaceAutoTrackedItem(ItemName itemName, RegionName targetRegion, SavedItem itemState)
    {
        // Query the region for its current vial slots
        var vials = _slotProvider.GetVialSlotsForRegion(targetRegion).ToList();
        if (vials.Count == 0)
        {
            return false;
        }

        var itemVialColor = itemName.ToVialColor();

        // Step 1: Check if item already exists in a slot of matching color
        var existingSlot = vials.FirstOrDefault(v =>
            v.VialColor == itemVialColor &&
            v.CurrentItemName == itemName);

        if (existingSlot != null)
        {
            // Item already placed; this is a no-op (state already updated)
            return true;
        }

        // Step 2: Look for an empty slot of matching color
        var emptySlot = vials.FirstOrDefault(v =>
            v.VialColor == itemVialColor &&
            v.CurrentItemName == null &&
            v.CanAcceptAutoTrackedItem(itemName, itemState));

        if (emptySlot != null)
        {
            emptySlot.AcceptAutoTrackedItem(itemName, itemState);
            return true;
        }

        // Step 3: Look for a non-autotracked slot of matching color
        var replaceable = vials.FirstOrDefault(v =>
        {
            if (v.VialColor != itemVialColor || v.CurrentItemName == null)
            {
                return false;
            }

            // Check if the current item is autotracked; if so, skip it
            var currentItemState = _itemTrackingService.GetItemState(v.CurrentItemName.Value);
            return currentItemState != null && !currentItemState.Autotracked && v.CanAcceptAutoTrackedItem(itemName, itemState);
        });

        if (replaceable != null)
        {
            replaceable.AcceptAutoTrackedItem(itemName, itemState);
            return true;
        }

        // Step 4: All matching vials are either occupied by autotracked items or unavailable
        return false;
    }
}
