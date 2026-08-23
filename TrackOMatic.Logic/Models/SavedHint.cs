using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models;

/// <summary>
/// Represents a saved hint with its associated data, including the hint panel key, location text, potion count text, path items, found items, and a hint ID.
/// </summary>
public class SavedHint
{
    /// <summary>
    /// Gets the ID of the hint. This is meant to be auto-generated if it doesn't exist.
    /// If not provided or empty, a 8-character ID will be generated from a random GUID during initialization.
    /// </summary>
    public HintId HintId { get; init; }

    /// <summary>
    /// Gets the type of the hint panel associated with this saved hint.
    /// </summary>
    public HintPanelType HintPanelType { get; init; } = HintPanelType.Unhinted;

    /// <summary>
    /// Gets or sets the location within the region/panel where the hint is relevant.
    /// </summary>
    /// <remarks>This is a free form text field, though drop downs can help guide users to a more specific location.</remarks>
    public string LocationText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of potions found within the specific region/panel.
    /// </summary>
    /// <remarks>
    /// This is a free form text field, but should ideally only have numbers.
    /// This field is only relevant for the "Potion Count" hint panel, and should be ignored for other panels.
    /// </remarks>
    public string PotionCountText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the dictionary of path items and their checked status.
    /// </summary>
    /// <remarks>This field is only relevant in the "Path Items" hint panel, and should be ignored for other panels.</remarks>
    public Dictionary<ItemName, bool> PathItems { get; set; } = [];

    /// <summary>
    /// Gets or sets the dictionary of found items and their checked status.
    /// </summary>
    /// <remarks>This field is only unused in the Foolish and Potion Counts panels.</remarks>
    public Dictionary<ItemName, bool> FoundItems { get; set; } = [];

    /// <summary>
    /// Parameterless constructor for deserialization. All properties can be initialized via object initializers.
    /// </summary>
    public SavedHint()
    {
        // This constructor is used by JSON deserialization and allows all properties to be set via initializers.
    }

    /// <summary>
    /// Initializes a new SavedHint with the specified parameters.
    /// </summary>
    /// <param name="hintPanelType">The type of the hint panel (init-only, cannot be changed after initialization).</param>
    /// <param name="locationText">The text describing the location of the hint.</param>
    /// <param name="potionCountText">The text describing the potion count.</param>
    /// <param name="pathItems">A dictionary of path items and their found status.</param>
    /// <param name="foundItems">A dictionary of found items and their found status.</param>
    /// <param name="hintId">The ID of the hint. If empty, will be auto-generated from a random GUID.</param>
    public SavedHint(HintPanelType hintPanelType, string locationText, string potionCountText, Dictionary<ItemName, bool> pathItems, Dictionary<ItemName, bool> foundItems, string hintId = "")
    {
        HintPanelType = hintPanelType;
        LocationText = locationText;
        PotionCountText = potionCountText;
        PathItems = pathItems;
        FoundItems = foundItems;
        HintId = new HintId(SavedHintDto.GenerateHintIdIfNeeded(hintId));
    }

    public static SavedHint CreateFromPanel(HintPanelType hintPanelType) => new(hintPanelType, "", "", [], [], SavedHintDto.GenerateHintIdIfNeeded(""));
}
