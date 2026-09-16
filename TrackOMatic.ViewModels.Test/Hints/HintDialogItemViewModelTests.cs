namespace TrackOMatic.ViewModels.Test.Hints;

public class HintDialogItemViewModelTests
{
    private static HintGridCell CreateTestCell(ItemName itemName = ItemName.DONKEY)
    {
        return new HintGridCell(Row: 0, Column: 0, ItemName: itemName);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidCell_Succeeds()
    {
        var cell = CreateTestCell();
        var vm = new HintDialogItemViewModel(cell, isSelected: true);
        Assert.NotNull(vm);
    }

    [Fact]
    public void Constructor_InitializesCell()
    {
        var cell = CreateTestCell();
        var vm = new HintDialogItemViewModel(cell, isSelected: false);
        Assert.Equal(cell, vm.Cell);
    }

    [Fact]
    public void Constructor_WithIsSelectedTrue_InitializesIsSelected()
    {
        var cell = CreateTestCell();
        var vm = new HintDialogItemViewModel(cell, isSelected: true);
        Assert.True(vm.IsSelected);
    }

    [Fact]
    public void Constructor_WithIsSelectedFalse_InitializesIsSelected()
    {
        var cell = CreateTestCell();
        var vm = new HintDialogItemViewModel(cell, isSelected: false);
        Assert.False(vm.IsSelected);
    }

    #endregion

    #region ImageResourceKey Tests

    [Fact]
    public void Constructor_WithIsSelectedTrue_SetsColorImageResourceKey()
    {
        var cell = CreateTestCell(ItemName.DONKEY);
        var vm = new HintDialogItemViewModel(cell, isSelected: true);
        Assert.Equal("donkey", vm.ImageResourceKey);
    }

    [Fact]
    public void Constructor_WithIsSelectedFalse_SetsBWImageResourceKey()
    {
        var cell = CreateTestCell(ItemName.DONKEY);
        var vm = new HintDialogItemViewModel(cell, isSelected: false);
        Assert.Equal("donkey_bw", vm.ImageResourceKey);
    }

    [Theory]
    [InlineData(ItemName.DONKEY, "donkey")]
    [InlineData(ItemName.DIDDY, "diddy")]
    [InlineData(ItemName.RAREWARE_COIN, "rareware_coin")]
    [InlineData(ItemName.FAIRY_CAMERA, "fairy_camera")]
    public void Constructor_WithSelectedItem_UsesCorrectItemName(ItemName itemName, string expectedKey)
    {
        var cell = CreateTestCell(itemName);
        var vm = new HintDialogItemViewModel(cell, isSelected: true);
        Assert.Equal(expectedKey, vm.ImageResourceKey);
    }

    [Theory]
    [InlineData(ItemName.DONKEY, "donkey_bw")]
    [InlineData(ItemName.DIDDY, "diddy_bw")]
    [InlineData(ItemName.RAREWARE_COIN, "rareware_coin_bw")]
    [InlineData(ItemName.FAIRY_CAMERA, "fairy_camera_bw")]
    public void Constructor_WithUnselectedItem_UsesCorrectBWKey(ItemName itemName, string expectedKey)
    {
        var cell = CreateTestCell(itemName);
        var vm = new HintDialogItemViewModel(cell, isSelected: false);
        Assert.Equal(expectedKey, vm.ImageResourceKey);
    }

    #endregion

    #region Toggle Selection Command Tests

    [Fact]
    public void ToggleSelection_WhenSelected_BecomesUnselected()
    {
        var cell = CreateTestCell();
        var vm = new HintDialogItemViewModel(cell, isSelected: true);

        vm.ToggleSelectionCommand.Execute(null);

        Assert.False(vm.IsSelected);
    }

    [Fact]
    public void ToggleSelection_WhenUnselected_BecomesSelected()
    {
        var cell = CreateTestCell();
        var vm = new HintDialogItemViewModel(cell, isSelected: false);

        vm.ToggleSelectionCommand.Execute(null);

        Assert.True(vm.IsSelected);
    }

    [Fact]
    public void ToggleSelection_UpdatesImageResourceKeyFromColorToBW()
    {
        var cell = CreateTestCell(ItemName.DONKEY);
        var vm = new HintDialogItemViewModel(cell, isSelected: true);
        Assert.Equal("donkey", vm.ImageResourceKey);

        vm.ToggleSelectionCommand.Execute(null);

        Assert.Equal("donkey_bw", vm.ImageResourceKey);
    }

    [Fact]
    public void ToggleSelection_UpdatesImageResourceKeyFromBWToColor()
    {
        var cell = CreateTestCell(ItemName.DONKEY);
        var vm = new HintDialogItemViewModel(cell, isSelected: false);
        Assert.Equal("donkey_bw", vm.ImageResourceKey);

        vm.ToggleSelectionCommand.Execute(null);

        Assert.Equal("donkey", vm.ImageResourceKey);
    }

    [Fact]
    public void ToggleSelection_CanBeToggledMultipleTimes()
    {
        var cell = CreateTestCell();
        var vm = new HintDialogItemViewModel(cell, isSelected: true);

        vm.ToggleSelectionCommand.Execute(null);
        Assert.False(vm.IsSelected);

        vm.ToggleSelectionCommand.Execute(null);
        Assert.True(vm.IsSelected);

        vm.ToggleSelectionCommand.Execute(null);
        Assert.False(vm.IsSelected);
    }

    [Fact]
    public void ToggleSelection_RaisesPropertyChangedForIsSelected()
    {
        var cell = CreateTestCell();
        var vm = new HintDialogItemViewModel(cell, isSelected: true);
        var propertyChangedRaised = false;
        var changedPropertyName = string.Empty;

        vm.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(vm.IsSelected))
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            }
        };

        vm.ToggleSelectionCommand.Execute(null);

        Assert.True(propertyChangedRaised);
        Assert.Equal(nameof(vm.IsSelected), changedPropertyName);
    }

    #endregion

    #region Cell Change Tests

    [Fact]
    public void CellChange_DoesNotAutomaticallyUpdateImageResourceKey()
    {
        var cell1 = CreateTestCell(ItemName.DONKEY);
        var vm = new HintDialogItemViewModel(cell1, isSelected: true);
        Assert.Equal("donkey", vm.ImageResourceKey);

        var cell2 = CreateTestCell(ItemName.DIDDY);
        vm.Cell = cell2;

        // ImageResourceKey is not automatically updated when Cell changes
        Assert.Equal("donkey", vm.ImageResourceKey);
    }

    [Fact]
    public void CellChange_MaintainsSelectionState()
    {
        var cell1 = CreateTestCell(ItemName.DONKEY);
        var vm = new HintDialogItemViewModel(cell1, isSelected: true);
        Assert.True(vm.IsSelected);

        var cell2 = CreateTestCell(ItemName.DIDDY);
        vm.Cell = cell2;

        Assert.True(vm.IsSelected);
    }

    #endregion
}
