using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Helpers;

public record HintTypeSettings(
    bool PathItemsVisible,
    bool FoundItemVisible,
    HintSuggestion HintSuggestion,
    bool PromptForFoundItem = false,
    bool PotionCountVisibility = false,
    bool HintSorterVisibility = false);

public static class HintTypeSettingsList
{
    public static readonly Dictionary<HintType, HintTypeSettings> SETTINGS = new(){
        {HintType.PATH, new HintTypeSettings(true, true, HintSuggestion.LOCATION, false, false, true) },
        {HintType.KONGS, new HintTypeSettings(false, true, HintSuggestion.NONE, true) },
        {HintType.WAY_OF_THE_HOARD, new HintTypeSettings(false, true, HintSuggestion.CHECK) },
        {HintType.REGION_POTION_COUNT, new HintTypeSettings(false, false, HintSuggestion.LOCATION, false, true) },
        {HintType.FOOLISH_REGION, new HintTypeSettings(false, false, HintSuggestion.LOCATION) },
        {HintType.PATHLESS_MOVE, new HintTypeSettings(false, false, HintSuggestion.MOVE) },
        {HintType.UNHINTED, new HintTypeSettings(false, true, HintSuggestion.CHECK, true) },
        {HintType.MISC, new HintTypeSettings(false, false, HintSuggestion.NONE) },
        {HintType.DIRECT_ITEM_HINT, new HintTypeSettings(false, true, HintSuggestion.DIRECT_ITEM_HINT, true) },
        {HintType.ADVANCED_ITEM_HINT, new HintTypeSettings(false, true, HintSuggestion.LOCATION, false) }
    };
}
