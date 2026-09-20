using System.Diagnostics.CodeAnalysis;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events;

/// <summary>
/// Represents the event arguments for when collectibles change, including the item type and the old and new values.
/// </summary>
/// <param name="itemType">The type of collectible item that changed.</param>
/// <param name="oldValue">The old value of the collectible item.</param>
/// <param name="newValue">The new value of the collectible item.</param>
[method: SetsRequiredMembers]
public class CollectiblesChangedEventArgs(ItemType itemType, int oldValue, int newValue) : ValueChangedEventArgs<int>(oldValue, newValue)
{
    public required ItemType ItemType { get; init; } = itemType;
}
