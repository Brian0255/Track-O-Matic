namespace TrackOMatic.ViewModels.Test;

/// <summary>
/// Test suite for FlexibleGridItemSelectorViewModel.
/// Tests constructor behavior with various input scenarios, grouping logic,
/// row organization, selection state management, and property notifications.
/// </summary>
public class FlexibleGridItemSelectorViewModelTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullItems_CreatesEmptyItemGroups()
    {
        var viewModel = new FlexibleGridItemSelectorViewModel(null!);

        Assert.NotNull(viewModel.ItemGroups);
        Assert.Empty(viewModel.ItemGroups);
    }

    [Fact]
    public void Constructor_WithEmptyList_CreatesEmptyItemGroups()
    {
        var items = new List<ItemGridCell>();
        var viewModel = new FlexibleGridItemSelectorViewModel(items);

        Assert.NotNull(viewModel.ItemGroups);
        Assert.Empty(viewModel.ItemGroups);
    }

    [Fact]
    public void Constructor_WithSingleItem_CreatesSingleRowGroup()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "item_1")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);
        Assert.Multiple(() =>
        {
            Assert.Single(viewModel.ItemGroups);
            Assert.Equal(0, viewModel.ItemGroups[0].RowIndex);
            Assert.Single(viewModel.ItemGroups[0].Items);
            Assert.Equal("item_1", viewModel.ItemGroups[0].Items[0].ResourceKey);
        });
    }

    [Fact]
    public void Constructor_WithMultipleItemsSingleRow_CreatesOneRowGroup()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "item_1"),
            new(0, 1, "item_2"),
            new(0, 2, "item_3")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);
        Assert.Multiple(() =>
        {
            Assert.Single(viewModel.ItemGroups);
            Assert.Equal(0, viewModel.ItemGroups[0].RowIndex);
            Assert.Equal(3, viewModel.ItemGroups[0].Items.Count);
        });
    }

    [Fact]
    public void Constructor_WithMultipleRows_CreatesMultipleRowGroups()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "item_1"),
            new(0, 1, "item_2"),
            new(1, 0, "item_3"),
            new(1, 1, "item_4"),
            new(2, 0, "item_5")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);
        Assert.Multiple(() =>
        {
            Assert.Equal(3, viewModel.ItemGroups.Count);
            Assert.Equal(0, viewModel.ItemGroups[0].RowIndex);
            Assert.Equal(1, viewModel.ItemGroups[1].RowIndex);
            Assert.Equal(2, viewModel.ItemGroups[2].RowIndex);
        });
    }

    [Fact]
    public void Constructor_ItemsInWrongOrder_GroupsAndSortsByRowThenColumn()
    {
        // Items provided out of order
        var items = new List<ItemGridCell>
        {
            new(2, 1, "item_5"),
            new(0, 2, "item_3"),
            new(1, 0, "item_4"),
            new(0, 0, "item_1"),
            new(0, 1, "item_2")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);

        // Should be sorted: row 0 (cols 0,1,2), row 1 (col 0), row 2 (col 1)
        Assert.Equal(3, viewModel.ItemGroups.Count);

        // Row 0
        Assert.Equal(3, viewModel.ItemGroups[0].Items.Count);
        Assert.Equal("item_1", viewModel.ItemGroups[0].Items[0].ResourceKey);
        Assert.Equal("item_2", viewModel.ItemGroups[0].Items[1].ResourceKey);
        Assert.Equal("item_3", viewModel.ItemGroups[0].Items[2].ResourceKey);

        // Row 1
        Assert.Single(viewModel.ItemGroups[1].Items);
        Assert.Equal("item_4", viewModel.ItemGroups[1].Items[0].ResourceKey);

        // Row 2
        Assert.Single(viewModel.ItemGroups[2].Items);
        Assert.Equal("item_5", viewModel.ItemGroups[2].Items[0].ResourceKey);
    }

    #endregion

    #region Item Grouping Tests

    [Fact]
    public void ItemGroups_MaintainsCorrectRowIndices()
    {
        var items = new List<ItemGridCell>
        {
            new(5, 0, "row5_col0"),
            new(3, 0, "row3_col0"),
            new(5, 1, "row5_col1"),
            new(3, 1, "row3_col1")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);
        Assert.Multiple(() =>
        {
            Assert.Equal(2, viewModel.ItemGroups.Count);
            Assert.Equal(3, viewModel.ItemGroups[0].RowIndex);
            Assert.Equal(5, viewModel.ItemGroups[1].RowIndex);
        });
    }

    [Fact]
    public void ItemGroups_SortsItemsByColumnWithinRow()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 3, "col3"),
            new(0, 1, "col1"),
            new(0, 2, "col2"),
            new(0, 0, "col0")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);

        Assert.Multiple(() =>
        {
            Assert.Single(viewModel.ItemGroups);
            var row = viewModel.ItemGroups[0];
            Assert.Equal(4, row.Items.Count);
            Assert.Equal("col0", row.Items[0].ResourceKey);
            Assert.Equal("col1", row.Items[1].ResourceKey);
            Assert.Equal("col2", row.Items[2].ResourceKey);
            Assert.Equal("col3", row.Items[3].ResourceKey);
        });
    }

    [Fact]
    public void ItemGroups_GroupsCorrectlyWithNonConsecutiveRows()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "row0"),
            new(10, 0, "row10"),
            new(5, 0, "row5")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);
        Assert.Multiple(() =>
        {
            Assert.Equal(3, viewModel.ItemGroups.Count);
            Assert.Equal(0, viewModel.ItemGroups[0].RowIndex);
            Assert.Equal(5, viewModel.ItemGroups[1].RowIndex);
            Assert.Equal(10, viewModel.ItemGroups[2].RowIndex);
        });
    }

    #endregion

    #region SelectedItem Tests

    [Fact]
    public void SelectedItem_InitializesToNull()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "item_1")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);

        Assert.Null(viewModel.SelectedItem);
    }

    [Fact]
    public void SelectedItem_CanBeSet()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "item_1"),
            new(0, 1, "item_2")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);
        var itemToSelect = viewModel.ItemGroups[0].Items[0];

        viewModel.SelectedItem = itemToSelect;

        Assert.NotNull(viewModel.SelectedItem);
        Assert.Equal("item_1", viewModel.SelectedItem!.ResourceKey);
    }

    [Fact]
    public void SelectedItem_CanBeChangedMultipleTimes()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "item_1"),
            new(0, 1, "item_2"),
            new(1, 0, "item_3")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);

        // Select first item
        viewModel.SelectedItem = viewModel.ItemGroups[0].Items[0];
        Assert.Equal("item_1", viewModel.SelectedItem!.ResourceKey);

        // Change to second item
        viewModel.SelectedItem = viewModel.ItemGroups[0].Items[1];
        Assert.Equal("item_2", viewModel.SelectedItem!.ResourceKey);

        // Change to item from different row
        viewModel.SelectedItem = viewModel.ItemGroups[1].Items[0];
        Assert.Equal("item_3", viewModel.SelectedItem!.ResourceKey);
    }

    [Fact]
    public void SelectedItem_CanBeSetToNull()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "item_1")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);
        var item = viewModel.ItemGroups[0].Items[0];

        viewModel.SelectedItem = item;
        Assert.NotNull(viewModel.SelectedItem);

        viewModel.SelectedItem = null;
        Assert.Null(viewModel.SelectedItem);
    }

    #endregion

    #region PropertyChanged Notification Tests

    [Fact]
    public void SelectedItem_RaisesPropertyChangedNotification()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "item_1"),
            new(0, 1, "item_2")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);
        var propertyChangedRaised = false;
        var changedProperty = string.Empty;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(FlexibleGridItemSelectorViewModel.SelectedItem))
            {
                propertyChangedRaised = true;
                changedProperty = e.PropertyName;
            }
        };

        viewModel.SelectedItem = viewModel.ItemGroups[0].Items[0];

        Assert.True(propertyChangedRaised);
        Assert.Equal(nameof(FlexibleGridItemSelectorViewModel.SelectedItem), changedProperty);
    }

    [Fact]
    public void SelectedItem_DoesNotRaisePropertyChangedIfSameValue()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "item_1")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);
        var item = viewModel.ItemGroups[0].Items[0];
        var notificationCount = 0;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(FlexibleGridItemSelectorViewModel.SelectedItem))
            {
                notificationCount++;
            }
        };

        viewModel.SelectedItem = item;
        Assert.Equal(1, notificationCount);

        // Set to same value again
        viewModel.SelectedItem = item;
        Assert.Equal(1, notificationCount); // Should not increase
    }

    [Fact]
    public void SelectedItem_RaisesPropertyChangedWhenSettingToNull()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "item_1")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);
        viewModel.SelectedItem = viewModel.ItemGroups[0].Items[0];

        var propertyChangedRaised = false;

        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(FlexibleGridItemSelectorViewModel.SelectedItem))
            {
                propertyChangedRaised = true;
            }
        };

        viewModel.SelectedItem = null;

        Assert.True(propertyChangedRaised);
    }

    #endregion

    #region Large Dataset Tests

    [Fact]
    public void Constructor_WithLargeDataset_HandlesCorrectly()
    {
        var items = new List<ItemGridCell>();

        // Create 10 rows with 10 items each
        for (int row = 0; row < 10; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                items.Add(new ItemGridCell(row, col, $"item_r{row}_c{col}"));
            }
        }

        var viewModel = new FlexibleGridItemSelectorViewModel(items);

        Assert.Equal(10, viewModel.ItemGroups.Count);

        for (int row = 0; row < 10; row++)
        {
            Assert.Equal(row, viewModel.ItemGroups[row].RowIndex);
            Assert.Equal(10, viewModel.ItemGroups[row].Items.Count);

            for (int col = 0; col < 10; col++)
            {
                Assert.Equal($"item_r{row}_c{col}", viewModel.ItemGroups[row].Items[col].ResourceKey);
            }
        }
    }

    #endregion

    #region ItemGridRowGroup Tests

    [Fact]
    public void ItemGridRowGroup_StoresRowIndexAndItems()
    {
        var items = new List<ItemGridCell>
        {
            new(5, 0, "item_1"),
            new(5, 1, "item_2")
        };

        var rowGroup = new ItemGridRowGroup(5, items);

        Assert.Multiple(() =>
        {
            Assert.Equal(5, rowGroup.RowIndex);
            Assert.Equal(2, rowGroup.Items.Count);
            Assert.Equal("item_1", rowGroup.Items[0].ResourceKey);
            Assert.Equal("item_2", rowGroup.Items[1].ResourceKey);
        });
    }

    [Fact]
    public void ItemGridRowGroup_WithEmptyItems_CreatesEmptyCollection()
    {
        var items = new List<ItemGridCell>();
        var rowGroup = new ItemGridRowGroup(0, items);

        Assert.Equal(0, rowGroup.RowIndex);
        Assert.Empty(rowGroup.Items);
    }

    [Fact]
    public void ItemGridRowGroup_ItemsAreObservable()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 0, "item_1")
        };

        var rowGroup = new ItemGridRowGroup(0, items);

        Assert.NotNull(rowGroup.Items);
        Assert.IsType<System.Collections.ObjectModel.ObservableCollection<ItemGridCell>>(rowGroup.Items, exactMatch: false);
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void Constructor_WithNegativeRowIndices_GroupsCorrectly()
    {
        var items = new List<ItemGridCell>
        {
            new(-1, 0, "neg_row"),
            new(0, 0, "zero_row"),
            new(1, 0, "pos_row")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);

        Assert.Multiple(() =>
        {
            Assert.Equal(3, viewModel.ItemGroups.Count);
            Assert.Equal(-1, viewModel.ItemGroups[0].RowIndex);
            Assert.Equal(0, viewModel.ItemGroups[1].RowIndex);
            Assert.Equal(1, viewModel.ItemGroups[2].RowIndex);
        });
    }

    [Fact]
    public void Constructor_WithNegativeColumnIndices_SortsCorrectly()
    {
        var items = new List<ItemGridCell>
        {
            new(0, 2, "col2"),
            new(0, -1, "col_neg1"),
            new(0, 0, "col0"),
            new(0, -2, "col_neg2")
        };

        var viewModel = new FlexibleGridItemSelectorViewModel(items);

        var row = viewModel.ItemGroups[0];
        Assert.Multiple(() =>
        {
            Assert.Equal(4, row.Items.Count);
            Assert.Equal("col_neg2", row.Items[0].ResourceKey);
            Assert.Equal("col_neg1", row.Items[1].ResourceKey);
            Assert.Equal("col0", row.Items[2].ResourceKey);
            Assert.Equal("col2", row.Items[3].ResourceKey);
        });
    }

    #endregion
}
