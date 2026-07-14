using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models;

public class ImportantCheck(ItemName itemName, ItemType itemType, VialColor vialColor = VialColor.YELLOW)
{
    public ItemName ItemName { get; } = itemName;
    public ItemType ItemType { get; } = itemType;
    public int PointValue { get; private set; } = 0;
    public VialColor VialColor { get; } = vialColor;

    public void InitPointValue()
    {
        if (PointValues.SpecificValues.TryGetValue(ItemName, out int nameValue))
        {
            PointValue = nameValue;
            return;
        }
        if (PointValues.GroupedValues.TryGetValue(ItemType, out int typeValue))
        {
            PointValue = typeValue;
            return;
        }
    }
}
