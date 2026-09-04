using CommunityToolkit.Mvvm.ComponentModel;

using Humanizer;

using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public partial class BossSlotViewModel: ObservableObject, IEquatable<BossSlotViewModel>, IDisposable
{
    protected static readonly List<ItemGridCell> _bossKongMappings =
    [
        new(0, 0, "unknown_kong_bw"),
        new(0, 1, "donkey"),
        new(0, 2, "diddy"),
        new(0, 3, "lanky"),
        new(0, 4, "tiny"),
        new(0, 5, "chunky"),
        new(1, 0, "army"),
        new(1, 1, "doga"),
        new(1, 2, "madjack"),
        new(1, 3, "pufftoss"),
        new(1, 4, "doga2"),
        new(1, 5, "army2"),
        new(1, 6, "kutout"),
    ];

    private readonly IEndGameProgressionService _endGameProgressionService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly SynchronizationContext _uiContext;
    private readonly int _slotNumber;

    #region Proxy Properties

    [ObservableProperty]
    public partial bool ShowsSlot { get; set; } = true;

    [ObservableProperty]
    public partial int BossValue { get; set; } = 0;

    [ObservableProperty]
    public partial string ImageResourceKey { get; set; } = "";

    #endregion

    public BossSlotViewModel(
        int slotNumber,
        IEndGameProgressionService endGameProgressionService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider
    )
    {
        if (slotNumber < 0 || slotNumber > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(slotNumber), "Slot number must be between 0 and 12.");
        }
        _slotNumber = slotNumber;
        _endGameProgressionService = endGameProgressionService ?? throw new ArgumentNullException(nameof(endGameProgressionService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null. Ensure this is called from the UI thread.");

        _endGameProgressionService.GauntletStateChanged += OnBossStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnDataReset;

        InitializeState();
    }

    private void InitializeState()
    {
        var bossKongs = _endGameProgressionService.GetBossKongs();
        BossValue = bossKongs[_slotNumber];
        OnPropertyChanged(nameof(ShowsSlot));
        UpdateImageResourceKey();
    }

    private void OnBossStateChanged(object? sender, GauntletStateChangedEventArgs e)
    {
        var newBossKongs = e.NewValue;

        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ => UpdateBossValue(newBossKongs[_slotNumber]), null);
        }
        else
        {
            UpdateBossValue(newBossKongs[_slotNumber]);
        }
    }

    private void UpdateBossValue(int newValue)
    {
        BossValue = newValue;
        UpdateImageResourceKey();
    }

    private void OnSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e)
    {
        if (e.NewData is null)
        {
            BossValue = 0;
            ShowsSlot = true;
        }
        else
        {
            var data = e.NewData.FinalBossOrder;
            var kongsRequired = data.Count;
            if (_slotNumber < kongsRequired)
            {
                BossValue = data[_slotNumber];
                ShowsSlot = true;
            }
            else
            {
                BossValue = 0;
                ShowsSlot = false;
            }
        }
        OnPropertyChanged(nameof(ShowsSlot));
        UpdateImageResourceKey();
    }

    private void OnDataReset(object? sender, EventArgs e) => InitializeState();

    protected virtual void UpdateImageResourceKey()
    {
        ImageResourceKey = _bossKongMappings[BossValue].ResourceKey;
    }

    protected string GetBossName()
    {
        return BossValue switch
        {
            0 => "Unknown",
            1 => "Donkey Phase",
            2 => "Diddy Phase",
            3 => "Lanky Phase",
            4 => "Tiny Phase",
            5 => "Chunky Phase",
            6 => "Army 1",
            7 => "Doga 1",
            8 => "Mad Jack",
            9 => "Puftoss",
            10 => "Doga 2",
            11 => "Army 2",
            12 => "Kutout",
            _ => throw new InvalidOperationException($"Invalid BossValue: {BossValue}"),
        };
    }

    #region Object Overrides

    public bool Equals(BossSlotViewModel? other) => other is not null && _slotNumber == other._slotNumber;

    public override bool Equals(object? obj) => Equals(obj as BossSlotViewModel);

    public override int GetHashCode() => _slotNumber.GetHashCode();

    public override string ToString()
    {
        return $"Broadcast: The {(_slotNumber + 1).Ordinalize()} final boss is {GetBossName()}.";
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
                _endGameProgressionService.GauntletStateChanged -= OnBossStateChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnSpoilerDataChanged;
                _savedProgressProvider.ProgressChanged -= OnDataReset;
            }
            _disposed = true;
        }
    }

    ~BossSlotViewModel()
    {
        Dispose(false);
    }

    #endregion
}
