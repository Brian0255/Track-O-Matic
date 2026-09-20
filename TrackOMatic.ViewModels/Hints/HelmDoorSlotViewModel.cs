using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class HelmDoorSlotViewModel : ObservableObject, IDisposable
{
    private readonly IBarrierService _barrierService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IItemSelectorService _itemSelectorService;
    private readonly SynchronizationContext _uiContext;
    private readonly HelmDoor _helmDoor;

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

    public HelmDoorSlotViewModel(
        HelmDoor helmDoor,
        IBarrierService barrierService,
        ISavedProgressProvider savedProgressProvider,
        IItemSelectorService itemSelectorService
    )
    {
        _helmDoor = helmDoor;
        _barrierService = barrierService ?? throw new ArgumentNullException(nameof(barrierService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _itemSelectorService = itemSelectorService ?? throw new ArgumentNullException(nameof(itemSelectorService));
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null. Ensure this is called from the UI thread.");

        _barrierService.HelmBarrierTypeChanged += OnBarrierChange;
        _savedProgressProvider.ProgressChanged += OnDataReset;

        InitializeState();
    }

    private void InitializeState()
    {
        var barrierType = _barrierService.GetHelmDoorItemType(_helmDoor);
        UpdateBarrierValue(barrierType);
    }

    #region Commands

    [RelayCommand]
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
        _barrierService.SetHelmDoorItemType(_helmDoor, newBarrierType);
    }

    [RelayCommand]
    private void CycleNext()
    {
        var oldValue = _barrierService.GetHelmDoorItemType(_helmDoor);
        var newValue = (BarrierItems)(((int)oldValue + 1) % _barrierMappsings.Count);
        _barrierService.SetHelmDoorItemType(_helmDoor, newValue);
    }

    [RelayCommand]
    private void CyclePrevious()
    {
        var oldValue = _barrierService.GetHelmDoorItemType(_helmDoor);
        var newValue = (BarrierItems)(((int)oldValue + _barrierMappsings.Count - 1) % _barrierMappsings.Count);
        _barrierService.SetHelmDoorItemType(_helmDoor, newValue);
    }

    #endregion

    #region Event Handling

    private void OnDataReset(object? sender, ProgressReplacedEventArgs e) => InitializeState();

    private void OnBarrierChange(object? sender, HelmBarrierTypeChangedEventArgs e)
    {
        if (e.HelmDoor != _helmDoor)
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
                _barrierService.HelmBarrierTypeChanged -= OnBarrierChange;
                _savedProgressProvider.ProgressChanged -= OnDataReset;
            }
            _disposed = true;
        }
    }

    ~HelmDoorSlotViewModel()
    {
        Dispose(false);
    }

    #endregion
}
