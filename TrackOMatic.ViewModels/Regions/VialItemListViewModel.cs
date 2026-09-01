using System.Collections.ObjectModel;
using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;

namespace TrackOMatic.ViewModels.Regions;

public partial class VialItemListViewModel : ObservableObject, IEquatable<VialItemListViewModel>, IDisposable
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly IThemeService _themeService;
    private readonly SynchronizationContext _syncContext;
    private readonly RegionName _regionName;

    /// <summary>
    /// An event that's fired when a vial's star state changes.
    /// This comes in handy since if the vial item is not set, the star is UI powered rather than state powered.
    /// </summary>
    public event Action? HoardStateChanged;

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
        IThemeService themeService
    )
    {
        _regionName = regionName;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _syncContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null. Ensure this ViewModel is created on the UI thread.");

        RefreshPlacedItems();
    }

    private void RefreshPlacedItems()
    {
        using var scope = _itemTrackingService.BeginBatchUpdate();

        // Unsubscribe from old vials
        foreach (var vial in Items)
        {
            vial.PropertyChanged -= OnVialDataChanged;
            vial.Dispose();
        }
        Items.Clear();

        if (_regionName == RegionName.START)
        {
            var startingItems = _parsedSpoilerDataService.CurrentData?.StartingItems
                .Where(kvp => kvp.Value == RegionName.START)
                .ToList() ?? [];

            for (int slotIndex = 0; slotIndex < startingItems.Count; ++slotIndex)
            {
                var itemName = startingItems[slotIndex].Key;
                var vialColor = itemName.ToVialColor();

                var vial = new VialItemViewModel(
                    itemName,
                    RegionName.START,
                    vialColor,
                    _itemTrackingService,
                    _parsedSpoilerDataService,
                    _themeService
                );

                vial.PropertyChanged += OnVialDataChanged;
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
                    vial.PropertyChanged += OnVialDataChanged;
                    Items.Add(vial);
                }
            }
        }
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

    private void OnVialDataChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VialItemViewModel.IsVialStarred))
        {
            HoardStateChanged?.Invoke();
        }
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
                foreach (var vial in Items)
                {
                    vial.PropertyChanged -= OnVialDataChanged;
                    vial.Dispose();
                }
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
