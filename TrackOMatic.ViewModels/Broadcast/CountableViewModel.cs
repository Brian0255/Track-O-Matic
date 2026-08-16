using System.ComponentModel;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

public partial class CountableViewModel : ObservableObject, IDisposable
{
    private readonly ICollectiblesService _collectiblesService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly SynchronizationContext _uiContext;
    private readonly ItemType _itemType;

    private static readonly HashSet<ItemType> AllowedItemTypes =
    [
        ItemType.GOLDEN_BANANA,
        ItemType.PEARL,
        ItemType.BANANA_MEDAL,
        ItemType.FAIRY,
        ItemType.RAINBOW_COIN,
        ItemType.BATTLE_CROWN,
        ItemType.COMPANY_COIN
    ];

    public CountableViewModel(
        ItemType itemType,
        ICollectiblesService collectiblesService,
        IParsedSpoilerDataService parsedSpoilerDataService
    )
    {
        if (!AllowedItemTypes.Contains(itemType))
        {
            throw new ArgumentException($"ItemType {itemType} is not allowed for CountableViewModel.", nameof(itemType));
        }
        _itemType = itemType;
        _collectiblesService = collectiblesService ?? throw new ArgumentNullException(nameof(collectiblesService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null.");

        _collectiblesService.CollectibleChanged += OnCollectibleChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;


        InitializeState();
    }

    private void InitializeState()
    {
        Count = _collectiblesService.GetCollectibleCount(_itemType);
        UpdateImageResourceKey();
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
                _collectiblesService.CollectibleChanged -= OnCollectibleChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnParsedSpoilerDataChanged;
            }
            _disposed = true;
        }
    }

    ~CountableViewModel()
    {
        Dispose(false);
    }

    #endregion

    protected virtual void OnCollectibleChanged(object? sender, CollectiblesChangedEventArgs e)
    {
        if (e.ItemType != _itemType)
        {
            return;
        }

        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ =>
            {
                HandleCollectibleChanged(e);
            }, null);
        }
        else
        {
            HandleCollectibleChanged(e);
        }
    }

    protected virtual void HandleCollectibleChanged(CollectiblesChangedEventArgs e)
    {
        if (e.ItemType != _itemType)
        {
            return;
        }
        Count = e.NewValue;
        UpdateImageResourceKey();
    }

    protected virtual void OnParsedSpoilerDataChanged(object? sender, ParsedSpoilerDataChangedEventArgs e) => InitializeState();

    protected virtual void UpdateImageResourceKey()
    {
        string baseKey = _itemType.ToResourceKey();
        if (Count <= 0)
        {
            baseKey = $"{baseKey}_bw";
        }
        ImageResourceKey = baseKey;
    }
}
