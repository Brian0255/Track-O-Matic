using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

/// <summary>
/// A view model for displaying data about a specific numbered level in the broadcast view.
/// </summary>
public partial class LevelViewModel : IslesViewModel, IEquatable<LevelViewModel>, INotifyPropertyChanged
{
    #region Proxy Properties

    public KeyViewModel KeyViewModel { get; private init; }

    [ObservableProperty]
    public partial string NumberResourceKey { get; set; } = "";

    // Key-level image resource (for the level's key display)
    [ObservableProperty]
    public partial string ImageResourceKey { get; set; } = "";

    // Whether the level's key is starred
    [ObservableProperty]
    public partial bool IsStarred { get; set; } = false;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelViewModel"/> class with the specified services and selected level.
    /// </summary>
    /// <param name="selectedLevel"></param>
    /// <param name="itemTrackingService"></param>
    /// <param name="levelOrderService"></param>
    /// <param name="parsedSpoilerDataService"></param>
    public LevelViewModel(
        int selectedLevel,
        IItemTrackingService itemTrackingService,
        ILevelOrderService levelOrderService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IUserSettingsService userSettingsService,
        ISavedProgressProvider savedProgressProvider,
        IDataSaverService dataSaverService
    ) : base(selectedLevel, itemTrackingService, levelOrderService, parsedSpoilerDataService, userSettingsService, dataSaverService)
    {
        KeyViewModel = new(selectedLevel, itemTrackingService, parsedSpoilerDataService, savedProgressProvider, dataSaverService);

        // Initialize state from service (if level exists)
        InitializeState();
    }

    private void InitializeState()
    {
        NumberResourceKey = $"number_{_selectedLevel}";
        RegionResourceKey = "unknown_label";
        ShowRegionImage = true;

        // Initialize key display properties
        ImageResourceKey = "basic_key_bw";
        IsStarred = false;

        UpdateRegionData();
    }

    /// <summary>
    /// Override to filter item state changes to only those relevant to this level's key.
    /// </summary>
    protected override void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        // Only update if the changed item is the key for this level
        if (e is ItemStateChangedEventArgs itemChangeEvent)
        {
            var targetKeyName = $"KEY_{_selectedLevel}";
            if (Enum.TryParse<ItemName>(targetKeyName, out var targetKey))
            {
                // Only update region data if the changed item matches this level's key
                if (itemChangeEvent.UpdatedItem.ItemName == targetKey)
                {
                    // Update the key's display properties
                    UpdateImageResourceKey(itemChangeEvent.UpdatedItem, targetKey);
                    IsStarred = itemChangeEvent.UpdatedItem.Starred;
                    UpdateRegionData();
                }
            }
        }
    }

    protected override void UpdateRegionData()
    {
        RegionName targetRegion = RegionName.UNKNOWN;

        // If the spoiler data has level orders built-in, use those.
        if (_parsedSpoilerDataService.CurrentData?.HasLevelOrder == true)
        {
            var levelData = _parsedSpoilerDataService.CurrentData.LevelOrder.ToDictionary(x => x.Value, x => x.Key);
            targetRegion = levelData.TryGetValue(_selectedLevel, out RegionName value) ? value : RegionName.UNKNOWN;
        }
        else
        {
            var levelOrderIndex = _selectedLevel - 1;
            var levelOrder = _levelOrderService.GetLevelOrder();
            var regionValue = levelOrder.ToList().IndexOf(_selectedLevel);

            // var regionValue = levelOrder[levelOrderIndex];
            targetRegion = (regionValue < 0) ? RegionName.UNKNOWN : (RegionName)(regionValue + 2);
        }
        RegionResourceKey = targetRegion == RegionName.UNKNOWN ? "unknown_label" : targetRegion.ToString().ToLowerInvariant() + "_label";
        UpdatePointsData(targetRegion);
    }

    protected override void UpdatePointsData(RegionName regionName)
    {
        if (regionName == RegionName.UNKNOWN)
        {
            ShowRemainingPoints = false;
            Points = -1;
            PointColorResource = "RegionInProgress";
            return;
        }
        base.UpdatePointsData(regionName);
    }

    private void UpdateImageResourceKey(SavedItem? item, ItemName itemName)
    {
        string baseKey = "basic_key";
        RegionName spoiledRegion = RegionName.UNKNOWN;
        if (_parsedSpoilerDataService.CurrentData?.StartingItems.ContainsKey(itemName) == true)
        {
            spoiledRegion = _parsedSpoilerDataService.CurrentData.StartingItems[itemName];
        }

        // If no item state, show default B&W variant
        if (item == null && spoiledRegion == RegionName.UNKNOWN)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }

        // If item is hinted, always show B&W
        if (item?.Hinted == true)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }

        // If the item is user hinted (opacity not 1, always show B&W)
        if (item is not null && item.Opacity < 1.0)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }

        // If item is in a valid region, show full-color
        if (spoiledRegion != RegionName.UNKNOWN || EndGameMappings.ValidMoveRegions.Contains(item?.Region ?? RegionName.UNKNOWN))
        {
            ImageResourceKey = baseKey;
            return;
        }

        // Otherwise, item is in ItemGrid or unknown region, show B&W
        ImageResourceKey = $"{baseKey}_bw";
    }

    #region Object Overrides

    public bool Equals(LevelViewModel? other) => other is not null && other._selectedLevel == _selectedLevel;

    public override bool Equals(object? obj) => Equals(obj as LevelViewModel);

    public override int GetHashCode() => _selectedLevel.GetHashCode();

    #endregion
}
