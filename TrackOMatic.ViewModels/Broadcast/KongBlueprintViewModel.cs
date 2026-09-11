using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public partial class KongBlueprintViewModel : ObservableObject, IEquatable<KongBlueprintViewModel>, IDisposable
{
    private readonly ICollectiblesService _collectiblesService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly SynchronizationContext _uiContext;
    private readonly ItemType _collectedType;
    private readonly ItemType _turnedInType;
    private int _collectedCount;
    private int _turnedInCount;

    private static readonly HashSet<ItemType> _allowedCollectedTypes =
    [
        ItemType.DONKEY_BLUEPRINT,
        ItemType.DIDDY_BLUEPRINT,
        ItemType.LANKY_BLUEPRINT,
        ItemType.TINY_BLUEPRINT,
        ItemType.CHUNKY_BLUEPRINT
    ];

    private static readonly HashSet<ItemType> _allowedTurnedInTypes =
    [
        ItemType.DONKEY_BLUEPRINT_TURNED,
        ItemType.DIDDY_BLUEPRINT_TURNED,
        ItemType.LANKY_BLUEPRINT_TURNED,
        ItemType.TINY_BLUEPRINT_TURNED,
        ItemType.CHUNKY_BLUEPRINT_TURNED
    ];


    #region Proxy Properties

    [ObservableProperty]
    public partial int Count { get; set; } = 0;

    [ObservableProperty]
    public partial string ImageResourceKey { get; set; } = "";

    #endregion

    public KongBlueprintViewModel(
        ItemType collectedType,
        ItemType turnedInType,
        ICollectiblesService collectiblesService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider
    )
    {
        if (!_allowedCollectedTypes.Contains(collectedType))
        {
            throw new ArgumentException($"ItemType {collectedType} is not allowed for collected blueprints.", nameof(collectedType));
        }
        if (!_allowedTurnedInTypes.Contains(turnedInType))
        {
            throw new ArgumentException($"ItemType {turnedInType} is not allowed for turned-in blueprints.", nameof(turnedInType));
        }
        _collectedType = collectedType;
        _turnedInType = turnedInType;
        _collectiblesService = collectiblesService ?? throw new ArgumentNullException(nameof(collectiblesService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null.");
        _collectiblesService.CollectibleChanged += OnCollectibleChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;
        _savedProgressProvider.ProgressChanged += OnSavedProgressChanged;
        InitializeState();
    }

    private void InitializeState()
    {
        _collectedCount = _collectiblesService.GetCollectibleCount(_collectedType);
        _turnedInCount = _collectiblesService.GetCollectibleCount(_turnedInType);
        Count = _collectedCount - _turnedInCount;
        UpdateImageResourceKey();
    }

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
                _collectiblesService.CollectibleChanged -= OnCollectibleChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnParsedSpoilerDataChanged;
                _savedProgressProvider.ProgressChanged -= OnSavedProgressChanged;
            }
            _disposed = true;
        }
    }

    ~KongBlueprintViewModel()
    {
        Dispose(false);
    }

    #endregion

    private void OnCollectibleChanged(object? sender, CollectiblesChangedEventArgs e)
    {
        if (e.ItemType != _collectedType && e.ItemType != _turnedInType)
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

    private void OnSavedProgressChanged(object? sender, ProgressReplacedEventArgs e) => InitializeState();

    private void OnParsedSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e) => InitializeState();

    protected virtual void UpdateImageResourceKey()
    {
        string baseKey = _collectedType.ToResourceKey();
        if (Count <= 0)
        {
            baseKey = $"{baseKey}_bw";
        }
        ImageResourceKey = baseKey;
    }

    #region Object Overrides

    public bool Equals(KongBlueprintViewModel? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return _collectedType == other._collectedType && _turnedInType == other._turnedInType;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_collectedType, _turnedInType);
    }

    public override string ToString()
    {
        return $"KongBlueprintViewModel: {Count} of the {_collectedType} blueprints on hand.";
    }

    #endregion
}
