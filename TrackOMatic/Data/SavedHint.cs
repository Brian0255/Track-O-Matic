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
        public List<ItemName> PathItems { get; set; }
        public List<ItemName> FoundItems { get; set; }

        public SavedHint(string hintPanelKey, string locationText, string potionCountText, List<ItemName> pathItems, List<ItemName> foundItems)
        {
            HintPanelKey = hintPanelKey;
            LocationText = locationText;
            PotionCountText = potionCountText;
            PathItems = pathItems;
            FoundItems = foundItems;
        }
    }
}
