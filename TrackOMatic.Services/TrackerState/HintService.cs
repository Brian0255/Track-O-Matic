using System;
using System.Collections.Generic;
using System.Text;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Events.Hints;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// A service for managing the hints found within the game.
/// </summary>
public class HintService : IHintService
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly ISavedProgressProvider _progressProvider;
    private readonly List<SavedHint> _hintCache;
    private readonly SynchronizationContext _uiContext;
    private bool _isBatchingUpdates;

    #region IHintService Events

    public event EventHandler<HintCreatedEventArgs>? HintCreated;
    public event EventHandler<HintUpdatedEventArgs>? HintLocationUpdated;
    public event EventHandler<HintUpdatedEventArgs>? HintPotionCountUpdated;
    public event EventHandler<HintPathItemsChangedEventArgs>? HintPathItemsUpdated;
    public event EventHandler<HintFoundItemsChangedEventArgs>? HintFoundItemsUpdated;
    public event EventHandler<HintItemToggleEventArgs>? HintPathItemToggled;
    public event EventHandler<HintItemToggleEventArgs>? HintFoundItemToggled;
    public event EventHandler<HintRemovedEventArgs>? HintRemoved;

    #endregion

    public HintService(IItemTrackingService itemTrackingService, ISavedProgressProvider progressProvider)
    {
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _progressProvider = progressProvider ?? throw new ArgumentNullException(nameof(progressProvider));
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("HintService must be created on the UI thread.");
        _hintCache = [];

        _progressProvider.ProgressChanged += OnProgressChanged;
        InitializeCache();
    }

    #region IHintService Method Implementation

    public SavedHint CreateHint(HintPanelType hintPanelType)
    {
        var hint = new SavedHint(hintPanelType, "", "", [], []);

        _progressProvider.CurrentProgress.SavedHints.Add(hint);
        _hintCache.Add(hint);
        HintCreated?.Invoke(this, new(hint.HintId, hint));
        return hint;
    }

    public IReadOnlyList<SavedHint> GetAllHints()
    {
        return _hintCache.AsReadOnly();
    }

    public SavedHint? GetHint(HintId hintId)
    {
        return _hintCache.FirstOrDefault(h => h.HintId == hintId);
    }

    public void RemoveHint(HintId hintId)
    {
        var oldHint = GetHint(hintId);
        if (oldHint == null)
        {
            return;
        }

        // Remove any items associated with this hint.
        var foundItems = oldHint.FoundItems.Select(kvp => kvp.Key);

        using var scope = _itemTrackingService.BeginBatchUpdate();
        foreach (var item in foundItems)
        {
            var savedItem = _itemTrackingService.GetItemState(item);
            if (savedItem is not null && savedItem.Hinted)
            {
                _itemTrackingService.ClearItemState(item);
            }
        }
        _hintCache.Remove(oldHint);
        _progressProvider.CurrentProgress.SavedHints.Remove(oldHint);
        HintRemoved?.Invoke(this, new HintRemovedEventArgs(hintId, oldHint));
    }

    public void SetFoundItems(HintId hintId, Dictionary<ItemName, bool> foundItems, HintPanelType hintPanelType)
    {
        var hint = GetHint(hintId) ?? throw new ArgumentException($"Hint with ID {hintId} does not exist.", nameof(hintId));

        var hintedRegion = hintPanelType.ToRegion();
        if (hintedRegion.IsPlayableRegion())
        {
            using var scope = _itemTrackingService.BeginBatchUpdate();

            foreach (var item in foundItems)
            {
                var itemState = _itemTrackingService.GetItemState(item.Key) ?? SavedItem.CreateEmpty(item.Key);

                if (!itemState.Region.IsPlayableRegion())
                {
                    _itemTrackingService.SetItemState(item.Key, itemState with
                    {
                        Hinted = true,
                        Region = hintedRegion
                    });
                }
            }
        }

        hint.FoundItems = foundItems;
        HintFoundItemsUpdated?.Invoke(this, new(hintId, foundItems, hintPanelType));
    }

    public void SetPathItems(HintId hintId, Dictionary<ItemName, bool> pathItems)
    {
        var hint = GetHint(hintId) ?? throw new ArgumentException($"Hint with ID {hintId} does not exist.", nameof(hintId));

        hint.PathItems = pathItems;
        HintPathItemsUpdated?.Invoke(this, new(hintId, pathItems));
    }

    public void ToggleFoundItem(HintId hintId, ItemName itemName)
    {
        var hint = GetHint(hintId) ?? throw new ArgumentException($"Hint with ID {hintId} does not exist.", nameof(hintId));

        if (hint.FoundItems.TryGetValue(itemName, out bool value))
        {
            hint.FoundItems[itemName] = !value;
            HintFoundItemToggled?.Invoke(this, new(hintId, itemName, !value));
        }
    }

    public void TogglePathItem(HintId hintId, ItemName itemName)
    {
        var hint = GetHint(hintId) ?? throw new ArgumentException($"Hint with ID {hintId} does not exist.", nameof(hintId));

        if (hint.PathItems.TryGetValue(itemName, out bool value))
        {
            hint.PathItems[itemName] = !value;
            HintPathItemToggled?.Invoke(this, new(hintId, itemName, !value));
        }
    }

    public void UpdateHintLocation(HintId hintId, string location)
    {
        var hint = GetHint(hintId) ?? throw new ArgumentException($"Hint with ID {hintId} does not exist.", nameof(hintId));

        hint.LocationText = location;
        HintLocationUpdated?.Invoke(this, new(hintId, hint));
    }

    public void UpdateHintPotionCount(HintId hintId, string potionCount)
    {
        var hint = GetHint(hintId) ?? throw new ArgumentException($"Hint with ID {hintId} does not exist.", nameof(hintId));

        hint.PotionCountText = potionCount;
        HintPotionCountUpdated?.Invoke(this, new(hintId, hint));
    }

    #endregion

    /// <summary>
    /// Begins a batch update scope where the cache refresh is deferred.
    /// When the returned scope is disposed, the cache is refreshed to reflect all loaded hints.
    /// </summary>
    public IDisposable BeginBatchUpdate()
    {
        return new BatchUpdateScope(this);
    }

    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e)
    {
        if (!_isBatchingUpdates)
        {
            InitializeCache();
        }
    }

    private void InitializeCache()
    {
        _hintCache.Clear();
        foreach (var hint in _progressProvider.CurrentProgress.SavedHints)
        {
            _hintCache.Add(hint);
        }
    }

    /// <summary>
    /// Private helper class that manages the batch update scope lifecycle.
    /// </summary>
    private class BatchUpdateScope : IDisposable
    {
        private readonly HintService _service;
        private bool _disposed;

        public BatchUpdateScope(HintService service)
        {
            _service = service;
            _service._isBatchingUpdates = true;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            // Exit batch mode and refresh the cache with all loaded hints
            _service._isBatchingUpdates = false;
            _service.InitializeCache();
        }
    }
}
