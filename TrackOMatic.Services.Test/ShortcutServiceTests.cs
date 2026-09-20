using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Logic.Services;

namespace TrackOMatic.Services.Test;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

public class ShortcutServiceTests
{
    #region Test Fixture & Data

    private static ShortcutService CreateService(bool includeEnemiesInAutofill = false)
    {
        var mockUserSettings = new Mock<IUserSettingsService>();
        mockUserSettings.Setup(x => x.EnemiesInAutofill).Returns(includeEnemiesInAutofill);

        // Create mock provider with test shortcuts (no HintData.Init() required)
        var mockHintDataProvider = new Mock<IHintDataProvider>();

        // Test data: predictable shortcuts for deterministic testing
        var testShortcuts = new Dictionary<string, Dictionary<string, string>>
        {
            ["Hint Regions"] = new()
            {
                ["K"] = "Kremling Collective",
                ["Kong"] = "Donkey Kong",
                ["Japes"] = "Jungle Japes",
                ["Crystal"] = "Crystal Caves"
            },
            ["Locations"] = new()
            {
                ["Cavern"] = "Cavern Entrance",
                ["Factory"] = "Frantic Factory"
            }
        };

        mockHintDataProvider.Setup(x => x.UserShortcuts)
            .Returns(testShortcuts.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyDictionary<string, string>)kvp.Value
            ));

        // Test data for direct item hints (items accessible in each playable region)
        var testRegionItems = new Dictionary<RegionName, IReadOnlyList<string>>
        {
            [RegionName.JUNGLE_JAPES] =
            [
                "Barrel Blast",
                "Coconut Cluster",
                "Enemy Kong",
                "Key Barrel"
            ],
            [RegionName.FRANTIC_FACTORY] =
            [
                "Blueprint Piece",
                "Conveyor Belt",
                "Gear",
                "Machine Part"
            ]
        };

        mockHintDataProvider.Setup(x => x.RegionsWithoutLevelName)
            .Returns(testRegionItems as IReadOnlyDictionary<RegionName, IReadOnlyList<string>>);

        return new ShortcutService(mockUserSettings.Object, mockHintDataProvider.Object);
    }

    private static HintSuggestionConfig CreateTestConfig(
        List<string>? jsonKeys = null,
        List<HintNameEntry>? defaultSuggestions = null)
    {
        return new HintSuggestionConfig(
            jsonKeys ?? [],
            defaultSuggestions ?? []
        );
    }

    /// <summary>
    /// Creates a minimal test config with a few sample suggestions.
    /// </summary>
    private static HintSuggestionConfig CreateConfigWithSuggestions()
    {
        var suggestions = new List<HintNameEntry>
        {
            new("Donkey Kong Country", "DKC", HintGroup.NONE),
            new("Enemy Boss", "Boss", HintGroup.ENEMY),
            new("Jungle Japes", "Japes", HintGroup.REGION_JAPES),
            new("Kremling Collective", "Kremlings", HintGroup.ENEMY),
        };
        return new HintSuggestionConfig([], suggestions);
    }

    #endregion

    #region Initialization Tests

    [Fact]
    public void Constructor_WithUserSettingsService_CreatesServiceSuccessfully()
    {
        // Act
        var service = CreateService();

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void FindMatches_WithNullLocationText_ReturnsAllSuggestions()
    {
        // Arrange
        var service = CreateService();
        var config = CreateConfigWithSuggestions();

        // Act - empty string matches all items (substring match)
        var result = service.FindMatches(string.Empty, HintPanelType.Foolish, config).ToList();

        // Assert - all non-enemy suggestions returned when enemies disabled
        Assert.Equal(2, result.Count);
        Assert.Contains("DKC", result);
        Assert.Contains("Japes", result);
    }

    #endregion

    #region DefaultSuggestions Tests

    [Fact]
    public void FindMatches_WithDefaultSuggestions_MatchesSubstringInFullName()
    {
        // Arrange
        var service = CreateService();
        var config = CreateConfigWithSuggestions();

        // Act
        var result = service.FindMatches("Kong", HintPanelType.Foolish, config).ToList();

        // Assert
        Assert.Single(result);
        Assert.Contains("DKC", result); // Matches "Donkey Kong Country"
    }

    [Fact]
    public void FindMatches_WithDefaultSuggestions_MatchesSubstringInShortName()
    {
        // Arrange
        var service = CreateService();
        var config = CreateConfigWithSuggestions();

        // Act
        var result = service.FindMatches("apes", HintPanelType.Foolish, config).ToList();

        // Assert
        Assert.Single(result);
        Assert.Contains("Japes", result); // Matches "Japes" (short name)
    }

    [Fact]
    public void FindMatches_WithDefaultSuggestions_IsCaseInsensitive()
    {
        // Arrange
        var service = CreateService();
        var config = CreateConfigWithSuggestions();

        // Act - try different cases
        var resultLower = service.FindMatches("dkc", HintPanelType.Foolish, config).ToList();
        var resultUpper = service.FindMatches("DKC", HintPanelType.Foolish, config).ToList();
        var resultMixed = service.FindMatches("DoNkEy", HintPanelType.Foolish, config).ToList();

        // Assert
        Assert.Single(resultLower);
        Assert.Single(resultUpper);
        Assert.Single(resultMixed);
    }

    [Fact]
    public void FindMatches_WithEnemyInSuggestions_IncludesEnemyWhenSettingEnabled()
    {
        // Arrange
        var service = CreateService(includeEnemiesInAutofill: true);
        var config = CreateConfigWithSuggestions();

        // Act
        var result = service.FindMatches("Boss", HintPanelType.Foolish, config).ToList();

        // Assert
        Assert.Single(result);
        Assert.Contains("Boss", result);
    }

    [Fact]
    public void FindMatches_WithEnemyInSuggestions_ExcludesEnemyWhenSettingDisabled()
    {
        // Arrange
        var service = CreateService(includeEnemiesInAutofill: false);
        var config = CreateConfigWithSuggestions();

        // Act
        var result = service.FindMatches("Boss", HintPanelType.Foolish, config).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindMatches_WithEnemyInSuggestions_ExcludesAllEnemiesWhenSettingDisabled()
    {
        // Arrange
        var service = CreateService(includeEnemiesInAutofill: false);
        var config = CreateConfigWithSuggestions();

        // Act
        var result = service.FindMatches("rem", HintPanelType.Foolish, config).ToList();

        // Assert
        Assert.Empty(result); // "Kremling" matches but should be excluded (ENEMY)
    }

    [Fact]
    public void FindMatches_WithEnemyAndNonEnemySuggestions_MixesResultsCorrectly()
    {
        // Arrange
        var service = CreateService(includeEnemiesInAutofill: true);
        var config = CreateConfigWithSuggestions();

        // Act
        var result = service.FindMatches("o", HintPanelType.Foolish, config).ToList();

        // Assert - should include "DKC" (Kong has 'o'), "Boss" (has 'o'), and "Kremlings" (has 'o')
        Assert.Equal(3, result.Count);
        Assert.Contains("DKC", result);
        Assert.Contains("Boss", result);
        Assert.Contains("Kremlings", result);
    }

    #endregion

    #region Deduplication & Sorting Tests

    [Fact]
    public void FindMatches_WithDuplicates_DeduplicatesResults()
    {
        // Arrange
        var service = CreateService();
        var suggestions = new List<HintNameEntry>
        {
            new("Apple", "App", HintGroup.NONE),
            new("Application", "App", HintGroup.NONE), // Same short name
            new("Apricot", "Apr", HintGroup.NONE),
        };
        var config = new HintSuggestionConfig([], suggestions);

        // Act
        var result = service.FindMatches("Ap", HintPanelType.Foolish, config).ToList();

        // Assert
        Assert.Equal(2, result.Count); // "App" should appear once, "Apr" once
        Assert.Contains("App", result);
        Assert.Contains("Apr", result);
    }

    [Fact]
    public void FindMatches_WithMultipleMatches_ReturnsSortedResults()
    {
        // Arrange
        var service = CreateService();
        var suggestions = new List<HintNameEntry>
        {
            new("Zebra", "Z", HintGroup.NONE),
            new("Apple", "A", HintGroup.NONE),
            new("Mango", "M", HintGroup.NONE),
        };
        var config = new HintSuggestionConfig([], suggestions);

        // Act
        var result = service.FindMatches("", HintPanelType.Foolish, config).ToList();

        // Assert - should be sorted alphabetically by short name
        Assert.Equal(["A", "M", "Z"], result);
    }

    [Fact]
    public void FindMatches_EmptyLocationText_WithConfig_ReturnsAllSuggestions()
    {
        // Arrange
        var service = CreateService();
        var suggestions = new List<HintNameEntry>
        {
            new("Apple", "App", HintGroup.NONE),
            new("Banana", "Ban", HintGroup.NONE),
        };
        var config = new HintSuggestionConfig([], suggestions);

        // Act
        var result = service.FindMatches("", HintPanelType.Foolish, config).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("App", result);
        Assert.Contains("Ban", result);
    }

    #endregion

    #region NullConfig Tests

    [Fact]
    public void FindMatches_WithNullConfig_ReturnsEmptyList()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.FindMatches("test", HintPanelType.Foolish, null);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindMatches_WithNullConfigAndPlayableRegion_ReturnsDirectItemMatches()
    {
        // Arrange
        var service = CreateService();
        // HintPanelType.Japes maps to RegionName.JUNGLE_JAPES (playable region)
        var config = new HintSuggestionConfig([], []);

        // Act
        var result = service.FindMatches("", HintPanelType.Japes, config);

        // Assert
        // Should return direct items from Jungle Japes region, sorted
        var resultList = result.ToList();
        Assert.Equal(
            new[] { "Barrel Blast", "Coconut Cluster", "Enemy Kong", "Key Barrel" },
            resultList
        );
    }

    [Fact]
    public void FindMatches_WithPlayableRegionAndFilter_ReturnsFilteredDirectItems()
    {
        // Arrange
        var service = CreateService();
        // HintPanelType.Factory maps to RegionName.FRANTIC_FACTORY (playable region)
        var config = new HintSuggestionConfig([], []);

        // Act
        // Filter for items containing "Blueprint"
        var result = service.FindMatches("Blue", HintPanelType.Factory, config);

        // Assert
        // Should find items matching substring
        var resultList = result.ToList();
        Assert.Equal(new[] { "Blueprint Piece" }, resultList);
    }

    [Fact]
    public void FindMatches_WithPlayableRegion_IsCaseInsensitive()
    {
        // Arrange
        var service = CreateService();
        var config = new HintSuggestionConfig([], []);

        // Act
        var lowerResult = service.FindMatches("barrel", HintPanelType.Japes, config);
        var upperResult = service.FindMatches("BARREL", HintPanelType.Japes, config);

        // Assert
        Assert.Equal(lowerResult, upperResult);
    }

    #endregion

    #region UserShortcuts Tests

    [Fact]
    public void FindMatches_WithRegionAndShortcutsConfig_SearchesByJSONKey()
    {
        // Arrange
        var service = CreateService();
        var config = new HintSuggestionConfig(["Hint Regions"], []);

        // Act
        var result = service.FindMatches("", HintPanelType.Foolish, config);

        // Assert
        // Should return all items from "Hint Regions" JSON key, sorted
        var resultList = result.ToList();
        Assert.Equal(
            new[] { "Crystal Caves", "Donkey Kong", "Jungle Japes", "Kremling Collective" },
            resultList
        );
    }

    [Fact]
    public void FindMatches_WithRegionAndShortcutsConfig_FiltersByPrefixMatch()
    {
        // Arrange
        var service = CreateService();
        var config = new HintSuggestionConfig(["Hint Regions"], []);

        // Act
        var result = service.FindMatches("K", HintPanelType.Foolish, config);

        // Assert
        // Should match keys starting with "K": "K" and "Kong"
        var resultList = result.ToList();
        Assert.Equal(
            new[] { "Donkey Kong", "Kremling Collective" },
            resultList
        );
    }

    [Fact]
    public void FindMatches_WithRegionAndShortcutsConfig_IsCaseInsensitive()
    {
        // Arrange
        var service = CreateService();
        var config = new HintSuggestionConfig(["Hint Regions"], []);

        // Act
        var lowerResult = service.FindMatches("k", HintPanelType.Foolish, config);
        var upperResult = service.FindMatches("K", HintPanelType.Foolish, config);

        // Assert
        Assert.Equal(lowerResult, upperResult);
    }

    [Fact]
    public void FindMatches_WithMultipleJSONKeys_CombinesResults()
    {
        // Arrange
        var service = CreateService();
        var config = new HintSuggestionConfig(["Hint Regions", "Locations"], []);

        // Act
        var result = service.FindMatches("", HintPanelType.Foolish, config);

        // Assert
        // Should combine results from both JSON keys, sorted
        var resultList = result.ToList();
        Assert.Equal(
            new[]
            {
                "Cavern Entrance",
                "Crystal Caves",
                "Donkey Kong",
                "Frantic Factory",
                "Jungle Japes",
                "Kremling Collective"
            },
            resultList
        );
    }

    [Fact]
    public void FindMatches_WithNonexistentJSONKey_ReturnsEmpty()
    {
        // Arrange
        var service = CreateService();
        var config = new HintSuggestionConfig(["NonexistentKey_XYZ"], []);

        // Act
        var result = service.FindMatches("", HintPanelType.Foolish, config);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindMatches_WithMultipleJSONKeysAndFilter_CombinesFilteredResults()
    {
        // Arrange
        var service = CreateService();
        var config = new HintSuggestionConfig(["Hint Regions", "Locations"], []);

        // Act
        var result = service.FindMatches("C", HintPanelType.Foolish, config);

        // Assert
        // Keys starting with "C": "Crystal" from Hint Regions, "Cavern" from Locations
        var resultList = result.ToList();
        Assert.Equal(
            new[] { "Cavern Entrance", "Crystal Caves" },
            resultList
        );
    }

    [Fact]
    public void FindMatches_WithShortcutsAndDefaultSuggestions_CombinesResults()
    {
        // Arrange
        var service = CreateService();
        var suggestions = new List<HintNameEntry>
        {
            new("King Kong", "KK", HintGroup.NONE),
            new("Donkey", "Donkey", HintGroup.NONE)
        };
        var config = new HintSuggestionConfig(["Hint Regions"], suggestions);

        // Act
        var result = service.FindMatches("", HintPanelType.Foolish, config);

        // Assert
        // Should combine:
        // - Shortcuts from "Hint Regions": Crystal Caves, Donkey Kong, Jungle Japes, Kremling Collective
        // - Default suggestions: returns ShortName of matching entries
        //   Both "King Kong" (FullName) and "Donkey" (ShortName) match empty substring
        //   So we get "KK" (shortName of "King Kong") and "Donkey" (shortName of "Donkey")
        var resultList = result.ToList();
        Assert.Equal(
            new[] { "Crystal Caves", "Donkey", "Donkey Kong", "Jungle Japes", "KK", "Kremling Collective" },
            resultList
        );
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void FindMatches_WithNoMatchingText_ReturnsEmptyList()
    {
        // Arrange
        var service = CreateService();
        var config = CreateConfigWithSuggestions();

        // Act
        var result = service.FindMatches("xyz123", HintPanelType.Foolish, config);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindMatches_WithEmptySuggestionsList_ReturnsEmptyList()
    {
        // Arrange
        var service = CreateService();
        var config = CreateTestConfig([], []);

        // Act
        var result = service.FindMatches("test", HintPanelType.Foolish, config);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindMatches_WithSpecialCharactersInLocationText_HandlesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var suggestions = new List<HintNameEntry>
        {
            new("C++ Programming", "C++", HintGroup.NONE),
            new("C# Code", "C#", HintGroup.NONE),
        };
        var config = new HintSuggestionConfig([], suggestions);

        // Act
        var result = service.FindMatches("C+", HintPanelType.Foolish, config).ToList();

        // Assert
        Assert.Single(result);
        Assert.Contains("C++", result);
    }

    [Fact]
    public void FindMatches_WithWhitespaceInLocationText_MatchesCorrectly()
    {
        // Arrange
        var service = CreateService();
        var suggestions = new List<HintNameEntry>
        {
            new("Donkey Kong Country", "DKC", HintGroup.NONE),
            new("Donkey Kong", "DK", HintGroup.NONE),
        };
        var config = new HintSuggestionConfig([], suggestions);

        // Act
        var result = service.FindMatches("Kong C", HintPanelType.Foolish, config).ToList();

        // Assert
        Assert.Single(result);
        Assert.Contains("DKC", result);
    }

    #endregion
}
#pragma warning restore CA1861 // Avoid constant arrays as arguments
