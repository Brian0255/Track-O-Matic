using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// Service for managing barrier constraints (B. Locker and Helm Door).
/// Wraps access to SavedProgress state for barrier counts and item types.
/// Re-initializes when SavedProgress changes (Reset or Load).
/// </summary>
public class BarrierService : IBarrierService
{
    private readonly ISavedProgressProvider _progressProvider;
    private SavedProgress _savedProgress;

    public event EventHandler<BlockerBarrierAmountChangedEventArgs>? BlockerBarrierAmountChanged;
    public event EventHandler<BlockerBarrierTypeChangedEventArgs>? BlockerBarrierTypeChanged;
    public event EventHandler<HelmBarrierAmountChangedEventArgs>? HelmBarrierAmountChanged;
    public event EventHandler<HelmBarrierTypeChangedEventArgs>? HelmBarrierTypeChanged;

    public BarrierService(ISavedProgressProvider progressProvider)
    {
        _progressProvider = progressProvider ?? throw new ArgumentNullException(nameof(progressProvider));
        _savedProgress = _progressProvider.CurrentProgress;

        // Ensure collections are initialized
        InitializeCollections();

        // Set up listener for SavedProgress changes
        _progressProvider.ProgressChanged += OnProgressChanged;
    }

    private void InitializeCollections()
    {
        _savedProgress.SavedGBCounts ??= [];
        _savedProgress.BLockerImageIndexes ??= [];
        _savedProgress.HelmDoorCounts ??= [];
        _savedProgress.HelmDoorImageIndexes ??= [];
    }

    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e)
    {
        _savedProgress = e.NewProgress;
        InitializeCollections();
    }


    // B. Locker domain
    public string GetBLockerCount(RegionName region)
    {
        if (_savedProgress.SavedGBCounts.TryGetValue(region, out var count))
        {
            return count;
        }
        return "?";
    }

    public void SetBLockerCount(RegionName region, string count)
    {
        string oldCount = GetBLockerCount(region);
        _savedProgress.SavedGBCounts[region] = count;
        BlockerBarrierAmountChanged?.Invoke(this, new(region, oldCount, count)
        {
            ChangeReason = ChangeReason.UserModified
        });
    }

    public BarrierItems GetBLockerItemType(RegionName region)
    {
        if (_savedProgress.BLockerImageIndexes.TryGetValue(region, out var index))
        {
            return (BarrierItems)index;
        }
        return BarrierItems.GOLDEN_BANANA;
    }

    public void SetBLockerItemType(RegionName region, BarrierItems itemType)
    {
        var oldItemType = GetBLockerItemType(region);
        _savedProgress.BLockerImageIndexes[region] = (int)itemType;
        BlockerBarrierTypeChanged?.Invoke(this, new(region, oldItemType, itemType)
        {
            ChangeReason = ChangeReason.UserModified
        });
    }
    // Helm Door domain
    public string GetHelmDoorCount(HelmDoor helmDoor)
    {
        int doorIndex = helmDoor == HelmDoor.First ? 0 : 1;
        if (_savedProgress.HelmDoorCounts.Count <= doorIndex)
        {
            return "?";
        }
        return _savedProgress.HelmDoorCounts[doorIndex];
    }

    public void SetHelmDoorCount(HelmDoor helmDoor, string count)
    {
        string oldCount = GetHelmDoorCount(helmDoor);
        int doorIndex = helmDoor == HelmDoor.First ? 0 : 1;
        // Ensure list is large enough
        while (_savedProgress.HelmDoorCounts.Count <= doorIndex)
        {
            _savedProgress.HelmDoorCounts.Add("?");
        }

        _savedProgress.HelmDoorCounts[doorIndex] = count;
        HelmBarrierAmountChanged?.Invoke(this, new(helmDoor, oldCount, count)
        {
            ChangeReason = ChangeReason.UserModified
        });
    }

    public BarrierItems GetHelmDoorItemType(HelmDoor helmDoor)
    {
        int doorIndex = helmDoor == HelmDoor.First ? 0 : 1;
        if (_savedProgress.HelmDoorImageIndexes.Count <= doorIndex)
        {
            return BarrierItems.GOLDEN_BANANA;
        }
        return (BarrierItems)_savedProgress.HelmDoorImageIndexes[doorIndex];
    }

    public void SetHelmDoorItemType(HelmDoor helmDoor, BarrierItems itemType)
    {
        BarrierItems oldItemType = GetHelmDoorItemType(helmDoor);
        int doorIndex = helmDoor == HelmDoor.First ? 0 : 1;
        // Ensure list is large enough
        while (_savedProgress.HelmDoorImageIndexes.Count <= doorIndex)
        {
            _savedProgress.HelmDoorImageIndexes.Add(0);
        }

        _savedProgress.HelmDoorImageIndexes[doorIndex] = (int)itemType;
        HelmBarrierTypeChanged?.Invoke(this, new(helmDoor, oldItemType, itemType)
        {
            ChangeReason = ChangeReason.UserModified
        });
    }
}
