using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;

namespace TrackOMatic
{
    public record GameVerificationInfo(uint TargetAddress, int TotalBits, uint TargetValue);
    public record AttachedProcessInfo(Process Process, uint StartAddress);
    public record OffsetInfoEntry(ItemName ItemName, uint Offset, int TotalBits, int Bitmask = -1, int TargetValue = 0);
    public record HintTypeSettings(Visibility PathItemsVisible, Visibility FoundItemVisible, HintSuggestion HintSuggestion);
}
