using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Events.Autotracking;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;


[assembly: InternalsVisibleTo("TrackOMatic.ViewModels.Test")]

namespace TrackOMatic.ViewModels.Regions;

public partial class RegionViewModel : ObservableObject, IEquatable<RegionViewModel>, IDisposable, IRegionSlotProvider
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly IThemeService _themeService;
    private readonly IDataSaverService _dataSaverService;
    private readonly RegionName _regionName;
    private readonly IAutotrackerService _autotrackerService;
    private readonly RegionSlotProviderRegistry? _slotProviderRegistry;
    private readonly SynchronizationContext _syncContext;

    private readonly VialItemListViewModel? _vialItemListViewModel;
    private readonly RegionItemListViewModel? _regionItemListViewModel;

    #region Proxy Properties

    public RegionName RegionName => _regionName;

    public RegionItemListViewModel? RegionItemListViewModel => _regionItemListViewModel;
    public VialItemListViewModel? VialItemListViewModel => _vialItemListViewModel;

    // Placed items in the grid
    [ObservableProperty]
    public partial ObservableCollection<IRegionItemViewModel> PlacedItems { get; set; } = [];

    [ObservableProperty]
    public partial string RegionResourceKey { get; set; } = "";

    [ObservableProperty]
    public partial int ImageColumnSpan { get; set; } = 1;

    [ObservableProperty]
    public partial string BackgroundResourceKey { get; set; } = "RegionBG";

    [ObservableProperty]
    public partial bool HasWothPoints { get; set; } = false;

    [ObservableProperty]
    public partial int WothPoints { get; set; } = -1;

    [ObservableProperty]
    public partial bool HasItemPoints { get; set; } = false;

    [ObservableProperty]
    public partial int ItemPoints { get; set; } = -1;

    [ObservableProperty]
    public partial string ItemPointColorKey { get; set; } = "";

    [ObservableProperty]
    public partial bool HasVials { get; set; } = false;

    public bool ShouldShowItemPoints
    {
        get => _regionName != RegionName.START && HasItemPoints;
    }

    public bool ShouldShowRegionLevel
    {
        get => _regionName != RegionName.START && _regionName != RegionName.DK_ISLES;
    }

    #endregion

    public RegionViewModel(
        RegionName regionName,
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IThemeService themeService,
        RegionSlotProviderRegistry slotProviderRegistry,
        IAutotrackerService autotrackerService,
        IDataSaverService dataSaverService
    )
    {
        _regionName = regionName;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _autotrackerService = autotrackerService ?? throw new ArgumentNullException(nameof(autotrackerService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _dataSaverService = dataSaverService ?? throw new ArgumentNullException(nameof(dataSaverService));

        // Capture the synchronization context for UI thread marshaling in event handlers
        // In tests (no UI thread), this will be null and we'll run directly
        _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();

        _vialItemListViewModel = new VialItemListViewModel(
            _regionName,
            _itemTrackingService,
            _parsedSpoilerDataService,
            savedProgressProvider,
            _themeService,
            _dataSaverService
        );
        _regionItemListViewModel = new RegionItemListViewModel(
            _regionName,
            _itemTrackingService,
            _parsedSpoilerDataService,
            savedProgressProvider,
            _themeService,
            _dataSaverService
        );

        // Registry is optional for backwards compatibility with existing tests/code
        _slotProviderRegistry = slotProviderRegistry;

        // Register this region's vial slots with the registry (if available)
        _slotProviderRegistry?.RegisterRegionProvider(_regionName, this);

        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;
        _vialItemListViewModel.VialItemStateChanged += OnVialStateChanged;
        _autotrackerService.RegionLightingChanged += OnRegionLightingChanged;
        _dataSaverService.DataLoadingCompleted += OnDataLoadingCompleted;

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
            savedProgressProvider,
            dataSaverService
        );
    }

    // RegionLevelViewModel for display
    private readonly RegionLevelViewModel _regionLevelViewModel;
    public RegionLevelViewModel RegionLevelViewModel => _regionLevelViewModel;

    private void InitializeState()
    {
        UpdateHoardText();
        UpdateItemPointText();
        UpdateImageColumnSpan();
        UpdateRegionResourceKey();
    }

    public IEnumerable<ObservableObject> SelectedItemList
    {
        get => HasVials
            ? VialItemListViewModel?.Items.Cast<ObservableObject>() ?? []
            : RegionItemListViewModel?.Items.Cast<ObservableObject>() ?? [];
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
        return [];
    }

    private void OnRegionLightingChanged(object? sender, AutotrackerRegionEventArgs e)
    {
        if (e.Region != _regionName)
        {
            return;
        }
        _syncContext.Post(_ => SetLighting(e.LightUp), null);
    }

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
        bool isRelevantChange = e.ChangeReason == ChangeReason.UserModified || e.ChangeReason == ChangeReason.AutoTracked;

        if (!isRelevantChange)
        {
            return;
        }

        UpdateHoardText();
        UpdateItemPointText();
    }

    private void OnDataLoadingCompleted(object? sender, DataLoadingCompletedEventArgs e)
    {
        _syncContext.Post(_ =>
        {
            HasVials = _parsedSpoilerDataService.GetSpoilerSettings()?.VialsEnabled ?? false;
            UpdateHoardText();
            UpdateItemPointText();
            UpdateImageColumnSpan();
            OnPropertyChanged(nameof(SelectedItemList));
        }, null);
    }

    private void OnParsedSpoilerDataChanged(object? sender, EventArgs e)
    {
        _syncContext.Post(_ =>
        {
            HasVials = _parsedSpoilerDataService.GetSpoilerSettings()?.VialsEnabled ?? false;
            UpdateHoardText();
            UpdateItemPointText();
            UpdateImageColumnSpan();
            OnPropertyChanged(nameof(SelectedItemList));
        }, null);
    }

    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e)
    {
        _syncContext.Post(_ =>
        {
            HasVials = _parsedSpoilerDataService.GetSpoilerSettings()?.VialsEnabled ?? false;
            UpdateHoardText();
            UpdateItemPointText();
            UpdateImageColumnSpan();
            OnPropertyChanged(nameof(SelectedItemList));
        }, null);
    }

    private void OnVialStateChanged(object? sender, VialItemStateChangedEventArgs e)
    {
        if (!HasVials)
        {
            return;
        }
        if (_regionName != e.RegionName)
        {
            return;
        }

        _syncContext.Post(_ =>
        {
            UpdateHoardText();
            UpdateItemPointText();
        }, null);
    }

    private void UpdateHoardText()
    {
        var wothPoints = _parsedSpoilerDataService.GetWothPointsForRegion(_regionName);
        if (wothPoints < 0)
        {
            WothPoints = -1;
            HasWothPoints = false;
            return;
        }

        var itemsinRegion = _itemTrackingService.GetItemsInRegion(_regionName);
        var starredItems = itemsinRegion.Count(x => x.Starred);

        // If we have vials active, check the vials without items next.
        var starredPotions = VialItemListViewModel?.Items.Count(x => x.CurrentItemName is null && x.IsVialStarred) ?? 0;

        WothPoints = wothPoints - starredItems - starredPotions;
        HasWothPoints = true;
    }

    private void UpdateImageColumnSpan()
    {
        var spoilerData = _parsedSpoilerDataService.CurrentData;
        if (spoilerData is not null && (spoilerData.SpoilerSettings.VialsEnabled || spoilerData.SpoilerSettings.PointsEnabled))
        {
            ImageColumnSpan = 2;
        }
        else
        {
            ImageColumnSpan = 4;
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
            ItemPointColorKey = "RegionInProgress";
        }
        else
        {
            var itemsInRegion = _itemTrackingService.GetItemsInRegion(_regionName);
            var categories = itemsInRegion.Select(i => i.ItemName.ToPointCategory());
            var points = regionPoints - categories.Sum(c => pointSpread[c]);
            ItemPoints = points;
            HasItemPoints = points >= 0;
            ItemPointColorKey = points <= 0 ? "RegionComplete" : "RegionInProgress";
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
                starred: false,
                autotracked: true,
                opacity: 1.0
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

    private void SetLighting(bool isLit)
    {
        BackgroundResourceKey = isLit ? "RegionBGLitUp" : "RegionBG";
    }


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
                // Unregister from slot provider registry
                _slotProviderRegistry?.UnregisterRegionProvider(_regionName);

                // Unsubscribe from service events
                _itemTrackingService.ItemStateChanged -= OnItemStateChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnParsedSpoilerDataChanged;
                _vialItemListViewModel?.VialItemStateChanged -= OnVialStateChanged;
                _autotrackerService.RegionLightingChanged -= OnRegionLightingChanged;
                _dataSaverService.DataLoadingCompleted -= OnDataLoadingCompleted;
            }
            _disposed = true;
        }
    }

    ~RegionViewModel()
    {
        Dispose(false);
    }

    #endregion

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
