using System;
using System.Collections.Generic;
using System.Text;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models;

/// <summary>
/// Represents a data transfer object (DTO) for saved hints in the game.
/// This class is used for serialization and deserialization of saved hint data.
/// </summary>
internal class SavedHintDto
{
    /// <summary>
    /// Gets or sets the key of the hint panel associated with this saved hint.
    /// </summary>
    /// <remarks>This is the old property to be replaced with <see cref="HintPanelType"/> on 3.0+ saves.</remarks>
    public string? HintPanelKey { get; set; }

    /// <summary>
    /// Gets or sets the type of the hint panel associated with this saved hint.
    /// </summary>
    public HintPanelType? HintPanelType { get; set; }

    /// <summary>
    /// Gets the unique identifier of the hint.
    /// </summary>
    public string? HintId { get; init; }

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

    public SavedHint ToSavedHint()
    {
        var panelType = HintPanelType ?? ConvertFromLegacyKey(HintPanelKey);
        var hintId = GenerateHintIdIfNeeded(HintId);

        return new(panelType, LocationText, PotionCountText, PathItems, FoundItems, hintId);
    }

    /// <summary>
    /// Generates a HintId if the provided ID is empty or null, using the first 8 characters of a random GUID.
    /// </summary>
    /// <param name="hintId">The hint ID to check and potentially replace.</param>
    /// <returns>The provided hint ID if non-empty, or a newly generated 8-character ID from a GUID.</returns>
    internal static string GenerateHintIdIfNeeded(string? hintId)
    {
        if (string.IsNullOrEmpty(hintId))
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        return hintId;
    }

    private static HintPanelType ConvertFromLegacyKey(string? key)
    {
        return key switch
        {
            "IslesPanel" => Enums.HintPanelType.Isles,
            "AztecPanel" => Enums.HintPanelType.Aztec,
            "GalleonPanel" => Enums.HintPanelType.Galleon,
            "CavesPanel" => Enums.HintPanelType.Caves,
            "JapesPanel" => Enums.HintPanelType.Japes,
            "FactoryPanel" => Enums.HintPanelType.Factory,
            "ForestPanel" => Enums.HintPanelType.Forest,
            "CastlePanel" => Enums.HintPanelType.Castle,
            "HelmPanel" => Enums.HintPanelType.Helm,
            "PathsPanel" => Enums.HintPanelType.Paths,
            "FoolishPanel" => Enums.HintPanelType.Foolish,
            "KongsPanel" => Enums.HintPanelType.Kongs,
            "WotHPanel" => Enums.HintPanelType.WayOfTheHoard,
            "PotionCountsPanel" => Enums.HintPanelType.PotionCounts,
            "UnhintedPanel" => Enums.HintPanelType.Unhinted,
            _ => throw new ArgumentException($"Invalid hint panel key: {key}"),
        };
    }
}
