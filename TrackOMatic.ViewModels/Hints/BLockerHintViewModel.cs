using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class BLockerHintViewModel : ObservableObject, IEquatable<BLockerHintViewModel>, IDisposable
{
    private readonly IBarrierService _barrierService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IItemSelectorService _itemSelectorService;
    private readonly IDataSaverService _dataSaverService;
    private readonly SynchronizationContext _uiContext;
    private readonly RegionName _regionName;

    #region Proxy Properties

    private BLockerSlotViewModel? _slotViewModel;
    public BLockerSlotViewModel? SlotViewModel
    {
        get
        {
            _slotViewModel ??= new BLockerSlotViewModel(
                _regionName,
                _barrierService,
                _parsedSpoilerDataService,
                _savedProgressProvider,
                _itemSelectorService
            );
            return _slotViewModel;
        }
    }

    [ObservableProperty]
    public partial string BarrierCount { get; set; } = "";

    [ObservableProperty]
    public partial string RegionResourceKey { get; set; } = "";

    public bool IsLocked
    {
        get => _parsedSpoilerDataService.HasRegionBarrier();
    }

    #endregion

    public BLockerHintViewModel(
        RegionName regionName,
        IBarrierService barrierService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IItemSelectorService itemSelectorService,
        IDataSaverService dataSaverService
    )
    {
        if (!EndGameMappings.LOBBY_ORDER.Contains(regionName))
        {
            throw new ArgumentException($"Region {regionName} is not a valid lobby region.", nameof(regionName));
        }
        _regionName = regionName;
        _barrierService = barrierService ?? throw new ArgumentNullException(nameof(barrierService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _itemSelectorService = itemSelectorService ?? throw new ArgumentNullException(nameof(itemSelectorService));
        _dataSaverService = dataSaverService ?? throw new ArgumentNullException(nameof(dataSaverService));
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null. Ensure this is called from the UI thread.");
        _barrierService.BlockerBarrierAmountChanged += OnBarrierAmountChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnDataReset;
        _dataSaverService.DataLoadingCompleted += OnDataLoaded;
        InitializeState();
    }

    private void InitializeState()
    {
        var barrierAmount = _barrierService.GetBLockerCount(_regionName);
        RegionResourceKey = _regionName.ToString().ToLowerInvariant();
        UpdateBarrierCount(barrierAmount);
    }

    #region Commands/Public Methods (Manually executed)

    [RelayCommand(CanExecute = nameof(CanClearPlaceholder))]
    private void ClearPlaceholder()
    {
        BarrierCount = "";
    }

    private bool CanClearPlaceholder()
    {
        return !HasRegionBarrier() && BarrierCount == "?";
    }

    [RelayCommand(CanExecute = nameof(CanSetBarrierCount))]
    private void SetBarrierCount()
    {
        _barrierService.SetBLockerCount(_regionName, BarrierCount);
    }
    private bool CanSetBarrierCount()
    {
        return !HasRegionBarrier() && BarrierCount != "?";
    }

    public bool HasRegionBarrier() => _parsedSpoilerDataService.HasRegionBarrier();

    #endregion

    #region Event Handling

    private void OnBarrierAmountChanged(object? sender, BlockerBarrierAmountChangedEventArgs e)
    {
        if (e.RegionName != _regionName)
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

    private void OnSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e)
    {
        if (e.NewData is null)
        {
            return;
        }

        string target;
        try
        {
            var barrierInfo = e.NewData.GetBarrierInfo(_regionName);
            int cost = barrierInfo.Cost;
            target = (cost < 0) ? "?" : cost.ToString();
        }
        catch (Exception)
        {
            target = "?";
        }

        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ =>
            {
                UpdateBarrierCount(target);
                OnPropertyChanged(nameof(IsLocked));
            }, null);
        }
        else
        {
            UpdateBarrierCount(target);
            OnPropertyChanged(nameof(IsLocked));
        }
    }

    private void OnDataReset(object? sender, ProgressReplacedEventArgs e)
    {
        InitializeState();
        OnPropertyChanged(nameof(IsLocked));
    }

    private void OnDataLoaded(object? sender, DataLoadingCompletedEventArgs e)
    {
        var spoilerBarrierData = _parsedSpoilerDataService.GetBarrierInfo(_regionName);
        if (spoilerBarrierData.Cost < 0)
        {
            InitializeState();
            OnPropertyChanged(nameof(IsLocked));
            return;
        }
        var target = spoilerBarrierData.Cost.ToString();
        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ =>
            {
                UpdateBarrierCount(target);
                OnPropertyChanged(nameof(IsLocked));
            }, null);
        }
        else
        {
            UpdateBarrierCount(target);
            OnPropertyChanged(nameof(IsLocked));
        }
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
                _barrierService.BlockerBarrierAmountChanged -= OnBarrierAmountChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnSpoilerDataChanged;
                _savedProgressProvider.ProgressChanged -= OnDataReset;
                _dataSaverService.DataLoadingCompleted -= OnDataLoaded;
            }
            _disposed = true;
        }
    }

    ~BLockerHintViewModel()
    {
        Dispose(false);
    }

    #endregion

    #region Object Overrides

    public bool Equals(BLockerHintViewModel? other)
    {
        if (other is null)
        {
            return false;
        }
        return _regionName == other._regionName;
    }

    public override bool Equals(object? obj) => Equals(obj as BLockerHintViewModel);

    public override int GetHashCode()
    {
        return _regionName.GetHashCode();
    }

    #endregion
}
