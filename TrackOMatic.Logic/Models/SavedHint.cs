using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models;

public class SavedHint(string hintPanelKey, string locationText, string potionCountText, Dictionary<ItemName, bool> pathItems, Dictionary<ItemName, bool> foundItems)
{
    public string HintPanelKey { get; } = hintPanelKey;
    public string LocationText { get; set; } = locationText;
    public string PotionCountText { get; set; } = potionCountText;
    public Dictionary<ItemName, bool> PathItems { get; set; } = pathItems;
    public Dictionary<ItemName, bool> FoundItems { get; set; } = foundItems;
}
