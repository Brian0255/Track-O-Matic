using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// Service for managing collectible item counts (blueprints, pearls, crowns, etc.).
/// Provides centralized access to collectible state and raises events on changes.
/// </summary>
public interface ICollectiblesService
{
    /// <summary>
    /// Raised when a collectible value changes.
    /// </summary>
    event EventHandler<CollectiblesChangedEventArgs>? CollectibleChanged;

    /// <summary>
    /// Gets the current count for a specific collectible item type.
    /// </summary>
    /// <param name="itemType">The type of collectible to retrieve.</param>
    /// <returns>The count, or 0 if not found.</returns>
    int GetCollectibleCount(ItemType itemType);

    /// <summary>
    /// Increments a collectible count by one.
    /// </summary>
    /// <param name="itemType">The type of collectible to increment.</param>
    void IncrementCollectible(ItemType itemType);

    /// <summary>
    /// Decrements a collectible count by one (minimum 0).
    /// </summary>
    /// <param name="itemType">The type of collectible to decrement.</param>
    void DecrementCollectible(ItemType itemType);

    /// <summary>
    /// Sets a collectible count to a specific value.
    /// </summary>
    /// <param name="itemType">The type of collectible to set.</param>
    /// <param name="count">The new count (will be clamped to minimum 0).</param>
    void SetCollectibleCount(ItemType itemType, int count);
}
