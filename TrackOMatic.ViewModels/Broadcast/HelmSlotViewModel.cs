using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public class HelmSlotViewModel: ObservableObject, IDisposable
{
    private readonly IEndGameProgressionService _endGameProgressionService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly SynchronizationContext _uiContext;
    private readonly int _slotNumber;

    protected static readonly List<ItemGridCell> _helmKongMappings =
    [
        new(0, 0, "unknown_kong_bw"),
        new(0, 1, "donkey"),
        new(0, 2, "diddy"),
        new(0, 3, "lanky"),
        new(0, 4, "tiny"),
        new(0, 5, "chunky")
    ];

    public HelmSlotViewModel(
        int slotNumber,
        IEndGameProgressionService endGameProgressionService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider
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
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null. Ensure this is called from the UI thread.");

        _endGameProgressionService.BlastStateChanged += OnBlastStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnDataReset;

        InitializeState();
    }

    private void InitializeState()
    {
        var helmKongs = _endGameProgressionService.GetHelmKongs();
        HelmValue = helmKongs[_slotNumber];
        OnPropertyChanged(nameof(ShowsSlot));
        UpdateImageResourceKey();
    }

    #region Proxy Properties

    private bool _showsSlot = true;
    public bool ShowsSlot
    {
        get => _showsSlot;
        protected set => SetProperty(ref _showsSlot, value);
    }

    private int _helmValue = 0;
    public int HelmValue
    {
        get => _helmValue;
        protected set => SetProperty(ref _helmValue, value);
    }

    private string _imageResourceKey = "";
    public string ImageResourceKey
    {
        get => _imageResourceKey;
        protected set => SetProperty(ref _imageResourceKey, value);
    }

    #endregion

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

    protected virtual void UpdateImageResourceKey()
    {
        ImageResourceKey = _helmKongMappings[HelmValue].ResourceKey;
    }

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
