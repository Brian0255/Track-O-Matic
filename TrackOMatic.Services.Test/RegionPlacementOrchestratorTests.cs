using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services.Test;

/// <summary>
/// Unit tests for <see cref="RegionPlacementOrchestrator"/>.
///
/// Tests the 4-step item placement algorithm:
/// 1. Item already exists in matching slot (no-op)
/// 2. Place in empty slot of matching color
/// 3. Replace non-autotracked item of matching color
/// 4. Fail if all slots are unavailable or occupied by autotracked items
/// </summary>
public class RegionPlacementOrchestratorTests
{
    private readonly Mock<IRegionSlotProvider> _mockSlotProvider;
    private readonly Mock<IItemTrackingService> _mockItemTrackingService;
    private readonly RegionPlacementOrchestrator _sut;

    public RegionPlacementOrchestratorTests()
    {
        _mockSlotProvider = new Mock<IRegionSlotProvider>();
        _mockItemTrackingService = new Mock<IItemTrackingService>();
        _sut = new RegionPlacementOrchestrator(_mockSlotProvider.Object, _mockItemTrackingService.Object);
    }

    #region Setup Helpers

    private static Mock<IVialSlot> CreateMockVialSlot(
        VialColor color,
        RegionName region = RegionName.JUNGLE_JAPES,
        ItemName? currentItem = null,
        bool canAccept = true)
    {
        var slot = new Mock<IVialSlot>();
        slot.Setup(s => s.VialColor).Returns(color);
        slot.Setup(s => s.RegionName).Returns(region);
        slot.Setup(s => s.CurrentItemName).Returns(currentItem);
        slot.Setup(s => s.CanAcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()))
            .Returns(canAccept);
        return slot;
    }

    private static SavedItem CreateSavedItem(ItemName itemName, bool autotracked = true)
    {
        return new SavedItem
        {
            ItemName = itemName,
            Autotracked = autotracked,
            Opacity = 1.0,
            Starred = false
        };
    }

    #endregion

    #region Step 1: Item Already Placed

    [Fact]
    public void TryPlaceAutoTrackedItem_WithItemAlreadyInMatchingSlot_ReturnTrue()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        var existingSlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: itemName);
        var slots = new[] { existingSlot.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        Assert.True(result);
        // Slot should not be called to accept the item since it's already there
        existingSlot.Verify(s => s.AcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
    }

    [Fact]
    public void TryPlaceAutoTrackedItem_WithDifferentItemInSlot_ContinueToStep2()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        // Slot with different item (not a match)
        var wrongItemSlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: ItemName.DIDDY);
        // Empty matching slot
        var emptySlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: null);
        emptySlot.Setup(s => s.CanAcceptAutoTrackedItem(itemName, itemState)).Returns(true);

        var slots = new[] { wrongItemSlot.Object, emptySlot.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        Assert.True(result);
        // Should not place in wrong item slot
        wrongItemSlot.Verify(s => s.AcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
        // Should place in empty slot
        emptySlot.Verify(s => s.AcceptAutoTrackedItem(itemName, itemState), Times.Once);
    }

    #endregion

    #region Step 2: Empty Slot Placement

    [Fact]
    public void TryPlaceAutoTrackedItem_WithEmptyMatchingSlot_PlaceInSlot()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        var emptySlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: null);
        emptySlot.Setup(s => s.CanAcceptAutoTrackedItem(itemName, itemState)).Returns(true);

        var slots = new[] { emptySlot.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        Assert.True(result);
        emptySlot.Verify(s => s.AcceptAutoTrackedItem(itemName, itemState), Times.Once);
    }

    [Fact]
    public void TryPlaceAutoTrackedItem_WithEmptySlotButCannotAccept_SkipToStep3()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        // Empty slot that cannot accept
        var cannotAcceptSlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: null, canAccept: false);

        // Replaceable non-autotracked item
        var replaceableSlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: ItemName.LANKY);
        replaceableSlot.Setup(s => s.CanAcceptAutoTrackedItem(itemName, itemState)).Returns(true);

        var slots = new[] { cannotAcceptSlot.Object, replaceableSlot.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        var replaceableItemState = CreateSavedItem(ItemName.LANKY, autotracked: false);
        _mockItemTrackingService
            .Setup(x => x.GetItemState(ItemName.LANKY))
            .Returns(replaceableItemState);

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        Assert.True(result);
        // Should not place in cannot-accept slot
        cannotAcceptSlot.Verify(s => s.AcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
        // Should place in replaceable slot
        replaceableSlot.Verify(s => s.AcceptAutoTrackedItem(itemName, itemState), Times.Once);
    }

    [Fact]
    public void TryPlaceAutoTrackedItem_WithMultipleEmptySlots_PlaceInFirst()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        var firstSlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: null);
        firstSlot.Setup(s => s.CanAcceptAutoTrackedItem(itemName, itemState)).Returns(true);

        var secondSlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: null);
        secondSlot.Setup(s => s.CanAcceptAutoTrackedItem(itemName, itemState)).Returns(true);

        var slots = new[] { firstSlot.Object, secondSlot.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        Assert.True(result);
        // Should only place in first matching slot
        firstSlot.Verify(s => s.AcceptAutoTrackedItem(itemName, itemState), Times.Once);
        secondSlot.Verify(s => s.AcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
    }

    #endregion

    #region Step 3: Replace Non-Autotracked Item

    [Fact]
    public void TryPlaceAutoTrackedItem_WithNonAutoTrackedItemInSlot_ReplaceIt()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        // No empty slots
        var occupiedSlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: ItemName.TINY);
        occupiedSlot.Setup(s => s.CanAcceptAutoTrackedItem(itemName, itemState)).Returns(true);

        var slots = new[] { occupiedSlot.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        // Current item is not autotracked
        var currentItemState = CreateSavedItem(ItemName.TINY, autotracked: false);
        _mockItemTrackingService
            .Setup(x => x.GetItemState(ItemName.TINY))
            .Returns(currentItemState);

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        Assert.True(result);
        occupiedSlot.Verify(s => s.AcceptAutoTrackedItem(itemName, itemState), Times.Once);
    }

    [Fact]
    public void TryPlaceAutoTrackedItem_WithPriorityToEmptyOverReplaceable_PlaceInEmpty()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        // Empty slot first
        var emptySlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: null);
        emptySlot.Setup(s => s.CanAcceptAutoTrackedItem(itemName, itemState)).Returns(true);

        // Replaceable slot after
        var replaceableSlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: ItemName.CHUNKY);
        replaceableSlot.Setup(s => s.CanAcceptAutoTrackedItem(itemName, itemState)).Returns(true);

        var slots = new[] { emptySlot.Object, replaceableSlot.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        var replaceableItemState = CreateSavedItem(ItemName.CHUNKY, autotracked: false);
        _mockItemTrackingService
            .Setup(x => x.GetItemState(ItemName.CHUNKY))
            .Returns(replaceableItemState);

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        Assert.True(result);
        // Should prefer empty slot
        emptySlot.Verify(s => s.AcceptAutoTrackedItem(itemName, itemState), Times.Once);
        replaceableSlot.Verify(s => s.AcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
    }

    #endregion

    #region Step 4: Failure Cases

    [Fact]
    public void TryPlaceAutoTrackedItem_WithNoVialsInRegion_ReturnFalse()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        var slots = Array.Empty<IVialSlot>();

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryPlaceAutoTrackedItem_WithWrongColorSlotsOnly_ReturnFalse()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        // All slots are wrong color
        var wrongColorSlot1 = CreateMockVialSlot(VialColor.KEY, region, currentItem: null);
        var wrongColorSlot2 = CreateMockVialSlot(VialColor.CLEAR, region, currentItem: null);

        var slots = new[] { wrongColorSlot1.Object, wrongColorSlot2.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        Assert.False(result);
        wrongColorSlot1.Verify(s => s.AcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
        wrongColorSlot2.Verify(s => s.AcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
    }

    [Fact]
    public void TryPlaceAutoTrackedItem_WithAllSlotsOccupiedByAutoTrackedItems_ReturnFalse()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        // Slots occupied by autotracked items
        var autoTrackedSlot1 = CreateMockVialSlot(VialColor.KONG, region, currentItem: ItemName.DIDDY);
        var autoTrackedSlot2 = CreateMockVialSlot(VialColor.KONG, region, currentItem: ItemName.LANKY);

        var slots = new[] { autoTrackedSlot1.Object, autoTrackedSlot2.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        // Both items are autotracked
        _mockItemTrackingService
            .Setup(x => x.GetItemState(ItemName.DIDDY))
            .Returns(CreateSavedItem(ItemName.DIDDY, autotracked: true));

        _mockItemTrackingService
            .Setup(x => x.GetItemState(ItemName.LANKY))
            .Returns(CreateSavedItem(ItemName.LANKY, autotracked: true));

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        Assert.False(result);
        autoTrackedSlot1.Verify(s => s.AcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
        autoTrackedSlot2.Verify(s => s.AcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
    }

    [Fact]
    public void TryPlaceAutoTrackedItem_WithSlotCannotAcceptAnyItem_ReturnFalse()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        // Empty slot but cannot accept items
        var emptyButCannotAcceptSlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: null, canAccept: false);

        // Occupied slot with non-autotracked item but cannot accept
        var occupiedCannotAcceptSlot = CreateMockVialSlot(VialColor.KONG, region, currentItem: ItemName.TINY, canAccept: false);

        var slots = new[] { emptyButCannotAcceptSlot.Object, occupiedCannotAcceptSlot.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        _mockItemTrackingService
            .Setup(x => x.GetItemState(ItemName.TINY))
            .Returns(CreateSavedItem(ItemName.TINY, autotracked: false));

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        Assert.False(result);
        emptyButCannotAcceptSlot.Verify(s => s.AcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
        occupiedCannotAcceptSlot.Verify(s => s.AcceptAutoTrackedItem(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void TryPlaceAutoTrackedItem_WithNullItemTrackingServiceResult_TreatAsNotAutotracked()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var region = RegionName.JUNGLE_JAPES;
        var itemState = CreateSavedItem(itemName);

        // Occupied slot with item whose state is unknown
        var slot = CreateMockVialSlot(VialColor.KONG, region, currentItem: ItemName.TINY);
        slot.Setup(s => s.CanAcceptAutoTrackedItem(itemName, itemState)).Returns(true);

        var slots = new[] { slot.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(slots);

        // ItemTrackingService returns null (unknown item)
        _mockItemTrackingService
            .Setup(x => x.GetItemState(ItemName.TINY))
            .Returns((SavedItem?)null);

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, region, itemState);

        // Assert
        // When null is returned, the orchestrator treats it as if the item exists but is not autotracked
        // and does NOT attempt to replace it (to be safe)
        Assert.False(result);
    }

    [Fact]
    public void TryPlaceAutoTrackedItem_WithMultipleRegions_OnlyCheckTargetRegion()
    {
        // Arrange
        var itemName = ItemName.DONKEY;
        var targetRegion = RegionName.JUNGLE_JAPES;
        var otherRegion = RegionName.ANGRY_AZTEC;
        var itemState = CreateSavedItem(itemName);

        var emptySlot = CreateMockVialSlot(VialColor.KONG, targetRegion, currentItem: null);
        emptySlot.Setup(s => s.CanAcceptAutoTrackedItem(itemName, itemState)).Returns(true);

        var slots = new[] { emptySlot.Object };

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(targetRegion))
            .Returns(slots);

        _mockSlotProvider
            .Setup(x => x.GetVialSlotsForRegion(otherRegion))
            .Returns([]);

        // Act
        var result = _sut.TryPlaceAutoTrackedItem(itemName, targetRegion, itemState);

        // Assert
        Assert.True(result);
        // Should only query target region
        _mockSlotProvider.Verify(x => x.GetVialSlotsForRegion(targetRegion), Times.Once);
        _mockSlotProvider.Verify(x => x.GetVialSlotsForRegion(otherRegion), Times.Never);
    }

    #endregion
}
