using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;

namespace TrackOMatic
{
    public record GameVerificationInfo(uint TargetAddress, int TotalBits, uint TargetValue);
    public record AttachedProcessInfo(Process Process, ulong StartAddress);
    public record OffsetInfoEntry(ItemName ItemName, uint Offset, int TotalBits, int Bitmask = 0, bool UsesCountStruct = false);
    public record HintTypeSettings(
        Visibility PathItemsVisible, 
        Visibility FoundItemVisible, 
        HintSuggestion HintSuggestion, 
        bool PromptForFoundItem = false, 
        Visibility PotionCountVisibility = Visibility.Collapsed, 
        Visibility HintSorterVisibility = Visibility.Collapsed);
    public record HintShortcutInfo(string JSONShortcutsKey, List<string> DefaultSortedList);
}
