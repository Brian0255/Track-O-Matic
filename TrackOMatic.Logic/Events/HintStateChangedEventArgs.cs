using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Events;

public class HintStateChangedEventArgs : EventChangeData
{
    /// <summary>
    /// Gets the updated hint that triggered the event.
    /// </summary>
    /// <remarks>This property contains the panel the hint is in, theoretically simplifying the arguments.</remarks>
    public required SavedHint UpdatedHint { get; init; }

    public SavedHint? PreviousState { get; init; }

    public HintStateChangedEventArgs(SavedHint updatedHint, SavedHint? previousState = null, ChangeReason changeReason = ChangeReason.Unknown)
    {
        UpdatedHint = updatedHint;
        PreviousState = previousState;
        ChangeReason = changeReason;
    }
}
