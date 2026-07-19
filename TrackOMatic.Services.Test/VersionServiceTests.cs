namespace TrackOMatic.Services.Test;

public class VersionServiceTests
{
    private readonly VersionService _sut = new();

    [Fact]
    public void GetApplicationVersion_ReturnsValidVersion()
    {
        // Arrange & Act
        var version = _sut.GetApplicationVersion();

        Assert.Multiple(() =>
        {
            // Assert
            Assert.NotNull(version);
            Assert.True(version.Major >= 0, "Major version should be non-negative");
        });
    }

    [Fact]
    public void GetApplicationVersion_ComponentsAreNonNegative()
    {
        // Arrange & Act
        var version = _sut.GetApplicationVersion();

        Assert.Multiple(() =>
        {
            // Assert
            Assert.True(version.Major >= 0, "Major version should be non-negative");
            Assert.True(version.Minor >= 0, "Minor version should be non-negative");
            Assert.True(version.Build >= 0, "Build version should be non-negative");
            Assert.True(version.Revision >= 0, "Revision should be non-negative");
        });
    }

    [Fact]
    public void GetVersionString_ReturnsFormattedString()
    {
        // Arrange & Act
        var versionString = _sut.GetVersionString();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.NotNull(versionString);
            Assert.NotEmpty(versionString);
            Assert.Matches(@"^\d+\.\d+\.\d+$", versionString); // Matches Major.Minor.Build format
        });
    }

    [Fact]
    public void GetVersionString_FormatsAsExpected()
    {
        // Arrange & Act
        var version = _sut.GetApplicationVersion();
        var versionString = _sut.GetVersionString();
        var expectedFormat = $"{version.Major}.{version.Minor}.{version.Build}";

        // Assert
        Assert.Equal(expectedFormat, versionString);
    }

    [Fact]
    public void GetVersionString_DoesNotIncludeRevision()
    {
        // Arrange & Act
        var version = _sut.GetApplicationVersion();
        var versionString = _sut.GetVersionString();

        // Assert
        // Should have exactly 2 dots for Major.Minor.Build format
        var dotCount = versionString.Count(c => c == '.');
        Assert.Equal(2, dotCount);
    }

    [Fact]
    public void GetApplicationVersion_ConsistentBetweenCalls()
    {
        // Arrange & Act
        var version1 = _sut.GetApplicationVersion();
        var version2 = _sut.GetApplicationVersion();

        // Assert
        Assert.Equal(version1, version2);
    }

    [Fact]
    public void GetVersionString_ConsistentBetweenCalls()
    {
        // Arrange & Act
        var versionString1 = _sut.GetVersionString();
        var versionString2 = _sut.GetVersionString();

        // Assert
        Assert.Equal(versionString1, versionString2);
    }

    [Fact]
    public void GetVersionString_VersionStringAndApplicationVersionAreConsistent()
    {
        // Arrange & Act
        var appVersion = _sut.GetApplicationVersion();
        var versionString = _sut.GetVersionString();

        // Parse the version string to verify it matches the application version
        var parts = versionString.Split('.');
        var parsedMajor = int.Parse(parts[0]);
        var parsedMinor = int.Parse(parts[1]);
        var parsedBuild = int.Parse(parts[2]);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(appVersion.Major, parsedMajor);
            Assert.Equal(appVersion.Minor, parsedMinor);
            Assert.Equal(appVersion.Build, parsedBuild);
        });
    }
}
