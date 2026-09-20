using System.Diagnostics.CodeAnalysis;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events;

/// <summary>
/// Represents the event arguments for when the amount of a Helm door changes in a specific region.
/// </summary>
/// <param name="helmDoor">The Helm door associated with the barrier constraint change.</param>
/// <param name="oldValue">The old value of the barrier constraint.</param>
/// <param name="newValue">The new value of the barrier constraint.</param>
[method: SetsRequiredMembers]
public class HelmBarrierAmountChangedEventArgs(HelmDoor helmDoor, string oldValue, string newValue) : ValueChangedEventArgs<string>(oldValue, newValue)
{
    /// <summary>
    /// Gets the Helm door associated with the barrier constraint change.
    /// </summary>
    public required HelmDoor HelmDoor { get; init; } = helmDoor;
}
