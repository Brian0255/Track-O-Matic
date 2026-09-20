using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Events.Hints;

/// <summary>
/// An event that is raised when the found items for a hint are changed.
/// </summary>
public class HintFoundItemsChangedEventArgs(HintId hintId, Dictionary<ItemName, bool> foundItems, HintPanelType hintPanelType) : EventArgs
{
    public HintId HintId { get; private init; } = hintId;
    public Dictionary<ItemName, bool> FoundItems { get; private init; } = foundItems;
    public HintPanelType HintPanelType { get; private init; } = hintPanelType;
}
