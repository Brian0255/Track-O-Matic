namespace TrackOMatic.Logic.Models.Hints;

/// <summary>
/// A configuration record that holds suggestions for hinted locations via dropdown system.
/// </summary>
/// <param name="JSONShortcutKeys"></param>
/// <param name="DefaultSuggestions"></param>
public record HintSuggestionConfig(List<string> JSONShortcutKeys, IReadOnlyList<HintNameEntry> DefaultSuggestions);
