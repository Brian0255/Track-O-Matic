using System;
using System.Collections.Generic;
using System.Text;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events.Hints;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// A service for managing the hints found within the game.
/// </summary>
/// <remarks>All mutations to <see cref="SavedHint"/> should go through this service.</remarks>
public interface IHintService
{
    #region Events

    event EventHandler<HintCreatedEventArgs>? HintCreated;

    event EventHandler<HintUpdatedEventArgs>? HintLocationUpdated;

    event EventHandler<HintUpdatedEventArgs>? HintPotionCountUpdated;

    event EventHandler<HintPathItemsChangedEventArgs>? HintPathItemsUpdated;

    event EventHandler<HintFoundItemsChangedEventArgs>? HintFoundItemsUpdated;

    event EventHandler<HintItemToggleEventArgs>? HintPathItemToggled;

    event EventHandler<HintItemToggleEventArgs>? HintFoundItemToggled;

    event EventHandler<HintRemovedEventArgs>? HintRemoved;

    #endregion

    #region Batch Operations

    /// <summary>
    /// Begins a batch update scope where multiple hint changes are deferred.
    /// When the returned scope is disposed, the cache is refreshed to reflect all loaded hints.
    /// </summary>
    /// <remarks>This is useful when loading hints from a save file to prevent multiple UI refresh events.</remarks>
    IDisposable BeginBatchUpdate();

    #endregion

    #region Creation

    /// <summary>
    /// Creates a new hint and adds it to the collection of saved hints.
    /// </summary>
    /// <remarks>This raises the <see cref="HintCreated"/> event.</remarks>
    /// <param name="hintPanelType">The type of the hint panel.</param>
    SavedHint CreateHint(HintPanelType hintPanelType);

    #endregion

    #region Updates via HintInfo

    /// <summary>
    /// Updates the location text of a hint.
    /// </summary>
    /// <remarks>This raises the <see cref="HintLocationUpdated"/> event.</remarks>
    /// <param name="hintId">The ID of the hint to update.</param>
    /// <param name="location">The new location text.</param>
    void UpdateHintLocation(HintId hintId, string location);

    /// <summary>
    /// Updates the potion count of a hint.
    /// </summary>
    /// <remarks>
    /// 1. This does not validate the potion count: the user can put whatever they want in here.
    /// 2. This raises the <see cref="HintPotionCountUpdated"/> event.
    /// </remarks>
    /// <param name="hintId">The ID of the hint to update.</param>
    /// <param name="potionCount">The new potion count.</param>
    void UpdateHintPotionCount(HintId hintId, string potionCount);

    #endregion

    #region Path Items

    /// <summary>
    /// Replaces the entire path items dictionary for a hint.
    /// </summary>
    /// <remarks>This raises the <see cref="HintPathItemsUpdated"/> event.</remarks>
    /// <param name="hintId">The ID of the hint to update.</param>
    /// <param name="pathItems">The new path items dictionary.</param>
    void SetPathItems(HintId hintId, Dictionary<ItemName, bool> pathItems);

    /// <summary>
    /// Toggles the found status of a path item for a hint.
    /// </summary>
    /// <remarks>This raises the <see cref="HintPathItemToggled"/> event.</remarks>
    /// <param name="hintId">The ID of the hint to update.</param>
    /// <param name="itemName">The name of the item to toggle.</param>
    void TogglePathItem(HintId hintId, ItemName itemName);

    #endregion

    #region Found Items

    /// <summary>
    /// Replaces the entire found items dictionary for a hint.
    /// </summary>
    /// <remarks>
    /// 1. If the <see cref="HintPanelType"/> is a direct item panel, the item is also added to the region in the same call.
    /// 2. This raises the <see cref="HintFoundItemsUpdated"/> event.
    /// </remarks>
    /// <param name="hintId">The ID of the hint to update.</param>
    /// <param name="foundItems">The new found items dictionary.</param>
    /// <param name="hintPanelType">The type of the hint panel.</param>
    void SetFoundItems(HintId hintId, Dictionary<ItemName, bool> foundItems, HintPanelType hintPanelType);

    /// <summary>
    /// Toggles the found status of a found item for a hint.
    /// </summary>
    /// <remarks>
    /// 1. Despite the toggle method touching a Found/boolean property, this will NOT coordinate with the item tracking service.
    /// 2. This raises the <see cref="HintFoundItemToggled"/> event.
    /// </remarks>
    /// <param name="hintId"></param>
    /// <param name="itemName"></param>
    void ToggleFoundItem(HintId hintId, ItemName itemName);

    #endregion

    #region Removal

    /// <summary>
    /// Removes a hint from the collection of saved hints.
    /// </summary>
    /// <remarks>
    /// 1. If the hint comes from a direct item panel, the items involved are also removed if they weren't properly found by the user.
    /// 2. This raises the <see cref="HintRemoved"/> event.
    /// </remarks>
    /// <param name="hintId">The ID of the hint to remove.</param>
    void RemoveHint(HintId hintId);

    #endregion

    #region Queries

    /// <summary>
    /// Gets a hint by its ID.
    /// </summary>
    /// <param name="hintId">The ID of the hint to get.</param>
    /// <returns>The hint with the specified ID, or null if not found.</returns>
    SavedHint? GetHint(HintId hintId);

    /// <summary>
    /// Gets all saved hints.
    /// </summary>
    /// <returns>A read-only list of all saved hints.</returns>
    IReadOnlyList<SavedHint> GetAllHints();

    /// <summary>
    /// Load hints directly into the cache without firing <see cref="HintCreated"/> events.
    /// </summary>
    /// <remarks>
    /// <strong>Internal Use Only.</strong> This method is intended only for loading files!
    /// </remarks>
    /// <param name="hints">The hints to load.</param>
    void LoadHintsSilently(IEnumerable<SavedHint> hints);

    #endregion
}
