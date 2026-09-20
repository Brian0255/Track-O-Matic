namespace TrackOMatic.Services.Test;

/// <summary>
/// Helper class for test context information and paths.
/// </summary>
internal static class TestContext
{
    /// <summary>
    /// Gets the project root path relative to the test assembly output directory.
    /// Test data files are copied to the output directory, so we use AppContext.BaseDirectory.
    /// </summary>
    public static string GetProjectRootPath() => AppContext.BaseDirectory;

    /// <summary>
    /// Gets the full path to a test data file by relative path from the output directory.
    /// </summary>
    /// <param name="relativePath">The relative path to the test data file (e.g., "TestData/file.json")</param>
    /// <returns>The full path to the test data file.</returns>
    public static string GetTestDataPath(string relativePath)
    {
        var fullPath = Path.Combine(GetProjectRootPath(), relativePath);
        return Path.GetFullPath(fullPath);
    }
}
