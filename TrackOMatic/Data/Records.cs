using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;


namespace TrackOMatic
{
    public record GameVerificationInfo(uint TargetAddress, int TotalBits, uint TargetValue);
    public record AttachedProcessInfo(Process Process, IntPtr Handle, ulong StartAddress);
    public record OffsetInfoEntry(ItemName ItemName, uint Offset, int TotalBits, int Bitmask = 0, bool UsesCountStruct = false);
    public record HintTypeSettings(
        Visibility PathItemsVisible, 
        Visibility FoundItemVisible, 
        HintSuggestion HintSuggestion, 
        bool PromptForFoundItem = false, 
        Visibility PotionCountVisibility = Visibility.Collapsed, 
        Visibility HintSorterVisibility = Visibility.Collapsed);
    public record HintSuggestionConfig(List<string> JSONShortcutKeys, IReadOnlyList<HintNameEntry> DefaultSuggestions);
    public record HintNameEntry
    {
        public string FullName { get; init; }
        public string ShortName { get; init; }
        public HintGroup HintGroup { get; init; }
        public HintNameEntry(string fullName, string shortName = "", HintGroup hintGroup = HintGroup.NONE)
        {
            FullName = fullName;
            ShortName = string.IsNullOrEmpty(shortName) ? fullName : shortName;
            HintGroup = hintGroup;
            //should be replaced but this works ok for now
            if (FullName.Contains("Enemy"))
            {
                HintGroup = HintGroup.ENEMY;
            }
        }
    }
    public record BLockerInfo(int item, int cost);
}
