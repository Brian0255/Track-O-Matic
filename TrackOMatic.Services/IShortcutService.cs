using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.Services;

/// <summary>
/// Service for finding hint suggestion shortcuts and filtered items based on location text.
/// Handles both built-in hint suggestions and direct item hint filters.
/// </summary>
public interface IShortcutService
{
    /// <summary>
    /// Finds shortcuts and default suggestions that match the given location text.
    /// </summary>
    /// <param name="locationText">The user-entered location text to filter on</param>
    /// <param name="hintPanelType">The type of hint being edited</param>
    /// <param name="config">Hint suggestion configuration (null for DirectItemHint)</param>
    /// <returns>Enumerable of matching suggestion strings, deduplicated and sorted</returns>
    IEnumerable<string> FindMatches(
        string locationText,
        HintPanelType hintPanelType,
        HintSuggestionConfig? config
    );
}
