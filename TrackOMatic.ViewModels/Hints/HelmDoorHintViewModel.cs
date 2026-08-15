using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class HelmDoorHintViewModel : ObservableObject, IDisposable
{
    private readonly IBarrierService _barrierService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IItemSelectorService _itemSelectorService;
    private readonly SynchronizationContext _uiContext;
    private readonly HelmDoor _helmDoor;

    #region Proxy Properties

    private HelmDoorSlotViewModel? _slotViewModel;
    public HelmDoorSlotViewModel? SlotViewModel
    {
        get
        {
            _slotViewModel ??= new HelmDoorSlotViewModel(
                _helmDoor,
                _barrierService,
                _savedProgressProvider,
                _itemSelectorService
            );
            return _slotViewModel;
        }
    }

    private string _barrierCount = "";
    public string BarrierCount
    {
        get => _barrierCount;
        set => SetProperty(ref _barrierCount, value);
    }

    private string _labelText = "";
    public string LabelText
    {
        get => _labelText;
        set => SetProperty(ref _labelText, value);
    }

    #endregion

    public HelmDoorHintViewModel(
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
        _barrierService.HelmBarrierAmountChanged += OnBarrierAmountChanged;
        _savedProgressProvider.ProgressChanged += OnDataReset;
        InitializeState();
    }

    private void InitializeState()
    {
        var barrierAmount = _barrierService.GetHelmDoorCount(_helmDoor);
        LabelText = "Door " + (_helmDoor == HelmDoor.First ? "1" : "2") + ":";
        UpdateBarrierCount(barrierAmount);
    }

    #region Commands/Public Methods (Manually executed)

    [RelayCommand]
    private void ClearPlaceholder()
    {
        BarrierCount = "";
    }

    [RelayCommand]
    private void SetBarrierCount()
    {
        _barrierService.SetHelmDoorCount(_helmDoor, BarrierCount);
    }

    #endregion

    #region Event Handling

    private void OnBarrierAmountChanged(object? sender, HelmBarrierAmountChangedEventArgs e)
    {
        if (e.HelmDoor != _helmDoor)
        {
            return;
        }
        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ => UpdateBarrierCount(e.NewValue), null);
        }
        else
        {
            UpdateBarrierCount(e.NewValue);
        }
    }

    private void OnDataReset(object? sender, ProgressReplacedEventArgs e)
    {
        InitializeState();
    }

    private void UpdateBarrierCount(string count)
    {
        BarrierCount = count;
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
                _barrierService.HelmBarrierAmountChanged -= OnBarrierAmountChanged;
                _savedProgressProvider.ProgressChanged -= OnDataReset;
            }
            _disposed = true;
        }
    }

    ~HelmDoorHintViewModel()
    {
        Dispose(false);
    }

    #endregion

    #region Object

    public override bool Equals(object? obj)
    {
        return obj is HelmDoorHintViewModel other && _helmDoor == other._helmDoor;
    }

    public override int GetHashCode()
    {
        return _helmDoor.GetHashCode();
    }

    #endregion
}
