using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models.Autotracking;

public class AutotrackedCheck(ItemName itemName, uint offset, int totalBits, int bitmask, bool usesCountStruct)
{
    public ItemName ItemName { get; } = itemName;
    public uint Offset { get; } = offset;
    public int Bitmask { get; } = bitmask;
    public int TotalBits { get; } = totalBits;
    public bool UsesCountStruct { get; } = usesCountStruct;
    public bool Tracked { get; } = false;
}
