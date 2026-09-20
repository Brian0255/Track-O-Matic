namespace TrackOMatic.ViewModels.Test.Hints;

public class HelmDoorPanelViewModelTests
{
    private readonly Mock<IUserSettingsService> _userSettingsServiceMock = new();
    private readonly Mock<IBarrierService> _barrierServiceMock = new();
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock = new();
    private readonly Mock<IItemSelectorService> _itemSelectorServiceMock = new();

    private HelmDoorPanelViewModel CreateViewModel(string lineColor = "#FF0000", bool helmDoorsEnabled = true)
    {
        _userSettingsServiceMock.Setup(x => x.HelmDoors).Returns(helmDoorsEnabled);

        return new HelmDoorPanelViewModel(
            lineColor,
            _userSettingsServiceMock.Object,
            _barrierServiceMock.Object,
            _savedProgressProviderMock.Object,
            _itemSelectorServiceMock.Object
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
    public void Constructor_WithNullUserSettingsService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HelmDoorPanelViewModel(
                "#FF0000",
                null!,
                _barrierServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object
            )
        );
        Assert.Equal("userSettingsService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullBarrierService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HelmDoorPanelViewModel(
                "#FF0000",
                _userSettingsServiceMock.Object,
                null!,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object
            )
        );
        Assert.Equal("barrierService", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HelmDoorPanelViewModel(
                "#FF0000",
                _userSettingsServiceMock.Object,
                _barrierServiceMock.Object,
                null!,
                _itemSelectorServiceMock.Object
            )
        );
        Assert.Equal("savedProgressProvider", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullItemSelectorService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new HelmDoorPanelViewModel(
                "#FF0000",
                _userSettingsServiceMock.Object,
                _barrierServiceMock.Object,
                _savedProgressProviderMock.Object,
                null!
            )
        );
        Assert.Equal("itemSelectorService", ex.ParamName);
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

    #region Door Creation Tests

    [Fact]
    public void Constructor_CreatesFirstDoor()
    {
        var vm = CreateViewModel();
        Assert.NotNull(vm.FirstDoor);
    }

    [Fact]
    public void Constructor_CreatesSecondDoor()
    {
        var vm = CreateViewModel();
        Assert.NotNull(vm.SecondDoor);
    }

    [Fact]
    public void FirstDoorAndSecondDoor_AreDistinctInstances()
    {
        var vm = CreateViewModel();
        Assert.NotEqual(vm.FirstDoor, vm.SecondDoor);
    }

    [Fact]
    public void FirstDoor_IsHelmDoorHintViewModel()
    {
        var vm = CreateViewModel();
        Assert.IsType<HelmDoorHintViewModel>(vm.FirstDoor);
    }

    [Fact]
    public void SecondDoor_IsHelmDoorHintViewModel()
    {
        var vm = CreateViewModel();
        Assert.IsType<HelmDoorHintViewModel>(vm.SecondDoor);
    }

    #endregion

    #region IsEnabled Tests

    [Fact]
    public void IsEnabled_WhenHelmDoorsEnabled_ReturnsTrue()
    {
        var vm = CreateViewModel(helmDoorsEnabled: true);
        Assert.True(vm.IsEnabled);
    }

    [Fact]
    public void IsEnabled_WhenHelmDoorsDisabled_ReturnsFalse()
    {
        var vm = CreateViewModel(helmDoorsEnabled: false);
        Assert.False(vm.IsEnabled);
    }

    #endregion

    #region Settings Changed Event Tests

    [Fact]
    public void OnSettingChanged_WithHelmDoorsPropertyChange_RaisesIsEnabledNotification()
    {
        var vm = CreateViewModel(helmDoorsEnabled: true);
        var propertyChangedRaised = false;
        var changedPropertyName = string.Empty;

        vm.PropertyChanged += (sender, e) =>
        {
            propertyChangedRaised = true;
            changedPropertyName = e.PropertyName;
        };

        _userSettingsServiceMock.Setup(x => x.HelmDoors).Returns(false);
        _userSettingsServiceMock.Raise(x => x.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IUserSettingsService.HelmDoors)));

        Assert.True(propertyChangedRaised);
        Assert.Equal(nameof(vm.IsEnabled), changedPropertyName);
    }

    [Fact]
    public void OnSettingChanged_WithOtherPropertyChange_DoesNotRaiseIsEnabledNotification()
    {
        var vm = CreateViewModel();
        var propertyChangedRaised = false;

        vm.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(vm.IsEnabled))
            {
                propertyChangedRaised = true;
            }
        };

        _userSettingsServiceMock.Raise(x => x.PropertyChanged += null,
            new PropertyChangedEventArgs("SomeOtherProperty"));

        Assert.False(propertyChangedRaised);
    }

    [Fact]
    public void IsEnabled_ReflectsLatestSettingValue_AfterPropertyChange()
    {
        var vm = CreateViewModel(helmDoorsEnabled: true);
        Assert.True(vm.IsEnabled);

        _userSettingsServiceMock.Setup(x => x.HelmDoors).Returns(false);
        _userSettingsServiceMock.Raise(x => x.PropertyChanged += null,
            new PropertyChangedEventArgs(nameof(IUserSettingsService.HelmDoors)));

        Assert.False(vm.IsEnabled);
    }

    #endregion
}
