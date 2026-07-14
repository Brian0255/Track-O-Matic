using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models.Autotracking;

public record OffsetInfoEntry(ItemName ItemName, uint Offset, int TotalBits, int Bitmask = 0, bool UsesCountStruct = false);
