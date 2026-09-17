using System.Collections.ObjectModel;
using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events.Autotracking;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels;

public partial class CollectiblesGridRowViewModel : ObservableObject, IDisposable
{
    private readonly IUserSettingsService _userSettingsService;
    private readonly IAutotrackerService _autotrackerService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly ICollectiblesService _collectiblesService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly SynchronizationContext _uiContext;
    private readonly ObservableCollection<ObservableObject> _collectibles;

    #region Properties

    [ObservableProperty]
    public partial bool IsWrinklyVisible { get; set; }

    [ObservableProperty]
    public partial string WrinklyResourceKey { get; set; } = "";

    [ObservableProperty]
    public partial string HintResourceKey { get; set; } = "";

    [ObservableProperty]
    public partial int AmountToNextHint { get; set; } = -1;

    [ObservableProperty]
    public partial int HiddenCollectibleCount { get; set; } = -1;

    public int VisibleCount => 8 - HiddenCollectibleCount;

    public ObservableCollection<ObservableObject> Collectibles => _collectibles;

    #endregion

    public CollectiblesGridRowViewModel(
        IUserSettingsService userSettingsService,
        IAutotrackerService autotrackerService,
        ICollectiblesService collectiblesService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider
    )
    {
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _autotrackerService = autotrackerService ?? throw new ArgumentNullException(nameof(autotrackerService));
        _collectiblesService = collectiblesService ?? throw new ArgumentNullException(nameof(collectiblesService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null. This class must be instantiated on the UI thread.");

        _userSettingsService.PropertyChanged += OnUserSettingsChanged;
        _autotrackerService.HintProgressUpdated += OnHintProgressUpdated;
        _autotrackerService.ProgHintItemUpdated += OnProgHintItemUpdated;

        _collectibles = [];

        InitializeState();
    }

    private void InitializeState()
    {
        WrinklyResourceKey = "wrinkly";
        UpdateWrinklyVisibility();
        UpdateWrinklyHintType(ItemType.GOLDEN_BANANA);
        UpdateWrinklyCount(0);
        RefreshCollectibles();
    }

    #region Event Handling

    private void OnUserSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (
            e.PropertyName == nameof(IUserSettingsService.ShowTotalBPs) ||
            e.PropertyName == nameof(IUserSettingsService.ShowCompanyCoins)
        )
        {
            if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
            {
                _uiContext.Post(_ => RefreshCollectibles(), null);
            }
            else
            {
                RefreshCollectibles();
            }
        }
        else if (e.PropertyName == nameof(IUserSettingsService.ShowAmountForHints) )
        {
            if (_uiContext is not null && SynchronizationContext.Current != _uiContext) {
                _uiContext.Post(_ => UpdateWrinklyVisibility(), null);
            }
            else
            {
                UpdateWrinklyVisibility();
            }
        }
    }

    private void OnHintProgressUpdated(object? sender, AutotrackerHintEventArgs e)
    {
        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ => UpdateWrinklyCount(e.AmountToNextHint), null);
        }
        else
        {
            UpdateWrinklyCount(e.AmountToNextHint);
        }
    }

    private void OnProgHintItemUpdated(object? sender, AutotrackerProgHintItemEventArgs e)
    {
        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ => UpdateWrinklyHintType(e.ProgHintItem), null);
        }
        else
        {
            UpdateWrinklyHintType(e.ProgHintItem);
        }
    }

    #endregion

    private void UpdateWrinklyVisibility()
    {
        IsWrinklyVisible = _userSettingsService.ShowAmountForHints;
    }

    private void UpdateWrinklyHintType(ItemType itemType)
    {
        HintResourceKey = itemType switch
        {
            ItemType.TOTAL_BLUEPRINTS => "total_blueprints",
            ItemType.KEY => "basic_key",
            ItemType.BANANA_MEDAL => "medal",
            ItemType.BATTLE_CROWN => "crown",
            ItemType.FAIRY => "fairy",
            ItemType.RAINBOW_COIN => "rainbow_coin",
            ItemType.PEARL => "pearl",
            ItemType.COLORED_BANANA => "colored_banana",
            _ => "golden_banana"
        };
    }

    private void UpdateWrinklyCount(int count)
    {
        AmountToNextHint = count;
    }

    private void RefreshCollectibles()
    {
        _collectibles.Clear();
        int hiddenCollectibleCount = 0;

        if (_userSettingsService.ShowTotalBPs)
        {
            _collectibles.Add(new UiTotalBlueprintViewModel(
                _collectiblesService,
                _parsedSpoilerDataService,
                _savedProgressProvider,
                _userSettingsService
            ));
        }
        else
        {
            hiddenCollectibleCount++;
        }

        if (_userSettingsService.ShowCompanyCoins)
        {
            _collectibles.Add(new UiCountableViewModel(
                ItemType.COMPANY_COIN,
                _collectiblesService,
                _parsedSpoilerDataService,
                _userSettingsService
            ));
        }
        else
        {
            hiddenCollectibleCount++;
        }

        var itemTypes = new[]
        {
            ItemType.PEARL, ItemType.BATTLE_CROWN, ItemType.BANANA_MEDAL, ItemType.RAINBOW_COIN, ItemType.FAIRY, ItemType.GOLDEN_BANANA
        };
        foreach (var itemType in itemTypes)
        {
            _collectibles.Add(new UiCountableViewModel(
                itemType,
                _collectiblesService,
                _parsedSpoilerDataService,
                _userSettingsService
            ));
        }

        HiddenCollectibleCount = hiddenCollectibleCount;
        OnPropertyChanged(nameof(Collectibles));
        OnPropertyChanged(nameof(VisibleCount));
    }

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
                // Unsubscribe from events
                _userSettingsService.PropertyChanged -= OnUserSettingsChanged;
                _autotrackerService.HintProgressUpdated -= OnHintProgressUpdated;
                _autotrackerService.ProgHintItemUpdated -= OnProgHintItemUpdated;

                foreach (var vm in _collectibles.OfType<IDisposable>())
                {
                    vm.Dispose();
                }
            }
            _disposed = true;
        }
    }

    ~CollectiblesGridRowViewModel()
    {
        Dispose(false);
    }

    #endregion
}
