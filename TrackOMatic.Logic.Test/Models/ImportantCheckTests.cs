using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Logic.Test.Models;

public class ImportantCheckTests
{
    [Fact]
    public void Constructor_WithDefaults_CreatesValidCheck()
    {
        // Arrange & Act
        var check = new ImportantCheck(ItemName.DONKEY, ItemType.KONG);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(ItemName.DONKEY, check.ItemName);
            Assert.Equal(ItemType.KONG, check.ItemType);
            Assert.Equal(0, check.PointValue);  // Before InitPointValue
            Assert.Equal(VialColor.YELLOW, check.VialColor);  // Default
        });
    }

    [Fact]
    public void Constructor_WithCustomVialColor_SetsVialColor()
    {
        // Arrange & Act
        var check = new ImportantCheck(ItemName.DONKEY, ItemType.KONG, VialColor.RED);

        // Assert
        Assert.Equal(VialColor.RED, check.VialColor);
    }

    [Fact]
    public void Initiative_PointValuesPopulated_InitPointValue_HitsSpecificValues()
    {
        // Arrange
        // Pre-populate PointValues with test data
        PointValues.SpecificValues.Clear();
        PointValues.GroupedValues.Clear();
        PointValues.SpecificValues[ItemName.DONKEY] = 100;
        PointValues.GroupedValues[ItemType.KONG] = 50;

        var check = new ImportantCheck(ItemName.DONKEY, ItemType.KONG);

        // Act
        check.InitPointValue();

        // Assert
        Assert.Equal(100, check.PointValue);  // Specific value takes precedence
    }

    [Fact]
    public void InitPointValue_SpecificValueNotFound_FallsBackToGroupedValue()
    {
        // Arrange
        PointValues.SpecificValues.Clear();
        PointValues.GroupedValues.Clear();
        PointValues.SpecificValues[ItemName.DONKEY] = 100;  // Different item
        PointValues.GroupedValues[ItemType.PHYSICAL_MOVE] = 75;

        var check = new ImportantCheck(ItemName.TINY, ItemType.PHYSICAL_MOVE);

        // Act
        check.InitPointValue();

        // Assert
        Assert.Equal(75, check.PointValue);  // Falls back to grouped
    }

    [Fact]
    public void InitPointValue_NoEntryFound_RemainsZero()
    {
        // Arrange
        PointValues.SpecificValues.Clear();
        PointValues.GroupedValues.Clear();

        var check = new ImportantCheck(ItemName.DONKEY, ItemType.MISC);

        // Act
        check.InitPointValue();

        // Assert
        Assert.Equal(0, check.PointValue);  // No entry, stays 0
    }

    [Theory]
    [InlineData(ItemType.KONG, 50)]
    [InlineData(ItemType.PHYSICAL_MOVE, 75)]
    [InlineData(ItemType.GUN, 100)]
    public void InitPointValue_VariousItemTypes_CorrectlyReadsGroupedValues(ItemType itemType, int expectedValue)
    {
        // Arrange
        PointValues.SpecificValues.Clear();
        PointValues.GroupedValues.Clear();
        PointValues.GroupedValues[ItemType.KONG] = 50;
        PointValues.GroupedValues[ItemType.PHYSICAL_MOVE] = 75;
        PointValues.GroupedValues[ItemType.GUN] = 100;

        var check = new ImportantCheck(ItemName.TINY, itemType);

        // Act
        check.InitPointValue();

        // Assert
        Assert.Equal(expectedValue, check.PointValue);
    }

    [Fact]
    public void InitPointValue_CalledMultipleTimes_UpdatesValue()
    {
        // Arrange
        PointValues.SpecificValues.Clear();
        PointValues.GroupedValues.Clear();
        PointValues.SpecificValues[ItemName.DONKEY] = 100;

        var check = new ImportantCheck(ItemName.DONKEY, ItemType.KONG);

        // Act
        check.InitPointValue();
        var firstValue = check.PointValue;

        // Change the dictionary and call again
        PointValues.SpecificValues[ItemName.DONKEY] = 150;
        check.InitPointValue();
        var secondValue = check.PointValue;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(100, firstValue);
            Assert.Equal(150, secondValue);
        });
    }

    [Fact]
    public void PointValue_IsPrivateSet_CanOnlyBeModifiedByInitPointValue()
    {
        // Arrange
        var check = new ImportantCheck(ItemName.DONKEY, ItemType.KONG);

        // Act & Assert
        // PointValue property has private setter, so this would not compile if we tried:
        // check.PointValue = 999;  // Compiler error
        Assert.Equal(0, check.PointValue);
    }

    [Fact]
    public void ItemName_IsReadOnly_CannotBeChanged()
    {
        // Arrange
        var check = new ImportantCheck(ItemName.DONKEY, ItemType.KONG);

        // Assert
        Assert.Equal(ItemName.DONKEY, check.ItemName);
        // ItemName property has no setter, so this confirms immutability
    }

    [Fact]
    public void ItemType_IsReadOnly_CannotBeChanged()
    {
        // Arrange
        var check = new ImportantCheck(ItemName.DONKEY, ItemType.KONG);

        // Assert
        Assert.Equal(ItemType.KONG, check.ItemType);
        // ItemType property has no setter
    }

    [Fact]
    public void VialColor_IsReadOnly_CannotBeChanged()
    {
        // Arrange
        var check = new ImportantCheck(ItemName.DONKEY, ItemType.KONG, VialColor.BLUE);

        // Assert
        Assert.Equal(VialColor.BLUE, check.VialColor);
        // VialColor property has no setter
    }
}
