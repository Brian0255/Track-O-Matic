namespace TrackOMatic.ViewModels.Test.Regions;

public class RegionItemViewModelTests
{
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock = new();
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock = new();
    private readonly Mock<IThemeService> _themeServiceMock = new();

    public RegionItemViewModelTests()
    {
        // Default setup
        _itemTrackingServiceMock.Setup(x => x.GetItemState(It.IsAny<ItemName>())).Returns((ItemName name) =>
            new SavedItem { ItemName = name, Region = RegionName.JUNGLE_JAPES, Opacity = 1.0 }
        );
        _parsedSpoilerDataServiceMock.SetupGet(x => x.CurrentData).Returns((ParsedSpoilerData?)null);
    }

    private RegionItemViewModel CreateViewModel(
        ItemName? itemName = ItemName.KEY_1,
        RegionName regionName = RegionName.JUNGLE_JAPES,
        IItemTrackingService? itemTrackingService = null,
        IParsedSpoilerDataService? parsedSpoilerDataService = null,
        IThemeService? themeService = null
    )
    {
        return new RegionItemViewModel(
            itemName,
            regionName,
            itemTrackingService ?? _itemTrackingServiceMock.Object,
            parsedSpoilerDataService ?? _parsedSpoilerDataServiceMock.Object,
            themeService ?? _themeServiceMock.Object
        );
    }

    #region Constructor Validation Tests

