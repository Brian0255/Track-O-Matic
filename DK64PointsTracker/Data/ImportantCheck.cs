namespace DK64PointsTracker
{
    public class ImportantCheck
    {
        public ItemName ItemName { get; }
        public ItemType ItemType { get; }
        public int PointValue { get; }
        public VialColor VialColor { get; }
        public ImportantCheck(ItemName itemName, ItemType itemType, VialColor vialColor = VialColor.YELLOW)
        {
            ItemName = itemName;
            ItemType = itemType;
            PointValue = 0;
            VialColor = vialColor;
            if (PointValues.SpecificValues.ContainsKey(itemName))
            {
                PointValue = PointValues.SpecificValues[itemName];
                return;
            }
            if (PointValues.GroupedValues.ContainsKey(itemType))
            {
                PointValue = PointValues.GroupedValues[itemType];
                return;
            }
        }
    }

}
