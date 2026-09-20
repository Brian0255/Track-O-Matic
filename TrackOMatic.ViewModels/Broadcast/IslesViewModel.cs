using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

/// <summary>
/// A view model for displaying data about the DK Isles region in the broadcast view.
/// </summary>
/// <remarks>This view model only changes based on point totals, but sub classes can watch for additional items.</remarks>
public partial class IslesViewModel : ObservableObject, IEquatable<IslesViewModel>, IDisposable
{
    protected readonly IItemTrackingService _itemTrackingService;
    protected readonly ILevelOrderService _levelOrderService;
    protected readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    protected readonly IUserSettingsService _userSettingsService;
    protected readonly IDataSaverService _dataSaverService;
    protected readonly int _selectedLevel;

    #region Proxy Properties

    [ObservableProperty]
    public partial int LevelNumber { get; set; } = int.MinValue;

    [ObservableProperty]
    public partial string RegionResourceKey { get; set; } = "";

    [ObservableProperty]
    public partial bool ShowRegionImage { get; set; } = false;

    [ObservableProperty]
    public partial bool ShowRemainingPoints { get; set; } = false;

    [ObservableProperty]
    public partial int Points { get; set; } = -1;

    [ObservableProperty]
    public partial string PointColorResource { get; set; } = "RegionInProgress";

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="IslesViewModel"/> class with the specified services.
    /// </summary>
    /// <param name="itemTrackingService"></param>
    /// <param name="levelOrderService"></param>
    /// <param name="parsedSpoilerDataService"></param>
    public IslesViewModel(
        IItemTrackingService itemTrackingService,
        ILevelOrderService levelOrderService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IUserSettingsService userSettingsService,
        IDataSaverService dataSaverService

    ) : this(9, itemTrackingService, levelOrderService, parsedSpoilerDataService, userSettingsService, dataSaverService)
    {
        _userSettingsService = userSettingsService;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IslesViewModel"/> class with the specified services and selected level.
    /// </summary>
    /// <remarks>This is meant to be called by the subclass only.</remarks>
    /// <param name="selectedLevel"></param>
    /// <param name="itemTrackingService"></param>
    /// <param name="levelOrderService"></param>
    /// <param name="parsedSpoilerDataService"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected internal IslesViewModel(
        int selectedLevel,
        IItemTrackingService itemTrackingService,
        ILevelOrderService levelOrderService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IUserSettingsService userSettingsService,
        IDataSaverService dataSaverService
    )
    {
        _selectedLevel = selectedLevel;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _levelOrderService = levelOrderService ?? throw new ArgumentNullException(nameof(levelOrderService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _dataSaverService = dataSaverService ?? throw new ArgumentNullException(nameof(dataSaverService));
        _levelOrderService.LevelOrderChanged += OnDataReset;
        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnDataReset;
        _dataSaverService.DataLoadingCompleted += OnDataReset;
        _userSettingsService.PropertyChanged += OnUserSettingsChanged;
        // Initialize state from service (if level exists)
        InitializeState();
    }

    private void InitializeState()
    {
        LevelNumber = _selectedLevel;
        RegionResourceKey = "dk_isles_label";
        ShowRegionImage = false;
        UpdateRegionData();
    }

    /// <summary>
    /// Handles the event when the data is reset, updating the region data accordingly.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    internal virtual void OnDataReset(object? sender, EventArgs e)
    {
        UpdateRegionData();
    }

    /// <summary>
    /// Handles the event when an item's state changes, updating the region data accordingly.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    internal virtual void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        UpdateRegionData();
    }

    /// <summary>
    /// Handles the event when the user changes their settings, updating specifically when the
    /// number label should change from hoard points to item points or vice~versa.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    internal virtual void OnUserSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IUserSettingsService.BroadcastNumberLabel))
        {
            UpdateRegionData();
        }
    }

    protected virtual void UpdateRegionData()
    {
        UpdatePointsData(RegionName.DK_ISLES);
    }

    protected virtual void UpdatePointsData(RegionName regionName)
    {
        var spoilerData = _parsedSpoilerDataService.CurrentData;

        if (spoilerData is null)
        {
            UpdateLabelsByNoPoints();
        }
        else if (spoilerData.SpoilerSettings.PointsEnabled && spoilerData.SpoilerSettings.WOTHEnabled)
        {
            Action<RegionName> updateLabel = _userSettingsService.BroadcastNumberLabel == BroadcastNumberLabel.WothCount
                ? UpdateLabelsByHoardPoints
                : UpdateLabelsByItemPoints;
            updateLabel(regionName);
        }
        else if (spoilerData.SpoilerSettings.WOTHEnabled)
        {
            UpdateLabelsByHoardPoints(regionName);
        }
        else if (spoilerData.SpoilerSettings.PointsEnabled)
        {
            UpdateLabelsByItemPoints(regionName);
        }
        else
        {
            UpdateLabelsByNoPoints();
        }
    }

    protected virtual void UpdateLabelsByNoPoints()
    {
        ShowRegionImage = false;
        ShowRemainingPoints = false;
        Points = -1;
        PointColorResource = "RegionInProgress";
    }

    protected virtual void UpdateLabelsByHoardPoints(RegionName regionName)
    {
        var maxHoardPoints = _parsedSpoilerDataService.GetWothPointsForRegion(regionName);
        var itemsInRegion = _itemTrackingService.GetItemsInRegion(regionName);
        var hoardPoints = itemsInRegion.Sum(i => i.Starred ? 1 : 0);
        var points = maxHoardPoints - hoardPoints;
        ShowRegionImage = true;
        ShowRemainingPoints = true;
        Points = points;
        PointColorResource = "RequiredChecksColor";
    }

    protected virtual void UpdateLabelsByItemPoints(RegionName regionName)
    {
        var pointSpread = _parsedSpoilerDataService.GetPointSpread();
        var regionPoints = _parsedSpoilerDataService.GetPointsForRegion(regionName);
        var itemsInRegion = _itemTrackingService.GetItemsInRegion(regionName);
        var categories = itemsInRegion.Select(i => i.ItemName.ToPointCategory());
        var points = regionPoints - categories.Sum(c => pointSpread[c]);
        var colorResource = points > 0 ? "RegionInProgress" : "RegionComplete";
        ShowRegionImage = true;
        ShowRemainingPoints = true;
        Points = points;
        PointColorResource = colorResource;
    }

    #region Object Overrides

    public bool Equals(IslesViewModel? other) => other is not null && other._selectedLevel == _selectedLevel;

    public override bool Equals(object? obj) => Equals(obj as IslesViewModel);

    public override int GetHashCode() => _selectedLevel.GetHashCode();

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
                _itemTrackingService.ItemStateChanged -= OnItemStateChanged;
                _levelOrderService.LevelOrderChanged -= OnDataReset;
                _parsedSpoilerDataService.ParsedSpoilerDataChanged -= OnDataReset;
            }
            _disposed = true;
        }
    }

    ~IslesViewModel()
    {
        Dispose(false);
    }

    #endregion
}
