using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels;

public partial class UiTotalBlueprintViewModel: ObservableObject, IDisposable
{
    private readonly ICollectiblesService _collectiblesService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IUserSettingsService _userSettingsService;
    private readonly SynchronizationContext _uiContext;


    #region Proxy Properties

    [ObservableProperty]
    public partial int Count { get; set; } = 0;

    [ObservableProperty]
    public partial string ImageResourceKey { get; set; } = "";

    #endregion

    public UiTotalBlueprintViewModel(
        ICollectiblesService collectiblesService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IUserSettingsService userSettingsService
    )
    {
        _collectiblesService = collectiblesService ?? throw new ArgumentNullException(nameof(collectiblesService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null. This class must be instantiated on the UI thread.");
        _collectiblesService.CollectibleChanged += OnCollectableChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnDataReset;
        _savedProgressProvider.ProgressChanged += OnDataReset;
        InitializeState();
    }

    private void InitializeState()
    {
        Count = _collectiblesService.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS);
        UpdateImageResourceKey();
    }

    #region Commands

    [RelayCommand(CanExecute = nameof(CanChangeCount))]
    private void IncrementCount()
    {
        var count = _collectiblesService.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS);
        _collectiblesService.SetCollectibleCount(ItemType.TOTAL_BLUEPRINTS, int.Min(count + 1, 40));
    }

    [RelayCommand(CanExecute = nameof(CanChangeCount))]
    private void DecrementCount()
    {
        var count = _collectiblesService.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS);
        _collectiblesService.SetCollectibleCount(ItemType.TOTAL_BLUEPRINTS, int.Max(count - 1, 0));
    }

    private bool CanChangeCount()
    {
        return !_userSettingsService.Autotracking;
    }

    #endregion

    #region IDisposable Implementation

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
                _collectiblesService.CollectibleChanged -= OnCollectableChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnDataReset;
                _savedProgressProvider.ProgressChanged -= OnDataReset;
            }
            _disposed = true;
        }
    }

    ~UiTotalBlueprintViewModel()
    {
        Dispose(false);
    }

    #endregion

    private void OnDataReset(object? sender, EventArgs e) => InitializeState();

    private void OnCollectableChanged(object? sender, CollectiblesChangedEventArgs e)
    {
        if (e.ItemType != ItemType.TOTAL_BLUEPRINTS)
        {
            return;
        }
        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ => InitializeState(), null);
        }
        else
        {
            InitializeState();
        }
    }

    private void UpdateImageResourceKey()
    {
        string baseKey = ItemType.TOTAL_BLUEPRINTS.ToResourceKey();
        if (Count <= 0)
        {
            baseKey = $"{baseKey}_bw";
        }
        ImageResourceKey = baseKey;
    }

    #region Object Overrides

    public override string ToString()
    {
        return $"Total blueprints: {Count}";
    }

    #endregion
}
