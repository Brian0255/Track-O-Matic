using System.Diagnostics.CodeAnalysis;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events;

/// <summary>
/// Represents the event arguments for when the barrier type of a Helm door changes in a specific region.
/// </summary>
/// <param name="helmDoor">The Helm door associated with the barrier type change.</param>
/// <param name="oldValue">The old barrier type value.</param>
/// <param name="newValue">The new barrier type value.</param>
[method: SetsRequiredMembers]
public class HelmBarrierTypeChangedEventArgs(HelmDoor helmDoor, BarrierItems oldValue, BarrierItems newValue) : ValueChangedEventArgs<BarrierItems>(oldValue, newValue)
{
    /// <summary>
    /// Gets the Helm door associated with the barrier type change.
    /// </summary>
    public required HelmDoor HelmDoor { get; init; } = helmDoor;
}
