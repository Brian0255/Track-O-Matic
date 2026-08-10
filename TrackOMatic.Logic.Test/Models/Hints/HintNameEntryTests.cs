using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.Logic.Test.Models.Hints;

public class HintNameEntryTests
{
    [Fact]
    public void Constructor_WithFullNameOnly_SetsDefaults()
    {
        // Arrange & Act
        var entry = new HintNameEntry("Test Hint");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal("Test Hint", entry.FullName);
            Assert.Equal("Test Hint", entry.ShortName);  // Defaults to FullName
            Assert.Equal(HintGroup.NONE, entry.HintGroup);  // Default
        });
    }

    [Fact]
    public void Constructor_WithCustomShortName_UsesProvidedValue()
    {
        // Arrange & Act
        var entry = new HintNameEntry("Full Name Here", "Short");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal("Full Name Here", entry.FullName);
            Assert.Equal("Short", entry.ShortName);
            Assert.Equal(HintGroup.NONE, entry.HintGroup);
        });
    }

    [Fact]
    public void Constructor_WithEmptyShortName_DefaultsToFullName()
    {
        // Arrange & Act
        var entry = new HintNameEntry("Full Name", "");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal("Full Name", entry.FullName);
            Assert.Equal("Full Name", entry.ShortName);  // Falls back to FullName
        });
    }

    [Fact]
    public void Constructor_WithExplicitHintGroup_SetsHintGroup()
    {
        // Arrange & Act
        var entry = new HintNameEntry("Region Hint", "Region", HintGroup.REGION_ISLES);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal("Region Hint", entry.FullName);
            Assert.Equal("Region", entry.ShortName);
            Assert.Equal(HintGroup.REGION_ISLES, entry.HintGroup);
        });
    }

    [Theory]
    [InlineData("Jungle Japes Enemy", HintGroup.ENEMY)]
    [InlineData("Enemy in Castle", HintGroup.ENEMY)]
    [InlineData("Enemy", HintGroup.ENEMY)]
    public void Constructor_WithEnemyInName_AutoAssignsEnemyHintGroup(string fullName, HintGroup expected)
    {
        // Arrange & Act
        var entry = new HintNameEntry(fullName);

        // Assert
        Assert.Equal(expected, entry.HintGroup);
    }

    [Theory]
    [InlineData("Jungle Japes Medal")]
    [InlineData("Blueprint Reward")]
    [InlineData("enemy spelled wrong")]
    [InlineData("Regular Hint")]
    public void Constructor_WithoutEnemyInName_KeepsSpecifiedHintGroup(string fullName)
    {
        // Arrange & Act
        var entry = new HintNameEntry(fullName, "Short", HintGroup.REGION_JAPES);

        // Assert
        Assert.Equal(HintGroup.REGION_JAPES, entry.HintGroup);  // Should keep original
    }

    [Fact]
    public void Constructor_EnemyDetection_CaseSensitive_FullwordMatch()
    {
        // Arrange & Act
        var entry = new HintNameEntry("Contains Enemy Here", "", HintGroup.NONE);

        // Assert
        // "Enemy" appears in the full name, should be detected
        Assert.Equal(HintGroup.ENEMY, entry.HintGroup);
    }

    [Fact]
    public void Constructor_WithNullShortName_DefaultsToFullName()
    {
        // Arrange & Act
        var entry = new HintNameEntry("Full Name", null ?? "");

        // Assert
        Assert.Equal("Full Name", entry.ShortName);
    }

    [Fact]
    public void Constructor_AllParametersProvided_SetsAllProperties()
    {
        // Arrange & Act
        var entry = new HintNameEntry("Complete Hint", "Short", HintGroup.REGION_AZTEC);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal("Complete Hint", entry.FullName);
            Assert.Equal("Short", entry.ShortName);
            Assert.Equal(HintGroup.REGION_AZTEC, entry.HintGroup);
        });
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_WithWhitespaceShortName_UsesAsProvidedNotFullName(string shortName)
    {
        // Arrange & Act
        var entry = new HintNameEntry("Full Name", shortName);

        // Assert
        // Whitespace is not IsNullOrEmpty, so it should be used
        Assert.Equal(shortName, entry.ShortName);
    }

    [Fact]
    public void Constructor_WithEmptyStringShortName_DefaultsToFullName()
    {
        // Arrange & Act - empty string is treated as null/empty by IsNullOrEmpty
        var entry = new HintNameEntry("Full Name", "");

        // Assert
        Assert.Equal("Full Name", entry.ShortName);
    }

    [Theory]
    [InlineData("Donkey Kong Enemy Boss")]
    [InlineData("King Karel The Enemy")]
    [InlineData("Enemy of Humanity")]
    public void Constructor_EnemyKeywordVariations_AllGetEnemyHintGroup(string fullName)
    {
        // Arrange & Act
        var entry = new HintNameEntry(fullName);

        // Assert
        Assert.Equal(HintGroup.ENEMY, entry.HintGroup);
    }

    [Fact]
    public void Constructor_EnemyCaseInsensitive_LowercaseEnemyNotDetected()
    {
        // Arrange & Act - "enemy" is lowercase, Contains is case-sensitive
        var entry = new HintNameEntry("Contains enemy lowercase");

        // Assert - should remain NONE since it's lowercase
        Assert.Equal(HintGroup.NONE, entry.HintGroup);
    }

    [Fact]
    public void Constructor_EnemyAllCaps_UppercaseEnemyNotDetected()
    {
        // Arrange & Act - "ENEMY" is uppercase, Contains looks for "Enemy" exactly
        var entry = new HintNameEntry("ENEMY");

        // Assert - should remain NONE since casing doesn't match
        Assert.Equal(HintGroup.NONE, entry.HintGroup);
    }

    [Fact]
    public void Record_TwoInstancesWithSameValues_AreEqual()
    {
        // Arrange
        var entry1 = new HintNameEntry("Test", "T", HintGroup.REGION_ISLES);
        var entry2 = new HintNameEntry("Test", "T", HintGroup.REGION_ISLES);

        // Act & Assert
        Assert.Equal(entry1, entry2);
    }

    [Fact]
    public void Record_TwoInstancesWithDifferentValues_AreNotEqual()
    {
        // Arrange
        var entry1 = new HintNameEntry("Test", "T", HintGroup.REGION_ISLES);
        var entry2 = new HintNameEntry("Test", "T", HintGroup.REGION_JAPES);

        // Act & Assert
        Assert.NotEqual(entry1, entry2);
    }
}
