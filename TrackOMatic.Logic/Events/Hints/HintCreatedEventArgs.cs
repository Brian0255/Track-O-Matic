using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Events.Hints;

/// <summary>
/// An event that is raised when a new hint is created and added to the collection of saved hints.
/// </summary>
/// <param name="hintId"></param>
/// <param name="hint"></param>
public class HintCreatedEventArgs(HintId hintId, SavedHint hint) : EventArgs
{
    public HintId HintId { get; private init; } = hintId;
    public SavedHint Hint { get; private init; } = hint;
}