    [Fact]
    public void Constructor_WithValidDependencies_Succeeds()
    {
        var vm = CreateViewModel();
        Assert.NotNull(vm);
    }

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionItemViewModel(
                ItemName.KEY_1,
                RegionName.JUNGLE_JAPES,
                null!,
                _parsedSpoilerDataServiceMock.Object,
                _themeServiceMock.Object
            )
        );
        Assert.Equal("itemTrackingService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionItemViewModel(
                ItemName.KEY_1,
                RegionName.JUNGLE_JAPES,
                _itemTrackingServiceMock.Object,
                null!,
                _themeServiceMock.Object
            )
        );
        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullThemeService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionItemViewModel(
                ItemName.KEY_1,
                RegionName.JUNGLE_JAPES,
                _itemTrackingServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                null!
            )
        );
        Assert.Equal("themeService", ex.ParamName);
    }

    #endregion

    #region Property Initialization Tests

    [Fact]
    public void Constructor_WithValidItem_InitializesProperties()
    {
        // Arrange
        var itemState = new SavedItem
        {
            ItemName = ItemName.KEY_1,
            Region = RegionName.JUNGLE_JAPES,
            Opacity = 0.5,
            Starred = true,
            Hinted = false
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(itemState);

        // Act
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.Equal(ItemName.KEY_1, vm.CurrentItemName);
        Assert.Equal(RegionName.JUNGLE_JAPES, vm.RegionName);
        Assert.Equal("key_1", vm.ImageResourceKey);
        Assert.True(vm.IsStarred);
        Assert.Equal(0.5, vm.Opacity);
    }

    [Fact]
    public void Constructor_WithNullItemName_InitializesWithDefaults()
    {
        // Act
        var vm = CreateViewModel(itemName: null);

        // Assert
        Assert.Null(vm.CurrentItemName);
        Assert.Empty(vm.ImageResourceKey);
        Assert.False(vm.IsStarred);
        Assert.Equal(1.0, vm.Opacity);
    }

    [Fact]
    public void Constructor_WithHintedItem_UsesBlackAndWhiteImage()
    {
        // Arrange
        var itemState = new SavedItem
        {
            ItemName = ItemName.KEY_1,
            Region = RegionName.JUNGLE_JAPES,
            Opacity = 1.0,
            Hinted = true
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(itemState);

        // Act
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.Equal("key_1_bw", vm.ImageResourceKey);
    }

    [Fact]
    public void Constructor_SubscribesToItemStateChanged()
    {
        // Arrange
        EventHandler<ItemStateChangedEventArgs>? capturedHandler = null;
        _itemTrackingServiceMock.SetupAdd(x => x.ItemStateChanged += It.IsAny<EventHandler<ItemStateChangedEventArgs>>())
            .Callback((EventHandler<ItemStateChangedEventArgs> handler) => capturedHandler = handler);

        // Act
        var vm = CreateViewModel();

        // Assert
        Assert.NotNull(capturedHandler);
    }

    [Fact]
    public void Constructor_WithBarrelPadItem_SubscribesToThemeChanged()
    {
        // Arrange
        EventHandler? capturedHandler = null;
        _themeServiceMock.SetupAdd(x => x.BarrelPadThemeChanged += It.IsAny<EventHandler>())
            .Callback((EventHandler handler) => capturedHandler = handler);

        // Act - Use a barrel pad item
        var vm = CreateViewModel(ItemName.STRONG_KONG);

        // Assert
        Assert.NotNull(capturedHandler);
    }

    [Fact]
    public void Constructor_WithNonBarrelPadItem_DoesNotSubscribeToThemeChanged()
    {
        // Arrange & Act
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert - Verify theme service add was not called for non-barrel pad items
        _themeServiceMock.VerifyAdd(x => x.BarrelPadThemeChanged += It.IsAny<EventHandler>(), Times.Never);
    }

    #endregion

    #region HoverText Tests

    [Fact]
    public void Constructor_WithPointsEnabled_SetsHoverText()
    {
        // Arrange
        var pointSpread = new Dictionary<PointCategory, int>();
        for (int i = 0; i <= 13; i++)
        {
            pointSpread[(PointCategory)i] = 10;
        }
        var spoiledData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
            PointSpread = pointSpread,
            SpoilerSettings = new SpoilerSettings(pointsEnabled: true)
        };
        _parsedSpoilerDataServiceMock.SetupGet(x => x.CurrentData).Returns(spoiledData);

        // Act
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.NotEmpty(vm.HoverText);
        Assert.True(vm.HasHoverText);
    }

    [Fact]
    public void Constructor_WithPointsDisabled_NoHoverText()
    {
        // Arrange
        var spoiledData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
            PointSpread = [],
            SpoilerSettings = new SpoilerSettings(pointsEnabled: false)
        };
        _parsedSpoilerDataServiceMock.SetupGet(x => x.CurrentData).Returns(spoiledData);

        // Act
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.Empty(vm.HoverText);
        Assert.False(vm.HasHoverText);
    }

    [Fact]
    public void Constructor_WithNullSpoilerData_NoHoverText()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.SetupGet(x => x.CurrentData).Returns((ParsedSpoilerData?)null);

        // Act
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.Empty(vm.HoverText);
        Assert.False(vm.HasHoverText);
    }

    [Theory]
    [InlineData(1, "Point")]
    [InlineData(5, "Points")]
    public void HoverText_UsesCorrectSingularPlural(int points, string expectedSuffix)
    {
        // Arrange
        var pointSpread = new Dictionary<PointCategory, int>();
        for (int i = 0; i <= 13; i++)
        {
            pointSpread[(PointCategory)i] = points;
        }
        var spoiledData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int> { { RegionName.JUNGLE_JAPES, 1 } },
            PointSpread = pointSpread,
            SpoilerSettings = new SpoilerSettings(pointsEnabled: true)
        };
        _parsedSpoilerDataServiceMock.SetupGet(x => x.CurrentData).Returns(spoiledData);

        // Act
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.Contains(expectedSuffix, vm.HoverText);
        Assert.Contains(points.ToString(), vm.HoverText);
    }

    #endregion

    #region CanRemoveFromRegion Tests

    [Fact]
    public void RemoveFromRegionCommand_CanExecute_WithTrackedNonAutoTrackedItem_ReturnsTrue()
    {
        // Arrange
        var itemState = new SavedItem
        {
            ItemName = ItemName.KEY_1,
            Autotracked = false
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(itemState);
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.True(vm.RemoveFromRegionCommand.CanExecute(null));
    }

    [Fact]
    public void RemoveFromRegionCommand_CanExecute_WithNullItem_ReturnsFalse()
    {
        // Arrange
        var vm = CreateViewModel(itemName: null);

        // Assert
        Assert.False(vm.RemoveFromRegionCommand.CanExecute(null));
    }

    [Fact]
    public void RemoveFromRegionCommand_CanExecute_WithAutoTrackedItem_ReturnsFalse()
    {
        // Arrange
        var itemState = new SavedItem
        {
            ItemName = ItemName.KEY_1,
            Autotracked = true
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(itemState);
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.False(vm.RemoveFromRegionCommand.CanExecute(null));
    }

    [Fact]
    public void RemoveFromRegionCommand_CanExecute_WithNullState_ReturnsFalse()
    {
        // Arrange
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns((SavedItem?)null);
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.False(vm.RemoveFromRegionCommand.CanExecute(null));
    }

    #endregion

    #region RemoveFromRegion Command Tests

    [Fact]
    public void RemoveFromRegionCommand_Execute_UpdatesItemRegionToUnknown()
    {
        // Arrange
        var itemState = new SavedItem
        {
            ItemName = ItemName.KEY_1,
            Region = RegionName.JUNGLE_JAPES,
            Opacity = 0.5,
            Autotracked = false
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(itemState);
        var vm = CreateViewModel(ItemName.KEY_1);

        // Act
        vm.RemoveFromRegionCommand.Execute(null);

        // Assert
        _itemTrackingServiceMock.Verify(x => x.SetItemState(
            ItemName.KEY_1,
            It.Is<SavedItem>(state =>
                state.Region == RegionName.UNKNOWN &&
                state.Opacity == 1.0
            )
        ), Times.Once);
    }

    [Fact]
    public void RemoveFromRegionCommand_Execute_WithNullItem_DoesNothing()
    {
        // Arrange
        var vm = CreateViewModel(itemName: null);

        // Act
        vm.RemoveFromRegionCommand.Execute(null);

        // Assert
        _itemTrackingServiceMock.Verify(x => x.SetItemState(It.IsAny<ItemName>(), It.IsAny<SavedItem>()), Times.Never);
    }

    #endregion

    #region CanToggleStar Tests

    [Fact]
    public void ToggleStarCommand_CanExecute_WithValidItem_ReturnsTrue()
    {
        // Arrange
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.True(vm.ToggleStarCommand.CanExecute(null));
    }

    [Fact]
    public void ToggleStarCommand_CanExecute_WithNullItem_ReturnsFalse()
    {
        // Arrange
        var vm = CreateViewModel(itemName: null);

        // Assert
        Assert.False(vm.ToggleStarCommand.CanExecute(null));
    }

    #endregion

    #region ToggleStar Command Tests

    [Fact]
    public void ToggleStarCommand_Execute_CallsItemTrackingService()
    {
        // Arrange
        var vm = CreateViewModel(ItemName.KEY_1);

        // Act
        vm.ToggleStarCommand.Execute(null);

        // Assert
        _itemTrackingServiceMock.Verify(x => x.ToggleStar(ItemName.KEY_1), Times.Once);
    }

    [Fact]
    public void ToggleStarCommand_Execute_WithNullItem_DoesNothing()
    {
        // Arrange
        var vm = CreateViewModel(itemName: null);

        // Act
        vm.ToggleStarCommand.Execute(null);

        // Assert
        _itemTrackingServiceMock.Verify(x => x.ToggleStar(It.IsAny<ItemName>()), Times.Never);
    }

    #endregion

    #region GetHoardPoints Tests

    [Fact]
    public void GetHoardPoints_WithStarredItem_ReturnsOne()
    {
        // Arrange
        var itemState = new SavedItem
        {
            ItemName = ItemName.KEY_1,
            Starred = true
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(itemState);
        var vm = CreateViewModel(ItemName.KEY_1);

        // Act
        var result = vm.GetHoardPoints();

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void GetHoardPoints_WithUnstarredItem_ReturnsZero()
    {
        // Arrange
        var itemState = new SavedItem
        {
            ItemName = ItemName.KEY_1,
            Starred = false
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(itemState);
        var vm = CreateViewModel(ItemName.KEY_1);

        // Act
        var result = vm.GetHoardPoints();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void GetHoardPoints_WithNullItem_ReturnsZero()
    {
        // Arrange
        var vm = CreateViewModel(itemName: null);

        // Act
        var result = vm.GetHoardPoints();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void GetHoardPoints_WithNullItemState_ReturnsZero()
    {
        // Arrange
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns((SavedItem?)null);
        var vm = CreateViewModel(ItemName.KEY_1);

        // Act
        var result = vm.GetHoardPoints();

        // Assert
        Assert.Equal(0, result);
    }

    #endregion

    #region GetItemPoints Tests

    [Fact]
    public void GetItemPoints_WithValidItem_ReturnsPointsFromSpoilerData()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.GetPointsForItem(ItemName.KEY_1)).Returns(15);
        var vm = CreateViewModel(ItemName.KEY_1);

        // Act
        var result = vm.GetItemPoints();

        // Assert
        Assert.Equal(15, result);
    }

    [Fact]
    public void GetItemPoints_WithNullItem_ReturnsZero()
    {
        // Arrange
        var vm = CreateViewModel(itemName: null);

        // Act
        var result = vm.GetItemPoints();

        // Assert
        Assert.Equal(0, result);
    }

    #endregion

    #region CanAcceptDrop Tests

    [Fact]
    public void CanAcceptDrop_AlwaysReturnsFalse()
    {
        // Arrange
        var vm = CreateViewModel(ItemName.KEY_1);

        // Act & Assert
        Assert.False(vm.CanAcceptDrop(ItemName.KEY_2, MouseDragType.Left));
        Assert.False(vm.CanAcceptDrop(ItemName.KEY_2, MouseDragType.Right));
        Assert.False(vm.CanAcceptDrop(ItemName.KEY_2, MouseDragType.None));
    }

    #endregion

    #region AcceptDropAndMutate Tests

    [Fact]
    public void AcceptDropAndMutate_AlwaysReturnsFalse()
    {
        // Arrange
        var vm = CreateViewModel(ItemName.KEY_1);

        // Act & Assert
        Assert.False(vm.AcceptDropAndMutate(ItemName.KEY_2, MouseDragType.Left));
        Assert.False(vm.AcceptDropAndMutate(ItemName.KEY_2, MouseDragType.Right));
        Assert.False(vm.AcceptDropAndMutate(ItemName.KEY_2, MouseDragType.None));
    }

    #endregion

    #region Event Handling Tests

    [Fact]
    public void OnItemStateChanged_WithMatchingItem_UpdatesProperties()
    {
        // Arrange
        EventHandler<ItemStateChangedEventArgs>? capturedHandler = null;
        _itemTrackingServiceMock.SetupAdd(x => x.ItemStateChanged += It.IsAny<EventHandler<ItemStateChangedEventArgs>>())
            .Callback((EventHandler<ItemStateChangedEventArgs> handler) => capturedHandler = handler);
        var vm = CreateViewModel(ItemName.KEY_1);

        // Update mock to return new state
        var newState = new SavedItem
        {
            ItemName = ItemName.KEY_1,
            Starred = true,
            Hinted = true,
            Opacity = 0.3
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(newState);

        // Act
        capturedHandler?.Invoke(
            _itemTrackingServiceMock.Object,
            new ItemStateChangedEventArgs(newState)
        );

        // Assert
        Assert.Equal("key_1_bw", vm.ImageResourceKey);
        Assert.True(vm.IsStarred);
        Assert.Equal(0.3, vm.Opacity);
    }

    [Fact]
    public void OnItemStateChanged_WithDifferentItem_DoesNotUpdateProperties()
    {
        // Arrange
        EventHandler<ItemStateChangedEventArgs>? capturedHandler = null;
        _itemTrackingServiceMock.SetupAdd(x => x.ItemStateChanged += It.IsAny<EventHandler<ItemStateChangedEventArgs>>())
            .Callback((EventHandler<ItemStateChangedEventArgs> handler) => capturedHandler = handler);
        var vm = CreateViewModel(ItemName.KEY_1);
        var originalImageKey = vm.ImageResourceKey;

        // Act - Trigger event with different item
        var otherItemState = new SavedItem
        {
            ItemName = ItemName.KEY_2,
            Starred = true
        };
        capturedHandler?.Invoke(
            _itemTrackingServiceMock.Object,
            new ItemStateChangedEventArgs(otherItemState)
        );

        // Assert
        Assert.Equal(originalImageKey, vm.ImageResourceKey);
        Assert.False(vm.IsStarred);
    }

    [Fact]
    public void OnBarrelPadThemeChanged_WithBarrelPadItem_UpdatesImageResourceKey()
    {
        // Arrange
        EventHandler? capturedHandler = null;
        _themeServiceMock.SetupAdd(x => x.BarrelPadThemeChanged += It.IsAny<EventHandler>())
            .Callback((EventHandler handler) => capturedHandler = handler);
        var itemState = new SavedItem
        {
            ItemName = ItemName.STRONG_KONG,
            Hinted = false
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.STRONG_KONG)).Returns(itemState);
        var vm = CreateViewModel(ItemName.STRONG_KONG);
        var originalKey = vm.ImageResourceKey;

        // Act
        capturedHandler?.Invoke(_themeServiceMock.Object, EventArgs.Empty);

        // Assert - Image key should have been temporarily cleared and re-set
        _itemTrackingServiceMock.Verify(x => x.GetItemState(ItemName.STRONG_KONG), Times.AtLeast(2));
    }

    #endregion

    #region VialSlot Implementation Tests

    [Fact]
    public void CanAcceptAutoTrackedItem_WithEmptySlot_ReturnsTrue()
    {
        // Arrange
        var vm = CreateViewModel(itemName: null);
        var itemState = new SavedItem { ItemName = ItemName.KEY_1 };

        // Act
        var result = vm.CanAcceptAutoTrackedItem(ItemName.KEY_1, itemState);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanAcceptAutoTrackedItem_WithNonAutoTrackedItem_ReturnsTrue()
    {
        // Arrange
        var currentState = new SavedItem
        {
            ItemName = ItemName.KEY_2,
            Autotracked = false
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_2)).Returns(currentState);
        var vm = CreateViewModel(ItemName.KEY_2);
        var newItemState = new SavedItem { ItemName = ItemName.KEY_1 };

        // Act
        var result = vm.CanAcceptAutoTrackedItem(ItemName.KEY_1, newItemState);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanAcceptAutoTrackedItem_WithAutoTrackedItem_ReturnsFalse()
    {
        // Arrange
        var currentState = new SavedItem
        {
            ItemName = ItemName.KEY_2,
            Autotracked = true
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_2)).Returns(currentState);
        var vm = CreateViewModel(ItemName.KEY_2);
        var newItemState = new SavedItem { ItemName = ItemName.KEY_1 };

        // Act
        var result = vm.CanAcceptAutoTrackedItem(ItemName.KEY_1, newItemState);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AcceptAutoTrackedItem_WritesItemToService()
    {
        // Arrange
        var vm = CreateViewModel(itemName: null);
        var itemState = new SavedItem { ItemName = ItemName.KEY_1 };

        // Act
        vm.AcceptAutoTrackedItem(ItemName.KEY_1, itemState);

        // Assert
        _itemTrackingServiceMock.Verify(x => x.SetItemState(
            ItemName.KEY_1,
            itemState,
            ChangeReason.AutoTracked
        ), Times.Once);
    }

    #endregion

    #region Equality/Hashing Tests

    [Fact]
    public void Equals_WithSameRegion_ReturnsTrue()
    {
        // Arrange
        var vm1 = CreateViewModel(regionName: RegionName.JUNGLE_JAPES);
        var vm2 = CreateViewModel(regionName: RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(vm1, vm2);
    }

    [Fact]
    public void Equals_WithDifferentRegion_ReturnsFalse()
    {
        // Arrange
        var vm1 = CreateViewModel(regionName: RegionName.JUNGLE_JAPES);
        var vm2 = CreateViewModel(regionName: RegionName.ANGRY_AZTEC);

        // Assert
        Assert.NotEqual(vm1, vm2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var vm = CreateViewModel();

        // Assert
        Assert.False(vm.Equals(null));
    }

    [Fact]
    public void Equals_WithSameReference_ReturnsTrue()
    {
        // Arrange
        var vm = CreateViewModel();

        // Assert
        Assert.Equal(vm, vm);
    }

    [Fact]
    public void GetHashCode_WithSameRegion_ReturnsSameHash()
    {
        // Arrange
        var vm1 = CreateViewModel(regionName: RegionName.JUNGLE_JAPES);
        var vm2 = CreateViewModel(regionName: RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Equal(vm1.GetHashCode(), vm2.GetHashCode());
    }

    [Fact]
    public void ToString_WithStarredItem_IncludesAsterisk()
    {
        // Arrange
        var itemState = new SavedItem
        {
            ItemName = ItemName.KEY_1,
            Starred = true
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(itemState);
        var vm = CreateViewModel(ItemName.KEY_1, RegionName.JUNGLE_JAPES);

        // Assert
        Assert.Contains("*", vm.ToString());
        Assert.Contains("JUNGLE_JAPES", vm.ToString());
    }

    [Fact]
    public void ToString_WithUnstarredItem_NoAsterisk()
    {
        // Arrange
        var itemState = new SavedItem
        {
            ItemName = ItemName.KEY_1,
            Starred = false
        };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(itemState);
        var vm = CreateViewModel(ItemName.KEY_1, RegionName.JUNGLE_JAPES);

        // Assert
        var result = vm.ToString();
        Assert.DoesNotContain("*", result);
        Assert.Contains("JUNGLE_JAPES", result);
    }

    #endregion

    #region VialColor Tests

    [Fact]
    public void VialColor_AlwaysReturnsNone()
    {
        // Arrange
        var vm = CreateViewModel();

        // Assert
        Assert.Equal(VialColor.NONE, vm.VialColor);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromItemStateChanged()
    {
        // Arrange
        EventHandler<ItemStateChangedEventArgs>? capturedHandler = null;
        _itemTrackingServiceMock.SetupAdd(x => x.ItemStateChanged += It.IsAny<EventHandler<ItemStateChangedEventArgs>>())
            .Callback((EventHandler<ItemStateChangedEventArgs> handler) => capturedHandler = handler);
        var vm = CreateViewModel(ItemName.KEY_1);

        // Act
        vm.Dispose();

        // Assert
        _itemTrackingServiceMock.VerifyRemove(x => x.ItemStateChanged -= capturedHandler!, Times.Once);
    }

    [Fact]
    public void Dispose_WithBarrelPadItem_UnsubscribesFromBarrelPadThemeChanged()
    {
        // Arrange
        EventHandler? capturedHandler = null;
        _themeServiceMock.SetupAdd(x => x.BarrelPadThemeChanged += It.IsAny<EventHandler>())
            .Callback((EventHandler handler) => capturedHandler = handler);
        var vm = CreateViewModel(ItemName.STRONG_KONG);

        // Act
        vm.Dispose();

        // Assert
        _themeServiceMock.VerifyRemove(x => x.BarrelPadThemeChanged -= capturedHandler!, Times.Once);
    }

    [Fact]
    public void Dispose_WithNonBarrelPadItem_DoesNotUnsubscribeFromThemeChanged()
    {
        // Arrange
        var vm = CreateViewModel(ItemName.KEY_1);

        // Act
        vm.Dispose();

        // Assert
        _themeServiceMock.VerifyRemove(x => x.BarrelPadThemeChanged -= It.IsAny<EventHandler>(), Times.Never);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act & Assert
        vm.Dispose();
        vm.Dispose(); // Should not throw
    }

    #endregion

    #region Interface Implementation Tests

    [Fact]
    public void CurrentItemName_ReturnsItemFromViewModel()
    {
        // Arrange
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.Equal(ItemName.KEY_1, vm?.CurrentItemName);
    }

    [Fact]
    public void ImageResourceKey_IsExposedViaInterface()
    {
        // Arrange
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.NotNull(vm?.ImageResourceKey);
        Assert.NotEmpty(vm!.ImageResourceKey);
    }

    [Fact]
    public void IsStarred_IsExposedViaInterface()
    {
        // Arrange
        var vm = CreateViewModel(ItemName.KEY_1);

        // Assert
        Assert.False(vm?.IsStarred);
    }

    #endregion
}
