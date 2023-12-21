using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace TrackOMatic
{
    public static class HintTypeSettingsList
    {
        public static readonly Dictionary<HintType, HintTypeSettings> SETTINGS = new(){
            {HintType.PATH, new HintTypeSettings(Visibility.Visible, Visibility.Visible, HintSuggestion.LOCATION) },
            {HintType.KONGS, new HintTypeSettings(Visibility.Hidden, Visibility.Hidden, HintSuggestion.NONE) },
            {HintType.WAY_OF_THE_HOARD, new HintTypeSettings(Visibility.Hidden, Visibility.Visible, HintSuggestion.NONE) },
            {HintType.REGION_POTION_COUNT, new HintTypeSettings(Visibility.Hidden, Visibility.Hidden, HintSuggestion.LOCATION) },
            {HintType.FOOLISH_REGION, new HintTypeSettings(Visibility.Hidden, Visibility.Hidden, HintSuggestion.LOCATION) },
            {HintType.PATHLESS_ITEM, new HintTypeSettings(Visibility.Hidden, Visibility.Hidden, HintSuggestion.MOVE) },
            {HintType.UNHINTED, new HintTypeSettings(Visibility.Hidden, Visibility.Visible, HintSuggestion.LOCATION) },
            {HintType.MISC, new HintTypeSettings(Visibility.Hidden, Visibility.Hidden, HintSuggestion.NONE) }
       };
    }
}
