using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class BLockerSlotViewModel : ObservableObject, IDisposable
{
    private readonly IBarrierService _barrierService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IItemSelectorService _itemSelectorService;
    private readonly SynchronizationContext _uiContext;
    private readonly RegionName _regionName;

    protected static readonly List<ItemGridCell> _barrierMappsings =
    [
        new(0, 0, "golden_banana"),
        new(0, 1, "blueprint"),
        new(0, 2, "pearl"),
        new(0, 3, "crown"),
        new(0, 4, "medal"),
        new(0, 5, "rainbow_coin"),
        new(0, 6, "fairy"),
        new(1, 0, "company_coin"),
        new(1, 1, "bean"),
    ];

    #region Proxy Properties

    [ObservableProperty]
    public partial int BarrierValue { get; set; } = -1;

    [ObservableProperty]
    public partial string ImageResourceKey { get; set; } = "";

    #endregion

    public BLockerSlotViewModel(
        RegionName regionName,
        IBarrierService barrierService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IItemSelectorService itemSelectorService
    )
    {
        if (!EndGameMappings.LOBBY_ORDER.Contains(regionName))
        {
            throw new ArgumentOutOfRangeException(nameof(regionName), "Region name must be a valid lobby region.");
        }
        _regionName = regionName;
        _barrierService = barrierService ?? throw new ArgumentNullException(nameof(barrierService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _itemSelectorService = itemSelectorService ?? throw new ArgumentNullException(nameof(itemSelectorService));
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null. Ensure this is called from the UI thread.");

        _barrierService.BlockerBarrierTypeChanged += OnBarrierChange;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnDataReset;

        InitializeState();
    }

    private void InitializeState()
    {
        var barrierType = _barrierService.GetBLockerItemType(_regionName);
        UpdateBarrierValue(barrierType);
    }

    #region Commands

    [RelayCommand(CanExecute = nameof(CanSelectItem))]
    private async Task SelectItem((double X, double Y) screenMousePosition)
    {
        var result = await _itemSelectorService.SelectItemAsync(
            _barrierMappsings.AsReadOnly(),
            "Select Barrier Item",
            screenMousePosition.X,
            screenMousePosition.Y
        );
        if (result is null)
        {
            return;
        }
        var key = result.ResourceKey;
        var value = _barrierMappsings.FindIndex(k => k.ResourceKey == key);
        var newBarrierType = (BarrierItems)value;
        _barrierService.SetBLockerItemType(_regionName, newBarrierType);
    }

    private bool CanSelectItem((double X, double Y) screenMousePosition) => !_parsedSpoilerDataService.HasRegionBarrier();

    [RelayCommand(CanExecute = nameof(CanCycle))]
    private void CycleNext()
    {
        var oldValue = _barrierService.GetBLockerItemType(_regionName);
        var newValue = (BarrierItems)(((int)oldValue + 1) % _barrierMappsings.Count);
        _barrierService.SetBLockerItemType(_regionName, newValue);
    }

    [RelayCommand(CanExecute = nameof(CanCycle))]
    private void CyclePrevious()
    {
        var oldValue = _barrierService.GetBLockerItemType(_regionName);
        var newValue = (BarrierItems)(((int)oldValue + _barrierMappsings.Count - 1) % _barrierMappsings.Count);
        _barrierService.SetBLockerItemType(_regionName, newValue);
    }

    private bool CanCycle() => !_parsedSpoilerDataService.HasRegionBarrier();

    #endregion

    #region Event Handling

    private void OnDataReset(object? sender, ProgressReplacedEventArgs e) => InitializeState();

    private void OnSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e)
    {
        BarrierItems target;

        if (e.NewData is null)
        {
            target = BarrierItems.GOLDEN_BANANA;
        }
        else
        {
            var blockerData = e.NewData.GetBarrierInfo(_regionName);
            target = blockerData.Item.FromRawGameToBarrier() ?? BarrierItems.GOLDEN_BANANA;
        }

        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ => UpdateBarrierValue(target), null);
        }
        else
        {
            UpdateBarrierValue(target);
        }
    }

    private void OnBarrierChange(object? sender, BlockerBarrierTypeChangedEventArgs e)
    {
        if (e.RegionName != _regionName)
        {
            return;
        }
        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ => UpdateBarrierValue(e.NewValue), null);
        }
        else
        {
            UpdateBarrierValue(e.NewValue);
        }
    }

    private void UpdateBarrierValue(BarrierItems item)
    {
        BarrierValue = (int)item;
        ImageResourceKey = _barrierMappsings[BarrierValue].ResourceKey;
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
                _barrierService.BlockerBarrierTypeChanged -= OnBarrierChange;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnSpoilerDataChanged;
                _savedProgressProvider.ProgressChanged -= OnDataReset;
            }
            _disposed = true;
        }
    }

    ~BLockerSlotViewModel()
    {
        Dispose(false);
    }

    #endregion
}
