using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models.Hints;

/// <summary>
/// A static class that contains a dictionary of hint type settings for each hint type.
/// </summary>
public static class HintTypeSettingsList
{
    public static readonly Dictionary<HintType, HintTypeSettings> SETTINGS = new()
    {
        {HintType.PATH, new HintTypeSettings(true, true, HintSuggestion.REGION_OR_MOVE, false, false, true) },
        {HintType.KONGS, new HintTypeSettings(false, true, HintSuggestion.NONE, true) },
        {HintType.WAY_OF_THE_HOARD, new HintTypeSettings(false, true, HintSuggestion.CHECK) },
        {HintType.REGION_POTION_COUNT, new HintTypeSettings(false, false, HintSuggestion.REGION, false, true) },
        {HintType.FOOLISH_REGION, new HintTypeSettings(false, false, HintSuggestion.REGION) },
        {HintType.PATHLESS_MOVE, new HintTypeSettings(false, false, HintSuggestion.MOVE) },
        {HintType.UNHINTED, new HintTypeSettings(false, true, HintSuggestion.CHECK, true) },
        {HintType.MISC, new HintTypeSettings(false, false, HintSuggestion.NONE) },
        {HintType.DIRECT_ITEM_HINT, new HintTypeSettings(false, true, HintSuggestion.DIRECT_ITEM_HINT, true, true) },
        {HintType.ADVANCED_ITEM_HINT, new HintTypeSettings(false, true, HintSuggestion.REGION, false) }
    };
}
