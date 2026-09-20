using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Events.Hints;

/// <summary>
/// An event that is raised when hint properties such as the location and potion count are updated.
/// </summary>
public class HintUpdatedEventArgs(HintId hintId, SavedHint updatedHint) : EventArgs
{
    public HintId HintId { get; private init; } = hintId;
    public SavedHint UpdatedHint { get; private init; } = updatedHint;
}
