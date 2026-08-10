namespace TrackOMatic.Logic.Models;

public class SpoilerSettings(bool pointsEnabled = false, bool vialsEnabled = false, bool wOTHEnabled = false)
{
    public bool PointsEnabled { get; } = pointsEnabled;
    public bool VialsEnabled { get; } = vialsEnabled;
    public bool WOTHEnabled { get; } = wOTHEnabled;

    public bool Empty()
    {
        return !PointsEnabled && !VialsEnabled && !WOTHEnabled;
    }
}
