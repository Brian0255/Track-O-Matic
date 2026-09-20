using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services.Test.TrackerState;

/// <summary>
/// Unit tests for BarrierService.
/// Tests B. Locker and Helm Door barrier state management.
/// </summary>
public class BarrierServiceTests
{
    private readonly SavedProgress _progress;
    private readonly TestSavedProgressProvider _provider;
    private readonly BarrierService _sut;

    public BarrierServiceTests()
    {
        _progress = new SavedProgress
        {
            SavedGBCounts = [],
            BLockerImageIndexes = [],
            HelmDoorCounts = ["?", "?"],
            HelmDoorImageIndexes = [0, 0]
        };
        _provider = new TestSavedProgressProvider(_progress);
        _sut = new BarrierService(_provider);
    }

    #region B. Locker Count Tests

    [Fact]
    public void GetBLockerCount_WithNoData_ReturnsQuestionMark()
    {
        // Act
        var count = _sut.GetBLockerCount(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal("?", count);
    }

    [Fact]
    public void GetBLockerCount_WithExistingData_ReturnsValue()
    {
        // Arrange
        _progress.SavedGBCounts[RegionName.ANGRY_AZTEC] = "5";

        // Act
        var count = _sut.GetBLockerCount(RegionName.ANGRY_AZTEC);

        // Assert
        Assert.Equal("5", count);
    }

    [Fact]
    public void SetBLockerCount_UpdatesValue()
    {
        // Act
        _sut.SetBLockerCount(RegionName.JUNGLE_JAPES, "7");

        // Assert
        Assert.Equal("7", _sut.GetBLockerCount(RegionName.JUNGLE_JAPES));
    }

    [Fact]
    public void SetBLockerCount_RaisesEvent()
    {
        // Arrange
        var eventFired = false;
        BlockerBarrierAmountChangedEventArgs? eventArgs = null;

        _sut.BlockerBarrierAmountChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        // Act
        _sut.SetBLockerCount(RegionName.JUNGLE_JAPES, "10");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(eventFired);
            Assert.NotNull(eventArgs);
            Assert.Equal(RegionName.JUNGLE_JAPES, eventArgs.RegionName);
            Assert.Equal("?", eventArgs.OldValue);
            Assert.Equal("10", eventArgs.NewValue);
        });
    }

    #endregion

    #region B. Locker ItemType Tests

    [Fact]
    public void GetBLockerItemType_WithNoData_ReturnsGoldenBanana()
    {
        // Act
        var itemType = _sut.GetBLockerItemType(RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(BarrierItems.GOLDEN_BANANA, itemType);
    }

    [Fact]
    public void GetBLockerItemType_WithExistingData_ReturnsValue()
    {
        // Arrange
        _progress.BLockerImageIndexes[RegionName.ANGRY_AZTEC] = (int)BarrierItems.BLUEPRINT;

        // Act
        var itemType = _sut.GetBLockerItemType(RegionName.ANGRY_AZTEC);

        // Assert
        Assert.Equal(BarrierItems.BLUEPRINT, itemType);
    }

    [Fact]
    public void SetBLockerItemType_UpdatesValue()
    {
        // Act
        _sut.SetBLockerItemType(RegionName.JUNGLE_JAPES, BarrierItems.CROWN);

        // Assert
        Assert.Equal(BarrierItems.CROWN, _sut.GetBLockerItemType(RegionName.JUNGLE_JAPES));
    }

    [Fact]
    public void SetBLockerItemType_RaisesEvent()
    {
        // Arrange
        var eventFired = false;
        BlockerBarrierTypeChangedEventArgs? eventArgs = null;

        _sut.BlockerBarrierTypeChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        // Act
        _sut.SetBLockerItemType(RegionName.JUNGLE_JAPES, BarrierItems.BLUEPRINT);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(eventFired);
            Assert.NotNull(eventArgs);
            Assert.Equal(RegionName.JUNGLE_JAPES, eventArgs.RegionName);
            Assert.Equal(BarrierItems.GOLDEN_BANANA, eventArgs.OldValue);
            Assert.Equal(BarrierItems.BLUEPRINT, eventArgs.NewValue);
        });
    }

    #endregion

    #region Helm Door Count Tests

    [Fact]
    public void GetHelmDoorCount_First_ReturnsValue()
    {
        // Arrange
        _progress.HelmDoorCounts[0] = "8";

        // Act
        var count = _sut.GetHelmDoorCount(HelmDoor.First);

        // Assert
        Assert.Equal("8", count);
    }

    [Fact]
    public void GetHelmDoorCount_Second_ReturnsValue()
    {
        // Arrange
        _progress.HelmDoorCounts[1] = "12";

        // Act
        var count = _sut.GetHelmDoorCount(HelmDoor.Second);

        // Assert
        Assert.Equal("12", count);
    }

    [Fact]
    public void SetHelmDoorCount_First_UpdatesValue()
    {
        // Act
        _sut.SetHelmDoorCount(HelmDoor.First, "15");

        // Assert
        Assert.Equal("15", _sut.GetHelmDoorCount(HelmDoor.First));
    }

    [Fact]
    public void SetHelmDoorCount_Second_UpdatesValue()
    {
        // Act
        _sut.SetHelmDoorCount(HelmDoor.Second, "20");

        // Assert
        Assert.Equal("20", _sut.GetHelmDoorCount(HelmDoor.Second));
    }

    [Fact]
    public void SetHelmDoorCount_RaisesEvent()
    {
        // Arrange
        var eventFired = false;
        HelmBarrierAmountChangedEventArgs? eventArgs = null;

        _sut.HelmBarrierAmountChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        // Act
        _sut.SetHelmDoorCount(HelmDoor.First, "25");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(eventFired);
            Assert.NotNull(eventArgs);
            Assert.Equal(HelmDoor.First, eventArgs.HelmDoor);
        });
    }

    #endregion

    #region Helm Door ItemType Tests

    [Fact]
    public void GetHelmDoorItemType_First_ReturnsValue()
    {
        // Arrange
        _progress.HelmDoorImageIndexes[0] = (int)BarrierItems.CROWN;

        // Act
        var itemType = _sut.GetHelmDoorItemType(HelmDoor.First);

        // Assert
        Assert.Equal(BarrierItems.CROWN, itemType);
    }

    [Fact]
    public void GetHelmDoorItemType_Second_ReturnsValue()
    {
        // Arrange
        _progress.HelmDoorImageIndexes[1] = (int)BarrierItems.BLUEPRINT;

        // Act
        var itemType = _sut.GetHelmDoorItemType(HelmDoor.Second);

        // Assert
        Assert.Equal(BarrierItems.BLUEPRINT, itemType);
    }

    [Fact]
    public void SetHelmDoorItemType_First_UpdatesValue()
    {
        // Act
        _sut.SetHelmDoorItemType(HelmDoor.First, BarrierItems.BLUEPRINT);

        // Assert
        Assert.Equal(BarrierItems.BLUEPRINT, _sut.GetHelmDoorItemType(HelmDoor.First));
    }

    [Fact]
    public void SetHelmDoorItemType_Second_UpdatesValue()
    {
        // Act
        _sut.SetHelmDoorItemType(HelmDoor.Second, BarrierItems.CROWN);

        // Assert
        Assert.Equal(BarrierItems.CROWN, _sut.GetHelmDoorItemType(HelmDoor.Second));
    }

    [Fact]
    public void SetHelmDoorItemType_RaisesEvent()
    {
        // Arrange
        var eventFired = false;
        HelmBarrierTypeChangedEventArgs? eventArgs = null;

        _sut.HelmBarrierTypeChanged += (sender, args) =>
        {
            eventFired = true;
            eventArgs = args;
        };

        // Act
        _sut.SetHelmDoorItemType(HelmDoor.First, BarrierItems.BLUEPRINT);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(eventFired);
            Assert.NotNull(eventArgs);
            Assert.Equal(HelmDoor.First, eventArgs.HelmDoor);
        });
    }

    #endregion

    #region Progress Replacement Tests

    [Fact]
    public void OnProgressChanged_ReinitializesWithNewProgress()
    {
        // Arrange
        var newProgress = new SavedProgress
        {
            SavedGBCounts = new Dictionary<RegionName, string> { { RegionName.JUNGLE_JAPES, "99" } },
            BLockerImageIndexes = [],
            HelmDoorCounts = ["?", "?"],
            HelmDoorImageIndexes = [0, 0]
        };

        // Act
        _provider.UpdateProgress(newProgress);

        // Assert
        Assert.Equal("99", _sut.GetBLockerCount(RegionName.JUNGLE_JAPES));
    }

    #endregion
}
