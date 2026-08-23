using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Events.Hints;

/// <summary>
/// An event that is raised when the entire path items dictionary is replaced via hint dialog.
/// </summary>
public class HintPathItemsChangedEventArgs(HintId hintId, Dictionary<ItemName, bool> pathItems) : EventArgs
{
    public HintId HintId { get; private init; } = hintId;
    public Dictionary<ItemName, bool> PathItems { get; private init; } = pathItems;
}
