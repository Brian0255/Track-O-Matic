namespace TrackOMatic.ViewModels.Test.Hints;

public class FoundHintItemListViewModelTests : IDisposable
{
    private readonly Mock<IItemTrackingService> _mockItemTrackingService;
    private readonly Mock<IHintItemSelectionService> _mockHintItemSelectionService;
    private readonly Mock<IHintService> _mockHintService;
    private readonly Mock<ISavedProgressProvider> _mockSavedProgressProvider;
    private readonly HintId _testHintId = new("testhint");
    private const string TestDialogTitle = "Test Dialog";

    public FoundHintItemListViewModelTests()
    {
        _mockItemTrackingService = new Mock<IItemTrackingService>();
        _mockHintItemSelectionService = new Mock<IHintItemSelectionService>();
        _mockHintService = new Mock<IHintService>();
        _mockSavedProgressProvider = new Mock<ISavedProgressProvider>();

        // Default GetHint to return a hint with some found items
        _mockHintService.Setup(s => s.GetHint(_testHintId)).Returns(new SavedHint
        {
            FoundItems = new Dictionary<ItemName, bool>
            {
                { ItemName.DONKEY, true },
                { ItemName.DIDDY, false }
            }
        });
    }

    [Fact]
    public void Constructor_WithValidParameters_Succeeds()
    {
        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );
        Assert.NotNull(vm);
    }

    [Fact]
    public void Constructor_WithNullDialogTitle_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new FoundHintItemListViewModel(
                HintPanelType.Isles,
                _testHintId,
                null!,
                _mockItemTrackingService.Object,
                _mockHintItemSelectionService.Object,
                _mockHintService.Object,
                _mockSavedProgressProvider.Object
            )
        );
        Assert.Equal("dialogTitle", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new FoundHintItemListViewModel(
                HintPanelType.Isles,
                _testHintId,
                TestDialogTitle,
                null!,
                _mockHintItemSelectionService.Object,
                _mockHintService.Object,
                _mockSavedProgressProvider.Object
            )
        );
        Assert.Equal("itemTrackingService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullHintItemSelectionService_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new FoundHintItemListViewModel(
                HintPanelType.Isles,
                _testHintId,
                TestDialogTitle,
                _mockItemTrackingService.Object,
                null!,
                _mockHintService.Object,
                _mockSavedProgressProvider.Object
            )
        );
        Assert.Equal("hintItemSelectionService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullHintService_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new FoundHintItemListViewModel(
                HintPanelType.Isles,
                _testHintId,
                TestDialogTitle,
                _mockItemTrackingService.Object,
                _mockHintItemSelectionService.Object,
                null!,
                _mockSavedProgressProvider.Object
            )
        );
        Assert.Equal("hintService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new FoundHintItemListViewModel(
                HintPanelType.Isles,
                _testHintId,
                TestDialogTitle,
                _mockItemTrackingService.Object,
                _mockHintItemSelectionService.Object,
                _mockHintService.Object,
                null!
            )
        );
        Assert.Equal("savedProgressProvider", exception.ParamName);
    }

    [Fact]
    public void Constructor_InitializesFoundItems()
    {
        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.NotEmpty(vm.FoundItems);
        Assert.Equal(2, vm.FoundItems.Count);
    }

    [Theory]
    [InlineData(HintPanelType.Foolish, false)]
    [InlineData(HintPanelType.PotionCounts, false)]
    [InlineData(HintPanelType.Isles, true)]
    [InlineData(HintPanelType.Paths, true)]
    public void Constructor_SetsShouldShowFocusListBasedOnHintPanelType(HintPanelType hintPanelType, bool expectedShow)
    {
        var vm = new FoundHintItemListViewModel(
            hintPanelType,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.Equal(expectedShow, vm.ShouldShowFocusList);
    }

    [Fact]
    public void RefreshRowSplits_WithZeroItems_AddsNonePlaceholder()
    {
        _mockHintService.Setup(s => s.GetHint(_testHintId))
            .Returns(new SavedHint { FoundItems = [] });

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.Multiple(() =>
        {
            Assert.Single(vm.TopRowItems);
            Assert.Equal(ItemName.NONE, vm.TopRowItems[0].ItemName);
            Assert.Empty(vm.BottomRowItems);
        });
    }

    [Fact]
    public void RefreshRowSplits_WithOneItem_PlacesInTopRow()
    {
        _mockHintService.Setup(s => s.GetHint(_testHintId))
            .Returns(new SavedHint
            {
                FoundItems = new Dictionary<ItemName, bool> { { ItemName.DONKEY, true } }
            });

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.Multiple(() =>
        {
            Assert.Single(vm.TopRowItems);
            Assert.Equal(ItemName.DONKEY, vm.TopRowItems[0].ItemName);
            Assert.Empty(vm.BottomRowItems);
        });
    }

    [Fact]
    public void RefreshRowSplits_WithTwoItems_BothInTopRow()
    {
        _mockHintService.Setup(s => s.GetHint(_testHintId))
            .Returns(new SavedHint
            {
                FoundItems = new Dictionary<ItemName, bool>
                {
                    { ItemName.DONKEY, true },
                    { ItemName.DIDDY, false }
                }
            });

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.Equal(2, vm.TopRowItems.Count);
        Assert.Empty(vm.BottomRowItems);
    }

    [Fact]
    public void RefreshRowSplits_WithThreeItems_SplitBetweenRows()
    {
        _mockHintService.Setup(s => s.GetHint(_testHintId))
            .Returns(new SavedHint
            {
                FoundItems = new Dictionary<ItemName, bool>
                {
                    { ItemName.DONKEY, true },
                    { ItemName.DIDDY, false },
                    { ItemName.LANKY, true }
                }
            });

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.Equal(2, vm.TopRowItems.Count);
        Assert.Single(vm.BottomRowItems);
    }

    [Theory]
    [InlineData(HintPanelType.Foolish, false, 1)]
    [InlineData(HintPanelType.PotionCounts, false, 1)]
    [InlineData(HintPanelType.Paths, false, 2)]  // Paths always sets RowsToShow=2, but ShouldShowSecondRow based on item count
    [InlineData(HintPanelType.Isles, false, 1)]
    public void RefreshRowSplits_SetsRowVisibilityBasedOnPanelType(HintPanelType hintPanelType, bool expectedShowSecondRow, int expectedRowsToShow)
    {
        var vm = new FoundHintItemListViewModel(
            hintPanelType,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.Equal(expectedShowSecondRow, vm.ShouldShowSecondRow);
        Assert.Equal(expectedRowsToShow, vm.RowsToShow);
    }

    [Theory]
    [InlineData(HintPanelType.Foolish, false)]
    [InlineData(HintPanelType.Isles, true)]
    [InlineData(HintPanelType.Paths, true)]
    [InlineData(HintPanelType.PotionCounts, true)]
    public void IsHintInfoNotFoolish_ReturnsBasedOnPanelType(HintPanelType hintPanelType, bool expected)
    {
        var vm = new FoundHintItemListViewModel(
            hintPanelType,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.Equal(expected, vm.IsHintInfoNotFoolish);
    }

    [Fact]
    public void ToString_IncludesAllFoundItems()
    {
        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        var result = vm.ToString();
        Assert.Multiple(() =>
        {
            Assert.Contains("Items found:", result);
            Assert.Contains("DONKEY", result);
            Assert.Contains("DIDDY", result);
        });
    }

    [Fact]
    public void ToString_WithEmptyItems_IsEmpty()
    {
        _mockHintService.Setup(s => s.GetHint(_testHintId))
            .Returns(new SavedHint { FoundItems = [] });

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        var result = vm.ToString();
        // ToString joins actual FoundItems which is empty, so only shows "Items found: "
        Assert.Equal("Items found: ", result);
    }

    #region SelectFoundItems Command

    [Fact]
    public async Task SelectFoundItems_WithValidSelection_CallsHintItemSelectionService()
    {
        var selectedCells = new List<HintGridCell>
        {
            new(0, 1, ItemName.DONKEY),
            new(1, 1, ItemName.DIDDY)
        };

        _mockHintItemSelectionService.Setup(s => s.SelectItemsAsync(
            It.IsAny<IReadOnlyList<HintGridCell>>(),
            It.IsAny<List<ItemName>>(),
            TestDialogTitle,
            It.IsAny<double>(),
            It.IsAny<double>()
        )).ReturnsAsync(selectedCells);

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        await vm.SelectFoundItemsCommand.ExecuteAsync((100.0, 200.0));

        _mockHintItemSelectionService.Verify(s => s.SelectItemsAsync(
            It.IsAny<IReadOnlyList<HintGridCell>>(),
            It.Is<List<ItemName>>(list => list.Contains(ItemName.DONKEY) && list.Contains(ItemName.DIDDY)),
            TestDialogTitle,
            100.0,
            200.0
        ), Times.Once);
    }

    [Fact]
    public async Task SelectFoundItems_WithValidSelection_CallsSetFoundItems()
    {
        var selectedCells = new List<HintGridCell>
        {
            new(0, 1, ItemName.DONKEY),
            new(1, 1, ItemName.DIDDY)
        };

        _mockHintItemSelectionService.Setup(s => s.SelectItemsAsync(
            It.IsAny<IReadOnlyList<HintGridCell>>(),
            It.IsAny<List<ItemName>>(),
            It.IsAny<string>(),
            It.IsAny<double>(),
            It.IsAny<double>()
        )).ReturnsAsync(selectedCells);

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        await vm.SelectFoundItemsCommand.ExecuteAsync((100.0, 200.0));

        _mockHintService.Verify(s => s.SetFoundItems(
            _testHintId,
            It.Is<Dictionary<ItemName, bool>>(dict =>
                dict.ContainsKey(ItemName.DONKEY) &&
                dict.ContainsKey(ItemName.DIDDY) &&
                dict[ItemName.DONKEY] == true && // Was found in original
                dict[ItemName.DIDDY] == false    // Was not found in original
            ),
            HintPanelType.Isles
        ), Times.Once);
    }

    [Fact]
    public async Task SelectFoundItems_WithPlayableRegion_UpdatesItemTracking()
    {
        var selectedCells = new List<HintGridCell>
        {
            new(0, 1, ItemName.DONKEY)
        };

        _mockHintItemSelectionService.Setup(s => s.SelectItemsAsync(
            It.IsAny<IReadOnlyList<HintGridCell>>(),
            It.IsAny<List<ItemName>>(),
            It.IsAny<string>(),
            It.IsAny<double>(),
            It.IsAny<double>()
        )).ReturnsAsync(selectedCells);

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles, // Isles is a playable region
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        await vm.SelectFoundItemsCommand.ExecuteAsync((100.0, 200.0));

        _mockItemTrackingService.Verify(s => s.SetItemState(
            ItemName.DONKEY,
            It.Is<SavedItem>(item =>
                item.Region == RegionName.DK_ISLES &&
                item.Hinted == true
            )
        ), Times.Once);
    }

    [Fact]
    public async Task SelectFoundItems_WithNonPlayableRegion_DoesNotUpdateItemTracking()
    {
        var selectedCells = new List<HintGridCell>
        {
            new(0, 1, ItemName.DONKEY)
        };

        _mockHintItemSelectionService.Setup(s => s.SelectItemsAsync(
            It.IsAny<IReadOnlyList<HintGridCell>>(),
            It.IsAny<List<ItemName>>(),
            It.IsAny<string>(),
            It.IsAny<double>(),
            It.IsAny<double>()
        )).ReturnsAsync(selectedCells);

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Foolish, // Foolish is not a playable region
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        await vm.SelectFoundItemsCommand.ExecuteAsync((100.0, 200.0));

        _mockItemTrackingService.Verify(s => s.SetItemState(
            It.IsAny<ItemName>(),
            It.IsAny<SavedItem>()
        ), Times.Never);
    }

    [Fact]
    public async Task SelectFoundItems_PreservesExistingFoundState()
    {
        // Setup with existing items where DONKEY is found and DIDDY is not
        _mockHintService.Setup(s => s.GetHint(_testHintId)).Returns(new SavedHint
        {
            FoundItems = new Dictionary<ItemName, bool>
            {
                { ItemName.DONKEY, true },
                { ItemName.DIDDY, false }
            }
        });

        var selectedCells = new List<HintGridCell>
        {
            new(0, 1, ItemName.DONKEY),
            new(1, 1, ItemName.DIDDY)
        };

        _mockHintItemSelectionService.Setup(s => s.SelectItemsAsync(
            It.IsAny<IReadOnlyList<HintGridCell>>(),
            It.IsAny<List<ItemName>>(),
            It.IsAny<string>(),
            It.IsAny<double>(),
            It.IsAny<double>()
        )).ReturnsAsync(selectedCells);

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        await vm.SelectFoundItemsCommand.ExecuteAsync((100.0, 200.0));

        _mockHintService.Verify(s => s.SetFoundItems(
            _testHintId,
            It.Is<Dictionary<ItemName, bool>>(dict =>
                dict[ItemName.DONKEY] == true &&  // Preserve found state
                dict[ItemName.DIDDY] == false     // Preserve not-found state
            ),
            HintPanelType.Isles
        ), Times.Once);
    }

    [Fact]
    public async Task SelectFoundItems_WithEmptySelection_CallsSetFoundItems()
    {
        _mockHintItemSelectionService.Setup(s => s.SelectItemsAsync(
            It.IsAny<IReadOnlyList<HintGridCell>>(),
            It.IsAny<List<ItemName>>(),
            It.IsAny<string>(),
            It.IsAny<double>(),
            It.IsAny<double>()
        )).ReturnsAsync([]);

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        await vm.SelectFoundItemsCommand.ExecuteAsync((100.0, 200.0));

        _mockHintService.Verify(s => s.SetFoundItems(
            _testHintId,
            It.Is<Dictionary<ItemName, bool>>(dict => dict.Count == 0),
            HintPanelType.Isles
        ), Times.Once);
    }

    [Fact]
    public async Task SelectFoundItems_UnknownRegionItem_SkipsItemTracking()
    {
        var selectedCells = new List<HintGridCell>
        {
            new(0, 1, ItemName.DONKEY)
        };

        _mockHintItemSelectionService.Setup(s => s.SelectItemsAsync(
            It.IsAny<IReadOnlyList<HintGridCell>>(),
            It.IsAny<List<ItemName>>(),
            It.IsAny<string>(),
            It.IsAny<double>(),
            It.IsAny<double>()
        )).ReturnsAsync(selectedCells);

        // Item already has a region (not UNKNOWN), so it shouldn't be updated
        _mockItemTrackingService.Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(new SavedItem
            {
                ItemName = ItemName.DONKEY,
                Region = RegionName.ANGRY_AZTEC // Already has a region
            });

        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        await vm.SelectFoundItemsCommand.ExecuteAsync((100.0, 200.0));

        // Should not update because region is already set
        _mockItemTrackingService.Verify(s => s.SetItemState(
            ItemName.DONKEY,
            It.IsAny<SavedItem>()
        ), Times.Never);
    }

    #endregion

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );

        vm.Dispose();
        vm.Dispose(); // Should not throw

        Assert.True(true);
    }

    [Fact]
    public void Dispose_CanBeCalledViaUsingStatement()
    {
        using var vm = new FoundHintItemListViewModel(
            HintPanelType.Isles,
            _testHintId,
            TestDialogTitle,
            _mockItemTrackingService.Object,
            _mockHintItemSelectionService.Object,
            _mockHintService.Object,
            _mockSavedProgressProvider.Object
        );
        Assert.NotNull(vm);
    }

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
                _mockItemTrackingService.Reset();
                _mockHintItemSelectionService.Reset();
                _mockHintService.Reset();
                _mockSavedProgressProvider.Reset();
            }
            _disposed = true;
        }
    }

    ~FoundHintItemListViewModelTests()
    {
        Dispose(false);
    }

    #endregion
}
