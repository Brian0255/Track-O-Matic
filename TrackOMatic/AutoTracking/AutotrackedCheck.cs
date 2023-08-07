namespace TrackOMatic
{
    public class AutotrackedCheck
    {
        public ItemName ItemName { get; }
        public uint Offset { get; }
        public int TargetValue { get; }
        public int Bitmask { get; }
        public int TotalBits { get; }
        public bool Tracked { get; }
        public AutotrackedCheck(ItemName itemName, uint offset, int totalBits, int targetValue, int bitmask)
        {
            ItemName = itemName;
            Offset = offset;
            TotalBits = totalBits;
            TargetValue = targetValue;
            Bitmask = bitmask;
            Tracked = false;
        }
    }

}
