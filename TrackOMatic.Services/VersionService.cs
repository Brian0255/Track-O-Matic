using System.Reflection;

namespace TrackOMatic.Services;

public class VersionService : IVersionService
{
    public Version GetApplicationVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return version ?? new(0, 0, 0, 0);
    }

    public string GetVersionString()
    {
        var version = GetApplicationVersion();
        return $"{version.Major}.{version.Minor}.{version.Build}";
    }
}
