using TrackOMatic.Logic.Events;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// Service for managing the level order state in the tracker.
/// </summary>
public interface ILevelOrderService
{
    /// <summary>
    /// Gets the current level order as a list of integers.
    /// </summary>
    /// <returns></returns>
    IList<int> GetLevelOrder();

    /// <summary>
    /// Sets the level order to a new sequence of integers.
    /// </summary>
    /// <param name="levelOrder">The new sequence of integers representing the level order.</param>
    void SetLevelOrder(IEnumerable<int> levelOrder);

    /// <summary>
    /// Sets the level order to a new sequence of integers without raising events.
    /// </summary>
    /// <remarks>
    /// <strong>Internal use only.</strong> This should only be called via file load operations!
    /// </remarks>
    /// <param name="levelOrder">The new sequence of integers representing the level order.</param>
    void LoadLevelOrderSilently(IEnumerable<int> levelOrder);

    /// <summary>
    /// Occurs when the level order state has changed.
    /// </summary>
    event EventHandler<LevelOrderStateChangedEventArgs>? LevelOrderChanged;
}
