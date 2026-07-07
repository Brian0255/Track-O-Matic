using TrackOMatic.Logic.Enums;

namespace TrackOMatic.AutoTracking;

public record GameVerificationInfo(uint TargetAddress, int TotalBits, uint TargetValue);

public record OffsetInfoEntry(ItemName ItemName, uint Offset, int TotalBits, int Bitmask = 0, bool UsesCountStruct = false);

