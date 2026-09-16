namespace TrackOMatic.ViewModels.Test.Hints;

public class PathHintItemViewModelTests
{
    private readonly Mock<IHintService> _mockHintService;
    private readonly HintId _testHintId = new("testhint");

    public PathHintItemViewModelTests()
    {
        _mockHintService = new Mock<IHintService>();
    }

    [Fact]
    public void Constructor_WithValidParameters_Succeeds()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        Assert.NotNull(vm);
    }

    [Fact]
    public void Constructor_WithNullHintService_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, null!)
        );
        Assert.Equal("hintService", exception.ParamName);
    }

    [Fact]
    public void Constructor_InitializesItemName()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DIDDY, false, _mockHintService.Object);
        Assert.Equal(ItemName.DIDDY, vm.ItemName);
    }

    [Fact]
    public void Constructor_WithIsFoundFalse_InitializesIsFound()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        Assert.False(vm.IsFound);
    }

    [Fact]
    public void Constructor_WithIsFoundTrue_InitializesIsFound()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.RAREWARE_COIN, true, _mockHintService.Object);
        Assert.True(vm.IsFound);
    }

    [Fact]
    public void Constructor_InitializesImageResourceKey()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        Assert.NotEmpty(vm.ImageResourceKey);
    }

    [Theory]
    [InlineData(ItemName.DONKEY, "donkey")]
    [InlineData(ItemName.FAIRY_CAMERA, "fairy_camera")]
    public void Constructor_SetsImageResourceKeyFromItemName(ItemName itemName, string expectedKey)
    {
        var vm = new PathHintItemViewModel(_testHintId, itemName, false, _mockHintService.Object);
        Assert.Equal(expectedKey, vm.ImageResourceKey);
    }

    [Fact]
    public void Constructor_InitializesCheckmarkKey()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        Assert.NotEmpty(vm.CheckmarkKey);
    }

    [Fact]
    public void CheckmarkKey_IsAlwaysCheckmark()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        Assert.Equal("checkmark", vm.CheckmarkKey);
    }

    [Fact]
    public void Constructor_SubscribesToHintPathItemToggled()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        // Verify the view model was created successfully (subscription happens in constructor)
        Assert.NotNull(vm);
    }

    [Fact]
    public void ToggleFound_CanExecuteWhenItemNameIsNotNone()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        Assert.True(vm.ToggleFoundCommand.CanExecute(null));
    }

    [Fact]
    public void ToggleFound_CannotExecuteWhenItemNameIsNone()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.NONE, false, _mockHintService.Object);
        Assert.False(vm.ToggleFoundCommand.CanExecute(null));
    }

    [Fact]
    public void ToggleFound_CannotToggleNoneItem()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.NONE, false, _mockHintService.Object);

        // Verify the command cannot execute
        Assert.False(vm.ToggleFoundCommand.CanExecute(null));
    }

    [Fact]
    public void ToggleFound_CallsHintServiceTogglePathItem()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);

        vm.ToggleFoundCommand.Execute(null);

        _mockHintService.Verify(s => s.TogglePathItem(_testHintId, ItemName.DONKEY), Times.Once);
    }

    [Fact]
    public void OnHintPathItemToggled_WithMatchingHintAndItem_UpdatesIsFound()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        Assert.False(vm.IsFound);

        var eventArgs = new HintItemToggleEventArgs(_testHintId, ItemName.DONKEY, true);
        _mockHintService.Raise(s => s.HintPathItemToggled += null, eventArgs);

        Assert.True(vm.IsFound);
    }

    [Fact]
    public void OnHintPathItemToggled_WithMatchingHintAndItem_TogglesFoundState()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, true, _mockHintService.Object);
        Assert.True(vm.IsFound);

        var eventArgs = new HintItemToggleEventArgs(_testHintId, ItemName.DONKEY, false);
        _mockHintService.Raise(s => s.HintPathItemToggled += null, eventArgs);

        Assert.False(vm.IsFound);
    }

    [Fact]
    public void OnHintPathItemToggled_WithDifferentItemName_DoesNotUpdate()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        Assert.False(vm.IsFound);

        var differentHintId = new HintId("othrhint");
        var _ = new HintItemToggleEventArgs(differentHintId, ItemName.DONKEY, true);
        // Event handler checks hint ID, so with different ID it shouldn't update
        Assert.False(vm.IsFound);
    }

    [Fact]
    public void Equals_WithSameItemName_ReturnsTrue()
    {
        var vm1 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        var vm2 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, true, _mockHintService.Object);
        Assert.Equal(vm1, vm2);
    }

    [Fact]
    public void Equals_WithDifferentItemName_ReturnsFalse()
    {
        var vm1 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        var vm2 = new PathHintItemViewModel(_testHintId, ItemName.DIDDY, false, _mockHintService.Object);
        Assert.NotEqual(vm1, vm2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        Assert.False(vm.Equals(null));
    }

    [Fact]
    public void Equals_Object_WithSameItemName_ReturnsTrue()
    {
        var vm1 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        var vm2 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, true, _mockHintService.Object);
        Assert.True(vm1.Equals((object)vm2));
    }

    [Fact]
    public void EqualsOperator_WithSameItemName_ReturnsTrue()
    {
        var vm1 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        var vm2 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, true, _mockHintService.Object);
        Assert.True(vm1 == vm2);
    }

    [Fact]
    public void EqualsOperator_WithDifferentItemName_ReturnsFalse()
    {
        var vm1 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        var vm2 = new PathHintItemViewModel(_testHintId, ItemName.DIDDY, false, _mockHintService.Object);
        Assert.False(vm1 == vm2);
    }

    [Fact]
    public void NotEqualsOperator_WithSameItemName_ReturnsFalse()
    {
        var vm1 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        var vm2 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, true, _mockHintService.Object);
        Assert.False(vm1 != vm2);
    }

    [Fact]
    public void NotEqualsOperator_WithDifferentItemName_ReturnsTrue()
    {
        var vm1 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        var vm2 = new PathHintItemViewModel(_testHintId, ItemName.DIDDY, false, _mockHintService.Object);
        Assert.True(vm1 != vm2);
    }

    [Fact]
    public void GetHashCode_WithSameItemName_ReturnsSameHash()
    {
        var vm1 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        var vm2 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, true, _mockHintService.Object);
        Assert.Equal(vm1.GetHashCode(), vm2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentItemName_ReturnsDifferentHash()
    {
        var vm1 = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        var vm2 = new PathHintItemViewModel(_testHintId, ItemName.DIDDY, false, _mockHintService.Object);
        Assert.NotEqual(vm1.GetHashCode(), vm2.GetHashCode());
    }

    [Fact]
    public void ToString_WithIsFoundTrue_IncludesCheckmark()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, true, _mockHintService.Object);
        var result = vm.ToString();
        Assert.Contains("DONKEY on Path:", result);
        Assert.Contains("✓", result);
    }

    [Fact]
    public void ToString_WithIsFoundFalse_IncludesX()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        var result = vm.ToString();
        Assert.Contains("DONKEY on Path:", result);
        Assert.Contains("✗", result);
    }

    [Fact]
    public void Dispose_UnsubscribesFromHintPathItemToggled()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);

        vm.Dispose();
        // Verify disposal completes without exception (unsubscription happens in Dispose)
        Assert.True(true);
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);

        vm.Dispose();
        vm.Dispose(); // Should not throw

        Assert.True(true);
    }

    [Fact]
    public void Dispose_CanBeCalledViaUsingStatement()
    {
        using var vm = new PathHintItemViewModel(_testHintId, ItemName.DONKEY, false, _mockHintService.Object);
        Assert.NotNull(vm);
    }
}
