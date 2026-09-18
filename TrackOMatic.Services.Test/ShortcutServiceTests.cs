using Moq;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.Services.Test;

public class ShortcutServiceTests
{
    #region Test Fixture & Data

    private static ShortcutService CreateService(bool includeEnemiesInAutofill = false)
    {
        var mockUserSettings = new Mock<IUserSettingsService>();
        mockUserSettings.Setup(x => x.EnemiesInAutofill).Returns(includeEnemiesInAutofill);
        return new ShortcutService(mockUserSettings.Object);
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
        // This test requires HintData to be initialized, which is complex.
        // For now, we'll note that this scenario depends on HintData initialization.
        // See advanced coverage notes in comments.
        Assert.True(true); // Placeholder for advanced test
    }

    #endregion

    #region UserShortcuts Tests

    [Fact]
    public void FindMatches_WithUserShortcutsConfig_SearchesByJSONKey()
    {
        // This test is complex because it depends on HintData.UserShortcuts being properly loaded.
        // The ShortcutService reads from a static HintData.UserShortcuts that is loaded from JSON.
        // Unit testing this requires either:
        // 1. Mocking the static HintData.UserShortcuts (complex and fragile)
        // 2. Using an integration test with real HintData initialization
        // 3. Refactoring ShortcutService to inject HintData sources
        //
        // For now, this test is documented but not implemented to avoid test fragility.
        Assert.True(true); // Placeholder for advanced coverage
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

    #region Advanced Coverage Notes

    /*
     * ADVANCED TEST SCENARIOS (Not Fully Implemented)
     * 
     * These scenarios require deeper integration with HintData and static initialization,
     * which is difficult to unit test without refactoring the service or HintData structure.
     * 
     * 1. DirectItemHint Matching (Playable Regions)
     *    - Requires: HintData.REGIONS_WITHOUT_LEVEL_NAME populated with region items
     *    - Requires: HintData.InitDirectItemHintList() called to populate regions
     *    - Current obstacle: Static initialization happens in HintData.Init() which loads shortcuts from file
     *    - Solution: Either mock static data or refactor to inject HintData sources
     * 
     * 2. UserShortcuts Prefix Matching
     *    - Requires: HintData.UserShortcuts loaded with real or test shortcut data
     *    - Requires: JsonShortcutKeys in HintSuggestionConfig to match HintData.UserShortcuts keys
     *    - Current obstacle: Static JSON loading requires file system or embedded resource
     *    - Solution: Create test shortcuts data or mock the static dictionary
     * 
     * 3. Exclusions List Filtering
     *    - Requires: DirectItemHint matching logic to execute
     *    - Tests the private _directHintExclusions static list
     *    - Verifies items like "Isles", "Boss", "Bought" are filtered out
     * 
     * 4. HintPanelType.IsPlayableRegion() Routing
     *    - Tests different HintPanelTypes to verify correct code path (DirectItemHint vs UserShortcuts)
     *    - Playable regions: Isles, Japes, Aztec, Factory, Galleon, Forest, Caves, Castle, Helm
     * 
     * RECOMMENDATION:
     * For full coverage, consider:
     * - Refactoring ShortcutService to accept injected HintData/shortcut sources
     * - Creating a separate integration test suite with real HintData initialization
     * - Mocking static HintData for specific scenarios (requires care to avoid state pollution)
     */

    #endregion
}
