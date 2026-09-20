using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// Manages item collection state across all regions.
/// </summary>
public interface IItemTrackingService
{
    /// <summary>
    /// Get the tracking state for a specific item.
    /// </summary>
    SavedItem? GetItemState(ItemName itemName);

    /// <summary>
    /// Update tracking state for an item (replaces entire entry) with a change reason.
    /// </summary>
    /// <param name="itemName">The item to update</param>
    /// <param name="state">The new item state</param>
    /// <param name="changeReason">The reason for this change (e.g., for filtering or logging)</param>
    void SetItemState(ItemName itemName, SavedItem state, ChangeReason changeReason = ChangeReason.UserModified);

    /// <summary>
    /// Toggles the star visibility state of an item.
    /// </summary>
    /// <param name="itemName">The item to update.</param>
    void ToggleStar(ItemName itemName);

    /// <summary>
    /// Removes the item that was directly hinted and placed by the program.
    /// </summary>
    /// <param name="itemName">The item to remove.</param>
    void RemoveDirectHintedItem(ItemName itemName);

    /// <summary>
    /// Load items direclty into the tracking cache without firing <see cref="ItemStateChanged"/> events.
    /// </summary>
    /// <remarks>
    /// <strong>Internal Use Only.</strong> This method is intended exclusively for loading from a file!
    /// </remarks>
    /// <param name="items">The items to load into the cache.</param>
    void LoadItemsSilently(Dictionary<ItemName, SavedItem> items);

    /// <summary>
    /// Remove tracking state for an item.
    /// </summary>
    /// <param name="itemName">The item to clear.</param>
    void ClearItemState(ItemName itemName);

    /// <summary>
    /// Get all items in a specific region.
    /// </summary>
    IEnumerable<SavedItem> GetItemsInRegion(RegionName region);

    /// <summary>
    /// Get all items in the order they were added, preserving the linked list order.
    /// </summary>
    /// <remarks>If a user removes an item and then adds it back, that item goes to the end of the list.</remarks>
    IEnumerable<SavedItem> GetAllItemsInOrder();

    /// <summary>
    /// Rebuilds the dictionary of <see cref="SavedItem">SavedItems</see> to match the new linked list order.
    /// </summary>
    /// <remarks>This should be called before saving to preserve the order of items found.</remarks>
    void SyncSavedItemsOrder();

    /// <summary>
    /// Fired when any item state changes.
    /// </summary>
    event EventHandler<ItemStateChangedEventArgs>? ItemStateChanged;

    /// <summary>
    /// Begins a batch update scope where multiple item changes are deferred.
    /// When the returned scope is disposed, all collected changes fire ItemStateChanged events.
    /// Use with a 'using' statement for automatic cleanup.
    /// </summary>
    /// <example>
    /// using (itemService.BeginBatchUpdate())
    /// {
    ///     itemService.SetItemState(item1, state1);
    ///     itemService.SetItemState(item2, state2);
    ///     itemService.ClearItemState(item3);
    /// } // Events fire here
    /// </example>
    IDisposable BeginBatchUpdate();
}
