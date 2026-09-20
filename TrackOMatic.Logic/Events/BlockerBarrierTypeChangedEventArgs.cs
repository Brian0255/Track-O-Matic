using System.Diagnostics.CodeAnalysis;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events;

/// <summary>
/// Represents the event arguments for when the barrier type of a B. Locker door changes in a specific region.
/// </summary>
/// <param name="regionName">The name of the region where the barrier type changed.</param>
/// <param name="oldValue">The old barrier type value.</param>
/// <param name="newValue">The new barrier type value.</param>
[method: SetsRequiredMembers]
public class BlockerBarrierTypeChangedEventArgs(RegionName regionName, BarrierItems oldValue, BarrierItems newValue) : ValueChangedEventArgs<BarrierItems>(oldValue, newValue)
{
    /// <summary>
    /// Gets the name of the region where the barrier type changed.
    /// </summary>
    public required RegionName RegionName { get; init; } = regionName;
}
