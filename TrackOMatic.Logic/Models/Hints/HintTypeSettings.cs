using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models.Hints;

/// <summary>
/// A record representing the settings for a specific hint type as used in the UI.
/// </summary>
/// <param name="PathItemsVisible">A flag indicating whether the collection of path items are visible.</param>
/// <param name="FoundItemVisible">A flag indicating whether the collection of found items are visible.</param>
/// <param name="HintSuggestion">The type of hint suggestion.</param>
/// <param name="PromptForFoundItem">A flag indicating whether to prompt the user to place an item after first entering the hint.</param>
/// <param name="PotionCountVisibility">A flag indicating whether to provide a field for entering a potion count.</param>
/// <param name="HintSorterVisibility">A flag indicating whether to provide a button to sort hints.</param>
public record HintTypeSettings(
    bool PathItemsVisible,
    bool FoundItemVisible,
    HintSuggestion HintSuggestion,
    bool PromptForFoundItem = false,
    bool PotionCountVisibility = false,
    bool HintSorterVisibility = false
);
