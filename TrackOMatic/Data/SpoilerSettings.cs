namespace TrackOMatic
{
    public class SpoilerSettings
    {
        public bool PointsEnabled { get; }
        public bool VialsEnabled { get; }
        public  bool WOTHEnabled { get; }
        public SpoilerSettings(bool pointsEnabled = false, bool vialsEnabled = false, bool wOTHEnabled = false )
        {
            PointsEnabled = pointsEnabled;
            VialsEnabled = vialsEnabled;
            WOTHEnabled = wOTHEnabled;
        }
        public bool Empty()
        {
            return !PointsEnabled && !VialsEnabled && !WOTHEnabled;
        }
    }
}
