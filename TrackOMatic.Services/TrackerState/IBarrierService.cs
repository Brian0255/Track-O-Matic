using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// Manages barrier / lock constraints for the various B. Lockers and Helm Doors.
/// B. Lockers are region-based (one per lobby region).
/// Helm Doors are technically indexed, but now use an enum themselves.
/// </summary>
public interface IBarrierService
{
    #region B. Locker's domain (Region Based)
    /// <summary>Get the count hint for a B. Locker in a specific region.</summary>
    string GetBLockerCount(RegionName region);

    /// <summary>Set the count hint for a B. Locker in a specific region.</summary>
    void SetBLockerCount(RegionName region, string count);

    /// <summary>Get the item type displayed in a B. Locker in a specific region.</summary>
    BarrierItems GetBLockerItemType(RegionName region);

    /// <summary>Set the item type displayed in a B. Locker in a specific region.</summary>
    void SetBLockerItemType(RegionName region, BarrierItems itemType);

    /// <summary>
    /// Load the B. Locker counts from a save file without raising events.
    /// </summary>
    /// <remarks>
    /// <strong>Internal use only.</strong> This is not meant to be called by external code!
    /// </remarks>
    /// <param name="counts"></param>
    void LoadBLockerCountsSilently(IDictionary<RegionName, string> counts);

    /// <summary>
    /// Load the B. Locker item types from a save file without raising events.
    /// </summary>
    /// <remarks>
    /// <strong>Internal use only.</strong> This is not meant to be called by external code!
    /// </remarks>
    /// <param name="types"></param>
    void LoadBLockerTypesSilently(IDictionary<RegionName, int> types);
    #endregion

    #region Helm Door domain (select a door)
    /// <summary>Get the count hint for a Helm Door.</summary>
    string GetHelmDoorCount(HelmDoor helmDoor);

    /// <summary>Set the count hint for a Helm Door.</summary>
    void SetHelmDoorCount(HelmDoor helmDoor, string count);

    /// <summary>Get the item type displayed in a Helm Door.</summary>
    BarrierItems GetHelmDoorItemType(HelmDoor helmDoor);

    /// <summary>Set the item type displayed in a Helm Door.</summary>
    void SetHelmDoorItemType(HelmDoor helmDoor, BarrierItems itemType);

    /// <summary>
    /// Load the Helm Door counts from a save file without raising events.
    /// </summary>
    /// <remarks>
    /// <strong>Internal use only.</strong> This is not meant to be called by external code!
    /// </remarks>
    /// <param name="counts"></param>
    void LoadHelmDoorCountsSilently(IList<string> counts);

    /// <summary>
    /// Load the Helm Door item types from a save file without raising events.
    /// </summary>
    /// <remarks>
    /// <strong>Internal use only.</strong> This is not meant to be called by external code!
    /// </remarks>
    /// <param name="types"></param>
    void LoadHelmDoorTypesSilently(IList<int> types);
    #endregion

    #region Event Handlers
    event EventHandler<BlockerBarrierAmountChangedEventArgs>? BlockerBarrierAmountChanged;

    event EventHandler<BlockerBarrierTypeChangedEventArgs>? BlockerBarrierTypeChanged;

    event EventHandler<HelmBarrierAmountChangedEventArgs>? HelmBarrierAmountChanged;

    event EventHandler<HelmBarrierTypeChangedEventArgs>? HelmBarrierTypeChanged;
    #endregion
}
