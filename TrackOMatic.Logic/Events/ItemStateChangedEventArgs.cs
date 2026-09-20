using System.Diagnostics.CodeAnalysis;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Events;

public class ItemStateChangedEventArgs : EventChangeData
{
    public required SavedItem UpdatedItem { get; init; }

    public SavedItem? PreviousState { get; init; }

    [SetsRequiredMembers]
    public ItemStateChangedEventArgs(SavedItem updatedItem, SavedItem? previousState = null, ChangeReason changeReason = ChangeReason.Unknown)
    {
        UpdatedItem = updatedItem;
        PreviousState = previousState;
        ChangeReason = changeReason;
    }
}
