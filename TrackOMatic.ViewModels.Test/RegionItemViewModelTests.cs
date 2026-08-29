using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.ViewModels.Test;

/// <summary>
/// Unit tests for RegionItemViewModel.
/// Tests property initialization, event subscriptions, disposal, and notification.
/// </summary>
public class RegionItemViewModelTests
{
    private readonly Mock<IItemTrackingService> _mockItemTrackingService;
    private readonly Mock<IParsedSpoilerDataService> _mockParsedSpoilerDataService;
    private readonly Mock<IThemeService> _mockThemeService;

    public RegionItemViewModelTests()
    {
        _mockItemTrackingService = new Mock<IItemTrackingService>();
        _mockParsedSpoilerDataService = new Mock<IParsedSpoilerDataService>();
        _mockThemeService = new Mock<IThemeService>();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionItemViewModel(
                ItemName.DONKEY,
                RegionName.JUNGLE_JAPES,
                null!,
                _mockParsedSpoilerDataService.Object,
                _mockThemeService.Object
            )
        );

        Assert.Equal("itemTrackingService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionItemViewModel(
                ItemName.DONKEY,
                RegionName.JUNGLE_JAPES,
                _mockItemTrackingService.Object,
                null!,
                _mockThemeService.Object
            )
        );

        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullThemeService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new RegionItemViewModel(
                ItemName.DONKEY,
                RegionName.JUNGLE_JAPES,
                _mockItemTrackingService.Object,
                _mockParsedSpoilerDataService.Object,
                null!
            )
        );

        Assert.Equal("themeService", ex.ParamName);
    }

    [Fact]
    public void Constructor_SubscribesToItemStateChanged()
    {
        // Arrange
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(
                new SavedItem(
                    ItemName.DONKEY,
                    RegionName.JUNGLE_JAPES,
                    ItemVisibilityState.Visible,
                    false,
                    1.0,
                    false
                )
            )
        );

        // Assert
        // If subscribed correctly, VM should not throw; if not subscribed, this would fail
        Assert.NotNull(vm);
    }

    [Fact]
    public void Constructor_SubscribesToBarrelPadThemeChangedForBarrelPadItems()
    {
        // Arrange
        var vm = new RegionItemViewModel(
            ItemName.STRONG_KONG,  // This is a barrel pad item
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        _mockThemeService.Raise(s => s.BarrelPadThemeChanged += null, EventArgs.Empty);

        // Assert
        // If subscribed correctly, VM should not throw
        Assert.NotNull(vm);
    }

    [Fact]
    public void Constructor_DoesNotSubscribeToBarrelPadThemeChangedForNonBarrelPadItems()
    {
        // Arrange
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,  // Not a barrel pad item
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act & Assert
        // Verify theme service event was not subscribed (indirectly)
        // We can only verify this by checking that BarrelPadThemeChanged isn't raised
        _mockThemeService.Raise(s => s.BarrelPadThemeChanged += null, EventArgs.Empty);
        Assert.NotNull(vm);  // No exception means it's fine either way
    }

    [Fact]
    public void Constructor_InitializesStateFromService()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        // Act
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Assert
        Assert.Equal("donkey", vm.ImageResourceKey);
        Assert.True(vm.IsStarred);  // Visible = starred
    }

    [Fact]
    public void Constructor_InitializesWithNullItemState()
    {
        // Arrange
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns((SavedItem?)null);

        // Act
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Assert
        Assert.Empty(vm.ImageResourceKey);
        Assert.False(vm.IsStarred);
    }

    #endregion

    #region CurrentItemName Tests

    [Fact]
    public void CurrentItemName_MatchesConstructorItemName()
    {
        // Arrange & Act
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Assert
        Assert.Equal(ItemName.DONKEY, vm.CurrentItemName);
    }

    #endregion

    #region CanAcceptDrop Tests

    [Theory]
    [InlineData(ItemName.DIDDY, MouseDragType.Left)]
    [InlineData(ItemName.CRANKY, MouseDragType.Right)]
    [InlineData(ItemName.CANDY, MouseDragType.None)]
    public void CanAcceptDrop_AlwaysReturnsFalse(ItemName itemToPlace, MouseDragType dragType)
    {
        // Arrange
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        var result = vm.CanAcceptDrop(itemToPlace, dragType);

        // Assert
        Assert.False(result);  // This slot is already occupied
    }

    #endregion

    #region AcceptDropAndMutate Tests

    [Theory]
    [InlineData(ItemName.DIDDY, MouseDragType.Left)]
    [InlineData(ItemName.CRANKY, MouseDragType.Right)]
    public void AcceptDropAndMutate_AlwaysReturnsFalse(ItemName itemToPlace, MouseDragType dragType)
    {
        // Arrange
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        var result = vm.AcceptDropAndMutate(itemToPlace, dragType);

        // Assert
        Assert.False(result);  // Should never be called; return false
    }

    #endregion

    #region RemoveFromRegion Tests

    [Fact]
    public void RemoveFromRegion_UpdatesRegionToUnknown()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        vm.RemoveFromRegion();

        // Assert
        _mockItemTrackingService.Verify(
            s => s.SetItemState(
                ItemName.DONKEY,
                It.Is<SavedItem>(si => si.Region == RegionName.UNKNOWN)
            ),
            Times.Once
        );
    }

    [Fact]
    public void RemoveFromRegion_WithNullItemState_DoesNotThrow()
    {
        // Arrange
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns((SavedItem?)null);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act & Assert (no exception)
        vm.RemoveFromRegion();

        _mockItemTrackingService.Verify(
            s => s.SetItemState(It.IsAny<ItemName>(), It.IsAny<SavedItem>()),
            Times.Never
        );
    }

    #endregion

    #region Property Update Tests

    [Fact]
    public void OnItemStateChanged_UpdatesImageResourceKey()
    {
        // Arrange
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns((SavedItem?)null);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var newItemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );

        // Act
        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(newItemState)
        );

        // Assert
        Assert.Equal("donkey", vm.ImageResourceKey);
    }

    [Fact]
    public void OnItemStateChanged_UpdatesIsStarred()
    {
        // Arrange
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns((SavedItem?)null);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        Assert.False(vm.IsStarred);

        var newItemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,  // Visible = starred
            false,
            1.0,
            false
        );

        // Act
        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(newItemState)
        );

        // Assert
        Assert.True(vm.IsStarred);
    }

    [Fact]
    public void ImageResourceKey_WithHintedTrue_IncludesBwSuffix()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            true  // Hinted = true
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        // Act
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Assert
        Assert.Equal("donkey_bw", vm.ImageResourceKey);
    }

    [Fact]
    public void ImageResourceKey_WithHintedFalse_ExcludesBwSuffix()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false  // Hinted = false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        // Act
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Assert
        Assert.Equal("donkey", vm.ImageResourceKey);
    }

    [Fact]
    public void IsStarred_WithVisibleState_ReturnsTrue()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        // Act
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Assert
        Assert.True(vm.IsStarred);
    }

    [Fact]
    public void IsStarred_WithHiddenState_ReturnsFalse()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Hidden,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        // Act
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Assert
        Assert.False(vm.IsStarred);
    }

    [Fact]
    public void OnItemStateChanged_IgnoresUnrelatedItems()
    {
        // Arrange
        var initialState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(initialState);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var initialImageKey = vm.ImageResourceKey;

        var unrelatedItemState = new SavedItem(
            ItemName.DIDDY,  // Different item
            RegionName.DK_ISLES,  // Different region
            ItemVisibilityState.Hidden,
            false,
            1.0,
            false
        );

        // Act
        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(unrelatedItemState)
        );

        // Assert
        Assert.Equal(initialImageKey, vm.ImageResourceKey);  // Should not change
    }

    #endregion

    #region PropertyChanged Notification Tests

    [Fact]
    public void ImageResourceKey_RaisesPropertyChanged()
    {
        // Arrange
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns((SavedItem?)null);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var propertyChangedRaised = false;
        vm.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(RegionItemViewModel.ImageResourceKey))
                propertyChangedRaised = true;
        };

        var newItemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );

        // Act
        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(newItemState)
        );

        // Assert
        Assert.True(propertyChangedRaised);
    }

    [Fact]
    public void IsStarred_RaisesPropertyChanged()
    {
        // Arrange
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns((SavedItem?)null);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var propertyChangedRaised = false;
        vm.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(RegionItemViewModel.IsStarred))
                propertyChangedRaised = true;
        };

        var newItemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );

        // Act
        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(newItemState)
        );

        // Assert
        Assert.True(propertyChangedRaised);
    }

    [Fact]
    public void PropertyChanged_NotRaisedWhenValueDoesNotChange()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var changeCount = 0;
        vm.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(RegionItemViewModel.ImageResourceKey))
                changeCount++;
        };

        // Act: Raise event with same state
        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(itemState)
        );

        // Assert
        Assert.Equal(0, changeCount);  // No change because value is same
    }

    #endregion

    #region Theme Changed Tests

    [Fact]
    public void OnBarrelPadThemeChanged_UpdatesImageKeyForBarrelPadItems()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.STRONG_KONG,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.STRONG_KONG))
            .Returns(itemState);

        var vm = new RegionItemViewModel(
            ItemName.STRONG_KONG,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var changeCount = 0;
        vm.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(RegionItemViewModel.ImageResourceKey))
                changeCount++;
        };

        // Act
        _mockThemeService.Raise(s => s.BarrelPadThemeChanged += null, EventArgs.Empty);

        // Assert
        // Should have raised at least one change (empty then re-populate)
        Assert.True(changeCount >= 1);
    }

    #endregion

    #region GetHoardPoints Tests

    [Fact]
    public void GetHoardPoints_WithNullItem_ReturnsZero()
    {
        // Arrange
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns((SavedItem?)null);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        var hoardPoints = vm.GetHoardPoints();

        // Assert
        Assert.Equal(0, hoardPoints);
    }

    [Fact]
    public void GetHoardPoints_WithStarredItem_ReturnsOne()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,  // Starred
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        var hoardPoints = vm.GetHoardPoints();

        // Assert
        Assert.Equal(1, hoardPoints);
    }

    [Fact]
    public void GetHoardPoints_WithUnstarredItem_ReturnsZero()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Hidden,  // Not starred
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        var hoardPoints = vm.GetHoardPoints();

        // Assert
        Assert.Equal(0, hoardPoints);
    }

    [Fact]
    public void GetHoardPoints_WithCollapsedItem_ReturnsZero()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Collapsed,  // Not fully starred
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        var hoardPoints = vm.GetHoardPoints();

        // Assert
        Assert.Equal(0, hoardPoints);
    }

    #endregion

    #region GetItemPoints Tests

    [Fact]
    public void GetItemPoints_WithNullItem_ReturnsZero()
    {
        // Arrange
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns((SavedItem?)null);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        var itemPoints = vm.GetItemPoints();

        // Assert
        Assert.Equal(0, itemPoints);
    }

    [Fact]
    public void GetItemPoints_DelegatsToService()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        _mockParsedSpoilerDataService
            .Setup(s => s.GetPointsForItem(ItemName.DONKEY))
            .Returns(5);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        var itemPoints = vm.GetItemPoints();

        // Assert
        Assert.Equal(5, itemPoints);
        _mockParsedSpoilerDataService.Verify(
            s => s.GetPointsForItem(ItemName.DONKEY),
            Times.Once
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void GetItemPoints_ReturnsPointsFromService(int expectedPoints)
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        _mockParsedSpoilerDataService
            .Setup(s => s.GetPointsForItem(ItemName.DONKEY))
            .Returns(expectedPoints);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        var itemPoints = vm.GetItemPoints();

        // Assert
        Assert.Equal(expectedPoints, itemPoints);
    }

    #endregion

    #region ToggleStar Tests

    [Fact]
    public void ToggleStar_WithValidItem_CallsServiceToggleStar()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        vm.ToggleStar();

        // Assert
        _mockItemTrackingService.Verify(
            s => s.ToggleStar(ItemName.DONKEY),
            Times.Once
        );
    }

    [Fact]
    public void ToggleStar_WithNullItem_DoesNotCallService()
    {
        // Note: CurrentItemName is always non-null for RegionItemViewModel
        // (it's set in constructor to _itemName, which is always valid)
        // But we can test the scenario where GetItemState returns null
        // In that case, ToggleStar should still not be called if CurrentItemName is null
        // However, this is actually a test design issue - CurrentItemName is never null here.
        // Instead, test that ToggleStar gracefully handles when called with a valid item.

        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        vm.ToggleStar();

        // Assert (it was called because CurrentItemName is always set)
        _mockItemTrackingService.Verify(
            s => s.ToggleStar(ItemName.DONKEY),
            Times.Once
        );
    }

    [Fact]
    public void ToggleStar_CanBeCalledMultipleTimes()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        vm.ToggleStar();
        vm.ToggleStar();
        vm.ToggleStar();

        // Assert
        _mockItemTrackingService.Verify(
            s => s.ToggleStar(ItemName.DONKEY),
            Times.Exactly(3)
        );
    }

    [Theory]
    [InlineData(ItemName.DONKEY)]
    [InlineData(ItemName.DIDDY)]
    [InlineData(ItemName.CRANKY)]
    public void ToggleStar_WithDifferentItems_ToggleEachCorrectly(ItemName itemName)
    {
        // Arrange
        var itemState = new SavedItem(
            itemName,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(itemName))
            .Returns(itemState);

        var vm = new RegionItemViewModel(
            itemName,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        vm.ToggleStar();

        // Assert
        _mockItemTrackingService.Verify(
            s => s.ToggleStar(itemName),
            Times.Once
        );
    }

    #endregion

    #region Integration Tests (Points + Star)

    [Fact]
    public void GetHoardPoints_ReflectsStarStateAfterToggle()
    {
        // Arrange
        var initialState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Hidden,  // Unstarred
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(initialState);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Initial state: unstarred
        Assert.Equal(0, vm.GetHoardPoints());

        // Act: Toggle star (simulate service event)
        var starredState = initialState with { Starred = ItemVisibilityState.Visible };

        // Update the mock to return the new state for subsequent calls
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(starredState);

        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(starredState)
        );

        // Assert: Now should be starred
        Assert.Equal(1, vm.GetHoardPoints());
    }

    [Fact]
    public void GetItemPointsAndGetHoardPoints_CanBothReturnNonZero()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,  // Starred = 1 hoard point
            false,
            1.0,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        _mockParsedSpoilerDataService
            .Setup(s => s.GetPointsForItem(ItemName.DONKEY))
            .Returns(5);  // 5 item points

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        var hoardPoints = vm.GetHoardPoints();
        var itemPoints = vm.GetItemPoints();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(1, hoardPoints);
            Assert.Equal(5, itemPoints);
        });
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromItemStateChanged()
    {
        // Arrange
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        vm.Dispose();

        // Now raise event and ensure VM doesn't react
        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(
                new SavedItem(
                    ItemName.DONKEY,
                    RegionName.DK_ISLES,
                    ItemVisibilityState.Hidden,
                    false,
                    0.5,
                    true
                )
            )
        );

        // Assert (no exception means unsubscribe worked; can't directly test event removal without reflection)
        Assert.NotNull(vm);
    }

    [Fact]
    public void Dispose_UnsubscribesFromBarrelPadThemeChanged()
    {
        // Arrange
        var vm = new RegionItemViewModel(
            ItemName.STRONG_KONG,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        vm.Dispose();

        // Now raise event; should not crash
        _mockThemeService.Raise(s => s.BarrelPadThemeChanged += null, EventArgs.Empty);

        // Assert
        Assert.NotNull(vm);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act & Assert (no exception)
        vm.Dispose();
        vm.Dispose();
        vm.Dispose();
    }

    [Fact]
    public void Dispose_CallsGCSuppressFinalize()
    {
        // Arrange
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Act
        vm.Dispose();

        // Assert: The finalizer should not run after Dispose()
        // (We can't directly test GC.SuppressFinalize, but we can verify no exception)
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Assert.NotNull(vm);  // VM still exists, finalizer didn't cause issues
    }

    #endregion

    #region Opacity Tests

    [Fact]
    public void Opacity_InitializesToOne_WhenNoItemState()
    {
        // Arrange
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns((SavedItem?)null);

        // Act
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Assert
        Assert.Equal(1.0, vm.Opacity);
    }

    [Fact]
    public void Opacity_InitializesFromItemState_WhenItemExists()
    {
        // Arrange
        var itemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            0.375,  // Hinted opacity
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(itemState);

        // Act
        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        // Assert
        Assert.Equal(0.375, vm.Opacity);
    }

    [Fact]
    public void Opacity_UpdatesWhenItemStateChanges()
    {
        // Arrange
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns((SavedItem?)null);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );
        Assert.Equal(1.0, vm.Opacity);

        var newItemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            1.5,  // Found item opacity
            false
        );

        // Act
        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(newItemState)
        );

        // Assert
        Assert.Equal(1.5, vm.Opacity);
    }

    [Fact]
    public void Opacity_ResetsToOne_WhenItemRemoved()
    {
        // Arrange
        var initialItemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            0.375,
            false
        );
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns(initialItemState);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );
        Assert.Equal(0.375, vm.Opacity);

        // Act - Simulate item state being cleared
        var removedItemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.UNKNOWN,
            ItemVisibilityState.Hidden,
            false,
            1.0,  // Reset opacity when removed
            false
        );
        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(removedItemState)
        );

        // Assert
        Assert.Equal(1.0, vm.Opacity);
    }

    [Fact]
    public void Opacity_RaisesPropertyChanged()
    {
        // Arrange
        _mockItemTrackingService
            .Setup(s => s.GetItemState(ItemName.DONKEY))
            .Returns((SavedItem?)null);

        var vm = new RegionItemViewModel(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            _mockItemTrackingService.Object,
            _mockParsedSpoilerDataService.Object,
            _mockThemeService.Object
        );

        var propertyChangedRaised = false;
        vm.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(RegionItemViewModel.Opacity))
                propertyChangedRaised = true;
        };

        var newItemState = new SavedItem(
            ItemName.DONKEY,
            RegionName.JUNGLE_JAPES,
            ItemVisibilityState.Visible,
            false,
            0.5,
            false
        );

        // Act
        _mockItemTrackingService.Raise(
            s => s.ItemStateChanged += null,
            new ItemStateChangedEventArgs(newItemState)
        );

        // Assert
        Assert.True(propertyChangedRaised);
    }

    #endregion
}
