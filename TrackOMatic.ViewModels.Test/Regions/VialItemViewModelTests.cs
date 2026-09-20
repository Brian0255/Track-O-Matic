namespace TrackOMatic.ViewModels.Test.Regions;

public class VialItemViewModelTests
{
    private readonly Mock<IItemTrackingService> _mockItemTrackingService;
    private readonly Mock<IParsedSpoilerDataService> _mockParsedSpoilerDataService;
    private readonly Mock<IThemeService> _mockThemeService;

    public VialItemViewModelTests()
    {
        _mockItemTrackingService = new Mock<IItemTrackingService>();
        _mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        _mockThemeService = new Mock<IThemeService>();

        var defaultSpoilerData = new ParsedSpoilerData
        {
            LevelOrder = [],
            RegionData = new Dictionary<RegionName, RegionSpoilerData>
            {
                { RegionName.JUNGLE_JAPES, new RegionSpoilerData() },
                { RegionName.ANGRY_AZTEC, new RegionSpoilerData() },
                { RegionName.FRANTIC_FACTORY, new RegionSpoilerData() },
                { RegionName.GLOOMY_GALLEON, new RegionSpoilerData() },
                { RegionName.FUNGI_FOREST, new RegionSpoilerData() },
                { RegionName.CRYSTAL_CAVES, new RegionSpoilerData() },
                { RegionName.CREEPY_CASTLE, new RegionSpoilerData() },
                { RegionName.HIDEOUT_HELM, new RegionSpoilerData() },
                { RegionName.DK_ISLES, new RegionSpoilerData() },
                { RegionName.SHOPS, new RegionSpoilerData() },
                { RegionName.START, new RegionSpoilerData() },
                { RegionName.UNHINTABLE_MOVES, new RegionSpoilerData() },
                { RegionName.UNKNOWN, new RegionSpoilerData() }
            }
        };

        _mockParsedSpoilerDataService.Setup(s => s.CurrentData).Returns(defaultSpoilerData);
        _mockItemTrackingService.Setup(s => s.SetItemState(It.IsAny<ItemName>(), It.IsAny<SavedItem>()));
        _mockItemTrackingService.Setup(s => s.SetItemState(It.IsAny<ItemName>(), It.IsAny<SavedItem>(), It.IsAny<ChangeReason>()));
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidStartRegionVial_Succeeds()
    {
        var vm = new VialItemViewModel(
            ItemName.KEY_1,
            RegionName.START,
            VialColor.KEY,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.NotNull(vm);
        Assert.Equal(RegionName.START, vm.RegionName);
        Assert.Equal(VialColor.KEY, vm.VialColor);
    }

    [Fact]
    public void Constructor_WithValidNonStartRegionVial_Succeeds()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.NotNull(vm);
        Assert.Equal(RegionName.JUNGLE_JAPES, vm.RegionName);
        Assert.Equal(VialColor.YELLOW, vm.VialColor);
    }

    [Fact]
    public void Constructor_WithStartRegionAndNoItemName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new VialItemViewModel(
                null,
                RegionName.START,
                VialColor.KEY,
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                _mockThemeService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNonStartRegionAndItemName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new VialItemViewModel(
                ItemName.KEY_1,
                RegionName.JUNGLE_JAPES,
                VialColor.KEY,
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                _mockThemeService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new VialItemViewModel(
                ItemName.KEY_1,
                RegionName.START,
                VialColor.KEY,
                null!,
                _mockParsedSpoilerDataService.Object,
                _mockThemeService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new VialItemViewModel(
                ItemName.KEY_1,
                RegionName.START,
                VialColor.KEY,
                _mockItemTrackingService.Object,
                null!,
                _mockThemeService.Object
            )
        );
    }

    [Fact]
    public void Constructor_WithNullThemeService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new VialItemViewModel(
                ItemName.KEY_1,
                RegionName.START,
                VialColor.KEY,
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                null!
            )
        );
    }

    #endregion

    #region IsVialStarred Property Tests

