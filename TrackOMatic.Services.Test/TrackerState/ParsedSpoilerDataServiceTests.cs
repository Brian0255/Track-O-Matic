using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services.Test.TrackerState;

/// <summary>
/// Unit tests for ParsedSpoilerDataService.
/// Tests spoiler data state management, event batching, and data queries.
/// </summary>
public class ParsedSpoilerDataServiceTests
{
    private readonly SavedProgress _progress;
    private readonly TestSavedProgressProvider _provider;
    private readonly ParsedSpoilerDataService _sut;

    public ParsedSpoilerDataServiceTests()
    {
        _progress = new SavedProgress();
        _provider = new TestSavedProgressProvider(_progress);
        _sut = new ParsedSpoilerDataService(_provider);
    }

    #region Initialization Tests

    [Fact]
    public void Constructor_InitializesWithNullData()
    {
        // Act
        var currentData = _sut.CurrentData;

        // Assert
        Assert.Null(currentData);
    }

    [Fact]
    public void Constructor_WithNullProvider_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ParsedSpoilerDataService(null!));
    }

    #endregion

    #region UpdateParsedData Tests - Basic Behavior

    [Fact]
    public void UpdateParsedData_WithNewData_UpdatesCurrentData()
    {
        // Arrange
        var newData = new ParsedSpoilerData { LevelOrder = [] };

        // Act
        _sut.UpdateParsedData(newData);

        // Assert
        Assert.NotNull(_sut.CurrentData);
        Assert.Same(newData, _sut.CurrentData);
    }

    [Fact]
    public void UpdateParsedData_WithNull_ClearsCurrentData()
    {
        // Arrange
        var initialData = new ParsedSpoilerData { LevelOrder = [] };
        _sut.UpdateParsedData(initialData);
        Assert.NotNull(_sut.CurrentData);

        // Act
        _sut.UpdateParsedData(null);

        // Assert
        Assert.Null(_sut.CurrentData);
    }

    #endregion

    #region UpdateParsedData Tests - Event Firing

    [Fact]
    public void UpdateParsedData_WithNewData_RaisesEvent()
    {
        // Arrange
        var newData = new ParsedSpoilerData { LevelOrder = [] };
        var eventFired = false;
        ParsedSpoilerDataChangedEventArgs? eventArgs = null;

        _sut.ParsedSpoilerDataChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        // Act
        _sut.UpdateParsedData(newData);

        // Assert
        Assert.True(eventFired);
        Assert.NotNull(eventArgs);
        Assert.Null(eventArgs.OldData);
        Assert.Same(newData, eventArgs.NewData);
    }

    [Fact]
    public void UpdateParsedData_WithNull_RaisesEvent()
    {
        // Arrange
        var initialData = new ParsedSpoilerData { LevelOrder = [] };
        _sut.UpdateParsedData(initialData);

        var eventFired = false;
        ParsedSpoilerDataChangedEventArgs? eventArgs = null;

        _sut.ParsedSpoilerDataChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        // Act
        _sut.UpdateParsedData(null);

        // Assert
        Assert.True(eventFired);
        Assert.NotNull(eventArgs);
        Assert.Same(initialData, eventArgs.OldData);
        Assert.Null(eventArgs.NewData);
    }

    [Fact]
    public void UpdateParsedData_SequentialUpdates_FiresMultipleEvents()
    {
        // Arrange
        var data1 = new ParsedSpoilerData { LevelOrder = [] };
        var data2 = new ParsedSpoilerData { LevelOrder = [] };
        var data3 = new ParsedSpoilerData { LevelOrder = [] };
        var eventCount = 0;

        _sut.ParsedSpoilerDataChanged += (sender, args) => eventCount++;

        // Act
        _sut.UpdateParsedData(data1);
        _sut.UpdateParsedData(data2);
        _sut.UpdateParsedData(data3);

        // Assert
        Assert.Equal(3, eventCount);
    }

    #endregion

    #region LoadParsedDataSilently Tests

    [Fact]
    public void LoadParsedDataSilently_WithNewData_UpdatesWithoutEvent()
    {
        // Arrange
        var newData = new ParsedSpoilerData { LevelOrder = [] };
        var eventFired = false;

        _sut.ParsedSpoilerDataChanged += (sender, args) => eventFired = true;

        // Act
        _sut.LoadParsedDataSilently(newData);

        // Assert
        Assert.False(eventFired);
        Assert.Same(newData, _sut.CurrentData);
    }

    [Fact]
    public void LoadParsedDataSilently_WithNull_UpdatesWithoutEvent()
    {
        // Arrange
        var initialData = new ParsedSpoilerData { LevelOrder = [] };
        _sut.UpdateParsedData(initialData);

        var eventFired = false;
        _sut.ParsedSpoilerDataChanged += (sender, args) => eventFired = true;

        // Act
        _sut.LoadParsedDataSilently(null);

        // Assert
        Assert.False(eventFired);
        Assert.Null(_sut.CurrentData);
    }

    #endregion

    #region Batch Update Tests

    [Fact]
    public void BeginBatchUpdate_MultipleUpdates_FiresSingleEvent()
    {
        // Arrange
        var data1 = new ParsedSpoilerData { LevelOrder = [] };
        var data2 = new ParsedSpoilerData { LevelOrder = [] };
        var eventFiredCount = 0;

        _sut.ParsedSpoilerDataChanged += (sender, args) => eventFiredCount++;

        // Act
        using (var batch = _sut.BeginBatchUpdate())
        {
            _sut.UpdateParsedData(data1);
            Assert.Equal(0, eventFiredCount); // No event yet
            _sut.UpdateParsedData(data2);
            Assert.Equal(0, eventFiredCount); // Still no event
        }

        // Assert
        Assert.Equal(1, eventFiredCount); // Single event after batch ends
    }

    [Fact]
    public void BeginBatchUpdate_Dispose_FiresEventWithFinalState()
    {
        // Arrange
        var initialData = new ParsedSpoilerData { LevelOrder = [] };
        _sut.UpdateParsedData(initialData);

        var data1 = new ParsedSpoilerData { LevelOrder = [] };
        var data2 = new ParsedSpoilerData { LevelOrder = [] };
        ParsedSpoilerDataChangedEventArgs? eventArgs = null;

        _sut.ParsedSpoilerDataChanged += (sender, args) => eventArgs = args;

        // Act
        using (var batch = _sut.BeginBatchUpdate())
        {
            _sut.UpdateParsedData(data1);
            _sut.UpdateParsedData(data2);
        }

        // Assert
        Assert.NotNull(eventArgs);
        // Note: _deferredOldData is overwritten on each update, so it captures the last oldData (data1)
        Assert.Same(data1, eventArgs.OldData);
        Assert.Same(data2, eventArgs.NewData);
    }

    [Fact]
    public void BeginBatchUpdate_NoUpdatesDuringBatch_NoEvent()
    {
        // Arrange
        var eventFired = false;
        _sut.ParsedSpoilerDataChanged += (sender, args) => eventFired = true;

        // Act
        using (var batch = _sut.BeginBatchUpdate())
        {
            // No updates during batch
        }

        // Assert
        Assert.False(eventFired);
    }

    [Fact]
    public void BeginBatchUpdate_NestedDisposable_CanBeDisposedMultipleTimes()
    {
        // Arrange
        var data = new ParsedSpoilerData { LevelOrder = [] };
        var eventFiredCount = 0;
        _sut.ParsedSpoilerDataChanged += (sender, args) => eventFiredCount++;

        // Act
        var batch = _sut.BeginBatchUpdate();
        _sut.UpdateParsedData(data);
        batch.Dispose();
        batch.Dispose(); // Second dispose should not cause issues

        // Assert
        Assert.Equal(1, eventFiredCount);
    }

    [Fact]
    public void BeginBatchUpdate_OnProgressChanged_DeferredUntilBatchEnd()
    {
        // Arrange
        var initialData = new ParsedSpoilerData { LevelOrder = [] };
        _sut.UpdateParsedData(initialData);

        var intermediateData = new ParsedSpoilerData { LevelOrder = [] };
        var eventFiredCount = 0;

        _sut.ParsedSpoilerDataChanged += (sender, args) => eventFiredCount++;

        // Act
        using (var batch = _sut.BeginBatchUpdate())
        {
            _sut.UpdateParsedData(intermediateData);
            // Simulate progress change (which would normally clear data)
            _provider.UpdateProgress(new SavedProgress());
            Assert.Equal(0, eventFiredCount); // Event deferred
        }

        // Assert
        Assert.Equal(1, eventFiredCount); // Single event after batch
    }

    #endregion

    #region GetPointsForItem Tests

    [Fact]
    public void GetPointsForItem_WithPointData_ReturnsCorrectPoints()
    {
        // Arrange
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            PointSpread = new()
            {
                { PointCategory.Kong, 100 },
                { PointCategory.Key, 50 }
            }
        };
        _sut.UpdateParsedData(data);

        // Act
        var points = _sut.GetPointsForItem(ItemName.DONKEY);

        // Assert
        Assert.Equal(100, points);
    }

    [Fact]
    public void GetPointsForItem_WithNoData_ReturnsZero()
    {
        // Act
        var points = _sut.GetPointsForItem(ItemName.DONKEY);

        // Assert
        Assert.Equal(0, points);
    }

    [Fact]
    public void GetPointsForItem_WithEmptyPointSpread_ReturnsZero()
    {
        // Arrange
        var data = new ParsedSpoilerData { LevelOrder = [], PointSpread = [] };
        _sut.UpdateParsedData(data);

        // Act
        var points = _sut.GetPointsForItem(ItemName.DONKEY);

        // Assert
        Assert.Equal(0, points);
    }

    #endregion

    #region GetPointsForRegion Tests

    [Fact]
    public void GetPointsForRegion_WithRegionData_ReturnsCorrectPoints()
    {
        // Arrange
        var regionData = new RegionSpoilerData { Points = 250 };
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new()
            {
                { RegionName.JUNGLE_JAPES, regionData }
            }
        };
        _sut.UpdateParsedData(data);

        // Act
        var points = _sut.GetPointsForRegion(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(250, points);
    }

    [Fact]
    public void GetPointsForRegion_WithNoData_ReturnsIntMinValue()
    {
        // Act
        var points = _sut.GetPointsForRegion(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(int.MinValue, points);
    }

    [Fact]
    public void GetPointsForRegion_WithUnknownRegion_ReturnsZero()
    {
        // Arrange
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new()
            {
                { RegionName.JUNGLE_JAPES, new RegionSpoilerData { Points = 100 } }
            }
        };
        _sut.UpdateParsedData(data);

        // Act
        var points = _sut.GetPointsForRegion(RegionName.ANGRY_AZTEC);

        // Assert
        Assert.Equal(0, points);
    }

    #endregion

    #region GetPointSpread Tests

    [Fact]
    public void GetPointSpread_WithData_ReturnsPointSpread()
    {
        // Arrange
        var pointSpread = new Dictionary<PointCategory, int>
        {
            { PointCategory.Kong, 100 },
            { PointCategory.Key, 50 }
        };
        var data = new ParsedSpoilerData { LevelOrder = [], PointSpread = pointSpread };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.GetPointSpread();

        // Assert
        Assert.Equal(pointSpread.Count, result.Count);
        Assert.Equal(100, result[PointCategory.Kong]);
        Assert.Equal(50, result[PointCategory.Key]);
    }

    [Fact]
    public void GetPointSpread_WithNoData_ReturnsEmptyDictionary()
    {
        // Act
        var result = _sut.GetPointSpread();

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetWothPointsForRegion Tests

    [Fact]
    public void GetWothPointsForRegion_WithRegionData_ReturnsCorrectWothPoints()
    {
        // Arrange
        var regionData = new RegionSpoilerData { WothCount = 150 };
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new()
            {
                { RegionName.JUNGLE_JAPES, regionData }
            }
        };
        _sut.UpdateParsedData(data);

        // Act
        var points = _sut.GetWothPointsForRegion(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(150, points);
    }

    [Fact]
    public void GetWothPointsForRegion_WithNoData_ReturnsIntMinValue()
    {
        // Act
        var points = _sut.GetWothPointsForRegion(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(int.MinValue, points);
    }

    [Fact]
    public void GetWothPointsForRegion_WithUnknownRegion_ReturnsIntMinValue()
    {
        // Arrange
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new()
            {
                { RegionName.JUNGLE_JAPES, new RegionSpoilerData { WothCount = 100 } }
            }
        };
        _sut.UpdateParsedData(data);

        // Act
        var points = _sut.GetWothPointsForRegion(RegionName.ANGRY_AZTEC);

        // Assert
        Assert.Equal(int.MinValue, points);
    }

    #endregion

    #region HasItemPoints Tests

    [Fact]
    public void HasItemPoints_WithItemPoints_ReturnsTrue()
    {
        // Arrange
        var regionData = new RegionSpoilerData { Points = 100 };
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new()
            {
                { RegionName.JUNGLE_JAPES, regionData }
            }
        };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.HasItemPoints();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasItemPoints_WithNoItemPoints_ReturnsFalse()
    {
        // Arrange
        var regionData = new RegionSpoilerData { Points = 0 };
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new()
            {
                { RegionName.JUNGLE_JAPES, regionData }
            }
        };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.HasItemPoints();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasItemPoints_WithNoData_ReturnsFalse()
    {
        // Act
        var result = _sut.HasItemPoints();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region HasHoardPoints Tests

    [Fact]
    public void HasHoardPoints_WithHoardPoints_ReturnsTrue()
    {
        // Arrange
        var regionData = new RegionSpoilerData { WothCount = 50 };
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new()
            {
                { RegionName.JUNGLE_JAPES, regionData }
            }
        };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.HasHoardPoints();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasHoardPoints_WithNoHoardPoints_ReturnsFalse()
    {
        // Arrange
        var regionData = new RegionSpoilerData { WothCount = 0 };
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new()
            {
                { RegionName.JUNGLE_JAPES, regionData }
            }
        };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.HasHoardPoints();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasHoardPoints_WithNoData_ReturnsFalse()
    {
        // Act
        var result = _sut.HasHoardPoints();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetStartingItems Tests

    [Fact]
    public void GetStartingItems_WithStartingItems_ReturnsItems()
    {
        // Arrange
        var startingItems = new Dictionary<ItemName, RegionName>
        {
            { ItemName.DONKEY, RegionName.START }
        };
        var data = new ParsedSpoilerData { LevelOrder = [], StartingItems = startingItems };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.GetStartingItems();

        // Assert
        Assert.Single(result);
        Assert.Equal(RegionName.START, result[ItemName.DONKEY]);
    }

    [Fact]
    public void GetStartingItems_WithNoData_ReturnsEmptyDictionary()
    {
        // Act
        var result = _sut.GetStartingItems();

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetVialColorsForRegion Tests

    [Fact]
    public void GetVialColorsForRegion_WithVialColors_ReturnsColors()
    {
        // Arrange
        var vialColors = new List<VialColor> { VialColor.RED, VialColor.BLUE };
        var regionData = new RegionSpoilerData { VialColors = vialColors };
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new()
            {
                { RegionName.JUNGLE_JAPES, regionData }
            }
        };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.GetVialColorsForRegion(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(VialColor.RED, result);
        Assert.Contains(VialColor.BLUE, result);
    }

    [Fact]
    public void GetVialColorsForRegion_WithNoColors_ReturnsEmpty()
    {
        // Arrange
        var regionData = new RegionSpoilerData { VialColors = [] };
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new()
            {
                { RegionName.JUNGLE_JAPES, regionData }
            }
        };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.GetVialColorsForRegion(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetVialColorsForRegion_WithUnknownRegion_ReturnsEmpty()
    {
        // Arrange
        var data = new ParsedSpoilerData { LevelOrder = [] };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.GetVialColorsForRegion(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetVialColorsForRegion_WithNoData_ReturnsEmpty()
    {
        // Act
        var result = _sut.GetVialColorsForRegion(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetSpoilerSettings Tests

    [Fact]
    public void GetSpoilerSettings_WithData_ReturnsSpoilerSettings()
    {
        // Arrange
        var settings = new SpoilerSettings();
        var data = new ParsedSpoilerData { LevelOrder = [], SpoilerSettings = settings };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.GetSpoilerSettings();

        // Assert
        Assert.Same(settings, result);
    }

    [Fact]
    public void GetSpoilerSettings_WithNoData_ReturnsNewSpoilerSettings()
    {
        // Act
        var result = _sut.GetSpoilerSettings();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<SpoilerSettings>(result);
    }

    #endregion

    #region HasHelmOrder Tests

    [Fact]
    public void HasHelmOrder_WithHelmOrder_ReturnsTrue()
    {
        // Arrange
        var data = new ParsedSpoilerData { LevelOrder = [], HelmOrder = [1, 2, 3] };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.HasHelmOrder();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasHelmOrder_WithEmptyHelmOrder_ReturnsFalse()
    {
        // Arrange
        var data = new ParsedSpoilerData { LevelOrder = [], HelmOrder = [] };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.HasHelmOrder();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasHelmOrder_WithNoData_ReturnsFalse()
    {
        // Act
        var result = _sut.HasHelmOrder();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region HasBossOrder Tests

    [Fact]
    public void HasBossOrder_WithBossOrder_ReturnsTrue()
    {
        // Arrange
        var data = new ParsedSpoilerData { LevelOrder = [], FinalBossOrder = [1, 2, 3] };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.HasBossOrder();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasBossOrder_WithEmptyBossOrder_ReturnsFalse()
    {
        // Arrange
        var data = new ParsedSpoilerData { LevelOrder = [], FinalBossOrder = [] };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.HasBossOrder();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasBossOrder_WithNoData_ReturnsFalse()
    {
        // Act
        var result = _sut.HasBossOrder();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region HasRegionBarrier Tests

    [Fact]
    public void HasRegionBarrier_WithBarrierInfo_ReturnsTrue()
    {
        // Arrange
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionBarrierInfo = [new BlockerInfoRaw { Item = 1, Cost = 5 }]
        };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.HasRegionBarrier();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasRegionBarrier_WithEmptyBarrierInfo_ReturnsFalse()
    {
        // Arrange
        var data = new ParsedSpoilerData { LevelOrder = [], RegionBarrierInfo = [] };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.HasRegionBarrier();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasRegionBarrier_WithNoData_ReturnsFalse()
    {
        // Act
        var result = _sut.HasRegionBarrier();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetBarrierInfo Tests

    [Fact]
    public void GetBarrierInfo_WithValidRegion_ReturnsBarrierInfo()
    {
        // Arrange
        var barrierInfo = new BlockerInfoRaw { Item = 1, Cost = 5 };
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionBarrierInfo = [barrierInfo]
        };
        _sut.UpdateParsedData(data);

        // Act
        var result = _sut.GetBarrierInfo(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(1, result.Item);
        Assert.Equal(5, result.Cost);
    }

    [Fact]
    public void GetBarrierInfo_WithNoData_ReturnsDefaultBlockerInfo()
    {
        // Act
        var result = _sut.GetBarrierInfo(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(0, result.Item);
        Assert.Equal(-1, result.Cost);
    }

    [Fact]
    public void GetBarrierInfo_WithInvalidRegionIndex_ReturnsDefaultBlockerInfo()
    {
        // Arrange
        var data = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionBarrierInfo = [new BlockerInfoRaw { Item = 1, Cost = 5 }]
        };
        _sut.UpdateParsedData(data);

        // Act - Try to access out-of-bounds region
        var result = _sut.GetBarrierInfo(RegionName.HIDEOUT_HELM);

        // Assert
        Assert.Equal(0, result.Item);
        Assert.Equal(-1, result.Cost);
    }

    #endregion

    #region ProgressChanged Event Tests

    [Fact]
    public void OnProgressChanged_WhenNotBatching_ClearsData()
    {
        // Arrange
        var initialData = new ParsedSpoilerData { LevelOrder = [] };
        _sut.UpdateParsedData(initialData);
        Assert.NotNull(_sut.CurrentData);

        var eventFired = false;
        _sut.ParsedSpoilerDataChanged += (sender, args) => eventFired = true;

        // Act
        var newProgress = new SavedProgress();
        _provider.UpdateProgress(newProgress);

        // Assert
        Assert.Null(_sut.CurrentData);
        Assert.True(eventFired);
    }

    [Fact]
    public void OnProgressChanged_WhenBatching_IgnoresProgressChange()
    {
        // Arrange
        var initialData = new ParsedSpoilerData { LevelOrder = [] };
        _sut.UpdateParsedData(initialData);

        var eventFiredCount = 0;
        _sut.ParsedSpoilerDataChanged += (sender, args) => eventFiredCount++;

        // Act
        using (var batch = _sut.BeginBatchUpdate())
        {
            var newProgress = new SavedProgress();
            _provider.UpdateProgress(newProgress);  // This should be ignored while batching
            Assert.Equal(0, eventFiredCount); // No event fired
        }

        // Assert
        // Still no event after exit, because OnProgressChanged was skipped while batching
        Assert.Equal(0, eventFiredCount);
        // Data should remain unchanged
        Assert.Same(initialData, _sut.CurrentData);
    }

    #endregion
}
