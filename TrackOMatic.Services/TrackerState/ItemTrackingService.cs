using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// Represents a deferred item state change during batch operations.
/// </summary>
internal record DeferredItemChange(ItemName ItemName, SavedItem? CurrentState, SavedItem? PreviousState);

/// <summary>
/// Service for managing item tracking state across regions.
/// Backed by SavedProgress and fires events when item state changes.
/// </summary>
public class ItemTrackingService : IItemTrackingService
{
    private readonly ISavedProgressProvider _progressProvider;
    private bool _isBatchingUpdates;
    private readonly List<DeferredItemChange> _deferredUpdates;

    /// <summary>
    /// A list that maintains the order of items as they are added to the cache.
    /// </summary>
    private readonly LinkedList<SavedItem> _itemOrder;
    /// <summary>
    /// A mapping from ItemName to the corresponding LinkedListNode in _itemOrder for quick access and reordering.
    /// </summary>
    private readonly Dictionary<ItemName, LinkedListNode<SavedItem>> _nodeMap;

    public event EventHandler<ItemStateChangedEventArgs>? ItemStateChanged;

    private readonly SynchronizationContext? _uiContext;

    public ItemTrackingService(ISavedProgressProvider progressProvider)
    {
        _progressProvider = progressProvider ?? throw new ArgumentNullException(nameof(progressProvider));

        // Capture the UI synchronization context at service creation time.
        // If created on the UI thread, this will be the UI sync context.
        // If created on a background thread, this will be null (and we'll use beacons to the current context).
        _uiContext = SynchronizationContext.Current;

        _itemOrder = new();
        _nodeMap = [];
        _isBatchingUpdates = false;
        _deferredUpdates = [];
        InitializeCache();

        // Subscribe to progress changes (Reset/Load operations)
        _progressProvider.ProgressChanged += OnProgressChanged;
    }

