using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Regions;

public partial class RegionLevelViewModel : ObservableObject, IEquatable<RegionLevelViewModel>, IDisposable
{
    private readonly RegionName _regionName;
    private readonly ILevelOrderService _levelOrderService;
    private readonly IUserSettingsService _userSettingsService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IDataSaverService _dataSaverService;

    #region Proxy Properties

    [ObservableProperty]
    public partial int LevelOrderNumber { get; set; }

    [ObservableProperty]
    public partial string LevelText { get; set; } = "?";

    #endregion

    public RegionLevelViewModel(
        RegionName regionName,
        ILevelOrderService levelOrderService,
        IUserSettingsService userSettingsService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IDataSaverService dataSaverService
    )
    {
        _regionName = regionName;
        _levelOrderService = levelOrderService ?? throw new ArgumentNullException(nameof(levelOrderService));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _dataSaverService = dataSaverService ?? throw new ArgumentNullException(nameof(dataSaverService));
        // Subscribe to level order changes
        _levelOrderService.LevelOrderChanged += OnLevelOrderChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnLevelOrderChanged;
        _savedProgressProvider.ProgressChanged += OnLevelOrderChanged;
        _dataSaverService.DataLoadingCompleted += OnLevelOrderChanged;
        // Initialize the level order number
        UpdateLevelOrderNumber();
    }

    #region Commands

    [RelayCommand(CanExecute = nameof(CanChangeLevelOrder))]
    private void IncrementLevelOrder()
    {
        var levelOrders = _levelOrderService.GetLevelOrder().ToList();
        var index = ToLevelOrderIndex();
        int maxSelectableLevelOrder = _userSettingsService.HelmInLevelOrder ? 8 : 7;
        if (index >= 0 && index < levelOrders.Count)
        {
            var currentNumber = levelOrders[index];
            levelOrders[index] = (currentNumber + 1) % (maxSelectableLevelOrder + 1);
            _levelOrderService.SetLevelOrder(levelOrders);
        }
    }

    [RelayCommand(CanExecute = nameof(CanChangeLevelOrder))]
    private void DecrementLevelOrder()
    {
        var levelOrders = _levelOrderService.GetLevelOrder().ToList();
        var index = ToLevelOrderIndex();
        int maxSelectableLevelOrder = _userSettingsService.HelmInLevelOrder ? 8 : 7;
        if (index >= 0 && index < levelOrders.Count)
        {
            var currentNumber = levelOrders[index];
            levelOrders[index] = (currentNumber + maxSelectableLevelOrder) % (maxSelectableLevelOrder + 1);
            _levelOrderService.SetLevelOrder(levelOrders);
        }
    }

    [RelayCommand(CanExecute = nameof(CanHandleMouseWheel))]
    private void HandleMouseWheel(int delta)
    {
        if (delta > 0)
        {
            IncrementLevelOrder();
        }
        else if (delta < 0)
        {
            DecrementLevelOrder();
        }
    }

    private bool CanChangeLevelOrder()
    {
        if (_parsedSpoilerDataService.CurrentData?.HasLevelOrder == false)
        {
            return false;
        }
        if (_regionName == RegionName.HIDEOUT_HELM && LevelOrderNumber == 8 && !_userSettingsService.HelmInLevelOrder)
        {
            return false;
        }
        return true;
    }

    private bool CanHandleMouseWheel(int _) => CanChangeLevelOrder();

    #endregion

    internal int ToLevelOrderIndex()
    {
        return (int)_regionName - 2;
    }

    #region Event Methods

    private void OnLevelOrderChanged(object? sender, EventArgs e)
    {
        UpdateLevelOrderNumber();
        UpdateLevelText();
    }

    private void UpdateLevelOrderNumber()
    {
        var spoiledLevelOrder = _parsedSpoilerDataService.CurrentData?.LevelOrder ?? [];
        if (spoiledLevelOrder.TryGetValue(_regionName, out int index))
        {
            LevelOrderNumber = index;
        }
        else
        {
            var levelOrders = _levelOrderService.GetLevelOrder();
            index = ToLevelOrderIndex();
            LevelOrderNumber = (index >= 0 && index < levelOrders.Count) ? levelOrders[index] : 0;
        }

        // If Hideout Helm is not in level order (setting disabled), force it to level 8
        if (_regionName == RegionName.HIDEOUT_HELM && !_userSettingsService.HelmInLevelOrder)
        {
            LevelOrderNumber = 8;
        }

        UpdateLevelText();
    }

    private void UpdateLevelText()
    {
        LevelText = LevelOrderNumber switch
        {
            1 => "1",
            2 => "2",
            3 => "3",
            4 => "4",
            5 => "5",
            6 => "6",
            7 => "7",
            8 => "8",
            _ => "?"
        }
    ;
    }

    #endregion

    #region Object/Equality Overrides

    public bool Equals(RegionLevelViewModel? other) => other is not null && _regionName == other._regionName;

    public override bool Equals(object? obj) => Equals(obj as RegionLevelViewModel);

    public override int GetHashCode() => _regionName.GetHashCode();

    public override string ToString() => $"{_regionName} at Level {LevelOrderNumber}";

    #endregion

    #region Operator Overloads

    public static bool operator ==(RegionLevelViewModel? left, RegionLevelViewModel? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(RegionLevelViewModel? left, RegionLevelViewModel? right) => !(left == right);

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
                // Unsubscribe from events
                _levelOrderService.LevelOrderChanged -= OnLevelOrderChanged;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnLevelOrderChanged;
                _savedProgressProvider.ProgressChanged -= OnLevelOrderChanged;
                _dataSaverService.DataLoadingCompleted -= OnLevelOrderChanged;
            }
            _disposed = true;
        }
    }

    ~RegionLevelViewModel()
    {
        Dispose(false);
    }

    #endregion

}
