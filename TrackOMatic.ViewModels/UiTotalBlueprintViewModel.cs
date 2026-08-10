using System.ComponentModel;
using System.Runtime.CompilerServices;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels;

public class UiTotalBlueprintViewModel: INotifyPropertyChanged, IDisposable
{
    private readonly ICollectiblesService _collectiblesService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IUserSettingsService _userSettingsService;
    private readonly SynchronizationContext _uiContext;

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

    public void IncrementCount()
    {
        if (!_userSettingsService.Autotracking)
        {
            var count = _collectiblesService.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS);
            _collectiblesService.SetCollectibleCount(ItemType.TOTAL_BLUEPRINTS, int.Min(count + 1, 40));
        }
    }

    public void DecrementCount()
    {
        if (!_userSettingsService.Autotracking)
        {
            var count = _collectiblesService.GetCollectibleCount(ItemType.TOTAL_BLUEPRINTS);
            _collectiblesService.SetCollectibleCount(ItemType.TOTAL_BLUEPRINTS, int.Max(count - 1, 0));
        }
    }

    #region Proxy Properties

    private int _count = 0;
    public int Count
    {
        get => _count;
        protected set
        {
            if (_count != value)
            {
                _count = value;
                OnPropertyChanged();
            }
        }
    }

    private string _imageResourceKey = "";
    public string ImageResourceKey
    {
        get => _imageResourceKey;
        protected set
        {
            if (_imageResourceKey != value)
            {
                _imageResourceKey = value;
                OnPropertyChanged();
            }
        }
    }

    #endregion

    #region INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
}
