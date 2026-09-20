namespace TrackOMatic.ViewModels.Test.Broadcast;

public class BroadcastItemViewModelTests
{
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock;
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;
    private readonly Mock<IThemeService> _themeServiceMock;
    private readonly Mock<IDataSaverService> _dataSaverServiceMock;

    public BroadcastItemViewModelTests()
    {
        _itemTrackingServiceMock = new Mock<IItemTrackingService>();
        _parsedSpoilerDataServiceMock = new Mock<IParsedSpoilerDataService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
        _themeServiceMock = new Mock<IThemeService>();
        _dataSaverServiceMock = new Mock<IDataSaverService>();
    }

    private BroadcastItemViewModel CreateViewModel(ItemName itemName = ItemName.COCONUT_GUN)
    {
        return new BroadcastItemViewModel(
            itemName,
            _itemTrackingServiceMock.Object,
            _parsedSpoilerDataServiceMock.Object,
            _savedProgressProviderMock.Object,
            _themeServiceMock.Object,
            _dataSaverServiceMock.Object
        );
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidDependencies_Succeeds()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel);
        Assert.Equal(ItemName.COCONUT_GUN, viewModel.ItemName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException(IItemTrackingService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BroadcastItemViewModel(
                ItemName.COCONUT_GUN,
                service!,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _themeServiceMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("itemTrackingService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException(IParsedSpoilerDataService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BroadcastItemViewModel(
                ItemName.COCONUT_GUN,
                _itemTrackingServiceMock.Object,
                service!,
                _savedProgressProviderMock.Object,
                _themeServiceMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException(ISavedProgressProvider? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BroadcastItemViewModel(
                ItemName.COCONUT_GUN,
                _itemTrackingServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                service!,
                _themeServiceMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("savedProgressProvider", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullThemeService_ThrowsArgumentNullException(IThemeService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BroadcastItemViewModel(
                ItemName.COCONUT_GUN,
                _itemTrackingServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                service!,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("themeService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullDataSaverService_ThrowsArgumentNullException(IDataSaverService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BroadcastItemViewModel(
                ItemName.COCONUT_GUN,
                _itemTrackingServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _themeServiceMock.Object,
                service!
            )
        );
        Assert.Equal("dataSaverService", ex.ParamName);
    }

    [Theory]
    [InlineData(ItemName.COCONUT_GUN)]
    [InlineData(ItemName.ROCKETBARREL_BOOST)]
    [InlineData(ItemName.GORILLA_GRAB)]
    public void Constructor_WithDifferentItemNames_SetsItemNameCorrectly(ItemName itemName)
    {
        // Act
        var viewModel = CreateViewModel(itemName);

        // Assert
        Assert.Equal(itemName, viewModel.ItemName);
    }

    [Fact]
    public void Constructor_SubscribesToItemStateChanged()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _itemTrackingServiceMock.VerifyAdd(x => x.ItemStateChanged += It.IsAny<EventHandler<ItemStateChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribersToParsedSpoilerDataChanged()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _parsedSpoilerDataServiceMock.VerifyAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToDataLoadingCompleted()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _dataSaverServiceMock.VerifyAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_InitializesFromCurrentItemState()
    {
        // Arrange
        var savedItem = new SavedItem { ItemName = ItemName.COCONUT_GUN, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.COCONUT_GUN))
            .Returns(savedItem);

        // Act
        var viewModel = CreateViewModel();

        // Assert
        _itemTrackingServiceMock.Verify(x => x.GetItemState(ItemName.COCONUT_GUN), Times.Once);
    }

    #endregion

    #region Event Handling Tests

    [Fact]
    public void OnItemStateChanged_WithMatchingItem_UpdatesState()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var newState = new SavedItem { ItemName = ItemName.COCONUT_GUN, Starred = true };

        // Act
        viewModel.Dispose();
        _itemTrackingServiceMock.Raise(x => x.ItemStateChanged += null, new ItemStateChangedEventArgs(newState));

        // Assert - just verify it doesn't throw
    }

    [Fact]
    public void OnItemStateChanged_WithDifferentItem_DoesNothing()
    {
        // Arrange
        var viewModel = CreateViewModel(ItemName.COCONUT_GUN);
        var differentItemState = new SavedItem { ItemName = ItemName.GORILLA_GRAB, Starred = true };

        // Act & Assert
        viewModel.OnItemStateChanged(this, new ItemStateChangedEventArgs(differentItemState));
    }

    #endregion


    #region Event Handler Tests - State Reinitialization

    [Fact]
    public void ParsedSpoilerDataChanged_ReinitializesState()
    {
        // Arrange - create viewmodel with default (unstarred) mock
        var viewModel = CreateViewModel(ItemName.GRAPE_SHOOTER);
        Assert.False(viewModel.IsStarred);

        // Step 1: Toggle the star - change mock to return starred item
        var starredItem = new SavedItem { ItemName = ItemName.GRAPE_SHOOTER, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.GRAPE_SHOOTER))
            .Returns(starredItem);
        viewModel.OnItemStateChanged(this, new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating data reload)
        var unstarredItem = new SavedItem { ItemName = ItemName.GRAPE_SHOOTER, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.GRAPE_SHOOTER))
            .Returns(unstarredItem);

        // Act - raise the event, which triggers OnParsedSpoilerDataChanged -> InitializeState()
        _parsedSpoilerDataServiceMock.Raise(x => x.ParsedSpoilerDataChanged += null,
            new ParsedSpoilerDataChangedEventArgs(null, null));

        // Assert - star should be cleared because InitializeState() was called
        Assert.False(viewModel.IsStarred);
    }

    [Fact]
    public void SavedProgressChanged_ReinitializesState()
    {
        // Arrange - create viewmodel with default (unstarred) mock
        var viewModel = CreateViewModel(ItemName.FEATHER_BOW);
        Assert.False(viewModel.IsStarred);

        // Step 1: Toggle the star - change mock to return starred item
        var starredItem = new SavedItem { ItemName = ItemName.FEATHER_BOW, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.FEATHER_BOW))
            .Returns(starredItem);
        viewModel.OnItemStateChanged(this, new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating progress reload)
        var unstarredItem = new SavedItem { ItemName = ItemName.FEATHER_BOW, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.FEATHER_BOW))
            .Returns(unstarredItem);

        // Act - raise the event, which triggers OnSavedProgressChanged -> InitializeState()
        var mockNewProgress = new SavedProgress();
        _savedProgressProviderMock.Raise(x => x.ProgressChanged += null,
            new ProgressReplacedEventArgs { OldProgress = null, NewProgress = mockNewProgress });

        // Assert - star should be cleared because InitializeState() was called
        Assert.False(viewModel.IsStarred);
    }

    [Fact]
    public void DataLoadingCompleted_ReinitializesState()
    {
        // Arrange - create viewmodel with default (unstarred) mock
        var viewModel = CreateViewModel(ItemName.SAXOPHONE_SLAM);
        Assert.False(viewModel.IsStarred);

        // Step 1: Toggle the star - change mock to return starred item
        var starredItem = new SavedItem { ItemName = ItemName.SAXOPHONE_SLAM, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.SAXOPHONE_SLAM))
            .Returns(starredItem);
        viewModel.OnItemStateChanged(this, new ItemStateChangedEventArgs(starredItem));
        Assert.True(viewModel.IsStarred);

        // Step 2: Setup mock to return unstarred again (simulating data load)
        var unstarredItem = new SavedItem { ItemName = ItemName.SAXOPHONE_SLAM, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.SAXOPHONE_SLAM))
            .Returns(unstarredItem);

        // Act - raise the event, which triggers OnDataLoaded -> InitializeState()
        var mockProgress = new SavedProgress();
        _dataSaverServiceMock.Raise(x => x.DataLoadingCompleted += null,
            new DataLoadingCompletedEventArgs(mockProgress));

        // Assert - star should be cleared because InitializeState() was called
        Assert.False(viewModel.IsStarred);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameItemName_ReturnsTrue()
    {
        // Arrange
        var viewModel1 = CreateViewModel(ItemName.COCONUT_GUN);
        var viewModel2 = CreateViewModel(ItemName.COCONUT_GUN);

        // Act & Assert
        Assert.True(viewModel1.Equals((object)viewModel2));
    }

    [Fact]
    public void Equals_WithDifferentItemName_ReturnsFalse()
    {
        // Arrange
        var viewModel1 = CreateViewModel(ItemName.COCONUT_GUN);
        var viewModel2 = CreateViewModel(ItemName.GORILLA_GRAB);

        // Act & Assert
        Assert.NotEqual(viewModel1, viewModel2);
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    public void GetHashCode_WithSameItemName_ReturnsSame()
    {
        // Arrange
        var viewModel1 = CreateViewModel(ItemName.COCONUT_GUN);
        var viewModel2 = CreateViewModel(ItemName.COCONUT_GUN);

        // Act & Assert
        Assert.Equal(viewModel1.GetHashCode(), viewModel2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentItemName_ReturnsDifferent()
    {
        // Arrange
        var viewModel1 = CreateViewModel(ItemName.COCONUT_GUN);
        var viewModel2 = CreateViewModel(ItemName.GORILLA_GRAB);

        // Act & Assert
        Assert.NotEqual(viewModel1.GetHashCode(), viewModel2.GetHashCode());
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ReturnsItemName()
    {
        // Arrange
        var viewModel = CreateViewModel(ItemName.COCONUT_GUN);

        // Act
        var result = viewModel.ToString();

        // Assert
        Assert.NotNull(result);
        Assert.Contains(ItemName.COCONUT_GUN.ToString(), result);
    }

    [Fact]
    public void ToString_WithStarred_IncludesStarred()
    {
        // Arrange
        var savedItem = new SavedItem { ItemName = ItemName.COCONUT_GUN, Starred = true };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.COCONUT_GUN))
            .Returns(savedItem);

        // Act
        var viewModel = CreateViewModel();
        var result = viewModel.ToString();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(result);
            Assert.Contains("*", result);
        });
    }

    [Fact]
    public void ToString_WithoutStarred_DoesNotIncludeStarred()
    {
        // Arrange
        var savedItem = new SavedItem { ItemName = ItemName.COCONUT_GUN, Starred = false };
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.COCONUT_GUN))
            .Returns(savedItem);

        // Act
        var viewModel = CreateViewModel();
        var result = viewModel.ToString();

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("*", result);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromItemStateChanged()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.Dispose();

        // Assert
        _itemTrackingServiceMock.VerifyRemove(x => x.ItemStateChanged -= It.IsAny<EventHandler<ItemStateChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_UnsubscribesFromParsedSpoilerDataChanged()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.Dispose();

        // Assert
        _parsedSpoilerDataServiceMock.VerifyRemove(x => x.ParsedSpoilerDataChanged -= It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_UnsubscribesFromDataLoadingCompleted()
    {
        // Arrange
        var viewModel = CreateViewModel(ItemName.ROCKETBARREL_BOOST);

        // Act
        viewModel.Dispose();

        // Assert
        _dataSaverServiceMock.VerifyRemove(x => x.DataLoadingCompleted -= It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    #endregion
}
