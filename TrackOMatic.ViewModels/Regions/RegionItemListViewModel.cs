using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Regions;

public partial class RegionItemListViewModel : ObservableObject, IItemListViewModel, IEquatable<RegionItemListViewModel>, IDisposable
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IThemeService _themeService;
    private readonly IDataSaverService _dataSaverService;
    private readonly RegionName _regionName;

    #region Proxy Properties

    [ObservableProperty]
    public partial ObservableCollection<RegionItemViewModel> Items { get; set; } = [];

    public RegionName RegionName => _regionName;

    #endregion

    public RegionItemListViewModel(
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

        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _savedProgressProvider.ProgressChanged += OnDataReset;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;
        _dataSaverService.DataLoadingCompleted += OnDataLoadingCompleted;

        RefreshPlacedItems();
    }

    /// <summary>
    /// Refresh the item list for the current region.
    /// </summary>
    private void RefreshPlacedItems()
    {
        using var scope = _itemTrackingService.BeginBatchUpdate();
        Items.Clear();

        var spoilerData = _parsedSpoilerDataService.CurrentData;
        var regionData = spoilerData?.RegionData[_regionName];

        // Have a list immediately to minimize batching/sync issues.
        var itemsInRegion = _itemTrackingService.GetItemsInRegion(_regionName).ToList();
        foreach (var item in itemsInRegion)
        {
            Items.Add(new(
                item.ItemName,
                _regionName,
                _itemTrackingService,
                _parsedSpoilerDataService,
                _themeService
            ));
        }

        OnPropertyChanged(nameof(Items));
    }

    public virtual bool TryAcceptDrop(ItemName itemName, MouseDragType dragType)
    {
        var itemState = _itemTrackingService.GetItemState(itemName);
        if (itemState is null)
        {
            return true;
        }
        if (itemState.Autotracked)
        {
            return false;
        }
        if (itemState.Region == RegionName.START || itemState.Region == RegionName.UNHINTABLE_MOVES)
        {
            return dragType == MouseDragType.None;
        }

        return true;
    }

    /// <summary>
    /// Handle item state changes from the item tracking service.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        var oldState = e.PreviousState;
        var newState = e.UpdatedItem;

        if (oldState?.Region == RegionName || newState.Region == RegionName)
        {
            RefreshPlacedItems();
        }
    }

    private void OnDataReset(object? sender, ProgressReplacedEventArgs e) => RefreshPlacedItems();

    private void OnDataLoadingCompleted(object? sender, DataLoadingCompletedEventArgs e) => RefreshPlacedItems();

    /// <summary>
    /// Refresh the item list when the parsed spoiler data changes, as the items in this region may have changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnParsedSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e) => RefreshPlacedItems();

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
                _itemTrackingService.ItemStateChanged -= OnItemStateChanged;
                _savedProgressProvider.ProgressChanged -= OnDataReset;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnParsedSpoilerDataChanged;
                _dataSaverService.DataLoadingCompleted -= OnDataLoadingCompleted;
            }
            _disposed = true;
        }
    }

    ~RegionItemListViewModel()
    {
        Dispose(false);
    }

    #endregion

    #region Object Overrides

    public bool Equals(RegionItemListViewModel? other)
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

    public override bool Equals(object? obj) => Equals(obj as RegionItemListViewModel);

    public override int GetHashCode()
    {
        return _regionName.GetHashCode();
    }

    public override string ToString()
    {
        return $"Item count for {_regionName}: {Items.Count}";
    }

    #endregion
}
