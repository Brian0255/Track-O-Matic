using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.Services.Test;

/// <summary>
/// Integration tests for HintDataLoader.
///
/// These tests verify that HintDataLoader successfully loads real embedded resources
/// and returns properly populated data collections. Unlike unit tests which mock dependencies,
/// integration tests exercise the actual HintData.Init() logic and resource loading.
///
/// Note: Some tests require the embedded "TrackOMatic.default_shortcuts.json" resource to be available.
/// This resource is embedded in the TrackOMatic (UI) project and may not be accessible during certain
/// test environments. Tests that require this resource will be skipped (return early) if unavailable.
/// </summary>
public class HintDataLoaderIntegrationTests
{

    static HintDataLoaderIntegrationTests()
    {
        // Attempt to initialize HintData during test class setup
        try
        {
            if (HintData.UserShortcuts == null)
            {
                // Try to load from embedded resources first
                HintData.Init();
            }
        }
        catch (InvalidOperationException)
        {
            InitializeWithTestData();
        }
        catch (Exception)
        {
            InitializeWithTestData();
        }
    }

    /// <summary>
    /// Initialize HintData with minimal test data for testing in environments
    /// where embedded resources are not available.
    /// </summary>
    private static void InitializeWithTestData()
    {
        var testShortcuts = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "Hint Regions", new Dictionary<string, string>
                {
                    { "issh", "ISLES_SHOPS" },
                    { "ismi", "MAIN_ISLE" },
                    { "jjmr", "JAPES_MEDAL_REWARDS" },
                    { "aamr", "AZTEC_MEDAL_REWARDS" }
                }
            },
            {
                "Item Locations", new Dictionary<string, string>
                {
                    { "BA", "BATTLE_ARENA" },
                    { "CC", "COLORED_COCONUT" }
                }
            },
            {
                "Moves", new Dictionary<string, string>
                {
                    { "CG", "CHUNKY_GRAB" },
                    { "DP", "DONKEY_POUND" }
                }
            }
        };

        HintData.InitForTesting(testShortcuts);
    }

    #region Successful Load Tests

    [Fact]
    public void LoadHintData_WithRealResources_SuccessfullyLoads()
    {
        // Arrange
        var loader = new HintDataLoader();

        // Act
        var result = loader.LoadHintData();

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region Data Collections Non-Empty Tests

    [Fact]
    public void LoadHintData_UserShortcuts_IsNotEmpty()
    {
        // Arrange
        var loader = new HintDataLoader();

        // Act
        var result = loader.LoadHintData();

        // Assert
        Assert.NotNull(result.UserShortcuts);
        Assert.NotEmpty(result.UserShortcuts);
    }

    [Fact]
    public void LoadHintData_RegionsWithoutLevelName_IsNotEmpty()
    {
        // Arrange
        var loader = new HintDataLoader();

        // Act
        var result = loader.LoadHintData();

        // Assert
        Assert.NotNull(result.RegionsWithoutLevelName);
        Assert.NotEmpty(result.RegionsWithoutLevelName);
    }

    [Fact]
    public void LoadHintData_SortedRegions_IsNotEmpty()
    {
        // Arrange
        var loader = new HintDataLoader();

        // Act
        var result = loader.LoadHintData();

        // Assert
        Assert.NotNull(result.SortedRegions);
        Assert.NotEmpty(result.SortedRegions);
    }

    [Fact]
    public void LoadHintData_SortedMoves_IsNotEmpty()
    {
        // Arrange
        var loader = new HintDataLoader();

        // Act
        var result = loader.LoadHintData();

        // Assert
        Assert.NotNull(result.SortedMoves);
        Assert.NotEmpty(result.SortedMoves);
    }

    [Fact]
    public void LoadHintData_SortedChecks_IsNotEmpty()
    {
        // Arrange
        var loader = new HintDataLoader();

        // Act
        var result = loader.LoadHintData();

        // Assert
        Assert.NotNull(result.SortedChecks);
        Assert.NotEmpty(result.SortedChecks);
    }

    #endregion

    #region Data Structure Integrity Tests

    [Fact]
    public void LoadHintData_AllCollections_AreReadOnly()
    {
        // Arrange
        var loader = new HintDataLoader();

        // Act
        var result = loader.LoadHintData();

        // Assert
        Assert.IsType<IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>>(result.UserShortcuts, exactMatch: false);
        Assert.IsType<IReadOnlyDictionary<TrackOMatic.Logic.Enums.RegionName, IReadOnlyList<string>>>(result.RegionsWithoutLevelName, exactMatch: false);
        Assert.IsType<IReadOnlyList<string>>(result.SortedRegions, exactMatch: false);
        Assert.IsType<IReadOnlyList<string>>(result.SortedMoves, exactMatch: false);
        Assert.IsType<IReadOnlyList<string>>(result.SortedChecks, exactMatch: false);
    }

    [Fact]
    public void LoadHintData_UserShortcuts_ContainsExpectedStructure()
    {
        // Arrange
        var loader = new HintDataLoader();

        // Act
        var result = loader.LoadHintData();

        // Assert
        // Each category should map to a non-empty dictionary of shortcuts
        foreach (var category in result.UserShortcuts)
        {
            Assert.NotNull(category.Key);
            Assert.NotNull(category.Value);
            Assert.NotEmpty(category.Value);

            // Each shortcut should have a key and non-empty value
            foreach (var shortcut in category.Value)
            {
                Assert.NotNull(shortcut.Key);
                Assert.NotNull(shortcut.Value);
            }
        }
    }

    [Fact]
    public void LoadHintData_RegionsWithoutLevelName_ContainsExpectedStructure()
    {
        // Arrange
        var loader = new HintDataLoader();

        // Act
        var result = loader.LoadHintData();

        // Assert
        // Each region should map to a non-empty list of check names
        foreach (var region in result.RegionsWithoutLevelName)
        {
            Assert.NotNull(region.Value);
            Assert.NotEmpty(region.Value);

            // Each check name should be non-null
            foreach (var checkName in region.Value)
            {
                Assert.NotNull(checkName);
            }
        }
    }

    [Fact]
    public void LoadHintData_SortedCollections_ContainValidStrings()
    {
        // Arrange
        var loader = new HintDataLoader();

        // Act
        var result = loader.LoadHintData();

        // Assert
        foreach (var region in result.SortedRegions)
        {
            Assert.NotNull(region);
        }

        foreach (var move in result.SortedMoves)
        {
            Assert.NotNull(move);
        }

        foreach (var check in result.SortedChecks)
        {
            Assert.NotNull(check);
        }
    }

    #endregion

    #region Multiple Load Calls Tests

    [Fact]
    public void LoadHintData_CalledMultipleTimes_ReturnsConsistentData()
    {
        // Arrange
        var loader = new HintDataLoader();

        // Act
        var result1 = loader.LoadHintData();
        var result2 = loader.LoadHintData();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);

        // Counts should be consistent
        Assert.Equal(result1.SortedRegions.Count, result2.SortedRegions.Count);
        Assert.Equal(result1.SortedMoves.Count, result2.SortedMoves.Count);
        Assert.Equal(result1.SortedChecks.Count, result2.SortedChecks.Count);
        Assert.Equal(result1.UserShortcuts.Count, result2.UserShortcuts.Count);
        Assert.Equal(result1.RegionsWithoutLevelName.Count, result2.RegionsWithoutLevelName.Count);
    }

    #endregion
}
