namespace TrackOMatic.ViewModels.Test.Hints;

public class BLockerPanelViewModelTests
{
    private readonly Mock<IBarrierService> _barrierServiceMock = new();
    private readonly Mock<IParsedSpoilerDataService> _parsedSpoilerDataServiceMock = new();
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock = new();
    private readonly Mock<IItemSelectorService> _itemSelectorServiceMock = new();
    private readonly Mock<IDataSaverService> _dataSaverServiceMock = new();

    private BLockerPanelViewModel CreateViewModel(string lineColor = "#FF0000")
    {
        return new BLockerPanelViewModel(
            lineColor,
            _barrierServiceMock.Object,
            _parsedSpoilerDataServiceMock.Object,
            _savedProgressProviderMock.Object,
            _itemSelectorServiceMock.Object,
            _dataSaverServiceMock.Object
        );
    }

    #region Constructor Validation Tests

    [Fact]
    public void Constructor_WithValidServices_Succeeds()
    {
        var vm = CreateViewModel();
        Assert.NotNull(vm);
    }

    [Fact]
    public void Constructor_WithNullBarrierService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BLockerPanelViewModel(
                "#FF0000",
                null!,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("barrierService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullParsedSpoilerDataService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BLockerPanelViewModel(
                "#FF0000",
                _barrierServiceMock.Object,
                null!,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("parsedSpoilerDataService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BLockerPanelViewModel(
                "#FF0000",
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                null!,
                _itemSelectorServiceMock.Object,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("savedProgressProvider", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullItemSelectorService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BLockerPanelViewModel(
                "#FF0000",
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                null!,
                _dataSaverServiceMock.Object
            )
        );
        Assert.Equal("itemSelectorService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullDataSaverService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new BLockerPanelViewModel(
                "#FF0000",
                _barrierServiceMock.Object,
                _parsedSpoilerDataServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object,
                null!
            )
        );
        Assert.Equal("dataSaverService", ex.ParamName);
    }

    #endregion

    #region LineColor Tests

    [Fact]
    public void Constructor_InitializesLineColor()
    {
        var vm = CreateViewModel("#FF0000");
        Assert.Equal("#FF0000", vm.LineColor);
    }

    [Fact]
    public void Constructor_InitializesLineColor_WithDifferentValue()
    {
        var vm = CreateViewModel("#00FF00");
        Assert.Equal("#00FF00", vm.LineColor);
    }

    #endregion

    #region Hints Collection Tests

    [Fact]
    public void Hints_ContainsExactlyEightItems()
    {
        var vm = CreateViewModel();
        Assert.Equal(8, vm.Hints.Count);
    }

    [Fact]
    public void Hints_AllItemsAreNonNull()
    {
        var vm = CreateViewModel();
        foreach (var hint in vm.Hints)
        {
            Assert.NotNull(hint);
        }
    }

    [Fact]
    public void Hints_AreAllBLockerHintViewModels()
    {
        var vm = CreateViewModel();
        foreach (var hint in vm.Hints)
        {
            Assert.IsType<BLockerHintViewModel>(hint);
        }
    }

    #endregion
}
