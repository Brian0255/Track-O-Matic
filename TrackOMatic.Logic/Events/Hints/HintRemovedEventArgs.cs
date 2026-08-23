using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Events.Hints;

/// <summary>
/// An event that is raised when a hint is removed by the user.
/// </summary>
/// <remarks>This should not necessarily fire if a load operation took place.</remarks>
public class HintRemovedEventArgs(HintId hintId, SavedHint removedHint) : EventArgs
{
    public HintId HintId { get; private init; } = hintId;

    public SavedHint RemovedHint { get; private init; } = removedHint;
}
