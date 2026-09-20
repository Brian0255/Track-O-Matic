using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Services;

namespace TrackOMatic.Services.Test;

/// <summary>
/// Unit tests for HintDataProvider.
///
/// HintDataProvider is now a simple data container that accepts pre-built data via constructor.
/// Tests pass explicit test data, avoiding any dependency on resource loading or initialization.
/// </summary>
public class HintDataProviderTests
{
    #region Constructor & Initialization

    [Fact]
    public void Constructor_WithTestData_CreatesInstanceSuccessfully()
    {
        // Arrange
        var testShortcuts = new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            ["Test Key"] = new Dictionary<string, string> { ["Test"] = "Value" } as IReadOnlyDictionary<string, string>
        };

        // Act
        var provider = new HintDataProvider(
            testShortcuts as IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>,
            new Dictionary<RegionName, IReadOnlyList<string>>() as IReadOnlyDictionary<RegionName, IReadOnlyList<string>>,
            new List<string>() as IReadOnlyList<string>,
            new List<string>() as IReadOnlyList<string>,
            new List<string>() as IReadOnlyList<string>
        );

        // Assert
        Assert.NotNull(provider);
        Assert.IsAssignableFrom<IHintDataProvider>(provider);
    }

    #endregion

    #region Property Access

    [Fact]
    public void UserShortcuts_ReturnsProvidedData()
    {
        // Arrange
        var testShortcuts = new Dictionary<string, IReadOnlyDictionary<string, string>>
        {
            ["Regions"] = new Dictionary<string, string>
            {
                ["K"] = "Kremling Kong",
                ["J"] = "Jungle"
            } as IReadOnlyDictionary<string, string>
        };
        var provider = new HintDataProvider(
            testShortcuts as IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>,
            new Dictionary<RegionName, IReadOnlyList<string>>() as IReadOnlyDictionary<RegionName, IReadOnlyList<string>>,
            new List<string>() as IReadOnlyList<string>,
            new List<string>() as IReadOnlyList<string>,
            new List<string>() as IReadOnlyList<string>
        );

        // Act
        var result = provider.UserShortcuts;

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal("Kremling Kong", result["Regions"]["K"]);
    }

    [Fact]
    public void RegionsWithoutLevelName_ReturnsProvidedData()
    {
        // Arrange
        var testRegions = new Dictionary<RegionName, IReadOnlyList<string>>
        {
            [RegionName.JUNGLE_JAPES] = new List<string> { "Barrel", "Chest" } as IReadOnlyList<string>
        };
        var provider = new HintDataProvider(
            new Dictionary<string, IReadOnlyDictionary<string, string>>() as IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>,
            testRegions as IReadOnlyDictionary<RegionName, IReadOnlyList<string>>,
            new List<string>() as IReadOnlyList<string>,
            new List<string>() as IReadOnlyList<string>,
            new List<string>() as IReadOnlyList<string>
        );

        // Act
        var result = provider.RegionsWithoutLevelName;

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Barrel", result[RegionName.JUNGLE_JAPES]);
    }

    [Fact]
    public void SortedRegions_ReturnsProvidedData()
    {
        // Arrange
        var sortedRegions = new List<string> { "Aztec", "Factory", "Forest" } as IReadOnlyList<string>;
        var provider = new HintDataProvider(
            new Dictionary<string, IReadOnlyDictionary<string, string>>() as IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>,
            new Dictionary<RegionName, IReadOnlyList<string>>() as IReadOnlyDictionary<RegionName, IReadOnlyList<string>>,
            sortedRegions,
            new List<string>() as IReadOnlyList<string>,
            new List<string>() as IReadOnlyList<string>
        );

        // Act
        var result = provider.SortedRegions;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Aztec", result[0]);
    }

    [Fact]
    public void SortedMoves_ReturnsProvidedData()
    {
        // Arrange
        var sortedMoves = new List<string> { "Charge", "Jump", "Roll" } as IReadOnlyList<string>;
        var provider = new HintDataProvider(
            new Dictionary<string, IReadOnlyDictionary<string, string>>() as IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>,
            new Dictionary<RegionName, IReadOnlyList<string>>() as IReadOnlyDictionary<RegionName, IReadOnlyList<string>>,
            new List<string>() as IReadOnlyList<string>,
            sortedMoves,
            new List<string>() as IReadOnlyList<string>
        );

        // Act
        var result = provider.SortedMoves;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Roll", result[2]);
    }

    [Fact]
    public void SortedChecks_ReturnsProvidedData()
    {
        // Arrange
        var sortedChecks = new List<string> { "Bunch Coins", "Green Banana", "Hive" } as IReadOnlyList<string>;
        var provider = new HintDataProvider(
            new Dictionary<string, IReadOnlyDictionary<string, string>>() as IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>,
            new Dictionary<RegionName, IReadOnlyList<string>>() as IReadOnlyDictionary<RegionName, IReadOnlyList<string>>,
            new List<string>() as IReadOnlyList<string>,
            new List<string>() as IReadOnlyList<string>,
            sortedChecks
        );

        // Act
        var result = provider.SortedChecks;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    #endregion

    #region Read-Only Contract

    [Fact]
    public void UserShortcuts_IsReadOnlyDictionary()
    {
        // Arrange
        var testShortcuts = new Dictionary<string, IReadOnlyDictionary<string, string>>() as IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>;
        var provider = new HintDataProvider(testShortcuts, new Dictionary<RegionName, IReadOnlyList<string>>() as IReadOnlyDictionary<RegionName, IReadOnlyList<string>>, new List<string>() as IReadOnlyList<string>, new List<string>() as IReadOnlyList<string>, new List<string>() as IReadOnlyList<string>);

        // Act
        var result = provider.UserShortcuts;

        // Assert
        Assert.IsAssignableFrom<IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>>(result);
    }

    [Fact]
    public void SortedRegions_IsReadOnlyList()
    {
        // Arrange
        var sortedRegions = new List<string>() as IReadOnlyList<string>;
        var provider = new HintDataProvider(
            new Dictionary<string, IReadOnlyDictionary<string, string>>() as IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>,
            new Dictionary<RegionName, IReadOnlyList<string>>() as IReadOnlyDictionary<RegionName, IReadOnlyList<string>>,
            sortedRegions,
            new List<string>() as IReadOnlyList<string>,
            new List<string>() as IReadOnlyList<string>
        );

        // Act
        var result = provider.SortedRegions;

        // Assert
        Assert.IsAssignableFrom<IReadOnlyList<string>>(result);
    }

    #endregion

    #region Null Handling

    [Fact]
    public void Constructor_WithNullCollections_AcceptsNulls()
    {
        // Arrange - intentionally pass nulls
        // Act
        var provider = new HintDataProvider(null!, null!, null!, null!, null!);

        // Assert
        Assert.NotNull(provider);
        Assert.Null(provider.UserShortcuts);
        Assert.Null(provider.RegionsWithoutLevelName);
        Assert.Null(provider.SortedRegions);
        Assert.Null(provider.SortedMoves);
        Assert.Null(provider.SortedChecks);
    }

    #endregion
}
