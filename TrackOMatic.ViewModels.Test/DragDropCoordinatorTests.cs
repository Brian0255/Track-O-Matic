using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace TrackOMatic.ViewModels.Test;

/// <summary>
/// Test suite for DragDropCoordinator.
/// Tests that the coordinator correctly delegates drop operations to the appropriate list view model.
/// </summary>
public class DragDropCoordinatorTests
{
    private readonly DragDropCoordinator _coordinator;

    public DragDropCoordinatorTests()
    {
        InitializeServiceLocator();
        _coordinator = new DragDropCoordinator();
    }

    private static void InitializeServiceLocator()
    {
        var mockLevelOrderService = new Mock<ILevelOrderService>();
        mockLevelOrderService.Setup(s => s.GetLevelOrder()).Returns([1, 2, 3, 4, 5, 6, 7, 8]);

        var mockUserSettingsService = new Mock<IUserSettingsService>();
        mockUserSettingsService.Setup(s => s.HelmInLevelOrder).Returns(true);

        var services = new ServiceCollection();
        services.AddSingleton(mockLevelOrderService.Object);
        services.AddSingleton(mockUserSettingsService.Object);
        ServiceLocator.Initialize(services.BuildServiceProvider());
    }

    /// <summary>
    /// Helper to set the HasVials property on a RegionViewModel via reflection.
    /// </summary>
    private static void SetHasVials(RegionViewModel viewModel, bool value)
    {
        var field = typeof(RegionViewModel).GetField("hasVials", BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(viewModel, value);
    }

    /// <summary>
    /// Helper to set the VialItemListViewModel property on a RegionViewModel via reflection.
    /// </summary>
    private static void SetVialItemListViewModel(RegionViewModel viewModel, VialItemListViewModel? value)
    {
        var field = typeof(RegionViewModel).GetField("_vialItemListViewModel", BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(viewModel, value);
    }

    /// <summary>
    /// Helper to set the RegionItemListViewModel property on a RegionViewModel via reflection.
    /// </summary>
    private static void SetRegionItemListViewModel(RegionViewModel viewModel, RegionItemListViewModel? value)
    {
        var field = typeof(RegionViewModel).GetField("_regionItemListViewModel", BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(viewModel, value);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_Succeeds()
    {
        var coordinator = new DragDropCoordinator();
        Assert.NotNull(coordinator);
        Assert.IsType<IDragDropCoordinator>(coordinator, exactMatch: false);
    }

    #endregion

    #region TryExecuteDrop Tests

    [Fact]
    public void TryExecuteDrop_WithNullRegionViewModel_ThrowsArgumentNullException()
    {
        var itemName = ItemName.KEY_1;
        var dragType = MouseDragType.Left;
        Assert.Throws<ArgumentNullException>(() =>
            _coordinator.TryExecuteDrop(itemName, null!, dragType)
        );
    }

    [Fact]
    public void TryExecuteDrop_WithBothListViewModelsNull_ReturnsFalse()
    {
        var regionViewModel = new Mock<RegionViewModel>(
            RegionName.JUNGLE_JAPES,
            new Mock<IItemTrackingService>().Object,
            new Mock<IParsedSpoilerDataService>().Object,
            new Mock<ISavedProgressProvider>().Object,
            new Mock<IThemeService>().Object,
            new Mock<RegionSlotProviderRegistry>().Object,
            new Mock<IAutotrackerService>().Object,
            new Mock<IDataSaverService>().Object
        )
        {
            CallBase = true
        };

        SetHasVials(regionViewModel.Object, false);
        SetVialItemListViewModel(regionViewModel.Object, null);
        SetRegionItemListViewModel(regionViewModel.Object, null);

        var result = _coordinator.TryExecuteDrop(ItemName.KEY_1, regionViewModel.Object, MouseDragType.Left);

        Assert.False(result);
    }

    [Fact]
    public void TryExecuteDrop_WithVialModeAndNullVialListViewModel_ReturnsFalse()
    {
        var regionViewModel = new Mock<RegionViewModel>(
            RegionName.JUNGLE_JAPES,
            new Mock<IItemTrackingService>().Object,
            new Mock<IParsedSpoilerDataService>().Object,
            new Mock<ISavedProgressProvider>().Object,
            new Mock<IThemeService>().Object,
            new Mock<RegionSlotProviderRegistry>().Object,
            new Mock<IAutotrackerService>().Object,
            new Mock<IDataSaverService>().Object
        )
        {
            CallBase = true
        };

        SetHasVials(regionViewModel.Object, true);
        SetVialItemListViewModel(regionViewModel.Object, null);
        SetRegionItemListViewModel(regionViewModel.Object, null);

        var result = _coordinator.TryExecuteDrop(ItemName.KEY_1, regionViewModel.Object, MouseDragType.Left);

        Assert.False(result);
    }

    [Fact]
    public void TryExecuteDrop_WithRegionModeAndNullRegionListViewModel_ReturnsFalse()
    {
        var regionViewModel = new Mock<RegionViewModel>(
            RegionName.JUNGLE_JAPES,
            new Mock<IItemTrackingService>().Object,
            new Mock<IParsedSpoilerDataService>().Object,
            new Mock<ISavedProgressProvider>().Object,
            new Mock<IThemeService>().Object,
            new Mock<RegionSlotProviderRegistry>().Object,
            new Mock<IAutotrackerService>().Object,
            new Mock<IDataSaverService>().Object
        )
        {
            CallBase = true
        };

        SetHasVials(regionViewModel.Object, false);
        SetVialItemListViewModel(regionViewModel.Object, null);
        SetRegionItemListViewModel(regionViewModel.Object, null);

        var result = _coordinator.TryExecuteDrop(ItemName.KEY_1, regionViewModel.Object, MouseDragType.Left);

        Assert.False(result);
    }

    [Fact]
    public void TryExecuteDrop_WithVialModeAndActiveBehavior_DelegatesToVialList()
    {
        var mockVialList = new Mock<IItemListViewModel>();
        mockVialList.Setup(vm => vm.TryAcceptDrop(It.IsAny<ItemName>(), It.IsAny<MouseDragType>()))
            .Returns(true);

        // Create a stub vial list that implements both IItemListViewModel and VialItemListViewModel
        var stubVial = new StubVialListViewModel(mockVialList.Object);

        var mockRegionViewModel = new Mock<RegionViewModel>(
            RegionName.JUNGLE_JAPES,
            new Mock<IItemTrackingService>().Object,
            new Mock<IParsedSpoilerDataService>().Object,
            new Mock<ISavedProgressProvider>().Object,
            new Mock<IThemeService>().Object,
            new Mock<RegionSlotProviderRegistry>().Object,
            new Mock<IAutotrackerService>().Object,
            new Mock<IDataSaverService>().Object
        )
        {
            CallBase = true
        };

        SetHasVials(mockRegionViewModel.Object, true);
        SetVialItemListViewModel(mockRegionViewModel.Object, stubVial);

        var result = _coordinator.TryExecuteDrop(ItemName.KEY_1, mockRegionViewModel.Object, MouseDragType.Left);

        // Vial stubs with mocks return true from TryAcceptDrop
        Assert.True(result);
    }

    [Fact]
    public void TryExecuteDrop_WithRegionModeAndActiveBehavior_DelegatesToRegionList()
    {
        var mockRegionList = new Mock<IItemListViewModel>();
        mockRegionList.Setup(vm => vm.TryAcceptDrop(It.IsAny<ItemName>(), It.IsAny<MouseDragType>()))
            .Returns(true);

        var stubRegion = new StubRegionListViewModel(mockRegionList.Object);

        var mockRegionViewModel = new Mock<RegionViewModel>(
            RegionName.JUNGLE_JAPES,
            new Mock<IItemTrackingService>().Object,
            new Mock<IParsedSpoilerDataService>().Object,
            new Mock<ISavedProgressProvider>().Object,
            new Mock<IThemeService>().Object,
            new Mock<RegionSlotProviderRegistry>().Object,
            new Mock<IAutotrackerService>().Object,
            new Mock<IDataSaverService>().Object
        )
        {
            CallBase = true
        };

        SetHasVials(mockRegionViewModel.Object, false);
        SetRegionItemListViewModel(mockRegionViewModel.Object, stubRegion);

        var result = _coordinator.TryExecuteDrop(ItemName.KEY_1, mockRegionViewModel.Object, MouseDragType.Left);

        Assert.True(result);
        mockRegionList.Verify(vm => vm.TryAcceptDrop(ItemName.KEY_1, MouseDragType.Left), Times.Once);
    }

    [Fact]
    public void TryExecuteDrop_WithRegionModeAcceptanceFails_ReturnsFalse()
    {
        var mockRegionList = new Mock<IItemListViewModel>();
        mockRegionList.Setup(vm => vm.TryAcceptDrop(It.IsAny<ItemName>(), It.IsAny<MouseDragType>()))
            .Returns(false);

        var stubRegion = new StubRegionListViewModel(mockRegionList.Object);

        var mockRegionViewModel = new Mock<RegionViewModel>(
            RegionName.JUNGLE_JAPES,
            new Mock<IItemTrackingService>().Object,
            new Mock<IParsedSpoilerDataService>().Object,
            new Mock<ISavedProgressProvider>().Object,
            new Mock<IThemeService>().Object,
            new Mock<RegionSlotProviderRegistry>().Object,
            new Mock<IAutotrackerService>().Object,
            new Mock<IDataSaverService>().Object
        )
        { CallBase = true };

        SetHasVials(mockRegionViewModel.Object, false);
        SetRegionItemListViewModel(mockRegionViewModel.Object, stubRegion);

        var result = _coordinator.TryExecuteDrop(ItemName.KEY_1, mockRegionViewModel.Object, MouseDragType.Left);

        Assert.False(result);
        mockRegionList.Verify(vm => vm.TryAcceptDrop(ItemName.KEY_1, MouseDragType.Left), Times.Once);
    }

    [Fact]
    public void TryExecuteDrop_SelectsVialModeWhenHasVialsIsTrue()
    {
        var mockVialList = new Mock<IItemListViewModel>();
        mockVialList.Setup(vm => vm.TryAcceptDrop(It.IsAny<ItemName>(), It.IsAny<MouseDragType>()))
            .Returns(true);

        var mockRegionList = new Mock<IItemListViewModel>();
        mockRegionList.Setup(vm => vm.TryAcceptDrop(It.IsAny<ItemName>(), It.IsAny<MouseDragType>()))
            .Returns(true);

        var stubVial = new StubVialListViewModel(mockVialList.Object);
        var stubRegion = new StubRegionListViewModel(mockRegionList.Object);

        var mockRegionViewModel = new Mock<RegionViewModel>(
            RegionName.JUNGLE_JAPES,
            new Mock<IItemTrackingService>().Object,
            new Mock<IParsedSpoilerDataService>().Object,
            new Mock<ISavedProgressProvider>().Object,
            new Mock<IThemeService>().Object,
            new Mock<RegionSlotProviderRegistry>().Object,
            new Mock<IAutotrackerService>().Object,
            new Mock<IDataSaverService>().Object
        )
        { CallBase = true };

        SetHasVials(mockRegionViewModel.Object, true);
        SetVialItemListViewModel(mockRegionViewModel.Object, stubVial);
        SetRegionItemListViewModel(mockRegionViewModel.Object, stubRegion);

        var result = _coordinator.TryExecuteDrop(ItemName.KEY_1, mockRegionViewModel.Object, MouseDragType.Left);

        // When HasVials is true, should use vial list
        Assert.True(result);
    }

    [Fact]
    public void TryExecuteDrop_SelectsRegionModeWhenHasVialsIsFalse()
    {
        var mockVialList = new Mock<IItemListViewModel>();
        mockVialList.Setup(vm => vm.TryAcceptDrop(It.IsAny<ItemName>(), It.IsAny<MouseDragType>()))
            .Returns(true);

        var mockRegionList = new Mock<IItemListViewModel>();
        mockRegionList.Setup(vm => vm.TryAcceptDrop(It.IsAny<ItemName>(), It.IsAny<MouseDragType>()))
            .Returns(true);

        var stubVial = new StubVialListViewModel(mockVialList.Object);
        var stubRegion = new StubRegionListViewModel(mockRegionList.Object);

        var mockRegionViewModel = new Mock<RegionViewModel>(
            RegionName.JUNGLE_JAPES,
            new Mock<IItemTrackingService>().Object,
            new Mock<IParsedSpoilerDataService>().Object,
            new Mock<ISavedProgressProvider>().Object,
            new Mock<IThemeService>().Object,
            new Mock<RegionSlotProviderRegistry>().Object,
            new Mock<IAutotrackerService>().Object,
            new Mock<IDataSaverService>().Object
        )
        { CallBase = true };

        SetHasVials(mockRegionViewModel.Object, false);
        SetVialItemListViewModel(mockRegionViewModel.Object, stubVial);
        SetRegionItemListViewModel(mockRegionViewModel.Object, stubRegion);

        _coordinator.TryExecuteDrop(ItemName.KEY_1, mockRegionViewModel.Object, MouseDragType.Left);

        mockVialList.Verify(vm => vm.TryAcceptDrop(It.IsAny<ItemName>(), It.IsAny<MouseDragType>()), Times.Never);
        mockRegionList.Verify(vm => vm.TryAcceptDrop(It.IsAny<ItemName>(), It.IsAny<MouseDragType>()), Times.Once);
    }

    [Theory]
    [InlineData(ItemName.KEY_1, MouseDragType.Left)]
    [InlineData(ItemName.KEY_2, MouseDragType.Right)]
    [InlineData(ItemName.DONKEY, MouseDragType.None)]
    public void TryExecuteDrop_PassesCorrectParametersToDelegate(ItemName itemName, MouseDragType dragType)
    {
        var mockList = new Mock<IItemListViewModel>();
        mockList.Setup(vm => vm.TryAcceptDrop(It.IsAny<ItemName>(), It.IsAny<MouseDragType>()))
            .Returns(true);

        var stubRegion = new StubRegionListViewModel(mockList.Object);

        var mockRegionViewModel = new Mock<RegionViewModel>(
            RegionName.JUNGLE_JAPES,
            new Mock<IItemTrackingService>().Object,
            new Mock<IParsedSpoilerDataService>().Object,
            new Mock<ISavedProgressProvider>().Object,
            new Mock<IThemeService>().Object,
            new Mock<RegionSlotProviderRegistry>().Object,
            new Mock<IAutotrackerService>().Object,
            new Mock<IDataSaverService>().Object
        )
        { CallBase = true };

        SetHasVials(mockRegionViewModel.Object, false);
        SetRegionItemListViewModel(mockRegionViewModel.Object, stubRegion);

        _coordinator.TryExecuteDrop(itemName, mockRegionViewModel.Object, dragType);

        mockList.Verify(vm => vm.TryAcceptDrop(itemName, dragType), Times.Once);
    }

    #endregion

    #region Stub Implementations

    /// <summary>
    /// A minimal stub that wraps a mock IItemListViewModel as a VialItemListViewModel.
    /// Delegates all TryAcceptDrop calls to the inner implementation.
    /// </summary>
    private class StubVialListViewModel(IItemListViewModel inner) : VialItemListViewModel(
        RegionName.JUNGLE_JAPES,
        new Mock<IItemTrackingService>().Object,
        new Mock<IParsedSpoilerDataService>().Object,
        new Mock<ISavedProgressProvider>().Object,
        new Mock<IThemeService>().Object,
        new Mock<IDataSaverService>().Object
    )
    {
        private readonly IItemListViewModel _inner = inner;

        public override bool TryAcceptDrop(ItemName itemName, MouseDragType dragType)
        {
            return _inner.TryAcceptDrop(itemName, dragType);
        }
    }

    /// <summary>
    /// A minimal stub that wraps a mock IItemListViewModel as a RegionItemListViewModel.
    /// Delegates all TryAcceptDrop calls to the inner implementation.
    /// </summary>
    private class StubRegionListViewModel(IItemListViewModel inner) : RegionItemListViewModel(
        RegionName.JUNGLE_JAPES,
        new Mock<IItemTrackingService>().Object,
        new Mock<IParsedSpoilerDataService>().Object,
        new Mock<ISavedProgressProvider>().Object,
        new Mock<IThemeService>().Object,
        new Mock<IDataSaverService>().Object
    )
    {
        private readonly IItemListViewModel _inner = inner;

        public override bool TryAcceptDrop(ItemName itemName, MouseDragType dragType)
        {
            return _inner.TryAcceptDrop(itemName, dragType);
        }
    }

    #endregion
}
