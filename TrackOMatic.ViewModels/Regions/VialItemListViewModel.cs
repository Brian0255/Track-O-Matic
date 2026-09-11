using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Regions;

public partial class VialItemListViewModel : ObservableObject, IItemListViewModel, IEquatable<VialItemListViewModel>, IDisposable
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IThemeService _themeService;
    private readonly IDataSaverService _dataSaverService;
    private readonly SynchronizationContext _syncContext;
    private readonly RegionName _regionName;

    public event EventHandler<VialItemStateChangedEventArgs>? VialItemStateChanged;

    #region Proxy Properties

    public RegionName RegionName => _regionName;

    [ObservableProperty]
    public partial ObservableCollection<VialItemViewModel> Items { get; set; } = [];

    #endregion

    public VialItemListViewModel(
        RegionName regionName,
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IThemeService themeService,
        IDataSaverService dataSaverService
    )
    {
        _regionName = regionName;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _dataSaverService = dataSaverService ?? throw new ArgumentNullException(nameof(dataSaverService));
        _syncContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null. Ensure this ViewModel is created on the UI thread.");

        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnDataReset;
        _dataSaverService.DataLoadingCompleted += OnDataLoadingCompleted;

        RefreshPlacedItems();
    }

    private void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        if (e.ChangeReason != ChangeReason.AutoTracked || e.UpdatedItem.Region != _regionName)
        {
            return;
        }

        // Find a vial and place the item in it. Worse case, replace a badly placed item.
        var item = e.UpdatedItem;
        var targetColor = item.ItemName.ToVialColor();
        var compatibleSlots = Items.Where(slot => slot.VialColor == targetColor).ToList();

        // First see if there's a slot with the item already placed. Maybe a user hinted item?
        var existingItemSlot = compatibleSlots.FirstOrDefault(slot => slot.CurrentItemName == item.ItemName);
        if (existingItemSlot is not null && existingItemSlot.CanAcceptAutoTrackedItem(item.ItemName, item))
        {
            existingItemSlot.AcceptAutoTrackedItemWithoutPersist(item.ItemName, item);
            return;
        }

        // See if there's an empty slot.
        var emptySlot = compatibleSlots.FirstOrDefault(slot => !slot.CurrentItemName.HasValue);
        if (emptySlot is not null && emptySlot.CanAcceptAutoTrackedItem(item.ItemName, item))
        {
            emptySlot.AcceptAutoTrackedItemWithoutPersist(item.ItemName, item);
            return;
        }
    }

    private void OnDataReset(object? sender, ProgressReplacedEventArgs e) => WipeVialConnections();

    private void OnDataLoadingCompleted(object? sender, DataLoadingCompletedEventArgs e) => RefreshPlacedItems();

    private void WipeVialConnections()
    {
        foreach (var vial in Items)
        {
            vial.VialItemStateChanged -= OnVialDataChanged;
            vial.Dispose();
        }
        Items.Clear();
    }

    private void OnParsedSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e) => RefreshPlacedItems();

    private void RefreshPlacedItems()
    {
        using var scope = _itemTrackingService.BeginBatchUpdate();

        WipeVialConnections();

        if (_regionName == RegionName.START)
        {
            var startingItems = _parsedSpoilerDataService.CurrentData?.StartingItems
                .Where(kvp => kvp.Value == RegionName.START)
                .ToList() ?? [];

            static int ItemNameSort(KeyValuePair<ItemName, RegionName> a, KeyValuePair<ItemName, RegionName> b)
            {
                // Fallback: compare against the item name itself.
                return a.Key.CompareTo(b.Key);
            }

            startingItems.Sort(ItemNameSort);
            foreach (var (itemName, index) in startingItems)
            {
                var vialColor = itemName.ToVialColor();

                var vial = new VialItemViewModel(
                    itemName,
                    RegionName.START,
                    vialColor,
                    _itemTrackingService,
                    _parsedSpoilerDataService,
                    _themeService
                );

                vial.VialItemStateChanged += OnVialDataChanged;
                Items.Add(vial);
            }
        }
        else
        {
            // It's a playable region instead, so use the regular approach.
            var spoilerData = _parsedSpoilerDataService.CurrentData;
            var regionData = spoilerData?.RegionData.GetValueOrDefault(_regionName);
            if (regionData is not null)
            {
                // Get all items currently in this region from the service (in their stored order)
                var itemsInRegion = _itemTrackingService.GetItemsInRegion(_regionName).ToList();
                var placedItems = new HashSet<ItemName>();

                for (int slotIndex = 0; slotIndex < regionData.VialColors.Count; ++slotIndex)
                {
                    var vialColor = regionData.VialColors[slotIndex];

                    // Create vial empty (required by constructor validation for non-START regions)
                    var vial = new VialItemViewModel(
                        null,
                        _regionName,
                        vialColor,
                        _itemTrackingService,
                        _parsedSpoilerDataService,
                        _themeService
                    );

                    // Look for the next unpaced item matching this vial color
                    var itemInSlot = itemsInRegion.FirstOrDefault(item => 
                        item.ItemName.ToVialColor() == vialColor && 
                        !placedItems.Contains(item.ItemName));

                    if (itemInSlot is not null)
                    {
                        // Place the saved item into the vial without re-persisting
                        vial.AcceptAutoTrackedItemWithoutPersist(itemInSlot.ItemName, itemInSlot);
                        placedItems.Add(itemInSlot.ItemName);
                    }

                    vial.VialItemStateChanged += OnVialDataChanged;
                    Items.Add(vial);
                }
            }
        }

        OnPropertyChanged(nameof(Items));
    }

    public bool TryAcceptDrop(ItemName itemToPlace, MouseDragType dragType)
    {
        // In vial enabled mode: find the first empty slot that matches the vial color
        var targetColor = itemToPlace.ToVialColor();
        var emptySlot = Items.FirstOrDefault(slot =>
            slot is VialItemViewModel spoilerSlot &&
            spoilerSlot.VialColor == targetColor &&
            !spoilerSlot.CurrentItemName.HasValue
        );

        if (emptySlot == null)
        {
            return false;
        }

        // Check if the slot can accept the drop
        if (!emptySlot.CanAcceptDrop(itemToPlace, dragType))
        {
            return false;
        }

        // Additional start region validation: don't let user drag/drops happen.
        if (RegionName == RegionName.START && dragType != MouseDragType.None)
        {
            return false;
        }

        // Actually mutate the slot state - this updates CurrentItemName and fires UpdateProperties
        return emptySlot.AcceptDropAndMutate(itemToPlace, dragType);
    }

    private void OnVialDataChanged(object? sender, VialItemStateChangedEventArgs e)
    {
        // Pass the event up: this view model doesn't care.
        VialItemStateChanged?.Invoke(sender, e);
    }

    #region Object Overrides

    public bool Equals(VialItemListViewModel? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return _regionName == other._regionName;
    }

    public override bool Equals(object? obj) => Equals(obj as VialItemListViewModel);

    public override int GetHashCode() => _regionName.GetHashCode();

    public override string ToString()
    {
        return $"{_regionName} vial/item count: {Items.Count}";
    }

    #endregion

    #region IDisposable

    private bool _disposed = false;
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
                WipeVialConnections();
                _itemTrackingService.ItemStateChanged -= OnItemStateChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnParsedSpoilerDataChanged;
                _savedProgressProvider.ProgressChanged -= OnDataReset;
                _dataSaverService.DataLoadingCompleted -= OnDataLoadingCompleted;
            }
            _disposed = true;
        }
    }

    ~VialItemListViewModel()
    {
        Dispose(false);
    }

    #endregion
}
