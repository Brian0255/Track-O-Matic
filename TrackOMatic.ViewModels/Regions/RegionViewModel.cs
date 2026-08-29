using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;


[assembly: InternalsVisibleTo("TrackOMatic.ViewModels.Test")]

namespace TrackOMatic.ViewModels.Regions;

public partial class RegionViewModel : ObservableObject, IEquatable<RegionViewModel>, IDisposable, IRegionSlotProvider
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IThemeService _themeService;
    private readonly RegionName _regionName;
    private readonly IRegionPlacementOrchestrator _regionPlacementOrchestrator;
    private readonly RegionSlotProviderRegistry? _slotProviderRegistry;
    private readonly SynchronizationContext _syncContext;
    private CancellationTokenSource? _refreshDebounceToken = null;


    public RegionViewModel(
        RegionName regionName,
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IRegionPlacementOrchestrator regionPlacementOrchestrator,
        IThemeService themeService,
        RegionSlotProviderRegistry? slotProviderRegistry = null
    )
    {
        _regionName = regionName;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _regionPlacementOrchestrator = regionPlacementOrchestrator ?? throw new ArgumentNullException(nameof(regionPlacementOrchestrator));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));

        // Capture the synchronization context for UI thread marshaling in event handlers
        // In tests (no UI thread), this will be null and we'll run directly
        _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();

        // Registry is optional for backwards compatibility with existing tests/code
        _slotProviderRegistry = slotProviderRegistry;

        // Register this region's vial slots with the registry (if available)
        if (_slotProviderRegistry != null)
        {
            _slotProviderRegistry.RegisterRegionProvider(_regionName, this);
        }

        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnProgressChanged;

        // Initialize state directly (not debounced) in constructor
        InitializeState();

        // Eagerly initialize RegionLevelViewModel
        var levelOrderService = ServiceLocator.GetService<ILevelOrderService>() ?? throw new InvalidOperationException($"{nameof(ILevelOrderService)} not found in service locator.");
        var userSettingsService = ServiceLocator.GetService<IUserSettingsService>() ?? throw new InvalidOperationException($"{nameof(IUserSettingsService)} not found in service locator.");

        _regionLevelViewModel = new RegionLevelViewModel(
            _regionName,
            levelOrderService,
            userSettingsService,
            _parsedSpoilerDataService,
            savedProgressProvider
        );
    }

    // RegionLevelViewModel for display
    private readonly RegionLevelViewModel _regionLevelViewModel;
    public RegionLevelViewModel RegionLevelViewModel => _regionLevelViewModel;

    private void InitializeState()
    {
        RefreshPlacedItems();
        UpdateHoardText();
        UpdateItemPointText();
        UpdateRegionResourceKey();
    }

    private void RefreshPlacedItems()
    {
        // Unsubscribe from PropertyChanged events on old vials before clearing
        foreach (var item in PlacedItems.OfType<VialItemViewModel>())
        {
            item.PropertyChanged -= OnPlacedVialPropertyChanged;
        }

        PlacedItems.Clear();

        // Special handling for START region - it uses StartingItems from spoiler data, not RegionData vials
        if (_regionName == RegionName.START && _parsedSpoilerDataService.CurrentData?.StartingItems.Count > 0)
        {
            // Create vial slots for starting items (excluding UNHINTABLE_MOVES)
            var startingItemsForDisplay = _parsedSpoilerDataService.CurrentData.StartingItems
                            .Where(kvp => kvp.Value == RegionName.START)
                            .ToList();

            for (int slotIndex = 0; slotIndex < startingItemsForDisplay.Count; slotIndex++)
            {
                var itemName = startingItemsForDisplay[slotIndex].Key;
                var vialColor = itemName.ToVialColor();

                var vial = new VialItemViewModel(
                    itemName, // Fixed item name for START slots
                    RegionName.START,
                    vialColor,
                    _itemTrackingService,
                    _parsedSpoilerDataService,
                    _themeService);

                vial.PropertyChanged += OnPlacedVialPropertyChanged;
                PlacedItems.Add(vial);
            }
        }
        else
        {
            var spoilerData = _parsedSpoilerDataService.CurrentData;
            var regionData = spoilerData?.RegionData.GetValueOrDefault(_regionName);
            if (regionData != null && spoilerData?.SpoilerSettings?.VialsEnabled == true)
            {
                for (int slotIndex = 0; slotIndex < regionData.VialColors.Count; slotIndex++)
                {
                    var vialColor = regionData.VialColors[slotIndex];
                    var vial = new VialItemViewModel(
                        null,
                        _regionName,
                        vialColor,
                        _itemTrackingService,
                        _parsedSpoilerDataService,
                        _themeService);

                    vial.PropertyChanged += OnPlacedVialPropertyChanged;
                    PlacedItems.Add(vial);
                }
            }

            else
            {
                // Materialize the enumerable before iterating
                var itemsInRegion = _itemTrackingService.GetItemsInRegion(_regionName).ToList();
                foreach (var item in itemsInRegion)
                {
                    PlacedItems.Add(new RegionItemViewModel(item.ItemName, _regionName, _itemTrackingService, _parsedSpoilerDataService, _themeService));
                }
            }
        }

        // Notify UI bindings that PlacedItems collection content has changed
        OnPropertyChanged(nameof(PlacedItems));
    }

    /// <summary>
    /// Determines if this region has vials enabled (spoiler mode with vials).
    /// </summary>
    private bool HasVials => _parsedSpoilerDataService.GetSpoilerSettings()?.VialsEnabled ?? false;

    /// <summary>
    /// Attempts to drop an item into this region.
    /// </summary>
    public bool TryAcceptDrop(ItemName itemToPlace, MouseDragType dragType)
    {
        if (HasVials)
        {
            // In vial enabled mode: find the first empty slot that matches the vial color
            var targetColor = itemToPlace.ToVialColor();
            var emptySlot = PlacedItems.FirstOrDefault(slot =>
            slot is VialItemViewModel spoilerSlot &&
            spoilerSlot.VialColor == targetColor &&
            !spoilerSlot.CurrentItemName.HasValue);

            if (emptySlot == null)
            {
                return false;
            }

            // Check if the slot can accept the drop
            if (!emptySlot.CanAcceptDrop(itemToPlace, dragType))
            {
                return false;
            }

            // Actually mutate the slot state - this updates CurrentItemName and fires UpdateProperties
            return emptySlot.AcceptDropAndMutate(itemToPlace, dragType);
        }
        else
        {
            // In no-spoiler mode: always accept the drop
            // The tracking service will handle the actual state change (replace or add) via UiItemViewModel.CompleteDrag
            return true;
        }
    }

    /// <summary>
    /// Gets all vial slots currently in this region.
    /// Called by the orchestrator when attempting autotracked item placement.
    /// </summary>
    public IEnumerable<IVialSlot> GetVialSlots()
    {
        return PlacedItems.OfType<IVialSlot>();
    }

    /// <summary>
    /// Explicit implementation of IRegionSlotProvider interface.
    /// Delegates to GetVialSlots() for this region.
    /// </summary>
    IEnumerable<IVialSlot> IRegionSlotProvider.GetVialSlotsForRegion(RegionName regionName)
    {
        // Only return slots if the queried region matches this region
        if (regionName == _regionName)
        {
            return GetVialSlots();
        }
        return Enumerable.Empty<IVialSlot>();
    }

    #region Proxy Properties

    // Placed items in the grid
    private ObservableCollection<IRegionItemViewModel> _placedItems = [];
    public ObservableCollection<IRegionItemViewModel> PlacedItems
    {
        get => _placedItems;
        private set => SetProperty(ref _placedItems, value);
    }

    private string _regionResourceKey = "";
    public string RegionResourceKey
    {
        get => _regionResourceKey;
        internal set => SetProperty(ref _regionResourceKey, value);
    }

    private string _backgroundResourceKey = "RegionBG";
    public string BackgroundResourceKey
    {
        get => _backgroundResourceKey;
        private set => SetProperty(ref _backgroundResourceKey, value);
    }

    private bool _hasWothPoints = false;
    public bool HasWothPoints
    {
        get => _hasWothPoints;
        private set => SetProperty(ref _hasWothPoints, value);
    }

    private int _wothPoints = -1;
    public int WothPoints
    {
        get => _wothPoints;
        internal set => SetProperty(ref _wothPoints, value);
    }

    private bool _hasItemPoints = false;
    public bool HasItemPoints
    {
        get => _hasItemPoints;
        private set => SetProperty(ref _hasItemPoints, value);
    }

    private int _itemPoints = -1;
    public int ItemPoints
    {
        get => _itemPoints;
        private set => SetProperty(ref _itemPoints, value);
    }

    public bool ShouldShowItemPoints
    {
        get => _regionName != RegionName.START && HasItemPoints;
    }

    public bool ShouldShowRegionLevel
    {
        get => _regionName != RegionName.START && _regionName != RegionName.DK_ISLES;
    }

    #endregion

    #region IDisposable

    private bool _disposed = false;

    // Cleanup
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Cancel any pending debounced refresh
                _refreshDebounceToken?.Cancel();
                _refreshDebounceToken?.Dispose();
                _refreshDebounceToken = null;

                // Unregister from slot provider registry
                _slotProviderRegistry?.UnregisterRegionProvider(_regionName);

                // Unsubscribe from events on placed vials
                foreach (var item in PlacedItems.OfType<VialItemViewModel>())
                {
                    item.PropertyChanged -= OnPlacedVialPropertyChanged;
                }

                // Unsubscribe from service events
                _itemTrackingService.ItemStateChanged -= OnItemStateChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnParsedSpoilerDataChanged;
                _savedProgressProvider.ProgressChanged -= OnProgressChanged;
            }
            _disposed = true;
        }
    }

    ~RegionViewModel()
    {
        Dispose(false);
    }

    #endregion

    private void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        // Only process if this item was in our region previously or is in our region now
        bool wasInThisRegion = e.PreviousState?.Region == _regionName;
        bool isInThisRegion = e.UpdatedItem.Region == _regionName;

        if (!wasInThisRegion && !isInThisRegion)
        {
            // Item change is not relevant to this region
            return;
        }

        // Only process certain change reasons to avoid stale state assumptions
        bool isRelevantChange = e.ChangeReason == ChangeReason.UserModified
                    || e.ChangeReason == ChangeReason.AutoTracked;
        //|| e.ChangeReason == "ItemMovedToUnknown"
        //|| e.ChangeReason == ChangeReason. "ItemCleared";

        if (!isRelevantChange)
        {
            return;
        }

        // In vial enabled mode, update hoard text based on starred items
        if (HasVials)
        {
            UpdateHoardText(e.UpdatedItem);
        }
        else
        {
            // In non-vial mode, debounce the refresh to batch multiple rapid events
            DebounceRefresh();
        }
    }

    private void OnParsedSpoilerDataChanged(object? sender, EventArgs e) => DebounceRefresh();

    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e) => DebounceRefresh();

    /// <summary>
    /// Debounce refresh requests: if multiple events fire rapidly, batch them into a single refresh.
    /// This prevents re-entrancy issues when autotracker places many items at once.
    /// Only applies debouncing to event handlers, not constructor initialization.
    /// </summary>
    private void DebounceRefresh()
    {
        // Cancel the previous debounce timer if it exists
        _refreshDebounceToken?.Cancel();
        _refreshDebounceToken = new CancellationTokenSource();

        // Schedule refresh after a short delay (50ms), allowing events to batch
        var token = _refreshDebounceToken.Token;
        Task.Delay(50, token).ContinueWith(_ =>
        {
            if (!token.IsCancellationRequested)
            {
                // In tests (SynchronizationContext.Current == null when constructed),
                // _syncContext will be a default SynchronizationContext() with no message pump.
                // Detect this and run directly instead of posting, to avoid deadlock.
                if (SynchronizationContext.Current == _syncContext)
                {
                    // We're on the same context already - run directly
                    InitializeState();
                }
                else
                {
                    // Try to post to the captured context (UI thread in production, or test default context)
                    _syncContext.Post(_ =>
                    {
                        if (!token.IsCancellationRequested)
                        {
                            InitializeState();
                        }
                    }, null);
                }
            }
        }, TaskScheduler.Default);
    }

    /// <summary>
    /// Handles PropertyChanged events from vial slots, specifically tracking IsVialStarred changes.
    /// When a vial is starred/unstarred, we need to recalculate hoard points.
    /// </summary>
    private void OnPlacedVialPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Only recalculate hoard when vial star state changes
        if (e.PropertyName == nameof(VialItemViewModel.IsVialStarred))
        {
            UpdateHoardText();
        }
    }

    private void UpdateHoardText(SavedItem? changedItem = null)
    {
        var wothPoints = _parsedSpoilerDataService.GetWothPointsForRegion(_regionName);
        if (!HasVials || wothPoints < 0)
        {
            WothPoints = -1;
            HasWothPoints = false;
        }
        else
        {
            // Count starred items by querying the actual item state from the tracking service,
            // not from the UI property which may be stale or in transition during optimistic updates
            int starredCount = 0;
            foreach (var item in PlacedItems)
            {
                // Get the item name from the view model (handles both fixed and current items)
                ItemName? itemName = null;
                if (item is VialItemViewModel spoiler)
                {
                    itemName = spoiler.CurrentItemName;
                }
                else if (item is RegionItemViewModel region)
                {
                    itemName = region.CurrentItemName;
                }

                // Check if this item is actually starred in the service
                if (itemName.HasValue)
                {
                    // If this is the item that just changed and we have it in the event args,
                    // use that instead of querying the service (more up-to-date)
                    bool isStarred;
                    if (changedItem?.ItemName == itemName.Value)
                    {
                        isStarred = changedItem.Starred == ItemVisibilityState.Visible;
                    }
                    else
                    {
                        var savedItem = _itemTrackingService.GetItemState(itemName.Value);
                        isStarred = savedItem is not null && savedItem.Starred == ItemVisibilityState.Visible;
                    }

                    if (isStarred)
                    {
                        starredCount++;
                    }
                }
                else
                {
                    // Check the vial to see if it's starred anyway.
                    if (item is VialItemViewModel spoiledItem && spoiledItem.IsVialStarred)
                    {
                        starredCount++;
                    }
                }
            }
            WothPoints = wothPoints - starredCount;
            HasWothPoints = true;
        }
    }

    internal void UpdateItemPointText()
    {
        // TODO: Have the point calculations take place in the service layer.
        var regionPoints = _parsedSpoilerDataService.GetPointsForRegion(_regionName);
        var pointSpread = _parsedSpoilerDataService.GetPointSpread();
        var hasPoints = _parsedSpoilerDataService.GetSpoilerSettings()?.PointsEnabled ?? false;
        if (!hasPoints || pointSpread.Count == 0)
        {
            ItemPoints = -1;
            HasItemPoints = false;
        }
        else
        {
            var itemsInRegion = _itemTrackingService.GetItemsInRegion(_regionName);
            var categories = itemsInRegion.Select(i => i.ItemName.ToPointCategory());
            var points = regionPoints - categories.Sum(c => pointSpread[c]);
            ItemPoints = points;
            HasItemPoints = points >= 0;
        }

        OnPropertyChanged(nameof(ShouldShowItemPoints));
    }

    internal void UpdateRegionResourceKey()
    {
        RegionResourceKey = _regionName.ToString().ToLowerInvariant();
    }

    /// <summary>
    /// Attempts to auto-place an item into an appropriate slot in this region.
    /// In vial enabled mode: prioritizes empty slots, then non-autotracked slots.
    /// In non-vial mode: direct placement through ItemTrackingService.
    /// </summary>
    public bool TryAutoPlaceItem(ItemName itemToPlace)
    {
        // Non-vial mode: direct placement through ItemTrackingService
        if (!HasVials)
        {
            var savedItem = new SavedItem(
                itemToPlace,
                _regionName,
                ItemVisibilityState.Hidden,
                Autotracked: true,
                Opacity: 1.0
            );

            _itemTrackingService.SetItemState(itemToPlace, savedItem);
            return true;
        }

        // Spoiler mode: find a vial slot that matches the color
        var targetColor = itemToPlace.ToVialColor();

        var matchingSlots = PlacedItems
                    .OfType<VialItemViewModel>()
                    .Where(slot => slot.VialColor == targetColor)
                    .ToList();

        // No vials of this color in this region
        if (matchingSlots.Count == 0)
        {
            return false;
        }

        // Priority 1: Try empty slots first
        var emptySlots = matchingSlots
            .Where(slot => !slot.CurrentItemName.HasValue)
            .ToList();

        foreach (var slot in emptySlots)
        {
            if (slot.CanAcceptDrop(itemToPlace, MouseDragType.None)
                && slot.AcceptDropAndMutate(itemToPlace, MouseDragType.None))
            {
                return true;
            }
        }

        // Priority 2: Try non-autotracked slots (can be overridden)
        var overridableSlots = matchingSlots
            .Where(slot => slot.CurrentItemName.HasValue)
            .ToList();

        foreach (var slot in overridableSlots)
        {
            // Check if the current item is autotracked; if so, skip it
            var currentItem = _itemTrackingService.GetItemState(slot.CurrentItemName!.Value);
            if (currentItem?.Autotracked == true)
            {
                // Skip autotracked items—can't override them
                continue;
            }

            if (slot.CanAcceptDrop(itemToPlace, MouseDragType.None)
                && slot.AcceptDropAndMutate(itemToPlace, MouseDragType.None))
            {
                return true;
            }
        }

        // All matching slots either rejected placement or are protected autotracked items
        return false;
    }

    public void SetLighting(bool isLit)
    {
        BackgroundResourceKey = isLit ? "RegionBGLitUp" : "RegionBG";
    }

    #region Object Overrides

    public bool Equals(RegionViewModel? other) => other is not null && other._regionName == _regionName;

    public override bool Equals(object? obj) => Equals(obj as RegionViewModel);

    public override int GetHashCode() => _regionName.GetHashCode();

    public override string ToString() => $"Region: {_regionName}, PlacedItems: {PlacedItems.Count}, WothPoints: {WothPoints}, ItemPoints: {ItemPoints}";

    #endregion

    #region Operator Overloads

    public static bool operator ==(RegionViewModel? left, RegionViewModel? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(RegionViewModel? left, RegionViewModel? right) => !(left == right);

    #endregion
}
