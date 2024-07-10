using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;

namespace TrackOMatic
{
    public class SavedHint
    {
        public string HintPanelKey { get; }
        public string LocationText { get; set; }
        public string PotionCountText { get; set; }
        public Dictionary<ItemName, bool> PathItems { get; set; }
        public Dictionary<ItemName, bool> FoundItems { get; set; }

        public SavedHint(string hintPanelKey, string locationText, string potionCountText, Dictionary<ItemName, bool> pathItems, Dictionary<ItemName, bool> foundItems)
        {
            HintPanelKey = hintPanelKey;
            LocationText = locationText;
            PotionCountText = potionCountText;
            PathItems = pathItems;
            FoundItems = foundItems;
        }
    }
}