    private void InitializeCache()
    {
        _itemOrder.Clear();
        _nodeMap.Clear();
        foreach (var item in _progressProvider.CurrentProgress.SavedItems)
        {
            var cleanItem = item.Value with
            {
                ItemName = item.Key // Ensure the ItemName is consistent with the dictionary string key
            };
            var node = _itemOrder.AddLast(cleanItem);
            _nodeMap[cleanItem.ItemName] = node;
        }
    }

    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e)
    {
        // When SavedProgress is replaced (Reset/Load), reinitialize cache
        InitializeCache();
    }

    /// <summary>
    /// Safely invokes the ItemStateChanged event on the UI thread (or sync context).
    /// If called from a background thread, marshals the event via SynchronizationContext.
    /// If called from the UI thread (or if no UI context available), invokes immediately.
    /// Uses async-friendly Post pattern suitable for modern .NET.
    /// </summary>
    private void RaiseItemStateChanged(SavedItem currentState, SavedItem? previousState, ChangeReason reason)
    {
        if (ItemStateChanged == null)
        {
            return;
        }

        // If we have a UI context and we're not on it, marshal asynchronously
        if (_uiContext != null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ =>
            {
                ItemStateChanged?.Invoke(this, new(currentState, previousState, reason));
            }, null);
        }
        else
        {
            // Already on the UI context (or no captured context), invoke directly
            ItemStateChanged?.Invoke(this, new(currentState, previousState, reason));
        }
    }

    public SavedItem? GetItemState(ItemName itemName)
    {
        return _nodeMap.TryGetValue(itemName, out var node) ? node.Value : null;
    }

    public void SetItemState(ItemName itemName, SavedItem state)
    {
        SetItemState(itemName, state, ChangeReason.UserModified);
    }

    public void SetItemState(ItemName itemName, SavedItem state, ChangeReason changeReason)
    {
        if (state.ItemName != itemName)
        {
            throw new ArgumentException($"SavedItem.ItemName ({state.ItemName}) does not match itemName parameter ({itemName})", nameof(state));
        }

        // If the new state is empty (no region, not starred, not hinted), clear it instead of storing
        if (state.IsEmpty)
        {
            ClearItemState(itemName);
            return;
        }

        SavedItem? previousState = null;

        // See if the item already exists.
        if (_nodeMap.TryGetValue(itemName, out var existingNode))
        {
            previousState = existingNode.Value;

            // Immediately abandon if this item is already pre-placed.
            if (previousState.IsStartingAutotracked)
            {
                return;
            }

            // Update the existing node's value to the new state
            existingNode.Value = state;
        }
        else
        {
            // We have a new item, so add to the end of the list.
            var newNode = _itemOrder.AddLast(state);
            _nodeMap[itemName] = newNode;
        }

        // Update SavedProgress (for serialization purposes)
        _progressProvider.CurrentProgress.SavedItems[itemName] = state;

        // If batching, defer the event; otherwise fire immediately
        if (_isBatchingUpdates)
        {
            _deferredUpdates.Add(new DeferredItemChange(itemName, state, previousState));
        }
        else
        {
            RaiseItemStateChanged(state, previousState, changeReason);
        }
    }

    public void ClearItemState(ItemName itemName)
    {
        if (!_nodeMap.TryGetValue(itemName, out var node))
        {
            return;
        }

        SavedItem removedState = node.Value;
        _itemOrder.Remove(node);
        _nodeMap.Remove(itemName);

        // Remove from SavedProgress
        _progressProvider.CurrentProgress.SavedItems.Remove(itemName);

        // If batching, defer the event; otherwise fire immediately
        if (_isBatchingUpdates)
        {
            if (removedState != null)
            {
                _deferredUpdates.Add(new DeferredItemChange(itemName, null, removedState));
            }
        }
        else
        {
            // Fire event to notify subscribers that the item was removed
            // Broadcast controls need to know to update their display
            if (removedState != null)
            {
                RaiseItemStateChanged(removedState with
                {
                    Region = RegionName.UNKNOWN,
                    Opacity = 1,
                    Starred = false,
                }, removedState, ChangeReason.UserModified);
            }
        }
    }

    public IEnumerable<SavedItem> GetItemsInRegion(RegionName region)
    {
        var snapshot = _itemOrder.ToList();
        var result = snapshot.Where(item => item is not null && item.Region == region).ToList();
        return result;
    }

    public IEnumerable<SavedItem> GetAllItemsInOrder()
    {
        return _itemOrder.ToList().AsReadOnly();
    }

    /// <summary>
    /// Toggles the star visibility state of an item.
    /// If no SavedItem exists for the item, creates one with the star toggled to Visible.
    /// This method centralizes star toggle logic to ensure consistent behavior across all view models.
    /// </summary>
    /// <param name="itemName">The name of the item whose star should be toggled.</param>
    public void ToggleStar(ItemName itemName)
    {
        var currentItem = GetItemState(itemName) ?? SavedItem.CreateEmpty(itemName);

        var newStarred = !currentItem.Starred;

        SavedItem updatedItem = new(currentItem.ItemName, currentItem.Region, newStarred, currentItem.Autotracked, currentItem.Opacity, currentItem.Hinted);
        SetItemState(itemName, updatedItem);
    }

    public void SyncSavedItemsOrder()
    {
        var oldItems = _progressProvider.CurrentProgress.SavedItems;
        _progressProvider.CurrentProgress.SavedItems = [];

        foreach (var item in _itemOrder)
        {
            _progressProvider.CurrentProgress.SavedItems[item.ItemName] = item;
        }
    }

    /// <summary>
    /// Begins a batch update scope where multiple item changes are deferred.
    /// When the returned scope is disposed, all collected changes fire a single ItemStateChanged event.
    /// </summary>
    public IDisposable BeginBatchUpdate()
    {
        return new BatchUpdateScope(this);
    }

    /// <summary>
    /// Private helper class that manages the batch update scope lifecycle.
    /// </summary>
    private class BatchUpdateScope : IDisposable
    {
        private readonly ItemTrackingService _service;
        private bool _disposed;

        public BatchUpdateScope(ItemTrackingService service)
        {
            _service = service;
            _service._isBatchingUpdates = true;
            _service._deferredUpdates.Clear();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            // Exit batch mode and process all deferred updates
            _service._isBatchingUpdates = false;
            var deferredUpdates = _service._deferredUpdates.ToList();
            _service._deferredUpdates.Clear();

            // Fire a composite event for all collected changes
            if (deferredUpdates.Count > 0)
            {
                foreach (var change in deferredUpdates)
                {
                    // For each deferred change, fire an event
                    // Note: CurrentState is null only for cleared items; PreviousState is always present
                    // because we only defer changes that actually occurred.
                    var itemToReport = change.CurrentState ?? change.PreviousState! with
                    {
                        Region = RegionName.UNKNOWN,
                        Opacity = 1,
                        Starred = false,
                    };
                    ArgumentNullException.ThrowIfNull(itemToReport, nameof(itemToReport));

                    _service.RaiseItemStateChanged(itemToReport, change.PreviousState, ChangeReason.Batched);
                }
            }
        }
    }
}
