using CommunityToolkit.Mvvm.ComponentModel;

using Humanizer;

using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public partial class HelmSlotViewModel : ObservableObject, IEquatable<HelmSlotViewModel>, IDisposable
{
    protected static readonly List<ItemGridCell> _helmKongMappings =
    [
        new(0, 0, "unknown_kong_bw"),
        new(0, 1, "donkey"),
        new(0, 2, "diddy"),
        new(0, 3, "lanky"),
        new(0, 4, "tiny"),
        new(0, 5, "chunky")
    ];

    private readonly IEndGameProgressionService _endGameProgressionService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IDataSaverService _dataSaverService;
    private readonly SynchronizationContext _uiContext;
    private readonly int _slotNumber;


    #region Proxy Properties

    [ObservableProperty]
    public partial bool ShowsSlot { get; set; } = true;

    [ObservableProperty]
    public partial int HelmValue { get; set; } = 0;

    [ObservableProperty]
    public partial string ImageResourceKey { get; set; } = "";

    #endregion

    public HelmSlotViewModel(
        int slotNumber,
        IEndGameProgressionService endGameProgressionService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IDataSaverService dataSaverService
    )
    {
        if (slotNumber < 0 || slotNumber > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(slotNumber), "Slot number must be between 0 and 4.");
        }
        _slotNumber = slotNumber;
        _endGameProgressionService = endGameProgressionService ?? throw new ArgumentNullException(nameof(endGameProgressionService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _dataSaverService = dataSaverService ?? throw new ArgumentNullException(nameof(dataSaverService));
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null. Ensure this is called from the UI thread.");

        _endGameProgressionService.BlastStateChanged += OnBlastStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnDataReset;
        _dataSaverService.DataLoadingCompleted += OnDataLoadingCompleted;

        InitializeState();
    }

    private void InitializeState()
    {
        var helmKongs = _endGameProgressionService.GetHelmKongs();
        HelmValue = helmKongs[_slotNumber];
        OnPropertyChanged(nameof(ShowsSlot));
        UpdateImageResourceKey();
    }

    private void OnBlastStateChanged(object? sender, BlastStateChangedEventArgs e)
    {
        var newHelmKongs = e.NewValue;

        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ => UpdateHelmValue(newHelmKongs[_slotNumber]), null);
        }
        else
        {
            UpdateHelmValue(newHelmKongs[_slotNumber]);
        }
    }

    private void UpdateHelmValue(int newValue)
    {
        HelmValue = newValue;
        UpdateImageResourceKey();
    }

    private void OnSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e)
    {
        if (e.NewData is null)
        {
            HelmValue = 0;
            ShowsSlot = true;
        }
        else
        {
            var data = e.NewData.HelmOrder;
            var kongsRequired = data.Count;
            if (_slotNumber < kongsRequired)
            {
                HelmValue = data[_slotNumber];
                ShowsSlot = true;
            }
            else
            {
                HelmValue = 0;
                ShowsSlot = false;
            }
        }
        OnPropertyChanged(nameof(ShowsSlot));
        UpdateImageResourceKey();
    }

    private void OnDataReset(object? sender, EventArgs e) => InitializeState();

    private void OnDataLoadingCompleted(object? sender, DataLoadingCompletedEventArgs e)
    {
        var hasSpoilerData = false;
        if (e.HasSpoiler && _parsedSpoilerDataService.HasHelmOrder())
        {
            hasSpoilerData = true;
        }
        if (!hasSpoilerData)
        {
            InitializeState();
            return;
        }
        var data = _parsedSpoilerDataService.CurrentData!.HelmOrder;
        var kongsRequired = data.Count;
        if (_slotNumber < kongsRequired)
        {
            HelmValue = data[_slotNumber];
            ShowsSlot = true;
        }
        else
        {
            HelmValue = 0;
            ShowsSlot = false;
        }
        OnPropertyChanged(nameof(ShowsSlot));
        UpdateImageResourceKey();
    }

    protected virtual void UpdateImageResourceKey()
    {
        ImageResourceKey = _helmKongMappings[HelmValue].ResourceKey;
    }

    protected string GetHelmName()
    {
        return HelmValue switch
        {
            0 => "Unknown",
            1 => "Donkey",
            2 => "Diddy",
            3 => "Lanky",
            4 => "Tiny",
            5 => "Chunky",
            _ => throw new InvalidOperationException($"Invalid HelmValue: {HelmValue}"),
        };
    }

    #region Object Overrides

    public bool Equals(HelmSlotViewModel? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return _slotNumber == other._slotNumber;
    }

    public override bool Equals(object? obj) => Equals(obj as HelmSlotViewModel);

    public override int GetHashCode() => _slotNumber.GetHashCode();

    public override string ToString()
    {
        return $"Broadcast: The {(_slotNumber + 1).Ordinalize()} Kong to destroy the Blast-O-Matic is {GetHelmName()}.";
    }

    #endregion

    #region IDisposable Pattern

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
                _endGameProgressionService.BlastStateChanged -= OnBlastStateChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnSpoilerDataChanged;
                _savedProgressProvider.ProgressChanged -= OnDataReset;
                _dataSaverService.DataLoadingCompleted -= OnDataLoadingCompleted;
            }
            _disposed = true;
        }
    }

    ~HelmSlotViewModel()
    {
        Dispose(false);
    }

    #endregion
}
