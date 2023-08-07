namespace TrackOMatic
{
    public class ImportantCheck
    {
        public ItemName ItemName { get; }
        public ItemType ItemType { get; }
        public int PointValue { get; private set; }
        public VialColor VialColor { get; }
        public ImportantCheck(ItemName itemName, ItemType itemType, VialColor vialColor = VialColor.YELLOW)
        {
            ItemName = itemName;
            ItemType = itemType;
            PointValue = 0;
            VialColor = vialColor;
        }

        public void InitPointValue()
        {
            if (PointValues.SpecificValues.ContainsKey(ItemName))
            {
                PointValue = PointValues.SpecificValues[ItemName];
                return;
            }
            if (PointValues.GroupedValues.ContainsKey(ItemType))
            {
                PointValue = PointValues.GroupedValues[ItemType];
                return;
            }
        }
    }
}
