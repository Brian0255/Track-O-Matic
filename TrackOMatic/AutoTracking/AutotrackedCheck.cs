namespace TrackOMatic
{
    public class AutotrackedCheck
    {
        public ItemName ItemName { get; }
        public uint Offset { get; }
        public int Bitmask { get; }
        public int TotalBits { get; }
        public bool UsesCountStruct { get; }
        public bool Tracked { get; }
        public AutotrackedCheck(ItemName itemName, uint offset, int totalBits, int bitmask, bool usesCountStruct)
        {
            ItemName = itemName;
            Offset = offset;
            TotalBits = totalBits;
            Bitmask = bitmask;
            Tracked = false;
            UsesCountStruct = usesCountStruct;
        }
    }

}
