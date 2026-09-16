namespace TrackOMatic.ViewModels.Test.Hints;

public class PathHintItemListViewModelTests : IDisposable
{
    private readonly Mock<IHintItemSelectionService> _mockHintItemSelectionService;
    private readonly Mock<IHintService> _mockHintService;
    private readonly Mock<ISavedProgressProvider> _mockSavedProgressProvider;
    private readonly HintId _testHintId = new("testhint");
    private const string TestDialogTitle = "Test Dialog";

    public PathHintItemListViewModelTests()
    {
        _mockHintItemSelectionService = new Mock<IHintItemSelectionService>();
        _mockHintService = new Mock<IHintService>();
        _mockSavedProgressProvider = new Mock<ISavedProgressProvider>();

        // Default GetHint to return a hint with some path items
        _mockHintService.Setup(s => s.GetHint(_testHintId)).Returns(new SavedHint
        {
            PathItems = new Dictionary<ItemName, bool>
            {
                { ItemName.DONKEY, true },
                { ItemName.DIDDY, false }
            }
        });
    }

    [Fact]
    public void Constructor_WithValidParameters_Succeeds()
    {
        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );
        Assert.NotNull(vm);
    }

    [Fact]
    public void Constructor_WithNullDialogTitle_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new PathHintItemListViewModel(
                HintPanelType.Paths,
                _testHintId,
                null!,
                _mockHintService.Object,
                _mockHintItemSelectionService.Object,
                _mockSavedProgressProvider.Object
            )
        );
        Assert.Equal("dialogTitle", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullHintService_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new PathHintItemListViewModel(
                HintPanelType.Paths,
                _testHintId,
                TestDialogTitle,
                null!,
                _mockHintItemSelectionService.Object,
                _mockSavedProgressProvider.Object
            )
        );
        Assert.Equal("hintService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullHintItemSelectionService_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new PathHintItemListViewModel(
                HintPanelType.Paths,
                _testHintId,
                TestDialogTitle,
                _mockHintService.Object,
                null!,
                _mockSavedProgressProvider.Object
            )
        );
        Assert.Equal("hintItemSelectionService", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new PathHintItemListViewModel(
                HintPanelType.Paths,
                _testHintId,
                TestDialogTitle,
                _mockHintService.Object,
                _mockHintItemSelectionService.Object,
                null!
            )
        );
        Assert.Equal("savedProgressProvider", exception.ParamName);
    }

    [Fact]
    public void Constructor_InitializesPathItems()
    {
        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.NotEmpty(vm.PathItems);
        Assert.Equal(2, vm.PathItems.Count);
    }

    [Theory]
    [InlineData(HintPanelType.Paths, true)]
    [InlineData(HintPanelType.Isles, false)]
    [InlineData(HintPanelType.Japes, false)]
    public void IsHintInfoPath_ReturnsBasedOnPanelType(HintPanelType hintPanelType, bool expected)
    {
        var vm = new PathHintItemListViewModel(
            hintPanelType,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.Equal(expected, vm.IsHintInfoPath);
    }

    [Fact]
    public void RefreshRow_WithZeroItems_AddsNonePlaceholder()
    {
        _mockHintService.Setup(s => s.GetHint(_testHintId))
            .Returns(new SavedHint { PathItems = [] });

        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.Single(vm.PathItems);
        Assert.Equal(ItemName.NONE, vm.PathItems[0].ItemName);
    }

    [Fact]
    public void RefreshRow_WithOneItem_PlacesInCollection()
    {
        _mockHintService.Setup(s => s.GetHint(_testHintId))
            .Returns(new SavedHint
            {
                PathItems = new Dictionary<ItemName, bool>
                {
                    { ItemName.DONKEY, true }
                }
            });

        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.Single(vm.PathItems);
        Assert.Equal(ItemName.DONKEY, vm.PathItems[0].ItemName);
    }

    [Fact]
    public void RefreshRow_WithMultipleItems_PlacesAllInCollection()
    {
        _mockHintService.Setup(s => s.GetHint(_testHintId))
            .Returns(new SavedHint
            {
                PathItems = new Dictionary<ItemName, bool>
                {
                    { ItemName.DONKEY, true },
                    { ItemName.DIDDY, false },
                    { ItemName.LANKY, true }
                }
            });

        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        Assert.Multiple(() =>
        {
            Assert.Equal(3, vm.PathItems.Count);
            Assert.Contains(vm.PathItems, item => item.ItemName == ItemName.DONKEY);
            Assert.Contains(vm.PathItems, item => item.ItemName == ItemName.DIDDY);
            Assert.Contains(vm.PathItems, item => item.ItemName == ItemName.LANKY);
        });
    }

    [Fact]
    public void PathItemsChanged_IsFiredWhenPathItemsPropertyChanges()
    {
        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        var eventFired = false;
        vm.PathItemsChanged += (sender, e) => eventFired = true;

        // Update PathItems to trigger the event
        vm.PathItems = [];

        Assert.True(eventFired);
    }

    [Fact]
    public void ToString_WithEmptyItems_IncludesNonePlaceholder()
    {
        _mockHintService.Setup(s => s.GetHint(_testHintId))
            .Returns(new SavedHint { PathItems = [] });

        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        var result = vm.ToString();
        // ToString joins actual PathItems. When empty, RefreshRow adds NONE placeholder
        Assert.Equal("Items on path: NONE", result);
    }

    [Fact]
    public void ToString_IncludesAllPathItems()
    {
        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        var result = vm.ToString();
        Assert.Contains("Items on path:", result);
        Assert.Contains("DONKEY", result);
        Assert.Contains("DIDDY", result);
    }

    #region SelectPathItems Command

    [Fact]
    public async Task SelectPathItems_WithValidSelection_CallsHintItemSelectionService()
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

        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        await vm.SelectPathItemsCommand.ExecuteAsync((100.0, 200.0));

        _mockHintItemSelectionService.Verify(s => s.SelectItemsAsync(
            It.IsAny<IReadOnlyList<HintGridCell>>(),
            It.Is<List<ItemName>>(list => list.Contains(ItemName.DONKEY) && list.Contains(ItemName.DIDDY)),
            TestDialogTitle,
            100.0,
            200.0
        ), Times.Once);
    }

    [Fact]
    public async Task SelectPathItems_WithValidSelection_CallsSetPathItems()
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

        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        await vm.SelectPathItemsCommand.ExecuteAsync((100.0, 200.0));

        _mockHintService.Verify(s => s.SetPathItems(
            _testHintId,
            It.Is<Dictionary<ItemName, bool>>(dict =>
                dict.ContainsKey(ItemName.DONKEY) &&
                dict.ContainsKey(ItemName.DIDDY) &&
                dict[ItemName.DONKEY] == true && // Was found in original
                dict[ItemName.DIDDY] == false    // Was not found in original
            )
        ), Times.Once);
    }

    [Fact]
    public async Task SelectPathItems_PreservesExistingFoundState()
    {
        // Setup with existing items where DONKEY is found and DIDDY is not
        _mockHintService.Setup(s => s.GetHint(_testHintId)).Returns(new SavedHint
        {
            PathItems = new Dictionary<ItemName, bool>
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

        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        await vm.SelectPathItemsCommand.ExecuteAsync((100.0, 200.0));

        _mockHintService.Verify(s => s.SetPathItems(
            _testHintId,
            It.Is<Dictionary<ItemName, bool>>(dict =>
                dict[ItemName.DONKEY] == true &&  // Preserve found state
                dict[ItemName.DIDDY] == false     // Preserve not-found state
            )
        ), Times.Once);
    }

    [Fact]
    public async Task SelectPathItems_WithEmptySelection_CallsSetPathItems()
    {
        _mockHintItemSelectionService.Setup(s => s.SelectItemsAsync(
            It.IsAny<IReadOnlyList<HintGridCell>>(),
            It.IsAny<List<ItemName>>(),
            It.IsAny<string>(),
            It.IsAny<double>(),
            It.IsAny<double>()
        )).ReturnsAsync([]);

        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        await vm.SelectPathItemsCommand.ExecuteAsync((100.0, 200.0));

        _mockHintService.Verify(s => s.SetPathItems(
            _testHintId,
            It.Is<Dictionary<ItemName, bool>>(dict => dict.Count == 0)
        ), Times.Once);
    }

    #endregion

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
            _mockSavedProgressProvider.Object
        );

        vm.Dispose();
        vm.Dispose(); // Should not throw

        Assert.True(true);
    }

    [Fact]
    public void Dispose_CanBeCalledViaUsingStatement()
    {
        using var vm = new PathHintItemListViewModel(
            HintPanelType.Paths,
            _testHintId,
            TestDialogTitle,
            _mockHintService.Object,
            _mockHintItemSelectionService.Object,
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
                _mockHintItemSelectionService.Reset();
                _mockHintService.Reset();
                _mockSavedProgressProvider.Reset();
            }
            _disposed = true;
        }
    }

    ~PathHintItemListViewModelTests()
    {
        Dispose(false);
    }

    #endregion
}
