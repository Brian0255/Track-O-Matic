namespace TrackOMatic.ViewModels.Test.Broadcast;

/// <summary>
/// Test coverage for the <see cref="KeyViewModel"/> class.
/// </summary>
public class KeyViewModelTests
{
    private readonly Mock<IItemTrackingService> _itemTrackingServiceMock;
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;
    private readonly Mock<IDataSaverService> _dataSaverServiceMock;

    public KeyViewModelTests()
    {
        _itemTrackingServiceMock = new Mock<IItemTrackingService>();
        _parsedSpoilerDataServiceMock = new Mock<IParsedSpoilerDataService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
        _dataSaverServiceMock = new Mock<IDataSaverService>();
    }

    private KeyViewModel CreateViewModel(int selectedKey = 1)
    {
        return new KeyViewModel(
            selectedKey,
            _itemTrackingServiceMock.Object,
            _parsedSpoilerDataServiceMock.Object,
            _savedProgressProviderMock.Object,
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
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullItemTrackingService_ThrowsArgumentNullException(IItemTrackingService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new KeyViewModel(1, service!, _parsedSpoilerDataServiceMock.Object, _savedProgressProviderMock.Object, _dataSaverServiceMock.Object)
        );
        Assert.Equal("itemTrackingService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException(IParsedSpoilerDataService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new KeyViewModel(1, _itemTrackingServiceMock.Object, service!, _savedProgressProviderMock.Object, _dataSaverServiceMock.Object)
        );
        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException(ISavedProgressProvider? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new KeyViewModel(1, _itemTrackingServiceMock.Object, _parsedSpoilerDataServiceMock.Object, service!, _dataSaverServiceMock.Object)
        );
        Assert.Equal("savedProgressProvider", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullDataSaverService_ThrowsArgumentNullException(IDataSaverService? service)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new KeyViewModel(1, _itemTrackingServiceMock.Object, _parsedSpoilerDataServiceMock.Object, _savedProgressProviderMock.Object, service!)
        );
        Assert.Equal("dataSaverService", ex.ParamName);
    }

    #endregion

    #region Event Subscription Tests

    [Fact]
    public void Constructor_SubscribesToItemTrackingService()
    {
        // Act
        CreateViewModel();
        // Assert
        _itemTrackingServiceMock.VerifyAdd(x => x.ItemStateChanged += It.IsAny<EventHandler<ItemStateChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToParsedSpoilerDataServiceParsedSpoilerDataChanged()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _parsedSpoilerDataServiceMock.VerifyAdd(x => x.ParsedSpoilerDataChanged += It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToSavedProgressProviderProgressChanged()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _savedProgressProviderMock.VerifyAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Constructor_SubscribesToDataSaverServiceDataLoadingCompleted()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        _dataSaverServiceMock.VerifyAdd(x => x.DataLoadingCompleted += It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Constructor_InitializesImageResourceKey()
    {
        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.NotNull(viewModel.ImageResourceKey);
    }

    #endregion

    #region Event Handler Tests - State Reiniitalization

    [Fact]
    public void ParsedSpoilerDataChanged_ReinitializeState()
    {
        // Arrange
        var viewModel = CreateViewModel();
        Assert.False(viewModel.IsStarred);

        // Act 1: Toggle the star.
        var starredKey = new SavedItem(ItemName.KEY_1, starred: true);
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(starredKey);
        viewModel.OnItemStateChanged(this, new ItemStateChangedEventArgs(starredKey));
        Assert.True(viewModel.IsStarred);

        // Act 2: Prepare the unstarred key to simulate data reload.
        var unstarredKey = new SavedItem(ItemName.KEY_1);
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(unstarredKey);

        // Act 3: Raise the event.
        _parsedSpoilerDataServiceMock.Raise(x => x.ParsedSpoilerDataChanged += null, new ParsedSpoilerDataChangedEventArgs(null, null));

        // Assert
        Assert.False(viewModel.IsStarred);
    }

    [Fact]
    public void SavedProgressProviderChanged_ReinitializeState()
    {
        // Arrange
        var viewModel = CreateViewModel();
        Assert.False(viewModel.IsStarred);

        // Act 1: Toggle the star.
        var starredKey = new SavedItem(ItemName.KEY_1, starred: true);
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(starredKey);
        viewModel.OnItemStateChanged(this, new ItemStateChangedEventArgs(starredKey));
        Assert.True(viewModel.IsStarred);

        // Act 2: Prepare the unstarred key to simulate data reload.
        var unstarredKey = new SavedItem(ItemName.KEY_1);
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(unstarredKey);

        // Act 3: Raise the event.
        _savedProgressProviderMock.Raise(x => x.ProgressChanged += null, new ProgressReplacedEventArgs { NewProgress = null! });

        // Assert
        Assert.False(viewModel.IsStarred);
    }

    [Fact]
    public void DataLoadingCompleted_ReinitializeState()
    {
        // Arrange
        var viewModel = CreateViewModel();
        Assert.False(viewModel.IsStarred);

        // Act 1: Toggle the star.
        var starredKey = new SavedItem(ItemName.KEY_1, starred: true);
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(starredKey);
        viewModel.OnItemStateChanged(this, new ItemStateChangedEventArgs(starredKey));
        Assert.True(viewModel.IsStarred);

        // Act 2: Prepare the unstarred key to simulate data reload.
        var unstarredKey = new SavedItem(ItemName.KEY_1);
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(unstarredKey);

        // Act 3: Raise the event.
        var mockProgress = new SavedProgress();
        _dataSaverServiceMock.Raise(x => x.DataLoadingCompleted += null, new DataLoadingCompletedEventArgs(mockProgress));

        // Assert
        Assert.False(viewModel.IsStarred);
    }

    #endregion

    #region UpdateImageResourceKey Tests

    [Fact]
    public void UpdateImageResourceKey_WithUnknownRegion_SetsBWImage()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal("basic_key_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_FromSpoilerData_SetsColorImage()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData { LevelOrder = [] };
        spoilerData.StartingItems[ItemName.KEY_1] = RegionName.UNHINTABLE_MOVES;
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal("basic_key", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithHintedRegion_SetsBWImage()
    {
        // Arrange
        var key1 = new SavedItem(ItemName.KEY_1, RegionName.GLOOMY_GALLEON, opacity: 0.375 );
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(key1);

        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal("basic_key_bw", viewModel.ImageResourceKey);
    }

    [Fact]
    public void UpdateImageResourceKey_WithKnownRegion_SetsColorImage()
    {
        // Arrange
        var key1 = new SavedItem(ItemName.KEY_1, RegionName.GLOOMY_GALLEON);
        _itemTrackingServiceMock.Setup(x => x.GetItemState(ItemName.KEY_1)).Returns(key1);

        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal("basic_key", viewModel.ImageResourceKey);
    }

    #endregion

    #region UpdateHoverText Tests

    [Fact]
    public void UpdateHoverText_WithNoSpoilerData_ClearsHoverText()
    {
        // Arrange
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns((ParsedSpoilerData?)null);

        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal("", viewModel.HoverText);
        Assert.False(viewModel.HasHoverText);
    }

    [Fact]
    public void UpdateHoverText_WithPointsDisabled_ClearsHoverText()
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(pointsEnabled: false)
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal("", viewModel.HoverText);
        Assert.False(viewModel.HasHoverText);
    }

    [Theory]
    [InlineData(1, "Point")]
    [InlineData(11, "Points")]
    public void UpdateHoverText_WithPointsEnabled_SetsSingularOrPluralText(int pointValue, string expectedSuffix)
    {
        // Arrange
        var spoilerData = new ParsedSpoilerData
        {
            LevelOrder = new Dictionary<RegionName, int>(),
            SpoilerSettings = new SpoilerSettings(pointsEnabled: true),
            PointSpread = new Dictionary<PointCategory, int>
            {
                { PointCategory.Key, pointValue }
            }
        };
        _parsedSpoilerDataServiceMock.Setup(x => x.CurrentData).Returns(spoilerData);

        // Act
        var viewModel = CreateViewModel();

        // Assert
        Assert.Equal($"{pointValue} {expectedSuffix}", viewModel.HoverText);
        Assert.True(viewModel.HasHoverText);
    }

    #endregion

    #region Object Override Tests

    [Fact]
    public void Equals_WithSameSelectedKey_ReturnsTrue()
    {
        // Arrange
        var viewModel1 = CreateViewModel(selectedKey: 1);
        var viewModel2 = CreateViewModel(selectedKey: 1);
        // Act
        bool areEqual = viewModel1.Equals((object)viewModel2);
        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Equals_WithDifferentSelectedKey_ReturnsFalse()
    {
        // Arrange
        var viewModel1 = CreateViewModel(selectedKey: 1);
        var viewModel2 = CreateViewModel(selectedKey: 2);
        // Act
        bool areEqual = viewModel1.Equals(viewModel2);
        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void GetHashCode_WithSameSelectedKey_ReturnsSameHashCode()
    {
        // Arrange
        var viewModel1 = CreateViewModel(selectedKey: 1);
        var viewModel2 = CreateViewModel(selectedKey: 1);
        // Act
        int hash1 = viewModel1.GetHashCode();
        int hash2 = viewModel2.GetHashCode();
        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_WithDifferentSelectedKey_ReturnsDifferentHashCode()
    {
        // Arrange
        var viewModel1 = CreateViewModel(selectedKey: 1);
        var viewModel2 = CreateViewModel(selectedKey: 2);
        // Act
        int hash1 = viewModel1.GetHashCode();
        int hash2 = viewModel2.GetHashCode();
        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        // Arrange
        var viewModel = CreateViewModel(selectedKey: 1);
        // Act
        string result = viewModel.ToString();
        // Assert
        Assert.Equal("Key 1", result);
    }

    [Fact]
    public void ToString_Starred_ReturnsExpectedFormat()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.IsStarred = true;

        // Assert
        Assert.Equal("Key 1*", viewModel.ToString());
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromAllEvents()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.Dispose();

        // Assert
        Assert.Multiple(
            () => _itemTrackingServiceMock.VerifyRemove(x => x.ItemStateChanged -= It.IsAny<EventHandler<ItemStateChangedEventArgs>>(), Times.Once),
            () => _parsedSpoilerDataServiceMock.VerifyRemove(x => x.ParsedSpoilerDataChanged -= It.IsAny<EventHandler<ParsedSpoilerDataChangedEventArgs>>(), Times.Once),
            () => _savedProgressProviderMock.VerifyRemove(x => x.ProgressChanged -= It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once),
            () => _dataSaverServiceMock.VerifyRemove(x => x.DataLoadingCompleted -= It.IsAny<EventHandler<DataLoadingCompletedEventArgs>>(), Times.Once)
        );
    }

    #endregion
}
