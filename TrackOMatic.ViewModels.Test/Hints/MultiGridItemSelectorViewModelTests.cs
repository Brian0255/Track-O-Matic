namespace TrackOMatic.ViewModels.Test.Hints;

public class MultiGridItemSelectorViewModelTests
{
    private static HintGridCell CreateCell(int row, int column, ItemName itemName)
    {
        return new HintGridCell(Row: row, Column: column, ItemName: itemName);
    }

    private static HintDialogItemViewModel CreateItem(ItemName itemName, bool isSelected = false)
    {
        var cell = CreateCell(0, 0, itemName);
        return new HintDialogItemViewModel(cell, isSelected);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidItems_Succeeds()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY),
            CreateItem(ItemName.DIDDY)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.NotNull(vm);
    }

    [Fact]
    public void Constructor_WithNullItems_InitializesEmptyCollections()
    {
        var vm = new MultiGridItemSelectorViewModel(null!);
        Assert.Empty(vm.AllItems);
    }

    [Fact]
    public void Constructor_WithEmptyItems_InitializesEmptyCollections()
    {
        var vm = new MultiGridItemSelectorViewModel([]);
        Assert.Empty(vm.AllItems);
    }

    [Fact]
    public void Constructor_WithValidItems_PopulatesAllItems()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY),
            CreateItem(ItemName.DIDDY),
            CreateItem(ItemName.RAREWARE_COIN)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Equal(3, vm.AllItems.Count);
    }

    [Fact]
    public void Constructor_InitializesEmptySelectedItems()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY, isSelected: true),
            CreateItem(ItemName.DIDDY, isSelected: true)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Empty(vm.SelectedItems);
    }

    #endregion

    #region Item Partitioning Tests

    [Fact]
    public void Constructor_WithKongItems_AddsToKongMovesCollection()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY),
            CreateItem(ItemName.DIDDY)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Multiple(() =>
        {
            Assert.Equal(2, vm.KongMoves.Count);
            Assert.Contains(items[0], vm.KongMoves);
            Assert.Contains(items[1], vm.KongMoves);
        });
    }

    [Fact]
    public void Constructor_WithFinalBossItems_AddsToFinalBossCollection()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY_KROOL),
            CreateItem(ItemName.DIDDY_KROOL),
            CreateItem(ItemName.CHUNKY_KROOL)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Equal(3, vm.FinalBossPhases.Count);
    }

    [Fact]
    public void Constructor_WithKeyItems_AddsToKeysCollection()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.KEY_1),
            CreateItem(ItemName.KEY_2),
            CreateItem(ItemName.KEY_3)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Equal(3, vm.Keys.Count);
    }

    [Fact]
    public void Constructor_WithBossItems_AddsToLevelBossesCollection()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.ARMY),
            CreateItem(ItemName.DOGA),
            CreateItem(ItemName.MADJACK)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Equal(3, vm.LevelBosses.Count);
    }

    [Fact]
    public void Constructor_WithSharedMoveItems_AddsToSharedMovesCollection()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.BEAN),
            CreateItem(ItemName.SNIPER_SCOPE),
            CreateItem(ItemName.HOMING_AMMO)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Equal(3, vm.SharedMoves.Count);
    }

    [Fact]
    public void Constructor_WithTrainingMoveItems_AddsToTrainingMovesCollection()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.VINE_SWINGING),
            CreateItem(ItemName.BARREL_THROWING),
            CreateItem(ItemName.DIVING)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Equal(3, vm.TrainingMoves.Count);
    }

    [Fact]
    public void Constructor_WithPotionItems_AddsToPotionsCollection()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.KONG_FAMILY),
            CreateItem(ItemName.POTION_DK),
            CreateItem(ItemName.POTION_DIDDY)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Equal(3, vm.Potions.Count);
    }

    [Fact]
    public void Constructor_WithShopkeeperItems_AddsToShopkeepersCollection()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.CRANKY),
            CreateItem(ItemName.CANDY),
            CreateItem(ItemName.FUNKY)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Equal(3, vm.Shopkeepers.Count);
    }

    [Fact]
    public void Constructor_WithMiscItems_AddsToMiscItemsCollection()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.UNKNOWN_VIAL),
            CreateItem(ItemName.NINTENDO_COIN),
            CreateItem(ItemName.HINT)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Equal(3, vm.MiscItems.Count);
    }

    [Fact]
    public void Constructor_SubscribesToAllItemsPropertyChanged()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY),
            CreateItem(ItemName.DIDDY)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        items[0].ToggleSelectionCommand.Execute(null);

        // Item should be tracked in SelectedItems
        Assert.Contains(items[0], vm.SelectedItems);
    }

    #endregion

    #region Selection Tracking Tests

    [Fact]
    public void OnItemSelected_AddsToSelectionList()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY, isSelected: false)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        Assert.Empty(vm.SelectedItems);

        items[0].ToggleSelectionCommand.Execute(null);

        Assert.Single(vm.SelectedItems);
        Assert.Contains(items[0], vm.SelectedItems);
    }

    [Fact]
    public void OnItemDeselected_RemovesFromSelectionList()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY, isSelected: true)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        items[0].ToggleSelectionCommand.Execute(null);

        Assert.Empty(vm.SelectedItems);
    }

    [Fact]
    public void OnMultipleItemsSelected_TrackesAllSelections()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY, isSelected: false),
            CreateItem(ItemName.DIDDY, isSelected: false),
            CreateItem(ItemName.RAREWARE_COIN, isSelected: false)
        };

        var vm = new MultiGridItemSelectorViewModel(items);

        items[0].ToggleSelectionCommand.Execute(null);
        items[2].ToggleSelectionCommand.Execute(null);

        Assert.Multiple(() =>
        {
            Assert.Equal(2, vm.SelectedItems.Count);
            Assert.Contains(items[0], vm.SelectedItems);
            Assert.Contains(items[2], vm.SelectedItems);
            Assert.DoesNotContain(items[1], vm.SelectedItems);
        });
    }

    [Fact]
    public void OnSelectionToggled_TracksChanges()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY, isSelected: false)
        };

        var vm = new MultiGridItemSelectorViewModel(items);

        items[0].ToggleSelectionCommand.Execute(null);
        Assert.Single(vm.SelectedItems);

        items[0].ToggleSelectionCommand.Execute(null);
        Assert.Empty(vm.SelectedItems);
    }

    #endregion

    #region GetSelectedCells Tests

    [Fact]
    public void GetSelectedCells_WithNoSelection_ReturnsEmpty()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY, isSelected: false),
            CreateItem(ItemName.DIDDY, isSelected: false)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        var selected = vm.GetSelectedCells();

        Assert.Empty(selected);
    }

    [Fact]
    public void GetSelectedCells_WithSingleSelection_ReturnsSingleCell()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY, isSelected: true),
            CreateItem(ItemName.DIDDY, isSelected: false)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        var selected = vm.GetSelectedCells();

        Assert.Single(selected);
        Assert.Equal(ItemName.DONKEY, selected[0].ItemName);
    }

    [Fact]
    public void GetSelectedCells_WithMultipleSelections_ReturnsAllSelectedCells()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY, isSelected: true),
            CreateItem(ItemName.DIDDY, isSelected: false),
            CreateItem(ItemName.RAREWARE_COIN, isSelected: true)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        var selected = vm.GetSelectedCells();

        Assert.Multiple(() =>
        {
            Assert.Equal(2, selected.Count);
            Assert.Contains(selected, c => c.ItemName == ItemName.DONKEY);
            Assert.Contains(selected, c => c.ItemName == ItemName.RAREWARE_COIN);
        });
    }

    [Fact]
    public void GetSelectedCells_ReflectsCurrentState()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY, isSelected: false),
            CreateItem(ItemName.DIDDY, isSelected: false)
        };

        var vm = new MultiGridItemSelectorViewModel(items);

        items[0].ToggleSelectionCommand.Execute(null);
        var selected = vm.GetSelectedCells();

        Assert.Single(selected);
        Assert.Equal(ItemName.DONKEY, selected[0].ItemName);
    }

    #endregion

    #region ConfirmSelection Command Tests

    [Fact]
    public void ConfirmSelection_RaisesCloseRequestedEvent()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        var eventRaised = false;

        vm.CloseRequested += (sender, e) => eventRaised = true;

        vm.ConfirmSelectionCommand.Execute(null);

        Assert.True(eventRaised);
    }

    [Fact]
    public void ConfirmSelection_SendsSelfAsEventSender()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        object? eventSender = null;

        vm.CloseRequested += (sender, e) => eventSender = sender;

        vm.ConfirmSelectionCommand.Execute(null);

        Assert.Equal(vm, eventSender);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_UnsubscribesFromAllItems()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY),
            CreateItem(ItemName.DIDDY)
        };

        var vm = new MultiGridItemSelectorViewModel(items);
        vm.Dispose();

        // After dispose, toggling should not affect selected items
        items[0].ToggleSelectionCommand.Execute(null);

        Assert.Empty(vm.SelectedItems);
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY)
        };

        var vm = new MultiGridItemSelectorViewModel(items);

        vm.Dispose();
        vm.Dispose(); // Should not throw

        Assert.True(true); // Reached without exception
    }

    [Fact]
    public void Dispose_CanBeCalledViaUsingStatement()
    {
        var items = new List<HintDialogItemViewModel>
        {
            CreateItem(ItemName.DONKEY)
        };

        using var vm = new MultiGridItemSelectorViewModel(items);
        Assert.NotNull(vm);
        // Should not throw
    }

    #endregion
}
