namespace TrackOMatic.ViewModels.Test;

/// <summary>
/// Test suite for UiItemViewModel.
/// Tests drag/drop operations (BeginDrag, EndDrag, CompleteDrag),
/// command execution (ToggleStar, RemoveFromRegion with gating),
/// image resource key updates with various item states,
/// event handling with synchronization context,
/// and equality/disposal mechanics.
/// </summary>
public class UiItemViewModelTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            new UiItemViewModel(
                ItemName.DONKEY_KROOL,
                null!,
                mockSpoilerDataService.Object,
                mockProgressProvider.Object,
                mockThemeService.Object,
                mockDataSaverService.Object
            )
        );

        Assert.Equal("itemTrackingService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullServices_ThrowsArgumentNullException()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            new UiItemViewModel(
                ItemName.DONKEY_KROOL,
                mockItemTrackingService.Object,
                null!,
                mockProgressProvider.Object,
                mockThemeService.Object,
                mockDataSaverService.Object
            )
        );

        Assert.Equal("parsedSpoilerDataService", exception.ParamName);
    }

    [Fact]
    public void Constructor_InitializesPropertiesToDefaults()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY_KROOL)).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        Assert.False(viewModel.IsDragging);
        Assert.Equal(0.0, viewModel.DragX);
        Assert.Equal(0.0, viewModel.DragY);
        Assert.Equal(0.0, viewModel.DragWidth);
        Assert.Equal(0.0, viewModel.DragHeight);
        Assert.Equal(1.0, viewModel.Opacity);
        Assert.Equal(RegionName.UNKNOWN, viewModel.RegionName);
        Assert.Equal(MouseDragType.None, viewModel.MouseDragType);

        viewModel.Dispose();
    }

    [Fact]
    public void Constructor_WithSavedItem_InitializesFromState()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var savedItem = new SavedItem(
            ItemName.DONKEY_KROOL,
            RegionName.JUNGLE_JAPES,
            starred: true,
            autotracked: false,
            opacity: 0.75,
            hinted: false
        );

        mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY_KROOL)).Returns(savedItem);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        Assert.True(viewModel.IsStarred);
        Assert.Equal(0.75, viewModel.Opacity);
        Assert.Equal(RegionName.JUNGLE_JAPES, viewModel.RegionName);

        viewModel.Dispose();
    }

    #endregion

    #region BeginDrag Tests

    [Theory]
    [InlineData(MouseDragType.Left, 1.0)]
    [InlineData(MouseDragType.Right, 0.375)]
    public void BeginDrag_SetsOpacityBasedOnDragType(MouseDragType dragType, double expectedOpacity)
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        viewModel.BeginDrag(dragType, 100, 150);

        Assert.Equal(expectedOpacity, viewModel.Opacity);
        Assert.Equal(dragType, viewModel.MouseDragType);
        Assert.Equal(100, viewModel.DragWidth);
        Assert.Equal(150, viewModel.DragHeight);
        Assert.True(viewModel.IsDragging);

        viewModel.Dispose();
    }

    [Fact]
    public void BeginDrag_WithNoneDragType_SetsOpacityToOne()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        viewModel.BeginDrag(MouseDragType.None, 50, 75);

        Assert.Equal(1.0, viewModel.Opacity);
        Assert.Equal(MouseDragType.None, viewModel.MouseDragType);

        viewModel.Dispose();
    }

    #endregion

    #region EndDrag Tests

    [Fact]
    public void EndDrag_ResetsPropertiesToDefaults()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        viewModel.BeginDrag(MouseDragType.Left, 100, 150);
        viewModel.EndDrag();

        Assert.False(viewModel.IsDragging);
        Assert.Equal(MouseDragType.None, viewModel.MouseDragType);
        Assert.Equal(1.0, viewModel.Opacity); // Restored to default

        viewModel.Dispose();
    }

    [Fact]
    public void EndDrag_RestoresOpacityFromSavedItem()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var savedItem = new SavedItem(
            ItemName.DONKEY_KROOL,
            RegionName.JUNGLE_JAPES,
            starred: false,
            autotracked: false,
            opacity: 0.75,
            hinted: false
        );

        mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY_KROOL)).Returns(savedItem);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        viewModel.BeginDrag(MouseDragType.Right, 100, 150);
        Assert.Equal(0.375, viewModel.Opacity);

        viewModel.EndDrag();

        Assert.Equal(0.75, viewModel.Opacity); // Restored from saved item
        Assert.False(viewModel.IsDragging);

        viewModel.Dispose();
    }

    #endregion

    #region CompleteDrag Tests

    [Theory]
    [InlineData(MouseDragType.Left, 1.0)]
    [InlineData(MouseDragType.Right, 0.375)]
    public void CompleteDrag_CallsSetItemStateWithNewRegion(MouseDragType dragType, double expectedOpacity)
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var previousItem = new SavedItem(
            ItemName.DONKEY_KROOL,
            RegionName.JUNGLE_JAPES,
            starred: true,
            autotracked: false,
            opacity: 1.0,
            hinted: false
        );

        mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY_KROOL)).Returns(previousItem);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        viewModel.BeginDrag(dragType, 100, 150);
        viewModel.CompleteDrag(RegionName.ANGRY_AZTEC);

        // Verify SetItemState was called with new region and updated opacity
        mockItemTrackingService.Verify(
            s => s.SetItemState(
                ItemName.DONKEY_KROOL,
                It.Is<SavedItem>(item =>
                    item.ItemName == ItemName.DONKEY_KROOL &&
                    item.Region == RegionName.ANGRY_AZTEC &&
                    item.Opacity == expectedOpacity &&
                    item.Starred == previousItem.Starred &&
                    item.Autotracked == previousItem.Autotracked
                )
            ),
            Times.Once
        );

        Assert.False(viewModel.IsDragging);
        Assert.Equal(MouseDragType.None, viewModel.MouseDragType);

        viewModel.Dispose();
    }

    [Fact]
    public void CompleteDrag_WithNoPreviousItem_CreatesEmptyItem()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY_KROOL)).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        viewModel.BeginDrag(MouseDragType.Left, 100, 150);
        viewModel.CompleteDrag(RegionName.JUNGLE_JAPES);

        // Verify SetItemState was called with the new region
        mockItemTrackingService.Verify(
            s => s.SetItemState(
                ItemName.DONKEY_KROOL,
                It.Is<SavedItem>(item =>
                    item.ItemName == ItemName.DONKEY_KROOL &&
                    item.Region == RegionName.JUNGLE_JAPES
                )
            ),
            Times.Once
        );

        viewModel.Dispose();
    }

    #endregion

    #region ToggleStar Command Tests

    [Fact]
    public void ToggleStarCommand_TogglesItemStarState()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        viewModel.ToggleStarCommand.Execute(null);

        mockItemTrackingService.Verify(s => s.ToggleStar(ItemName.DONKEY_KROOL), Times.Once);

        viewModel.Dispose();
    }

    [Fact]
    public void ToggleStarCommand_AlwaysExecutable()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        Assert.True(viewModel.ToggleStarCommand.CanExecute(null));

        viewModel.Dispose();
    }

    #endregion

    #region RemoveFromRegion Command Tests

    [Fact]
    public void RemoveFromRegionCommand_CanExecute_WithoutItemState_ReturnsFalse()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        Assert.False(viewModel.RemoveFromRegionCommand.CanExecute(null));

        viewModel.Dispose();
    }

    [Fact]
    public void RemoveFromRegionCommand_CanExecute_WithAutotracked_ReturnsFalse()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var autoTrackedItem = new SavedItem(
            ItemName.DONKEY_KROOL,
            RegionName.JUNGLE_JAPES,
            starred: false,
            autotracked: true,
            opacity: 1.0,
            hinted: false
        );

        mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY_KROOL)).Returns(autoTrackedItem);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        Assert.False(viewModel.RemoveFromRegionCommand.CanExecute(null));

        viewModel.Dispose();
    }

    [Fact]
    public void RemoveFromRegionCommand_CanExecute_WithValidItem_ReturnsTrue()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var validItem = new SavedItem(
            ItemName.DONKEY_KROOL,
            RegionName.JUNGLE_JAPES,
            starred: false,
            autotracked: false,
            opacity: 1.0,
            hinted: false
        );

        mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY_KROOL)).Returns(validItem);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        Assert.True(viewModel.RemoveFromRegionCommand.CanExecute(null));

        viewModel.Dispose();
    }

    [Fact]
    public void RemoveFromRegionCommand_Execute_ClearsItemState()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var validItem = new SavedItem(
            ItemName.DONKEY_KROOL,
            RegionName.JUNGLE_JAPES,
            starred: false,
            autotracked: false,
            opacity: 1.0,
            hinted: false
        );

        mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY_KROOL)).Returns(validItem);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        viewModel.RemoveFromRegionCommand.Execute(null);

        mockItemTrackingService.Verify(s => s.ClearItemState(ItemName.DONKEY_KROOL), Times.Once);

        viewModel.Dispose();
    }

    #endregion

    #region UpdateImageResourceKey Tests

    [Fact]
    public void UpdateImageResourceKey_WhenDragging_UsesFullColorVariant()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        viewModel.BeginDrag(MouseDragType.Left, 100, 150);

        // When dragging, should use full color
        Assert.Equal("donkey_krool", viewModel.ImageResourceKey);

        viewModel.Dispose();
    }

    [Fact]
    public void UpdateImageResourceKey_WhenNotDragging_WithNoItemState_UsesBwVariant()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        // No item state, should use B&W
        Assert.Equal("donkey_krool_bw", viewModel.ImageResourceKey);

        viewModel.Dispose();
    }

    [Fact]
    public void UpdateImageResourceKey_WithValidRegion_UsesFullColorVariant()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var itemWithRegion = new SavedItem(
            ItemName.DONKEY_KROOL,
            RegionName.JUNGLE_JAPES,
            starred: false,
            autotracked: false,
            opacity: 1.0,
            hinted: false
        );

        mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY_KROOL)).Returns(itemWithRegion);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        // Item in valid region should use full color
        Assert.Equal("donkey_krool", viewModel.ImageResourceKey);

        viewModel.Dispose();
    }

    [Fact]
    public void UpdateImageResourceKey_WhenHinted_UsesBwVariant()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        var hintedItem = new SavedItem(
            ItemName.DONKEY_KROOL,
            RegionName.JUNGLE_JAPES,
            starred: false,
            autotracked: false,
            opacity: 1.0,
            hinted: true
        );

        mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY_KROOL)).Returns(hintedItem);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        // Hinted items use B&W
        Assert.Equal("donkey_krool_bw", viewModel.ImageResourceKey);

        viewModel.Dispose();
    }

    [Fact]
    public void UpdateImageResourceKey_WhenEmpty_UsesBwVariant()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        // Empty item: not starred, not in any region, not hinted
        var emptyItem = new SavedItem(
            ItemName.DONKEY_KROOL,
            RegionName.UNKNOWN,
            starred: false,
            autotracked: false,
            opacity: 1.0,
            hinted: false
        );

        mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY_KROOL)).Returns(emptyItem);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        // Empty items use B&W
        Assert.Equal("donkey_krool_bw", viewModel.ImageResourceKey);

        viewModel.Dispose();
    }

    #endregion

    #region Event Handling Tests

    [Fact]
    public void OnItemStateChanged_UpdatesPropertiesWhenItemMatchesName()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        var updatedItem = new SavedItem(
            ItemName.DONKEY_KROOL,
            RegionName.JUNGLE_JAPES,
            starred: true,
            autotracked: false,
            opacity: 0.5,
            hinted: false
        );

        var eventArgs = new ItemStateChangedEventArgs(updatedItem);
        mockItemTrackingService.Raise(s => s.ItemStateChanged += null, eventArgs);

        Assert.True(viewModel.IsStarred);
        Assert.Equal(0.5, viewModel.Opacity);

        viewModel.Dispose();
    }

    [Fact]
    public void OnItemStateChanged_IgnoresUnrelatedItems()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        var unrelatedItem = new SavedItem(
            ItemName.DIDDY_KROOL,
            RegionName.JUNGLE_JAPES,
            starred: true,
            autotracked: false,
            opacity: 0.5,
            hinted: false
        );

        var eventArgs = new ItemStateChangedEventArgs(unrelatedItem);
        mockItemTrackingService.Raise(s => s.ItemStateChanged += null, eventArgs);

        // Should not update because item name doesn't match
        Assert.False(viewModel.IsStarred);

        viewModel.Dispose();
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameItemName_ReturnsTrue()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel1 = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        var viewModel2 = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        Assert.Equal(viewModel1, viewModel2);

        viewModel1.Dispose();
        viewModel2.Dispose();
    }

    [Fact]
    public void Equals_WithDifferentItemName_ReturnsFalse()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel1 = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        var viewModel2 = new UiItemViewModel(
            ItemName.DIDDY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        Assert.NotEqual(viewModel1, viewModel2);

        viewModel1.Dispose();
        viewModel2.Dispose();
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnhooksEventHandlers()
    {
        var mockItemTrackingService = new Mock<IItemTrackingService>();
        var mockSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        var mockProgressProvider = new Mock<ISavedProgressProvider>();
        var mockThemeService = new Mock<IThemeService>();
        var mockDataSaverService = new Mock<IDataSaverService>();

        mockItemTrackingService.Setup(s => s.GetItemState(It.IsAny<ItemName>())).Returns((SavedItem?)null);

        var viewModel = new UiItemViewModel(
            ItemName.DONKEY_KROOL,
            mockItemTrackingService.Object,
            mockSpoilerDataService.Object,
            mockProgressProvider.Object,
            mockThemeService.Object,
            mockDataSaverService.Object
        );

        viewModel.Dispose();

        mockItemTrackingService.VerifyAdd(s => s.ItemStateChanged += It.IsAny<EventHandler<ItemStateChangedEventArgs>>(), Times.Once);
        mockSpoilerDataService.VerifyAdd(s => s.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
        mockProgressProvider.VerifyAdd(s => s.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
        mockDataSaverService.VerifyAdd(s => s.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    #endregion
}