    [Fact]
    public void Constructor_InitializesIsVialStarredToFalse()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.False(vm.IsVialStarred);
    }

    [Fact]
    public void IsVialStarred_CanBeToggled()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        )
        {
            IsVialStarred = true
        };

        Assert.True(vm.IsVialStarred);
    }

    #endregion

    #region CanAcceptAutoTrackedItem Tests

    [Fact]
    public void CanAcceptAutoTrackedItem_WithMismatchedVialColor_ReturnsFalse()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW, // Yellow vial
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var keyItem = SavedItem.CreateEmpty(ItemName.KEY_1);
        var result = vm.CanAcceptAutoTrackedItem(ItemName.KEY_1, keyItem); // KEY_1 is KEY color

        Assert.False(result);
    }

    [Fact]
    public void CanAcceptAutoTrackedItem_WithMatchingVialColorAndEmptyVial_ReturnsTrue()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.KEY,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var keyItem = SavedItem.CreateEmpty(ItemName.KEY_1);
        var result = vm.CanAcceptAutoTrackedItem(ItemName.KEY_1, keyItem);

        Assert.True(result);
    }

    [Fact]
    public void CanAcceptAutoTrackedItem_WithStartRegionAndMatchingItem_ReturnsTrue()
    {
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.KEY_1))
            .Returns(new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.START });

        var vm = new VialItemViewModel(
            ItemName.KEY_1,
            RegionName.START,
            VialColor.KEY,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var keyItem = SavedItem.CreateEmpty(ItemName.KEY_1);
        var result = vm.CanAcceptAutoTrackedItem(ItemName.KEY_1, keyItem);

        Assert.True(result);
    }

    [Fact]
    public void CanAcceptAutoTrackedItem_WithStartRegionAndDifferentItem_ReturnsFalse()
    {
        var vm = new VialItemViewModel(
            ItemName.KEY_1,
            RegionName.START,
            VialColor.KEY,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var keyItem = SavedItem.CreateEmpty(ItemName.KEY_2);
        var result = vm.CanAcceptAutoTrackedItem(ItemName.KEY_2, keyItem);

        Assert.False(result);
    }

    #endregion

    #region AcceptAutoTrackedItem Tests

    [Fact]
    public void AcceptAutoTrackedItem_ClearsVialStar()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        )
        {
            IsVialStarred = true
        };

        var itemState = new SavedItem { ItemName = ItemName.STRONG_KONG, Region = RegionName.UNKNOWN };
        vm.AcceptAutoTrackedItem(ItemName.STRONG_KONG, itemState);

        Assert.False(vm.IsVialStarred);
    }

    [Fact]
    public void AcceptAutoTrackedItem_UpdatesItemState()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var itemState = new SavedItem { ItemName = ItemName.STRONG_KONG, Region = RegionName.UNKNOWN, Starred = false };
        vm.AcceptAutoTrackedItem(ItemName.STRONG_KONG, itemState);

        _mockItemTrackingService.Verify(
            s => s.SetItemState(ItemName.STRONG_KONG, It.IsAny<SavedItem>(), ChangeReason.AutoTracked),
            Times.Once
        );
    }

    #endregion

    #region AcceptAutoTrackedItemWithoutPersist Tests

    [Fact]
    public void AcceptAutoTrackedItemWithoutPersist_UpdatesViewModelState()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        )
        {
            IsVialStarred = true
        };

        var itemState = new SavedItem { ItemName = ItemName.STRONG_KONG, Region = RegionName.JUNGLE_JAPES, Starred = false };
        vm.AcceptAutoTrackedItemWithoutPersist(ItemName.STRONG_KONG, itemState);

        Assert.False(vm.IsVialStarred);
        Assert.True(vm.IsStarred);
    }

    [Fact]
    public void AcceptAutoTrackedItemWithoutPersist_DoesNotCallSetItemState()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var itemState = new SavedItem { ItemName = ItemName.STRONG_KONG, Region = RegionName.JUNGLE_JAPES };
        vm.AcceptAutoTrackedItemWithoutPersist(ItemName.STRONG_KONG, itemState);

        _mockItemTrackingService.Verify(
            s => s.SetItemState(It.IsAny<ItemName>(), It.IsAny<SavedItem>(), It.IsAny<ChangeReason>()),
            Times.Never
        );
    }

    #endregion

    #region CanAcceptDrop Tests

    [Fact]
    public void CanAcceptDrop_WithStartRegionAndUserDrag_ReturnsFalse()
    {
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.KEY_1))
            .Returns(new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.START });

        var vm = new VialItemViewModel(
            ItemName.KEY_1,
            RegionName.START,
            VialColor.KEY,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var result = vm.CanAcceptDrop(ItemName.KEY_2, MouseDragType.Left);

        Assert.False(result);
    }

    [Fact]
    public void CanAcceptDrop_WithStartRegionAndAutotrackingDrop_ReturnsTrue()
    {
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.KEY_1))
            .Returns(new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.START });

        var vm = new VialItemViewModel(
            ItemName.KEY_1,
            RegionName.START,
            VialColor.KEY,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var result = vm.CanAcceptDrop(ItemName.KEY_1, MouseDragType.None);

        Assert.True(result);
    }

    [Fact]
    public void CanAcceptDrop_WithMismatchedVialColor_ReturnsFalse()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var result = vm.CanAcceptDrop(ItemName.KEY_1, MouseDragType.Left); // KEY color doesn't match YELLOW

        Assert.False(result);
    }

    [Fact]
    public void CanAcceptDrop_WithEmptyVial_ReturnsTrue()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var result = vm.CanAcceptDrop(ItemName.STRONG_KONG, MouseDragType.Left);

        Assert.True(result);
    }

    #endregion

    #region AcceptDropAndMutate Tests

    [Fact]
    public void AcceptDropAndMutate_WithValidDrop_ReturnsTrue()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var result = vm.AcceptDropAndMutate(ItemName.STRONG_KONG, MouseDragType.Left);

        Assert.True(result);
    }

    [Fact]
    public void AcceptDropAndMutate_WithInvalidDrop_ReturnsFalse()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var result = vm.AcceptDropAndMutate(ItemName.KEY_1, MouseDragType.Left); // Wrong color

        Assert.False(result);
    }

    [Fact]
    public void AcceptDropAndMutate_TransfersVialStar()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        )
        {
            IsVialStarred = true
        };

        vm.AcceptDropAndMutate(ItemName.STRONG_KONG, MouseDragType.Left);

        Assert.False(vm.IsVialStarred);
        Assert.True(vm.IsStarred);
    }

    [Fact]
    public void AcceptDropAndMutate_UpdatesItemState()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        vm.AcceptDropAndMutate(ItemName.STRONG_KONG, MouseDragType.Left);

        _mockItemTrackingService.Verify(
            s => s.SetItemState(ItemName.STRONG_KONG, It.IsAny<SavedItem>()),
            Times.Once
        );
    }

    [Fact]
    public void AcceptDropAndMutate_SetsItemRegionToVialRegion()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        vm.AcceptDropAndMutate(ItemName.STRONG_KONG, MouseDragType.Left);

        _mockItemTrackingService.Verify(
            s => s.SetItemState(It.Is<ItemName>(name => name == ItemName.STRONG_KONG),
                               It.Is<SavedItem>(state => state.Region == RegionName.JUNGLE_JAPES)),
            Times.Once
        );
    }

    #endregion

    #region RemoveVialFromRegion Tests

    [Fact]
    public void RemoveVialFromRegionCommand_CanExecute_WithStartRegion_ReturnsFalse()
    {
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.KEY_1))
            .Returns(new SavedItem { ItemName = ItemName.KEY_1, Region = RegionName.START });

        var vm = new VialItemViewModel(
            ItemName.KEY_1,
            RegionName.START,
            VialColor.KEY,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.False(vm.RemoveVialFromRegionCommand.CanExecute(null));
    }

    [Fact]
    public void RemoveVialFromRegionCommand_CanExecute_WithEmptyVial_ReturnsFalse()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.False(vm.RemoveVialFromRegionCommand.CanExecute(null));
    }

    [Fact]
    public void RemoveVialFromRegionCommand_CanExecute_WithOccupiedNonStartVial_ReturnsTrue()
    {
        var itemState = new SavedItem
        {
            ItemName = ItemName.STRONG_KONG,
            Region = RegionName.JUNGLE_JAPES,
            Autotracked = false
        };

        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.STRONG_KONG))
            .Returns(itemState);

        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Simulate the item being in the vial
        vm.AcceptAutoTrackedItem(ItemName.STRONG_KONG, itemState);

        Assert.True(vm.RemoveVialFromRegionCommand.CanExecute(null));
    }

    [Fact]
    public void RemoveVialFromRegionCommand_CanExecute_WithAutoTrackedItem_ReturnsFalse()
    {
        var autoTrackedState = new SavedItem
        {
            ItemName = ItemName.STRONG_KONG,
            Region = RegionName.JUNGLE_JAPES,
            Autotracked = true
        };

        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.STRONG_KONG))
            .Returns(autoTrackedState);

        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        vm.AcceptAutoTrackedItem(ItemName.STRONG_KONG, autoTrackedState);

        Assert.False(vm.RemoveVialFromRegionCommand.CanExecute(null));
    }

    [Fact]
    public void RemoveVialFromRegionCommand_CanExecute_WithNullItemState_ReturnsFalse()
    {
        _mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>()))
            .Returns((SavedItem?)null);

        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.False(vm.RemoveVialFromRegionCommand.CanExecute(null));
    }

    [Fact]
    public void RemoveVialFromRegionCommand_Execute_RemovesItemFromVial()
    {
        var itemState = new SavedItem
        {
            ItemName = ItemName.STRONG_KONG,
            Region = RegionName.JUNGLE_JAPES,
            Autotracked = false
        };

        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.STRONG_KONG))
            .Returns(itemState);

        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        vm.AcceptAutoTrackedItem(ItemName.STRONG_KONG, itemState);
        vm.RemoveVialFromRegionCommand.Execute(null);

        _mockItemTrackingService.Verify(
            s => s.SetItemState(
                ItemName.STRONG_KONG,
                It.Is<SavedItem>(state => state.Region == RegionName.UNKNOWN),
                It.IsAny<ChangeReason>()
            ),
            Times.Once
        );
    }

    [Fact]
    public void RemoveVialFromRegionCommand_Execute_ClearsCurrentItem()
    {
        var itemState = new SavedItem
        {
            ItemName = ItemName.STRONG_KONG,
            Region = RegionName.JUNGLE_JAPES,
            Autotracked = false
        };

        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.STRONG_KONG))
            .Returns(itemState);

        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        vm.AcceptAutoTrackedItem(ItemName.STRONG_KONG, itemState);
        Assert.Equal(ItemName.STRONG_KONG, vm.CurrentItemName);

        vm.RemoveVialFromRegionCommand.Execute(null);

        Assert.Null(vm.CurrentItemName);
    }

    [Fact]
    public void RemoveVialFromRegionCommand_Execute_SetsItemRegionToUnknown()
    {
        var itemState = new SavedItem
        {
            ItemName = ItemName.STRONG_KONG,
            Region = RegionName.JUNGLE_JAPES,
            Autotracked = false
        };

        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.STRONG_KONG))
            .Returns(itemState);

        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        vm.AcceptAutoTrackedItem(ItemName.STRONG_KONG, itemState);
        vm.RemoveVialFromRegionCommand.Execute(null);

        _mockItemTrackingService.Verify(
            s => s.SetItemState(
                It.IsAny<ItemName>(),
                It.Is<SavedItem>(state => state.Region == RegionName.UNKNOWN && state.Opacity == 1),
                It.IsAny<ChangeReason>()
            ),
            Times.Once
        );
    }

    [Fact]
    public void RemoveVialFromRegionCommand_Execute_RaisesVialItemStateChangedEvent()
    {
        var itemState = new SavedItem
        {
            ItemName = ItemName.STRONG_KONG,
            Region = RegionName.JUNGLE_JAPES,
            Autotracked = false
        };

        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.STRONG_KONG))
            .Returns(itemState);

        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        VialItemStateChangedEventArgs? capturedArgs = null;
        vm.VialItemStateChanged += (s, e) => capturedArgs = e;

        vm.AcceptAutoTrackedItem(ItemName.STRONG_KONG, itemState);
        vm.RemoveVialFromRegionCommand.Execute(null);

        Assert.NotNull(capturedArgs);
        Assert.Null(capturedArgs.ItemName);
    }

    #endregion

    #region ToggleVialStar Tests

    [Fact]
    public void ToggleVialStarCommand_CanBeExecuted()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.True(vm.ToggleVialStarCommand.CanExecute(null));
    }

    [Fact]
    public void ToggleVialStarCommand_WithEmptyVial_TogglesVialStar()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        vm.ToggleVialStarCommand.Execute(null);

        Assert.True(vm.IsVialStarred);
    }

    [Fact]
    public void ToggleVialStarCommand_WithEmptyVial_ToggleTwice_ReturnsToUnstarred()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        vm.ToggleVialStarCommand.Execute(null);
        Assert.True(vm.IsVialStarred);

        vm.ToggleVialStarCommand.Execute(null);
        Assert.False(vm.IsVialStarred);
    }

    [Fact]
    public void ToggleVialStarCommand_WithOccupiedVial_TogglesItemStar()
    {
        var itemState = new SavedItem { ItemName = ItemName.STRONG_KONG, Region = RegionName.JUNGLE_JAPES, Starred = false };
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.STRONG_KONG))
            .Returns(itemState);

        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Simulate acceptance of an autotracked item which sets CurrentItemName
        vm.AcceptAutoTrackedItem(ItemName.STRONG_KONG, itemState);

        vm.ToggleVialStarCommand.Execute(null);

        _mockItemTrackingService.Verify(s => s.ToggleStar(ItemName.STRONG_KONG), Times.Once);
    }

    [Fact]
    public void ToggleVialStarCommand_WithEmptyVial_RaisesVialItemStateChangedEvent()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        VialItemStateChangedEventArgs? capturedArgs = null;
        vm.VialItemStateChanged += (s, e) => capturedArgs = e;

        vm.ToggleVialStarCommand.Execute(null);

        Assert.NotNull(capturedArgs);
        Assert.Equal(RegionName.JUNGLE_JAPES, capturedArgs.RegionName);
        Assert.Equal(VialColor.YELLOW, capturedArgs.VialColor);
        Assert.True(capturedArgs.IsStarred);
        Assert.Null(capturedArgs.ItemName);
    }

    [Fact]
    public void ToggleVialStarCommand_WithOccupiedVial_RaisesVialItemStateChangedEvent()
    {
        var itemState = new SavedItem { ItemName = ItemName.STRONG_KONG, Region = RegionName.JUNGLE_JAPES, Starred = false };
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.STRONG_KONG))
            .Returns(itemState);

        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        vm.AcceptAutoTrackedItem(ItemName.STRONG_KONG, itemState);

        VialItemStateChangedEventArgs? capturedArgs = null;
        vm.VialItemStateChanged += (s, e) => capturedArgs = e;

        vm.ToggleVialStarCommand.Execute(null);

        Assert.NotNull(capturedArgs);
        Assert.Equal(ItemName.STRONG_KONG, capturedArgs.ItemName);
    }

    [Fact]
    public void ToggleVialStarCommand_AlwaysCanBeExecuted()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Toggle star command should always be executable
        Assert.True(vm.ToggleVialStarCommand.CanExecute(null));
    }

    #endregion

    #region OnItemStateChanged Tests

    #endregion

    #region VialItemStateChanged Event Tests

    [Fact]
    public void ToggleVialStarCommand_RaisesVialItemStateChangedEvent()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        VialItemStateChangedEventArgs? capturedArgs = null;
        vm.VialItemStateChanged += (s, e) => capturedArgs = e;

        vm.ToggleVialStarCommand.Execute(null);

        Assert.NotNull(capturedArgs);
        Assert.Equal(RegionName.JUNGLE_JAPES, capturedArgs.RegionName);
        Assert.Equal(VialColor.YELLOW, capturedArgs.VialColor);
        Assert.True(capturedArgs.IsStarred);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameRegionAndVialColor_ReturnsTrue()
    {
        var vm1 = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var vm2 = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.Equal(vm1, vm2);
    }

    [Fact]
    public void Equals_WithDifferentVialColor_ReturnsFalse()
    {
        var vm1 = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var vm2 = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.RED,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.NotEqual(vm1, vm2);
    }

    [Fact]
    public void Equals_WithDifferentRegion_ReturnsFalse()
    {
        var vm1 = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var vm2 = new VialItemViewModel(
            null,
            RegionName.ANGRY_AZTEC,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.NotEqual(vm1, vm2);
    }

    [Fact]
    public void GetHashCode_WithSameRegionAndVialColor_ReturnsSameHash()
    {
        var vm1 = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var vm2 = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.Equal(vm1.GetHashCode(), vm2.GetHashCode());
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithEmptyVial_FormatsCorrectly()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var result = vm.ToString();

        Assert.Contains("YELLOW", result);
        Assert.Contains("JUNGLE_JAPES", result);
    }

    [Fact]
    public void ToString_WithEmptyVialAndStar_IncludesAsterisk()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        )
        {
            IsVialStarred = true
        };

        var result = vm.ToString();

        Assert.Contains("YELLOW*", result);
    }

    [Fact]
    public void ToString_WithOccupiedVial_IncludesItemName()
    {
        var itemState = new SavedItem { ItemName = ItemName.STRONG_KONG, Region = RegionName.JUNGLE_JAPES, Starred = false };
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.STRONG_KONG))
            .Returns(itemState);

        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        vm.AcceptAutoTrackedItem(ItemName.STRONG_KONG, itemState);

        var result = vm.ToString();

        Assert.Contains("STRONG_KONG", result);
    }

    #endregion

    #region IVialSlot Interface Tests

    [Fact]
    public void VialColor_ReturnsConstructorValue()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.RED,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.Equal(VialColor.RED, vm.VialColor);
    }

    [Fact]
    public void ImplementsIVialSlot()
    {
        var vm = new VialItemViewModel(
            null,
            RegionName.JUNGLE_JAPES,
            VialColor.YELLOW,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.IsType<IVialSlot>(vm, exactMatch: false);
    }

    #endregion
}
