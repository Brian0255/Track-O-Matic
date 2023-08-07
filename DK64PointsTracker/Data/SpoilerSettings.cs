namespace DK64PointsTracker
{
    public class SpoilerSettings
    {
        public bool PointsEnabled { get; }
        public bool VialsEnabled { get; }
        public  bool WOTHEnabled { get; }
        public SpoilerSettings(bool pointsEnabled, bool vialsEnabled, bool wOTHEnabled)
        {
            PointsEnabled = pointsEnabled;
            VialsEnabled = vialsEnabled;
            WOTHEnabled = wOTHEnabled;
        }
    }
}
