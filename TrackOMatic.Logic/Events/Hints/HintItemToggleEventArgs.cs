using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Events.Hints;

/// <summary>
/// An event that is raised when a hint item is toggled on the hint panel.
/// </summary>
/// <remarks>This event should work for both found and pathed item lists.</remarks>
public class HintItemToggleEventArgs(HintId hintId, ItemName itemName, bool isFound) : EventArgs
{
    public HintId HintId { get; private init; } = hintId;
    public ItemName ItemName { get; private init; } = itemName;
    public bool IsFound { get; private init; } = isFound;
}
