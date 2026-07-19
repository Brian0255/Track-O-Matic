namespace TrackOMatic.Services;

public interface IVersionService
{
    public Version GetApplicationVersion();

    public string GetVersionString();
}
