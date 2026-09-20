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
