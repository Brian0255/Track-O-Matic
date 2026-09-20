using Xunit;

using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Hints;
using TrackOMatic.ViewModels.Test.Helpers;

namespace TrackOMatic.ViewModels.Test.Hints;

public class HelmDoorHintViewModelTests
{
    private readonly Mock<IBarrierService> _barrierServiceMock;
    private readonly Mock<ISavedProgressProvider> _savedProgressProviderMock;
    private readonly Mock<IItemSelectorService> _itemSelectorServiceMock;

    public HelmDoorHintViewModelTests()
    {
        _barrierServiceMock = new Mock<IBarrierService>();
        _savedProgressProviderMock = new Mock<ISavedProgressProvider>();
        _itemSelectorServiceMock = new Mock<IItemSelectorService>();

        // Default mock setup
        _barrierServiceMock.Setup(x => x.GetHelmDoorCount(It.IsAny<HelmDoor>())).Returns("0");
    }

    private HelmDoorHintViewModel CreateViewModel(HelmDoor helmDoor)
    {
        return SynchronizationContextTestHelper.RunWithUIContext(() =>
            new HelmDoorHintViewModel(
                helmDoor,
                _barrierServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object
            )
        );
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullBarrierService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new HelmDoorHintViewModel(
                    HelmDoor.First,
                    null!,
                    _savedProgressProviderMock.Object,
                    _itemSelectorServiceMock.Object
                )
            )
        );
    }

    [Fact]
    public void Constructor_WithNullSavedProgressProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new HelmDoorHintViewModel(
                    HelmDoor.First,
                    _barrierServiceMock.Object,
                    null!,
                    _itemSelectorServiceMock.Object
                )
            )
        );
    }

    [Fact]
    public void Constructor_WithNullItemSelectorService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SynchronizationContextTestHelper.RunWithUIContext(() =>
                new HelmDoorHintViewModel(
                    HelmDoor.First,
                    _barrierServiceMock.Object,
                    _savedProgressProviderMock.Object,
                    null!
                )
            )
        );
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void Constructor_InitializesBarrierCount_FromService()
    {
        _barrierServiceMock.Setup(x => x.GetHelmDoorCount(HelmDoor.First)).Returns("5");

        var vm = CreateViewModel(HelmDoor.First);

        Assert.Equal("5", vm.BarrierCount);
    }

    [Fact]
    public void Constructor_InitializesLabelText_BasedOnDoor()
    {
        var vmFirst = CreateViewModel(HelmDoor.First);
        var vmSecond = CreateViewModel(HelmDoor.Second);

        Assert.Equal("Door 1:", vmFirst.LabelText);
        Assert.Equal("Door 2:", vmSecond.LabelText);
    }

    #endregion

    #region Command Execution Tests

    [Fact]
    public void ClearPlaceholderCommand_ClearsBarrierCount()
    {
        _barrierServiceMock.Setup(x => x.GetHelmDoorCount(HelmDoor.First)).Returns("?");

        var vm = CreateViewModel(HelmDoor.First);
        vm.ClearPlaceholderCommand.Execute(null);

        Assert.Equal("", vm.BarrierCount);
    }

    [Fact]
    public void SetBarrierCountCommand_CallsService_WithCurrentBarrierCount()
    {
        var vm = CreateViewModel(HelmDoor.First);
        vm.BarrierCount = "5";

        vm.SetBarrierCountCommand.Execute(null);

        _barrierServiceMock.Verify(x => x.SetHelmDoorCount(HelmDoor.First, "5"), Times.Once);
    }

    #endregion

    #region Slot View Model Lazy Loading Tests

    [Fact]
    public void SlotViewModel_CreatesInstance_WhenAccessed()
    {
        _barrierServiceMock.Setup(x => x.GetHelmDoorCount(HelmDoor.First)).Returns("0");

        var vm = CreateViewModel(HelmDoor.First);
        var slot = vm.SlotViewModel;

        Assert.NotNull(slot);
    }

    [Fact]
    public void SlotViewModel_ReusesInstance_WhenAccessedMultipleTimes()
    {
        _barrierServiceMock.Setup(x => x.GetHelmDoorCount(HelmDoor.First)).Returns("0");

        var vm = CreateViewModel(HelmDoor.First);
        var slot1 = vm.SlotViewModel;
        var slot2 = vm.SlotViewModel;

        Assert.Same(slot1, slot2);
    }

    #endregion

    #region Event Subscription Tests

    [Fact]
    public void Constructor_SubscribesToHelmBarrierAmountChangedEvent()
    {
        _barrierServiceMock.SetupAdd(x => x.HelmBarrierAmountChanged += It.IsAny<EventHandler<HelmBarrierAmountChangedEventArgs>>())
            .Verifiable();

        var vm = CreateViewModel(HelmDoor.First);

        _barrierServiceMock.Verify();
    }

    [Fact]
    public void Constructor_SubscribesToProgressChangedEvent()
    {
        _savedProgressProviderMock.SetupAdd(x => x.ProgressChanged += It.IsAny<EventHandler<ProgressReplacedEventArgs>>())
            .Verifiable();

        var vm = CreateViewModel(HelmDoor.First);

        _savedProgressProviderMock.Verify();
    }

    #endregion

    #region Barrier Amount Changed Event Tests

    [Fact]
    public void OnBarrierAmountChanged_WithMatchingDoor_UpdatesBarrierCount()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<HelmBarrierAmountChangedEventArgs>? barrierChangeHandler = null;
            _barrierServiceMock.SetupAdd(x => x.HelmBarrierAmountChanged += It.IsAny<EventHandler<HelmBarrierAmountChangedEventArgs>>())
                .Callback((EventHandler<HelmBarrierAmountChangedEventArgs> handler) => barrierChangeHandler = handler);

            var vm = new HelmDoorHintViewModel(
                HelmDoor.First,
                _barrierServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object
            );

            barrierChangeHandler?.Invoke(_barrierServiceMock.Object,
                new HelmBarrierAmountChangedEventArgs(HelmDoor.First, "0", "5"));

            Thread.Sleep(50); // Allow UI context post to execute

            Assert.Equal("5", vm.BarrierCount);
        });
    }

    [Fact]
    public void OnBarrierAmountChanged_WithNonMatchingDoor_IgnoresEvent()
    {
        SynchronizationContextTestHelper.RunWithUIContext(() =>
        {
            EventHandler<HelmBarrierAmountChangedEventArgs>? barrierChangeHandler = null;
            _barrierServiceMock.SetupAdd(x => x.HelmBarrierAmountChanged += It.IsAny<EventHandler<HelmBarrierAmountChangedEventArgs>>())
                .Callback((EventHandler<HelmBarrierAmountChangedEventArgs> handler) => barrierChangeHandler = handler);

            _barrierServiceMock.Setup(x => x.GetHelmDoorCount(HelmDoor.First)).Returns("0");
            var vm = new HelmDoorHintViewModel(
                HelmDoor.First,
                _barrierServiceMock.Object,
                _savedProgressProviderMock.Object,
                _itemSelectorServiceMock.Object
            );

            barrierChangeHandler?.Invoke(_barrierServiceMock.Object,
                new HelmBarrierAmountChangedEventArgs(HelmDoor.Second, "0", "5"));

            Assert.Equal("0", vm.BarrierCount);
        });
    }

    #endregion

    #region Progress Changed Event Tests

    [Fact]
    public void OnDataReset_ReinitializesState()
    {
        _barrierServiceMock.Setup(x => x.GetHelmDoorCount(HelmDoor.First)).Returns("5");
        var vm = CreateViewModel(HelmDoor.First);

        _barrierServiceMock.Setup(x => x.GetHelmDoorCount(HelmDoor.First)).Returns("10");
        _savedProgressProviderMock.Raise(x => x.ProgressChanged += null,
            new ProgressReplacedEventArgs { OldProgress = null, NewProgress = null! });

        Assert.Equal("10", vm.BarrierCount);
    }

    #endregion

    #region Cross-Thread Marshaling Tests

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromHelmBarrierAmountChangedEvent()
    {
        var vm = CreateViewModel(HelmDoor.First);
        vm.Dispose();

        _barrierServiceMock.VerifyRemove(x => x.HelmBarrierAmountChanged -= It.IsAny<EventHandler<HelmBarrierAmountChangedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_UnsubscribesFromProgressChangedEvent()
    {
        var vm = CreateViewModel(HelmDoor.First);
        vm.Dispose();

        _savedProgressProviderMock.VerifyRemove(x => x.ProgressChanged -= It.IsAny<EventHandler<ProgressReplacedEventArgs>>(), Times.Once);
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var vm = CreateViewModel(HelmDoor.First);

        vm.Dispose();
        vm.Dispose();

        // Should not throw
        _barrierServiceMock.VerifyRemove(x => x.HelmBarrierAmountChanged -= It.IsAny<EventHandler<HelmBarrierAmountChangedEventArgs>>(), Times.Once);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_WithSameDoor_ReturnsTrue()
    {
        var vm1 = CreateViewModel(HelmDoor.First);
        var vm2 = CreateViewModel(HelmDoor.First);

        Assert.Equal(vm1, vm2);
    }

    [Fact]
    public void Equals_WithDifferentDoor_ReturnsFalse()
    {
        var vm1 = CreateViewModel(HelmDoor.First);
        var vm2 = CreateViewModel(HelmDoor.Second);

        Assert.NotEqual(vm1, vm2);
    }

    [Fact]
    public void GetHashCode_WithSameDoor_ReturnsSameHash()
    {
        var vm1 = CreateViewModel(HelmDoor.First);
        var vm2 = CreateViewModel(HelmDoor.First);

        Assert.Equal(vm1.GetHashCode(), vm2.GetHashCode());
    }

    #endregion
}
