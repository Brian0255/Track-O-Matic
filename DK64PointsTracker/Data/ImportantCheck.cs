namespace DK64PointsTracker
{
    public class ImportantCheck
    {
        public ItemName ItemName { get; }
        public ItemType ItemType { get; }
        public int PointValue { get; }
        public ImportantCheck(ItemName itemName, ItemType itemType)
        {
            ItemName = itemName;
            ItemType = itemType;
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
            PointValue = 0;
        }
    }

}
