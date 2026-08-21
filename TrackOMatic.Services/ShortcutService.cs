using System.Globalization;

using TrackOMatic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.Services;

/// <summary>
/// Default implementation of IShortcutService.
/// Searches HintData.UserShortcuts and default hint suggestions for matches.
/// </summary>
public class ShortcutService : IShortcutService
{
    private readonly IUserSettingsService _userSettingsService;

    /// <summary>
    /// Items to exclude from DirectItemHint suggestions.
    /// </summary>
    private static readonly List<string> DirectHintExclusions = new()
    {
        "Isles", "Japes", "Aztec", "Factory", "Galleon", "Forest", "Caves", "Castle", "Helm", "Boss", "Bought"
    };

    public ShortcutService(IUserSettingsService userSettingsService)
    {
        _userSettingsService = userSettingsService;
    }

    public IEnumerable<string> FindMatches(
        string locationText,
        HintType hintType,
        HintSuggestionConfig? config,
        RegionName regionName)
    {
        var matches = new List<string>();

        // Get shortcut matches (from UserShortcuts JSON)
        matches.AddRange(FindShortcutMatches(locationText, hintType, config?.JSONShortcutKeys, regionName));

        // Get default suggestion matches (from HintSuggestionConfig)
        if (config != null)
        {
            matches.AddRange(FindDefaultSuggestionMatches(locationText, config.DefaultSuggestions));
        }

        // Deduplicate and sort
        return matches.Distinct().OrderBy(x => x).ToList();
    }

    /// <summary>
    /// Find matches from HintData.UserShortcuts and DirectItemHint region items.
    /// </summary>
    private IEnumerable<string> FindShortcutMatches(
        string locationText,
        HintType hintType,
        List<string>? jsonKeys,
        RegionName regionName)
    {
        var result = new List<string>();

        // DirectItemHint: Use substring match on region items
        if (hintType == HintType.DIRECT_ITEM_HINT)
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
                if (shortcutKey.ToLower().StartsWith(locationText.ToLower()))
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
    private IEnumerable<string> FindDirectItemMatches(string locationText, RegionName regionName)
    {
        if (!HintData.REGIONS_WITHOUT_LEVEL_NAME.ContainsKey(regionName))
        {
            return new List<string>();
        }

        var regionItems = HintData.REGIONS_WITHOUT_LEVEL_NAME[regionName];

        return regionItems
            .Where(item =>
                item.ToLower().Contains(locationText.ToLower())
                && !DirectHintExclusions.Contains(item))
            .ToList();
    }

    /// <summary>
    /// Find default suggestions by searching FullName and ShortName fields.
    /// Respects user settings for including/excluding enemies.
    /// </summary>
    private IEnumerable<string> FindDefaultSuggestionMatches(
        string locationText,
        IReadOnlyList<HintNameEntry> suggestions)
    {
        var input = locationText.ToLower();
        var includeEnemies = _userSettingsService.EnemiesInAutofill;

        return suggestions
            .Where(loc =>
                (loc.FullName.ToLower().Contains(input) || loc.ShortName.ToLower().Contains(input))
                && (includeEnemies || loc.HintGroup != HintGroup.ENEMY))
            .Select(loc => loc.ShortName)
            .ToList();
    }
}
