using System.ComponentModel;
using System.Runtime.CompilerServices;

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
public class IslesViewModel : ObservableObject, IDisposable
{
    protected readonly IItemTrackingService _itemTrackingService;
    protected readonly ILevelOrderService _levelOrderService;
    protected readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    protected readonly IUserSettingsService _userSettingsService;
    protected readonly int _selectedLevel;

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
        IUserSettingsService userSettingsService

    ) : this(9, itemTrackingService, levelOrderService, parsedSpoilerDataService, userSettingsService)
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
        IUserSettingsService userSettingsService
    )
    {
        _selectedLevel = selectedLevel;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _levelOrderService = levelOrderService ?? throw new ArgumentNullException(nameof(levelOrderService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _levelOrderService.LevelOrderChanged += OnDataReset;
        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnDataReset;
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

    #region Proxy Properties

    private int _levelNumber = int.MinValue;
    public int LevelNumber
    {
        get => _levelNumber;
        protected set => SetProperty(ref _levelNumber, value);
    }

    private string _keyResourceKey = "";
    public string RegionResourceKey
    {
        get => _keyResourceKey;
        protected set => SetProperty(ref _keyResourceKey, value);
    }

    private bool _showRegionImage = false;
    public bool ShowRegionImage
    {
        get => _showRegionImage;
        protected set => SetProperty(ref _showRegionImage, value);
    }

    private bool _showRemainingPoints = false;
    public bool ShowRemainingPoints
    {
        get => _showRemainingPoints;
        protected set => SetProperty(ref _showRemainingPoints, value);
    }

    private string _pointsText = "";
    public string PointsText
    {
        get => _pointsText;
        protected set => SetProperty(ref _pointsText, value);
    }

    private string _pointColorResource = "RegionInProgress";
    public string PointColorResource
    {
        get => _pointColorResource;
        protected set => SetProperty(ref _pointColorResource, value);
    }

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

    /// <summary>
    /// Handles the event when the data is reset, updating the region data accordingly.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnDataReset(object? sender, EventArgs e)
    {
        UpdateRegionData();
    }

    /// <summary>
    /// Handles the event when an item's state changes, updating the region data accordingly.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        UpdateRegionData();
    }

    /// <summary>
    /// Handles the event when the user changes their settings, updating specifically when the
    /// number label should change from hoard points to item points or vice~versa.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnUserSettingsChanged(object? sender, PropertyChangedEventArgs e)
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
        var pointSetting = _userSettingsService.BroadcastNumberLabel;
        if (pointSetting == BroadcastNumberLabel.WothCount)
        {
            var hoardData = GetHoardPointsData(regionName);
            ShowRegionImage = hoardData.Item1;
            ShowRemainingPoints = hoardData.Item2;
            PointsText = hoardData.Item3.ToString();
            PointColorResource = hoardData.Item4;
        }
        else
        {
            var itemData = GetItemPointsData(regionName);
            ShowRegionImage = itemData.Item1;
            ShowRemainingPoints = itemData.Item2;
            PointsText = itemData.Item3.ToString();
            PointColorResource = itemData.Item4;
        }
    }

    private (bool, bool, int, string) GetHoardPointsData(RegionName regionName)
    {
        var maxHoardPoints = _parsedSpoilerDataService.GetWothPointsForRegion(regionName);
        if (maxHoardPoints < 0)
        {
            return (regionName != RegionName.DK_ISLES, false, 0, "RequiredChecksColor");
        }

        var itemsInRegion = _itemTrackingService.GetItemsInRegion(regionName);
        var hoardPoints = itemsInRegion.Sum(i => i.IsStarred ? 1 : 0);
        var points = maxHoardPoints - hoardPoints;
        return (true, true, points, "RequiredChecksColor");
    }

    private (bool, bool,int, string) GetItemPointsData(RegionName regionName)
    {
        var pointSpread = _parsedSpoilerDataService.GetPointSpread();
        if (pointSpread.Count == 0)
        {
            return (regionName != RegionName.DK_ISLES, false, 0, "RegionInProgress");
        }

        var regionPoints = _parsedSpoilerDataService.GetPointsForRegion(regionName);
        var itemsInRegion = _itemTrackingService.GetItemsInRegion(regionName);
        var categories = itemsInRegion.Select(i => i.ItemName.ToPointCategory());
        var points = regionPoints - categories.Sum(c => pointSpread[c]);
        var colorResource = points > 0 ? "RegionInProgress" : "RegionComplete";
        return (true, true, points, colorResource);
    }

    public override bool Equals(object? obj) => obj is IslesViewModel other && other._selectedLevel == _selectedLevel;

    public override int GetHashCode() => _selectedLevel.GetHashCode();
}
