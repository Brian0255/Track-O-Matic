using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.Services;

/// <summary>
/// Default implementation of IShortcutService.
/// Searches HintData.UserShortcuts and default hint suggestions for matches.
/// </summary>
public class ShortcutService(IUserSettingsService userSettingsService) : IShortcutService
{
    private readonly IUserSettingsService _userSettingsService = userSettingsService;

    /// <summary>
    /// Items to exclude from DirectItemHint suggestions.
    /// </summary>
    private static readonly List<string> _directHintExclusions =
    [
        "Isles", "Japes", "Aztec", "Factory", "Galleon", "Forest", "Caves", "Castle", "Helm", "Boss", "Bought"
    ];

    public IEnumerable<string> FindMatches(
        string locationText,
        HintPanelType hintPanelType,
        HintSuggestionConfig? config
    )
    {
        var matches = new List<string>();

        // Get shortcut matches (from UserShortcuts JSON)
        matches.AddRange(FindShortcutMatches(locationText, hintPanelType, config?.JSONShortcutKeys));

        // Get default suggestion matches (from HintSuggestionConfig)
        if (config != null)
        {
            matches.AddRange(FindDefaultSuggestionMatches(locationText, config.DefaultSuggestions));
        }

        // Deduplicate and sort
        return [.. matches.Distinct().OrderBy(x => x)];
    }

    /// <summary>
    /// Find matches from HintData.UserShortcuts and DirectItemHint region items.
    /// </summary>
    private static List<string> FindShortcutMatches(
        string locationText,
        HintPanelType hintPanelType,
        List<string>? jsonKeys
    )
    {
        var result = new List<string>();
        var regionName = hintPanelType.ToRegion();

        if (regionName.IsPlayableRegion())
        {
            result.AddRange(FindDirectItemMatches(locationText, regionName));
            return result;
        }

        // Other hint types: Search UserShortcuts with prefix match on keys
        if (jsonKeys == null || jsonKeys.Count == 0)
        {
            return result;
        }

        foreach (var jsonKey in jsonKeys)
        {
            if (!HintData.UserShortcuts.ContainsKey(jsonKey))
            {
                continue;
            }

            foreach (var entry in HintData.UserShortcuts[jsonKey])
            {
                var shortcutKey = entry.Key;
                var shortcutValue = entry.Value;

                // Prefix match on the shortcut key
                if (shortcutKey.StartsWith(locationText, StringComparison.CurrentCultureIgnoreCase))
                {
                    result.Add(shortcutValue);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Find DirectItemHint suggestions by region using substring match on item names.
    /// Excludes region names and other non-item entries.
    /// </summary>
    private static IEnumerable<string> FindDirectItemMatches(string locationText, RegionName regionName)
    {
        if (!HintData.REGIONS_WITHOUT_LEVEL_NAME.TryGetValue(regionName, out List<string>? regionItems))
        {
            return [];
        }

        return [.. regionItems
            .Where(item =>
                item.Contains(locationText, StringComparison.CurrentCultureIgnoreCase) &&
                !_directHintExclusions.Contains(item)
            )
        ];
    }

    /// <summary>
    /// Find default suggestions by searching FullName and ShortName fields.
    /// Respects user settings for including/excluding enemies.
    /// </summary>
    private List<string> FindDefaultSuggestionMatches(
        string locationText,
        IReadOnlyList<HintNameEntry> suggestions)
    {
        var input = locationText.ToLower();
        var includeEnemies = _userSettingsService.EnemiesInAutofill;

        return [.. suggestions
            .Where(loc =>
                (
                    loc.FullName.Contains(input, StringComparison.CurrentCultureIgnoreCase) ||
                    loc.ShortName.Contains(input, StringComparison.CurrentCultureIgnoreCase)
                ) && (includeEnemies || loc.HintGroup != HintGroup.ENEMY))
            .Select(loc => loc.ShortName)];
    }
}
