using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// Implementation of ICollectiblesService that manages collectible state
/// through the shared SavedProgress model.
/// </summary>
public class CollectiblesService(ISavedProgressProvider progressProvider) : ICollectiblesService
{
    private readonly ISavedProgressProvider _progressProvider = progressProvider ?? throw new ArgumentNullException(nameof(progressProvider));

    public event EventHandler<CollectiblesChangedEventArgs>? CollectibleChanged;

    public int GetCollectibleCount(ItemType itemType)
    {
        var currentProgress = _progressProvider.CurrentProgress;
        if (currentProgress.Collectibles.TryGetValue(itemType, out var count))
        {
            return Math.Max(count, 0);
        }
        return 0;
    }

    public void IncrementCollectible(ItemType itemType)
    {
        var currentCount = GetCollectibleCount(itemType);
        SetCollectibleCount(itemType, currentCount + 1);
    }

    public void DecrementCollectible(ItemType itemType)
    {
        var currentCount = GetCollectibleCount(itemType);
        SetCollectibleCount(itemType, Math.Max(currentCount - 1, 0));
    }

    public void SetCollectibleCount(ItemType itemType, int count)
    {
        var currentProgress = _progressProvider.CurrentProgress;
        var oldValue = GetCollectibleCount(itemType);
        var newValue = Math.Max(count, 0);

        if (oldValue != newValue)
        {
            currentProgress.Collectibles[itemType] = newValue;
            CollectibleChanged?.Invoke(this, new(itemType, oldValue, newValue));
        }
    }
}
