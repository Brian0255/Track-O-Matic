using System.Collections.ObjectModel;
using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Regions;

public partial class VialItemListViewModel : ObservableObject, IItemListViewModel, IEquatable<VialItemListViewModel>, IDisposable
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IThemeService _themeService;
    private readonly SynchronizationContext _syncContext;
    private readonly RegionName _regionName;

    public event EventHandler<VialItemStateChangedEventArgs>? VialItemStateChanged;

    #region Proxy Properties

    public RegionName RegionName => _regionName;

    private ObservableCollection<VialItemViewModel> _items = [];
    public ObservableCollection<VialItemViewModel> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    #endregion

    public VialItemListViewModel(
        RegionName regionName,
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IThemeService themeService
    )
    {
        _regionName = regionName;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _syncContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null. Ensure this ViewModel is created on the UI thread.");

        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnDataReset;

        RefreshPlacedItems();
    }

    private void OnDataReset(object? sender, ProgressReplacedEventArgs e) => WipeVialConnections();

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
                // Helpful no matter which region.
                var vialColors = _parsedSpoilerDataService.GetVialColorsForRegion(_regionName);

                for (int slotIndex = 0; slotIndex < regionData.VialColors.Count; ++slotIndex)
                {
                    var vialColor = regionData.VialColors[slotIndex];
                    var vial = new VialItemViewModel(
                        null,
                        _regionName,
                        vialColor,
                        _itemTrackingService,
                        _parsedSpoilerDataService,
                        _themeService
                    );
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
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnParsedSpoilerDataChanged;
                _savedProgressProvider.ProgressChanged -= OnDataReset;
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
