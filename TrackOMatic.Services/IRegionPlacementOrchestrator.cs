using System;
using System.Collections.Generic;
using System.Text;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services;

/// <summary>
/// Orchestrates placement of autotracked items into vial slots based on vial color matching
/// and autotrack state protection.
///
/// This orchestrator is stateless and query-driven; it queries regions for available slots
/// at placement time rather than maintaining registrations.
/// </summary>
public interface IRegionPlacementOrchestrator
{
    /// <summary>
    /// Attempts to place an autotracked item following the 4-step algorithm:
    /// 1. Check if item already exists in a matching slot
    /// 2. Fill empty slot of matching color
    /// 3. Replace non-autotracked item of matching color
    /// 4. Fail if all slots are autotracked or no slots exist
    ///
    /// This method queries the target region for available vial slots and attempts placement.
    /// Each slot handles its own state updates via ItemTrackingService subscription.
    /// </summary>
    /// <param name="itemName">The item being placed</param>
    /// <param name="targetRegion">The region where the item was found</param>
    /// <param name="itemState">The current state of the item being placed</param>
    /// <returns>True if placement succeeded; false otherwise</returns>
    bool TryPlaceAutoTrackedItem(ItemName itemName, RegionName targetRegion, SavedItem itemState);
}

/// <summary>
/// Represents a single vial slot that can accept item placement.
/// Implemented by VialItemViewModel.
/// </summary>
public interface IVialSlot
{
    /// <summary>
    /// The item currently occupying this slot, if any.
    /// </summary>
    ItemName? CurrentItemName { get; }

    /// <summary>
    /// The color requirement for this vial.
    /// </summary>
    VialColor VialColor { get; }

    /// <summary>
    /// The region this vial belongs to.
    /// </summary>
    RegionName RegionName { get; }

    /// <summary>
    /// Checks if an item can be placed in this slot (considers vial color and autotrack protection).
    /// </summary>
    bool CanAcceptAutoTrackedItem(ItemName itemToPlace, SavedItem itemState);

    /// <summary>
    /// Places an autotracked item in this slot and updates state via ItemTrackingService.
    /// Only called if CanAcceptAutoTrackedItem returned true.
    /// The slot will receive the update via ItemStateChanged and update its own state.
    /// </summary>
    void AcceptAutoTrackedItem(ItemName itemToPlace, SavedItem itemState);
}

/// <summary>
/// Provides access to regions and their vial slots.
/// Implemented by the view model container (e.g., MainWindow or a dedicated registry).
/// </summary>
public interface IRegionSlotProvider
{
    /// <summary>
    /// Gets all vial slots in a specific region.
    /// </summary>
    /// <param name="regionName">The region to query</param>
    /// <returns>Collection of vial slots in that region, or empty if region not found</returns>
    IEnumerable<IVialSlot> GetVialSlotsForRegion(RegionName regionName);
}
