using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace TrackOMatic
{
    public static class HintTypeSettingsList
    {
        public static readonly Dictionary<HintType, HintTypeSettings> SETTINGS = new(){
            {HintType.PATH, new HintTypeSettings(Visibility.Visible, Visibility.Visible, HintSuggestion.LOCATION, false, Visibility.Collapsed, Visibility.Visible) },
            {HintType.KONGS, new HintTypeSettings(Visibility.Hidden, Visibility.Visible, HintSuggestion.NONE, true) },
            {HintType.WAY_OF_THE_HOARD, new HintTypeSettings(Visibility.Hidden, Visibility.Visible, HintSuggestion.CHECK) },
            {HintType.REGION_POTION_COUNT, new HintTypeSettings(Visibility.Hidden, Visibility.Hidden, HintSuggestion.LOCATION, false, Visibility.Visible) },
            {HintType.FOOLISH_REGION, new HintTypeSettings(Visibility.Hidden, Visibility.Hidden, HintSuggestion.LOCATION) },
            {HintType.PATHLESS_MOVE, new HintTypeSettings(Visibility.Hidden, Visibility.Hidden, HintSuggestion.MOVE) },
            {HintType.UNHINTED, new HintTypeSettings(Visibility.Hidden, Visibility.Visible, HintSuggestion.CHECK, true) },
            {HintType.MISC, new HintTypeSettings(Visibility.Hidden, Visibility.Hidden, HintSuggestion.NONE) },
            {HintType.DIRECT_ITEM_HINT, new HintTypeSettings(Visibility.Hidden, Visibility.Visible, HintSuggestion.DIRECT_ITEM_HINT, true, Visibility.Collapsed) },
            {HintType.ADVANCED_ITEM_HINT, new HintTypeSettings(Visibility.Hidden, Visibility.Visible, HintSuggestion.LOCATION, false) }
       };
    }
}
